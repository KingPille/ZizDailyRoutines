using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using InteropGenerator.Runtime;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent.Abstractions;

namespace OmenTools.Interop.Game.AddonEvent;

public class AddonContextMenuEvent : AddonEventBase
{
	public unsafe static bool Select(IReadOnlyList<string> text)
	{
		if (!(*Addons.ContextMenuAddon).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!TryScanContextMenuText(text, out var index))
		{
			return false;
		}
		return Select(index);
	}

	public unsafe static bool Select(string text)
	{
		if (!(*Addons.ContextMenuAddon).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!TryScanContextMenuText(text, out var index))
		{
			return false;
		}
		return Select(index);
	}

	public unsafe static bool Select(int index)
	{
		if (!(*Addons.ContextMenuAddon).IsAddonAndNodesReady())
		{
			return false;
		}
		(*Addons.ContextMenuAddon).Callback(0, index, 0u, 0, 0);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool TryScanContextMenuText(string text, out int index)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		index = -1;
		if (Addons.ContextMenuAddon == null)
		{
			return false;
		}
		AtkValue* atkValues = ((AddonContextMenu)Addons.ContextMenuAddon).AtkValues;
		uint uInt = ((AtkValue)atkValues).UInt;
		if (uInt == 0)
		{
			return false;
		}
		Span<char> span = stackalloc char[512];
		ReadOnlySpan<char> value = text.AsSpan();
		for (int i = 0; i < uInt; i++)
		{
			ref AtkValue reference = ref *(AtkValue*)((byte*)atkValues + (nint)(i + 8) * (nint)Unsafe.SizeOf<AtkValue>());
			if ((int)reference.Type == 0 || !((CStringPointer)(ref reference.String)).HasValue)
			{
				continue;
			}
			ReadOnlySpan<byte> bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(CStringPointer.op_Implicit(reference.String));
			if (!bytes.IsEmpty)
			{
				int charCount = Encoding.UTF8.GetCharCount(bytes);
				Span<char> span2 = ((charCount <= span.Length) ? span.Slice(0, charCount) : ((Span<char>)new char[charCount]));
				Encoding.UTF8.GetChars(bytes, span2);
				if (span2.Contains(value, StringComparison.OrdinalIgnoreCase))
				{
					index = i;
					return true;
				}
			}
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool TryScanContextMenuText(IReadOnlyList<string> texts, out int index)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		index = -1;
		if (Addons.ContextMenuAddon == null)
		{
			return false;
		}
		AtkValue* atkValues = ((AddonContextMenu)Addons.ContextMenuAddon).AtkValues;
		uint uInt = ((AtkValue)atkValues).UInt;
		if (uInt == 0)
		{
			return false;
		}
		Span<char> span = stackalloc char[512];
		for (int i = 0; i < uInt; i++)
		{
			ref AtkValue reference = ref *(AtkValue*)((byte*)atkValues + (nint)(i + 8) * (nint)Unsafe.SizeOf<AtkValue>());
			if ((int)reference.Type == 0 || !((CStringPointer)(ref reference.String)).HasValue)
			{
				continue;
			}
			ReadOnlySpan<byte> bytes = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(CStringPointer.op_Implicit(reference.String));
			if (bytes.IsEmpty)
			{
				continue;
			}
			int charCount = Encoding.UTF8.GetCharCount(bytes);
			Span<char> span2 = ((charCount <= span.Length) ? span.Slice(0, charCount) : ((Span<char>)new char[charCount]));
			Encoding.UTF8.GetChars(bytes, span2);
			int count = texts.Count;
			for (int j = 0; j < count; j++)
			{
				if (span2.Contains(texts[j].AsSpan(), StringComparison.OrdinalIgnoreCase))
				{
					index = i;
					return true;
				}
			}
		}
		return false;
	}
}
