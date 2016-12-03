using Parse;

namespace CuentasPorPagar.Models
{
    [ParseClassName("Supplier")]
    public class Supplier : ParseObject
    {
        [ParseFieldName("identification")]
        public string Identification
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("balance")]
        public int Balance
        {
            get { return GetProperty<int>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("objectId")]
        public string Id
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("type")]
        public string Type
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