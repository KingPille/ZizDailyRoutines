using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using Lumina.Excel.Sheets;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Interface;

public class AutoMateriaAttach : ModuleBase
{
	private unsafe delegate void nCdwyVmx28cSFhm8RbO(MateriaAttachData* data);

	private enum SidebarTab
	{
		Plans,
		Import
	}

	private readonly struct sUJIEympPHpoiidxbEK : IEquatable<sUJIEympPHpoiidxbEK>
	{
		public string Label { get; init; }

		public string Value { get; init; }

		public Vector4 ValueColor { get; init; }

		public string? Tooltip { get; init; }

		public sUJIEympPHpoiidxbEK(string Label, string Value, Vector4 ValueColor, string? Tooltip)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.Label = Label;
			this.Value = Value;
			this.ValueColor = ValueColor;
			this.Tooltip = Tooltip;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5F674 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53573938 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			if (mXmm3CcZ8c(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool mXmm3CcZ8c(StringBuilder builder)
		{
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F3C65 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
			builder.Append((object?)Label);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7C4C5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
			builder.Append((object?)Value);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AC1AB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			builder.Append(ValueColor.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951F847 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			builder.Append((object?)Tooltip);
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(sUJIEympPHpoiidxbEK left, sUJIEympPHpoiidxbEK right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(sUJIEympPHpoiidxbEK left, sUJIEympPHpoiidxbEK right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<string>.Default.GetHashCode(Label) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value)) * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(ValueColor)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Tooltip);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is sUJIEympPHpoiidxbEK)
			{
				return Equals((sUJIEympPHpoiidxbEK)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(sUJIEympPHpoiidxbEK other)
		{
			if (EqualityComparer<string>.Default.Equals(Label, other.Label) && EqualityComparer<string>.Default.Equals(Value, other.Value) && EqualityComparer<Vector4>.Default.Equals(ValueColor, other.ValueColor))
			{
				return EqualityComparer<string>.Default.Equals(Tooltip, other.Tooltip);
			}
			return false;
		}

		[CompilerGenerated]
		public void OYUmBMC3Cf(out string Label, out string Value, out Vector4 ValueColor, out string? Tooltip)
		{
			Label = this.Label;
			Value = this.Value;
			ValueColor = this.ValueColor;
			Tooltip = this.Tooltip;
		}
	}

	private class nSiWNGmmrHVI7N2Bt9j
	{
		public string Name { get; set; }

		public string OriginalURL { get; set; }

		public List<pxgVHKmo62VsKBeM4fK> MeldingInfos { get; set; }

		public nSiWNGmmrHVI7N2Bt9j()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Name = string.Empty;
			OriginalURL = string.Empty;
			MeldingInfos = new List<pxgVHKmo62VsKBeM4fK>();
			base._002Ector();
		}
	}

	private class k82K69mig4f2t8DLn8R : ModuleConfig
	{
		public List<nSiWNGmmrHVI7N2Bt9j> AllMeldingPlans;

		public nSiWNGmmrHVI7N2Bt9j? CurrentMeldingPlan;

		public k82K69mig4f2t8DLn8R()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			AllMeldingPlans = new List<nSiWNGmmrHVI7N2Bt9j>();
			base._002Ector();
		}
	}

	private struct zeD1U3mPqm2MXRQBwgd
	{
		public uint MateriaID { get; set; }

		public byte Grade { get; set; }

		public zeD1U3mPqm2MXRQBwgd(uint materiaID, byte grade)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			MateriaID = materiaID;
			Grade = grade;
		}
	}

	private struct VBPCbFmhCSmnVu0mgbR
	{
		public int Required { get; set; }

		public int Available { get; set; }

		public uint ItemID { get; set; }

		public VBPCbFmhCSmnVu0mgbR(int required, int available, uint itemID)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			Required = required;
			Available = available;
			ItemID = itemID;
		}
	}

	private class pxgVHKmo62VsKBeM4fK
	{
		public uint ItemID { get; set; }

		public List<uint> MateriaIDs { get; set; }

