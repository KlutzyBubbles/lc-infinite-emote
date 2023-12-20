using LethalCompanyInputUtils.Api;
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
                .WithGamepadPath(Patches.stopEmoteController)
                .WithBindingName("Stop Emote")
                .Finish();
        }
    }
}
