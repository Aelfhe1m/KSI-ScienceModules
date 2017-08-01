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

        [KSPField]
        public bool isRunning;

        [KSPField]
        public bool isFinished;

        public abstract void CreateExperimentResult();
        public abstract bool CanRunExperiment();

        void FixedUpdate()
        {
            //if (CanRunExperiment())
            //{
            //    Events["StartExperiment"].active = true;
            //    Actions["StartExperiment"].active = true;
            //}
            //else
            //{
            //    Events["StartExperiment"].active = false;
            //    Actions["StartExperiment"].active = false;
            //}

            Events["ReviewEvent"].active = storedData.Count > 0;
            Events["EVACollect"].active = storedData.Count > 0;

            Fields["timeRemaining"].guiActive = isRunning || isFinished;

            UpdateTimeRemaining();
        }

        void UpdateTimeRemaining()
        {
            if (startTime == 0 || !isRunning)
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
                timeRemaining = KSPUtil.dateTimeFormatter.PrintDateDelta(remaining, true, true, true);
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
                    part, data, xmitBase, xmitBonus, false, "", true, new ScienceLabSearch(vessel, data), DumpData, KeepData, TransmitData, SendToLab));
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
