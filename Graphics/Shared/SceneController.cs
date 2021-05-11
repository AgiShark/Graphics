using ExtensibleSaveFormat;
using Graphics.Settings;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System.Linq;
using UnityEngine;

namespace Graphics
{
    internal class SceneController : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            Studio.Studio studio = GetStudio();
            Graphics parent = Graphics.Instance;
            PluginData pluginData = GetExtendedData();

            parent?.PresetManager?.Load(pluginData);

            // load scene reflection probe information
            if (pluginData != null && pluginData.data != null && pluginData.data.ContainsKey("containsDefaultReflectionProbeData") && (bool)pluginData.data["containsDefaultReflectionProbeData"])
            {
                Graphics.Instance.SkyboxManager.SetupDefaultReflectionProbe(Graphics.Instance.LightingSettings, true);
            }

            if (pluginData != null && pluginData.data != null && pluginData.data.ContainsKey("reflectionProbeBytes"))
            {
                ReflectionProbeSettings[] settings = MessagePackSerializer.Deserialize<ReflectionProbeSettings[]>((byte[])pluginData.data["reflectionProbeBytes"]);
                if (settings != null && settings.Length > 0)
                    ApplyReflectionProbeSettings(settings);
            }
        }

        protected override void OnSceneSave()
        {
            PluginData pluginData = Graphics.Instance?.PresetManager.GetExtendedData();

            if (pluginData != null)
            {
                // add scene reflection probe information
                pluginData.data.Add("reflectionProbeBytes", MessagePackSerializer.Serialize(BuildReflectionProbeSettings()));
                if (Graphics.Instance.SkyboxManager.DefaultReflectionProbe() != null && Graphics.Instance.SkyboxManager.DefaultReflectionProbe().intensity > 0)
                {
                    pluginData.data.Add("containsDefaultReflectionProbeData", true);
                }
            }

            SetExtendedData(pluginData);
        }

        private void ApplyReflectionProbeSettings(ReflectionProbeSettings[] settings)
        {
            string[] probeNames = settings.Select(s => s.Name).ToArray();

            ReflectionProbe[] probes = Graphics.Instance.SkyboxManager.GetReflectinProbes();
            if (probes != null)
            {
                foreach (string probeName in probeNames)
                {
                    int counter = 0;
                    foreach (ReflectionProbeSettings setting in settings.Where(s => s.Name == probeName).ToList())
                    {
                        if (probes.Where(p => p.name == probeName).Skip(counter).Any())
                        {
                            ReflectionProbe probe = probes.Where(p => p.name == probeName).Skip(counter).First();
                            setting.ApplySettings(probe);
                            counter++;
                        }
                    }
                }
            }
        }

        private ReflectionProbeSettings[] BuildReflectionProbeSettings()
        {
            ReflectionProbe[] probes = Graphics.Instance.SkyboxManager.GetReflectinProbes();
            if (probes.Length > 0)
            {
                ReflectionProbeSettings[] settings = new ReflectionProbeSettings[probes.Length];
                for (int i = 0; i < probes.Length; i++)
                {
                    settings[i] = new ReflectionProbeSettings();
                    settings[i].FillSettings(probes[i]);
                }
                return settings;
            }
            return null;
        }
    }
}
