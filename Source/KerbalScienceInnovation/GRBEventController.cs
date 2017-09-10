using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalScienceInnovation
{
    [KSPAddon(KSPAddon.Startup.FlightAndKSC, false)]
    public class GRBEventController : MonoBehaviour
    {
        private double lastEvent = 0;
        private double waitInterval;
        private bool loaded;

        private GRBMessageBox view;


        public void Start()
        {
            DontDestroyOnLoad(this);
            waitInterval = 60 * 60 * 100; // 100 hours for testing

            GameEvents.onHideUI.Add(OnHideUI);
            GameEvents.onShowUI.Add(OnShowUI);
            GameEvents.onGamePause.Add(OnGamePause);
            GameEvents.onGameUnpause.Add(OnGameUnpause);

            StartCoroutine("WaitForEventLogLoaded");
        }

        public void OnDestroy()
        {
            GameEvents.onHideUI.Remove(OnHideUI);
            GameEvents.onShowUI.Remove(OnShowUI);
            GameEvents.onGamePause.Remove(OnGamePause);
            GameEvents.onGameUnpause.Remove(OnGameUnpause);
        }

        public void OnGamePause()
        {
            if (view != null)
                view.Dismiss();
        }

        public void OnGameUnpause()
        {
            if (view != null)
                view.Show();
        }

        /// <summary>
		/// Hide UI.
		/// </summary>
		private void OnHideUI()
        {
            if (view != null)
                view.Dismiss();
        }

        /// <summary>
        /// Show UI.
        /// </summary>
        private void OnShowUI()
        {
            if (view != null)
                view.Dismiss();
        }

        public IEnumerator WaitForEventLogLoaded()
        {
            while (!GRBEventLog.Instance.Loaded)
                yield return null;

            lastEvent = GRBEventLog.Instance.LatestEventTime;
            loaded = true;
        }


        public void FixedUpdate()
        {
            if (!loaded)
                return;

            if (Planetarium.GetUniversalTime() < lastEvent + waitInterval)
                return;

            // time for new event
            lastEvent = Planetarium.GetUniversalTime();
            var evt = GRBEventLog.Instance.NewEvent();
            if (view == null)
            {
                view = new GRBMessageBox(OnMessageBoxClose, evt);
            }
            view.Show();
        }

        public void OnMessageBoxClose()
        {
            if (view != null)
            {
                view.Dismiss();
                view = null;
            }
        }
    }
}
