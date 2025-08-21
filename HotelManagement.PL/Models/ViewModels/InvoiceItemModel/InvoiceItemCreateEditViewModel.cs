using HotelManagement.Domain.Enums;
using HotelManagement.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.InvoiceItem
{
    public class InvoiceItemCreateEditViewModel
    {
      
        [Display(Name = "Invoice Item ID")]
        public int? InvoiceItemID { get; set; }

        [Required(ErrorMessage = "Invoice ID is required.")]
        [Display(Name = "Invoice ID")]
        public int InvoiceID { get; set; }

        [Display(Name = "Item Description")]
        [Required(ErrorMessage = "Item description is required.")]
        [StringLength(255, ErrorMessage = "Item description cannot exceed 255 characters.")]
        public string ItemDescription { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Unit price is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Unit price must be greater than zero.")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Item Type")]
        [Required(ErrorMessage = "Item type is required.")]
        public InvoiceItemTypeEnum ItemType { get; set; }

        // We use a nullable decimal for the view model.
        // The LineTotal will be calculated in the service layer or database.
        public decimal? LineTotal { get; set; }

        // This is a navigation property to hold the dropdown list data.
        // It's a great approach to provide options for the foreign key.
        public IEnumerable<SelectListItem>? InvoiceList { get; set; }

        // Optional: A reference to the parent entity for display purposes
        public Invoice? Invoice { get; set; }
    }
}