		public pxgVHKmo62VsKBeM4fK()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			MateriaIDs = new List<uint>();
			base._002Ector();
		}
	}

	private struct uTmPD7mgXEISZyRMsCN
	{
		public uint ItemID { get; set; }

		public int ItemIndex { get; set; }

		public uTmPD7mgXEISZyRMsCN(uint itemID, int itemIndex)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ItemID = itemID;
			ItemIndex = itemIndex;
		}
	}

	private class rREVgcmw6gwyIYa4fy3
	{
		public pxgVHKmo62VsKBeM4fK MeldingInfo { get; init; }

		public InventoryItem InventoryItem { get; init; }

		public int ItemIndex { get; init; }

		public string ItemName { get; init; }

		public Item ItemData { get; init; }

		public ISharedImmediateTexture Icon { get; init; }

		public bool IsCompleted { get; init; }

		public List<zeD1U3mPqm2MXRQBwgd> AttachedMateria { get; init; }

		public bool IsFound { get; init; }

		public rREVgcmw6gwyIYa4fy3()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			ItemName = string.Empty;
			AttachedMateria = new List<zeD1U3mPqm2MXRQBwgd>();
			base._002Ector();
		}
	}

	private static class EtXT6vmEShWSiW3fWHS
	{
		private class yxK6GfPDtPWfp1KpJtV
		{
			private const int LEGACY_VERSION = 4;

			private const int FIRST_PACKED_VERSION = 5;

			private const int CURRENT_UNPACKED_JOB_VERSION = 6;

			public int Version { get; }

			public int Job { get; private set; }

			public int JobLevel { get; private set; }

			public int SyncLevel { get; private set; }

			public int GearTypeCount { get; private set; }

			public int GearID { get; private set; }

			public int MateriaGrade { get; private set; }

			public int SpecialGear { get; private set; }

			public int CustomStat { get; private set; }

			public bool bicPeikZtO(int version)
			{
				if (version <= 4)
				{
					return false;
				}
				if (version < 5)
				{
					Job = 33;
					JobLevel = 6;
					SyncLevel = 800;
					GearTypeCount = 8;
					GearID = 60000;
					MateriaGrade = 12;
					SpecialGear = 9;
					CustomStat = 1001;
				}
				if (version >= 6)
				{
					Job++;
				}
				return true;
			}

			public yxK6GfPDtPWfp1KpJtV()
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				Version = 77;
				Job = 33;
				JobLevel = 6;
				SyncLevel = 800;
				GearTypeCount = 8;
				GearID = 60000;
				MateriaGrade = 12;
				SpecialGear = 9;
				CustomStat = 1001;
				base._002Ector();
			}
		}

		private struct MqbQNsPNLB7y1cm6hfV : IEquatable<MqbQNsPNLB7y1cm6hfV>
		{
			public string Stat { get; set; }

			public int Grade { get; set; }

			public MqbQNsPNLB7y1cm6hfV(string Stat, int Grade)
			{
				UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
				this.Stat = Stat;
				this.Grade = Grade;
			}

			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D740918 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
				stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F7912 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
				if (O32P76v21x(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private readonly bool O32P76v21x(StringBuilder builder)
			{
				builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23115BC3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d));
				builder.Append((object?)Stat);
				builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F37B390 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
				builder.Append(Grade.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(MqbQNsPNLB7y1cm6hfV left, MqbQNsPNLB7y1cm6hfV right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(MqbQNsPNLB7y1cm6hfV left, MqbQNsPNLB7y1cm6hfV right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<string>.Default.GetHashCode(Stat) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(Grade);
			}

			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				if (obj is MqbQNsPNLB7y1cm6hfV)
				{
					return Equals((MqbQNsPNLB7y1cm6hfV)obj);
				}
				return false;
			}

			[CompilerGenerated]
			public readonly bool Equals(MqbQNsPNLB7y1cm6hfV other)
			{
				if (EqualityComparer<string>.Default.Equals(Stat, other.Stat))
				{
					return EqualityComparer<int>.Default.Equals(Grade, other.Grade);
				}
				return false;
			}

			[CompilerGenerated]
			public readonly void EbZPdtPx0i(out string Stat, out int Grade)
			{
				Stat = this.Stat;
				Grade = this.Grade;
			}
		}

		private enum GearType
		{
			Special = 6,
			Customizable
		}

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003C_003Ec__DisplayClass3_0
		{
			public BigInteger input;
		}

		private static readonly Dictionary<char, BigInteger> CharsetMap;

		private static readonly List<(string Job, string[] StatDecode)> JobDecode;

		private static readonly int[] SpecialGearDecode;

		public static List<pxgVHKmo62VsKBeM4fK> HZimbpoIJ6(string s)
		{
			List<pxgVHKmo62VsKBeM4fK> list = new List<pxgVHKmo62VsKBeM4fK>();
			if (string.IsNullOrWhiteSpace(s))
			{
				return list;
			}
			_003C_003Ec__DisplayClass3_0 _003C_003Ec__DisplayClass3_1 = default(_003C_003Ec__DisplayClass3_0);
			_003C_003Ec__DisplayClass3_1.input = 0;
			foreach (char key in s)
			{
				if (CharsetMap.TryGetValue(key, out var value))
				{
					_003C_003Ec__DisplayClass3_1.input = _003C_003Ec__DisplayClass3_1.input * 62 + value;
				}
			}
			yxK6GfPDtPWfp1KpJtV yxK6GfPDtPWfp1KpJtV2 = new yxK6GfPDtPWfp1KpJtV();
			if (!yxK6GfPDtPWfp1KpJtV2.bicPeikZtO(KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.Version, ref _003C_003Ec__DisplayClass3_1)))
			{
				return list;
			}
			int num = KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.Job, ref _003C_003Ec__DisplayClass3_1);
			if (num >= JobDecode.Count)
			{
				return list;
			}
			string[] item = JobDecode[num].StatDecode;
			int num2 = item.Length;
			if (CvumqbtIYp(ref _003C_003Ec__DisplayClass3_1))
			{
				KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.JobLevel, ref _003C_003Ec__DisplayClass3_1);
				KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.SyncLevel, ref _003C_003Ec__DisplayClass3_1);
			}
			List<int> list2 = new List<int>();
			for (int j = 0; j < yxK6GfPDtPWfp1KpJtV2.GearTypeCount; j++)
			{
				if (CvumqbtIYp(ref _003C_003Ec__DisplayClass3_1))
				{
					list2.Add(j);
				}
			}
			int num3 = KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.MateriaGrade + 1, ref _003C_003Ec__DisplayClass3_1);
			List<MqbQNsPNLB7y1cm6hfV?> list3 = new List<MqbQNsPNLB7y1cm6hfV?>();
			for (int num4 = yxK6GfPDtPWfp1KpJtV2.MateriaGrade; num4 >= num3; num4--)
			{
				if (num4 != 0)
				{
					for (int k = 0; k < num2; k++)
					{
						if (CvumqbtIYp(ref _003C_003Ec__DisplayClass3_1))
						{
							list3.Add(new MqbQNsPNLB7y1cm6hfV(item[k], num4));
						}
					}
				}
			}
			if (num3 == 0)
			{
				list3.Add(null);
			}
			int num5 = -1;
			int num6 = 0;
			int num7 = 0;
			bool flag = false;
			while (_003C_003Ec__DisplayClass3_1.input != BigInteger.Zero && list2.Count != 0)
			{
				int num8 = list2[KdImcqRv7S(list2.Count, ref _003C_003Ec__DisplayClass3_1)];
				pxgVHKmo62VsKBeM4fK pxgVHKmo62VsKBeM4fK2 = new pxgVHKmo62VsKBeM4fK();
				switch (num8)
				{
				case 6:
					pxgVHKmo62VsKBeM4fK2.ItemID = (uint)SpecialGearDecode[KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.SpecialGear, ref _003C_003Ec__DisplayClass3_1)];
					list.Add(pxgVHKmo62VsKBeM4fK2);
					continue;
				case 7:
					foreach (string item2 in item.Reverse())
					{
						_ = item2;
						KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.CustomStat, ref _003C_003Ec__DisplayClass3_1);
					}
					break;
				default:
				{
					if (list3.Count <= 0)
					{
						break;
					}
					for (int l = 0; l < num8; l++)
					{
						MqbQNsPNLB7y1cm6hfV? mqbQNsPNLB7y1cm6hfV = list3[KdImcqRv7S(list3.Count, ref _003C_003Ec__DisplayClass3_1)];
						if (mqbQNsPNLB7y1cm6hfV.HasValue)
						{
							pxgVHKmo62VsKBeM4fK2.MateriaIDs.Add(jsmmF3Mhhy(mqbQNsPNLB7y1cm6hfV.Value));
						}
					}
					break;
				}
				}
				if (num5 == -1)
				{
					num5 = KdImcqRv7S(yxK6GfPDtPWfp1KpJtV2.GearID, ref _003C_003Ec__DisplayClass3_1);
					if (num5 == 0)
					{
						break;
					}
					num6 = KdImcqRv7S(num5, ref _003C_003Ec__DisplayClass3_1);
					num7 = (CvumqbtIYp(ref _003C_003Ec__DisplayClass3_1) ? 1 : (-1));
					flag = CvumqbtIYp(ref _003C_003Ec__DisplayClass3_1);
				}
				else
				{
					int num9 = KdImcqRv7S(num6, ref _003C_003Ec__DisplayClass3_1);
					num5 += (num9 - ((_003C_003Ec__DisplayClass3_1.input == BigInteger.Zero) ? 1 : 0)) * num7;
				}
				pxgVHKmo62VsKBeM4fK2.ItemID = (uint)num5;
				list.Add(pxgVHKmo62VsKBeM4fK2);
			}
			if (flag != (num7 == -1))
			{
				list.Reverse();
			}
			return list;
		}

		private static uint jsmmF3Mhhy(MqbQNsPNLB7y1cm6hfV m)
		{
			//IL_0385: Unknown result type (might be due to invalid IL or missing references)
			//IL_038a: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
			string stat = m.Stat;
			if (stat == null)
			{
				goto IL_0350;
			}
			int length = stat.Length;
			uint num;
			if (length != 2)
			{
				if (length != 3)
				{
					goto IL_0350;
				}
				switch (stat[1])
				{
				case 'R':
					break;
				case 'H':
					goto IL_0107;
				case 'E':
					goto IL_0133;
				case 'K':
					goto IL_018a;
				case 'P':
					goto IL_01b5;
				case 'I':
					goto IL_01e0;
				case 'M':
					goto IL_0211;
				case 'T':
					goto IL_023c;
				case 'C':
					goto IL_0268;
				default:
					goto IL_0350;
				}
				if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D60D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)))
				{
					if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4584E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)))
					{
						goto IL_0350;
					}
					num = 23u;
				}
				else
				{
					num = 15u;
				}
			}
			else
			{
				char c = stat[0];
				if (c != 'C')
				{
					if (c != 'G' || !(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5E96C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)))
					{
						goto IL_0350;
					}
					num = 20u;
				}
				else
				{
					if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D14ACB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)))
					{
						goto IL_0350;
					}
					num = 22u;
				}
			}
			goto IL_0352;
			IL_0211:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C2BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)))
			{
				goto IL_0350;
			}
			num = 21u;
			goto IL_0352;
			IL_0107:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1624630900 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)))
			{
				goto IL_0350;
			}
			num = 14u;
			goto IL_0352;
			IL_023c:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x1F62D386 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)))
			{
				goto IL_0350;
			}
			num = 18u;
			goto IL_0352;
			IL_0352:
			uint num2 = num;
			if (num2 == 0)
			{
				return 0u;
			}
			if (!LuminaGetter.TryGetRow<Materia>(num2, out Materia item))
			{
				return 0u;
			}
			if (m.Grade - 1 < 0 || m.Grade - 1 >= ((Materia)(ref item)).Item.Count)
			{
				return 0u;
			}
			Item value = ((Materia)(ref item)).Item[m.Grade - 1].Value;
			return ((Item)(ref value)).RowId;
			IL_01e0:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE691 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)))
			{
				goto IL_0350;
			}
			num = 7u;
			goto IL_0352;
			IL_018a:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD14 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)))
			{
				goto IL_0350;
			}
			num = 24u;
			goto IL_0352;
			IL_0350:
			num = 0u;
			goto IL_0352;
			IL_0133:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317A1B5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273)))
			{
				if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B49F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)))
				{
					goto IL_0350;
				}
				num = 17u;
			}
			else
			{
				num = 16u;
			}
			goto IL_0352;
			IL_01b5:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F7C2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)))
			{
				goto IL_0350;
			}
			num = 25u;
			goto IL_0352;
			IL_0268:
			if (!(stat == LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B45814 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)))
			{
				goto IL_0350;
			}
			num = 19u;
			goto IL_0352;
		}

		static EtXT6vmEShWSiW3fWHS()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			CharsetMap = LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53516AB2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08).Select((char c, int i) => new { c, i }).ToDictionary(x => x.c, x => (BigInteger)x.i);
			int num = 34;
			List<(string, string[])> list = new List<(string, string[])>(num);
			CollectionsMarshal.SetCount(list, num);
			Span<(string, string[])> span = CollectionsMarshal.AsSpan(list);
			span[0] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75FE72 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F072C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E5F14 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C178E9B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194B4F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF11EB4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)
			});
			span[1] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAED572 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE6DB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9B18A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951E797 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F7D8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B31480 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae)
			});
			span[2] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDC060 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D60D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194B48E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B31494 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC9C26 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055D4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)
			});
			span[3] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7DA37 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB6A17 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -437280524 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C696 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA41C86 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7DB3B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)
			});
			span[4] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695E88B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C617F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D62F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469ADE59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C613B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7DB59 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f)
			});
			span[5] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36447C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D1A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055DE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C2F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017A792 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA41C92 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)
			});
			span[6] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D5799 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39876F31 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -168140929 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC9C44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017A792 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2CEC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)
			});
			span[7] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1675397288 ^ -614624583 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec), new string[5]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F072C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F7A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAED446 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -181610621 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)
			});
			span[8] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6A5F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC9C78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3148A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C14534F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055A2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)
			});
			span[9] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC557A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B2242 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA41CFA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B227E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4D30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)
			});
			span[10] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310AC23 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD877A9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC5420 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD76 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B45826 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)
			});
			span[11] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B459EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F69F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055DE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8524 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53572982 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb)
			});
			span[12] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C421D0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x329F734C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD68 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B45844 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39876F6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)
			});
			span[13] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75FEF0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D1A0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310AD6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE6E7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194B4F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)
			});
			span[14] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5712A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8518 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D56FD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE6E7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE685 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)
			});
			span[15] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1452CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5E9F6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E0055DE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9B194 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2310AD13 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)
			});
			span[16] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F227D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177851AF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F7A4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD76 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75FF2E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)
			});
			span[17] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C60D7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D5670F2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D182 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B227E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68AC9C3C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09)
			});
			span[18] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F37DC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD4A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E570BE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B49E6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4D2A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)
			});
			span[19] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951E66F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -2072208530 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDC17C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAED446 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C289 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)
			});
			span[20] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317A05D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC5402 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B2260 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB6A2B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C6EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)
			});
			span[21] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D7FD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68541D91 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7CD68 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535729E0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C6EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)
			});
			span[22] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F3644CA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAED40A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B4584E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F6964 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825)
			});
			span[23] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F37EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D56AF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4CF2D196 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a07aac30eb32484e94cb4d1e62117765),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4DFC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1)
			});
			span[24] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695EBB3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B45808 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B2274 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3951E739 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497)
			});
			span[25] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177853B7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F8EF2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4C2FB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317A105 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273)
			});
			span[26] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B45A76 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F6986 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39876F07 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF11E0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912)
			});
			span[27] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C49E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35C6DA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C178E91 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5700E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)
			});
			span[28] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D437 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C8568 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B49EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5700E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)
			});
			span[29] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x1F3E2CC1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C178ED7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE5D63B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695E933 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)
			});
			span[30] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F2189 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109C857E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B222E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2C3C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)
			});
			span[31] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B316F4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F3664 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F7EA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C421C0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)
			});
			span[32] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7D951 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f), new string[3]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDC138 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E4D02 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427F71C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b)
			});
			span[33] = (LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x5871C48C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc), new string[4]
			{
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20A6C1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x535729FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D14A65 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c),
				LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2CF8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb)
			});
			JobDecode = list;
			SpecialGearDecode = new int[9] { 10337, 10338, 10339, 10340, 10341, 10342, 10343, 10344, 17726 };
		}

		[CompilerGenerated]
		internal static bool CvumqbtIYp(ref _003C_003Ec__DisplayClass3_0 P_0)
		{
			return KdImcqRv7S(2, ref P_0) == 1;
		}

		[CompilerGenerated]
		internal static int KdImcqRv7S(BigInteger range, ref _003C_003Ec__DisplayClass3_0 P_1)
		{
			if (range <= 0L)
			{
				return 0;
			}
			int result = (int)(P_1.input % range);
			P_1.input /= range;
			return result;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass24_0
	{
		public InventoryType inventoryType;

		public short slot;

		public uint materiaID;

		public _003C_003Ec__DisplayClass24_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal unsafe void PfQmAFjlBu()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			AgentMateriaAttach* agentByInternalId = (AgentMateriaAttach*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)45);
			InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(inventoryType, (int)slot);
			MateriaAttachEntry* itemArraySorted = *((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).ItemArraySorted;
			((MateriaAttachEntry)itemArraySorted).Item = inventorySlot;
			Item value = LuminaGetter.GetRow<Item>(((InventoryItem)inventorySlot).ItemId).Value;
			((MateriaAttachEntry)itemArraySorted).ItemLevel = (short)((Item)(ref value)).LevelItem.RowId;
		}

		internal unsafe bool U6emQcVL0i()
		{
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x79DDE0F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def), 100u))
			{
				return false;
			}
			AgentMateriaAttach* agentByInternalId = (AgentMateriaAttach*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)45);
			for (int i = 0; i < ((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).MateriaSorted.Length; i++)
			{
				Pointer<MateriaAttachEntry> val = ((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).MateriaSorted[i];
				if (((MateriaAttachEntry)val.Value).Item != null && ((InventoryItem)((MateriaAttachEntry)val.Value).Item).ItemId == materiaID)
				{
					((AgentId)45).SendEvent(1uL, 2, i, 1, 0);
					return true;
				}
			}
			return false;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass28_0
	{
		public uint itemID;

		public _003C_003Ec__DisplayClass28_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool pc7mJfnTXn(InventoryItem i)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return i.ItemId == itemID;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass33_0
	{
		public uint materiaID;

		public _003C_003Ec__DisplayClass33_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool A6HmrLnear(InventoryItem i)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return i.ItemId == materiaID;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass35_0
	{
		public pxgVHKmo62VsKBeM4fK materiaInfo;

		public _003C_003Ec__DisplayClass35_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool kMEmGpRI06(InventoryItem i)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return i.ItemId == materiaInfo.ItemID;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass36_0
	{
		public uint itemID;

		public _003C_003Ec__DisplayClass36_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool cium6HI0X9(InventoryItem i)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return i.ItemId == itemID;
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass65_0
	{
		public uint itemID;

		public _003C_003Ec__DisplayClass65_0()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
		}

		internal bool i7dmk4ffTs(InventoryItem i)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return i.ItemId == itemID;
		}
	}

	private SidebarTab currentSidebarTab;

	private string planSearchInput;

	private int planToDeleteIndex;

	private bool isItemTooltipActive;

	private static readonly CompSig ClearBlockedItemsSig;

	private Hook<nCdwyVmx28cSFhm8RbO> ClearBlockedItemsHook;

	private static readonly CompSig AlwaysAllowAllMateriaSig;

	private MemoryPatch alwaysAllowAllMateriaPatch;

	private uTmPD7mgXEISZyRMsCN? singleMeldingTarget;

	private k82K69mig4f2t8DLn8R config;

	private string gearsetURLInput;

	private string gearsetNameInput;

	private const ImGuiWindowFlags OVERLAY_CHILD_FLAGS = (ImGuiWindowFlags)152;

	private static readonly SidebarTab[] SidebarTabs;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected unsafe override void Init()
	{
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		alwaysAllowAllMateriaPatch = new MemoryPatch(AlwaysAllowAllMateriaSig.Get(), new global::_003C_003Ez__ReadOnlyArray<byte?>(new byte?[2]
		{
			(byte)144,
			(byte)144
		}));
		config = ModuleConfigExtension.Load<k82K69mig4f2t8DLn8R>(this) ?? new k82K69mig4f2t8DLn8R();
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 30000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		if (base.Overlay == null)
		{
			Overlay overlay = new Overlay(this);
			((Window)overlay).Flags = (ImGuiWindowFlags)8;
			((Window)overlay).WindowName = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C143D2D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317CFF9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273);
			WindowSizeConstraints value = default(WindowSizeConstraints);
			((WindowSizeConstraints)(ref value))._002Ector();
			((WindowSizeConstraints)(ref value)).MaximumSize = new Vector2(float.MaxValue);
			((WindowSizeConstraints)(ref value)).MinimumSize = Globals.ScaledVector2(380f, 260f);
			((Window)overlay).SizeConstraints = value;
			Overlay overlay2 = overlay;
			base.Overlay = overlay;
		}
		if (ClearBlockedItemsHook == null)
		{
			ClearBlockedItemsHook = ClearBlockedItemsSig.GetHook<nCdwyVmx28cSFhm8RbO>(NBmv84rKGH);
		}
		ClearBlockedItemsHook.Enable();
		alwaysAllowAllMateriaPatch.Enable();
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FE020 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7), new AddonEventDelegate(c0Zvs9hPxp));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E003B5E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa), new AddonEventDelegate(c0Zvs9hPxp));
		if ((*Addons.MateriaAttach).IsAddonAndNodesReady())
		{
			c0Zvs9hPxp((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { c0Zvs9hPxp });
	}

	public void StartBatchAutoMateriaAttach()
	{
		base.TaskHelper.Abort();
		if (!H0rviu23Oo())
		{
			swfvv0KgF0();
			return;
		}
		singleMeldingTarget = null;
		kO5vMLLdKy();
	}

	private void eTlv1He04q(pxgVHKmo62VsKBeM4fK materiaInfo, int itemIndex = 0)
	{
		base.TaskHelper.Abort();
		if (!mu9vPSL4H0(materiaInfo, itemIndex))
		{
			swfvv0KgF0();
			return;
		}
		singleMeldingTarget = new uTmPD7mgXEISZyRMsCN(materiaInfo.ItemID, itemIndex);
		kO5vMLLdKy();
	}

	private unsafe void kO5vMLLdKy()
	{
		base.TaskHelper.Enqueue(() => (*Addons.MateriaAttach).IsAddonAndNodesReady());
		base.TaskHelper.Enqueue(delegate
		{
			(*Addons.MateriaAttach).Callback(0, 0);
		});
		base.TaskHelper.Enqueue((Action)YANv43Y5Mg, (string?)null, 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private unsafe void YANv43Y5Mg()
	{
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass24_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass24_0();
		if (PFuvlkascq(out CS_0024_003C_003E8__locals6.inventoryType, out CS_0024_003C_003E8__locals6.slot, out CS_0024_003C_003E8__locals6.materiaID))
		{
			base.TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)1]);
			base.TaskHelper.Enqueue(delegate
			{
				((AgentId)45).SendEvent(1uL, 1, 0, 1, 0);
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB04D3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9));
			base.TaskHelper.Enqueue((Func<bool>)z4Jvo1gH6r, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E239A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
			base.TaskHelper.Enqueue(delegate
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_004c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0051: Unknown result type (might be due to invalid IL or missing references)
				//IL_0054: Unknown result type (might be due to invalid IL or missing references)
				//IL_0059: Unknown result type (might be due to invalid IL or missing references)
				AgentMateriaAttach* agentByInternalId = (AgentMateriaAttach*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)45);
				InventoryItem* inventorySlot = ((InventoryManager)InventoryManager.Instance()).GetInventorySlot(CS_0024_003C_003E8__locals6.inventoryType, (int)CS_0024_003C_003E8__locals6.slot);
				MateriaAttachEntry* itemArraySorted = *((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).ItemArraySorted;
				((MateriaAttachEntry)itemArraySorted).Item = inventorySlot;
				Item value = LuminaGetter.GetRow<Item>(((InventoryItem)inventorySlot).ItemId).Value;
				((MateriaAttachEntry)itemArraySorted).ItemLevel = (short)((Item)(ref value)).LevelItem.RowId;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F362BD4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
			base.TaskHelper.Enqueue(delegate
			{
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x79DDE0F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def), 100u))
				{
					return false;
				}
				AgentMateriaAttach* agentByInternalId = (AgentMateriaAttach*)((AgentModule)AgentModule.Instance()).GetAgentByInternalId((AgentId)45);
				for (int i = 0; i < ((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).MateriaSorted.Length; i++)
				{
					Pointer<MateriaAttachEntry> val = ((MateriaAttachData)((AgentMateriaAttach)agentByInternalId).Data).MateriaSorted[i];
					if (((MateriaAttachEntry)val.Value).Item != null && ((InventoryItem)((MateriaAttachEntry)val.Value).Item).ItemId == CS_0024_003C_003E8__locals6.materiaID)
					{
						((AgentId)45).SendEvent(1uL, 2, i, 1, 0);
						return true;
					}
				}
				return false;
			}, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35A846 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			base.TaskHelper.DelayNext(100);
			base.TaskHelper.Enqueue(() => DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)1], LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA473C8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55));
			base.TaskHelper.Enqueue((Action)YANv43Y5Mg, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317CE97 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273), 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
		}
		else
		{
			string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469AB157 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(text);
			singleMeldingTarget = null;
		}
	}

	private unsafe void c0Zvs9hPxp(AddonEvent type, AddonArgs args)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Invalid comparison between Unknown and I4
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		Overlay overlay = base.Overlay;
		bool isOpen = (int)type == 1 || ((int)type != 6 && ((Window)base.Overlay).IsOpen);
		((Window)overlay).IsOpen = isOpen;
		if ((int)type != 6)
		{
			return;
		}
		InventoryManager* ptr = InventoryManager.Instance();
		foreach (InventoryType item in Inventories.PlayerWithArmory)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				((InventoryManager)InventoryManager.Instance()).SetSlotUnblocked(item, (short)i);
			}
		}
	}

	private unsafe void NBmv84rKGH(MateriaAttachData* data)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		InventoryManager* ptr = InventoryManager.Instance();
		foreach (InventoryType item in Inventories.PlayerWithArmory)
		{
			InventoryContainer* inventoryContainer = ((InventoryManager)ptr).GetInventoryContainer(item);
			for (int i = 0; i < ((InventoryContainer)inventoryContainer).Size; i++)
			{
				((InventoryManager)InventoryManager.Instance()).SetSlotUnblocked(item, (short)i);
			}
		}
		ClearBlockedItemsHook.Original(data);
	}

	private static void swfvv0KgF0()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string text = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591C89BB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
		OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
		OmenServiceBase<NotifyHelper>.Instance().NotificationError(text);
	}

	private bool PFuvlkascq(out InventoryType inventoryType, out short slot, out uint materiaID)
	{
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected I4, but got Unknown
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		inventoryType = (InventoryType)9999;
		materiaID = 0u;
		slot = 0;
		if (config.CurrentMeldingPlan?.MeldingInfos == null)
		{
			return false;
		}
		foreach (IGrouping<uint, pxgVHKmo62VsKBeM4fK> item in from info in config.CurrentMeldingPlan.MeldingInfos
			group info by info.ItemID)
		{
			_003C_003Ec__DisplayClass28_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass28_0();
			CS_0024_003C_003E8__locals3.itemID = item.Key;
			List<pxgVHKmo62VsKBeM4fK> list = item.ToList();
			if ((singleMeldingTarget.HasValue && singleMeldingTarget.Value.ItemID != CS_0024_003C_003E8__locals3.itemID) || !((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetItems((Predicate<InventoryItem>)((InventoryItem i) => i.ItemId == CS_0024_003C_003E8__locals3.itemID), out List<InventoryItem> itemResult) || itemResult.Count <= 0)
			{
				continue;
			}
			for (int num = 0; num < list.Count; num++)
			{
				if ((singleMeldingTarget.HasValue && singleMeldingTarget.Value.ItemIndex != num) || num >= itemResult.Count)
				{
					continue;
				}
				pxgVHKmo62VsKBeM4fK pxgVHKmo62VsKBeM4fK2 = list[num];
				InventoryItem val = itemResult[num];
				List<uint> materiaIDs = pxgVHKmo62VsKBeM4fK2.MateriaIDs;
				if (materiaIDs == null || materiaIDs.Count == 0 || W9EvpbaFj9(val, pxgVHKmo62VsKBeM4fK2))
				{
					continue;
				}
				for (int num2 = 0; num2 < pxgVHKmo62VsKBeM4fK2.MateriaIDs.Count; num2++)
				{
					uint num3 = pxgVHKmo62VsKBeM4fK2.MateriaIDs[num2];
					if (num2 >= 5 || ((InventoryItem)(ref val)).Materia[num2] == 0 || !tmsv3xqeGB(new zeD1U3mPqm2MXRQBwgd(((InventoryItem)(ref val)).Materia[num2], ((InventoryItem)(ref val)).MateriaGrades[num2]), num3))
					{
						if (wm0vmmXdGK(num3) < 1)
						{
							swfvv0KgF0();
							base.TaskHelper.Abort();
							return false;
						}
						inventoryType = (InventoryType)(int)val.Container;
						materiaID = num3;
						slot = val.Slot;
						return true;
					}
				}
			}
		}
		return false;
	}

	private static bool YGZvxJrg0k(InventoryItem item, List<uint> requiredMateriaIds, out bool isCompleted, out List<zeD1U3mPqm2MXRQBwgd> attachedMateria)
	{
		attachedMateria = new List<zeD1U3mPqm2MXRQBwgd>();
		isCompleted = false;
		for (int i = 0; i < 5 && ((InventoryItem)(ref item)).Materia[i] != 0; i++)
		{
			attachedMateria.Add(new zeD1U3mPqm2MXRQBwgd(((InventoryItem)(ref item)).Materia[i], ((InventoryItem)(ref item)).MateriaGrades[i]));
		}
		if (attachedMateria.Count < requiredMateriaIds.Count)
		{
			return true;
		}
		for (int j = 0; j < requiredMateriaIds.Count; j++)
		{
			if (!tmsv3xqeGB(attachedMateria[j], requiredMateriaIds[j]))
			{
				return true;
			}
		}
		isCompleted = attachedMateria.Count >= requiredMateriaIds.Count;
		return true;
	}

	private static bool W9EvpbaFj9(InventoryItem item, pxgVHKmo62VsKBeM4fK materiaInfo)
	{
		for (int i = 0; i < materiaInfo.MateriaIDs.Count && i < 5; i++)
		{
			if (((InventoryItem)(ref item)).Materia[i] != 0 && !tmsv3xqeGB(new zeD1U3mPqm2MXRQBwgd(((InventoryItem)(ref item)).Materia[i], ((InventoryItem)(ref item)).MateriaGrades[i]), materiaInfo.MateriaIDs[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool tmsv3xqeGB(zeD1U3mPqm2MXRQBwgd attachedMateria, uint requiredMateriaID)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Materia>(attachedMateria.MateriaID, out Materia item) || attachedMateria.Grade >= ((Materia)(ref item)).Item.Count)
		{
			return false;
		}
		Item value = ((Materia)(ref item)).Item[(int)attachedMateria.Grade].Value;
		return ((Item)(ref value)).RowId == requiredMateriaID;
	}

	private static Item I9VvBc7Rqi(zeD1U3mPqm2MXRQBwgd materia)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!LuminaGetter.TryGetRow<Materia>(materia.MateriaID, out Materia item) || materia.Grade >= ((Materia)(ref item)).Item.Count)
		{
			return LuminaGetter.GetRow<Item>(1u).GetValueOrDefault();
		}
		return ((Materia)(ref item)).Item[(int)materia.Grade].Value;
	}

	private static int wm0vmmXdGK(uint materiaID)
	{
		_003C_003Ec__DisplayClass33_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass33_0();
		CS_0024_003C_003E8__locals2.materiaID = materiaID;
		if (!((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetItems((Predicate<InventoryItem>)((InventoryItem i) => i.ItemId == CS_0024_003C_003E8__locals2.materiaID), out List<InventoryItem> itemResult))
		{
			return 0;
		}
		return itemResult.Sum((InventoryItem item) => item.Quantity);
	}

	private bool H0rviu23Oo()
	{
		if (g4FvhCSYxV(out Dictionary<uint, VBPCbFmhCSmnVu0mgbR> stats))
		{
			return !stats.Any((KeyValuePair<uint, VBPCbFmhCSmnVu0mgbR> kvp) => kvp.Value.Available < kvp.Value.Required);
		}
		return false;
	}

	private static bool mu9vPSL4H0(pxgVHKmo62VsKBeM4fK materiaInfo, int itemIndex)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass35_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass35_0();
		CS_0024_003C_003E8__locals6.materiaInfo = materiaInfo;
		Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
		if (!((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetItems((Predicate<InventoryItem>)((InventoryItem i) => i.ItemId == CS_0024_003C_003E8__locals6.materiaInfo.ItemID), out List<InventoryItem> itemResult) || itemResult.Count <= itemIndex)
		{
			return false;
		}
		InventoryItem item = itemResult[itemIndex];
		if (W9EvpbaFj9(item, CS_0024_003C_003E8__locals6.materiaInfo))
		{
			return false;
		}
		for (int num = 0; num < CS_0024_003C_003E8__locals6.materiaInfo.MateriaIDs.Count && num < 5; num++)
		{
			if (((InventoryItem)(ref item)).Materia[num] == 0 || !tmsv3xqeGB(new zeD1U3mPqm2MXRQBwgd(((InventoryItem)(ref item)).Materia[num], ((InventoryItem)(ref item)).MateriaGrades[num]), CS_0024_003C_003E8__locals6.materiaInfo.MateriaIDs[num]))
			{
				uint key = CS_0024_003C_003E8__locals6.materiaInfo.MateriaIDs[num];
				dictionary[key] = dictionary.GetValueOrDefault(key, 0) + 1;
			}
		}
		foreach (var (materiaID, num4) in dictionary)
		{
			if (wm0vmmXdGK(materiaID) < num4)
			{
				return false;
			}
		}
		return true;
	}

	private bool g4FvhCSYxV(out Dictionary<uint, VBPCbFmhCSmnVu0mgbR> stats)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		stats = new Dictionary<uint, VBPCbFmhCSmnVu0mgbR>();
		Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
		if (config.CurrentMeldingPlan?.MeldingInfos == null)
		{
			return true;
		}
		foreach (IGrouping<uint, pxgVHKmo62VsKBeM4fK> item2 in from info in config.CurrentMeldingPlan.MeldingInfos
			group info by info.ItemID)
		{
			_003C_003Ec__DisplayClass36_0 CS_0024_003C_003E8__locals2 = new _003C_003Ec__DisplayClass36_0();
			CS_0024_003C_003E8__locals2.itemID = item2.Key;
			List<pxgVHKmo62VsKBeM4fK> list = item2.ToList();
			((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetItems((Predicate<InventoryItem>)((InventoryItem i) => i.ItemId == CS_0024_003C_003E8__locals2.itemID), out List<InventoryItem> itemResult);
			if (itemResult == null)
			{
				itemResult = new List<InventoryItem>();
			}
			for (int num = 0; num < list.Count; num++)
			{
				pxgVHKmo62VsKBeM4fK pxgVHKmo62VsKBeM4fK2 = list[num];
				List<uint> materiaIDs = pxgVHKmo62VsKBeM4fK2.MateriaIDs;
				if (materiaIDs == null || materiaIDs.Count == 0)
				{
					continue;
				}
				if (num < itemResult.Count)
				{
					InventoryItem item = itemResult[num];
					if (W9EvpbaFj9(item, pxgVHKmo62VsKBeM4fK2))
					{
						continue;
					}
					for (int num2 = 0; num2 < pxgVHKmo62VsKBeM4fK2.MateriaIDs.Count; num2++)
					{
						uint num3 = pxgVHKmo62VsKBeM4fK2.MateriaIDs[num2];
						if (num2 >= 5 || ((InventoryItem)(ref item)).Materia[num2] == 0 || !tmsv3xqeGB(new zeD1U3mPqm2MXRQBwgd(((InventoryItem)(ref item)).Materia[num2], ((InventoryItem)(ref item)).MateriaGrades[num2]), num3))
						{
							dictionary[num3] = dictionary.GetValueOrDefault(num3, 0) + 1;
						}
					}
					continue;
				}
				foreach (uint materiaID in pxgVHKmo62VsKBeM4fK2.MateriaIDs)
				{
					dictionary[materiaID] = dictionary.GetValueOrDefault(materiaID, 0) + 1;
				}
			}
		}
		foreach (var (num6, required) in dictionary)
		{
			stats[num6] = new VBPCbFmhCSmnVu0mgbR(required, wm0vmmXdGK(num6), num6);
		}
		return true;
	}

	private unsafe static bool z4Jvo1gH6r()
	{
		if (Addons.MateriaAttach != null)
		{
			return ((AtkValue)((byte*)((AtkUnitBase)Addons.MateriaAttach).AtkValues + (nint)287 * (nint)Unsafe.SizeOf<AtkValue>())).Int != -1;
		}
		return false;
	}

	protected override void ConfigUI()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC3BF4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361))));
	}

	protected unsafe override void OverlayUI()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			bool isHoveredAnyItem = false;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F39FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825)), 2, (ImGuiTableFlags)1);
			try
			{
				if (!val)
				{
					return;
				}
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695B9F9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)), (ImGuiTableColumnFlags)8, 220f * Globals.GlobalUIScale, 0u);
				ImGui.TableSetupColumn(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B1996 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848)), (ImGuiTableColumnFlags)4, 0f, 0u);
				ImGui.TableNextRow();
				ImGui.TableNextColumn();
				ChildDisposable val2 = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE8404 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)), new Vector2(-1f), true, (ImGuiWindowFlags)152);
				try
				{
					if (ChildDisposable.op_Implicit(val2))
					{
						fP8vgW5sFk();
					}
				}
				finally
				{
					((ChildDisposable)(ref val2)).Dispose();
				}
				ImGui.TableNextColumn();
				ChildDisposable val3 = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x9B40814 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2f1c0cb46b4440d499a69ed8afe22360)), new Vector2(-1f), true, (ImGuiWindowFlags)152);
				try
				{
					if (ChildDisposable.op_Implicit(val3))
					{
						tOvvc2rMpO(ref isHoveredAnyItem);
					}
				}
				finally
				{
					((ChildDisposable)(ref val3)).Dispose();
				}
				if (isHoveredAnyItem)
				{
					isItemTooltipActive = true;
				}
				else if (isItemTooltipActive)
				{
					isItemTooltipActive = false;
					if (Addons.ScreenText != null)
					{
						(*AtkStage.Instance()).HideTooltip(((AtkUnitBase)Addons.ScreenText).Id);
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
		}
	}

	private void fP8vgW5sFk()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		bool isBusy = base.TaskHelper.IsBusy;
		float num = (isBusy ? (ImGui.GetTextLineHeight() * 2.8f + 10f * Globals.GlobalUIScale) : 0f);
		float num2;
		if (!isBusy)
		{
			num2 = -1f;
		}
		else
		{
			float num3 = 0f - num;
			ImGuiStylePtr style = ImGui.GetStyle();
			num2 = num3 - ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
		}
		float y = num2;
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EB435 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)), new Vector2(-1f, y), false, (ImGuiWindowFlags)136);
		try
		{
			if (ChildDisposable.op_Implicit(val))
			{
				YSYvwhK4eq();
				ImGui.Spacing();
				switch (currentSidebarTab)
				{
				case SidebarTab.Plans:
					WJuvEayRdd();
					break;
				case SidebarTab.Import:
					pxsvFtm1jC();
					break;
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
		if (isBusy)
		{
			oO9vq52e4e(num);
		}
	}

	private void YSYvwhK4eq()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		float num = ImGui.GetTextLineHeight() * 1.15f + 6f * Globals.GlobalUIScale;
		float num2 = x / (float)SidebarTabs.Length;
		float num3 = 4f * Globals.GlobalUIScale;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + new Vector2(x, num);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, ImGui.GetColorU32((ImGuiCol)7, 0.35f), num3);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, ImGui.GetColorU32((ImGuiCol)5, 0.2f), num3, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
		ImU8String val = default(ImU8String);
		for (int i = 0; i < SidebarTabs.Length; i++)
		{
			SidebarTab sidebarTab = SidebarTabs[i];
			bool flag = currentSidebarTab == sidebarTab;
			string text = ((sidebarTab != SidebarTab.Import) ? FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61642) : FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61543));
			string text2 = text;
			text = ((sidebarTab != SidebarTab.Import) ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5207C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E49261 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7)));
			string text3 = text;
			int num4 = ((sidebarTab == SidebarTab.Plans) ? config.AllMeldingPlans.Count : 0);
			Vector2 vector2 = cursorScreenPos + new Vector2((float)i * num2, 0f);
			Vector2 vector3 = vector2 + new Vector2(num2, num);
			ImGui.SetCursorScreenPos(vector2);
			((ImU8String)(ref val))._002Ector(14, 1);
			((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E1C50 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
			((ImU8String)(ref val)).AppendFormatted<int>((int)sidebarTab);
			if (ImGui.InvisibleButton(val, new Vector2(num2, num), (ImGuiButtonFlags)0))
			{
				currentSidebarTab = sidebarTab;
				planToDeleteIndex = -1;
			}
			bool flag2 = ImGui.IsItemHovered();
			if (flag2)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			if (flag)
			{
				uint num5 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag2 ? 0.35f : 0.25f).ToUInt();
				uint num6 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.85f).ToUInt();
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector3, num5, num3);
				((ImDrawListPtr)(ref windowDrawList)).AddRect(vector2, vector3, num6, num3, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			}
			else if (flag2)
			{
				uint colorU = ImGui.GetColorU32((ImGuiCol)8, 0.4f);
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector2, vector3, colorU, num3);
			}
			string text4;
			if (num4 <= 0)
			{
				text4 = text2 + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE337BD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818) + text3;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 3);
				defaultInterpolatedStringHandler.AppendFormatted(text2);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FD869 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009));
				defaultInterpolatedStringHandler.AppendFormatted(text3);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854E197 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
				defaultInterpolatedStringHandler.AppendFormatted(num4);
				defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF205ACF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
				text4 = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			string text5 = text4;
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text5), false, -1f);
				Vector2 vector5 = new Vector2(vector2.X + MathF.Max(2f, (num2 - vector4.X) / 2f), vector2.Y + (num - vector4.Y) / 2f);
				uint num7 = (flag ? KnownColor.Chocolate.ToUInt() : (flag2 ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)1)));
				((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector2, vector3, true);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, num7, ImU8String.op_Implicit(text5));
				((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			}
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, num));
	}

	private void WJuvEayRdd()
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		int count = config.AllMeldingPlans.Count;
		if (count < 4)
		{
			if (count == 0)
			{
				lk4v6e2Wt2(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61468), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE58753 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1402E9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)));
				return;
			}
		}
		else
		{
			float x = ImGui.GetContentRegionAvail().X;
			bool num = !string.IsNullOrEmpty(planSearchInput);
			float frameHeight = ImGui.GetFrameHeight();
			if (num)
			{
				float num2 = x - frameHeight;
				ImGuiStylePtr style = ImGui.GetStyle();
				ImGui.SetNextItemWidth(num2 - ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
			}
			else
			{
				ImGui.SetNextItemWidth(x);
			}
			ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3B738E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C173431 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb))), ref planSearchInput, 128, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (num)
			{
				ImGui.SameLine();
				if (ImGui.Button(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE866E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)), new Vector2(frameHeight, 0f)))
				{
					planSearchInput = string.Empty;
				}
				ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C1433DF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)));
			}
			ImGui.Spacing();
		}
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE72553 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6)), new Vector2(-1f, -1f), false, (ImGuiWindowFlags)128);
		try
		{
			if (!val)
			{
				return;
			}
			for (int i = 0; i < config.AllMeldingPlans.Count; i++)
			{
				nSiWNGmmrHVI7N2Bt9j nSiWNGmmrHVI7N2Bt9j2 = config.AllMeldingPlans[i];
				if (string.IsNullOrWhiteSpace(planSearchInput) || nSiWNGmmrHVI7N2Bt9j2.Name.Contains(planSearchInput, StringComparison.OrdinalIgnoreCase))
				{
					xvIvbLn3W6(i, nSiWNGmmrHVI7N2Bt9j2);
				}
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private void xvIvbLn3W6(int index, nSiWNGmmrHVI7N2Bt9j plan)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b5: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(9, 1);
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACCE34 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
		((ImU8String)(ref val)).AppendFormatted<int>(index);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			bool flag = config.CurrentMeldingPlan?.Name == plan.Name;
			float x = ImGui.GetContentRegionAvail().X;
			float num = ImGui.GetTextLineHeight() + 12f * Globals.GlobalUIScale;
			float num2 = 22f * Globals.GlobalUIScale;
			float num3 = 8f * Globals.GlobalUIScale;
			bool flag2 = !string.IsNullOrWhiteSpace(plan.OriginalURL);
			float num4 = x - (flag2 ? (num2 * 2f) : num2);
			Vector2 vector = new Vector2(x, num);
			float num5 = 4f * Globals.GlobalUIScale;
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			Vector2 vector2 = cursorScreenPos + vector;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			ImGui.SetCursorScreenPos(cursorScreenPos);
			ImU8String val3 = new ImU8String(17, 1);
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE097125 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363));
			((ImU8String)(ref val3)).AppendFormatted<int>(index);
			if (ImGui.InvisibleButton(val3, new Vector2(num4, num), (ImGuiButtonFlags)0))
			{
				config.CurrentMeldingPlan = plan;
				config.Save(this);
				planToDeleteIndex = -1;
			}
			bool flag3 = ImGui.IsItemHovered();
			if (flag3)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			}
			Vector4 color;
			Vector4 color2;
			if (flag)
			{
				color = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), flag3 ? 0.3f : 0.2f);
				color2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.85f);
			}
			else if (flag3)
			{
				color = ImGui.GetColorU32((ImGuiCol)8, 0.4f).ToVector4();
				color2 = ImGui.GetColorU32((ImGuiCol)5, 0.35f).ToVector4();
			}
			else
			{
				color = ImGui.GetColorU32((ImGuiCol)7, 0.22f).ToVector4();
				color2 = ImGui.GetColorU32((ImGuiCol)5, 0.12f).ToVector4();
			}
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector2, color.ToUInt(), num5);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector2, color2.ToUInt(), num5, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			Vector2 vector3 = new Vector2(cursorScreenPos.X + num3, cursorScreenPos.Y + (num - ImGui.GetTextLineHeight()) / 2f);
			Vector2 vector4 = cursorScreenPos;
			Vector2 vector5 = vector2;
			vector5.X = cursorScreenPos.X + num4 - 4f * Globals.GlobalUIScale;
			((ImDrawListPtr)(ref windowDrawList)).PushClipRect(vector4, vector5, true);
			using (OmenServiceBase<FontManager>.Instance().UIFont.Push())
			{
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, flag ? KnownColor.Chocolate.ToUInt() : ImGui.GetColorU32((ImGuiCol)0), ImU8String.op_Implicit(plan.Name));
			}
			((ImDrawListPtr)(ref windowDrawList)).PopClipRect();
			if (false)
			{
				vector5 = cursorScreenPos;
				vector5.X = vector2.X - num2 * 2f;
				Vector2 cursorScreenPos2 = vector5;
				Vector2 vector6 = new Vector2(num2, num);
				ImGui.SetCursorScreenPos(cursorScreenPos2);
				ImU8String val4 = new ImU8String(11, 1);
				((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D118D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
				((ImU8String)(ref val4)).AppendFormatted<int>(index);
				if (ImGui.InvisibleButton(val4, vector6, (ImGuiButtonFlags)0))
				{
					_ = plan.OriginalURL;
				}
				bool num6 = ImGui.IsItemHovered();
				if (num6)
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE39EAD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
				}
				string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62301);
				uint num7 = (num6 ? KnownColor.Chocolate.ToUInt() : ImGui.GetColorU32((ImGuiCol)1, 0.5f));
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					Vector2 vector7 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
					Vector2 vector8 = new Vector2(cursorScreenPos2.X + (vector6.X - vector7.X) / 2f, cursorScreenPos2.Y + (vector6.Y - vector7.Y) / 2f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector8, num7, ImU8String.op_Implicit(text));
				}
			}
			bool flag4 = planToDeleteIndex == index;
			vector5 = cursorScreenPos;
			vector5.X = vector2.X - num2;
			Vector2 cursorScreenPos3 = vector5;
			Vector2 vector9 = new Vector2(num2, num);
			ImGui.SetCursorScreenPos(cursorScreenPos3);
			ImU8String val5 = new ImU8String(10, 1);
			((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x4C9DB2E5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3aeae81f85f84848b273be7b2bfbf497));
			((ImU8String)(ref val5)).AppendFormatted<int>(index);
			if (ImGui.InvisibleButton(val5, vector9, (ImGuiButtonFlags)0))
			{
				if (flag4)
				{
					if (config.CurrentMeldingPlan?.Name == plan.Name)
					{
						config.CurrentMeldingPlan = null;
					}
					config.AllMeldingPlans.RemoveAt(index);
					config.Save(this);
					planToDeleteIndex = -1;
					return;
				}
				planToDeleteIndex = index;
			}
			bool flag5 = ImGui.IsItemHovered();
			if (flag5)
			{
				ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				ImGuiOm.TooltipHover(flag4 ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695BB6D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9)) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE37C31 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
			}
			if (flag4 && ImGui.IsMouseClicked((ImGuiMouseButton)0) && !flag5)
			{
				planToDeleteIndex = -1;
			}
			string text2 = (flag4 ? FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452) : FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61944));
			uint num8 = (flag4 ? KnownColor.OrangeRed.ToUInt() : (flag5 ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)1, 0.5f)));
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				Vector2 vector10 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
				Vector2 vector11 = new Vector2(cursorScreenPos3.X + (vector9.X - vector10.X) / 2f, cursorScreenPos3.Y + (vector9.Y - vector10.Y) / 2f);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector11, num8, ImU8String.op_Implicit(text2));
			}
			ImGui.SetCursorScreenPos(cursorScreenPos);
			ImGui.Dummy(vector);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private void pxsvFtm1jC()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(1, 2);
		((ImU8String)(ref val)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)62831));
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E0DD5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467));
		((ImU8String)(ref val)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E52030 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3)));
		ImGui.TextColored(ref vector, val);
		ImGui.Spacing();
		bool flag = !string.IsNullOrEmpty(gearsetURLInput);
		float frameHeight = ImGui.GetFrameHeight();
		float frameHeight2 = ImGui.GetFrameHeight();
		float num = x - frameHeight;
		float num2;
		ImGuiStylePtr style;
		if (!flag)
		{
			num2 = 0f;
		}
		else
		{
			style = ImGui.GetStyle();
			num2 = frameHeight2 + ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
		}
		float num3 = num - num2;
		style = ImGui.GetStyle();
		ImGui.SetNextItemWidth(num3 - ((ImGuiStylePtr)(ref style)).ItemSpacing.X);
		ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x177802A7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1)), ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FDDD2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7)), ref gearsetURLInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		if (flag)
		{
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6F7C4 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)), new Vector2(frameHeight2, 0f)))
			{
				gearsetURLInput = string.Empty;
			}
			ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x3FF554F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)));
		}
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(11, 1);
		((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61674));
		((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD78869 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
		if (ImGui.Button(val2, new Vector2(frameHeight, 0f)))
		{
			string clipboardText = ImGui.GetClipboardText();
			if (!string.IsNullOrWhiteSpace(clipboardText))
			{
				gearsetURLInput = clipboardText.Trim();
			}
		}
		ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EB73D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec)));
		ImGui.Spacing();
		ImGui.SetNextItemWidth(x);
		ImGui.InputTextWithHint(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x39873CF5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5)), ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C35022A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def))), ref gearsetNameInput, 64, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		ImGui.Spacing();
		bool flag2 = string.IsNullOrWhiteSpace(gearsetURLInput) || string.IsNullOrWhiteSpace(gearsetNameInput);
		DisabledDisposable val3 = ImRaii.Disabled(flag2);
		try
		{
			ImU8String val4 = new ImU8String(2, 2);
			((ImU8String)(ref val4)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452));
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1EFAC6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423EE564 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71)));
			if (ImGui.Button(val4, new Vector2(x, ImGui.GetFrameHeight() * 1.25f)) && !flag2 && config.AllMeldingPlans.All((nSiWNGmmrHVI7N2Bt9j nSiWNGmmrHVI7N2Bt9j3) => nSiWNGmmrHVI7N2Bt9j3.Name != gearsetNameInput))
			{
				string text = gearsetURLInput.Trim();
				int num4 = text.LastIndexOf('?');
				string text2 = ((num4 == -1 || num4 >= text.Length - 1) ? text : text.Substring(num4 + 1));
				if (string.IsNullOrEmpty(text2))
				{
					goto IL_04b8;
				}
				List<pxgVHKmo62VsKBeM4fK> list = EtXT6vmEShWSiW3fWHS.HZimbpoIJ6(text2);
				if (list == null || list.Count <= 0)
				{
					goto IL_04b8;
				}
				nSiWNGmmrHVI7N2Bt9j nSiWNGmmrHVI7N2Bt9j2 = new nSiWNGmmrHVI7N2Bt9j
				{
					Name = gearsetNameInput,
					OriginalURL = text,
					MeldingInfos = list
				};
				config.CurrentMeldingPlan = nSiWNGmmrHVI7N2Bt9j2;
				config.AllMeldingPlans.Add(nSiWNGmmrHVI7N2Bt9j2);
				config.Save(this);
				gearsetURLInput = string.Empty;
				gearsetNameInput = string.Empty;
				currentSidebarTab = SidebarTab.Plans;
				OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-864210836 ^ -1568852358 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)));
			}
			goto end_IL_02fe;
			IL_04b8:
			OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CB28D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)));
			end_IL_02fe:;
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		ImGui.Spacing();
		ImGui.Separator();
		ImGui.Spacing();
		TextWrapDisposable val5 = ImRaii.TextWrapPos(x);
		try
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5357462A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb))));
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
	}

	private void oO9vq52e4e(float cardHeight)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.GetContentRegionAvail().X;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = cursorScreenPos + new Vector2(x, cardHeight);
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 4f * Globals.GlobalUIScale;
		float num2 = 6f * Globals.GlobalUIScale;
		float num3 = 5f * Globals.GlobalUIScale;
		Vector2 vector2 = new Vector2(16f * Globals.GlobalUIScale, 16f * Globals.GlobalUIScale);
		Vector2 cursorScreenPos2 = new Vector2(vector.X - num2 - vector2.X, cursorScreenPos.Y + num3);
		ImGui.SetCursorScreenPos(cursorScreenPos2);
		if (ImGui.InvisibleButton(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D799D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846)), vector2, (ImGuiButtonFlags)0))
		{
			base.TaskHelper.Abort();
		}
		bool flag = ImGui.IsItemHovered();
		if (flag)
		{
			ImGui.SetMouseCursor((ImGuiMouseCursor)7);
			ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x21946C04 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)));
		}
		uint num4 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.18f).ToUInt();
		uint num5 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.75f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector, num4, num);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector, num5, num, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61459) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F7534 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17780507 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(cursorScreenPos.X + num2, cursorScreenPos.Y + num3), KnownColor.Chocolate.ToUInt(), ImU8String.op_Implicit(text));
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			string text2 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61453);
			Vector2 vector3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
			Vector2 vector4 = new Vector2(cursorScreenPos2.X + (vector2.X - vector3.X) / 2f, cursorScreenPos2.Y + (vector2.Y - vector3.Y) / 2f);
			uint num6 = (flag ? KnownColor.OrangeRed.ToUInt() : ImGui.GetColorU32((ImGuiCol)1, 0.6f));
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, num6, ImU8String.op_Implicit(text2));
			string text3 = (singleMeldingTarget.HasValue ? (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE58353 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)) ?? "") : (LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CD004 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)) ?? ""));
			((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(cursorScreenPos.X + num2, cursorScreenPos.Y + num3 + ImGui.GetTextLineHeight() + 4f * Globals.GlobalUIScale), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text3));
		}
		ImGui.SetCursorScreenPos(cursorScreenPos + new Vector2(0f, cardHeight));
	}

	private void tOvvc2rMpO(ref bool isHoveredAnyItem)
	{
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0889: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_096b: Unknown result type (might be due to invalid IL or missing references)
		//IL_096d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0972: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		nSiWNGmmrHVI7N2Bt9j currentMeldingPlan = config.CurrentMeldingPlan;
		if (currentMeldingPlan == null || currentMeldingPlan.MeldingInfos.Count == 0)
		{
			lk4v6e2Wt2(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61468), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CB785 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)), LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1976305562 ^ 0x1F62827A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)));
			return;
		}
		List<rREVgcmw6gwyIYa4fy3> list = xkavkcI9XC();
		int count = list.Count;
		int num = list.Count((rREVgcmw6gwyIYa4fy3 rREVgcmw6gwyIYa4fy4) => rREVgcmw6gwyIYa4fy4.IsFound);
		int num2 = list.Count((rREVgcmw6gwyIYa4fy3 rREVgcmw6gwyIYa4fy4) => rREVgcmw6gwyIYa4fy4.IsCompleted);
		int num3 = list.Sum((rREVgcmw6gwyIYa4fy3 rREVgcmw6gwyIYa4fy4) => rREVgcmw6gwyIYa4fy4.MeldingInfo.MateriaIDs.Count);
		int num4 = 0;
		foreach (rREVgcmw6gwyIYa4fy3 item in list)
		{
			for (int num5 = 0; num5 < item.MeldingInfo.MateriaIDs.Count; num5++)
			{
				if (num5 < item.AttachedMateria.Count && tmsv3xqeGB(item.AttachedMateria[num5], item.MeldingInfo.MateriaIDs[num5]))
				{
					num4++;
				}
			}
		}
		g4FvhCSYxV(out Dictionary<uint, VBPCbFmhCSmnVu0mgbR> stats);
		bool flag = H0rviu23Oo();
		int num6 = stats.Values.Sum((VBPCbFmhCSmnVu0mgbR r) => Math.Max(0, r.Required - r.Available));
		float frameHeight;
		using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
		{
			frameHeight = ImGui.GetFrameHeight();
		}
		GroupDisposable val = ImRaii.Group();
		try
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
			{
				ImGui.AlignTextToFramePadding();
				ImGui.TextUnformatted(ImU8String.op_Implicit(currentMeldingPlan.Name));
				if (ImGui.IsItemHovered())
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				if (ImGui.IsItemClicked())
				{
					ImGui.SetClipboardText(ImU8String.op_Implicit(currentMeldingPlan.Name));
					OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F9FBF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17DB39 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb) + currentMeldingPlan.Name);
				}
			}
			bool flag2 = count > 0 && num2 == count;
			bool isBusy = base.TaskHelper.IsBusy;
			float x = ImGui.GetContentRegionAvail().X;
			float num7 = 120f * Globals.GlobalUIScale;
			float num8 = 75f * Globals.GlobalUIScale;
			float num9 = 65f * Globals.GlobalUIScale;
			ImGuiStylePtr style = ImGui.GetStyle();
			float x2 = ((ImGuiStylePtr)(ref style)).ItemSpacing.X;
			float num10 = num7 + x2 + num8 + (isBusy ? (x2 + num9) : 0f);
			ImGui.SameLine(x - num10);
			DisabledDisposable val2 = ImRaii.Disabled(isBusy || flag2);
			try
			{
				if (ImGuiOm.HoldButton(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D798D2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846), flag2 ? (FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D56C75A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EB179 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))) : (FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61515) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2F392A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE8166 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a))), null, autoReset: true, new Vector2(num7, frameHeight), 1.5f))
				{
					StartBatchAutoMateriaAttach();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			if (isBusy)
			{
				ImGui.SameLine();
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, KnownColor.OrangeRed.ToVector4(), true);
				try
				{
					ImU8String val4 = new ImU8String(2, 2);
					((ImU8String)(ref val4)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61517));
					((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E47565 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
					((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE314A3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
					if (ImGui.Button(val4, new Vector2(num9, frameHeight)))
					{
						base.TaskHelper.Abort();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.Spacing();
		List<sUJIEympPHpoiidxbEK> list2 = new List<sUJIEympPHpoiidxbEK>();
		string label = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68ACCA66 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4722689f1a4d41dc9dc800bfe1a89c09));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
		defaultInterpolatedStringHandler.AppendFormatted(num2);
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420CF26A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
		defaultInterpolatedStringHandler.AppendFormatted(count);
		list2.Add(new sUJIEympPHpoiidxbEK(label, defaultInterpolatedStringHandler.ToStringAndClear(), (num2 == count && count > 0) ? KnownColor.GreenYellow.ToVector4() : KnownColor.Chocolate.ToVector4(), null));
		string label2 = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194E2EC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
		defaultInterpolatedStringHandler2.AppendFormatted(num4);
		defaultInterpolatedStringHandler2.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF1C050 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
		defaultInterpolatedStringHandler2.AppendFormatted(num3);
		list2.Add(new sUJIEympPHpoiidxbEK(label2, defaultInterpolatedStringHandler2.ToStringAndClear(), (num4 == num3 && num3 > 0) ? KnownColor.GreenYellow.ToVector4() : KnownColor.Goldenrod.ToVector4(), null));
		list2.Add(new sUJIEympPHpoiidxbEK(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75A9CC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)), flag ? LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C3591FA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def)) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695BF4F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9), num6), flag ? KnownColor.GreenYellow.ToVector4() : KnownColor.OrangeRed.ToVector4(), null));
		list2.Add(new sUJIEympPHpoiidxbEK(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CD280 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948)), (num == count) ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53513208 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08) : LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--367535148 ^ 0x52BB070C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_790eb00575b3425495c924ac13469f4c), count - num), (num == count) ? KnownColor.GreenYellow.ToVector4() : KnownColor.OrangeRed.ToVector4(), null));
		QmuvroKiyf(list2);
		ImGui.Spacing();
		TabBarDisposable val5 = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F5F0E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414)), (ImGuiTabBarFlags)32);
		try
		{
			if (!val5)
			{
				return;
			}
			ImU8String val6 = new ImU8String(11, 1);
			((ImU8String)(ref val6)).AppendFormatted<string>(LuminaWrapper.GetAddonText(10402u));
			((ImU8String)(ref val6)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F361D62 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
			TabItemDisposable val7 = ImRaii.TabItem(val6, (ImGuiTabItemFlags)64);
			try
			{
				if (TabItemDisposable.op_Implicit(val7))
				{
					HTAvAtJKiP(list, ref isHoveredAnyItem);
				}
			}
			finally
			{
				((TabItemDisposable)(ref val7)).Dispose();
			}
			ImU8String val8 = new ImU8String(11, 1);
			((ImU8String)(ref val8)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F3192 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825)));
			((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E07E2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
			TabItemDisposable val9 = ImRaii.TabItem(val8);
			try
			{
				if (TabItemDisposable.op_Implicit(val9))
				{
					lcovJ51T1e(stats, ref isHoveredAnyItem);
				}
			}
			finally
			{
				((TabItemDisposable)(ref val9)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val5)).Dispose();
		}
	}

	private void HTAvAtJKiP(List<rREVgcmw6gwyIYa4fy3> displayItems, ref bool isHoveredAnyItem)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (displayItems.Count == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1998u)));
			return;
		}
		ChildDisposable val = ImRaii.Child(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D0E15 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e)), new Vector2(-1f, -1f), false, (ImGuiWindowFlags)128);
		try
		{
			if (!val)
			{
				return;
			}
			foreach (rREVgcmw6gwyIYa4fy3 displayItem in displayItems)
			{
				U36vQGctgR(displayItem, ref isHoveredAnyItem);
			}
		}
		finally
		{
			((ChildDisposable)(ref val)).Dispose();
		}
	}

	private unsafe void U36vQGctgR(rREVgcmw6gwyIYa4fy3 displayItem, ref bool isHoveredAnyItem)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0492: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a31: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a7e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b2a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b76: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0bcf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c70: Unknown result type (might be due to invalid IL or missing references)
		ImU8String val = default(ImU8String);
		((ImU8String)(ref val))._002Ector(11, 2);
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2FD79E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
		((ImU8String)(ref val)).AppendFormatted<uint>(displayItem.MeldingInfo.ItemID);
		((ImU8String)(ref val)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BF29C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded));
		((ImU8String)(ref val)).AppendFormatted<int>(displayItem.ItemIndex);
		IdDisposable val2 = ImRaii.PushId(val, true);
		try
		{
			float x = ImGui.GetContentRegionAvail().X;
			float num = 8f * Globals.GlobalUIScale;
			float num2 = 6f * Globals.GlobalUIScale;
			float num3 = 5f * Globals.GlobalUIScale;
			float num4 = 4f * Globals.GlobalUIScale;
			float num5 = MathF.Max(ImGui.GetFrameHeight(), ImGui.GetTextLineHeight() + 6f * Globals.GlobalUIScale);
			float num6 = ImGui.GetTextLineHeight() * 1.15f + 4f * Globals.GlobalUIScale;
			float num7 = num5 + num3 + num6;
			float y = num2 * 2f + num7;
			Vector2 vector = new Vector2(x, y);
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			Vector2 vector2 = cursorScreenPos + vector;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			bool flag = displayItem.IsFound && W9EvpbaFj9(displayItem.InventoryItem, displayItem.MeldingInfo);
			uint num8 = (displayItem.IsCompleted ? VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.08f).ToUInt() : (flag ? VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), 0.08f).ToUInt() : ImGui.GetColorU32((ImGuiCol)7, 0.22f)));
			uint num9 = (displayItem.IsCompleted ? VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.35f).ToUInt() : (flag ? VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), 0.4f).ToUInt() : ImGui.GetColorU32((ImGuiCol)5, 0.18f)));
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, vector2, num8, num4);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, vector2, num9, num4, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
			Vector2 vector3 = new Vector2(cursorScreenPos.X + num, cursorScreenPos.Y + num2);
			((ImDrawListPtr)(ref windowDrawList)).AddImage(displayItem.Icon.GetWrapOrEmpty().Handle, vector3, vector3 + new Vector2(num7));
			if (ImGui.IsMouseHoveringRect(vector3, vector3 + new Vector2(num7)))
			{
				if (Addons.ScreenText != null)
				{
					(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, displayItem.MeldingInfo.ItemID);
				}
				isHoveredAnyItem = true;
				ImGuiOm.TooltipHover(displayItem.ItemName);
			}
			float num10 = vector3.X + num7 + 8f * Globals.GlobalUIScale;
			float num11 = cursorScreenPos.Y + num2;
			float num12 = 105f * Globals.GlobalUIScale;
			Vector2 cursorScreenPos2 = new Vector2(vector2.X - num - num12, num11);
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				float num13 = num11 + (num5 - ImGui.GetTextLineHeight()) / 2f;
				((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num10, num13), displayItem.IsFound ? ImGui.GetColorU32((ImGuiCol)0) : KnownColor.Gray.ToUInt(), ImU8String.op_Implicit(displayItem.ItemName));
				if (!displayItem.IsFound)
				{
					float x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(displayItem.ItemName), false, -1f).X;
					bYkvGKJf7X(windowDrawList, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9E89C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37)), num10 + x2 + 6f * Globals.GlobalUIScale, num13 - Globals.GlobalUIScale, VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), 0.2f).ToUInt(), KnownColor.OrangeRed.ToUInt());
				}
			}
			ImGui.SetCursorScreenPos(cursorScreenPos2);
			if (displayItem.IsCompleted)
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
				{
					string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C357102 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E52548 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
					Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(vector2.X - num - vector4.X, num11 + (num5 - vector4.Y) / 2f), KnownColor.GreenYellow.ToUInt(), ImU8String.op_Implicit(text));
				}
			}
			else if (flag)
			{
				using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
				{
					string text2 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61553) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE37BA3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017FEB0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf));
					Vector2 vector5 = ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f);
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(vector2.X - num - vector5.X, num11 + (num5 - vector5.Y) / 2f), KnownColor.OrangeRed.ToUInt(), ImU8String.op_Implicit(text2));
				}
				Vector2 vector6 = new Vector2(vector2.X - num - 120f * Globals.GlobalUIScale, num11);
				Vector2 vector7 = vector2;
				vector7.Y = num11 + num5;
				if (ImGui.IsMouseHoveringRect(vector6, vector7))
				{
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3956D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818)));
				}
			}
			else if (displayItem.IsFound)
			{
				DisabledDisposable val3 = ImRaii.Disabled(base.TaskHelper.IsBusy);
				try
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5B058 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
					defaultInterpolatedStringHandler.AppendFormatted(displayItem.MeldingInfo.ItemID);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D752FAE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
					defaultInterpolatedStringHandler.AppendFormatted(displayItem.ItemIndex);
					if (ImGuiOm.HoldButton(defaultInterpolatedStringHandler.ToStringAndClear(), FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61613) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x10DF7AC8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec) + LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75AA2E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1)), null, autoReset: true, new Vector2(num12, num5)))
					{
						eTlv1He04q(displayItem.MeldingInfo, displayItem.ItemIndex);
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			float num14 = num11 + num5 + num3;
			float num15 = num10;
			ISharedImmediateTexture val5 = default(ISharedImmediateTexture);
			ISharedImmediateTexture val7 = default(ISharedImmediateTexture);
			for (int i = 0; i < displayItem.MeldingInfo.MateriaIDs.Count; i++)
			{
				uint num16 = displayItem.MeldingInfo.MateriaIDs[i];
				if (!LuminaGetter.TryGetRow<Item>(num16, out Item item))
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val4 = new GameIconLookup((uint)((Item)(ref item)).Icon, false, true, (ClientLanguage?)null);
				if (!obj.TryGetFromGameIcon(ref val4, ref val5))
				{
					continue;
				}
				bool flag2 = displayItem.IsFound && i < displayItem.AttachedMateria.Count;
				bool flag3 = flag2 && tmsv3xqeGB(displayItem.AttachedMateria[i], num16);
				float num17 = 3f * Globals.GlobalUIScale;
				float num18 = 4f * Globals.GlobalUIScale;
				float num19 = num6 - 4f * Globals.GlobalUIScale;
				string text3 = ((uint)(i + 1)).ToSESquareCount() ?? "";
				string text4 = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
				float x3;
				float x4;
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					x3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f).X;
					x4 = ImGui.CalcTextSize(ImU8String.op_Implicit(text4), false, -1f).X;
				}
				float num20 = num18 * 3f + x3 + num19 + x4;
				if (flag2 && !flag3)
				{
					num20 += num19 + 4f * Globals.GlobalUIScale;
				}
				Vector2 vector8 = new Vector2(num15, num14);
				Vector2 vector9 = new Vector2(num15 + num20, num14 + num6);
				uint num21;
				uint num22;
				uint num23;
				if (flag3)
				{
					num21 = VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.18f).ToUInt();
					num22 = VectorExtensions.WithW(KnownColor.GreenYellow.ToVector4(), 0.6f).ToUInt();
					num23 = KnownColor.GreenYellow.ToUInt();
				}
				else if (flag2)
				{
					num21 = VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), 0.2f).ToUInt();
					num22 = VectorExtensions.WithW(KnownColor.OrangeRed.ToVector4(), 0.7f).ToUInt();
					num23 = KnownColor.OrangeRed.ToUInt();
				}
				else
				{
					num21 = ImGui.GetColorU32((ImGuiCol)7, 0.3f);
					num22 = ImGui.GetColorU32((ImGuiCol)5, 0.2f);
					num23 = ImGui.GetColorU32((ImGuiCol)1);
				}
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector8, vector9, num21, num17);
				((ImDrawListPtr)(ref windowDrawList)).AddRect(vector8, vector9, num22, num17, (ImDrawFlags)0, 1f * Globals.GlobalUIScale);
				float num24 = vector8.X + num18;
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num24, vector8.Y + (num6 - ImGui.GetTextLineHeight()) / 2f), num23, ImU8String.op_Implicit(text3));
				}
				num24 += x3 + num18;
				Vector2 vector10 = new Vector2(num24, vector8.Y + 2f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref windowDrawList)).AddImage(val5.GetWrapOrEmpty().Handle, vector10, vector10 + new Vector2(num19));
				if (ImGui.IsMouseHoveringRect(vector10, vector10 + new Vector2(num19)))
				{
					if (Addons.ScreenText != null)
					{
						(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, num16);
					}
					isHoveredAnyItem = true;
					ImGuiOm.TooltipHover(text4);
				}
				num24 += num19 + 2f * Globals.GlobalUIScale;
				using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
				{
					((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(num24, vector8.Y + (num6 - ImGui.GetTextLineHeight()) / 2f), num23, ImU8String.op_Implicit(text4));
				}
				if (flag2 && !flag3)
				{
					num24 += x4 + 4f * Globals.GlobalUIScale;
					Item val6 = I9VvBc7Rqi(displayItem.AttachedMateria[i]);
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					val4 = new GameIconLookup((uint)((Item)(ref val6)).Icon, false, true, (ClientLanguage?)null);
					if (obj2.TryGetFromGameIcon(ref val4, ref val7))
					{
						Vector2 vector11 = new Vector2(num24, vector8.Y + 2f * Globals.GlobalUIScale);
						((ImDrawListPtr)(ref windowDrawList)).AddImage(val7.GetWrapOrEmpty().Handle, vector11, vector11 + new Vector2(num19));
						if (ImGui.IsMouseHoveringRect(vector11, vector11 + new Vector2(num19)))
						{
							if (Addons.ScreenText != null)
							{
								(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, ((Item)(ref val6)).RowId);
							}
							isHoveredAnyItem = true;
							ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1487420814 ^ -1249820922 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FFACD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009) + ((object)((Item)(ref val6)).Name/*cast due to constrained. prefix*/).ToString());
						}
					}
				}
				num15 += num20 + 4f * Globals.GlobalUIScale;
			}
			ImGui.SetCursorScreenPos(cursorScreenPos);
			ImGui.Dummy(vector);
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}

	private unsafe static void lcovJ51T1e(Dictionary<uint, VBPCbFmhCSmnVu0mgbR> reqStats, ref bool isHoveredAnyItem)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ba: Unknown result type (might be due to invalid IL or missing references)
		if (reqStats.Count == 0)
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(1998u)));
			return;
		}
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C140A97 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6)), 5, (ImGuiTableFlags)33556353, new Vector2(-1f, ImGui.GetContentRegionAvail().Y));
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(4457u)), (ImGuiTableColumnFlags)4, 40f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B8D6E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848))), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(358u)), (ImGuiTableColumnFlags)4, 25f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE72D5F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6))), (ImGuiTableColumnFlags)4, 15f, 0u);
			ImGui.TableSetupColumn(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351D12A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))), (ImGuiTableColumnFlags)8, ImGui.CalcTextSize(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7BDBD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f))), false, -1f).X + 16f * Globals.GlobalUIScale, 0u);
			ImGui.TableHeadersRow();
			ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
			foreach (var (num2, vBPCbFmhCSmnVu0mgbR2) in reqStats)
			{
				if (!LuminaGetter.TryGetRow<Item>(num2, out Item item))
				{
					continue;
				}
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val2 = new GameIconLookup((uint)((Item)(ref item)).Icon, false, true, (ClientLanguage?)null);
				if (!obj.TryGetFromGameIcon(ref val2, ref val3))
				{
					continue;
				}
				ImU8String val4 = new ImU8String(11, 1);
				((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C4246B6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
				((ImU8String)(ref val4)).AppendFormatted<uint>(num2);
				IdDisposable val5 = ImRaii.PushId(val4, true);
				try
				{
					ImGui.TableNextRow();
					bool num3 = vBPCbFmhCSmnVu0mgbR2.Available >= vBPCbFmhCSmnVu0mgbR2.Required;
					int num4 = Math.Max(0, vBPCbFmhCSmnVu0mgbR2.Required - vBPCbFmhCSmnVu0mgbR2.Available);
					ImGui.TableNextColumn();
					float textLineHeight = ImGui.GetTextLineHeight();
					ImGui.Image(val3.GetWrapOrEmpty().Handle, new Vector2(textLineHeight));
					if (ImGui.IsItemHovered())
					{
						if (Addons.ScreenText != null)
						{
							(*AtkStage.Instance()).ShowItemTooltip(((AtkUnitBase)Addons.ScreenText).RootNode, ((Item)(ref item)).RowId);
						}
						isHoveredAnyItem = true;
					}
					ImGui.SameLine();
					ImGui.TextUnformatted(ImU8String.op_Implicit(((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
					ImGuiOmExtension.ClickToCopyAndNotify(((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
					ImGui.TableNextColumn();
					ImU8String val6 = new ImU8String(0, 1);
					((ImU8String)(ref val6)).AppendFormatted<int>(vBPCbFmhCSmnVu0mgbR2.Required);
					ImGui.TextUnformatted(val6);
					ImGui.TableNextColumn();
					Vector4 vector = (num3 ? KnownColor.GreenYellow.ToVector4() : KnownColor.OrangeRed.ToVector4());
					ImU8String val7 = new ImU8String(0, 1);
					((ImU8String)(ref val7)).AppendFormatted<int>(vBPCbFmhCSmnVu0mgbR2.Available);
					ImGui.TextColored(ref vector, val7);
					ImGui.TableNextColumn();
					if (num3)
					{
						vector = KnownColor.GreenYellow.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C17D9F7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb))));
					}
					else
					{
						vector = KnownColor.OrangeRed.ToVector4();
						ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--664844326 ^ 0x1D695F60 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37), num4)));
					}
					ImGui.TableNextColumn();
					ImU8String val8 = new ImU8String(12, 2);
					((ImU8String)(ref val8)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61637));
					((ImU8String)(ref val8)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469A8439 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
					((ImU8String)(ref val8)).AppendFormatted<uint>(num2);
					if (ImGui.SmallButton(val8))
					{
						ImGui.SetClipboardText(ImU8String.op_Implicit(((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString()));
						OmenServiceBase<NotifyHelper>.Instance().NotificationSuccess(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351D650 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C34E1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee) + ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString());
					}
					ImGuiOm.TooltipHover(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53510C9E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08)));
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static void QmuvroKiyf(IReadOnlyList<sUJIEympPHpoiidxbEK> metrics, float? customWidth = null)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		if (metrics.Count == 0)
		{
			return;
		}
		float num = customWidth ?? ImGui.GetContentRegionAvail().X;
		float num2 = 6f * Globals.GlobalUIScale;
		float num3 = num / (float)metrics.Count;
		float textLineHeight;
		using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
		{
			textLineHeight = ImGui.GetTextLineHeight();
		}
		float textLineHeight2;
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			textLineHeight2 = ImGui.GetTextLineHeight();
		}
		float num4 = num2 * 2f + textLineHeight + textLineHeight2 + 3f * Globals.GlobalUIScale;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 vector = cursorScreenPos;
		Vector2 vector2 = cursorScreenPos + new Vector2(num, num4);
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(vector, vector2, ImGui.GetColorU32((ImGuiCol)7, 0.45f), 4f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(vector, vector2, ImGui.GetColorU32((ImGuiCol)5, 0.25f), 4f * Globals.GlobalUIScale);
		for (int i = 0; i < metrics.Count; i++)
		{
			sUJIEympPHpoiidxbEK sUJIEympPHpoiidxbEK2 = metrics[i];
			Vector2 vector3 = vector + new Vector2((float)i * num3, 0f);
			Vector2 vector4 = vector3 + new Vector2(num3, num4);
			if (i > 0)
			{
				float x = vector.X + (float)i * num3;
				((ImDrawListPtr)(ref windowDrawList)).AddLine(new Vector2(x, vector.Y + num4 * 0.2f), new Vector2(x, vector2.Y - num4 * 0.2f), ImGui.GetColorU32((ImGuiCol)5, 0.35f));
			}
			bool flag = !string.IsNullOrEmpty(sUJIEympPHpoiidxbEK2.Tooltip);
			string text = sUJIEympPHpoiidxbEK2.Label + (flag ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3967B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818) : string.Empty);
			float x2;
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
			}
			float x3 = vector3.X + MathF.Max(0f, (num3 - x2) / 2f);
			using (OmenServiceBase<FontManager>.Instance().UIFont60.Push())
			{
				((ImDrawListPtr)(ref windowDrawList)).AddText(new Vector2(x3, vector3.Y + num2), ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(text));
			}
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				Vector2 vector5 = ImGui.CalcTextSize(ImU8String.op_Implicit(sUJIEympPHpoiidxbEK2.Value), false, -1f);
				Vector2 vector6 = new Vector2(vector3.X + MathF.Max(0f, (num3 - vector5.X) / 2f), vector3.Y + num2 + textLineHeight + 2f * Globals.GlobalUIScale);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector6, ImGui.GetColorU32(sUJIEympPHpoiidxbEK2.ValueColor), ImU8String.op_Implicit(sUJIEympPHpoiidxbEK2.Value));
			}
			if (!flag || !ImGui.IsMouseHoveringRect(vector3, vector4))
			{
				continue;
			}
			TooltipDisposable val = ImRaii.Tooltip();
			try
			{
				TextWrapDisposable val2 = ImRaii.TextWrapPos(ImGui.GetFontSize() * 40f);
				try
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit(sUJIEympPHpoiidxbEK2.Tooltip));
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			finally
			{
				((TooltipDisposable)(ref val)).Dispose();
			}
		}
		ImGui.Dummy(new Vector2(num, num4));
	}

	private static float bYkvGKJf7X(ImDrawListPtr drawList, string text, float x, float y, uint backgroundColor, uint textColor, float maxX = float.PositiveInfinity, bool requireFit = false)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
		float num = 3f * Globals.GlobalUIScale;
		Vector2 vector2 = new Vector2(x, y);
		Vector2 vector3 = new Vector2(x + vector.X + num * 2f, y + vector.Y + 2f * Globals.GlobalUIScale);
		if (requireFit ? (vector3.X >= maxX) : (vector2.X >= maxX))
		{
			return x;
		}
		((ImDrawListPtr)(ref drawList)).AddRectFilled(vector2, vector3, backgroundColor, 2f * Globals.GlobalUIScale);
		((ImDrawListPtr)(ref drawList)).AddText(new Vector2(vector2.X + num, vector2.Y + 1f * Globals.GlobalUIScale), textColor, ImU8String.op_Implicit(text));
		return vector3.X + 4f * Globals.GlobalUIScale;
	}

	private static void lk4v6e2Wt2(string icon, string message, string? subMessage = null)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		Vector2 vector = ImGui.GetCursorScreenPos() + contentRegionAvail / 2f;
		ImDrawListPtr windowDrawList;
		using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
		{
			Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(icon), false, -1f);
			Vector2 vector3 = new Vector2(vector.X - vector2.X / 2f, vector.Y - vector2.Y - 8f * Globals.GlobalUIScale);
			windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, ImGui.GetColorU32((ImGuiCol)1, 0.4f), ImU8String.op_Implicit(icon));
		}
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Vector2 vector4 = ImGui.CalcTextSize(ImU8String.op_Implicit(message), false, -1f);
			Vector2 vector5 = new Vector2(vector.X - vector4.X / 2f, vector.Y + 4f * Globals.GlobalUIScale);
			windowDrawList = ImGui.GetWindowDrawList();
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector5, ImGui.GetColorU32((ImGuiCol)1, 0.7f), ImU8String.op_Implicit(message));
			if (!string.IsNullOrEmpty(subMessage))
			{
				Vector2 vector6 = ImGui.CalcTextSize(ImU8String.op_Implicit(subMessage), false, -1f);
				Vector2 vector7 = new Vector2(vector.X - vector6.X / 2f, vector5.Y + vector4.Y + 4f * Globals.GlobalUIScale);
				windowDrawList = ImGui.GetWindowDrawList();
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector7, ImGui.GetColorU32((ImGuiCol)1, 0.45f), ImU8String.op_Implicit(subMessage));
			}
		}
	}

	private List<rREVgcmw6gwyIYa4fy3> xkavkcI9XC()
	{
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		List<rREVgcmw6gwyIYa4fy3> list = new List<rREVgcmw6gwyIYa4fy3>();
		if (config.CurrentMeldingPlan?.MeldingInfos == null)
		{
			return list;
		}
		ISharedImmediateTexture icon = default(ISharedImmediateTexture);
		foreach (IGrouping<uint, pxgVHKmo62VsKBeM4fK> item2 in from info in config.CurrentMeldingPlan.MeldingInfos
			where info.MateriaIDs.Count > 0
			group info by info.ItemID)
		{
			_003C_003Ec__DisplayClass65_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass65_0();
			CS_0024_003C_003E8__locals3.itemID = item2.Key;
			List<pxgVHKmo62VsKBeM4fK> list2 = item2.ToList();
			if (!LuminaGetter.TryGetRow<Item>(CS_0024_003C_003E8__locals3.itemID, out Item item))
			{
				continue;
			}
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup((uint)((Item)(ref item)).Icon, false, true, (ClientLanguage?)null);
			if (!obj.TryGetFromGameIcon(ref val, ref icon))
			{
				continue;
			}
			string text = ((object)((Item)(ref item)).Name/*cast due to constrained. prefix*/).ToString();
			((IEnumerable<InventoryType>)Inventories.PlayerWithArmory).TryGetItems((Predicate<InventoryItem>)((InventoryItem i) => i.ItemId == CS_0024_003C_003E8__locals3.itemID), out List<InventoryItem> itemResult);
			for (int num = 0; num < list2.Count; num++)
			{
				pxgVHKmo62VsKBeM4fK pxgVHKmo62VsKBeM4fK2 = list2[num];
				string text2;
				if (list2.Count <= 1)
				{
					text2 = text;
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 2);
					defaultInterpolatedStringHandler.AppendFormatted(text);
					defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x109CDF7C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_44b9808560124617907f0048883b8948));
					defaultInterpolatedStringHandler.AppendFormatted(num + 1);
					text2 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				string itemName = text2;
				if (itemResult != null && num < itemResult.Count)
				{
					InventoryItem val2 = itemResult[num];
					if (YGZvxJrg0k(val2, pxgVHKmo62VsKBeM4fK2.MateriaIDs, out bool isCompleted, out List<zeD1U3mPqm2MXRQBwgd> attachedMateria))
					{
						list.Add(new rREVgcmw6gwyIYa4fy3
						{
							MeldingInfo = pxgVHKmo62VsKBeM4fK2,
							InventoryItem = val2,
							ItemIndex = num,
							ItemName = itemName,
							ItemData = item,
							Icon = icon,
							IsCompleted = isCompleted,
							AttachedMateria = attachedMateria,
							IsFound = true
						});
					}
				}
				else
				{
					list.Add(new rREVgcmw6gwyIYa4fy3
					{
						MeldingInfo = pxgVHKmo62VsKBeM4fK2,
						ItemName = itemName,
						ItemData = item,
						Icon = icon,
						IsFound = false
					});
				}
			}
		}
		return list;
	}

	public AutoMateriaAttach()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E003BA2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F361F40 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)),
			Category = ModuleCategory.Interface,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBDA68C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc) },
			ModulesPrerequisite = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x18CC0EA2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_33babcee9ecc460a9e530039fd18d361) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		planSearchInput = string.Empty;
		planToDeleteIndex = -1;
		gearsetURLInput = string.Empty;
		gearsetNameInput = string.Empty;
		base._002Ector();
	}

	static AutoMateriaAttach()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ClearBlockedItemsSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489F5DDC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b2294f2f9bb24a76b00cf1af75888414));
		AlwaysAllowAllMateriaSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D0D87 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
		SidebarTabs = new SidebarTab[2]
		{
			SidebarTab.Plans,
			SidebarTab.Import
		};
	}

	[CompilerGenerated]
	private bool uUyv2IsjRG(nSiWNGmmrHVI7N2Bt9j x)
	{
		return x.Name != gearsetNameInput;
	}
}
