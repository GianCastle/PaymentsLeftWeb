using Parse;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CuentasPorPagar.Views.Query
{

    public partial class Supplier : Window
    {
        private ParseQuery<Models.Supplier> Query = new ParseQuery<Models.Supplier>();
        public Supplier()
        {
            InitializeComponent();
        }

        private async void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            DateTime? dateFrom = date1.SelectedDate;
            DateTime? dateTo = date2.SelectedDate;
            var query = new ParseQuery<Models.Supplier>();

            var state = ((ComboBoxItem) stateCmb.SelectedItem).Content.ToString();
            var type = ((ComboBoxItem) typeCmb.SelectedItem).Content.ToString();

            try
            {
                
                if (!string.IsNullOrEmpty(nameTxt.Text))
                    query = query.WhereMatches("name", nameTxt.Text);
              
               if (string.IsNullOrEmpty(documentTxt.Text))
                    query = query.WhereContains("identification", documentTxt.Text);
                
                if (state.Equals("Activo") || state.Equals("Inactivo"))
                    query = query.WhereContains("state", state);
                
                if (type.Equals("Fisica") || type.Equals("Juridica"))
                    query = query.WhereContains("type", type);
                
                if (!string.IsNullOrEmpty(amountTxt1.Text))
                     query = query.WhereGreaterThanOrEqualTo("balance", int.Parse(amountTxt1.Text));
                
                if (!string.IsNullOrEmpty(amountTxt2.Text))
                     query = query.WhereLessThanOrEqualTo("balance", int.Parse(amountTxt2.Text));

                if (!string.IsNullOrEmpty(date1.SelectedDate.ToString()) )
                    query = query.WhereGreaterThanOrEqualTo("createdAt", dateFrom);
                
                if (!string.IsNullOrEmpty(date2.SelectedDate.ToString()) )
                    query = query.WhereLessThanOrEqualTo("createdAt", dateTo);
                

                var result = await query.FindAsync();
                dataGrid.ItemsSource = result.Select(o => new
                {
                    Id = o.ObjectId,
                    Nombre = o.Name,
                    Identificacion = o.Identification,
                    Estado = o.State,
                    Tipo = o.Type,
                    Balance = Utilities.ToDopCurrencyFormat(o.Balance),
                    Creado = o.CreatedAt

                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {  
            Populate(); 
        }
        public async void Populate()
        {
            var query = await new ParseQuery<Models.Supplier>().FindAsync();
            var list = query.Select(p => new
            {
                Id = p.ObjectId,
                Nombre = p.Name,
                Identificacion = p.Identification,
                Estado = p.State,
                Tipo = p.Type,
                Balance = Utilities.ToDopCurrencyFormat(p.Balance),
                Creado = p.CreatedAt
            });

            dataGrid.ItemsSource = list;

        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void exportBtn_Click(object sender, RoutedEventArgs e)
        {
            Utilities.ExportToPdf(this.dataGrid, "Lista exportada de Proveedores", "Lista exportada de Proveedores ", "");
        }
    }
}

