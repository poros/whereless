﻿using System;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using log4net;
using log4net.Config;
using System.Windows;
using whereless.LocalizationService;
using whereless.ViewModel;
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

            MessageBox.Show("Element id: " + ((Button)sender).Content.ToString(), "Pressed delete element",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Question);


        }



        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.Beep(1000,2000);
            
        }
    }
}
