using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Parse;
using System.Net.Http;
using JsonRequest;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace CuentasPorPagar.Views
{
    /// <summary>
    /// Interaction logic for Checkout.xaml
    /// </summary>
    public partial class Checkout : Window
    {

        public Checkout()
        {
            InitializeComponent();
            
        }

        public int CurrentAmount { get; set; }
        public string ID  { get; set; }
    

    private void AmounTxt_TextChanged(object sender, TextChangedEventArgs e)
        {

            var amount = (TextBox) sender;
            try
            {
                if (Regex.IsMatch(amount.Text, "\\D"))
                {
                    var index = amount.SelectionStart - 1;
                    amount.Text = amount.Text.Remove(index, 1);

                    amount.SelectionStart = index;
                    amount.SelectionLength = 0;
                }
            }
            catch (Exception)
            {
            }

        }

        private  async void PaymentButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Guardo el modelo de pago
                var amount = CurrentAmount - int.Parse(AmounTxt.Text);
                var abonated = int.Parse(AmounTxt.Text);
                var payment = new Models.Payment
                {
                    Supplier = SupplierNameTxt.Text,
                    Concept = ConceptTxt.Text,
                    Amount = int.Parse(AmounTxt.Text), 
                    State = amount.Equals(0) ? "Completado" : "Abonado"
                };
                
                await payment.SaveAsync();
                
                //Actualizar el documento, si el balance abonado es el total a pagar, entonces el balance será 0, por ende pasa de pendiente a pagado
                var document = await new ParseQuery<Models.DocumentEntry>()
                    .Where(p => p.ObjectId.Equals(ID)).FirstAsync();

                document.Amount = amount;
                document.Status = (amount.Equals(0)) ? "pagado" : "pendiente";
                
                await document.SaveAsync();

                //Actualizar el balance total  de cada suplidor. El balance total es la suma de todos los documentos pendientes
                //Retira una lista de balance de los documentos donde exista un suplidor N
                var sumOfDocuments =
                    await new ParseQuery<Models.DocumentEntry>()
                    .Where(d => d.Supplier.Equals(payment.Supplier)).FindAsync();

                var listOfDocumentBalance = sumOfDocuments.Sum(sum => sum.Amount);
                var updateSupplier = await new ParseQuery<Models.Supplier>()
                                    .Where(o => o.Name.Equals(payment.Supplier)).FirstAsync(); //Como no existe dos veces un suplidor. Busco el objeto por el nombre

                updateSupplier.Balance -= int.Parse(AmounTxt.Text); //Actualizo su balance
                await updateSupplier.SaveAsync();
                
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://contabilidadservice.azurewebsites.net/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var accountEntry = new { entradasContables = new { descripcion = ConceptTxt.Text, auxiliar = 1, cuenta = 8, tipoMovimiento = "CR", monto = AmounTxt.Text, moneda = 1 } };
                    var json = new JavaScriptSerializer().Serialize(accountEntry);
                    var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await client.PostAsync("ContabilidadWS/webresources/ContabilidadWS/registrarAsientos", stringContent);
                    MessageBox.Show(response.ToString());
                }
                this.Close();
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
           
        }

   
    }
}
