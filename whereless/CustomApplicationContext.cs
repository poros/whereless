using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Reflection;
using FluentNHibernate.Diagnostics;
using whereless;
using whereless.LocalizationService;
using whereless.ViewModel;
using System.ComponentModel;


namespace whereless
{
   //class to be passed to Application.Run
   public class CustomApplicationContext : ApplicationContext
    {
       private static readonly string IconFileName = @"..\..\icon\TrayIcon_Yellow.ico";
       private static readonly string TrayIconGreen = @"..\..\icon\TrayIcon_Green.ico";
       private static readonly string TrayIconYellow = @"..\..\icon\TrayIcon_Yellow.ico";
       private static readonly string TrayIconRed = @"..\..\icon\TrayIcon_Red.ico"; 
       private static readonly string DefaultTooltip = "Whereless started";

       

       private System.ComponentModel.IContainer components;	// a list of components to dispose when the context is disposed
       private static NotifyIcon notifyIcon; // the icon that sits in the system tray
       
       
       //Only during developing
       private System.Windows.Forms.MenuItem menuExit;
       private System.Windows.Forms.ContextMenu contextMenu1;
       private System.Windows.Forms.MenuItem menuItem1;

        public CustomApplicationContext()
        {
            InitializeContext();
  
        }



        private void InitializeContext()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(IconFileName),
                Text = DefaultTooltip,
                Visible = true
            };

            notifyIcon.MouseDoubleClick += Mouse_DoubleClick;



            WherelessViewModel viewModel = WherelessViewModel.GetWherelessViewModel();
            viewModel.PropertyChanged +=new PropertyChangedEventHandler(viewModel_PropertyChange);

          
            
            //Only during developing
            this.contextMenu1 = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.contextMenu1.MenuItems.AddRange(
                        new System.Windows.Forms.MenuItem[] { this.menuItem1 });
            this.menuItem1.Index = 0;
            this.menuItem1.Text = "Exit";
            this.menuItem1.Click += new System.EventHandler(this.exitMenu);
            notifyIcon.ContextMenu = this.contextMenu1;

        }

       //just to close quickly (only during developing)
       private void exitMenu(object sender, EventArgs e)
       {
           Environment.Exit(0);
       }


        private void Mouse_DoubleClick(object sender, MouseEventArgs m)
       {
            Window bigWnd=new MainWindow();
            bigWnd.Show();
       }



       private static void viewModel_PropertyChange(object sender, PropertyChangedEventArgs e)
       {
           //Console.Beep(1000, 5000);
           if (e.PropertyName.Equals("CurrentLocation"))
           {
               if (((WherelessViewModel)sender).CurrentLocation.Name.Equals("UNKNWON") == true)
               {
                   notifyIcon.Icon = new Icon(TrayIconYellow);
                   //notifyIcon.ShowBalloonTip(4000, "Current location update", "This Location is UNKNWON", ToolTipIcon.Info);
               }
               else
               {
                   notifyIcon.Icon = new Icon(TrayIconGreen);
                   WherelessViewModel viewModel = WherelessViewModel.GetWherelessViewModel();
                   notifyIcon.ShowBalloonTip(4000, "Current location update", "You are at:"+viewModel.CurrentLocation.Name, ToolTipIcon.Info);
                   //Console.Beep(1000, 2000);
               }
           }
           else
           {
               if (e.PropertyName.Equals("RadioOff"))
               {
                   if (((WherelessViewModel)sender).RadioOff==true)
                   {
                       //Console.Beep(1000, 5000);
                       notifyIcon.Icon = new Icon(TrayIconRed);
                       notifyIcon.ShowBalloonTip(4000, "Change RADIO Status", "Radio is OFF", ToolTipIcon.Info);

                   }
                   else
                   {
                       notifyIcon.Icon = new Icon(TrayIconYellow);
                       notifyIcon.ShowBalloonTip(4000, "Change RADIO Status", "Radio is ON", ToolTipIcon.Info);
                   }
               }
               else
               {
                   if (e.PropertyName.Equals("ServicePaused"))
                   {
                       if (((WherelessViewModel)sender).ServicePaused==true)
                       {
                           notifyIcon.Icon = new Icon(TrayIconRed);
                           notifyIcon.ShowBalloonTip(4000, "Change SERVICE Status", "Service is OFF", ToolTipIcon.Info);
                       }
                       else
                       {
                           notifyIcon.Icon = new Icon(TrayIconYellow);
                           notifyIcon.ShowBalloonTip(4000, "Change SERVICE Status", "Service is ON", ToolTipIcon.Info);
                       }
                   }
               }
           }


        }



       



        
        // When the application context is disposed, dispose things like the notify icon.
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) { components.Dispose(); }
        }




    }
}
