using KSP.UI.Screens.Flight.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;
using UnityEngine;

namespace KerbalScienceInnovation
{

    //MODULE
    //{
        // name = KSIGravityWaveModule
        // timeToCollect = 151200 // time between starting experiment and results available. Kerbin day is 21600 seconds
        // fractionOfSoi = 0.9 // Pe and Ap must be > 90% of SOI radius
        // inclination = 90 // polar orbit
        // inclinationError = 2.5 // allow orbit inclination to be off by this number of degrees
	
    //}



public class KSIGravityWaveModule : ModuleScienceExtended
    {
        [KSPField]
        public string experimentId = "KSIGravityWaveExperiment";

        [KSPField]
        public new int resultsDelay = 151200;

        [KSPField]
        public double fractionOfSOI = 0.9;

        [KSPField]
        public double inclination = 90;

        [KSPField]
        public double allowedIncError = 2.5;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            
        }

        public override bool CanRunExperiment()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return false;

            var msg = "";
            var isOK = true;

            var debugInfo = new StringBuilder();

            debugInfo.Append("[KSI]: altitude: ");
            debugInfo.Append(vessel.altitude);
            debugInfo.Append(", SOI: ");
            debugInfo.Append(vessel.mainBody.sphereOfInfluence);
            debugInfo.Append(", Inc: ");
            debugInfo.Append(vessel.orbit.inclination);

            // Debug.Log(debugInfo.ToString());

            if (vessel.orbit.PeA < fractionOfSOI * vessel.mainBody.sphereOfInfluence)
            {
                msg += Localizer.GetStringByTag("#KSI_Grav_PeTooLow");
                isOK = false;
            }
            if (vessel.orbit.ApA < fractionOfSOI * vessel.mainBody.sphereOfInfluence)
            {
                msg += Localizer.GetStringByTag("#KSI_Grav_ApTooLow");
                isOK = false;
            }
            if (vessel.orbit.ApA > vessel.mainBody.sphereOfInfluence)
            {
                msg += Localizer.GetStringByTag("#KSI_Grav_ApTooHigh");
                isOK = false;
            }

            if (vessel.orbit.inclination > inclination + allowedIncError || vessel.orbit.inclination < inclination - allowedIncError)
            {
                msg += Localizer.GetStringByTag("#KSI_Grav_IncWrong");
                isOK = false;
            }

            if (msg.Length == 0)
                msg = Localizer.GetStringByTag("#KSI_Grav_OrbitOK");

            status = msg;

            return isOK;
        }

        public override void RunExperiment(bool silent = false)
        {
            if (!CanRunExperiment())
            {
                ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_Grav_RequiredOrbitMessage", (fractionOfSOI * vessel.mainBody.sphereOfInfluence).ToString("N0")), 5f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (!isRunning && !isFinished)
            {
                ScreenMessages.PostScreenMessage(Localizer.GetStringByTag("#KSI_Grav_startedMessage"), 5f, ScreenMessageStyle.UPPER_CENTER);
                isRunning = true;
                startTime = Planetarium.GetUniversalTime();
                return;
            }

            if (isFinished)
                CreateExperimentResult();

            if (!silent)
                ReviewData();
        }

        public override void CreateExperimentResult()
        {

            storedData.Clear();
            ScienceData data = null;
            ScienceExperiment experiment = ResearchAndDevelopment.GetExperiment(experimentId);
            ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(experiment, ExperimentSituations.InSpaceHigh, vessel.mainBody, "", "");

            

            if (data == null)
                return;
            storedData.Add(data);
        }

    }
}
