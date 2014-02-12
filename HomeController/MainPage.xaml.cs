using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using HomeController.Core;
using HomeController.Resources;

namespace HomeController
{
    public partial class MainPage : PhoneApplicationPage
    {
        private MainViewModel viewModel;

        private bool scenarioPlayed = false;

        public MainPage()
        {
            InitializeComponent();
            this.viewModel = App.MainViewModel;
            this.DataContext = this.viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var scenarioName = string.Empty;
            if(NavigationContext.QueryString.TryGetValue("scenario", out scenarioName) && !scenarioPlayed)
            {
                var scenario = await ScenarioCacheHelper.LoadScenario(scenarioName);
                await scenario.Play();

                await this.viewModel.LoadLightsDataAsync();
                this.scenarioPlayed = true;
            }

            base.OnNavigatedTo(e);
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

            await this.viewModel.LoadLightsDataAsync();
            await this.viewModel.LoadPandoraStationsDataAsync();
            await this.viewModel.LoadScenariosAsync();
            
            SystemTray.ProgressIndicator.IsVisible = false;
        }

        private async void lbLightSwitches_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lbLightSwitches.SelectedIndex != -1)
            {
                SystemTray.ProgressIndicator.IsVisible = true;
                var ls = this.lbLightSwitches.SelectedItem as LightSwitch;
                var result = await HomeControllerApi.FlipSwitch(ls);

                this.lbLightSwitches.SelectedIndex = -1;
                await this.viewModel.LoadLightsDataAsync();

                SystemTray.ProgressIndicator.IsVisible = false;
            }
        }

        private async void btnRefreshSwitches_Click(object sender, EventArgs e)
        {
            SystemTray.ProgressIndicator.IsVisible = true;
            await this.viewModel.LoadLightsDataAsync();
            SystemTray.ProgressIndicator.IsVisible = false; 
        }

        private void btnEditScenarios_Click(object sender, EventArgs e)
        {
            this.viewModel.ScenarioEditMode = !this.viewModel.ScenarioEditMode;
            this.lbScenarios.SelectedIndex = -1;
        }

        private void btnNowPlaying_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NowPlaying.xaml", UriKind.Relative));
        }

        private async void btnPlay_Click(object sender, EventArgs e)
        {
            if (lbPandoraStations.SelectedIndex != -1)
            {
                SystemTray.ProgressIndicator.IsVisible = true;
                var radioStation = lbPandoraStations.SelectedItem as PandoraRadioStation;

                var result = await HomeControllerApi.Play(radioStation);

                if (!result)
                    MessageBox.Show("Could not play radio");

                SystemTray.ProgressIndicator.IsVisible = false;
            }
        }

        private async void btnPause_Click(object sender, EventArgs e)
        {
            var result = await HomeControllerApi.Pause();

            if (!result)
                MessageBox.Show("Could not pause");
        }

        private async void btnRefreshStations_Click(object sender, EventArgs e)
        {
            await this.viewModel.LoadPandoraStationsDataAsync();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (((Pivot)sender).SelectedIndex)
            {
                case 0:
                    ApplicationBar = ((ApplicationBar)this.Resources["LightsAppBar"]);
                    break;
                case 1:
                    ApplicationBar = ((ApplicationBar)this.Resources["MusicAppBar"]);
                    break;
                case 2:
                    ApplicationBar = ((ApplicationBar)this.Resources["ScenariosAppBar"]);
                    break;
            }
        }

        private async void lbPandoraStations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbPandoraStations.SelectedIndex != -1)
            {
                var radioStation = lbPandoraStations.SelectedItem as PandoraRadioStation;

                var result = await HomeControllerApi.Play(radioStation);

                if (!result)
                    MessageBox.Show("Could not play radio");
            }
        }

        private void btnAddScenario_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/EditScenario.xaml?action=add", UriKind.Relative));
        }

        private async void lbScenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((sender as ListBox).SelectedIndex != -1)
            {
                var selectedScenario = (sender as ListBox).SelectedItem;
                this.viewModel.SelectedScenario = selectedScenario as Scenario;

                if (this.viewModel.ScenarioEditMode)
                {
                    NavigationService.Navigate(new Uri("/EditScenario.xaml", UriKind.Relative));
                }
                else
                {
                    await viewModel.SelectedScenario.Play();
                }
            }
            else
            {
                this.viewModel.SelectedScenario = null;
            }
        }
    }
}