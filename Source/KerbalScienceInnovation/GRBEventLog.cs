using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalScienceInnovation
{
    public class GRBEventLog
    {
        private static GRBEventLog instance;
        private static List<GRBEvent> events = new List<GRBEvent>();
        private bool _loaded = false;
        private double _latestEventTime = 0;

        public bool Loaded => _loaded;

        public List<GRBEvent> Events => events;

        private GRBEventLog() { }

        public static GRBEventLog Instance
        {
            get
            {
                if (instance == null)
                    instance = new GRBEventLog();
                return instance;
            }
        }

        internal void SaveScenario(ConfigNode node)
        {
            var evtsNode = node.AddNode("GRBEvents");
            foreach (var evt in events)
            {
                var nd = new ConfigNode("Event");
                nd.AddValue("image", evt.ImageUrl);
                nd.AddValue("msg", evt.Message);
                nd.AddValue("ut", evt.UT);
                nd.AddValue("ra", evt.RA);
                nd.AddValue("dec", evt.Dec);

                evtsNode.AddNode(nd);
            }
            Debug.Log($"[KSI]: saved {events.Count} GRB events to scenario");
        }

        public double LatestEventTime => _latestEventTime;

        internal GRBEvent NewEvent()
        {
            var evt = new GRBEvent {
                UT = Planetarium.GetUniversalTime(),
                ImageUrl = "KSI/Resources/grb1",
                Message = "#KSI_GRBWindow_DiscoveryMessage1",
                RA = 100.52,
                Dec = -27.34
            };
            events.Add(evt);
            if (evt.UT > _latestEventTime)
                _latestEventTime = evt.UT;

            
            var delta = 100.0f; // TODO: calc amount of science to award for new GRB discovery
            ResearchAndDevelopment.Instance.AddScience(delta, TransactionReasons.Progression);

            ScreenMessages.PostScreenMessage(Localizer.Format("#KSI_GRB_DiscoveryScienceMessage", delta), 5f, ScreenMessageStyle.UPPER_CENTER);

            return evt;
        }

        internal void LoadScenario(ConfigNode node)
        {
            events.Clear();
            var evtsNode = node.GetNode("GRBEvents");
            foreach (var nd in evtsNode.GetNodes("Event"))
            {
                var evt = new GRBEvent
                {
                    ImageUrl = nd.GetValue("image"),
                    Message = nd.GetValue("msg"),
                    UT = double.Parse(nd.GetValue("ut")),
                    RA = double.Parse(nd.GetValue("ra")),
                    Dec = double.Parse(nd.GetValue("dec"))
                };
                if (evt.UT > _latestEventTime)
                    _latestEventTime = evt.UT;
                events.Add(evt);
            }
            _loaded = true;
            Debug.Log($"[KSI]: loaded {events.Count} GRB events from scenario");
        }

    }
}
