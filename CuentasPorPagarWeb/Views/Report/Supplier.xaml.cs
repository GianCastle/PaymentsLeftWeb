using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Parse;
using DataGrid = System.Windows.Controls.DataGrid;
using DataGridCell = System.Windows.Controls.DataGridCell;
using MessageBox = System.Windows.MessageBox;

namespace CuentasPorPagar.Views.Report
{
    /// <summary>
    /// Interaction logic for Supplier.xaml
    /// </summary>
    public partial class Supplier : Window
    {
        public Supplier()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
           Utilities.ExportToPdf(this.SupplierDgv, "Reporte de suplidores", "Reporte de suplidores", "");
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = await new ParseQuery<Models.Supplier>().FindAsync();
                var result = query.Select(o => new
                {
                    Id = o.ObjectId,
                    Nombre = o.Name,
                    Identificacion = o.Identification,
                    Balance = Utilities.ToDopCurrencyFormat(o.Balance),
                    Estado = o.State,
                    Creado = o.CreatedAt,
                    
                });
              
                SupplierDgv.ItemsSource = result;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al popular \n{0}", ex.ToString());
            }
        }

        private void ExitBTN_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
