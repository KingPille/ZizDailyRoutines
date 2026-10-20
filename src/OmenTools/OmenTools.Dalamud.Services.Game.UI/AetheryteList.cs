using System;
using System.Collections;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.UI.Abstractions;

namespace OmenTools.Dalamud.Services.Game.UI;

internal sealed class AetheryteList : IAetheryteList, IReadOnlyCollection<IAetheryteEntry>, IEnumerable<IAetheryteEntry>, IEnumerable, IOmenDalamudService<IAetheryteList>, IOmenDalamudService
{
	private struct Enumerator(AetheryteList aetheryteList) : IEnumerator<IAetheryteEntry>, IEnumerator, IDisposable
	{
		private int index = -1;

		public IAetheryteEntry Current { get; private set; } = null;

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			if (++index < aetheryteList.Length)
			{
				Current = aetheryteList[index];
				return true;
			}
			Current = null;
			return false;
		}

		public void Reset()
		{
			index = -1;
		}

		public void Dispose()
		{
		}
	}

	private unsafe readonly Telepo* telepoInstance = Telepo.Instance();

	public unsafe int Length
	{
		get
		{
			if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				return 0;
			}
			Update();
			if (((Telepo)telepoInstance).TeleportList.First == ((Telepo)telepoInstance).TeleportList.Last)
			{
				return 0;
			}
			return ((Telepo)telepoInstance).TeleportList.Count;
		}
	}

	public unsafe IAetheryteEntry? this[int index]
	{
		get
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			if (index < 0 || index >= Length)
			{
				return null;
			}
			if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer == null)
			{
				return null;
			}
			return new AetheryteEntry(((Telepo)telepoInstance).TeleportList[index]);
		}
	}

	public int Count => Length;

	private unsafe void Update()
	{
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
		{
			((Telepo)telepoInstance).UpdateAetheryteList();
		}
	}

	public IEnumerator<IAetheryteEntry> GetEnumerator()
	{
		return new Enumerator(this);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
