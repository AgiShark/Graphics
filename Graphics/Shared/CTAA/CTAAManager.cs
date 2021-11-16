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
     //       if (Graphics.Instance.CameraSettings.MainCamera.stereoEnabled)
     //       {
     //           CTaaSettings.Load(Graphics.Instance.CameraSettings.MainCamera.GetComponent<CTAAVR_VIVE>());
     //       }
     //       else
     //       {
                CTaaSettings.SwitchMode(CTaaSettings.Mode, true);
     //       }
        }
    }
}
