using LethalCompanyInputUtils.Api;
using System;
using UnityEngine.InputSystem;

namespace InfiniteEmote
{
    internal class Keybinds : LcInputActions
    {
        public InputAction StopEmote => Asset["StopEmote"];

        public override void CreateInputActions(in InputActionMapBuilder builder)
        {
            base.CreateInputActions(builder);
            builder.NewActionBinding()
                .WithActionId("StopEmote")
                .WithActionType(InputActionType.Button)
                .WithKbmPath(Patches.stopEmoteKey)
                .WithBindingName("Stop Emote")
                .Finish();
        }
    }
}
