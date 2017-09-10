using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;

namespace KerbalScienceInnovation
{
    public class KSIParameters : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomIntParameterUI("#KSI_Setting_EnableVerboseLogging", autoPersistance = true)]
        public bool verboseLogging = false;

        public override string Title 
            => Localizer.GetStringByTag("#KSI_Setting_Title"); // Kerbal Science Innovations;

        public override string DisplaySection 
            => Localizer.GetStringByTag("#KSI_Setting_DisplaySection"); // KSI General Settings

        public override string Section => "KSI";

        public override int SectionOrder => 1;

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        // TODO: Add presets for game difficulties
        public override bool HasPresets => false;
    }


}
