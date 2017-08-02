using KSP.Localization;
using KSP.UI.Screens.Flight.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalScienceInnovation
{
    public abstract class ModuleScienceExtended : PartModule, IScienceDataContainer
    {
        /// <summary>
        /// Time between starting experiment and results being available
        /// </summary>
        [KSPField]
        protected int resultsDelay = 151200;

        [KSPField(isPersistant = false, guiName = "#KSI_Status", guiActive =true)]
        public string status;

        [KSPField(isPersistant = false, guiName = "#KSI_TimeRemaining", guiActive = false)]
        public string timeRemaining;

        [KSPField(isPersistant = true)]
        public double startTime;

        [KSPField]
        public float xmitBase = 1f;

        [KSPField]
        public float xmitBonus = 0f;

        [KSPField(isPersistant =true)]
        public bool isRunning;

        [KSPField(isPersistant =true)]
        public bool isFinished;

        [KSPField]
        public bool isResetable = false;

        [KSPField]
        public float interactionRange = 1.5f;

        [KSPField]
        public double powerConsumption = 10f;

        public abstract void CreateExperimentResult();
        public abstract bool CanRunExperiment();
        public abstract void OnNoPower();

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            Events["EVACollect"].unfocusedRange = interactionRange;
        }

        void FixedUpdate()
        {
            var canRun = CanRunExperiment();

            if (isRunning || isFinished)
                Events["StartExperiment"].active = false;
            else
                Events["StartExperiment"].active = true;

            if (isFinished)
                Events["GetResults"].active = true;
            else
                Events["GetResults"].active = false;

            if (isRunning)
            {
                // power consumption
                var required = TimeWarp.deltaTime * powerConsumption;
                if (ResourceAvailable("ElectricCharge") > required)
                    RequestResource("ElectricCharge", required);
                else
                    OnNoPower();
            }

            Events["ReviewEvent"].active = storedData.Count > 0;
            Events["EVACollect"].active = storedData.Count > 0;

            Fields["timeRemaining"].guiActive = isRunning || isFinished;

            UpdateTimeRemaining();
        }

        void UpdateTimeRemaining()
        {
            if (startTime == 0)
                return;

            if (Planetarium.GetUniversalTime() > startTime + resultsDelay)
            {
                isFinished = true;
                isRunning = false;
                timeRemaining = "Complete";
            }
            else
            {
                var remaining = startTime + resultsDelay - Planetarium.GetUniversalTime();
                timeRemaining = KSPUtil.dateTimeFormatter.PrintDateDeltaCompact(remaining, true, true, true);
            }

        }

        [KSPEvent(guiActive = true, guiName = "#KSI_StartExperiment", active = true)]
        public void StartExperiment()
        {
            RunExperiment();
        }

        [KSPAction("#KSI_StartExperiment")]
        public void StartExperiment(KSPActionParam param)
        {
                RunExperiment();
        }

        public abstract void RunExperiment(bool silent = false);

        [KSPEvent(guiActive = true, guiName = "#KSI_GetResults", active = false)]
        public void GetResults()
        {
            GetScience();
        }

        [KSPAction("#KSI_GetResults")]
        public void GetResults(KSPActionParam param)
        {
            GetScience();
        }

        public abstract void GetScience(bool silent = false);
        
        [KSPEvent(guiActive = true, guiName = "#KSI_Review", active = false)]
        public void ReviewEvent()
        {
            ReviewData();
        }

        [KSPEvent(guiActiveUnfocused = true, guiName = "#KSI_CollectEVA", externalToEVAOnly = true, unfocusedRange = 1.5f, active = false)]
        public void EVACollect()
        {
            List<ModuleScienceContainer> EVACont = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceContainer>();
            if (storedData.Count > 0)
            {
                if (EVACont.First().StoreData(new List<IScienceDataContainer>() { this }, false))
                {
                    foreach (ScienceData data in storedData)
                        DumpData(data);
                }
            }
        }

        protected List<ScienceData> storedData = new List<ScienceData>();
        protected ExperimentsResultDialog expDialog = null;

        public override void OnLoad(ConfigNode node)
        {
            if (node.HasNode("ScienceData"))
            {
                foreach (ConfigNode storedDataNode in node.GetNodes("ScienceData"))
                {
                    ScienceData data = new ScienceData(storedDataNode);
                    storedData.Add(data);
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            node.RemoveNodes("ScienceData");

            foreach (ScienceData data in storedData)
            {
                ConfigNode storedDataNode = node.AddNode("ScienceData");
                data.Save(storedDataNode);
            }
        }

        #region IScienceDataContainer

        public void DumpData(ScienceData data)
        {
            expDialog = null;

            if (storedData.Contains(data))
                storedData.Remove(data);
        }

        private double ResourceAvailable(string resource)
        {
            var res = PartResourceLibrary.Instance.GetDefinition(resource);
            double amount, maxAmount;
            part.GetConnectedResourceTotals(res.id, out amount, out maxAmount);
            return amount;
        }

        private float RequestResource(string resource, double amount)
        {
            var res = PartResourceLibrary.Instance.GetDefinition(resource);
            return (float)this.part.RequestResource(res.id, amount);
        }


        public ScienceData[] GetData() => storedData.ToArray();

        public int GetScienceCount() => storedData.Count;

        public bool IsRerunnable() => false;

        public void ReturnData(ScienceData data)
        {
            if (data == null)
                return;

            storedData.Clear();

            storedData.Add(data);
        }

        public void ReviewData()
        {
            if (storedData.Count < 1)
                return;

            expDialog = null;
            ScienceData data = storedData[0];
            expDialog = ExperimentsResultDialog.DisplayResult(
                new ExperimentResultDialogPage(
                    part, data, xmitBase, xmitBonus, false, "", false, new ScienceLabSearch(vessel, data), DumpData, KeepData, TransmitData, SendToLab));
        }

        private void KeepData(ScienceData data)
        {
            expDialog = null;
        }

        private void TransmitData(ScienceData data)
        {
            expDialog = null;

            IScienceDataTransmitter bestTransmitter = ScienceUtil.GetBestTransmitter(vessel);

            if (bestTransmitter != null)
            {
                bestTransmitter.TransmitData(new List<ScienceData> { data });
                DumpData(data);
            }
            else if (CommNet.CommNetScenario.CommNetEnabled)
                ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_237738"), 3f, ScreenMessageStyle.UPPER_CENTER); // No usable, in-range Comms Devices on this vessel. Cannot Transmit Data.
            else
                ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_237740"), 3f, ScreenMessageStyle.UPPER_CENTER); // No usable Comms Devices on this vessel. Cannot Transmit Data.
        }

        private void SendToLab(ScienceData data)
        {
            expDialog = null;
            ScienceLabSearch labSearch = new ScienceLabSearch(vessel, data);

            if (labSearch.NextLabForDataFound)
            {
                StartCoroutine(labSearch.NextLabForData.ProcessData(data, null));
                DumpData(data);
            }
            else
                labSearch.PostErrorToScreen();
        }

        public void ReviewDataItem(ScienceData data) => ReviewData();

        #endregion
    }
}
