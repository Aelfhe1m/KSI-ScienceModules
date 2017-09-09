using KSP.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalScienceInnovation
{
    class KSIParametersGRB : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomFloatParameterUI("#KSI_Setting_GRB_Frequency", autoPersistance = true, minValue = 0.1f, maxValue = 10f, toolTip = "#KSI_Settings_GRB_Frequency_ToolTip" )]
        public float GRBFequency = 1f;

        //[GameParameters.CustomIntParameterUI("#KSI_Settings_GRB_KACAlarms", autoPersistance = true, toolTip = "KSI_Settings_GRB_KACAlarms_ToolTip")]
        //public bool enableKACAlarms = true;

        public override string Title 
            => Localizer.GetStringByTag("#KSI_Setting_GRB_Title"); // Kerbal Science Innovations;

        public override string DisplaySection 
            => Localizer.GetStringByTag("#KSI_Setting_DisplaySection"); // Gamma Ray Bursts

        public override string Section => "KSI";

        public override int SectionOrder => 2;

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.SCIENCE | GameParameters.GameMode.CAREER;

        // TODO: Add presets for game difficulties
        public override bool HasPresets => false;
    }
}
