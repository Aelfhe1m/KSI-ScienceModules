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
            return evt;
        }

        internal void LoadScenario(ConfigNode node)
        {
            events.Clear();
            var evtsNode = node.GetNode("GRBEvents");
            foreach (var evt in evtsNode.GetNodes("Event"))
            {
                events.Add(new GRBEvent {
                    ImageUrl = evt.GetValue("image"),
                    Message = evt.GetValue("msg"),
                    UT = double.Parse(evt.GetValue("ut")),
                    RA = double.Parse(evt.GetValue("ra")),
                    Dec = double.Parse(evt.GetValue("dec"))
                });
            }
            _loaded = true;
            Debug.Log($"[KSI]: loaded {events.Count} GRB events from scenario");
        }

    }
}
