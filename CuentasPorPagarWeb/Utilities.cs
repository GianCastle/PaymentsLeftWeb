using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;

namespace CuentasPorPagar
{
    internal static class Utilities
    {
        public static bool ValidateRnc(string arg)

        {
            var vnTotal = 0;
            var vcCedula = arg.Replace("-", "");
            var pLongCed = vcCedula.Trim().Length;
            var multiplier = new int[11] {1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1};

            if (pLongCed < 11 || pLongCed > 11)
                return false;

            for (var vDig = 1; vDig <= pLongCed; vDig++)
            {
                var calculate = int.Parse(vcCedula.Substring(vDig - 1, 1))*multiplier[vDig - 1];
                vnTotal += calculate < 10
                    ? calculate
                    : int.Parse(calculate.ToString().Substring(0, 1)) + int.Parse(calculate.ToString().Substring(1, 1));
            }
            return vnTotal%10 == 0;
        }

        public static bool ValidatePassword(string password) => password.Length >= 6;

        public static bool NotEmpty(string elem)
        {
            if (String.IsNullOrEmpty(elem))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool IsEmpty(string elem)
        {
            if (String.IsNullOrEmpty(elem))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateEmail(string email)
        {
            var pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
            return Regex.IsMatch(email, pattern);
        }

        public static void Clear(Visual window)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(window);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = (Visual) VisualTreeHelper.GetChild(window, i);
                dynamic found;
                if (child is TextBox)
                {
                    found = (TextBox) child;
                    found.Clear();
                }
                else if (child is ComboBox)
                {
                    found = (ComboBox) child;
                    found.SelectedIndex = 0;
                }

                Clear(child);
            }
        }

        public static string ToDopCurrencyFormat(int value) => (value.Equals(0)) ? "SIN MONTO" : $"{value:RD$#,##0.00;($#,##0.00);''}";

        public static void ExportToPdf(DataGrid grid, string name, string title, string sum)
        {
            var table = new PdfPTable(grid.Columns.Count);

            using (var doc = new Document(iTextSharp.text.PageSize.A4))
            {
                using (PdfWriter.GetInstance(doc, new FileStream($"{name}.pdf", FileMode.Create)))
                {

                    var parentPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                    var logo = Image.GetInstance(parentPath + "/Resources/CS.jpg");
                    var robotoFontBlack = new Font(BaseFont.CreateFont(parentPath + "/Resources/roboto/Roboto-Medium.ttf",
                                                    BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 30f);
                    var robotoFontLight = new Font(BaseFont.CreateFont(parentPath + "/Resources/roboto/Roboto-Light.ttf",
                                                    BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 14f);
                    var robotoFontLightSmall = new Font(BaseFont.CreateFont(parentPath + "/Resources/roboto/Roboto-Light.ttf",
                                                        BaseFont.IDENTITY_H, BaseFont.EMBEDDED), 9f);

                    
                    doc.Open();
                    foreach (var t in grid.Columns)
                    {
                        table.AddCell(new Phrase(t.Header.ToString()) {Font = robotoFontLight });
                    }
                    table.HeaderRows = 1;
                    var itemsSource = grid.ItemsSource;
                    if (itemsSource != null)
                    {
                        foreach (var presenter
                            in
                            (from object item in itemsSource select grid.ItemContainerGenerator.ContainerFromItem(item))
                                .OfType<DataGridRow>().Select(FindVisualChild<DataGridCellsPresenter>))
                        {
                            for (var i = 0; i < grid.Columns.Count; ++i)
                            {
                                var cell = (DataGridCell) presenter.ItemContainerGenerator.ContainerFromIndex(i);
                                var txt = cell.Content as TextBlock;
                                if (txt != null)
                                    table.AddCell(new Phrase(txt.Text, robotoFontLightSmall));  
                            }
                        }

                        try
                        {
                            
                            
                            logo.Alignment = Element.ALIGN_MIDDLE;
                            doc.Add(logo);
                            doc.Add(new Paragraph("CUENTAS POR PAGAR", robotoFontBlack) {Alignment = Element.ALIGN_CENTER,});
                            doc.Add(new Paragraph($"{title}", robotoFontBlack) {Alignment = Element.ALIGN_CENTER });
                            doc.Add(Chunk.NEWLINE);
                            doc.Add(table);
                            doc.Add((string.IsNullOrEmpty(sum)) ? new Paragraph("") : new Paragraph(sum));
                            doc.Close();

                            var docPath = parentPath + "\\bin\\Debug\\" +name+".pdf";
                            if (MessageBox.Show("Desea ir al archivo generado?", "Documento exportado satisfactoriamente", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                System.Diagnostics.Process.Start("explorer.exe", "/select, "+docPath);
                            }
                            

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                    }
                }
            }
        }

        private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T) child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public static childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            return FindVisualChildren<childItem>(obj).FirstOrDefault();
        }

        public static Stream GetEmbeddedResourceStream(string resourceName)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        }
    

    public static string[] GetEmbeddedResourceNames()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

    }
}