using Graphics.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Graphics.CTAA
{
    public class CTAAManager
    {
        public static CTAASettings CTaaSettings { get; set; } = new CTAASettings();

        public static void ApplySetting()
        {
            if (CTaaSettings.Enabled)
                CTaaSettings.SwitchMode(CTaaSettings.Mode, true);
        }
    }
}
