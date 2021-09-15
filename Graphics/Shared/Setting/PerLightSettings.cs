using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using static Graphics.LightManager;

namespace Graphics
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class PerLightSettings
    {
        public bool Disabled { get; set; } = true;
        public bool UseAlloyLight { get; set; }
        public string LightName { get; set; }
        public Color Color { get; set; }
        public float ColorTemperature { get; set; }
        public int ShadowType { get; set; }
        public float ShadowStrength { get; set; }
        public int ShadowResolutionType { get; set; }
        public int ShadowResolutionCustom { get; set; }
        public float ShadowBias { get; set; }
        public float ShadowNormalBias { get; set; }
        public float ShadowNearPlane { get; set; }
        public float LightIntensity { get; set; }
        public float IndirectMultiplier { get; set; }
        public bool SegiSun { get; set; }
        public Vector3 Rotation { get; set; }
        public float Range { get; set; }
        public float SpotAngle { get; set; }
        public float Specular { get; set; }
        public float Length { get; set; }
        public int RenderMode { get; set; }
        public int CullingMask { get; set; }
        public PathElement HierarchyPath { get; set; }

        public int Type { get; set; }

        internal void ApplySettings(LightObject lightObject)
        {
            lightObject.enabled = !Disabled;            

            Graphics.Instance.LightManager.UseAlloyLight = UseAlloyLight;

            if (!string.IsNullOrEmpty(LightName)) lightObject.light.name = LightName;

            lightObject.color = Color;
            lightObject.light.colorTemperature = ColorTemperature;

            lightObject.shadows = (LightShadows)ShadowType;
            lightObject.light.shadowStrength = ShadowStrength;
            if (LightType.Directional == lightObject.type && Graphics.Instance.Settings.UsePCSS)
                lightObject.ShadowCustomResolution = ShadowResolutionCustom;
            else
                lightObject.light.shadowResolution = (LightShadowResolution)ShadowResolutionType;

            lightObject.light.shadowBias = ShadowBias;
            lightObject.light.shadowNormalBias = ShadowNormalBias;
            lightObject.light.shadowNearPlane = ShadowNearPlane;

            lightObject.intensity = LightIntensity;
            lightObject.light.bounceIntensity = IndirectMultiplier;
            if (SegiSun)
            {
                if (null != Graphics.Instance.CameraSettings.MainCamera)
                {
                    SEGI segi = Graphics.Instance.CameraSettings.MainCamera.GetComponent<SEGI>();
                    if (null != segi && segi.enabled)
                    {
                        segi.sun = lightObject.light;
                    }
                }
            }

            // Exclude Cam Light from rotation setting
            if (KKAPI.Studio.StudioAPI.InsideStudio && lightObject.light.name != "Cam Light" && !lightObject.light.transform.IsChildOf(GameObject.Find("StudioScene/Camera").transform))
                lightObject.rotation = Rotation;
            else if (!KKAPI.Studio.StudioAPI.InsideStudio && lightObject.light.name.StartsWith("(Graphics)"))
                lightObject.rotation = Rotation;


            lightObject.range = Range;
            lightObject.spotAngle = SpotAngle;
            if (Graphics.Instance.LightManager.UseAlloyLight)
            {
                AlloyAreaLight alloyLight = lightObject.light.GetComponent<AlloyAreaLight>();
                if (alloyLight != null)
                {
                    alloyLight.Radius = Specular;
                    alloyLight.Length = Length;
                }
            }
            lightObject.light.renderMode = (LightRenderMode)RenderMode;
            lightObject.light.cullingMask = CullingMask;
        }

        internal void FillSettings(LightObject lightObject)
        {
            Disabled = !lightObject.enabled;

            Type = (int)lightObject.type;

            UseAlloyLight = Graphics.Instance.LightManager.UseAlloyLight;

            LightName = lightObject.light.name;

            Color = lightObject.color;
            ColorTemperature = lightObject.light.colorTemperature;

            ShadowType = (int)lightObject.shadows;
            ShadowStrength = lightObject.light.shadowStrength;
            ShadowResolutionType = (int)lightObject.light.shadowResolution;
            ShadowResolutionCustom = lightObject.ShadowCustomResolution;
            ShadowBias = lightObject.light.shadowBias;
            ShadowNormalBias = lightObject.light.shadowNormalBias;
            ShadowNearPlane = lightObject.light.shadowNearPlane;

            LightIntensity = lightObject.intensity;
            IndirectMultiplier = lightObject.light.bounceIntensity;
            if (null != Graphics.Instance.CameraSettings.MainCamera)
            {
                SEGI segi = Graphics.Instance.CameraSettings.MainCamera.GetComponent<SEGI>();
                if (null != segi && segi.enabled)
                {
                    SegiSun = ReferenceEquals(lightObject.light, segi.sun);
                }
                else
                    SegiSun = false;
            }
            else
                SegiSun = false;

            Rotation = lightObject.rotation;

            Range = lightObject.range;
            SpotAngle = lightObject.spotAngle;
            if (Graphics.Instance.LightManager.UseAlloyLight)
            {
                AlloyAreaLight alloyLight = lightObject.light.GetComponent<AlloyAreaLight>();
                if (alloyLight != null)
                {
                    Specular = alloyLight.Radius;
                    Length = alloyLight.Length;
                }
                else
                {
                    Specular = 1.0f;
                    Length = 1.0f;
                }
            }
            else
            {
                Specular = 1.0f;
                Length = 1.0f;
            }

            RenderMode = (int)lightObject.light.renderMode;
            CullingMask = lightObject.light.cullingMask;

            HierarchyPath = PathElement.Build(lightObject.light.gameObject.transform);
        }        
    }
}