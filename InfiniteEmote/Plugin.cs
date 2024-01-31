using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;
using BepInEx.Bootstrap;
using System.Collections.Generic;

namespace InfiniteEmote
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("MoreEmotes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("BetterEmotes", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Texture2D texture;

        public static bool debug = false;

        public static new ManualLogSource Logger;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loading...");

            ConfigEntry<bool> configDebug = Config.Bind("Dev", "Debug", false, "Whether or not to enable debug logging and debug helpers");
            debug = configDebug.Value;
            Logger.LogInfo($"Debug enabled: {debug}");

            new Config(Config);

            Debug("Loading keybind defaults");
            ConfigEntry<string> configStopEmoteKey = Config.Bind("Keys", "Stop Emote Key", "<Keyboard>/minus", "Default keybind to stop emoting");
            Patches.stopEmoteKey = validatePrefixes(["<Keyboard>", "<Mouse>"], "<Keyboard>", configStopEmoteKey.Value);
            ConfigEntry<string> configStopEmoteController = Config.Bind("Keys", "Stop Emote Button", "<Gamepad>/leftStickPress", "Default controller button to stop emoting");
            Patches.stopEmoteController = validatePrefixes(["<Gamepad>"], "<Gamepad>", configStopEmoteController.Value);
            Debug($"Loaded key '{Patches.stopEmoteKey}'");
            Debug($"Loaded button '{Patches.stopEmoteController}'");

            checkForMods();
            Patches.keybinds = new Keybinds();
            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll(typeof(Patches));
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static string validatePrefixes(string[] prefixes, string defaultPrefix, string value)
        {
            if (value.Equals(""))
            {
                return $"";
            }
            foreach (string prefix in prefixes)
            {
                if (value.ToLower().StartsWith(prefix.ToLower()))
                {
                    return value;
                }
            }
            return $"{defaultPrefix}/{value}";
        }

        public static void Debug(string message)
        {
            if (debug) Logger.LogDebug(message);
        }

        public static void checkForMods()
        {
            Debug("checkForMods");
            foreach (KeyValuePair<string, BepInEx.PluginInfo> keyValuePair in Chainloader.PluginInfos)
            {
                BepInPlugin metadata = keyValuePair.Value.Metadata;
                if (metadata.GUID.Equals("MoreEmotes") || metadata.GUID.Equals("BetterEmotes"))
                {
                    Debug("Found MoreEmotes/BetterEmotes");
                    Patches.moreEmotes = true;
                    break;
                }
            }
        }
    }
}