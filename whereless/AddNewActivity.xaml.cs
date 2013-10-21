using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using whereless.Model.Entities;
using whereless.ViewModel;
using MessageBox = System.Windows.Forms.MessageBox;


namespace whereless
{
    /// <summary>
    /// Logica di interazione per AddNewActivity.xaml
    /// </summary>
    public partial class AddNewActivity : Window
    {
        private static string filename;
        private static string actionName;
        private static string type;
        private static string argument;

        public AddNewActivity()
        {
            InitializeComponent();
            TextActName.Text = "";
            TextActName.IsEnabled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == true)
            {
                // Open document 
                filename = dlg.FileName;
            }

            AddAc.IsEnabled = true;

        }


        private void AddActivity(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();
            Location l = viewModel.CurrentLocation;


            if (Radio_01.IsChecked == true)
            {
                type = "Wallpaper";
            }
            else
            {
                if (Radio_02.IsChecked == true)
                {
                    type = "ExeFile";
                }
                else
                {
                    if (Radio_03.IsChecked == true)
                    {
                        type = "BatchFile";
                    }
                }
            }

            actionName = TextActName.Text;
            argument = TextArgument.Text;

            if (actionName.Equals("") == false && filename.Equals("") == false)
            {
                viewModel.AddActivityToLocation(l.Name, actionName, filename, argument, type);
                
                MessageBox.Show("Activity "+actionName+" added", "ACTIVITY ADDED",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                this.Close();
            }

            
        }


    }
}
