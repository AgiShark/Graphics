using System;
using System.Collections.Generic;
using System.Text;
using KKAPI;

namespace Graphics
{
    public enum PresetDefaultType
    {
        MAIN_GAME,
        TITLE,
        MAKER,
        VR_GAME,
        STUDIO
    }

    static class PresetDefaultTypeUtils
    {
        internal static PresetDefaultType ForGameMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.MainGame:
                    return PresetDefaultType.MAIN_GAME;
                case GameMode.Maker:
                    return PresetDefaultType.MAKER;
                case GameMode.Studio:
                    return PresetDefaultType.STUDIO;
                case GameMode.Unknown:  
                    return PresetDefaultType.MAIN_GAME;
                default:
                    return PresetDefaultType.MAIN_GAME;
            }
        }
    }
}
