using CodeXErpSystem.BLL.Services.Interfaces;
using CodeXErpSystem.DAL.Entites;
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

                return Ok(new { success = true, data = invoice });
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
            if (incrementalAmount <= 0.001m) return;

            var allPayments = await _unitOfWork.GetRepository<Payment>().GetAll(false);
            var maxNumber = allPayments.Any() ? allPayments.Max(p => int.TryParse(p.ReceiptNumber, out int n) ? n : 0) : 1000;
            string nextReceiptNum = (maxNumber + 1).ToString();

            var newReceipt = new Payment
            {
                ReceiptNumber = nextReceiptNum,
                Date = DateTime.Now,
                Amount = incrementalAmount,
                PaymentMethod = CodeXErpSystem.DAL.Entites.Enums.PaymentMethod.Cash,
                Reference = $"سداد من المتبقي - فاتورة رقم {invoice.InvoiceNumber}",
                InvoiceId = invoice.Id,
                CustomerId = invoice.CustomerId,
                SupplierId = invoice.SupplierId,
                CreatedBy = "Admin"
            };
            _unitOfWork.GetRepository<Payment>().Add(newReceipt);

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
                    supplier.Balance = (supplier.Balance ?? 0) + incrementalAmount;
                    _unitOfWork.GetRepository<Supplier>().Update(supplier);
                }
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

