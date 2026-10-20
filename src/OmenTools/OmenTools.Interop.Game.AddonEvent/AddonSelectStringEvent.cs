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

public class AddonSelectStringEvent : AddonEventBase
{
	public unsafe static bool Select(IReadOnlyList<string> text)
	{
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!TryScanSelectStringText(text, out var index))
		{
			return false;
		}
		return Select(index);
	}

	public unsafe static bool Select(string text)
	{
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			return false;
		}
		if (!TryScanSelectStringText(text, out var index))
		{
			return false;
		}
		return Select(index);
	}

	public unsafe static bool Select(int index)
	{
		if (!(*Addons.SelectString).IsAddonAndNodesReady())
		{
			return false;
		}
		(*Addons.SelectString).Callback(index);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static bool TryScanSelectStringText(string text, out int index)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		index = -1;
		if (Addons.SelectString == null)
		{
			return false;
		}
		int entryCount = ((PopupMenu)(&((PopupMenuDerive)(&((AddonSelectString)Addons.SelectString).PopupMenu)).PopupMenu)).EntryCount;
		AtkValue* atkValues = ((AddonSelectString)Addons.SelectString).AtkValues;
		Span<char> span = stackalloc char[512];
		ReadOnlySpan<char> value = text.AsSpan();
		for (int i = 0; i < entryCount; i++)
		{
			ref AtkValue reference = ref *(AtkValue*)((byte*)atkValues + (nint)(i + 7) * (nint)Unsafe.SizeOf<AtkValue>());
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
	public unsafe static bool TryScanSelectStringText(IReadOnlyList<string> texts, out int index)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		index = -1;
		if (Addons.SelectString == null)
		{
			return false;
		}
		int entryCount = ((PopupMenu)(&((PopupMenuDerive)(&((AddonSelectString)Addons.SelectString).PopupMenu)).PopupMenu)).EntryCount;
		AtkValue* atkValues = ((AddonSelectString)Addons.SelectString).AtkValues;
		Span<char> span = stackalloc char[512];
		for (int i = 0; i < entryCount; i++)
		{
			ref AtkValue reference = ref *(AtkValue*)((byte*)atkValues + (nint)(i + 7) * (nint)Unsafe.SizeOf<AtkValue>());
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
