using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;

namespace KerbalScienceInnovation
{
    public class KSIGammaRayBurstDetector : PartModule
    {
        public override string GetInfo()
        {
            return Localizer.GetStringByTag("#KSI_GRBDetector_Info");
        }
    }
}
