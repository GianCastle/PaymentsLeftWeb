using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parse;

namespace CuentasPorPagar.Views.CRUD
{
    /// <summary>
    ///     Interaction logic for Supplier.xaml
    /// </summary>
    public partial class Supplier : Window
    {
        public Supplier()
        {
            InitializeComponent();
        }

        private void CreateSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            Crud("save");
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                
                PopulateGrid();

                if (bool.Parse(Application.Current.Properties["IsAdmin"].ToString()))
                    DeleteSupplierBtn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DeleteSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            Crud("Delete");
        }

        private void ExitSupplierBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            StateCbx.IsEnabled = true;
            var id = SupplierDgv.SelectedIndex;
            var query = await new ParseQuery<Models.Supplier>().FindAsync();
            var result = query.Select(o => new {Id = o.ObjectId});
            var element = result.ElementAt(id).Id;
            var editQuery = from p in new ParseQuery<Models.Supplier>()
                where p.ObjectId.Equals(element)
                select p;

            var editElements = await editQuery.FirstAsync();

            IdTxt.Text = editElements.ObjectId;
            NameTxt.Text = editElements.Name;
            IdentificationTxt.Text = editElements.Identification;
            BalanceTxt.Text = editElements.Balance.ToString();
            TypeCbx.SelectedIndex = (editElements.Type.Equals("Juridica")) ? 1 : 2;
            StateCbx.SelectedIndex = (editElements.State.Equals("Activo")) ? 2 : 1;
            
        }

        public async void Crud(string option)
        {
            var ID = SupplierDgv.SelectedIndex;
            var query = await new ParseQuery<Models.Supplier>().FindAsync();
            var list = query.Select(p => new
            {
                Id = p.ObjectId,
                Nombre = p.Name,
                Identificacion = p.Identification,
                p.Balance,
                Creado = p.CreatedAt
            });

            switch (option.ToLower())
            {
                //TODO: Identification validation. Returns false
                case "save":
                    if (Utilities.ValidateRnc(IdentificationTxt.Text))
                    {
                        if (string.IsNullOrEmpty(NameTxt.Text).Equals(false) && string.IsNullOrEmpty(IdentificationTxt.Text).Equals(false))
                        {
                            try
                            {

                                //Cuando se crea un suplidor, este debe de tener un balance de RD$0. La razón es que existe una relación balance-documentos
                                //donde el balance es la suma de todos sus documentos pendientes. Por ende, tampoco se podrá editar el balance de un suplidor directamente.
                                //De igual forma, el estado al crearlo será inactivo por default, porque el estado de un suplidor se maneja por medio del balance.
                                //Dejaré que se permita modificar el estado de un suplidor ya existente, debido a que puede que pase un tiempo y este, si se desea,
                                //pase a inactivo y tenga documentos pendientes.

                                Models.Supplier supplier;
                                if (!(IdTxt.Text.Length > 2))
                                {
                                    supplier = new Models.Supplier
                                    {
                                        Name = NameTxt.Text,
                                        Balance = 0,
                                        Identification = IdentificationTxt.Text,
                                        State = "Inactivo",
                                        Type = ((ComboBoxItem)TypeCbx.SelectedItem).Content.ToString()
                                    };
                                    await supplier.SaveAsync();
                                    MessageBox.Show("Suplidor creado");
                                }
                                else
                                {
                                    supplier = new Models.Supplier
                                    {
                                        ObjectId = IdTxt.Text,
                                        Name = NameTxt.Text,
                                        Identification = IdentificationTxt.Text,
                                        State = ((ComboBoxItem)StateCbx.SelectedItem).Content.ToString(),
                                        Type = ((ComboBoxItem)StateCbx.SelectedItem).Content.ToString()
                                    };
                                    await supplier.SaveAsync();
                                    MessageBox.Show("Suplidor actualizado");
                                }
                                PopulateGrid();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                    } 
                    else
                    {
                        MessageBox.Show("Cedula/RNC invalida");
                        break;
                    }
                    break;
                case "delete":
                    try
                    {
                        var element = list.ElementAt(ID);
                        var deleteSupplier = from a in new ParseQuery<Models.Supplier>()
                            where a.Id.Equals(element.Id)
                            select a;

                        await deleteSupplier.FirstAsync().Result.DeleteAsync();
                        MessageBox.Show("Eliminado satisfactoriamente");
                        PopulateGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error eliminando usuario\n{ex}");
                    }

                    break;
                    
            }


        }

        private async void PopulateGrid()
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
                    Ultimo = o.UpdatedAt
                });

                SupplierDgv.ItemsSource = result;
                SupplierDgv.Columns[6].Header = "Última abonación";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al popular \n{0}", ex.ToString());
            }
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Utilities.Clear(this);
        }

        private void NameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = (TextBox) sender;

            try
            {
                if (Regex.IsMatch(name.Text, "\\P{L}"))
                {
                    var index = name.SelectionStart - 1;
                    name.Text = name.Text.Remove(index, 1);

                    name.SelectionStart = index;
                    name.SelectionLength = 0;
                }
            }
            catch (Exception)
            {
               //No need to implement
            }
        }

        private void BalanceTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var balance = (TextBox) sender;
            
            if (Regex.IsMatch(balance.Text, "\\D"))
            {
                var index = balance.SelectionStart - 1;
                balance.Text = balance.Text.Remove(index, 1);

                balance.SelectionStart = index;
                balance.SelectionLength = 0;
            }
        }

        private void IdentificationTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var identification = (TextBox) sender;

            try
            {

               if (Regex.IsMatch(identification.Text, "\\D"))
                {
                    var index = identification.SelectionStart - 1;
                    identification.Text = identification.Text.Remove(index, 1);

                    identification.SelectionStart = index;
                    identification.SelectionLength = 0;
                }
            }
            catch (Exception)
            {
                //No need to implement
            }

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var window = new Query.Supplier();
                window.Show();
        }
    }
}