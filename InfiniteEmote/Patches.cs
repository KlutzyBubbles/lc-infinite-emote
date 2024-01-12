using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using BepInEx.Bootstrap;
using BepInEx;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;

namespace InfiniteEmote
{
    internal class Patches
    {
        public static Keybinds keybinds;

        public static bool whileJumping;
        public static bool whileWalking;
        public static bool whileSprinting;
        public static bool whileCrouching;
        public static bool whileLadder;
        public static bool whileGrabbing;
        public static bool whileTyping;
        public static bool whileTerminal;
        public static bool whileHolding;
        public static bool whileHoldingTwoHand;

        public static string stopEmoteKey;
        public static string stopEmoteController;

        public static bool moreEmotes = false;

        private static PlayerControllerB localPlayerController
        {
            get
            {
                StartOfRound instance = StartOfRound.Instance;
                return (instance != null) ? instance.localPlayerController : null;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        private static void StartPostfix(PlayerControllerB __instance)
        {
            Plugin.Debug("PlayerControllerB.StartPostfix");
            keybinds.StopEmote.performed += onStopEmoteKey;
            keybinds.StopEmote.Enable();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisablePostfix(PlayerControllerB __instance)
        {
            Plugin.Debug("OnDisablePostfix");
            if (__instance == localPlayerController)
            {
                Plugin.Debug("localPlayerController is instance");
                keybinds.StopEmote.performed -= onStopEmoteKey;
                keybinds.StopEmote.Disable();
            }
        }

        private static void onStopEmoteKey(InputAction.CallbackContext context)
        {
            Plugin.Debug("onStopEmoteKey");
            if (localPlayerController.isPlayerControlled && localPlayerController.IsOwner)
            {
                Plugin.Debug("isPlayerControlled and IsOwner");
                localPlayerController.performingEmote = false;
                localPlayerController.StopPerformingEmoteServerRpc();
                localPlayerController.timeSinceStartingEmote = 0f;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
        [HarmonyPostfix]
        private static void CheckConditionsForEmotePatch(PlayerControllerB __instance, ref bool __result)
        {
            Plugin.Debug("CheckConditionsForEmotePatch");
            if (__instance.inSpecialInteractAnimation || __instance.isPlayerDead)
            {
                Plugin.Debug($"inSpecialInteractAnimation ({__instance.inSpecialInteractAnimation}) or isPlayerDead ({__instance.isPlayerDead})");
                __result = false;
                return;
            }
            if (__instance.isGrabbingObjectAnimation)
            {
                Plugin.Debug($"isGrabbingObjectAnimation ({__instance.isGrabbingObjectAnimation}), whileGrabbing ({whileGrabbing})");
                if (whileGrabbing)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                    return;
                }
            }
            if (__instance.isHoldingObject)
            {
                Plugin.Debug($"isHoldingObject ({__instance.isHoldingObject}), whileHolding ({whileHolding})");
                if (whileHolding)
                {
                    if (__instance.twoHanded)
                    {
                        Plugin.Debug($"twoHanded ({__instance.twoHanded}), whileHoldingTwoHand ({whileHoldingTwoHand})");
                        if (whileHoldingTwoHand)
                        {
                            __result = true;
                        }
                        else
                        {
                            __result = false;
                            return;
                        }
                    }
                    else
                    {
                        __result = true;
                    }
                }
                else
                {
                    __result = false;
                    return;
                }
            }
            if (__instance.isJumping)
            {
                Plugin.Debug($"isJumping, whileJumping ({whileJumping})");
                __result = whileJumping;
            }
            if (__instance.isWalking)
            {
                Plugin.Debug($"isWalking, whileWalking ({whileWalking})");
                __result = whileWalking;
            }
            if (__instance.isSprinting)
            {
                Plugin.Debug($"isSprinting, whileSprinting ({whileSprinting})");
                __result = whileSprinting;
            }
            if (__instance.isCrouching)
            {
                Plugin.Debug($"isCrouching, whileCrouching ({whileCrouching})");
                __result = whileCrouching;
            }
            if (__instance.isClimbingLadder)
            {
                Plugin.Debug($"isClimbingLadder, whileLadder ({whileLadder})");
                __result = whileLadder;
            }
            if (__instance.isTypingChat)
            {
                int currentEmote = __instance.playerBodyAnimator.GetInteger("emoteNumber");
                Plugin.Debug($"isTypingChat, whileTyping ({whileTyping}), moreEmotes ({moreEmotes}), currentEmote ({currentEmote})");
                if (moreEmotes && currentEmote == 10)
                {
                    Plugin.Debug($"isEmoteNumber 10 (Sign for MoreEmotes)");
                    __result = true;
                }
                else
                {
                    __result = whileTyping;
                }
            }
            if (__instance.inTerminalMenu)
            {
                Plugin.Debug($"inTerminalMenu, whileTerminal ({whileTerminal})");
                __result = whileTerminal;
            }
            if (__result == false)
            {
                Plugin.Debug($"__result false, following values");
                Plugin.Debug($"!__instance.inSpecialInteractAnimation ({!__instance.inSpecialInteractAnimation})");
                Plugin.Debug($"!__instance.isPlayerDead ({!__instance.isPlayerDead})");
                Plugin.Debug($"!__instance.isJumping ({!__instance.isJumping})");
                Plugin.Debug($"!__instance.isWalking ({!__instance.isWalking})");
                Plugin.Debug($"!__instance.isCrouching ({!__instance.isCrouching})");
                Plugin.Debug($"!__instance.isClimbingLadder ({!__instance.isClimbingLadder})");
                Plugin.Debug($"!__instance.isGrabbingObjectAnimation ({!__instance.isGrabbingObjectAnimation})");
                Plugin.Debug($"!__instance.inTerminalMenu ({!__instance.inTerminalMenu})");
                Plugin.Debug($"!__instance.isTypingChat ({!__instance.isTypingChat})");
                bool result = !__instance.inSpecialInteractAnimation && !__instance.isPlayerDead && !__instance.isJumping && !__instance.isWalking && !__instance.isCrouching && !__instance.isClimbingLadder && !__instance.isGrabbingObjectAnimation && !__instance.inTerminalMenu && !__instance.isTypingChat;
                Plugin.Debug($"result ({result})");
                __result = result;
            }
            Plugin.Debug("--------------------");
            return;
        }

        [HarmonyPatch(typeof(InitializeGame), "Start")]
        [HarmonyPrefix]
        private static void StartPostfix()
        {
            Plugin.Debug("InitializeGame.StartPostfix");
            foreach (KeyValuePair<string, BepInEx.PluginInfo> keyValuePair in Chainloader.PluginInfos)
            {
                BepInPlugin metadata = keyValuePair.Value.Metadata;
                if (metadata.GUID.Equals("MoreEmotes"))
                {
                    Plugin.Debug("Found MoreEmotes");
                    moreEmotes = true;
                    break;
                }
            }
        }
    }
}
