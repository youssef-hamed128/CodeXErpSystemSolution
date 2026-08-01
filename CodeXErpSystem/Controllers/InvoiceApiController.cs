using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.DAL.Entites;
using CodeXErpSystem.DAL.Entites.Enums;
using CodeXErpSystem.DAL.Repository.Inetrfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "مدير النظام, مبيعات, مشتريات ومخازن")]
    public class InvoiceApiController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceApiController(IInvoiceService invoiceService, IUnitOfWork unitOfWork)
        {
            _invoiceService = invoiceService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("GetInvoiceByNumber")]
        public async Task<IActionResult> GetInvoiceByNumber(string invoiceNumber)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                return BadRequest(new { success = false, message = "رقم الفاتورة مطلوب" });

            try
            {
                var invoice = await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
                if (invoice == null)
                    return NotFound(new { success = false, message = "الفاتورة غير موجودة" });

                var returnInvoices = await _unitOfWork.GetRepository<Invoice>().FindAsync(
                    i => i.ReferenceNumber == invoiceNumber && (i.Type == InvoiceType.SalesReturn || i.Type == InvoiceType.PurchaseReturn),
                    includeProperties: "Items");

                var returnedQtyMap = returnInvoices
                    .Where(r => r.Items != null)
                    .SelectMany(r => r.Items)
                    .GroupBy(item => item.ProductId)
                    .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

                var itemsWithRemaining = invoice.Items.Select(item => {
                    decimal returnedQty = returnedQtyMap.ContainsKey(item.ProductId) ? returnedQtyMap[item.ProductId] : 0;
                    decimal remainingQty = Math.Max(0, item.Quantity - returnedQty);
                    return new {
                        id = item.Id,
                        productId = item.ProductId,
                        productName = item.ProductName,
                        unitMeasure = item.UnitOfMeasure,
                        quantity = item.Quantity,
                        returnedQuantity = returnedQty,
                        remainingQuantity = remainingQty,
                        unitPrice = item.UnitPrice,
                        total = item.Total
                    };
                });

                var resultData = new {
                    id = invoice.Id,
                    invoiceNumber = invoice.InvoiceNumber,
                    type = (int)invoice.Type,
                    typeDisplay = invoice.TypeDisplay,
                    customerId = invoice.CustomerId,
                    customerName = invoice.CustomerName,
                    supplierId = invoice.SupplierId,
                    supplierName = invoice.SupplierName,
                    date = invoice.Date,
                    dueDate = invoice.DueDate,
                    totalAmount = invoice.TotalAmount,
                    paidAmount = invoice.PaidAmount,
                    remainingAmount = invoice.RemainingAmount,
                    statusDisplay = invoice.StatusDisplay,
                    paymentMethodDisplay = invoice.PaymentMethodDisplay,
                    hasReturns = returnInvoices.Any(),
                    returnCount = returnInvoices.Count(),
                    items = itemsWithRemaining
                };

                return Ok(new { success = true, data = resultData });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء استرداد الفاتورة", details = ex.Message });
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || request.InvoiceId <= 0)
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });

            try
            {
                var invoice = await _unitOfWork.GetRepository<Invoice>().GetById(request.InvoiceId);
                if (invoice == null)
                    return NotFound(new { success = false, message = "الفاتورة غير موجودة" });

                decimal oldPaid = invoice.PaidAmount;
                invoice.Status = request.NewStatus;
                if (request.NewStatus == CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid)
                {
                    invoice.PaidAmount = invoice.TotalAmount;
                }
                else if (request.NewStatus == CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Unpaid)
                {
                    invoice.PaidAmount = 0;
                }
                else if (request.PaidAmount.HasValue)
                {
                    if (request.PaidAmount.Value > invoice.TotalAmount + 0.01m)
                    {
                        return BadRequest(new { success = false, message = $"لا يمكن إدخال مبلغ أكبر من إجمالي الفاتورة ({invoice.TotalAmount:N2} ج.م)" });
                    }
                    invoice.PaidAmount = request.PaidAmount.Value;
                }

                decimal incremental = invoice.PaidAmount - oldPaid;
                await CreateReceiptForPaymentAsync(invoice, incremental);

                _unitOfWork.GetRepository<Invoice>().Update(invoice);
                await _unitOfWork.CompleteAsync();
                return Ok(new { success = true, message = "تم حفظ الحالة والمبلغ المدفوع بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث الحالة", details = ex.Message });
            }
        }

        [HttpPost("RecordPayment")]
        public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentRequest request)
        {
            if (request == null || request.InvoiceId <= 0 || request.PaidAmount < 0)
                return BadRequest(new { success = false, message = "بيانات الدفعة غير صالحة" });

            try
            {
                var invoice = await _unitOfWork.GetRepository<Invoice>().GetById(request.InvoiceId);
                if (invoice == null)
                    return NotFound(new { success = false, message = "الفاتورة غير موجودة" });

                if (request.PaidAmount > invoice.TotalAmount + 0.01m)
                {
                    decimal rem = Math.Max(0, invoice.TotalAmount - invoice.PaidAmount);
                    return BadRequest(new { success = false, message = $"لا يمكن إضافة مبلغ سداد أكبر من المبلغ المتبقي على الفاتورة ({rem:N2} ج.م)" });
                }

                decimal oldPaid = invoice.PaidAmount;
                invoice.PaidAmount = request.PaidAmount;
                if (invoice.PaidAmount >= invoice.TotalAmount - 0.01m)
                    invoice.Status = CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Paid;
                else if (invoice.PaidAmount > 0.01m)
                    invoice.Status = CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Partial;
                else
                    invoice.Status = CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus.Unpaid;

                decimal incremental = invoice.PaidAmount - oldPaid;
                await CreateReceiptForPaymentAsync(invoice, incremental);

                _unitOfWork.GetRepository<Invoice>().Update(invoice);
                await _unitOfWork.CompleteAsync();
                return Ok(new { success = true, message = "تم تسجيل الدفعة وتحديث المدفوع بنجاح" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تسجيل الدفعة", details = ex.Message });
            }
        }

        private async Task CreateReceiptForPaymentAsync(Invoice invoice, decimal incrementalAmount)
        {
            if (Math.Abs(incrementalAmount) <= 0.001m) return;

            if (invoice.CustomerId.HasValue)
            {
                var customer = await _unitOfWork.GetRepository<Customer>().GetById(invoice.CustomerId.Value);
                if (customer != null)
                {
                    customer.Balance = (customer.Balance ?? 0) - incrementalAmount;
                    _unitOfWork.GetRepository<Customer>().Update(customer);
                }
            }
            else if (invoice.SupplierId.HasValue)
            {
                var supplier = await _unitOfWork.GetRepository<Supplier>().GetById(invoice.SupplierId.Value);
                if (supplier != null)
                {
                    supplier.Balance = (supplier.Balance ?? 0) - incrementalAmount;
                    _unitOfWork.GetRepository<Supplier>().Update(supplier);
                }
            }

            if (incrementalAmount > 0.01m)
            {
                var allPayments = await _unitOfWork.GetRepository<Payment>().GetAll(false);
                var maxNumber = allPayments.Any() ? allPayments.Max(p => int.TryParse(p.ReceiptNumber, out int n) ? n : 0) : 1000;
                string nextReceiptNum = (maxNumber + 1).ToString();

                var newReceipt = new Payment
                {
                    ReceiptNumber = nextReceiptNum,
                    Date = DateTime.Now,
                    Amount = incrementalAmount,
                    PaymentMethod = CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Cash,
                    Reference = $"سداد من الفاتورة رقم {invoice.InvoiceNumber}",
                    InvoiceId = invoice.Id,
                    CustomerId = invoice.CustomerId,
                    SupplierId = invoice.SupplierId,
                    CreatedBy = "Admin"
                };
                _unitOfWork.GetRepository<Payment>().Add(newReceipt);
            }
        }
    }

    public class UpdateStatusRequest
    {
        public int InvoiceId { get; set; }
        public CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus NewStatus { get; set; }
        public decimal? PaidAmount { get; set; }
    }

    public class RecordPaymentRequest
    {
        public int InvoiceId { get; set; }
        public decimal PaidAmount { get; set; }
    }
}

