using HarmonyLib;
using GameNetcodeStuff;

namespace InfiniteEmote
{
    internal class Patches
    {
        public static Keybinds keybinds = new Keybinds();

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

        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        [HarmonyPostfix]
        private static void StartPostfix(PlayerControllerB __instance)
        {
            keybinds.StopEmote.performed += delegate
            {
                if (__instance.isPlayerControlled && __instance.IsOwner)
                {
                    __instance.performingEmote = false;
                    __instance.StopPerformingEmoteServerRpc();
                    __instance.timeSinceStartingEmote = 0f;
                }
            };
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
            bool? isJumpingOpt = Traverse.Create(__instance).Field("isJumping").GetValue() as bool?;
            bool isJumping = isJumpingOpt ?? false;
            if (whileJumping && isJumping)
            {
                __result = true;
            }
            bool? isWalkingOpt = Traverse.Create(__instance).Field("isWalking").GetValue() as bool?;
            bool isWalking = isWalkingOpt ?? false;
            if (whileWalking && isWalking)
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
                __result = !__instance.inSpecialInteractAnimation && !__instance.isPlayerDead && !isJumping && !isWalking && !__instance.isCrouching && !__instance.isClimbingLadder && !__instance.isGrabbingObjectAnimation && !__instance.inTerminalMenu && !__instance.isTypingChat;
            }
            return false;
        }
    }
}
