using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeController.Core
{
    public static class ScenarioPlayer
    {
        public async static Task Play(this Scenario scenario)
        {
            foreach(var sw in scenario.SwitchStates)
            {
                await HomeControllerApi.SetSwitch(sw, sw.State);
            }

            if(scenario.PlayRadio)
            {
                await HomeControllerApi.Play(scenario.StationToPlay);
            }
            else
            {
                await HomeControllerApi.Pause();
            }

        }
    }
}
