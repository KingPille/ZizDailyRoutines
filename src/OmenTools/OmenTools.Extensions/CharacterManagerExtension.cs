using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.Interop;

namespace OmenTools.Extensions;

public static class CharacterManagerExtension
{
	extension(scoped ref CharacterManager manager)
	{
		public unsafe BattleChara* FindFirst(delegate*<BattleChara*, bool> predicate)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate(val.Value))
					{
						return val.Value;
					}
				}
			}
			return null;
		}

		public unsafe BattleChara* FindFirst(Func<nint, bool> predicate)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return null;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						return val.Value;
					}
				}
			}
			return null;
		}

		public unsafe void FindAll(delegate*<BattleChara*, bool> predicate, List<nint> results)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			results.Clear();
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate(val.Value))
					{
						results.Add((nint)val.Value);
					}
				}
			}
		}

		public unsafe void FindAll(Func<nint, bool> predicate, List<nint> results)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			results.Clear();
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						results.Add((nint)val.Value);
					}
				}
			}
		}

		public unsafe List<nint> FindAll(delegate*<BattleChara*, bool> predicate)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			List<nint> list = new List<nint>();
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return list;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate(val.Value))
					{
						list.Add((nint)val.Value);
					}
				}
			}
			return list;
		}

		public unsafe List<nint> FindAll(Func<nint, bool> predicate)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			List<nint> list = new List<nint>();
			fixed (CharacterManager* ptr = &manager)
			{
				if (ptr == null)
				{
					return list;
				}
				Span<Pointer<BattleChara>> battleCharas = ((CharacterManager)ptr).BattleCharas;
				for (int i = 0; i < battleCharas.Length; i++)
				{
					Pointer<BattleChara> val = battleCharas[i];
					if (val.Value != null && predicate((nint)val.Value))
					{
						list.Add((nint)val.Value);
					}
				}
			}
			return list;
		}
	}
}
