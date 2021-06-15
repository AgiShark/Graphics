# Graphics
Graphics settings plugin for AIGirl and HoneySelect2

Inspired and heavily influenced by PHIBL.

Please refer to unity's [post processing stack manual](https://docs.unity3d.com/2018.2/Documentation/Manual/PostProcessingOverview.html) for detailed description of the settings. [日本語](https://docs.unity3d.com/ja/2018.2/Manual/PostProcessingOverview.html)

Similar to PHIBL, Graphics uses cubemaps for imaged based lighting (IBL). Some sample cubemaps available [here](https://mega.nz/#F!PEMRkASB!I0ZTv4OgV-mSxX07MWDMQw). Put them in a folder named "cubemaps" at the root of the installation folder. You can configure the path for them with [Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

# Note from OrangeSpork

While Hooh labs works on the proper overhaul of this mod, the version of HS2Graphics I've been packaging with the VR plugin has picked up a bunch of...non-VR related stuff in it. I'm finally breaking down and moving the release out of the VR plugin over into the proper repo. This is just an update to the link to the release place going forward.

This is totally optional, if you don't know why you want this, you can just stay on what you've got, no issues.

Installation: Remove any existing Graphics.dll or .dl_, unzip the zip to game root. If you have a recent repack, InitSettings will pick up the new HS2Graphics.dll version and let you control it normally via the checkbox.

Defaults: It comes with default presets, if you don't care for the out of the box ones, just load the preset you want and/or fiddle with settings and then hit the 'Save current as default' button on the appropriate default type (Main Game/Maker/Studio/Title/VR) on the presets tab. 

Major changes list:

- Default Presets - Graphics fires up on the title screen now and auto-loads the default preset when you jump between scenes (especially handy in Main Game)
- SSS fixes and improvements (mirrors, phone cams, etc), profile per object implemented, culling layer implemented, etc.
- Fix to prevent graphics window spawning off screen
- Fix to allow VNGE and Graphics options window to both be up at the same time
- Studio toolbar icon
- Lights and Reflection Probe settings save and load along with scenes
- Pulsed Realtime Reflection Probes setting - Updates realtime reflection probes every N seconds, benefits of realtime probes with less overall performance impact.
- Numerous small bug fixes

## Attributions
[Alloy](https://github.com/Josh015/Alloy)  
[SEGI](https://github.com/sonicether/SEGI)  
[PCSS](https://github.com/TheMasonX/UnityPCSS)  
[keijiro/PostProcessingUtilities](https://github.com/keijiro/PostProcessingUtilities)  
[IllusionModdingAPI](https://github.com/IllusionMods/IllusionModdingAPI)
