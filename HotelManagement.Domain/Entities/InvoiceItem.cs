using HotelManagement.Domain.Enums;


namespace HotelManagement.Domain.Entities
{
    public class InvoiceItem
    {
        // Public setters are required for Entity Framework
        public int InvoiceItemID { get; set; }
        public int InvoiceID { get; set; }
        public string ItemDescription { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public InvoiceItemTypeEnum ItemType { get; set; }
        public decimal LineTotal { get; set; }

        public Invoice? Invoice { get; set; }

        // For creating a new invoice item
        // NOTE: The constructor no longer takes 'lineTotal' as an argument.
        public InvoiceItem(int invoiceID, string itemDescription, int quantity, decimal unitPrice, InvoiceItemTypeEnum itemType)
        {
            if (invoiceID <= 0) throw new ArgumentException("Invoice ID must be greater than zero", nameof(invoiceID));
            if (quantity < 0) throw new ArgumentException("Quantity can not be negative", nameof(quantity));
            if (unitPrice < 0) throw new ArgumentException("Unit price can not be negative", nameof(unitPrice));
            if (string.IsNullOrWhiteSpace(itemDescription)) throw new ArgumentNullException(nameof(itemDescription));

            InvoiceID = invoiceID;
            ItemDescription = itemDescription.Trim();
            Quantity = quantity;
            UnitPrice = unitPrice;
            ItemType = itemType;

            // The LineTotal is calculated automatically upon creation
           // CalculateLineTotal();
        }

        // For retrieving an existing invoice item
        public InvoiceItem(int invoiceItemID, int invoiceID, string itemDescription, int quantity, decimal unitPrice, decimal lineTotal, InvoiceItemTypeEnum itemType)
        {
            InvoiceItemID = invoiceItemID;
            InvoiceID = invoiceID;
            ItemDescription = itemDescription;
            Quantity = quantity;
            UnitPrice = unitPrice;
            LineTotal = lineTotal;
            ItemType = itemType;
        }

        public InvoiceItem()
        {
        }

        public void ChangeQuantity(int newQuantity)
        {
            if (newQuantity < 0) throw new ArgumentException("Quantity can not be negative", nameof(newQuantity));
            Quantity = newQuantity;
            CalculateLineTotal(); // Recalculate total after change
        }

        public void ChangeUnitPrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentException("New price can not be negative or Zero.", nameof(newPrice));
            UnitPrice = newPrice;
            CalculateLineTotal(); // Recalculate total after change
        }

        public void ChangeItemDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription)) throw new ArgumentNullException(nameof(newDescription));
            ItemDescription = newDescription.Trim();
        }

        public void ChangeItemType(InvoiceItemTypeEnum newItemType)
        {
            ItemType = newItemType;
        }

        // This method is now private because it's an internal function
        // that the class uses to maintain its own state.
        private void CalculateLineTotal()
        {
            // You can add validation here for extra safety
            if (Quantity < 0) throw new ArgumentException("Quantity can not be negative", nameof(Quantity));
            if (UnitPrice < 0) throw new ArgumentException("Unit price can not be negative", nameof(UnitPrice));
            this.LineTotal = Quantity * UnitPrice;
        }
    }
}
