using Parse;

namespace CuentasPorPagar.Models
{
    [ParseClassName("Users")]
    internal class Users : ParseObject
    {
        [ParseFieldName("username")]
        public string Username
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("Name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("password")]
        public string Password
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("Permission")]
        public string Permission
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        [ParseFieldName("email")]
        public string Email
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }
    }
}