using System;
using System.Collections.Generic;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Manager;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.AddonEvent;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;

namespace DailyRoutines.ModulesPublic;

public class AutoMiniCactpot : ModuleBase
{
	internal sealed class MiniCactpotSolver
	{
		public const int TOTAL_NUMBERS = 9;

		public const int TOTAL_LANES = 8;

		private static readonly ushort[] Payouts = new ushort[25]
		{
			0, 0, 0, 0, 0, 0, 10000, 36, 720, 360,
			80, 252, 108, 72, 54, 180, 72, 180, 119, 36,
			306, 1080, 144, 1800, 3600
		};

		private static readonly byte[][] Lines = new byte[8][]
		{
			new byte[3] { 0, 1, 2 },
			new byte[3] { 3, 4, 5 },
			new byte[3] { 6, 7, 8 },
			new byte[3] { 0, 3, 6 },
			new byte[3] { 1, 4, 7 },
			new byte[3] { 2, 5, 8 },
			new byte[3] { 0, 4, 8 },
			new byte[3] { 2, 4, 6 }
		};

		private readonly Dictionary<ulong, float> memo = new Dictionary<ulong, float>();

		public bool[] Solve(ReadOnlySpan<byte> state)
		{
			ulong num = 0uL;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 9; i++)
			{
				byte b = state.get_Item(i);
				if (b > 0)
				{
					num |= (ulong)b << i * 4;
					num3 |= 1 << b - 1;
					num2++;
				}
			}
			if (num2 == 4)
			{
				bool[] array = new bool[8];
				double num4 = -1.0;
				Span<byte> span = stackalloc byte[9];
				int length = 0;
				for (int j = 1; j <= 9; j++)
				{
					if ((num3 & (1 << j - 1)) == 0)
					{
						span[length++] = (byte)j;
					}
				}
				Span<byte> span2 = span.Slice(0, length);
				for (int k = 0; k < 8; k++)
				{
					float num5 = CalculateLineEV(num, k, span2);
					if ((double)num5 > num4 + 0.001)
					{
						num4 = num5;
						Array.Clear(array, 0, array.Length);
						array[k] = true;
					}
					else if (Math.Abs((double)num5 - num4) < 0.001)
					{
						array[k] = true;
					}
				}
				return array;
			}
			memo.Clear();
			bool[] array2 = new bool[9];
			double num6 = -1.0;
			for (int l = 0; l < 9; l++)
			{
				if (((num >> l * 4) & 0xF) == 0L)
				{
					float num7 = EvaluateCell(num, num3, num2, l);
					if ((double)num7 > num6 + 0.001)
					{
						num6 = num7;
						Array.Clear(array2, 0, array2.Length);
						array2[l] = true;
					}
					else if (Math.Abs((double)num7 - num6) < 0.001)
					{
						array2[l] = true;
					}
				}
			}
			return array2;
		}

		private float GetMaxEV(ulong board, int usedMask, int revealedCount)
		{
			if (revealedCount == 4)
			{
				if (memo.TryGetValue(board, out var value))
				{
					return value;
				}
				float num = 0f;
				Span<byte> span = stackalloc byte[9];
				int length = 0;
				for (int i = 1; i <= 9; i++)
				{
					if ((usedMask & (1 << i - 1)) == 0)
					{
						span[length++] = (byte)i;
					}
				}
				Span<byte> span2 = span.Slice(0, length);
				for (int j = 0; j < 8; j++)
				{
					float num2 = CalculateLineEV(board, j, span2);
					if (num2 > num)
					{
						num = num2;
					}
				}
				memo[board] = num;
				return num;
			}
			if (memo.TryGetValue(board, out var value2))
			{
				return value2;
			}
			float num3 = 0f;
			for (int k = 0; k < 9; k++)
			{
				if (((board >> k * 4) & 0xF) == 0L)
				{
					float num4 = EvaluateCell(board, usedMask, revealedCount, k);
					if (num4 > num3)
					{
						num3 = num4;
					}
				}
			}
			memo[board] = num3;
			return num3;
		}

		private float EvaluateCell(ulong board, int usedMask, int revealedCount, int cellIndex)
		{
			float num = 0f;
			int num2 = 0;
			int num3 = ~usedMask & 0x1FF;
			for (int i = 1; i <= 9; i++)
			{
				if ((num3 & (1 << i - 1)) != 0)
				{
					ulong board2 = board | (ulong)((long)i << cellIndex * 4);
					num += GetMaxEV(board2, usedMask | (1 << i - 1), revealedCount + 1);
					num2++;
				}
			}
			if (num2 != 0)
			{
				return num / (float)num2;
			}
			return 0f;
		}

