using System;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Interop.Game.AgentEvent.Abstractions;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Interop.Game.AgentEvent;

public sealed class AgentLobbyEvent : AgentEventBase
{
	public unsafe static void SelectCharacterByIndex(uint index)
	{
		((AgentId)0).SendEvent(0uL, 21, (int)index);
		((AgentId)0).SendEvent(3uL, 0);
	}

	public unsafe static bool SelectCharacter(Predicate<CharaSelectCharacterEntry> condition)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (condition == null)
		{
			return false;
		}
		AgentLobby* ptr = AgentLobby.Instance();
		if (ptr == null)
		{
			return false;
		}
		StdVector<Pointer<CharaSelectCharacterEntry>> charaSelectEntries = ((LobbyData)(&((AgentLobby)ptr).LobbyData)).CharaSelectEntries;
		if (charaSelectEntries.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < charaSelectEntries.Count; i++)
		{
			Pointer<CharaSelectCharacterEntry> val = charaSelectEntries[i];
			if (!(val == Pointer<CharaSelectCharacterEntry>.op_Implicit((CharaSelectCharacterEntry*)null)) && val.Value != null)
			{
				CharaSelectCharacterEntry* value = val.Value;
				if (condition(*value))
				{
					SelectCharacterByIndex((uint)i);
					return true;
				}
			}
		}
		return false;
	}

	public unsafe static void SelectWorldByIndex(uint index)
	{
		((AgentId)0).SendEvent(0uL, 25, 0, (int)index);
	}

	public unsafe static bool SelectWorldByName(string name)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		name = name.Trim().ToLowerInvariant();
		if (string.IsNullOrEmpty(name))
		{
			return false;
		}
		CStringPointer* stringArray = ((StringArrayData)((AtkStage)AtkStage.Instance()).GetStringArrayData((StringArrayType)1)).StringArray;
		for (int i = 0; i < 8; i++)
		{
			CStringPointer val = ((CStringPointer*)stringArray)[i];
			if (((CStringPointer)(ref val)).HasValue && val.Value != null)
			{
				string text = ((object)(*(CStringPointer*)(&val))/*cast due to constrained. prefix*/).ToString().Trim().ToLowerInvariant();
				if (!string.IsNullOrEmpty(text) && text.Contains(name))
				{
					SelectWorldByIndex((uint)i);
					return true;
				}
			}
		}
		return false;
	}

	public static bool SelectWorldByID(uint worldID)
	{
		return SelectWorldByName(LuminaWrapper.GetWorldName(worldID));
	}

	public unsafe static void OpenWorldSelect()
	{
		((AgentId)0).SendEvent(0uL, 36);
	}

	public unsafe static void OpenCharacterCreation()
	{
		((AgentId)0).SendEvent(5uL, 7);
	}

	public unsafe static void CloseCharacterCreation()
	{
		((AgentId)0).SendEvent(0uL, 22);
	}

	public unsafe static void CloseCharacterSelect()
	{
		((AgentId)0).SendEvent(0uL, 19);
	}

	public unsafe static void OpenCharacterSelect()
	{
		((AgentId)0).SendEvent(0uL, 4);
	}
}
