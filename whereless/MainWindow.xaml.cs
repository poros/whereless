using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using log4net;
using log4net.Config;
using System.Windows;
using whereless.LocalizationService;
using whereless.Model.Entities;
using whereless.ViewModel;
using whereless.Model;
using System.Threading;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.Forms.MessageBox;

namespace whereless
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource wherelessViewModelViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("wherelessViewModelViewSource")));
            // Caricare i dati impostando la proprietà CollectionViewSource.Source:
            // wherelessViewModelViewSource.Source = [origine dati generica]
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            if (((Button)sender).Name.Equals("ButtonPause") == true || ((Button)sender).Name.Equals("ButtonPauseB") == true || ((Button)sender).Name.Equals("ButtonPauseC") == true)
            {
                viewModel.PauseService();
            }
            else
            {
                if (((Button)sender).Name.Equals("ButtonPlay") == true)
                {
                    viewModel.PlayService();
                }
            }
            
        }


        private void DeleteActionClicked(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            //MessageBox.Show("Element id: " + ((Button)sender).Content.ToString(), "Pressed delete element",
            //                     MessageBoxButtons.YesNo,
            //                     MessageBoxIcon.Question);

            viewModel.DeleteActivityFromLocation(viewModel.CurrentLocation.Name, int.Parse(((Button)sender).Content.ToString()));
            
            viewModel.UpdateLocations();
        }



        private void OpenAddActivity(object sender, RoutedEventArgs e)
        {
            Window WndNewAct=new AddNewActivity();
            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(WndNewAct);

            WndNewAct.Show();
        }
        





        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Console.Beep(1000,2000);
            
        }

        private void Buotton_ForceUn(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();
            viewModel.UpdateLocations();

            viewModel.ForceUnknown();
        }


        private void AddOrSetLocation(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            string locNameStatusKnown = AutoCompleteBox.Text;
            string locNameStatusUnknown = AutoCompleteBoxU.Text;

            if (locNameStatusKnown.Equals("")==false)
            {
                if (viewModel.Locations.Any(l => l.Name.Equals(locNameStatusKnown) == true))
                {
                    viewModel.ForceLocation(locNameStatusKnown);
                    AutoCompleteBox.Text = "";
                    AutoCompleteBoxU.Text = "";
                    
                    viewModel.UpdateLocations();

                    return;
                }
                viewModel.RegisterLocation(locNameStatusKnown);

                AutoCompleteBox.Text = "";
                AutoCompleteBoxU.Text = "";

                viewModel.UpdateLocations();

                return;

            }



            if (locNameStatusUnknown.Equals("") == false)
            {
                if (viewModel.Locations.Any(l => l.Name.Equals(locNameStatusUnknown) == true))
                {
                    viewModel.ForceLocation(locNameStatusUnknown);
                    AutoCompleteBox.Text = "";
                    AutoCompleteBoxU.Text = "";
                    viewModel.UpdateLocations();
                    return;
                }
                viewModel.RegisterLocation(locNameStatusUnknown);
                viewModel.UpdateLocations();
                AutoCompleteBox.Text = "";
                AutoCompleteBoxU.Text = "";
                return;
            }   
            
        }



        private void LocationForce(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            string loc = ((Button) sender).Content.ToString();
            viewModel.ForceLocation(loc);
            viewModel.UpdateLocations();
            return;
        }


        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            
        }


        private void DeleteLocation(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            string loc = ((Button)sender).Content.ToString();
            
            viewModel.DeleteLocation(loc);
           Thread.Sleep(100);
            viewModel.UpdateLocations();
            return;
            
            
            
        }


        private void DeleteActionClickedAllLocations(object sender, RoutedEventArgs e)
        {
            WherelessViewModel viewModel = WherelessViewModel.GetInstance();

            string location = ((Button)sender).Tag.ToString();

            MessageBox.Show("Element id: " + ((Button)sender).Content.ToString()+ "loca: "+location, "Pressed delete element",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);

            viewModel.DeleteActivityFromLocation(location, int.Parse(((Button)sender).Content.ToString()));
            
            viewModel.UpdateLocations();
        }
    }
}
