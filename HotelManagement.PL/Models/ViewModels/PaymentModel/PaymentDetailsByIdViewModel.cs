using System.ComponentModel.DataAnnotations;

namespace HotelManagement.Web.Models.ViewModels.PaymentModel
{
    public class PaymentDetailsByIdViewModel
    {
        [Display(Name = "Payment ID")]
        public int PaymentID { get; set; }

        [Display(Name = "Invoice ID")]
        public int InvoiceID { get; set; }

        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Payment Method")]
        public string PaymentMethodName { get; set; }

        [Display(Name = "Transaction Reference")]
        public string TransactionReference { get; set; }

        [Display(Name = "Recorded by Employee ID")]
        public int RecordedByEmployeeID { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Invoice Status")]
        public string InvoiceStatus { get; set; }
    }
}
