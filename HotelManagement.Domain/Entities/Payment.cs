


namespace HotelManagement.Domain.Entities
{
    public class Payment
    {

        public int PaymentID { get; set; }
        public int InvoiceID { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int PaymentMethodID { get; set; }
        public string? TransactionReference { get; set; }
        public int? RecordedByEmployeeID { get; set; }

        public Employee employee { get; set; } // Navigation property to Employee entity

        public Invoice invoice { get; set; }
       
        public PaymentMethod paymentMethod { get; set; } // Navigation property to PaymentMethod entity


        // Constructor for creating a new payment
        public Payment(  int invoiceID, decimal amount, DateTime? paymentDate, int paymentMethodID, string? transactionReference, int? recordedByEmployeeID)
        {
           
            if (invoiceID <= 0)
            {
                throw new ArgumentException("InvoiceID must be greater than zero.", nameof(invoiceID));
            }
            
            if(amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero.", nameof(amount));
            }

            if(paymentMethodID <= 0)
            {
                throw new ArgumentException("PaymentMethodID must be greater than zero.", nameof(paymentMethodID));
            }

            if (recordedByEmployeeID.HasValue&&recordedByEmployeeID <= 0)
            {
                throw new ArgumentException("RecordedByEmployeeID must be greater than zero.", nameof(recordedByEmployeeID));
            }

           
            InvoiceID = invoiceID;
            Amount = amount;
            PaymentDate = paymentDate;
            PaymentMethodID = paymentMethodID;
            TransactionReference = string.IsNullOrEmpty(transactionReference)?null:transactionReference.Trim();
            RecordedByEmployeeID =recordedByEmployeeID;

           
        }


        // Constructor for retrieving payment details
        public Payment(int paymentID, int invoiceID, decimal amount, DateTime? paymentDate, int paymentMethodID, string? transactionReference, int? recordedByEmployeeID)
        {
            PaymentID = paymentID;
            InvoiceID = invoiceID;
            Amount = amount;
            PaymentDate = paymentDate;
            PaymentMethodID = paymentMethodID;
            TransactionReference = string.IsNullOrEmpty(transactionReference) ? null : transactionReference.Trim();
            RecordedByEmployeeID = recordedByEmployeeID;
        }

        private Payment()
        {
        }
    }
}
