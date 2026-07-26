using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeXErpSystem.DAL.Entites.Enums
{
    public enum PaymentMethod
    {
        [Display(Name = "نقدي")]
        Cash = 1,
        [Display(Name = "آجل (ذمم)")]
        Credit,
        [Display(Name = "تحويل بنكي")]
        BankTransfer,
        [Display(Name = "بطاقة ائتمان")]
        CreaditCard,
        [Display(Name = "شيك")]
        Check,
        [Display(Name = "خصم من رصيد العميل / المورد")]
        BalanceDeduction = 6
    }
}
