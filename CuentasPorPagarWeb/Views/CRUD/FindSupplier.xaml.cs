using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parse;

namespace CuentasPorPagar.Views.CRUD
{
    /// <summary>
    ///     Interaction logic for FindSupplier.xaml
    /// </summary>
    public partial class FindSupplier : Window
    {
        public string supplier;

        public FindSupplier()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = new ParseQuery<Models.Supplier>();
                var result = await query.FindAsync();
                
                var list = from p in result
                    select new
                    {
                        Id = p.ObjectId,
                        Nombre = p.Name,
                        Identificacion = p.Identification
                    };

                supplierDvg.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            returnData();
        }

        private void selectBtn_Click(object sender, RoutedEventArgs e)
        {
            returnData();
        }

        public void returnData()
        {
            DialogResult = true;
            var aux = supplierDvg.SelectedItem.ToString();
            var from = aux.IndexOf("Nombre = ") + "Nombre = ".Length;
            var aux1 = aux.Substring(from);
            var to = aux1.LastIndexOf(",");
            supplier = aux1.Substring(0, to);
            Close();
        }

        private async void searchTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var query = new ParseQuery<Models.Supplier>().WhereContains("name", searchTxt.Text);
                var result = await query.FindAsync();
                var list = from p in result
                    select new
                    {
                        Id = p.ObjectId,
                        Nombre = p.Name,
                        Identificacion = p.Identification
                    };

                supplierDvg.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}