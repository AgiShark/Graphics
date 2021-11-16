using Graphics.CTAA;
using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Graphics.Settings
{
    [MessagePackObject(true)]
    public class CTAASettings
    {
        public bool Enabled = false;
        public IntValue TemporalStability = new IntValue(6, false);
        public FloatValue HdrResponse = new FloatValue(1.2f, false);
        public FloatValue EdgeResponse = new FloatValue(0.5f, false);
        public FloatValue AdaptiveSharpness = new FloatValue(0.2f, false);
        public FloatValue TemporalJitterScale = new FloatValue(0.475f, false);
        public FloatValue VRTemporalEdgePower = new FloatValue(2.0f, false);
        public CTAA_MODE Mode = CTAA_MODE.STANDARD;

        public enum CTAA_MODE
        {
            STANDARD = 0,
            CINA_SOFT = 1,
            CINA_ULTRA = 2

        }

        private bool runningCoroutine = false;
        public void SwitchMode(CTAA_MODE newMode, bool force = false)
        {
            if (Mode != newMode || force)
            {
                Mode = newMode;
                runningCoroutine = true;
                Graphics.Instance.StartCoroutine(DoSwitchMode());
            }
        }

        public IEnumerator DoSwitchMode()
        {            
            CTAA_PC ctaa = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<CTAA_PC>();
            if (ctaa != null && ctaa.SuperSampleMode != (int)Mode)
            {
                GameObject.DestroyImmediate(ctaa);
                yield return null;
                ctaa = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<CTAA_PC>();
                ctaa.SuperSampleMode = (int)Mode;
                ctaa.ExtendedFeatures = Mode != CTAA_MODE.STANDARD;
                DoLoad(ctaa);
                yield return null;
                ctaa.enabled = false;
                yield return null;
                ctaa.enabled = true;

                Graphics.Instance.Log.LogInfo($"Switching CTAA Mode to {Mode}");
                runningCoroutine = false;
            }
        }

        public void Load(CTAAVR_VIVE vrCtaa)
        {
            if (vrCtaa == null && !Enabled)
            {
                return;
            }
            else if (vrCtaa == null)
            {
                CTAAVR_Velocity_OPENVR vrCtaaVelocity = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<CTAAVR_Velocity_OPENVR>();
                vrCtaa = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<CTAAVR_VIVE>();
                if (vrCtaa != null)
                {
                    DoLoad(vrCtaa);
                    Graphics.Instance.Log.LogInfo($"Enabling VR CTAA");
                }
                return;
            }
            DoLoad(vrCtaa);
        }

        public void Load(CTAA_PC ctaa)
        {            
            if (runningCoroutine)
                return;

            if (ctaa == null && !Enabled)
            {
                return;
            }
            else if (ctaa == null)
            {
                SwitchMode(Mode, true);
                return;
            }

            DoLoad(ctaa);
        }

        private void DoLoad(CTAAVR_VIVE vrCtaa)
        {
            vrCtaa.enabled = Enabled;
            vrCtaa.CTAA_Enabled = Enabled;

            if (VRTemporalEdgePower.overrideState)
                vrCtaa.TemporalEdgePower = VRTemporalEdgePower.value;
            else
                vrCtaa.TemporalEdgePower = 2.0f;

            if (TemporalJitterScale.overrideState)
                vrCtaa.TemporalJitterScale = TemporalJitterScale.value;
            else
                vrCtaa.TemporalJitterScale = 0.25f;

            if (AdaptiveSharpness.overrideState)
                vrCtaa.AdaptiveSharpness = AdaptiveSharpness.value;
            else
                vrCtaa.AdaptiveSharpness = 0.33f;


        }

        private void DoLoad(CTAA_PC ctaa)
        { 
            ctaa.enabled = Enabled;
            ctaa.CTAA_Enabled = Enabled;

            if (TemporalStability.overrideState)
                ctaa.TemporalStability = TemporalStability.value;
            else
                ctaa.TemporalStability = 6;

            if (HdrResponse.overrideState)
                ctaa.HdrResponse = HdrResponse.value;
            else
                ctaa.HdrResponse = 1.2f;

            if (EdgeResponse.overrideState)
                ctaa.EdgeResponse = EdgeResponse.value;
            else
                ctaa.EdgeResponse = 0.5f;

            if (AdaptiveSharpness.overrideState)
                ctaa.AdaptiveSharpness = AdaptiveSharpness.value;
            else
                ctaa.AdaptiveSharpness = 0.2f;

            if (TemporalJitterScale.overrideState)
                ctaa.TemporalJitterScale = TemporalJitterScale.value;
            else
                ctaa.TemporalJitterScale = 0.475f;            
        }
    }
}
