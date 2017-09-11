using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalScienceInnovation
{
    public class KSIGammaRayBurstFollowUpExperiment : PartModule
    {
        [KSPEvent(guiActive = true, guiName = "#KSI_GRB_FollowUpAction", active = true)]
        public void FollowUpObservation()
        {
            MakeFollowUpObservation();
        }

        [KSPAction("#KSI_GRB_FollowUpAction")]
        public void FollowUpObservation(KSPActionParam param)
        {
            MakeFollowUpObservation();
        }

        private void MakeFollowUpObservation()
        {
            ScreenMessages.PostScreenMessage("TODO: make a science report about a GRB", 5f, ScreenMessageStyle.LOWER_CENTER);
        }

        public override string GetInfo()
        {
            return Localizer.GetStringByTag("#KSI_GRBFollowUp_Info");
        }
    }
}
