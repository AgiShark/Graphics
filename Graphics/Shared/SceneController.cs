using ExtensibleSaveFormat;
using Graphics.Settings;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using MessagePack;
using Studio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Graphics.LightManager;

namespace Graphics
{
    public class SceneController : SceneCustomFunctionController
    {
        public byte[] ExportSettingBytes()
        {
            return MessagePackSerializer.Serialize(DoSave());            
        }

        public void ImportSettingBytes(byte[] bytes)
        {
            DoLoad(MessagePackSerializer.Deserialize<PluginData>(bytes));
        }

        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            PluginData pluginData = GetExtendedData();
            DoLoad(pluginData);
        }        

        private void DoLoad(PluginData pluginData)
        {
            Studio.Studio studio = GetStudio();
            Graphics parent = Graphics.Instance;
            parent?.PresetManager?.Load(pluginData);

            if (pluginData != null && pluginData.data != null && pluginData.data.ContainsKey("reflectionProbeBytes"))
            {
                ReflectionProbeSettings[] settings = MessagePackSerializer.Deserialize<ReflectionProbeSettings[]>((byte[])pluginData.data["reflectionProbeBytes"]);
                if (settings != null && settings.Length > 0)
                    ApplyReflectionProbeSettings(settings);
            }

            // load scene reflection probe information
            if (pluginData != null && pluginData.data != null && pluginData.data.ContainsKey("containsDefaultReflectionProbeData") && (bool)pluginData.data["containsDefaultReflectionProbeData"])
            {
                Graphics.Instance.SkyboxManager.SetupDefaultReflectionProbe(Graphics.Instance.LightingSettings, true);
            }
            else
            {
                Graphics.Instance.SkyboxManager.SetupDefaultReflectionProbe(Graphics.Instance.LightingSettings, false);
            }

            if (pluginData != null && pluginData.data != null && pluginData.data.ContainsKey("lightDataBytes"))
            {
                PerLightSettings[] settings = MessagePackSerializer.Deserialize<PerLightSettings[]>((byte[])pluginData.data["lightDataBytes"]);
               if (settings != null && settings.Length > 0)
                    ApplyLightSettings(settings);
            }            
        }

        protected override void OnSceneSave()
        {           
            SetExtendedData(DoSave());
        }

        private PluginData DoSave()
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

                // add per light settings data
                pluginData.data.Add("lightDataBytes", MessagePackSerializer.Serialize(BuildLightSettings()));
            }

            return pluginData;
        }

        public static void ApplyLightSettings(PerLightSettings[] settings)
        {
            LightManager lightManager = Graphics.Instance.LightManager;
            lightManager.Light();
            int counter = 0;

            List<PerLightSettings> newDirectionalLights = new List<PerLightSettings>(settings.Where(pls => pls.Type == (int)LightType.Directional));

            if (settings.Length > 0 && settings[0].HierarchyPath != null)
            {
                foreach (LightObject light in lightManager.DirectionalLights)
                {
                    PerLightSettings setting = settings.FirstOrDefault(s => s.HierarchyPath.Matches(light.light.gameObject.transform));
                    if (setting != null)
                    {
                        setting.ApplySettings(light);
                        newDirectionalLights.Remove(setting);
                    }
                }

                foreach (LightObject light in lightManager.PointLights)
                {
                    PerLightSettings setting = settings.FirstOrDefault(s => s.HierarchyPath.Matches(light.light.gameObject.transform));
                    if (setting != null)
                        setting.ApplySettings(light);
                }

                foreach (LightObject light in lightManager.SpotLights)
                {
                    PerLightSettings setting = settings.FirstOrDefault(s => s.HierarchyPath.Matches(light.light.gameObject.transform));
                    if (setting != null)
                        setting.ApplySettings(light);
                }
            }
            else
            {
                foreach (LightObject light in lightManager.DirectionalLights)
                {
                    settings[counter++].ApplySettings(light);
                    if (counter >= settings.Length)
                        return;
                }

                foreach (LightObject light in lightManager.PointLights)
                {
                    settings[counter++].ApplySettings(light);
                    if (counter >= settings.Length)
                        return;
                }

                foreach (LightObject light in lightManager.SpotLights)
                {
                    settings[counter++].ApplySettings(light);
                    if (counter >= settings.Length)
                        return;
                }
            }

            if (!KKAPI.Studio.StudioAPI.InsideStudio && newDirectionalLights.Count > 0)
            {
                // Add extra lights if requested
                foreach (PerLightSettings setting in newDirectionalLights)
                {
                    GameObject lightGameObject = new GameObject(setting.LightName);
                    Light lightComp = lightGameObject.AddComponent<Light>();
                    lightGameObject.GetComponent<Light>().type = LightType.Directional;      
                }
                lightManager.Light();
                foreach (LightObject light in lightManager.DirectionalLights)
                {
                    PerLightSettings setting = settings.FirstOrDefault(s => s.HierarchyPath.Matches(light.light.gameObject.transform));
                    if (setting != null)
                    {
                        setting.ApplySettings(light);
                    }
                }
            }
        }

        public static PerLightSettings[] BuildLightSettings()
        {
            LightManager lightManager = Graphics.Instance.LightManager;
            lightManager.Light();
            PerLightSettings[] settings = new PerLightSettings[lightManager.DirectionalLights.Count + lightManager.PointLights.Count + lightManager.SpotLights.Count];
            int counter = 0;

            foreach (LightObject light in lightManager.DirectionalLights)
            {
                PerLightSettings setting = new PerLightSettings();
                setting.FillSettings(light);
                settings[counter++] = setting;
            }

            foreach (LightObject light in lightManager.PointLights)
            {
                PerLightSettings setting = new PerLightSettings();
                setting.FillSettings(light);
                settings[counter++] = setting;
            }

            foreach (LightObject light in lightManager.SpotLights)
            {
                PerLightSettings setting = new PerLightSettings();
                setting.FillSettings(light);
                settings[counter++] = setting;
            }

            return settings;
        }

        public static void ApplyReflectionProbeSettings(ReflectionProbeSettings[] settings)
        {

            ReflectionProbe[] probes = Graphics.Instance.SkyboxManager.GetReflectinProbes();
            if (probes != null && settings != null)
            {
                if (settings.Length > 0 && settings[0].HierarchyPath == null) 
                {
                    string[] probeNames = settings.Select(s => s.Name).ToArray();
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
                else
                {
                    foreach (ReflectionProbe probe in probes)
                    {
                        ReflectionProbeSettings setting = settings.FirstOrDefault(s => s.HierarchyPath.Matches(probe.gameObject.transform));
                        if (setting != null)
                            setting.ApplySettings(probe);
                    }
                }
            }
        }

        public static ReflectionProbeSettings[] BuildReflectionProbeSettings()
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
