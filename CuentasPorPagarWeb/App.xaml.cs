using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using CuentasPorPagar.Models;
using Parse;

namespace CuentasPorPagarWeb
{ 
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            ParseObject.RegisterSubclass<DocumentEntry>();
            ParseObject.RegisterSubclass<Supplier>();
            ParseObject.RegisterSubclass<Payment>();
            ParseObject.RegisterSubclass<Users>();
            ParseClient.Initialize(new ParseClient.Configuration
            {
                ApplicationId = "yMVc5a3J9DSgpGdHDqB2kxKIiO72RVovr4Bxs5Iv",
                WindowsKey = "f1FpJWDQu6tBknQP5uOnr0kA4FMnUdHId1mSP3qM"
            });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            //StartupUri = new Uri("/Page1.xaml", UriKind.Relative);
            StartupUri = new Uri("/Views/Login.xaml", UriKind.Relative);
        }
    }
}
