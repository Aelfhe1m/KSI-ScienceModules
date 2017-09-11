using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;

namespace KerbalScienceInnovation
{
    public class KSIGammaRayBurstDetector : PartModule
    {
        [KSPField(isPersistant = true)]
        public bool isRunning;

        [KSPEvent(guiActive = true, guiName = "#KSI_GRB_StartMonitoring", active = true)]
        public void StartMonitoring()
        {
            StartMonitoringProgram();
        }

        [KSPAction("#KSI_GRB_StartMonitoring")]
        public void StartMonitoring(KSPActionParam param)
        {
            StartMonitoringProgram();
        }

        private void StartMonitoringProgram()
        {
            isRunning = true;
            ScreenMessages.PostScreenMessage(Localizer.GetStringByTag("#KSI_GRB_MonitoringStarted"), 10f, ScreenMessageStyle.UPPER_CENTER);
        }

        public override string GetInfo()
        {
            return Localizer.GetStringByTag("#KSI_GRBDetector_Info");
        }
    }
}