		private static float CalculateLineEV(ulong board, int lineIdx, ReadOnlySpan<byte> available)
		{
			byte[] array = Lines[lineIdx];
			int num = 0;
			int num2 = 0;
			Span<byte> span = stackalloc byte[3];
			for (int i = 0; i < 3; i++)
			{
				byte b = array[i];
				int num3 = (int)((board >> b * 4) & 0xF);
				if (num3 > 0)
				{
					num += num3;
				}
				else
				{
					span[num2++] = b;
				}
			}
			if (num2 == 0)
			{
				return (int)Payouts[num];
			}
			int num4 = 0;
			int num5 = 0;
			switch (num2)
			{
			case 1:
			{
				ReadOnlySpan<byte> readOnlySpan = available;
				for (int m = 0; m < readOnlySpan.Length; m++)
				{
					byte b2 = readOnlySpan.get_Item(m);
					num4 += Payouts[num + b2];
				}
				num5 = available.Length;
				break;
			}
			case 2:
			{
				for (int n = 0; n < available.Length; n++)
				{
					for (int num6 = 0; num6 < available.Length; num6++)
					{
						if (n != num6)
						{
							num4 += Payouts[num + available.get_Item(n) + available.get_Item(num6)];
							num5++;
						}
					}
				}
				break;
			}
			default:
			{
				for (int j = 0; j < available.Length; j++)
				{
					for (int k = 0; k < available.Length; k++)
					{
						if (j == k)
						{
							continue;
						}
						for (int l = 0; l < available.Length; l++)
						{
							if (l != j && l != k)
							{
								num4 += Payouts[num + available.get_Item(j) + available.get_Item(k) + available.get_Item(l)];
								num5++;
							}
						}
					}
				}
				break;
			}
			}
			return (float)num4 / (float)num5;
		}
	}

	private readonly MiniCactpotSolver solver = new MiniCactpotSolver();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoMiniCactpotTitle"),
		Description = LanguageManager.Get("AutoMiniCactpotDescription"),
		Category = ModuleCategory.GoldSaucer
	};

	protected unsafe override void Init()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		if (base.TaskHelper == null)
		{
			TaskHelper obj = new TaskHelper
			{
				TimeoutMS = 5000
			};
			TaskHelper taskHelper = obj;
			base.TaskHelper = obj;
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "LotteryDaily", new AddonEventDelegate(OnAddon));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "LotteryDaily", new AddonEventDelegate(OnAddon));
		if (Addons.LotteryDaily != null)
		{
			OnAddon((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnAddon });
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
	}

	private void OnAddon(AddonEvent type, AddonArgs args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
				base.TaskHelper.Enqueue(() => AddonSelectYesnoEvent.ClickYes());
			}
		}
		else
		{
			base.TaskHelper.Abort();
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate));
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		AddonLotteryDaily* lotteryDaily = (AddonLotteryDaily*)Addons.LotteryDaily;
		AgentLotteryDaily* ptr = AgentLotteryDaily.Instance();
		if (lotteryDaily == null || ptr == null)
		{
			return;
		}
		Span<byte> span = stackalloc byte[9];
		for (int i = 0; i < 9; i++)
		{
			span[i] = ((AgentLotteryDaily)ptr).Numbers[i];
		}
		switch (((AgentLotteryDaily)ptr).Status)
		{
		case 1:
		{
			bool[] array2 = solver.Solve(span);
			for (int k = 0; k < 9; k++)
			{
				if (array2[k])
				{
					ClickGameNode(lotteryDaily, k);
					break;
				}
			}
			break;
		}
		case 2:
		{
			bool[] array = solver.Solve(span);
			ReadOnlySpan<int> readOnlySpan = new int[8] { 6, 3, 4, 5, 7, 0, 1, 2 };
			for (int j = 0; j < 8; j++)
			{
				if (array[readOnlySpan.get_Item(j)])
				{
					ClickLaneNode(lotteryDaily, j);
					break;
				}
			}
			break;
		}
		case 4:
			((AddonLotteryDaily)lotteryDaily).AtkUnitBase.Callback(-1);
			((AddonLotteryDaily)lotteryDaily).Close(true);
			break;
		case 3:
			break;
		}
	}

	private unsafe static void ClickGameNode(AddonLotteryDaily* addon, int i)
	{
		uint nodeId = ((AtkResNode)(&((AtkComponentNode)((AtkComponentBase)(&((AtkComponentButton)(&((AtkComponentCheckBox)((GameTileBoard)(&((AddonLotteryDaily)addon).GameBoard))[i]).AtkComponentButton)).AtkComponentBase)).OwnerNode).AtkResNode)).NodeId;
		if ((nodeId >= 30 && nodeId <= 38) || 1 == 0)
		{
			((AddonLotteryDaily)addon).AtkUnitBase.Callback(1, (int)(nodeId - 30));
		}
	}

	private unsafe static void ClickLaneNode(AddonLotteryDaily* addon, int i)
	{
		if ((i >= 0 && i <= 8) || 1 == 0)
		{
			uint nodeId = ((AtkComponentNode)((AtkComponentRadioButton)((LaneTileSelector)(&((AddonLotteryDaily)addon).LaneSelector))[i]).OwnerNode).NodeId;
			if ((nodeId >= 21 && nodeId <= 28) || 1 == 0)
			{
				int num = (int)(nodeId - 21);
				int* ptr = (int*)addon + 251;
				*ptr = num;
				((AddonLotteryDaily)addon).AtkUnitBase.Callback(2, num);
			}
		}
	}
}
