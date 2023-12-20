using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

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
            keybinds.StopEmote.performed += onStopEmoteKey;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "OnDisable")]
        [HarmonyPostfix]
        public static void OnDisablePostfix(PlayerControllerB __instance)
        {
            if (__instance == localPlayerController)
            {
                keybinds.StopEmote.performed -= onStopEmoteKey;
                keybinds.StopEmote.Disable();
            }
        }

        private static void onStopEmoteKey(InputAction.CallbackContext context)
        {
            if (localPlayerController.isPlayerControlled && localPlayerController.IsOwner)
            {
                localPlayerController.performingEmote = false;
                localPlayerController.StopPerformingEmoteServerRpc();
                localPlayerController.timeSinceStartingEmote = 0f;
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
        [HarmonyPrefix]
        private static bool CheckConditionsForEmotePatch(PlayerControllerB __instance, ref bool __result)
        {
            if (__instance.inSpecialInteractAnimation || __instance.isPlayerDead)
            {
                __result = false;
                return false;
            }
            if (__instance.isGrabbingObjectAnimation)
            {
                if (whileGrabbing)
                {
                    __result = true;
                }
                else
                {
                    __result = false;
                    return false;
                }
            }
            if (__instance.isHoldingObject)
            {
                if (whileHolding)
                {
                    if (__instance.twoHanded)
                    {
                        if (whileHoldingTwoHand)
                        {
                            __result = true;
                        }
                        else
                        {
                            __result = false;
                            return false;
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
                    return false;
                }
            }
            if (whileJumping && __instance.isJumping)
            {
                __result = true;
            }
            if (whileWalking && __instance.isWalking)
            {
                if (__instance.isSprinting)
                {
                    __result = whileSprinting;
                }
                else
                {
                    __result = true;
                }
            }
            if (whileSprinting && __instance.isSprinting)
            {
                __result = true;
            }
            if (whileCrouching && __instance.isCrouching)
            {
                __result = true;
            }
            if (whileLadder && __instance.isClimbingLadder)
            {
                __result = true;
            }
            if (whileTyping && __instance.isTypingChat)
            {
                __result = true;
            }
            if (whileTerminal && __instance.inTerminalMenu)
            {
                __result = true;
            }
            if (__result == false)
            {
                __result = !__instance.inSpecialInteractAnimation && !__instance.isPlayerDead && !__instance.isJumping && !__instance.isWalking && !__instance.isCrouching && !__instance.isClimbingLadder && !__instance.isGrabbingObjectAnimation && !__instance.inTerminalMenu && !__instance.isTypingChat;
            }
            return false;
        }
    }
}
