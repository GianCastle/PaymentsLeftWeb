using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CuentasPorPagar.Models;
using Parse;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using JsonRequest;

namespace CuentasPorPagar.Views.CRUD.DocumentsEntry
{
    /// <summary>
    ///     Interaction logic for DocumentsEntry.xaml
    /// </summary>
    public partial class DocumentsEntry : Window
    {
        public DocumentsEntry()
        {
            InitializeComponent();
        }
        private async void CreateDocumentBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(amountTxt.Text).Equals(false) &&
                    string.IsNullOrEmpty(conceptTxt.Text).Equals(false) &&
                    string.IsNullOrEmpty(supplierTxt.Text).Equals(false))
                {
                    Crud("save");
                    var query = await new ParseQuery<Models.Supplier>()
                            .Where(o => o.Name.Equals(supplierTxt.Text)).FirstAsync();

                    if (query.State.Equals("Inactivo"))
                        query.State =  "Activo";

                    await query.SaveAsync();      
                }
               
                else
                    MessageBox.Show("Faltan campos por llenar");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
           
        }

        private async void PopulateGrid()
        {
            try
            {
                var query = await new ParseQuery<DocumentEntry>().FindAsync();
                
                DocumentDgv.ItemsSource = query.Select(p => new
                {
                    Id = p.ObjectId,
                    Recibo = p.ReceiptNumber,
                    Concepto = p.Concept,
                    Total = p.TotalAmount,
                    Monto = Utilities.ToDopCurrencyFormat(p.Amount),
                    Suplidor = p.Supplier,
                    Estado = p.Status,
                    Fecha = p.CreatedAt
                }); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private async void Crud(string option)
        {
            try
            {
                var id = DocumentDgv.SelectedIndex;
                var query = await new ParseQuery<DocumentEntry>().FindAsync();
                var list = query.Select(p => new { Id = p.ObjectId, p.Supplier });
                
                switch (option.ToLower())
                {
                    case "save":
                        if (conceptTxt.Text != "" && (int.Parse(amountTxt.Text)) > 0 ||
                            supplierTxt.Text != "" && (int.Parse(numberTxt.Text)) > 0)
                        {
                            try
                            {
                                DocumentEntry document;
                                if (!(objectIdTxt.Text.Length > 2))
                                {
                                    document = new DocumentEntry
                                    {
                                        Concept = conceptTxt.Text,
                                        Amount = int.Parse(amountTxt.Text),
                                        TotalAmount = int.Parse(amountTxt.Text),
                                        Supplier = supplierTxt.Text,
                                        ReceiptNumber = int.Parse(numberTxt.Text),
                                        Status = "pendiente"
                                    };
                                    
                                    var updateSupplierAmount = await new ParseQuery<Models.Supplier>()
                                        .Where(s => s.Name.Equals(supplierTxt.Text)).FirstAsync();
                                        updateSupplierAmount.Balance += int.Parse(amountTxt.Text);

                                    await updateSupplierAmount.SaveAsync();
                                    await document.SaveAsync();
                                    MessageBox.Show("Documento creado");
                                }
                                else
                                {
                                    document = new DocumentEntry
                                    {
                                        ObjectId = objectIdTxt.Text,
                                        Concept = conceptTxt.Text,
                                        TotalAmount = int.Parse(amountTxt.Text),
                                        Supplier = supplierTxt.Text,
                                        ReceiptNumber = int.Parse(numberTxt.Text),
                                        Status = (amountTxt.Text.Equals("0")) ? "pagado" : "pendiente"
                                    };
                                    await document.SaveAsync();
                                    MessageBox.Show("Documento actualizado");
                                    PopulateGrid();
                                }
                                PopulateGrid();
                                Utilities.Clear(this);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }

                        break;

                    case "delete":
                       if (id >= 0)
                        {
                            try
                            {
                                await new ParseQuery<DocumentEntry>().Where(o => o.ObjectId.Equals(list.ElementAt(id).Id))
                                .FirstAsync().Result.DeleteAsync();
                                
                                try
                                {
                                    var updateState = await new ParseQuery<Models.Supplier>()
                                                 .Where(o => o.Name.Equals(list.ElementAt(id).Supplier)).FirstAsync();

                                    if (updateState.State.Equals("Activo"))
                                    {
                                        updateState.State = "Inactivo";
                                        updateState.Balance = 0;
                                        await updateState.SaveAsync();
                                    }
                                }
                                catch (ParseException ex)
                                {
                                    MessageBox.Show($"No se pudo actualizar el estado en el proovedor. \n{ex}", "UPS!", MessageBoxButton.OK);
                                }
                               
                                PopulateGrid();
                            }
                            catch (ParseException ex)
                            {
                                MessageBox.Show($"Error eliminando documento\n{ex}", "UPS!", MessageBoxButton.OK);
                            }
                        }
                        else
                            MessageBox.Show("Primero se debe seleccionar un elemento de la lista.", "HEY!", MessageBoxButton.OK);

                        break;

                    default:
                        MessageBox.Show("No se pasaron parametros");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error desconocido.\n{ex}", "UPS!", MessageBoxButton.OK);
            }  
        }

        private async void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var query = await new ParseQuery<DocumentEntry>().FindAsync();
            var list = query.Select(p => new {Id = p.ObjectId});
            
            var element = list.ElementAt(DocumentDgv.SelectedIndex).Id;
            var editQuery = new ParseQuery<DocumentEntry>().Where(aux => aux.ObjectId.Equals(element));
            
            var editElements = await editQuery.FirstAsync();
            conceptTxt.Text = editElements.Concept;
            amountTxt.Text = editElements.Amount.ToString();
            numberTxt.Text = editElements.ReceiptNumber.ToString();
            supplierTxt.Text = editElements.Supplier;
            objectIdTxt.Text = editElements.ObjectId;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bool.Parse(Application.Current.Properties["IsAdmin"].ToString()))
                DeleteDocumentBtn.IsEnabled = true;
            PopulateGrid();
            
        }

        private void loadSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            var findSupplier = new FindSupplier();
            findSupplier.ShowDialog();
            supplierTxt.Text = findSupplier.supplier;
        }
        
        private async void getNumberBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var query = await new ParseQuery<DocumentEntry>().OrderByDescending("receiptNum").FindAsync();
                var list = query.Select(p => new { p.ReceiptNumber });
                numberTxt.Text = (list.ElementAt(0).ReceiptNumber + 1).ToString();
            }
            catch (Exception)
            {
               numberTxt.Text = "1";
            }
            
        }

        private void amountTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var balance = (TextBox)sender;

            try
            {
                if (Regex.IsMatch(balance.Text, "\\D"))
                {
                    var index = balance.SelectionStart - 1;
                    balance.Text = balance.Text.Remove(index, 1);

                    balance.SelectionStart = index;
                    balance.SelectionLength = 0;
                }
            }
            catch (Exception){ }
        }

        private void DeleteDocumentBtn_Click(object sender, RoutedEventArgs e) => Crud("delete");
        private void ExitDocumentBtn_Click(object sender, RoutedEventArgs e) => Close();
        private void clearBtn_Click(object sender, RoutedEventArgs e) => Utilities.Clear(this);
    }
}