using Parse;

namespace CuentasPorPagar.Models
{
    [ParseClassName("DocumentEntry")]
    internal class DocumentEntry : ParseObject
    {
        [ParseFieldName("objectId")]
        public string DocumentNumber
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("receiptNum")]
        public int ReceiptNumber
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("concept")]
        public string Concept
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("createdAt")]
        public string DocumentDate
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("amount")]
        public int Amount
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("supplier")]
        public string Supplier
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("status")]
        public string Status
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
        [ParseFieldName("total_payment")]
        public int TotalAmount
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }
    }
}