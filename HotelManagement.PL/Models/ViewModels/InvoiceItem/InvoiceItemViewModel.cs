using HotelManagement.Domain.Entities;
using HotelManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.InvoiceItem
{
    public class InvoiceItemViewModel
    {
        [Display(Name = "Invoice Item ID")]
        public int InvoiceItemID { get;  set; }


        [Display(Name = "Invoice ID")]
        public int InvoiceID { get;  set; }


        [Display(Name = "Item Description")]
        public string ItemDescription { get;  set; }


        [Display(Name = "Quantity")]
        public int Quantity { get;  set; }


        [Display(Name = "Unit Price")]
        [DataType(DataType.Currency)]
        public decimal UnitPrice { get;  set; }


        [Display(Name = "Item Type")]
        [DataType(DataType.Text)]
        public InvoiceItemTypeEnum ItemType { get;  set; }


        [Display(Name = "Line Total")]
        [DataType(DataType.Currency)]
        public decimal? LineTotal { get; set; }


        [Display(Name = "Invoice")]

        public IEnumerable<SelectListItem>? InvoiceList { get; set; }

        public Invoice? Invoice { get; set; }

    }
}
