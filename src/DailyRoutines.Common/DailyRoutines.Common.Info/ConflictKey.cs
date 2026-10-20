using System.Linq;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.GamePad;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Plugin.Services;
using OmenTools.Extensions;

namespace DailyRoutines.Common.Info;

public class ConflictKey
{
	[CompilerGenerated]
	private VirtualKey _003CKeyboard_003Ek__BackingField = (VirtualKey)16;

	public VirtualKey Keyboard
	{
		[CompilerGenerated]
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _003CKeyboard_003Ek__BackingField;
		}
		set
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			_003CKeyboard_003Ek__BackingField = (VirtualKey)((!DalamudServiceExtension.Instance<IKeyState>().GetValidVirtualKeys().Contains(_003CKeyboard_003Ek__BackingField)) ? 16 : ((int)value));
		}
	}

	public GamepadButtons Gamepad { get; set; } = (GamepadButtons)96;

	public bool IsPressed()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!DalamudServiceExtension.Instance<IKeyState>()[Keyboard])
		{
			return DalamudServiceExtension.Instance<IGamepadState>().Raw(Gamepad) == 1f;
		}
		return true;
	}

	public override string ToString()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		return $"{Keyboard} | {Gamepad}";
	}
}
