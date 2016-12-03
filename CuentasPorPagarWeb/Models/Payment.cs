using Parse;

namespace CuentasPorPagar.Models
{
    [ParseClassName("Payment")]
    internal class Payment : ParseObject
    {
        [ParseFieldName("objectId")]
        public string Id
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("concept")]
        public string Concept
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

        [ParseFieldName("state")]
        public string State
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

       


    }
}