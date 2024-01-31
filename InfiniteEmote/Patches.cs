using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine.InputSystem;
using System;

namespace InfiniteEmote
{
    internal class Patches
    {
        public static Keybinds keybinds;

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
        [HarmonyAfter(["BetterEmotes", "MoreEmotes"])]
        private static void CheckConditionsForEmotePatch(PlayerControllerB __instance, ref bool __result)
        {
            if (__instance == null)
            {
                return;
            }
            if (__instance.inSpecialInteractAnimation || __instance.isPlayerDead)
            {
                Plugin.Debug($"inSpecialInteractAnimation ({__instance.inSpecialInteractAnimation}) or isPlayerDead ({__instance.isPlayerDead})");
                __result = false;
                return;
            }
            if (__instance.isGrabbingObjectAnimation)
            {
                Plugin.Debug($"isGrabbingObjectAnimation ({__instance.isGrabbingObjectAnimation}), whileGrabbing ({Config.Instance.WhileGrabbing})");
                if (Config.Instance.WhileGrabbing)
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
                Plugin.Debug($"isHoldingObject ({__instance.isHoldingObject}), whileHolding ({Config.Instance.WhileHolding})");
                if (Config.Instance.WhileHolding)
                {
                    if (__instance.twoHanded)
                    {
                        Plugin.Debug($"twoHanded ({__instance.twoHanded}), whileHoldingTwoHand ({Config.Instance.WhileHoldingTwoHand})");
                        if (Config.Instance.WhileHoldingTwoHand)
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
                Plugin.Debug($"isJumping, whileJumping ({Config.Instance.WhileJumping})");
                __result = Config.Instance.WhileJumping;
            }
            if (__instance.isWalking)
            {
                Plugin.Debug($"isWalking, whileWalking ({Config.Instance.WhileWalking})");
                __result = Config.Instance.WhileWalking;
            }
            if (__instance.isSprinting)
            {
                Plugin.Debug($"isSprinting, whileSprinting ({Config.Instance.WhileSprinting})");
                __result = Config.Instance.WhileSprinting;
            }
            if (__instance.isCrouching)
            {
                Plugin.Debug($"isCrouching, whileCrouching ({Config.Instance.WhileCrouching})");
                __result = Config.Instance.WhileCrouching;
            }
            if (__instance.isClimbingLadder)
            {
                Plugin.Debug($"isClimbingLadder, whileLadder ({Config.Instance.WhileLadder})");
                __result = Config.Instance.WhileLadder;
            }
            if (__instance.isTypingChat)
            {
                int currentEmote = __instance.playerBodyAnimator.GetInteger("emoteNumber");
                Plugin.Debug($"isTypingChat, whileTyping ({Config.Instance.WhileTyping}), moreEmotes ({moreEmotes}), currentEmote ({currentEmote})");
                if (moreEmotes && currentEmote == 10)
                {
                    Plugin.Debug($"isEmoteNumber 10 (Sign for MoreEmotes)");
                    __result = true;
                }
                else
                {
                    __result = Config.Instance.WhileTyping;
                }
            }
            if (__instance.inTerminalMenu)
            {
                Plugin.Debug($"inTerminalMenu, whileTerminal ({Config.Instance.WhileTerminal})");
                __result = Config.Instance.WhileTerminal;
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
            return;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        private static void InitializeLocalPlayer()
        {
            Plugin.Debug("InitializeLocalPlayer()");
            if (Config.IsHost)
            {
                try
                {
                    Config.MessageManager.RegisterNamedMessageHandler("ModName_OnRequestConfigSync", Config.OnRequestSync);
                    Config.Synced = true;
                }
                catch (Exception e)
                {
                    Plugin.Logger.LogError(e);
                }

                return;
            }

            Config.Synced = false;
            Config.MessageManager.RegisterNamedMessageHandler("ModName_OnReceiveConfigSync", Config.OnReceiveSync);
            Config.RequestSync();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        public static void PlayerLeave()
        {
            Plugin.Debug("PlayerLeave()");
            Config.RevertSync();
        }
    }
}
