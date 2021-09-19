using Graphics.GTAO;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using static Graphics.GTAO.GroundTruthAmbientOcclusion;

namespace Graphics.Settings
{
    [MessagePackObject(true)]
    public class GTAOSettings
    {
        public bool Enabled = false;
        public IntValue DirSampler = new IntValue(2, false);
        public IntValue SliceSampler = new IntValue(2, false);
        public FloatValue Radius = new FloatValue(2.5f, false);
        public FloatValue Intensity = new FloatValue(1, false);
        public FloatValue Power = new FloatValue(2.5f, false);
        public BoolValue MultiBounce = new BoolValue(true, false);
        public FloatValue Sharpeness = new FloatValue(0.25f, false);
        public FloatValue TemporalScale = new FloatValue(1, false);
        public FloatValue TemporalResponse = new FloatValue(1, false);
        public IntValue AODeBug = new IntValue((int)OutPass.Combien, false);

        public void Load(GroundTruthAmbientOcclusion gtao)
        {
            if (gtao == null)
                return;

            gtao.enabled = Enabled;
            if (DirSampler.overrideState)
                gtao.DirSampler = DirSampler.value;
            else
                gtao.DirSampler = 2;

            if (SliceSampler.overrideState)
                gtao.SliceSampler = SliceSampler.value;
            else
                gtao.SliceSampler = 2;

            if (Radius.overrideState)
                gtao.Radius = Radius.value;
            else
                gtao.Radius = 2.5f;

            if (Intensity.overrideState)
                gtao.Intensity = Intensity.value;
            else
                gtao.Intensity = 1f;

            if (Power.overrideState)
                gtao.Power = Power.value;
            else
                gtao.Power = 2.5f;

            if (MultiBounce.overrideState)
                gtao.MultiBounce = MultiBounce.value;
            else
                gtao.MultiBounce = true;

            if (Sharpeness.overrideState)
                gtao.Sharpeness = Sharpeness.value;
            else
                gtao.Sharpeness = 0.25f;

            if (TemporalScale.overrideState)
                gtao.TemporalScale = TemporalScale.value;
            else
                gtao.TemporalScale = 1;

            if (TemporalResponse.overrideState)
                gtao.TemporalResponse = TemporalResponse.value;
            else
                gtao.TemporalResponse = 1;

            if (AODeBug.overrideState)
                gtao.AODeBug = (OutPass)AODeBug.value;
            else
                gtao.AODeBug = OutPass.Combien;
        }

        public void Save(GroundTruthAmbientOcclusion gtao)
        {
            if (gtao == null)
                return;

            Enabled = gtao.enabled;
            DirSampler.value = gtao.DirSampler;
            SliceSampler.value = gtao.SliceSampler;
            Radius.value = gtao.Radius;
            Intensity.value = gtao.Intensity;
            Power.value = gtao.Power;
            MultiBounce.value = gtao.MultiBounce;
            Sharpeness.value = gtao.Sharpeness;
            TemporalScale.value = gtao.TemporalScale;
            TemporalResponse.value = gtao.TemporalResponse;
            AODeBug.value = (int)gtao.AODeBug;
        }
    }
}
