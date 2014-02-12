using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace HomeController
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbServerIP.Text = SettingsHelper.ServerIP;
            this.tbPandoraUser.Text = SettingsHelper.PandoraUser;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            SettingsHelper.ServerIP = this.tbServerIP.Text;
            SettingsHelper.PandoraUser = this.tbPandoraUser.Text;

            base.OnBackKeyPress(e);
        }
    }
}