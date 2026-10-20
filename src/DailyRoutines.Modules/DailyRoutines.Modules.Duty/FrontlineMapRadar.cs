using System;
using System.CodeDom.Compiler;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Hooking;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.STD;
using InteropGenerator.Runtime;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Extensions;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;
using lcOILHT0EGBgw0K6BhI;
using yTbFJ0TRnmIlA1BYNIA;

namespace DailyRoutines.Modules.Duty;

public class FrontlineMapRadar : ModuleBase
{
	private unsafe delegate MapMarkerData* Mnc9dYiw72HcjgQF19U(MapMarkerData* self, uint levelID, Utf8String* tooltipString, uint iconID, float x, float y, float z, float radius, ushort territoryTypeID, uint mapID, uint placeNameZoneID, uint placeNameID, ushort recommendedLevel, sbyte eventState);

	private class ovIld8iEGlRppOJbTsm : ModuleConfig
	{
		public float DotRadius;

		public bool HideFriendlyInPVP;

		public bool HideLoadingRangeInPVP;

		public HashSet<uint> HighlightedJobs;

		public float JobIconSize;

		public int JobIconStyle;

		public bool ShowAllianceDots;

		public bool ShowAllianceJobIcons;

		public bool ShowAllianceNames;

		public bool ShowControlPointScore;

		public bool ShowCountdown;

		public bool ShowFriendDots;

		public bool ShowFriendJobIcons;

		public bool ShowFriendNames;

		public bool ShowHealthPercent;

		public bool ShowOtherDots;

		public bool ShowOtherJobIcons;

		public bool ShowOtherNames;

		public bool ShowOutsideFrontline;

		public bool ShowPartyDots;

		public bool ShowPartyJobIcons;

		public bool ShowPartyNames;

