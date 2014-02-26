using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using HomeController.Core;

namespace HomeController
{
    public partial class NowPlaying : PhoneApplicationPage
    {
        public NowPlaying()
        {
            InitializeComponent();
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = await HomeControllerApi.GetCurrentSong();
        }

        private async void btnVolumeUp_Click(object sender, EventArgs e)
        {
            var currentVolume = await HomeControllerApi.GetVolume();
            await HomeControllerApi.SetVolume(currentVolume + 5);
        }

        private async void btnVolumeDown_Click(object sender, EventArgs e)
        {
            var currentVolume = await HomeControllerApi.GetVolume();
            await HomeControllerApi.SetVolume(currentVolume - 5);
        }
    }
}