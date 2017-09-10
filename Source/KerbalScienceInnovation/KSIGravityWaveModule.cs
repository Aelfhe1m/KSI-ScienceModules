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
	//	name = KSIGravityWaveModule
    //  experimentId = KSIGravityWaveExperiment
    //  isResetable = false // one experiment, one result. Send new probe for more.
	//	resultsDelay = 151200 // 7 * 21600 = 7 Kerbal days
	//	fractionOfSOI = 0.9 // for planets the minimum Ap/Pe will be fractionOfSOI * PlanetsSOI
	//	inclination = 90 // polar orbit
	//	allowedIncError = 2.5 // allow a small deviation from polar
	//	minSolarOrbit = 75000000000;   // Jool Ap = 72 212 238 387m + SOI = 2 455 985 200
	//	samplesInSolarOrbit = 2 // if it's solar orbit then allow experiment to be run twice (north and south)
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

        [KSPField]
        public double minSolarOrbit = 75000000000;   // Jool Ap = 72 212 238 387m + SOI = 2 455 985 200

        [KSPField]
        public int samplesInSolarOrbit = 2;

        [KSPField(isPersistant = true)]
        public int solarSamplesCollected = 0;

        [KSPField]
        public float solarScienceMultiplier = 1;


        public override string GetInfo()
        {
            return Localizer.Format("#KSI_Grav_EditorInfo", (fractionOfSOI * 100).ToString(), (minSolarOrbit / 1000).ToString("N0"), resultsDelay / KSPUtil.dateTimeFormatter.Day, powerConsumption, RUIutils.GetYesNoUIString(true), RUIutils.GetYesNoUIString(false));
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

            var body = vessel.mainBody;
            var bodyType = BodyUtils.BodyType(body);

            double minOrbit = 0;

            if (bodyType == CelestialBodyType.MOON || bodyType == CelestialBodyType.NOT_APPLICABLE)
            {
                msg = "Body must be a sun or planet";
                isOK = false;
            }
            else if (bodyType == CelestialBodyType.SUN)
            {
                minOrbit = minSolarOrbit;
            }
            else if (bodyType == CelestialBodyType.PLANET)
            {
                minOrbit = fractionOfSOI * body.sphereOfInfluence;
            }


            // Debug.Log(debugInfo.ToString());
            if (bodyType != CelestialBodyType.MOON && bodyType != CelestialBodyType.NOT_APPLICABLE)
            {
                if (vessel.orbit.PeA < minOrbit)
                {
                    msg += " " + Localizer.GetStringByTag("#KSI_Grav_PeTooLow") + " ";
                    isOK = false;
                }
                if (vessel.orbit.ApA < minOrbit)
                {
                    msg += " " + Localizer.GetStringByTag("#KSI_Grav_ApTooLow") + " ";
                    isOK = false;
                }
                if (vessel.orbit.ApA > body.sphereOfInfluence)
                {
                    msg += " " + Localizer.GetStringByTag("#KSI_Grav_ApTooHigh") + " ";
                    isOK = false;
                }

                if (vessel.orbit.inclination > inclination + allowedIncError || vessel.orbit.inclination < inclination - allowedIncError)
                {
                    msg += " " + Localizer.GetStringByTag("#KSI_Grav_IncWrong") + " ";
                    isOK = false;
                }
            }

            if (msg.Length == 0)
                msg = " " + Localizer.GetStringByTag("#KSI_Grav_OrbitOK") + " ";

            status = msg;

            return isOK;
        }

        public override void RunExperiment(bool silent = false)
        {
            if (!CanRunExperiment())
            {
                var bodyType = BodyUtils.BodyType(vessel.mainBody);
                if (bodyType == CelestialBodyType.MOON || bodyType == CelestialBodyType.NOT_APPLICABLE)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_Grav_RequiredBodyMessage"), 5f, ScreenMessageStyle.UPPER_CENTER);
                }
                else if (bodyType == CelestialBodyType.SUN)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_Grav_RequiredOrbitMessage", minSolarOrbit.ToString("N0")), 5f, ScreenMessageStyle.UPPER_CENTER);
                }
                else if (bodyType == CelestialBodyType.PLANET)
                {
                    ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_Grav_RequiredOrbitMessage", (fractionOfSOI * vessel.mainBody.sphereOfInfluence).ToString("N0")), 5f, ScreenMessageStyle.UPPER_CENTER);
                }
                
                return;
            }

            if (!isRunning && !isFinished)
            {
                var days = resultsDelay / KSPUtil.dateTimeFormatter.Day;
                Debug.Log($"[KSI]: delay = {resultsDelay}, day = {KSPUtil.dateTimeFormatter.Day}, days = {days}");
                ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_Grav_startedMessage", days), 5f, ScreenMessageStyle.UPPER_CENTER);
                isRunning = true;
                startTime = Planetarium.GetUniversalTime();
                return;
            }

           
        }


        public override void GetScience(bool silent = false)
        {
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
            var biome = GetBiome();
            var displayBiome = biome == "" ? "" : Localizer.GetStringByTag("#KSI_Biome_" + biome);
  
            ScienceSubject subject = ResearchAndDevelopment.GetExperimentSubject(experiment, ExperimentSituations.InSpaceHigh, vessel.mainBody, biome, displayBiome);
            float amount = experiment.baseValue * subject.dataScale;
            amount = (IsSolar()) ? amount * solarScienceMultiplier : amount;
            data = new ScienceData(amount, xmitBase, xmitBonus, subject.id, subject.title, false, part.flightID);


            if (data == null)
                return;
            storedData.Add(data);
        }

        public string GetBiome()
        {
            if (!IsSolar())
                return "";

            var lat = (vessel.latitude + 180 + 90) % 180 - 90;
            if (lat > 0)
                return "North";
            else
                return "South";

        }

        public bool IsSolar()
        {
            var bodyType = BodyUtils.BodyType(vessel.mainBody);
            return (bodyType == CelestialBodyType.SUN);
        }

        public override void OnNoPower()
        {
            // reset experiment
            ScreenMessages.PostScreenMessage(Localizer.GetStringByTag("#KSI_Grav_OutOfPowerMessage"), 5f, ScreenMessageStyle.UPPER_CENTER);
            isRunning = false;
            isFinished = false;
            startTime = 0;
        }
    }
}
