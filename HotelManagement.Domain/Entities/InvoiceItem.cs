using HotelManagement.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelManagement.Domain.Entities
{
    public class InvoiceItem
    {

        public int InvoiceItemID { get;private set; }
        public int InvoiceID { get; private set; }
        public string ItemDescription { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public InvoiceItemTypeEnum ItemType { get; private set; }
       
        public decimal LineTotal { get;  set; }
        public Invoice? Invoice { get; set; }

        //For creating a new invoice item
        public InvoiceItem(int invoiceID, string itemDescription, int quantity, decimal unitPrice, decimal? lineTotal, InvoiceItemTypeEnum itemType)
        {

            if(invoiceID <= 0) throw new ArgumentException("Invoice ID must be greater than zero", nameof(invoiceID));
            if(quantity < 0) throw new ArgumentException("Quantity can not be negative", nameof(quantity));
            if(unitPrice < 0) throw new ArgumentException("Unit price can not be negative", nameof(unitPrice));


            InvoiceID = invoiceID;
            ItemDescription = itemDescription.Trim()?? throw new ArgumentNullException(nameof(itemDescription));
            Quantity = quantity;
            UnitPrice = unitPrice;
            ItemType = itemType;
        }

        //For retrieving an existing invoice item   
        public InvoiceItem(int invoiceItemID, int invoiceID, string itemDescription, int quantity, decimal unitPrice,decimal lineTotal, InvoiceItemTypeEnum itemType)
        {
            InvoiceItemID = invoiceItemID;
            InvoiceID = invoiceID;
            ItemDescription = itemDescription;
            Quantity = quantity;
            UnitPrice = unitPrice;
            LineTotal = lineTotal;
            ItemType = itemType;
        }

        private InvoiceItem()
        {
        }


        public void ChangeQuantity(int newQuantity)
        {

            if(newQuantity < 0) throw new ArgumentException("Quentity can not be negative",nameof(newQuantity));

            Quantity = newQuantity;
        }

        public void ChangeUnitPrice(decimal newPrice)
        {
            if (newPrice <= 0) throw new ArgumentException("New price can not be negative or Zero.", nameof(newPrice));

            UnitPrice = newPrice;
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

        public decimal CalculateLineTotal()
        {
            if (Quantity < 0) throw new ArgumentException("Quantity can not be negative", nameof(Quantity));
            if (UnitPrice < 0) throw new ArgumentException("Unit price can not be negative", nameof(UnitPrice));
         this.LineTotal = Quantity * UnitPrice;
            return LineTotal;
        }

    }
}
