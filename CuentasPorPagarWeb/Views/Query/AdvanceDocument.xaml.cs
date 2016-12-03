using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using CuentasPorPagar.Models;

namespace CuentasPorPagar.Views.Query
{
    /// <summary>
    /// Interaction logic for AdvanceDocument.xaml
    /// </summary>
    public partial class AdvanceDocument : Window
    {
        public AdvanceDocument()
        {
            InitializeComponent();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            Utilities.ExportToPdf(this.DocumentDataGrid, "Reporte de documentos " + DateTime.Now.ToString().Replace(":","_").Replace("/","_").Replace(" ","-"), "Reporte de documentos ", "");
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? dateFrom = this.dateFrom.SelectedDate;
            DateTime? dateTo = this.dateTo.SelectedDate;

            var state = ((ComboBoxItem)StateComboBox.SelectedItem).Content.ToString();

            var query = new ParseQuery<Models.DocumentEntry>();
            if (!string.IsNullOrEmpty(ConceptTextBox.Text)) 
                query = query.WhereMatches("concept", ConceptTextBox.Text);

            if (!string.IsNullOrEmpty(SupplierTextBox.Text))
                query = query.WhereEqualTo("supplier", SupplierTextBox.Text);

            if (!string.IsNullOrEmpty(BalanceTextBox.Text))
                query = query.WhereGreaterThanOrEqualTo("total_payment", int.Parse(BalanceTextBox.Text));

            if (StateComboBox.SelectedIndex > 0)
                query = query.WhereEqualTo("status", state);

            if (dateFrom != null)
                query = query.WhereGreaterThanOrEqualTo("createdAt", dateFrom);

            if (dateTo != null)
                query = query.WhereLessThanOrEqualTo("createdAt", dateTo);

            var result = await query.FindAsync();
           

            DocumentDataGrid.ItemsSource = result.Select(p => new
            {
                Id = p.ObjectId,
                Recibo = p.ReceiptNumber,
                Concepto = p.Concept,
                Total = Utilities.ToDopCurrencyFormat(p.TotalAmount),
                Abonado = Utilities.ToDopCurrencyFormat(p.Amount),
                Suplidor = p.Supplier,
                Estatus = p.Status,
                Fecha = p.CreatedAt
            });
        }

        private void CleanButton_OnClick(object sender, RoutedEventArgs e)
        {
            Utilities.Clear(this);
        }
        public async void PopulateWindow()
        {
            try
            {
                var query = new ParseQuery<Models.DocumentEntry>();
                var result = await query.FindAsync();
                DocumentDataGrid.ItemsSource = result.Select(p => new
                {
                    Id = p.ObjectId,
                    Recibo = p.ReceiptNumber,
                    Concepto = p.Concept,
                    Total = Utilities.ToDopCurrencyFormat(p.TotalAmount),
                    Abonado = Utilities.ToDopCurrencyFormat(p.Amount),
                    Suplidor = p.Supplier,
                    Estatus = p.Status,
                    Fecha = p.CreatedAt
                });
            }
            catch (Exception)
            {
                DocumentDataGrid.Visibility = 0;
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            PopulateWindow();
        }

        private void exitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
