using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace InfiniteEmote
{
    internal class Patches
    {
        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote")]
        [HarmonyPrefix]
        private static bool CheckConditionsForEmotePatch(PlayerControllerB __instance, ref bool __result)
        {
            __result = !__instance.inSpecialInteractAnimation && !__instance.isPlayerDead && !__instance.isCrouching && !__instance.isClimbingLadder && !__instance.isGrabbingObjectAnimation && !__instance.inTerminalMenu;
            return false;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPrefix]
        private static void UpdatePrefix(PlayerControllerB __instance)
        {
            if (__instance.isPlayerControlled && __instance.IsOwner)
            {
                if (Keyboard.current[StopEmoteKey].IsPressed(0f) && !StopEmoteKeyPressed)
                {
                    StopEmoteKeyPressed = true;
                    __instance.performingEmote = false;
                    __instance.StopPerformingEmoteServerRpc();
                    __instance.timeSinceStartingEmote = 0f;
                    return;
                }
                else
                {
                    if (!Keyboard.current[StopEmoteKey].IsPressed(0f))
                    {
                        StopEmoteKeyPressed = false;
                    }
                }
            }
        }

        private static bool StopEmoteKeyPressed;

        public static string StopEmoteKey;
    }
}
