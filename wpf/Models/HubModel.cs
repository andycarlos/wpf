using System;
using System.Collections.Generic;
using System.Text;

namespace wpf.Models
{
    public class HubModel
    {
    }

    public class IQbModel
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
    public class IQbCustomer : IQbModel
    {
        public decimal? CreditLimit { get; set; }
        public string Email { get; set; }
        public string SaleRepListID { get; set; }
        public List<ShipToAddress>? ShipToAddress { get; set; }
    }
    public class ShipToAddress
    {

        private string nameField;

        private string addr1Field;

        private string addr2Field;

        private string addr3Field;

        private string addr4Field;

        private string addr5Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string noteField;

        private string defaultShipToField;

        /// <remarks/>
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string Addr1
        {
            get
            {
                return this.addr1Field;
            }
            set
            {
                this.addr1Field = value;
            }
        }

        /// <remarks/>
        public string Addr2
        {
            get
            {
                return this.addr2Field;
            }
            set
            {
                this.addr2Field = value;
            }
        }

        /// <remarks/>
        public string Addr3
        {
            get
            {
                return this.addr3Field;
            }
            set
            {
                this.addr3Field = value;
            }
        }

        /// <remarks/>
        public string Addr4
        {
            get
            {
                return this.addr4Field;
            }
            set
            {
                this.addr4Field = value;
            }
        }

        /// <remarks/>
        public string Addr5
        {
            get
            {
                return this.addr5Field;
            }
            set
            {
                this.addr5Field = value;
            }
        }

        /// <remarks/>
        public string City
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string State
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        public string PostalCode
        {
            get
            {
                return this.postalCodeField;
            }
            set
            {
                this.postalCodeField = value;
            }
        }

        /// <remarks/>
        public string Country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public string Note
        {
            get
            {
                return this.noteField;
            }
            set
            {
                this.noteField = value;
            }
        }

        /// <remarks/>
        public string DefaultShipTo
        {
            get
            {
                return this.defaultShipToField;
            }
            set
            {
                this.defaultShipToField = value;
            }
        }
    }

    public class IQbVendors
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class IQbEmployee
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class IQbOtherName
    {
        public string ListID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
    public class IQbItem : IQbModel
    {
        public string SalesDesc { get; set; }
        public double SalesPrice { get; set; }
        public string Type { get; set; }
    }
    public class IQbInvoce
    {
        public string TxnID { get; set; }
        public DateTime TxnDate { get; set; }//DATE
        public DateTime DueDate { get; set; }//DUE DATE
        public string RefNumber { get; set; }//NUM
        public bool IsPaid { get; set; }
        public decimal Subtotal { get; set; }//AMOUNT
        public decimal BalanceRemaining { get; set; }//OPEN BALANCE
        public List<IQbInvoiceLineItem> Items { get; set; }
    }
    public class IQbInvoceFilter
    {
        public string CustomerID { get; set; }
        public string PaidStatus { get; set; }
        public Boolean Overdue { get; set; }
        public Boolean includeLineItems { get; set; }
    }
    public class IQbInvoiceLineItem
    {
        public string ListID { get; set; }
        public double Quantity { get; set; }
        public double Rate { get; set; }
        public double Amount { get; set; }
    }
    public class IQbSaleOrder
    {
        public string CustomerRefListID { get; set; }
        public List<IQbSalesOrderLineAdd> SalesOrderLineAdd { get; set; }
    }
    public class IQbSalesOrderLineAdd
    {
        public string ItemRefListID { get; set; }
        public double Quantity { get; set; }
        public decimal Amount { get; set; }
    }
    public class IQbSalesRep
    {
        public string SaleRepListID { get; set; }
        public string UserListID { get; set; }
    }
}
