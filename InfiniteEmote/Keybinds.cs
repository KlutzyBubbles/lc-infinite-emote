using LethalCompanyInputUtils.Api;
using UnityEngine.InputSystem;

namespace InfiniteEmote
{
    internal class Keybinds : LcInputActions
    {
        [InputAction("<Keyboard>/0", Name = "Stop Emoting")]
        public InputAction StopEmote { get; set; }
    }
}
