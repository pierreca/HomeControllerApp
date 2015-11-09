using HomeController.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController
{
    public class MainViewModel : ViewModelBase
    {
        private bool _lightApiFailure = false;
        private string _lightApiFailureMessage = string.Empty;
        private ObservableCollection<LightSwitch> _lightSwitches;
        private ObservableCollection<PandoraRadioStation> _radioStations;

        private bool _scenarioEditMode = false;
        private ObservableCollection<Scenario> _scenarios;
        private Scenario _selectedScenario;

        public bool LightApiFailure
        {
            get { return this._lightApiFailure; }
            set { this.SetProperty<bool>(ref this._lightApiFailure, value); }
        }

        public string LightApiFailureMessage
        {
            get { return this._lightApiFailureMessage; }
            set { this.SetProperty<string>(ref this._lightApiFailureMessage, value); }
        }

        public ObservableCollection<LightSwitch> LightSwitches
        {
            get { return this._lightSwitches; }
            set { this.SetProperty<ObservableCollection<LightSwitch>>(ref this._lightSwitches, value); }
        }

        public ObservableCollection<PandoraRadioStation> RadioStations
        {
            get { return this._radioStations; }
            set { this.SetProperty<ObservableCollection<PandoraRadioStation>>(ref this._radioStations, value); }
        }

        public ObservableCollection<Scenario> Scenarios
        {
            get { return this._scenarios; }
            set { this.SetProperty<ObservableCollection<Scenario>>(ref _scenarios, value); }
        }

        public Scenario SelectedScenario
        {
            get { return this._selectedScenario; }
            set { this.SetProperty<Scenario>(ref this._selectedScenario, value); }
        }

        public bool ScenarioEditMode
        {
            get { return this._scenarioEditMode; }
            set { this.SetProperty<bool>(ref this._scenarioEditMode, value); }
        }

        public async Task LoadLightsDataAsync()
        {
            try
            {
                this.LightSwitches = new ObservableCollection<LightSwitch>(await HomeControllerApi.GetSwitches());
                this.LightApiFailure = false;
            }
            catch (HomeControllerApiException ex)
            {
                this.LightApiFailure = true;
                this.LightApiFailureMessage = ex.Message;
            }
        }

        public async Task LoadPandoraStationsDataAsync()
        {
            this.RadioStations = new ObservableCollection<PandoraRadioStation>(await HomeControllerApi.GetPandoraStations());
        }

        public async Task LoadScenariosAsync()
        {
            this.Scenarios = new ObservableCollection<Scenario>(await ScenarioCacheHelper.LoadOrCreateScenarios());
        }
    }
}
