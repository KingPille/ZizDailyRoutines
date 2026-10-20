using System.Collections.Frozen;
using System.Collections.Generic;
using Dalamud.Game.Text;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.Data;

public static class XIVChatTypes
{
	public static FrozenDictionary<XivChatType, string> ChatTypeToAddonText { get; } = new Dictionary<XivChatType, string>
	{
		[(XivChatType)10] = LuminaWrapper.GetAddonText(1907u),
		[(XivChatType)11] = LuminaWrapper.GetAddonText(1908u),
		[(XivChatType)13] = LuminaWrapper.GetAddonText(1909u),
		[(XivChatType)14] = LuminaWrapper.GetAddonText(1910u),
		[(XivChatType)15] = LuminaWrapper.GetAddonText(1932u),
		[(XivChatType)16] = LuminaWrapper.GetAddonText(7890u),
		[(XivChatType)17] = LuminaWrapper.GetAddonText(7891u),
		[(XivChatType)18] = LuminaWrapper.GetAddonText(7892u),
		[(XivChatType)19] = LuminaWrapper.GetAddonText(7893u),
		[(XivChatType)20] = LuminaWrapper.GetAddonText(7894u),
		[(XivChatType)21] = LuminaWrapper.GetAddonText(7895u),
		[(XivChatType)22] = LuminaWrapper.GetAddonText(7896u),
		[(XivChatType)23] = LuminaWrapper.GetAddonText(7897u),
		[(XivChatType)24] = LuminaWrapper.GetAddonText(1933u),
		[(XivChatType)27] = LuminaWrapper.GetAddonText(7898u),
		[(XivChatType)30] = LuminaWrapper.GetAddonText(1931u),
		[(XivChatType)36] = LuminaWrapper.GetAddonText(7899u),
		[(XivChatType)37] = LuminaWrapper.GetAddonText(7866u),
		[(XivChatType)101] = LuminaWrapper.GetAddonText(8390u),
		[(XivChatType)102] = LuminaWrapper.GetAddonText(8391u),
		[(XivChatType)103] = LuminaWrapper.GetAddonText(8392u),
		[(XivChatType)104] = LuminaWrapper.GetAddonText(8393u),
		[(XivChatType)105] = LuminaWrapper.GetAddonText(8394u),
		[(XivChatType)106] = LuminaWrapper.GetAddonText(8395u),
		[(XivChatType)107] = LuminaWrapper.GetAddonText(8396u),
		[(XivChatType)56] = LuminaWrapper.GetAddonText(1912u)
	}.ToFrozenDictionary();
}
