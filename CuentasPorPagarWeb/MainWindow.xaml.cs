using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CuentasPorPagar.Models;
using CuentasPorPagar.Views;
using CuentasPorPagar.Views.CRUD;
using CuentasPorPagar.Views.CRUD.DocumentsEntry;
using CuentasPorPagar.Views.Query;
using Parse;
using Supplier = CuentasPorPagar.Views.CRUD.Supplier;
using SupplierReport = CuentasPorPagar.Views.Report.Supplier;

namespace CuentasPorPagar
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private void SupplierCrudItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new Supplier();
            window.Show();
        }

        private void PaymentCrudItemm_Click(object sender, RoutedEventArgs e)
        {
            var window = new Payments();
            window.Show();
        }



        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var window = new DocumentsEntry();
            window.Show();
        }

        private void UserCrudItem_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new User();
            window.Show();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var window = new SupplierReport();
            window.Show();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var loggedUser = ParseUser.CurrentUser.Username;
                var query = new ParseQuery<Users>().Where(a => a.Username.Equals(loggedUser));
                var tableResult = query.FirstAsync().Result.Permission;
                txtUserPermission.Content = $"Tipo: {tableResult}";
                Application.Current.Properties["IsAdmin"] = tableResult == "Administrador";


                PopulateWindow();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async void PopulateWindow()
        {
            try
            {

                var pendent = new ParseQuery<DocumentEntry>().Where(o => o.Status.Equals("pendiente")).OrderBy("receiptNum");
                var result = await pendent.FindAsync();
                var list = result.Select(p => new
                {
                    Id = p.ObjectId,
                    Suplidor = p.Supplier,
                    Concepto = p.Concept,
                    Factura = p.ReceiptNumber,
                    Total = Utilities.ToDopCurrencyFormat(p.TotalAmount),
                    Restante = Utilities.ToDopCurrencyFormat(p.Amount),
                    Creado = p.CreatedAt,
                    Ultimo = p.UpdatedAt
                });


                var total = result.Sum(v => v.Amount);

                TotalLbl.Content = Utilities.ToDopCurrencyFormat(total);
                dataGrid.ItemsSource = list;
                dataGrid.Columns[7].Header = "Última abonación";
            }
            catch (Exception)
            {
                dataGrid.Visibility = 0;
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var supplierQuery = new Views.Query.Supplier();
            supplierQuery.Show();
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private async void RowDoubleClick_Event(object sender, MouseButtonEventArgs e)
        {

            var query = await new ParseQuery<DocumentEntry>().FindAsync();
            var list = query.Select(p => new { Id = p.ObjectId, p.Amount, p.Concept, p.Supplier, p.ReceiptNumber }).ToList();

            var window = new Checkout
            {
                ID = list[dataGrid.SelectedIndex].Id,
                DocumentNumber = { Text = list[dataGrid.SelectedIndex].ReceiptNumber.ToString() },
                SupplierNameTxt = { Text = list[dataGrid.SelectedIndex].Supplier },
                ConceptTxt = { Text = list[dataGrid.SelectedIndex].Concept },
                AmounTxt = { Text = list[dataGrid.SelectedIndex].Amount.ToString() },
                CurrentAmount = list[dataGrid.SelectedIndex].Amount
            };

            if (!window.ShowDialog().Equals(true))
                PopulateWindow();

        }

        private void Reloadbutton_OnClick(object sender, RoutedEventArgs e)
        {
            PopulateWindow();
        }

        private void ConsultMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var window = new AdvanceDocument();
            window.Show();
        }

        private void ExportToPdfButton_OnClick(object sender, RoutedEventArgs e)
        {
            var str = TotalLbl.Content.ToString();
            Utilities.ExportToPdf(this.dataGrid, "Documentos Pendientes", "Documentos Pendientes", str);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reportes de Documentos no disponibles actualmente");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Reportes de Cierre no disponibles actualmente");
        }
    }
}