using System;
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

        public void Start()
        {
            waitInterval = 60 * 60 * 10; // 10 hours for testing
            loaded = true;
        }

        private GRBMessageBox view;

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
