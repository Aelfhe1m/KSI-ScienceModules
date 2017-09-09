using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalScienceInnovation
{
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.FLIGHT)]
    public class KSIScenario : ScenarioModule
    {
        public override void OnLoad(ConfigNode node)
        {
            GRBEventLog.Instance.LoadScenario(node);
        }

        public override void OnSave(ConfigNode node)
        {
            GRBEventLog.Instance.SaveScenario(node);
        }
    }
}
