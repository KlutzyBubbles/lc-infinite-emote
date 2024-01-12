using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using BepInEx.Configuration;

namespace InfiniteEmote
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public static Texture2D texture;

        public static bool debug = false;

        public static ManualLogSource StaticLogger;

        private void Awake()
        {
            StaticLogger = Logger;
            StaticLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} loading...");

            ConfigEntry<bool> configDebug = Config.Bind("Dev", "Debug", false, "Whether or not to enable debug logging and debug helpers");
            debug = configDebug.Value;
            StaticLogger.LogInfo($"Debug enabled: {debug}");

            Debug("Loading keybind defaults");
            ConfigEntry<string> configStopEmoteKey = Config.Bind("Keys", "Stop Emote Key", "<Keyboard>/minus", "Default keybind to stop emoting");
            Patches.stopEmoteKey = configStopEmoteKey.Value.Equals("") ? "" : (configStopEmoteKey.Value.ToLower().StartsWith("<keyboard>") ? configStopEmoteKey.Value : $"<Keyboard>/{configStopEmoteKey.Value}");
            ConfigEntry<string> configStopEmoteController = Config.Bind("Keys", "Stop Emote Button", "<Gamepad>/leftStickPress", "Default controller button to stop emoting");
            Patches.stopEmoteController = configStopEmoteController.Value.Equals("") ? "" : (configStopEmoteController.Value.ToLower().StartsWith("<gamepad>") ? configStopEmoteController.Value : $"<Gamepad>/{configStopEmoteController.Value}");
            Debug($"Loaded key '{Patches.stopEmoteKey}'");
            Debug($"Loaded button '{Patches.stopEmoteController}'");

            Debug("Loading emote while options");
            ConfigEntry<bool> whileJumpingConfig = Config.Bind<bool>("Emote while", "Jumping", true, "Whether or not to allow emoting while Jumping");
            Patches.whileJumping = whileJumpingConfig.Value;
            ConfigEntry<bool> whileWalkingConfig = Config.Bind<bool>("Emote while", "Walking", true, "Whether or not to allow emoting while Walking");
            Patches.whileWalking = whileWalkingConfig.Value;
            ConfigEntry<bool> whileSprintingConfig = Config.Bind<bool>("Emote while", "Sprinting", true, "Whether or not to allow emoting while Sprinting");
            Patches.whileSprinting = whileSprintingConfig.Value;
            ConfigEntry<bool> whileCrouchingConfig = Config.Bind<bool>("Emote while", "Crouching", false, "Whether or not to allow emoting while Crouching");
            Patches.whileCrouching = whileCrouchingConfig.Value;
            ConfigEntry<bool> whileLadderConfig = Config.Bind<bool>("Emote while", "Ladder", false, "Whether or not to allow emoting while climbing Ladder");
            Patches.whileLadder = whileLadderConfig.Value;
            ConfigEntry<bool> whileGrabbingConfig = Config.Bind<bool>("Emote while", "Grabbing", false, "Whether or not to allow emoting while Grabbing");
            Patches.whileGrabbing = whileGrabbingConfig.Value;
            ConfigEntry<bool> whileTypingConfig = Config.Bind<bool>("Emote while", "Typing", false, "Whether or not to allow emoting while Typing");
            Patches.whileTyping = whileTypingConfig.Value;
            ConfigEntry<bool> whileTerminalConfig = Config.Bind<bool>("Emote while", "Terminal", false, "Whether or not to allow emoting while in the Terminal");
            Patches.whileTerminal = whileTerminalConfig.Value;
            ConfigEntry<bool> whileHoldingConfig = Config.Bind<bool>("Emote while", "Holding", true, "Whether or not to allow emoting while Holding an object");
            Patches.whileHolding = whileHoldingConfig.Value;
            ConfigEntry<bool> whileHoldingTwoHandConfig = Config.Bind<bool>("Emote while", "HoldingTwoHand", true, "Whether or not to allow emoting while Holding a two handed object");
            Patches.whileHoldingTwoHand = whileHoldingTwoHandConfig.Value;
            Debug($"Loaded whileJumping '{Patches.whileJumping}'");
            Debug($"Loaded whileWalking '{Patches.whileWalking}'");
            Debug($"Loaded whileSprinting '{Patches.whileSprinting}'");
            Debug($"Loaded whileCrouching '{Patches.whileCrouching}'");
            Debug($"Loaded whileLadder '{Patches.whileLadder}'");
            Debug($"Loaded whileGrabbing '{Patches.whileGrabbing}'");
            Debug($"Loaded whileTyping '{Patches.whileTyping}'");
            Debug($"Loaded whileTerminal '{Patches.whileTerminal}'");
            Debug($"Loaded whileHoldingTwoHand '{Patches.whileHoldingTwoHand}'");

            Patches.keybinds = new Keybinds();
            new Harmony(PluginInfo.PLUGIN_GUID).PatchAll(typeof(Patches));
            StaticLogger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }
        public static void Debug(string message)
        {
            if (debug) StaticLogger.LogDebug(message);
        }
    }
}