		public ovIld8iEGlRppOJbTsm()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DotRadius = 1f;
			HideFriendlyInPVP = true;
			HighlightedJobs = new HashSet<uint>();
			JobIconSize = 1.4f;
			JobIconStyle = 1;
			ShowAllianceDots = true;
			ShowControlPointScore = true;
			ShowCountdown = true;
			ShowFriendJobIcons = true;
			ShowFriendNames = true;
			ShowHealthPercent = true;
			ShowOtherDots = true;
			ShowPartyJobIcons = true;
			ShowPartyNames = true;
			base._002Ector();
		}
	}

	private readonly struct JreSXXibiAjvqLPISRq : IEquatable<JreSXXibiAjvqLPISRq>
	{
		public uint IconID { get; init; }

		public Vector3 Position { get; init; }

		public string DisplayText { get; init; }

		public uint TextColor { get; init; }

		public JreSXXibiAjvqLPISRq(uint IconID, Vector3 Position, string DisplayText, uint TextColor)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.IconID = IconID;
			this.Position = Position;
			this.DisplayText = DisplayText;
			this.TextColor = TextColor;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258F6788 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4427E762 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8dfe57222f64fa68d9d16c8b6fe9d0b));
			if (vRbiFhetPj(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool vRbiFhetPj(StringBuilder builder)
		{
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE779F9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
			builder.Append(IconID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CF7F1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			builder.Append(Position.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0F3888 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			builder.Append((object?)DisplayText);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CE80D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			builder.Append(TextColor.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(JreSXXibiAjvqLPISRq left, JreSXXibiAjvqLPISRq right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(JreSXXibiAjvqLPISRq left, JreSXXibiAjvqLPISRq right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((EqualityComparer<uint>.Default.GetHashCode(IconID) * -1521134295 + EqualityComparer<Vector3>.Default.GetHashCode(Position)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DisplayText)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(TextColor);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is JreSXXibiAjvqLPISRq)
			{
				return Equals((JreSXXibiAjvqLPISRq)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(JreSXXibiAjvqLPISRq other)
		{
			if (EqualityComparer<uint>.Default.Equals(IconID, other.IconID) && EqualityComparer<Vector3>.Default.Equals(Position, other.Position) && EqualityComparer<string>.Default.Equals(DisplayText, other.DisplayText))
			{
				return EqualityComparer<uint>.Default.Equals(TextColor, other.TextColor);
			}
			return false;
		}

		[CompilerGenerated]
		public void CmBiqAIT5O(out uint IconID, out Vector3 Position, out string DisplayText, out uint TextColor)
		{
			IconID = this.IconID;
			Position = this.Position;
			DisplayText = this.DisplayText;
			TextColor = this.TextColor;
		}
	}

	private record YfPKklicMbcJd6C26WJ
	{
		public uint IconID { get; init; }

		public DateTime? ControlStartTime { get; init; }

		public int InitialScore { get; init; }

		public int? CurrentScore { get; set; }

		public double? CurrentPercent { get; set; }

		public DateTime? ReachedZeroTime { get; set; }

		public double DisplayScore
		{
			get
			{
				if (!CurrentPercent.HasValue)
				{
					return CurrentScore.GetValueOrDefault();
				}
				return (double)InitialScore * CurrentPercent.Value / 100.0;
			}
		}

		public YfPKklicMbcJd6C26WJ(uint IconID, DateTime? ControlStartTime, int InitialScore)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.IconID = IconID;
			this.ControlStartTime = ControlStartTime;
			this.InitialScore = InitialScore;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x423E51C0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_6368ba44fa4749c09da09b5ebf853b71));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C144397 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			if (PrintMembers(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x420C2236 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_84ee025b19bb4145ba9b4b6b7acb91eb));
			builder.Append(IconID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5CE3C31B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3054c6e6ef664be696bb2ad005864818));
			builder.Append(ControlStartTime.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x53516512 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_9085260e6b1a40438d219e85182b5d08));
			builder.Append(InitialScore.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D5993 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e));
			builder.Append(CurrentScore.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E005A6A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			builder.Append(CurrentPercent.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF11120 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			builder.Append(ReachedZeroTime.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD7D4D3 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f));
			builder.Append(DisplayScore.ToString());
			return true;
		}

		[CompilerGenerated]
		protected YfPKklicMbcJd6C26WJ(YfPKklicMbcJd6C26WJ original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			IconID = original.IconID;
			ControlStartTime = original.ControlStartTime;
			InitialScore = original.InitialScore;
			CurrentScore = original.CurrentScore;
			CurrentPercent = original.CurrentPercent;
			ReachedZeroTime = original.ReachedZeroTime;
		}

		[CompilerGenerated]
		public void fVviAbLFmN(out uint IconID, out DateTime? ControlStartTime, out int InitialScore)
		{
			IconID = this.IconID;
			ControlStartTime = this.ControlStartTime;
			InitialScore = this.InitialScore;
		}
	}

	private readonly struct qaQsUDiQ0PrsjpUaM73 : IEquatable<qaQsUDiQ0PrsjpUaM73>
	{
		public bool ShowName { get; init; }

		public string Name { get; init; }

		public uint NameColor { get; init; }

		public bool ShowJobIcon { get; init; }

		public uint JobID { get; init; }

		public bool ShowDotIcon { get; init; }

		public PlayerIconType DotIconType { get; init; }

		public bool IsPvP { get; init; }

		public qaQsUDiQ0PrsjpUaM73(bool ShowName, string Name, uint NameColor, bool ShowJobIcon, uint JobID, bool ShowDotIcon, PlayerIconType DotIconType, bool IsPvP)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.ShowName = ShowName;
			this.Name = Name;
			this.NameColor = NameColor;
			this.ShowJobIcon = ShowJobIcon;
			this.JobID = JobID;
			this.ShowDotIcon = ShowDotIcon;
			this.DotIconType = DotIconType;
			this.IsPvP = IsPvP;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x66B31B42 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_762f55d0e7ef4ecd8f3f55c0f8a10fae));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C144397 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			if (RadiJceips(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool RadiJceips(StringBuilder builder)
		{
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x258E99FE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_dadeb2ab61424d31be7cc1966e675825));
			builder.Append(ShowName.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x43D7DC4C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_974d377e1ed74986b81c34c95c2cc846));
			builder.Append((object?)Name);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7C2E7EAC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_12c2d68ba4654622a4959ee73cacc6d7));
			builder.Append(NameColor.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E419B8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			builder.Append(ShowJobIcon.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC841C2 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37));
			builder.Append(JobID.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4B0EC680 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_8e002b0644ef421698ef6f63772929af));
			builder.Append(ShowDotIcon.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469B2ECB ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			builder.Append(DotIconType.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591D1611 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118));
			builder.Append(IsPvP.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(qaQsUDiQ0PrsjpUaM73 left, qaQsUDiQ0PrsjpUaM73 right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(qaQsUDiQ0PrsjpUaM73 left, qaQsUDiQ0PrsjpUaM73 right)
		{
			return left.Equals(right);
		}

		[CompilerGenerated]
		public override int GetHashCode()
		{
			return ((((((EqualityComparer<bool>.Default.GetHashCode(ShowName) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(NameColor)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(ShowJobIcon)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(JobID)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(ShowDotIcon)) * -1521134295 + EqualityComparer<PlayerIconType>.Default.GetHashCode(DotIconType)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(IsPvP);
		}

		[CompilerGenerated]
		public override bool Equals(object obj)
		{
			if (obj is qaQsUDiQ0PrsjpUaM73)
			{
				return Equals((qaQsUDiQ0PrsjpUaM73)obj);
			}
			return false;
		}

		[CompilerGenerated]
		public bool Equals(qaQsUDiQ0PrsjpUaM73 other)
		{
			if (EqualityComparer<bool>.Default.Equals(ShowName, other.ShowName) && EqualityComparer<string>.Default.Equals(Name, other.Name) && EqualityComparer<uint>.Default.Equals(NameColor, other.NameColor) && EqualityComparer<bool>.Default.Equals(ShowJobIcon, other.ShowJobIcon) && EqualityComparer<uint>.Default.Equals(JobID, other.JobID) && EqualityComparer<bool>.Default.Equals(ShowDotIcon, other.ShowDotIcon) && EqualityComparer<PlayerIconType>.Default.Equals(DotIconType, other.DotIconType))
			{
				return EqualityComparer<bool>.Default.Equals(IsPvP, other.IsPvP);
			}
			return false;
		}

		[CompilerGenerated]
		public void o7ZirKh78f(out bool ShowName, out string Name, out uint NameColor, out bool ShowJobIcon, out uint JobID, out bool ShowDotIcon, out PlayerIconType DotIconType, out bool IsPvP)
		{
			ShowName = this.ShowName;
			Name = this.Name;
			NameColor = this.NameColor;
			ShowJobIcon = this.ShowJobIcon;
			JobID = this.JobID;
			ShowDotIcon = this.ShowDotIcon;
			DotIconType = this.DotIconType;
			IsPvP = this.IsPvP;
		}
	}

	private enum PlayerIconType
	{
		Friend,
		Party,
		Alliance,
		Other,
		PvPDead,
		PvPMaelstrom,
		PvPTwinAdder,
		PvPImmortalFlames
	}

	private static readonly CompSig SetDataSig;

	private Hook<Mnc9dYiw72HcjgQF19U>? SetDataHook;

	private ovIld8iEGlRppOJbTsm config;

	private static readonly Vector2[] MapPosSize;

	private static readonly Dictionary<Vector3, JreSXXibiAjvqLPISRq> MapMarkers;

	private static readonly Dictionary<Vector3, YfPKklicMbcJd6C26WJ> ControlPointStates;

	private static readonly HashSet<Vector3> VisibleMarkerPositions;

	private static readonly Dictionary<PlayerIconType, float> CachedDotIconSizes;

	private static Vector2? MapOrigin;

	private static float GlobalUIScale;

	private const uint LOCAL_PLAYER_ICON_ID = 60443u;

	private const int CACHE_SUFFIX_LENGTH = 5;

	private const int CONTROL_POINT_DECREASE_INTERVAL_SECONDS = 3;

	private const double CONTROL_POINT_DECREASE_FACTOR = 0.1;

	private const double ZERO_SCORE_DISPLAY_DURATION = 1.0;

	private const float PVP_LOADING_RADIUS = 125f;

	private static readonly List<ClassJob> ValidJobs;

	private static readonly uint[] JobIconBaseIDs;

	private static readonly FrozenSet<uint> CountdownIconIDs;

	private static readonly FrozenSet<uint> HealthIconIDs;

	private static readonly FrozenSet<uint> ControlPointIconIDs;

	private static readonly FrozenSet<uint> IgnoredIconIDs;

	private static readonly FrozenSet<uint> PlayerMarkerIconIDs;

	private static readonly FrozenSet<uint> NeutralControlPointIconIDs;

	private static readonly FrozenSet<uint> AllTrackedIconIDs;

	private static readonly FrozenDictionary<uint, int> SealRockControlPointScores;

	private static readonly FrozenDictionary<uint, int> AzimSteppeControlPointScores;

	private static readonly FrozenDictionary<PlayerIconType, (uint IconID, float FixedScale)> PlayerIcons;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	private unsafe static float MapScale
	{
		get
		{
			AtkUnitBase* areaMap = Addons.AreaMap;
			if (areaMap == null)
			{
				return 1f;
			}
			return ((float*)areaMap)[245];
		}
	}

	protected unsafe override void Init()
	{
		config = ModuleConfigExtension.Load<ovIld8iEGlRppOJbTsm>(this) ?? new ovIld8iEGlRppOJbTsm();
		icvlMRPNPR();
		SetDataHook = SetDataSig.GetHook<Mnc9dYiw72HcjgQF19U>(LwrlyIRuwZ);
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += GUClnOO3YE;
		GUClnOO3YE(0u);
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= GUClnOO3YE;
		OmenServiceBase<WindowManager>.Instance().PostDraw -= GKbl8bmM4q;
		MapMarkers.Clear();
		ControlPointStates.Clear();
		VisibleMarkerPositions.Clear();
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
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		TabBarDisposable val = ImRaii.TabBar(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194F188 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b)));
		try
		{
			if (!val)
			{
				return;
			}
			TabItemDisposable val2 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE7320D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6))));
			try
			{
				if (TabItemDisposable.op_Implicit(val2))
				{
					a5IlLMctHf();
				}
			}
			finally
			{
				((TabItemDisposable)(ref val2)).Dispose();
			}
			TabItemDisposable val3 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5DD79EC1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c7355392ad9e43b6b82798cc42ea6c6f))));
			try
			{
				if (TabItemDisposable.op_Implicit(val3))
				{
					FDslH0xxcX();
				}
			}
			finally
			{
				((TabItemDisposable)(ref val3)).Dispose();
			}
			TabItemDisposable val4 = ImRaii.TabItem(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317E799 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273))));
			try
			{
				if (TabItemDisposable.op_Implicit(val4))
				{
					RwwlSYyv2H();
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

	private void GUClnOO3YE(uint territoryID)
	{
		SetDataHook.Disable();
		OmenServiceBase<WindowManager>.Instance().PostDraw -= GKbl8bmM4q;
		MapMarkers.Clear();
		ControlPointStates.Clear();
		VisibleMarkerPositions.Clear();
		if (C9EloSF6c7())
		{
			SetDataHook.Enable();
			OmenServiceBase<WindowManager>.Instance().PostDraw += GKbl8bmM4q;
		}
	}

	private unsafe MapMarkerData* LwrlyIRuwZ(MapMarkerData* self, uint levelID, Utf8String* tooltipString, uint iconID, float x, float y, float z, float radius, ushort territoryTypeID, uint mapID, uint placeNameZoneID, uint placeNameID, ushort recommendedLevel, sbyte eventState)
	{
		if (!C9EloSF6c7())
		{
			SetDataHook.Disable();
			return SetDataHook.Original(self, levelID, tooltipString, iconID, x, y, z, radius, territoryTypeID, mapID, placeNameZoneID, placeNameID, recommendedLevel, eventState);
		}
		MapMarkerData* result = SetDataHook.Original(self, levelID, tooltipString, iconID, x, y, z, radius, territoryTypeID, mapID, placeNameZoneID, placeNameID, recommendedLevel, eventState);
		Vector3 vector = new Vector3(x, y, z);
		if (IgnoredIconIDs.Contains(iconID))
		{
			return result;
		}
		if (MapMarkers.TryGetValue(vector, out var value) && value.IconID != iconID && wvulfbe9Cl(value.IconID))
		{
			MapMarkers.Remove(vector);
			if (!ControlPointIconIDs.Contains(iconID))
			{
				ControlPointStates.Remove(vector);
			}
		}
		if (!wvulfbe9Cl(iconID))
		{
			return result;
		}
		try
		{
			if (tooltipString == null || !((CStringPointer)(&((Utf8String)tooltipString).StringPtr)).HasValue)
			{
				return result;
			}
			string text = ((object)(*(Utf8String*)tooltipString)/*cast due to constrained. prefix*/).ToString();
			string tooltipText = nTVlCCykYs(text);
			string displayText = string.Empty;
			uint textColor = 0u;
			if (CountdownIconIDs.Contains(iconID))
			{
				displayText = M1hljvWHeO(tooltipText);
				textColor = KnownColor.Goldenrod.ToUInt();
			}
			else if (HealthIconIDs.Contains(iconID))
			{
				displayText = t3ZlODvxD9(tooltipText);
				textColor = KnownColor.Crimson.ToUInt();
			}
			else if (ControlPointIconIDs.Contains(iconID))
			{
				displayText = t3ZlODvxD9(tooltipText);
				textColor = KnownColor.Chocolate.ToUInt();
				G4vlKkhZfi(iconID, vector, text);
			}
			MapMarkers[vector] = new JreSXXibiAjvqLPISRq(iconID, vector, displayText, textColor);
		}
		catch
		{
		}
		return result;
	}

	private void a5IlLMctHf()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		if ((0u | (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3D563696 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e9e296e241f44828a57c750102ce71c1))), ref config.ShowCountdown) ? 1u : 0u) | (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2DBD87FC ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5d8d924f1edf4ec39f023b3c7bb646cc))), ref config.ShowHealthPercent) ? 1u : 0u) | (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x45E48425 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1904f95d2a394c929e1a6d4f72943ad7))), ref config.ShowControlPointScore) ? 1u : 0u)) != 0)
		{
			config.Save(this);
		}
	}

	private void FDslH0xxcX()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(200f * GlobalUIScale);
		ImGui.SliderFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F6499 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))), ref config.DotRadius, 0.5f, 5f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -388594937 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7)), (ImGuiSliderFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			icvlMRPNPR();
			config.Save(this);
		}
		ImGui.SetNextItemWidth(200f * GlobalUIScale);
		ImGui.SliderFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2194F330 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b))), ref config.JobIconSize, 1f, 5f, ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7FD4D9 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009)), (ImGuiSliderFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xE09649D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_aba6092d7cea44a49d1667540500e363))), ref config.HideLoadingRangeInPVP))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD83FAF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3))), ref config.HideFriendlyInPVP))
		{
			config.Save(this);
		}
		ImGui.NewLine();
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1698454374 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_23522a2a14314da28eb44187a09cd0b5))));
		ImGuiOm.HelpMarker(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7FD83F0F ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_1eab987c1db64b928220e27bd03141d3)), 40f, (FontAwesomeIcon)61530);
		ImGui.SetNextItemWidth(200f * GlobalUIScale);
		ImGui.SliderInt(ImU8String.op_Implicit(LuminaWrapper.GetAddonText(15020u)), ref config.JobIconStyle, 1, 4, default(ImU8String), (ImGuiSliderFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.JobIconStyle = Math.Clamp(config.JobIconStyle, 1, 4);
			config.Save(this);
		}
		YkLl9Q0UnX();
	}

	private void RwwlSYyv2H()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5A7F6ACF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5154ccd9fed0487f8fa0244531466009))), ref config.ShowOutsideFrontline))
		{
			config.Save(this);
		}
		jlhlVFZO37(PlayerIconType.Friend, LuminaWrapper.GetAddonText(2941u), KnownColor.Orange, ref config.ShowFriendDots, ref config.ShowFriendNames, ref config.ShowFriendJobIcons);
		jlhlVFZO37(PlayerIconType.Party, LuminaWrapper.GetAddonText(654u), KnownColor.Cyan, ref config.ShowPartyDots, ref config.ShowPartyNames, ref config.ShowPartyJobIcons);
		jlhlVFZO37(PlayerIconType.Alliance, LuminaWrapper.GetAddonText(648u), KnownColor.Green, ref config.ShowAllianceDots, ref config.ShowAllianceNames, ref config.ShowAllianceJobIcons);
		jlhlVFZO37(PlayerIconType.Other, LuminaWrapper.GetAddonText(832u), KnownColor.White, ref config.ShowOtherDots, ref config.ShowOtherNames, ref config.ShowOtherJobIcons);
	}

	private void jlhlVFZO37(PlayerIconType iconType, string categoryText, KnownColor color, ref bool showDotIcons, ref bool showNames, ref bool showJobIcons)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = color.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(categoryText));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImU8String val2 = new ImU8String(6, 2);
			((ImU8String)(ref val2)).AppendFormatted<string>(LuminaWrapper.GetAddonText(12425u));
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x51E5B6F8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_a90a92322bff42e180982e106200d4a3));
			((ImU8String)(ref val2)).AppendFormatted<PlayerIconType>(iconType);
			((ImU8String)(ref val2)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F360C62 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2));
			bool num = ImGui.Checkbox(val2, ref showDotIcons);
			ImGui.SameLine();
			ImU8String val3 = new ImU8String(7, 2);
			((ImU8String)(ref val3)).AppendFormatted<string>(LuminaWrapper.GetAddonText(293u));
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xB1E8B0A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_79d931aaf1c142a2a09f53e716260ad1));
			((ImU8String)(ref val3)).AppendFormatted<PlayerIconType>(iconType);
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-1003249140 ^ -224001313 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			bool flag = ImGui.Checkbox(val3, ref showNames);
			ImGui.SameLine();
			ImU8String val4 = new ImU8String(10, 2);
			((ImU8String)(ref val4)).AppendFormatted<string>(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x591CAFBF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_20724779fecb4a91959b8028d04d1118)));
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE51069 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
			((ImU8String)(ref val4)).AppendFormatted<PlayerIconType>(iconType);
			((ImU8String)(ref val4)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xBA4557C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_2edd08450a8b42e6997855e5217d6d55));
			bool flag2 = ImGui.Checkbox(val4, ref showJobIcons);
			if (num || flag || flag2)
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void YkLl9Q0UnX()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x76EB23C7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_b988ddaee02846b69dd4e446b5da9de9)), 4, (ImGuiTableFlags)8192);
		try
		{
			if (!val)
			{
				return;
			}
			ISharedImmediateTexture val4 = default(ISharedImmediateTexture);
			for (int i = 0; i < ValidJobs.Count; i++)
			{
				ClassJob val2 = ValidJobs[i];
				if (i % 4 == 0)
				{
					ImGui.TableNextRow();
				}
				ImGui.TableNextColumn();
				uint rowId = ((ClassJob)(ref val2)).RowId;
				bool flag = config.HighlightedJobs.Contains(rowId);
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val3 = new GameIconLookup(NvNlg1olt6() + rowId, false, true, (ClientLanguage?)null);
				if (obj.TryGetFromGameIcon(ref val3, ref val4))
				{
					float textLineHeightWithSpacing = ImGui.GetTextLineHeightWithSpacing();
					ImGui.Image(val4.GetWrapOrEmpty().Handle, new Vector2(textLineHeightWithSpacing));
					ImGui.SameLine();
				}
				ImU8String val5 = new ImU8String(5, 2);
				((ImU8String)(ref val5)).AppendFormatted<string>(((object)((ClassJob)(ref val2)).Name/*cast due to constrained. prefix*/).ToString());
				((ImU8String)(ref val5)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x27D10393 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_39d5d8025f5249e6b489bc374eefbb3c));
				((ImU8String)(ref val5)).AppendFormatted<uint>(rowId);
				if (ImGui.Checkbox(val5, ref flag))
				{
					if (flag)
					{
						config.HighlightedJobs.Add(rowId);
					}
					else
					{
						config.HighlightedJobs.Remove(rowId);
					}
					config.Save(this);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private static bool wvulfbe9Cl(uint iconID)
	{
		return AllTrackedIconIDs.Contains(iconID);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string nTVlCCykYs(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			if (text.Length <= 5)
			{
				return text;
			}
			return text.Substring(text.Length - 5);
		}
		return string.Empty;
	}

	private static string M1hljvWHeO(string tooltipText)
	{
		Match match = aKalwiUlEX().Match(tooltipText);
		if (!match.Success)
		{
			return string.Empty;
		}
		if (!int.TryParse(match.Groups[1].ValueSpan, out var result) || !int.TryParse(match.Groups[2].ValueSpan, out var result2))
		{
			return string.Empty;
		}
		if (result >= 1)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
			defaultInterpolatedStringHandler.AppendFormatted(result);
			defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3C148969 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_d71d1e134fde413fa0bee3b2419181b6));
			defaultInterpolatedStringHandler.AppendFormatted(result2, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6C358F56 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88a2dcd1996b4a3fbefaf51a98b59def));
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		return result2.ToString();
	}

	private static string t3ZlODvxD9(string tooltipText)
	{
		Match match = luIlEwDrAi().Match(tooltipText);
		if (!match.Success || !int.TryParse(match.Groups[1].ValueSpan, out var result))
		{
			return string.Empty;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 1);
		defaultInterpolatedStringHandler.AppendFormatted(result);
		defaultInterpolatedStringHandler.AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x7EE73E8D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_88432e96079448078bdf00e7b62f8ab6));
		return defaultInterpolatedStringHandler.ToStringAndClear();
	}

	private static void G4vlKkhZfi(uint iconID, Vector3 position, string tooltipText)
	{
		switch (GameState.TerritoryType)
		{
		case 431u:
			TvdltFbY5K(iconID, position, tooltipText);
			break;
		case 888u:
		case 1313u:
			zCglWUeXw2(iconID, position);
			break;
		}
	}

	private static void TvdltFbY5K(uint iconID, Vector3 position, string tooltipText)
	{
		Match match = luIlEwDrAi().Match(tooltipText);
		if (!match.Success || !int.TryParse(match.Groups[1].ValueSpan, out var result))
		{
			return;
		}
		int num = result % 10;
		bool flag = ((num == 2 || num == 7) ? true : false);
		double value = (flag ? ((double)result + 0.5) : ((double)result));
		if (!SealRockControlPointScores.TryGetValue(iconID, out var value2))
		{
			return;
		}
		if (ControlPointStates.TryGetValue(position, out YfPKklicMbcJd6C26WJ value3))
		{
			if (value3.IconID == iconID)
			{
				value3.CurrentPercent = value;
				return;
			}
			ControlPointStates.Remove(position);
		}
		ControlPointStates[position] = new YfPKklicMbcJd6C26WJ(iconID, null, value2)
		{
			CurrentPercent = value
		};
	}

	private static void zCglWUeXw2(uint iconID, Vector3 position)
	{
		if (!AzimSteppeControlPointScores.TryGetValue(iconID, out var value))
		{
			return;
		}
		bool flag = NeutralControlPointIconIDs.Contains(iconID);
		if (ControlPointStates.TryGetValue(position, out YfPKklicMbcJd6C26WJ value2))
		{
			if (value2.IconID == iconID)
			{
				return;
			}
			ControlPointStates.Remove(position);
		}
		ControlPointStates[position] = new YfPKklicMbcJd6C26WJ(iconID, flag ? ((DateTime?)null) : new DateTime?(OmenServiceBase<StandardTimeManager>.Instance().Now), value)
		{
			CurrentScore = value
		};
	}

	private unsafe static void b63lThRGUx()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		VisibleMarkerPositions.Clear();
		try
		{
			AgentMap* ptr = AgentMap.Instance();
			if (ptr == null)
			{
				return;
			}
			StdVector<MapMarkerData> eventMarkers = ((AgentMap)ptr).EventMarkers;
			int count = eventMarkers.Count;
			for (int i = 0; i < count; i++)
			{
				ref MapMarkerData reference = ref eventMarkers[i];
				if (!PlayerMarkerIconIDs.Contains(reference.IconId))
				{
					VisibleMarkerPositions.Add(reference.Position);
				}
			}
		}
		catch
		{
		}
	}

	private static void Eicl1cbei5()
	{
		uint territoryType = GameState.TerritoryType;
		if ((territoryType != 888 && territoryType != 1313) || 1 == 0)
		{
			return;
		}
		DateTime now = OmenServiceBase<StandardTimeManager>.Instance().Now;
		List<Vector3> list = null;
		foreach (var (item, yfPKklicMbcJd6C26WJ2) in ControlPointStates)
		{
			if (!yfPKklicMbcJd6C26WJ2.ControlStartTime.HasValue)
			{
				continue;
			}
			int num = (int)((now - yfPKklicMbcJd6C26WJ2.ControlStartTime.Value).TotalSeconds / 3.0);
			int num2 = (int)((double)yfPKklicMbcJd6C26WJ2.InitialScore * 0.1);
			int num3 = num * num2;
			yfPKklicMbcJd6C26WJ2.CurrentScore = Math.Max(0, yfPKklicMbcJd6C26WJ2.InitialScore - num3);
			if (yfPKklicMbcJd6C26WJ2.CurrentScore > 0)
			{
				continue;
			}
			if (!yfPKklicMbcJd6C26WJ2.ReachedZeroTime.HasValue)
			{
				yfPKklicMbcJd6C26WJ2.ReachedZeroTime = now;
			}
			else if ((now - yfPKklicMbcJd6C26WJ2.ReachedZeroTime.Value).TotalSeconds >= 1.0)
			{
				if (list == null)
				{
					list = new List<Vector3>();
				}
				list.Add(item);
			}
		}
		if (list == null)
		{
			return;
		}
		foreach (Vector3 item2 in list)
		{
			ControlPointStates.Remove(item2);
		}
	}

	private void icvlMRPNPR()
	{
		float num = config.DotRadius * 40f;
		CachedDotIconSizes.Clear();
		foreach (KeyValuePair<PlayerIconType, (uint, float)> playerIcon in PlayerIcons)
		{
			playerIcon.Deconstruct(out var key, out var value);
			(uint, float) tuple = value;
			PlayerIconType key2 = key;
			float item = tuple.Item2;
			CachedDotIconSizes[key2] = item * num;
		}
	}

	private static PlayerIconType NpOl4QSl3W(byte battalion, bool isDead)
	{
		if (isDead)
		{
			return PlayerIconType.PvPDead;
		}
		return battalion switch
		{
			0 => PlayerIconType.PvPMaelstrom, 
			1 => PlayerIconType.PvPTwinAdder, 
			2 => PlayerIconType.PvPImmortalFlames, 
			_ => PlayerIconType.PvPDead, 
		};
	}

	private unsafe qaQsUDiQ0PrsjpUaM73? MFLls7i3dx(IPlayerCharacter player, Character* character)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		if ((int)GameState.TerritoryIntendedUse == 18)
		{
			if (config.HideFriendlyInPVP && (((Character)character).IsPartyMember || ((Character)character).IsAllianceMember))
			{
				return null;
			}
			PlayerIconType dotIconType = NpOl4QSl3W(((CharacterData)(&((Character)character).CharacterData)).Battalion, player.IsDead);
			bool showJobIcon = config.HighlightedJobs.Contains(player.ClassJob.RowId);
			return new qaQsUDiQ0PrsjpUaM73(ShowName: false, string.Empty, KnownColor.White.ToUInt(), showJobIcon, player.ClassJob.RowId, ShowDotIcon: true, dotIconType, IsPvP: true);
		}
		bool isFriend = ((Character)character).IsFriend;
		bool isPartyMember = ((Character)character).IsPartyMember;
		bool isAllianceMember = ((Character)character).IsAllianceMember;
		bool flag;
		bool flag2;
		bool flag3;
		PlayerIconType dotIconType2;
		KnownColor knownColor;
		if (isFriend)
		{
			flag = config.ShowFriendDots;
			flag2 = config.ShowFriendJobIcons;
			flag3 = config.ShowFriendNames;
			dotIconType2 = PlayerIconType.Friend;
			knownColor = KnownColor.Orange;
		}
		else if (isPartyMember)
		{
			flag = config.ShowPartyDots;
			flag2 = config.ShowPartyJobIcons;
			flag3 = config.ShowPartyNames;
			dotIconType2 = PlayerIconType.Party;
			knownColor = KnownColor.Cyan;
		}
		else if (isAllianceMember)
		{
			flag = config.ShowAllianceDots;
			flag2 = config.ShowAllianceJobIcons;
			flag3 = config.ShowAllianceNames;
			dotIconType2 = PlayerIconType.Alliance;
			knownColor = KnownColor.Green;
		}
		else
		{
			flag = config.ShowOtherDots;
			flag2 = config.ShowOtherJobIcons;
			flag3 = config.ShowOtherNames;
			dotIconType2 = PlayerIconType.Other;
			knownColor = KnownColor.White;
		}
		if (!flag && !flag2 && !flag3)
		{
			return null;
		}
		return new qaQsUDiQ0PrsjpUaM73(flag3, player.Name, knownColor.ToUInt(), flag2, player.ClassJob.RowId, flag, dotIconType2, IsPvP: false);
	}

	private void GKbl8bmM4q()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Invalid comparison between Unknown and I4
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!C9EloSF6c7())
		{
			OmenServiceBase<WindowManager>.Instance().PostDraw -= GKbl8bmM4q;
		}
		else
		{
			if (!KG7lhPgCo4() || GameState.TerritoryType == 0)
			{
				return;
			}
			MiPlBLS4Xt();
			Vector2? mapOrigin = MapOrigin;
			if (!mapOrigin.HasValue)
			{
				return;
			}
			Vector2 valueOrDefault = mapOrigin.GetValueOrDefault();
			if (!(valueOrDefault == Vector2.Zero))
			{
				Map mapData = GameState.MapData;
				ImDrawListPtr backgroundDrawList = ImGui.GetBackgroundDrawList();
				float scale = MapScale * GlobalUIScale;
				((ImDrawListPtr)(ref backgroundDrawList)).PushClipRect(MapPosSize[0], MapPosSize[1]);
				if ((int)GameState.TerritoryIntendedUse == 18)
				{
					b63lThRGUx();
					Eicl1cbei5();
					SF4lviG4tY(backgroundDrawList, valueOrDefault, mapData, scale);
				}
				uyclxxJSnr(backgroundDrawList, valueOrDefault, mapData, scale);
				((ImDrawListPtr)(ref backgroundDrawList)).PopClipRect();
			}
		}
	}

	private void SF4lviG4tY(ImDrawListPtr drawList, Vector2 origin, Map map, float scale)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		if (MapMarkers.Count == 0)
		{
			return;
		}
		Vector2 vector = PositionHelper.WorldToTexture(IOmenDalamudService<IObjectTable>.Instance().LocalPlayer?.Position ?? Vector3.Zero, map);
		foreach (JreSXXibiAjvqLPISRq value in MapMarkers.Values)
		{
			if (VisibleMarkerPositions.Contains(value.Position))
			{
				Vector2 vector2 = (PositionHelper.WorldToTexture(value.Position, map) - vector) * scale;
				Vector2 pos = origin + vector2;
				BBwllo8sfa(drawList, pos, value);
			}
		}
	}

	private void BBwllo8sfa(ImDrawListPtr drawList, Vector2 pos, JreSXXibiAjvqLPISRq marker)
	{
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		uint textColor = KnownColor.White.ToUInt();
		if (!string.IsNullOrEmpty(marker.DisplayText))
		{
			bool num = CountdownIconIDs.Contains(marker.IconID) && !marker.DisplayText.Contains('%');
			bool flag = HealthIconIDs.Contains(marker.IconID) && marker.DisplayText.Contains('%');
			if ((num && config.ShowCountdown) || (flag && config.ShowHealthPercent))
			{
				ImGuiOm.TextOutlined(pos, textColor, marker.DisplayText, marker.TextColor, 1.5f, drawList);
			}
		}
		if (ControlPointIconIDs.Contains(marker.IconID) && config.ShowControlPointScore && ControlPointStates.TryGetValue(marker.Position, out YfPKklicMbcJd6C26WJ value))
		{
			double displayScore = value.DisplayScore;
			if (displayScore > 0.0 || (value.ControlStartTime.HasValue && displayScore == 0.0))
			{
				string text = ((int)Math.Round(displayScore)).ToString();
				ImGuiOm.TextOutlined(pos, textColor, text, marker.TextColor, 1.5f, drawList);
			}
		}
	}

	private unsafe void uyclxxJSnr(ImDrawListPtr drawList, Vector2 origin, Map map, float scale)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Invalid comparison between Unknown and I4
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		Vector2 vector = PositionHelper.WorldToTexture(localPlayer.Position, map);
		uint jobIconBaseID = NvNlg1olt6();
		foreach (IPlayerCharacter item in ManagerBase<PlayersManager>.Instance().PlayersAround)
		{
			Character* ptr = (Character*)item.ToStruct();
			if (ptr != null && item.Address != localPlayer.Address)
			{
				qaQsUDiQ0PrsjpUaM73? qaQsUDiQ0PrsjpUaM74 = MFLls7i3dx(item, ptr);
				if (qaQsUDiQ0PrsjpUaM74.HasValue)
				{
					qaQsUDiQ0PrsjpUaM73 valueOrDefault = qaQsUDiQ0PrsjpUaM74.GetValueOrDefault();
					Vector2 vector2 = (PositionHelper.WorldToTexture(item.Position, map) - vector) * scale;
					Vector2 pos = origin + vector2;
					g9tlpEQOYF(drawList, pos, valueOrDefault, jobIconBaseID);
				}
			}
		}
		if ((int)GameState.TerritoryIntendedUse == 18 && !config.HideLoadingRangeInPVP)
		{
			Vector3 position = localPlayer.Position;
			Vector3 vector3 = position;
			vector3.X = position.X + 125f;
			Vector3 pos2 = vector3;
			Vector2 value = PositionHelper.WorldToTexture(position, map);
			Vector2 value2 = PositionHelper.WorldToTexture(pos2, map);
			float num = Vector2.Distance(value, value2) * scale;
			((ImDrawListPtr)(ref drawList)).AddCircle(origin, num, KnownColor.Gray.ToUInt(), 0, 2f);
		}
	}

	private void g9tlpEQOYF(ImDrawListPtr drawList, Vector2 pos, qaQsUDiQ0PrsjpUaM73 info, uint jobIconBaseID)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (info.ShowJobIcon)
		{
			uint num = jobIconBaseID + info.JobID;
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(num, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
			if (obj.TryGetFromGameIcon(ref val, ref val2))
			{
				float num2 = config.JobIconSize * ImGui.GetTextLineHeightWithSpacing();
				Vector2 vector = (info.IsPvP ? (pos - new Vector2(num2 / 2f)) : (pos - new Vector2(num2 / 2f, num2 / 1.2f)));
				((ImDrawListPtr)(ref drawList)).AddImage(val2.GetWrapOrEmpty().Handle, vector, vector + new Vector2(num2), Vector2.Zero, Vector2.One, KnownColor.White.ToUInt());
			}
		}
		if (info.ShowDotIcon)
		{
			uint item = PlayerIcons[info.DotIconType].IconID;
			ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(item, false, true, (ClientLanguage?)null);
			ISharedImmediateTexture val3 = default(ISharedImmediateTexture);
			if (obj2.TryGetFromGameIcon(ref val, ref val3))
			{
				float num3 = CachedDotIconSizes[info.DotIconType];
				Vector2 vector2 = pos - new Vector2(num3 / 2f);
				((ImDrawListPtr)(ref drawList)).AddImage(val3.GetWrapOrEmpty().Handle, vector2, vector2 + new Vector2(num3), Vector2.Zero, Vector2.One, KnownColor.White.ToUInt());
			}
		}
		if (info.ShowName && !string.IsNullOrWhiteSpace(info.Name))
		{
			rKNl3iRA4A(drawList, pos, info.Name, info.NameColor, stroke: true);
		}
	}

	private static void rKNl3iRA4A(ImDrawListPtr drawList, Vector2 pos, string text, uint col, bool stroke, bool centerAlignX = true)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (centerAlignX)
		{
			pos -= new Vector2(ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X / 2f, 0f);
		}
		if (stroke)
		{
			uint num = KnownColor.Black.ToUInt();
			((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(-1f, -1f), num, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(-1f, 1f), num, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(1f, -1f), num, ImU8String.op_Implicit(text));
			((ImDrawListPtr)(ref drawList)).AddText(pos + new Vector2(1f, 1f), num, ImU8String.op_Implicit(text));
		}
		((ImDrawListPtr)(ref drawList)).AddText(pos, col, ImU8String.op_Implicit(text));
	}

	private unsafe static void MiPlBLS4Xt()
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		MapOrigin = null;
		AtkUnitBase* areaMap = Addons.AreaMap;
		if (!(*areaMap).IsAddonAndNodesReady() || ((AtkUldManager)(&((AtkUnitBase)areaMap).UldManager)).NodeListCount <= 4)
		{
			return;
		}
		GlobalUIScale = ((AtkUnitBase)areaMap).Scale;
		AtkComponentNode* ptr = (AtkComponentNode*)((AtkUldManager)(&((AtkUnitBase)areaMap).UldManager)).NodeList[3];
		if (ptr != null && ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr).Component).UldManager)).NodeListCount >= 233)
		{
			AtkResNode atkResNode = ((AtkComponentNode)ptr).AtkResNode;
			if (iUTlmGdDtG(ptr, out var result))
			{
				GdXlPIuM1n(areaMap, atkResNode, result);
			}
		}
	}

	private unsafe static bool iUTlmGdDtG(AtkComponentNode* componentNode, out AtkComponentNode* result)
	{
		result = null;
		for (int i = 6; i < ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNode).Component).UldManager)).NodeListCount - 1; i++)
		{
			AtkResNode* ptr = ((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)componentNode).Component).UldManager)).NodeList[i];
			if (ptr != null && ((AtkResNode)ptr).IsVisible())
			{
				AtkComponentNode* ptr2 = (AtkComponentNode*)ptr;
				AtkImageNode* imageNode = (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)((AtkComponentNode)ptr2).Component).UldManager)).NodeList[4];
				if (IN7li7IXa0(imageNode) == 60443)
				{
					result = ptr2;
					return true;
				}
			}
		}
		return false;
	}

	private unsafe static uint? IN7li7IXa0(AtkImageNode* imageNode)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Invalid comparison between Unknown and I4
		if (((AtkImageNode)imageNode).PartsList == null || ((AtkImageNode)imageNode).PartId >= ((AtkUldPartsList)((AtkImageNode)imageNode).PartsList).PartCount)
		{
			return null;
		}
		AtkUldAsset* uldAsset = ((AtkUldPart*)((AtkUldPartsList)((AtkImageNode)imageNode).PartsList).Parts)[(int)((AtkImageNode)imageNode).PartId].UldAsset;
		if (uldAsset == null || (int)((AtkTexture)(&((AtkUldAsset)uldAsset).AtkTexture)).TextureType != 1 || ((AtkTexture)(&((AtkUldAsset)uldAsset).AtkTexture)).Resource == null)
		{
			return null;
		}
		return ((AtkTextureResource)((AtkTexture)(&((AtkUldAsset)uldAsset).AtkTexture)).Resource).IconId;
	}

	private unsafe static void GdXlPIuM1n(AtkUnitBase* areaMapAddon, AtkResNode baseNode, AtkComponentNode* innerComponentNode)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		AtkResNode atkResNode = ((AtkComponentNode)innerComponentNode).AtkResNode;
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		Vector2 vector = new Vector2(((AtkUnitBase)areaMapAddon).X, ((AtkUnitBase)areaMapAddon).Y);
		MapOrigin = ((ImGuiViewportPtr)(ref mainViewport)).Pos + vector + (new Vector2(baseNode.X, baseNode.Y) + new Vector2(atkResNode.X, atkResNode.Y) + new Vector2(atkResNode.OriginX, atkResNode.OriginY)) * GlobalUIScale;
		MapPosSize[0] = ((ImGuiViewportPtr)(ref mainViewport)).Pos + vector + new Vector2(baseNode.X, baseNode.Y) * GlobalUIScale;
		MapPosSize[1] = ((ImGuiViewportPtr)(ref mainViewport)).Pos + vector + (new Vector2(baseNode.X, baseNode.Y) + new Vector2((int)baseNode.Width, (int)baseNode.Height) * GlobalUIScale);
	}

	private static bool KG7lhPgCo4()
	{
		if (IOmenDalamudService<IObjectTable>.Instance().LocalPlayer != null)
		{
			return !DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)45];
		}
		return false;
	}

	private bool C9EloSF6c7()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		if ((int)GameState.TerritoryIntendedUse != 18)
		{
			return config.ShowOutsideFrontline;
		}
		return true;
	}

	private uint NvNlg1olt6()
	{
		return JobIconBaseIDs[Math.Clamp(config.JobIconStyle, 1, 4) - 1];
	}

	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	[GeneratedRegex("(\\d+):(\\d+)")]
	private static Regex aKalwiUlEX()
	{
		return ND9hdrTagCGnhqUV0JO.Instance;
	}

	[GeneratedCode("System.Text.RegularExpressions.Generator", "10.0.14.42308")]
	[GeneratedRegex("(\\d+)%")]
	private static Regex luIlEwDrAi()
	{
		return gbiTcrT5DPfkfcbsjVW.Instance;
	}

	public FrontlineMapRadar()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317E879 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6AAE9E44 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_839f542231154d29b0562b687afa328a)),
			Category = ModuleCategory.Duty,
			Author = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C2B0D ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee) },
			PreviewImageURL = new string[1] { LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3AC9FBC8 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3932528062874ea4bb7e2e1185394e37) }
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		base._002Ector();
	}

	static FrontlineMapRadar()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		SetDataSig = new CompSig(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2CE59D77 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c9dc773883f149b7befba3bdbeac55f7));
		MapPosSize = new Vector2[2];
		MapMarkers = new Dictionary<Vector3, JreSXXibiAjvqLPISRq>();
		ControlPointStates = new Dictionary<Vector3, YfPKklicMbcJd6C26WJ>();
		VisibleMarkerPositions = new HashSet<Vector3>();
		CachedDotIconSizes = new Dictionary<PlayerIconType, float>();
		GlobalUIScale = 1f;
		ValidJobs = (from x in (IEnumerable<ClassJob>)LuminaGetter.Get<ClassJob>()
			where !((ClassJob)(ref x)).IsLimitedJob && ((ClassJob)(ref x)).DohDolJobIndex == -1 && ((ClassJob)(ref x)).ItemSoulCrystal.RowId != 0
			orderby ((ClassJob)(ref x)).Role switch
			{
				1 => 0, 
				4 => 1, 
				2 => 2, 
				3 => 3, 
				_ => 4, 
			}, ((ClassJob)(ref x)).RowId
			select x).ToList();
		JobIconBaseIDs = new uint[4] { 62000u, 62100u, 62225u, 62800u };
		CountdownIconIDs = FrozenSet.Create<uint>(60628u, 60629u, 60630u, 60624u, 60625u, 60626u, 60620u, 60621u, 60622u, 60989u, 60990u, 63987u, 63979u, 63986u, 60992u, 63985u, 60991u);
		HealthIconIDs = FrozenSet.Create<uint>(60902u, 60904u, 60999u, 60998u);
		ControlPointIconIDs = FrozenSet.Create<uint>(60585u, 60586u, 60587u, 60588u, 60589u, 60590u, 60591u, 60592u, 60593u, 60594u, 60595u, 60596u);
		IgnoredIconIDs = FrozenSet.Create<uint>(60484u, 60485u, 60486u);
		PlayerMarkerIconIDs = FrozenSet.Create<uint>(60359u, 60360u, 60361u);
		NeutralControlPointIconIDs = FrozenSet.Create<uint>(60585u, 60589u, 60593u);
		FrozenSet<uint> countdownIconIDs = CountdownIconIDs;
		FrozenSet<uint> healthIconIDs = HealthIconIDs;
		FrozenSet<uint> controlPointIconIDs = ControlPointIconIDs;
		int num = 0;
		uint[] array = new uint[countdownIconIDs.Count + healthIconIDs.Count + controlPointIconIDs.Count];
		foreach (uint item in countdownIconIDs)
		{
			array[num] = item;
			num++;
		}
		foreach (uint item2 in healthIconIDs)
		{
			array[num] = item2;
			num++;
		}
		foreach (uint item3 in controlPointIconIDs)
		{
			array[num] = item3;
			num++;
		}
		AllTrackedIconIDs = FrozenSet.Create(new ReadOnlySpan<uint>(array));
		SealRockControlPointScores = new Dictionary<uint, int>
		{
			[60585u] = 80,
			[60586u] = 80,
			[60587u] = 80,
			[60588u] = 80,
			[60589u] = 120,
			[60590u] = 120,
			[60591u] = 120,
			[60592u] = 120,
			[60593u] = 160,
			[60594u] = 160,
			[60595u] = 160,
			[60596u] = 160
		}.ToFrozenDictionary();
		AzimSteppeControlPointScores = new Dictionary<uint, int>
		{
			[60585u] = 50,
			[60586u] = 50,
			[60587u] = 50,
			[60588u] = 50,
			[60589u] = 100,
			[60590u] = 100,
			[60591u] = 100,
			[60592u] = 100,
			[60593u] = 200,
			[60594u] = 200,
			[60595u] = 200,
			[60596u] = 200
		}.ToFrozenDictionary();
		PlayerIcons = new Dictionary<PlayerIconType, (uint, float)>
		{
			[PlayerIconType.Friend] = (60424u, 1.8f),
			[PlayerIconType.Party] = (60421u, 1f),
			[PlayerIconType.Alliance] = (60403u, 1f),
			[PlayerIconType.Other] = (60909u, 1.2f),
			[PlayerIconType.PvPDead] = (60909u, 1.2f),
			[PlayerIconType.PvPMaelstrom] = (60359u, 1f),
			[PlayerIconType.PvPTwinAdder] = (60360u, 1f),
			[PlayerIconType.PvPImmortalFlames] = (60361u, 1f)
		}.ToFrozenDictionary();
	}
}
