using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game;
using OmenTools.Info.Game.Enums;
using OmenTools.Interop.Game.ExecuteCommand.Implementations;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules;

public class AutoTradeReviveCount : ModuleBase
{
	private class GFi0cipSXsoDP2AEjEO : ModuleConfig
	{
		public bool IsToRevive;

		public uint SelectedActionID;

		public GFi0cipSXsoDP2AEjEO()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			IsToRevive = true;
			base._002Ector();
		}
	}

	private sealed class IReS99pVc6oAGgZ17uD : IEquatable<IReS99pVc6oAGgZ17uD>
	{
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(IReS99pVc6oAGgZ17uD);
			}
		}

		public uint ActionID { get; init; }

		public uint RequiredStatusID { get; init; }

		public IReS99pVc6oAGgZ17uD(uint ActionID, uint RequiredStatusID)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.ActionID = ActionID;
			this.RequiredStatusID = RequiredStatusID;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x17784043 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f7cf93b006904393bb56d82c0a688fa1));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F26E6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			if (YYep92C3B7(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool YYep92C3B7(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F2434 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			builder.Append(ActionID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7C93F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
			builder.Append(RequiredStatusID.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(IReS99pVc6oAGgZ17uD? left, IReS99pVc6oAGgZ17uD? right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(IReS99pVc6oAGgZ17uD? left, IReS99pVc6oAGgZ17uD? right)
		{
			if ((object)left != right)
			{
				return left?.Equals(right) ?? false;
			}
			return true;
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return (EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(ActionID)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(RequiredStatusID);
		}

		[CompilerGenerated]
		public override bool Equals(object? obj)
		{
			return Equals(obj as IReS99pVc6oAGgZ17uD);
		}

		[CompilerGenerated]
		public bool Equals(IReS99pVc6oAGgZ17uD? other)
		{
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<uint>.Default.Equals(ActionID, other.ActionID))
				{
					return EqualityComparer<uint>.Default.Equals(RequiredStatusID, other.RequiredStatusID);
				}
				return false;
			}
			return true;
		}

		[CompilerGenerated]
		public IReS99pVc6oAGgZ17uD aDspfZsIas()
		{
			return new IReS99pVc6oAGgZ17uD(this);
		}

		[CompilerGenerated]
		private IReS99pVc6oAGgZ17uD(IReS99pVc6oAGgZ17uD original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			ActionID = original.ActionID;
			RequiredStatusID = original.RequiredStatusID;
		}

		[CompilerGenerated]
		public void EGIpCRcDhV(out uint ActionID, out uint RequiredStatusID)
		{
			ActionID = this.ActionID;
			RequiredStatusID = this.RequiredStatusID;
		}
	}

	private sealed class VBKS9hpjr8KgMjkYUHc : IEquatable<VBKS9hpjr8KgMjkYUHc>
	{
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(VBKS9hpjr8KgMjkYUHc);
			}
		}

		public uint ActionID { get; init; }

		public Func<uint, bool> IsActionAvailable { get; init; }

		public IReS99pVc6oAGgZ17uD? Preparation { get; init; }

		public string Name => LuminaWrapper.GetActionName(ActionID);

		public bool IsAvailable
		{
			get
			{
				if (IsActionAvailable(ActionID))
				{
					if (!(Preparation == null))
					{
						return IsActionAvailable(Preparation.ActionID);
					}
					return true;
				}
				return false;
			}
		}

		public VBKS9hpjr8KgMjkYUHc(uint ActionID, Func<uint, bool> IsActionAvailable, IReS99pVc6oAGgZ17uD? Preparation = null)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.ActionID = ActionID;
			this.IsActionAvailable = IsActionAvailable;
			this.Preparation = Preparation;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF10CE2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x489B593E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cade14e68bb84906959f3cda5c496848));
			if (w9tpOAqdot(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool w9tpOAqdot(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAEC64C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a));
			builder.Append(ActionID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4D05F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7));
			builder.Append(IsActionAvailable);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20B463 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
			builder.Append(Preparation);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x68540C97 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5));
			builder.Append((object?)Name);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3695FB6F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			builder.Append(IsAvailable.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(VBKS9hpjr8KgMjkYUHc? left, VBKS9hpjr8KgMjkYUHc? right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(VBKS9hpjr8KgMjkYUHc? left, VBKS9hpjr8KgMjkYUHc? right)
		{
			if ((object)left != right)
			{
				return left?.Equals(right) ?? false;
			}
			return true;
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(ActionID)) * -1521134295 + EqualityComparer<Func<uint, bool>>.Default.GetHashCode(IsActionAvailable)) * -1521134295 + EqualityComparer<IReS99pVc6oAGgZ17uD>.Default.GetHashCode(Preparation);
		}

		[CompilerGenerated]
		public override bool Equals(object? obj)
		{
			return Equals(obj as VBKS9hpjr8KgMjkYUHc);
		}

		[CompilerGenerated]
		public bool Equals(VBKS9hpjr8KgMjkYUHc? other)
		{
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<uint>.Default.Equals(ActionID, other.ActionID) && EqualityComparer<Func<uint, bool>>.Default.Equals(IsActionAvailable, other.IsActionAvailable))
				{
					return EqualityComparer<IReS99pVc6oAGgZ17uD>.Default.Equals(Preparation, other.Preparation);
				}
				return false;
			}
			return true;
		}

		[CompilerGenerated]
		public VBKS9hpjr8KgMjkYUHc jgSpKWLVoj()
		{
			return new VBKS9hpjr8KgMjkYUHc(this);
		}

		[CompilerGenerated]
		private VBKS9hpjr8KgMjkYUHc(VBKS9hpjr8KgMjkYUHc original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			ActionID = original.ActionID;
			IsActionAvailable = original.IsActionAvailable;
			Preparation = original.Preparation;
		}

		[CompilerGenerated]
		public void RmIpt9UJSH(out uint ActionID, out Func<uint, bool> IsActionAvailable, out IReS99pVc6oAGgZ17uD? Preparation)
		{
			ActionID = this.ActionID;
			IsActionAvailable = this.IsActionAvailable;
			Preparation = this.Preparation;
		}
	}

	private enum ReviveAcceptPhase
	{
		WaitRevive,
		Accept,
		RedirectToMonster,
		AttackMonster
	}

	private GFi0cipSXsoDP2AEjEO config;

	private (uint EntityID, string? Name) monsterInfo;

	private (uint EntityID, string? Name) memberInfo;

	private uint territoryID;

	private bool isRunning;

	private ReviveAcceptPhase reviveAcceptPhase;

	private long phaseStartedAt;

	private long lastActionAttemptAt;

	private int consecutiveActionFailures;

	private const uint RAISE_PENDING_STATUS_ID = 148u;

	private const int ACTION_RETRY_INTERVAL = 500;

	private const int ACTION_FAILURE_LIMIT = 5;

	private const int REVIVE_TIMEOUT = 15000;

	private static readonly uint[] ReviveCountAchievements;

	private static readonly VBKS9hpjr8KgMjkYUHc[] ReviveMethods;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	private VBKS9hpjr8KgMjkYUHc? SelectedMethod => ReviveMethods.FirstOrDefault((VBKS9hpjr8KgMjkYUHc method) => method.ActionID == config.SelectedActionID);

	protected override void Init()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<GFi0cipSXsoDP2AEjEO>(this) ?? new GFi0cipSXsoDP2AEjEO();
		OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(rZdMDZnFUh), 100u);
	}

	protected override void Uninit()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		OmenServiceBase<FrameworkManager>.Instance().Unreg(rZdMDZnFUh);
		Stop();
	}

	protected override void ConfigUI()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E009398 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa)));
		try
		{
			if (!val)
			{
				return;
			}
			TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5E3BE42E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2d7d8a5df6744c51b35f486f03b53ded))));
			try
			{
				if (TabItemDisposable.op_Implicit(val2))
				{
					ATfM5jcX9h();
					ImGui.NewLine();
					DisabledDisposable val3 = ImRaii.Disabled(isRunning);
					try
					{
						lqEM2oufOX();
						ImGui.NewLine();
						scLMZtu9Hs();
						ImGui.NewLine();
						BgwMaHlSOv();
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
			}
			finally
			{
				((TabItemDisposable)(ref val2)).Dispose();
			}
			TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5B606 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3))));
			try
			{
				if (TabItemDisposable.op_Implicit(val4))
				{
					iuBM0glnU8();
				}
			}
			finally
			{
				((TabItemDisposable)(ref val4)).Dispose();
			}
		}
		finally
		{
			((TabBarDisposable)(ref val)).Dispose();
		}
	}

	private void lqEM2oufOX()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5351ACBE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			if (ImGui.RadioButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E4047F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7))), config.IsToRevive))
			{
				config.IsToRevive = true;
				config.Save(this);
			}
			ImGui.SameLine();
			if (ImGui.RadioButton(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E009300 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa))), !config.IsToRevive))
			{
				config.IsToRevive = false;
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void scLMZtu9Hs()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D663A6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.AlignTextToFramePadding();
			ImU8String val2 = new ImU8String(2, 2);
			((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6854DAC5 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5)));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B3CDBA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			((ImU8String)(ref val2)).AppendFormatted<string>(monsterInfo.Name);
			ImGui.TextUnformatted(val2);
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61531, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D18D1B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4DBBC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55)))
			{
				IGameObject target = TargetManager.Target;
				if (target != null)
				{
					monsterInfo = (EntityID: target.EntityID, Name: target.Name);
				}
			}
			ImGui.AlignTextToFramePadding();
			ImU8String val3 = new ImU8String(2, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50176048 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(--1470179789 ^ 0x4BB67E78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb));
			((ImU8String)(ref val3)).AppendFormatted<string>(memberInfo.Name);
			ImGui.TextUnformatted(val3);
			ImGui.SameLine();
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61531, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FF140 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af)) + LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x12D6630C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7fcf5a71a7ed445197f132979519c560)))
			{
				IGameObject target2 = TargetManager.Target;
				if (target2 != null)
				{
					memberInfo = (EntityID: target2.EntityID, Name: target2.Name);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void BgwMaHlSOv()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x1C177DB3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_accf3844f1c6464481df1849f0aee1bb))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ComboDisposable val2 = ImRaii.Combo(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF20617B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372)), ImU8String.op_Implicit(SelectedMethod?.Name ?? string.Empty));
			try
			{
				if (!val2)
				{
					return;
				}
				VBKS9hpjr8KgMjkYUHc[] reviveMethods = ReviveMethods;
				foreach (VBKS9hpjr8KgMjkYUHc vBKS9hpjr8KgMjkYUHc in reviveMethods)
				{
					bool flag = config.SelectedActionID == vBKS9hpjr8KgMjkYUHc.ActionID;
					if (ImGui.Selectable(ImU8String.op_Implicit(vBKS9hpjr8KgMjkYUHc.Name), flag, (ImGuiSelectableFlags)0, default(Vector2)))
					{
						config.SelectedActionID = vBKS9hpjr8KgMjkYUHc.ActionID;
						config.Save(this);
					}
					if (flag)
					{
						ImGui.SetItemDefaultFocus();
					}
				}
			}
			finally
			{
				((ComboDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void iuBM0glnU8()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454E31B9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467))));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			uint[] reviveCountAchievements = ReviveCountAchievements;
			foreach (uint id in reviveCountAchievements)
			{
				if (OmenServiceBase<AchievementManager>.Instance().TryGetAchievement(id, out AchievementInfo achievementInfo))
				{
					float num = ((achievementInfo.Max == 0) ? 0f : ((float)achievementInfo.Current / (float)achievementInfo.Max));
					Vector2 vector2 = new Vector2(-1f, 0f);
					ImU8String val2 = new ImU8String(5, 3);
					((ImU8String)(ref val2)).AppendFormatted<string>(achievementInfo.Name);
					((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369530B3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
					((ImU8String)(ref val2)).AppendFormatted<uint>(achievementInfo.Current);
					((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D71386 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
					((ImU8String)(ref val2)).AppendFormatted<uint>(achievementInfo.Max);
					ImGui.ProgressBar(num, vector2, val2);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ATfM5jcX9h()
	{
		DisabledDisposable val = ImRaii.Disabled(isRunning || !(SelectedMethod != null) || !xOQMRnYjZo(out IBattleNPC _, out IPlayerCharacter _));
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61515, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E8790 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71))))
			{
				Start();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		ImGui.SameLine();
		val = ImRaii.Disabled(!isRunning);
		try
		{
			if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)61517, LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0FEEAA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af))))
			{
				Stop();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void Start()
	{
		if (!xOQMRnYjZo(out IBattleNPC _, out IPlayerCharacter member))
		{
			return;
		}
		if (config.IsToRevive)
		{
			member = LocalPlayerState.Object;
			if (member != null && member.IsDead)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x50176026 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf)));
				return;
			}
		}
		territoryID = GameState.TerritoryType;
		member = LocalPlayerState.Object;
		reviveAcceptPhase = ((member == null || !member.IsDead) ? ReviveAcceptPhase.AttackMonster : ReviveAcceptPhase.WaitRevive);
		phaseStartedAt = Environment.TickCount64;
		lastActionAttemptAt = 0L;
		consecutiveActionFailures = 0;
		isRunning = true;
	}

	private void Stop(bool notify = false)
	{
		if (isRunning)
		{
			if (!config.IsToRevive)
			{
				AutoAttackCommand.Disable();
			}
			isRunning = false;
			consecutiveActionFailures = 0;
			if (notify)
			{
				OmenServiceBase<NotifyHelper>.Instance().NotificationError(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E98C6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71)));
			}
		}
	}

	private bool xOQMRnYjZo(out IBattleNPC monster, out IPlayerCharacter member)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Invalid comparison between Unknown and I4
		monster = null;
		member = null;
		if (LocalPlayerState.Object == null)
		{
			return false;
		}
		IGameObject gameObject = IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(monsterInfo.EntityID);
		if (!(gameObject is IBattleNPC battleNPC) || (int)gameObject.ObjectKind != 2 || gameObject.IsDead || !gameObject.IsTargetable)
		{
			return false;
		}
		if (!(IOmenDalamudService<IObjectTable>.Instance().SearchByEntityID(memberInfo.EntityID) is IPlayerCharacter playerCharacter) || playerCharacter.EntityID == LocalPlayerState.EntityID)
		{
			return false;
		}
		monster = battleNPC;
		member = playerCharacter;
		return true;
	}

	private void rZdMDZnFUh(IFramework framework)
	{
		if (!isRunning)
		{
			return;
		}
		IBattleNPC monster;
		IPlayerCharacter member;
		if (GameState.TerritoryType != territoryID)
		{
			Stop(notify: true);
		}
		else if (!xOQMRnYjZo(out monster, out member))
		{
			Stop(notify: true);
		}
		else if (config.IsToRevive)
		{
			IPlayerCharacter playerCharacter = LocalPlayerState.Object;
			if (playerCharacter == null || playerCharacter.IsDead)
			{
				Stop(notify: true);
			}
			else
			{
				EumMe2iNii(member);
			}
		}
		else
		{
			hD8M7KZeMR(monster);
		}
	}

	private unsafe void EumMe2iNii(IPlayerCharacter member)
	{
		if (!member.IsDead)
		{
			consecutiveActionFailures = 0;
			return;
		}
		if (((BattleChara)member.ToBCStruct()).HasStatus(148u))
		{
			consecutiveActionFailures = 0;
			return;
		}
		VBKS9hpjr8KgMjkYUHc selectedMethod = SelectedMethod;
		IReS99pVc6oAGgZ17uD preparation = selectedMethod.Preparation;
		if ((object)preparation != null && !LocalPlayerState.HasStatus(preparation.RequiredStatusID, out var _))
		{
			Xe8MNGQuxs(preparation.ActionID, LocalPlayerState.Object);
		}
		else
		{
			Xe8MNGQuxs(selectedMethod.ActionID, member);
		}
	}

	private unsafe void Xe8MNGQuxs(uint actionID, IGameObject target)
	{
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)27])
		{
			return;
		}
		ActionManager* ptr = ActionManager.Instance();
		if (ptr == null)
		{
			Stop(notify: true);
			return;
		}
		if (((ActionManager)ptr).GetActionStatus((ActionType)1, actionID, 3758096384uL, true, true, (uint*)null) != 0)
		{
			consecutiveActionFailures = 0;
			return;
		}
		long tickCount = Environment.TickCount64;
		if (tickCount - lastActionAttemptAt < 500)
		{
			return;
		}
		lastActionAttemptAt = tickCount;
		if (ActionManager.CanUseActionOnTarget(actionID, target.ToStruct()) && OmenServiceBase<UseActionManager>.Instance().UseAction((ActionType)1, actionID, target.EntityID, 0u, (UseActionMode)0))
		{
			consecutiveActionFailures = 0;
			return;
		}
		consecutiveActionFailures++;
		if (consecutiveActionFailures >= 5)
		{
			Stop(notify: true);
		}
	}

	private void hD8M7KZeMR(IBattleNPC monster)
	{
		IPlayerCharacter playerCharacter = LocalPlayerState.Object;
		int index;
		switch (reviveAcceptPhase)
		{
		case ReviveAcceptPhase.WaitRevive:
			if (!playerCharacter.IsDead)
			{
				reviveAcceptPhase = ReviveAcceptPhase.AttackMonster;
				phaseStartedAt = Environment.TickCount64;
			}
			else if (LocalPlayerState.HasStatus(148u, out index))
			{
				reviveAcceptPhase = ReviveAcceptPhase.Accept;
				phaseStartedAt = Environment.TickCount64;
			}
			break;
		case ReviveAcceptPhase.Accept:
			if (!playerCharacter.IsDead)
			{
				reviveAcceptPhase = ReviveAcceptPhase.RedirectToMonster;
				phaseStartedAt = Environment.TickCount64;
			}
			else if (LocalPlayerState.HasStatus(148u, out index))
			{
				OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.Revive, 5u);
				reviveAcceptPhase = ReviveAcceptPhase.RedirectToMonster;
				phaseStartedAt = Environment.TickCount64;
			}
			else if (Environment.TickCount64 - phaseStartedAt >= 15000)
			{
				Stop(notify: true);
			}
			break;
		case ReviveAcceptPhase.RedirectToMonster:
			OmenServiceBase<MovementManager>.Instance().TPPlayerAddress(monster.Position);
			if (!playerCharacter.IsDead)
			{
				reviveAcceptPhase = ReviveAcceptPhase.AttackMonster;
				phaseStartedAt = Environment.TickCount64;
			}
			else if (Environment.TickCount64 - phaseStartedAt >= 15000)
			{
				Stop(notify: true);
			}
			break;
		case ReviveAcceptPhase.AttackMonster:
			if (playerCharacter.IsDead)
			{
				reviveAcceptPhase = ReviveAcceptPhase.WaitRevive;
				phaseStartedAt = Environment.TickCount64;
				break;
			}
			AutoAttackCommand.Enable(monster.EntityID);
			OmenServiceBase<ExecuteCommandManager>.Instance().ExecuteCommand(ExecuteCommandFlag.RemoveStatus, 418u, 0u, 3758096384u);
			reviveAcceptPhase = ReviveAcceptPhase.WaitRevive;
			phaseStartedAt = Environment.TickCount64;
			break;
		}
	}

	public AutoTradeReviveCount()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E521D6 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x23106537 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a96fab9e97944b39a8afe3fb1109ba7d)),
			Category = ModuleCategory.Script
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static AutoTradeReviveCount()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		ReviveCountAchievements = new uint[9] { 2691u, 2692u, 2693u, 3672u, 3673u, 3674u, 4014u, 4015u, 4016u };
		ReviveMethods = new VBKS9hpjr8KgMjkYUHc[3]
		{
			new VBKS9hpjr8KgMjkYUHc(7523u, (uint actionID) => LocalPlayerState.ClassJob == 35 && ActionManagerExtension.IsActionUnlocked(actionID), new IReS99pVc6oAGgZ17uD(7514u, 1249u)),
			new VBKS9hpjr8KgMjkYUHc(41634u, (uint actionID) => CrescentSupportJob.Chemist.IsThisJob() && CrescentSupportJob.Chemist.IsActionUnlocked(actionID)),
			new VBKS9hpjr8KgMjkYUHc(49070u, (uint actionID) => CrescentSupportJob.WhiteMage.IsThisJob() && CrescentSupportJob.WhiteMage.IsActionUnlocked(actionID))
		};
	}

	[CompilerGenerated]
	private bool HW1MdFCxCs(VBKS9hpjr8KgMjkYUHc method)
	{
		return method.ActionID == config.SelectedActionID;
	}
}
