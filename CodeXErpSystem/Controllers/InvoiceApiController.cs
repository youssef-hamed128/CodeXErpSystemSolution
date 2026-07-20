using CodeXErpSystem.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace CodeXErpSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceApiController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceApiController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
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
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء جلب الفاتورة", details = ex.Message });
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            if (request == null || request.InvoiceId <= 0)
                return BadRequest(new { success = false, message = "بيانات غير صالحة" });

            try
            {
                var result = await _invoiceService.UpdateInvoiceStatusAsync(request.InvoiceId, request.NewStatus);
                if (result)
                    return Ok(new { success = true, message = "تم تحديث الحالة بنجاح" });
                else
                    return NotFound(new { success = false, message = "لم يتم العثور على الفاتورة أو حدث خطأ أثناء التحديث" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "حدث خطأ أثناء تحديث الحالة", details = ex.Message });
            }
        }
    }

    public class UpdateStatusRequest
    {
        public int InvoiceId { get; set; }
        public CodeXErpSystem.DAL.Entites.Enums.InvoiceStatus NewStatus { get; set; }
    }
}
