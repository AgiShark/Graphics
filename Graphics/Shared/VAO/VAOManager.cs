using Graphics.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Graphics.VAO
{
    public class VAOManager
    {
        public static Settings.VAOSettings settings;

        internal static VAOEffectCommandBuffer VAOInstance;
        private static List<VAOEffectCommandBuffer> otherVAOInstances = new List<VAOEffectCommandBuffer>();

        // Initialize Components
        internal void Initialize()
        {
            VAOInstance = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<VAOEffect>();
            Graphics.Instance.Log.LogInfo($"Adding VAO to {Graphics.Instance.CameraSettings.MainCamera.name}");
            if (settings == null)
                settings = new VAOSettings();

            settings.Load(VAOInstance);
            CopySettingsToOtherInstances();
        }

        // When user enabled the option
        internal void Start()
        {
        }

        public static void RegisterAdditionalInstance(VAOEffect otherInstance)
        {
            if (!otherVAOInstances.Contains(otherInstance))
            {
                otherVAOInstances.Add(otherInstance);
                VAOManager.CopySettingsToOtherInstances();
            }
        }
        public static void UpdateSettings()
        {
            if (settings == null)
                settings = new VAOSettings();
            if (VAOInstance != null)
                settings.Load(VAOInstance);

            CopySettingsToOtherInstances();
        }

        internal static void CopySettingsToOtherInstances()
        {
            foreach (VAOEffect otherInstance in otherVAOInstances)
            {
                settings.Load(otherInstance);
            }
        }

        // When user disabled the option
        internal void Destroy()
        {
            DestroyVAOInstance(VAOInstance);
            for (int i = otherVAOInstances.Count - 1; i >= 0; i--)
            {
                VAOEffectCommandBuffer otherInstance = otherVAOInstances[i];
                if (otherInstance == null)
                {
                }
                else
                {
                    otherInstance.enabled = VAOInstance.enabled;
                    DestroyVAOInstance(otherInstance);

                }
            }
        }

        public static void DestroyVAOInstance(VAOEffectCommandBuffer VAOInstance)
        {
            otherVAOInstances.Remove(VAOInstance);
        }

        IEnumerator WaitForCamera()
        {
            Camera camera = Graphics.Instance.CameraSettings.MainCamera;
            yield return new WaitUntil(() => camera != null);
            CheckInstance();
        }
        public void CheckInstance()
        {
            Camera camera = Graphics.Instance.CameraSettings.MainCamera;
            if (camera.GetComponent<VAOEffect>() == null && camera.GetComponent<VAOEffectCommandBuffer>() == null)
            {
                Graphics.Instance.Log.LogInfo($"Adding VAO to {camera.name}");
                VAOInstance = camera.GetOrAddComponent<VAOEffect>();
                UpdateSettings();
            }
        }
    }
}
