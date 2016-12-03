using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CuentasPorPagar.Models;
using Parse;

namespace CuentasPorPagar.Views.CRUD
{
    /// <summary>
    ///     Interaction logic for User.xaml
    /// </summary>
    public partial class User : Window
    {
        public User()
        {
            InitializeComponent();
        }

        public async void PopulateGrid()
        {
            var query = await new ParseQuery<Users>().FindAsync();
            var result = from o in query
                select new
                {
                    ID = o.ObjectId,
                    Usuario = o.Username,
                    Correo = o.Email,
                    Fecha = o.CreatedAt
                };
            UserDgv.ItemsSource = result;
        }

        private void CreateUserBtn_Click(object sender, RoutedEventArgs e)
        {
            Crud("create");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (bool.Parse(Application.Current.Properties["IsAdmin"].ToString()))
            { 
                DeleteUserBtn.IsEnabled = true;
                CreateUserBtn.IsEnabled = true;
            }

            PopulateGrid();
          

        }

        public async void Crud(string option)
        {
            try
            {
                var id = UserDgv.SelectedIndex;
                var query = await new ParseQuery<Users>().FindAsync();
                var result = query.Select(o => o.ObjectId);
                var element = "";


                element = result.ElementAt(id);

                switch (option.ToLower())
                {
                    case "create":
                        if (Utilities.ValidatePassword(passwordBox.Password) &&
                            Utilities.ValidateEmail(EmailTxt.Text) && Utilities.NotEmpty(NicknameTxt.Text))
                        {

                            try
                            {
                                Models.Users user;
                                if (IdTxt.Text.Length > 2)
                                {
                                    user = new Users

                                    {
                                        ObjectId = IdTxt.Text,
                                        Username = NicknameTxt.Text,
                                        Password = passwordBox.Password,
                                        Email = EmailTxt.Text,
                                        Name = UserNameTxt.Text,
                                        Permission = ((ComboBoxItem)PermissionsCbx.SelectedItem).Content.ToString()
                                    };
                                    await user.SaveAsync();
                                    MessageBox.Show("Usuario editado satisfactoriamente");
                                }
                                else
                                {
                                    user = new Users

                                    {
                                        Username = NicknameTxt.Text,
                                        Password = passwordBox.Password,
                                        Email = EmailTxt.Text,
                                        Name = UserNameTxt.Text,
                                        Permission = ((ComboBoxItem)PermissionsCbx.SelectedItem).Content.ToString()
                                    };
                                    await user.SaveAsync();
                                    MessageBox.Show("Usuario creado satisfactoriamente");
                                }


                                PopulateGrid();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString());
                            }
                        }
                       
                        else
                            MessageBox.Show("El campo Email, Contraseña o Nombre de usuario no son validos");

                        break;

                    case "delete":
                        try
                        {
                            var deleteQuery = new ParseQuery<Users>().Where(o => o.ObjectId.Equals(element));
                            await deleteQuery.FirstAsync().Result.DeleteAsync();
                            PopulateGrid();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        break;

                    case "edit":
                        try
                        {
                            var editQuery = new ParseQuery<Users>().Where(o => o.ObjectId.Equals(element));

                            var editElements = await editQuery.FirstAsync();
                            IdTxt.Text = editElements.ObjectId;
                            NicknameTxt.Text = editElements.Username;
                            UserNameTxt.Text = editElements.Name;
                            passwordBox.Password = editElements.Password;
                            EmailTxt.Text = editElements.Email;
                            PermissionsCbx.SelectedIndex = (editElements.Permission.Equals("Administrador")) ? 1 : 2;


                            var user = new Users
                            {

                                ObjectId = IdTxt.Text,
                                Username = NicknameTxt.Text,
                                Name = UserNameTxt.Text,
                                Email = EmailTxt.Text,
                                Password = passwordBox.Password
                            };
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                        break;


                    default:
                        MessageBox.Show("No cases were assigned");
                        break;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("No se ha seleccionado ningun elemento");
            }
           
        }

        private void DeleteUserBtn_Click(object sender, RoutedEventArgs e)
        {
            Crud("delete");
        }

        private void EditUserBtn_Click(object sender, RoutedEventArgs e)
        {
            Crud("edit");
        }

        private void UserNameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var name = (TextBox)sender;

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

        private void NicknameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var username = (TextBox)sender;

            try
            {
                if (Regex.IsMatch(username.Text, "\\P{L}"))
                {
                    var index = username.SelectionStart - 1;
                    username.Text = username.Text.Remove(index, 1);

                    username.SelectionStart = index;
                    username.SelectionLength = 0;
                }
            }
            catch (Exception)
            {
                //No need to implement
            }
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            Crud("edit");
        }

        private void ClearBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Utilities.Clear(this);
        }

        private void ExitUserBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}