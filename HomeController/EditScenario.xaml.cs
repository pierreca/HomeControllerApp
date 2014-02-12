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
using System.Collections.ObjectModel;

namespace HomeController
{
    public partial class EditScenario : PhoneApplicationPage
    {
        public EditScenario()
        {
            InitializeComponent();
            DataContext = App.MainViewModel;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if(App.MainViewModel.SelectedScenario.PlayRadio)
            {
                App.MainViewModel.SelectedScenario.StationToPlay = lpRadioStations.SelectedItem as PandoraRadioStation;
            }

            await ScenarioCacheHelper.SaveScenario(App.MainViewModel.SelectedScenario);
            NavigationService.GoBack();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            await ScenarioCacheHelper.DeleteScenario(App.MainViewModel.SelectedScenario.Name);
            NavigationService.GoBack();
        }

        private async void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            SystemTray.SetIsVisible(this, true);
            SystemTray.SetOpacity(this, 0.5);

            var prog = new ProgressIndicator();
            prog.IsVisible = true;
            prog.IsIndeterminate = true;
            prog.Text = "loading...";

            SystemTray.SetProgressIndicator(this, prog);

            if (App.MainViewModel.SelectedScenario == null)
            {
                App.MainViewModel.SelectedScenario = new Scenario();
                App.MainViewModel.SelectedScenario.SwitchStates = (await HomeControllerApi.GetSwitches()).ToList();
            }
            else if (App.MainViewModel.SelectedScenario.StationToPlay != null)
            {
                var candidate = from r in lpRadioStations.ItemsSource as ObservableCollection<PandoraRadioStation>
                                where string.Compare(r.Id, App.MainViewModel.SelectedScenario.StationToPlay.Id, StringComparison.InvariantCultureIgnoreCase) == 0
                                select r;

                if (candidate.Count() > 0)
                    lpRadioStations.SelectedItem = candidate.First();
            }

            SystemTray.ProgressIndicator.IsVisible = false;
        }

        private void btnPin_Click(object sender, EventArgs e)
        {
            var pageParam = HttpUtility.UrlEncode(App.MainViewModel.SelectedScenario.Name);

            if(ShellTile.ActiveTiles.Where(tile => tile.NavigationUri.ToString().Contains(pageParam)).Count() == 0)
            {
                var tileData = new StandardTileData();
                tileData.Title = App.MainViewModel.SelectedScenario.Name;
                tileData.BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative);
                
                ShellTile.Create(new Uri("/MainPage.xaml?scenario=" + pageParam, UriKind.Relative), tileData);
            }
            else
            { 
                MessageBox.Show("Scenario already pinned to the start screen");
            }
        }
    }
}