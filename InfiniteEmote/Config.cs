using BepInEx.Configuration;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace InfiniteEmote
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {
        public bool WhileJumping = true;
        public bool WhileWalking = true;
        public bool WhileSprinting = true;
        public bool WhileCrouching = false;
        public bool WhileLadder = false;
        public bool WhileGrabbing = false;
        public bool WhileTyping = false;
        public bool WhileTerminal = false;
        public bool WhileHolding = true;
        public bool WhileHoldingTwoHand = true;

        [NonSerialized]
        private static string Section = "Emote while";

        public Config(ConfigFile configFile)
        {
            InitInstance(this);
            Plugin.Debug("Loading emote while options");
            WhileJumping = configFile.Bind(Section, "Jumping", WhileJumping, "Whether or not to allow emoting while Jumping").Value;
            WhileWalking = configFile.Bind(Section, "Walking", WhileWalking, "Whether or not to allow emoting while Walking").Value;
            WhileSprinting = configFile.Bind(Section, "Sprinting", WhileSprinting, "Whether or not to allow emoting while Sprinting").Value;
            WhileCrouching = configFile.Bind(Section, "Crouching", WhileCrouching, "Whether or not to allow emoting while Crouching").Value;
            WhileLadder = configFile.Bind(Section, "Ladder", WhileLadder, "Whether or not to allow emoting while climbing Ladder").Value;
            WhileGrabbing = configFile.Bind(Section, "Grabbing", WhileGrabbing, "Whether or not to allow emoting while Grabbing").Value;
            WhileTyping = configFile.Bind(Section, "Typing", WhileTyping, "Whether or not to allow emoting while Typing").Value;
            WhileTerminal = configFile.Bind(Section, "Terminal", WhileTerminal, "Whether or not to allow emoting while in the Terminal").Value;
            WhileHolding = configFile.Bind(Section, "Holding", WhileHolding, "Whether or not to allow emoting while Holding an object").Value;
            WhileHoldingTwoHand = configFile.Bind(Section, "HoldingTwoHand", WhileHoldingTwoHand, "Whether or not to allow emoting while Holding a two handed object").Value;

        }

        public void DebugValues()
        {
            Plugin.Debug($"WhileJumping '{WhileJumping}'");
            Plugin.Debug($"WhileWalking '{WhileWalking}'");
            Plugin.Debug($"WhileSprinting '{WhileSprinting}'");
            Plugin.Debug($"WhileCrouching '{WhileCrouching}'");
            Plugin.Debug($"WhileLadder '{WhileLadder}'");
            Plugin.Debug($"WhileGrabbing '{WhileGrabbing}'");
            Plugin.Debug($"WhileTyping '{WhileTyping}'");
            Plugin.Debug($"WhileTerminal '{WhileTerminal}'");
            Plugin.Debug($"WhileHoldingTwoHand '{WhileHoldingTwoHand}'");
        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("ModName_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            Plugin.Logger.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("ModName_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                Plugin.Logger.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                Plugin.Logger.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                Plugin.Logger.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            Plugin.Logger.LogInfo("Successfully synced config with host.");
        }
    }
}
