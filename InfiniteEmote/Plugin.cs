using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace InfiniteEmote
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Texture2D texture;

        public static ManualLogSource StaticLogger;

        private void Awake()
        {
            StaticLogger = Logger;

            StopKeyConfig = Config.Bind<string>("Keybinds", "StopEmoteKey", "0", "SUPPORTED KEYS A-Z | 0-9 | F1-F12 ");
            Patches.StopEmoteKey = StopKeyConfig.Value;

            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll(typeof(Patches));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private ConfigEntry<string> StopKeyConfig;
    }
}