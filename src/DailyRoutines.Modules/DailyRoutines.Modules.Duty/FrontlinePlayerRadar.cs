using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;
using OmenTools;
using OmenTools.Dalamud.Services.Game.Object.Abstractions;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Dalamud.Services.Game.Object.Enums;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.Info.Game.Data;
using OmenTools.Interop.Game.Helpers;
using OmenTools.Interop.Game.Lumina;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading;
using SU7hSZh8HD1Lef5xwqx;
using XY51fthOdjivExIYfkA;

namespace DailyRoutines.Modules.Duty;

public class FrontlinePlayerRadar : ModuleBase
{
	private class DVFObWiGAOssFJDUgxn : ModuleConfig
	{
		public float DotRadius;

		public bool HideFriendlyCharacters;

		public bool OnlyDisplayDot;

		public bool ValidOutsideFrontline;

		public DVFObWiGAOssFJDUgxn()
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			DotRadius = 8f;
			base._002Ector();
		}
	}

	private sealed class KaymSqi6x8cncKDt1bh : IEquatable<KaymSqi6x8cncKDt1bh>
	{
		[CompilerGenerated]
		private Type EqualityContract
		{
			[CompilerGenerated]
			get
			{
				return typeof(KaymSqi6x8cncKDt1bh);
			}
		}

		public ICharacter Character { get; init; }

		public string Name { get; init; }

		public BattalionFlags Battalion { get; init; }

		public uint Color { get; init; }

		public uint JobIcon { get; init; }

		public uint MarkerIcon { get; init; }

		public KaymSqi6x8cncKDt1bh(ICharacter Character, string Name, BattalionFlags Battalion, uint Color, uint JobIcon, uint MarkerIcon = 0u)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			this.Character = Character;
			this.Name = Name;
			this.Battalion = Battalion;
			this.Color = Color;
			this.JobIcon = JobIcon;
			this.MarkerIcon = MarkerIcon;
			base._002Ector();
		}

		public static KaymSqi6x8cncKDt1bh MIyikZrfZe(FrontlinePlayerRadar instance, ICharacter chara)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			string name = chara.Name + (chara.IsDead ? LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x2195444E ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_cd1b319f326a44d1aaea645e95d8698b) : string.Empty);
			BattalionFlags battalion = chara.Battalion;
			uint color = DrGlQTQPSX(battalion).ToUInt();
			uint jobIcon = 62100 + chara.ClassJob.RowId;
			uint markerIcon = 0u;
			if (instance.targetMarkers.TryGetValue(chara.GameObjectID, out var value))
			{
				markerIcon = TJWlJ8Rm22(value);
			}
			return new KaymSqi6x8cncKDt1bh(chara, name, battalion, color, jobIcon, markerIcon);
		}

		public bool ByWi2Ipmln(out float currentCastTime, out float totalCastTime)
		{
			currentCastTime = (totalCastTime = 1f);
			if (!Character.IsValid() || !(Character is IBattleChara battleChara))
			{
				return false;
			}
			currentCastTime = battleChara.CurrentCastTime;
			totalCastTime = battleChara.TotalCastTime;
			return battleChara.IsCasting;
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C43EC78 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd));
			stringBuilder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6E004518 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_c40146824d9043e0906f4e1889cf39aa));
			if (xy8iZOQGO7(stringBuilder))
			{
				stringBuilder.Append(' ');
			}
			stringBuilder.Append('}');
			return stringBuilder.ToString();
		}

		[CompilerGenerated]
		private bool xy8iZOQGO7(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5356D8EE ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_0676edd1b45a4ff096830fd6229bc1fb));
			builder.Append(Character);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x64E5F8F0 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f53034f3efc64a309b10c7885bf5d9bd));
			builder.Append((object?)Name);
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x6FF0EFCA ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912));
			builder.Append(Battalion.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x469B2F13 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_bb1eb496bad948e4a2baaa3e1edc7a92));
			builder.Append(Color.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x369418CF ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_26e231ba68f74e8784503e099ddfc2d9));
			builder.Append(JobIcon.ToString());
			builder.Append(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x454F07E1 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_09da05f368544962acb62b42d9155467));
			builder.Append(MarkerIcon.ToString());
			return true;
		}

		[CompilerGenerated]
		public static bool operator !=(KaymSqi6x8cncKDt1bh? left, KaymSqi6x8cncKDt1bh? right)
		{
			return !(left == right);
		}

		[CompilerGenerated]
		public static bool operator ==(KaymSqi6x8cncKDt1bh? left, KaymSqi6x8cncKDt1bh? right)
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
			return (((((EqualityComparer<Type>.Default.GetHashCode(EqualityContract) * -1521134295 + EqualityComparer<ICharacter>.Default.GetHashCode(Character)) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name)) * -1521134295 + EqualityComparer<BattalionFlags>.Default.GetHashCode(Battalion)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(Color)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(JobIcon)) * -1521134295 + EqualityComparer<uint>.Default.GetHashCode(MarkerIcon);
		}

		[CompilerGenerated]
		public override bool Equals(object? obj)
		{
			return Equals(obj as KaymSqi6x8cncKDt1bh);
		}

		[CompilerGenerated]
		public bool Equals(KaymSqi6x8cncKDt1bh? other)
		{
			if ((object)this != other)
			{
				if ((object)other != null && EqualityContract == other.EqualityContract && EqualityComparer<ICharacter>.Default.Equals(Character, other.Character) && EqualityComparer<string>.Default.Equals(Name, other.Name) && EqualityComparer<BattalionFlags>.Default.Equals(Battalion, other.Battalion) && EqualityComparer<uint>.Default.Equals(Color, other.Color) && EqualityComparer<uint>.Default.Equals(JobIcon, other.JobIcon))
				{
					return EqualityComparer<uint>.Default.Equals(MarkerIcon, other.MarkerIcon);
				}
				return false;
			}
			return true;
		}

		[CompilerGenerated]
		public KaymSqi6x8cncKDt1bh fkqianFl32()
		{
			return new KaymSqi6x8cncKDt1bh(this);
		}

		[CompilerGenerated]
		private KaymSqi6x8cncKDt1bh(KaymSqi6x8cncKDt1bh original)
		{
			UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
			base._002Ector();
			Character = original.Character;
			Name = original.Name;
			Battalion = original.Battalion;
			Color = original.Color;
			JobIcon = original.JobIcon;
			MarkerIcon = original.MarkerIcon;
		}

		[CompilerGenerated]
		public void TLvi0tQRW3(out ICharacter Character, out string Name, out BattalionFlags Battalion, out uint Color, out uint JobIcon, out uint MarkerIcon)
		{
			Character = this.Character;
			Name = this.Name;
			Battalion = this.Battalion;
			Color = this.Color;
			JobIcon = this.JobIcon;
			MarkerIcon = this.MarkerIcon;
		}
	}

	private DVFObWiGAOssFJDUgxn config;

	private readonly Dictionary<ulong, uint> targetMarkers;

	private readonly Dictionary<uint, Vector3> fieldMarkers;

	private readonly Dictionary<ulong, KaymSqi6x8cncKDt1bh> playerInfos;

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)2896863;

	public override ModuleInfo Info { get; }

	public override ModulePermission Permission { get; }

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<DVFObWiGAOssFJDUgxn>(this) ?? new DVFObWiGAOssFJDUgxn();
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged += UAFlbN1KHE;
		UAFlbN1KHE(0u);
	}

	protected override void ConfigUI()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(75f * Globals.GlobalUIScale);
		ImGui.InputFloat(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x477EAF0B ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_ba27244341a5414496a87eae29cf3dec))), ref config.DotRadius, 0f, 0f, default(ImU8String), (ImGuiInputTextFlags)0);
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x5017EC30 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_e227921f7ee94b6aa1c28169395ba0cf))), ref config.ValidOutsideFrontline))
		{
			config.Save(this);
			UAFlbN1KHE(0u);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x32D1A81 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_86d75c161ced498ab6a767d2eb82d38e))), ref config.OnlyDisplayDot))
		{
			config.Save(this);
		}
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(-224923271 ^ -1654217887 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_5859f4ea3042459db2c9b610b3099912))), ref config.HideFriendlyCharacters))
		{
			config.Save(this);
		}
	}

	protected override void Uninit()
	{
		DalamudServiceExtension.Instance<IClientState>().TerritoryChanged -= UAFlbN1KHE;
		OmenServiceBase<WindowManager>.Instance().PostDraw -= FiAlqNJCPg;
	}

	private void UAFlbN1KHE(uint zone)
	{
		OmenServiceBase<WindowManager>.Instance().PostDraw -= FiAlqNJCPg;
		targetMarkers.Clear();
		fieldMarkers.Clear();
		playerInfos.Clear();
		if (BOclFjbSN1())
		{
			OmenServiceBase<WindowManager>.Instance().PostDraw += FiAlqNJCPg;
		}
	}

	private bool BOclFjbSN1()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Invalid comparison between Unknown and I4
		if (!config.ValidOutsideFrontline)
		{
			return (int)GameState.TerritoryIntendedUse == 18;
		}
		return true;
	}

	private unsafe void FiAlqNJCPg()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Invalid comparison between Unknown and I4
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_052f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_050b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0546: Unknown result type (might be due to invalid IL or missing references)
		//IL_057a: Unknown result type (might be due to invalid IL or missing references)
		//IL_057f: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		if (DalamudServiceExtension.Instance<ICondition>()[(ConditionFlag)38] || Addons.FrontlineRecord != null)
		{
			return;
		}
		IPlayerCharacter localPlayer = IOmenDalamudService<IObjectTable>.Instance().LocalPlayer;
		if (localPlayer == null)
		{
			return;
		}
		cEvlAL7hWT();
		Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4C42506A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_4643f8ec0000417ba22db1f06f6757bd)), false, -1f);
		float textLineHeightWithSpacing = ImGui.GetTextLineHeightWithSpacing();
		ImGuiStylePtr style = ImGui.GetStyle();
		float y = ((ImGuiStylePtr)(ref style)).ItemSpacing.Y;
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		Vector3 vector2 = new Vector3(0f, ((ModelContainer)(&((BattleChara)localPlayer.ToStruct()).ModelContainer)).CalculateHeight(), 0f);
		Vector2 vector3 = default(Vector2);
		bool flag = default(bool);
		ISharedImmediateTexture val2 = default(ISharedImmediateTexture);
		ISharedImmediateTexture val7 = default(ISharedImmediateTexture);
		ISharedImmediateTexture val8 = default(ISharedImmediateTexture);
		foreach (IPlayerCharacter item in ManagerBase<PlayersManager>.Instance().PlayersAround)
		{
			BattleChara* ptr = item.ToStruct();
			if (ptr == null)
			{
				continue;
			}
			ulong contentId = ((BattleChara)ptr).ContentId;
			if (!playerInfos.TryGetValue(contentId, out KaymSqi6x8cncKDt1bh value) || (config.HideFriendlyCharacters && (int)GameState.TerritoryIntendedUse == 18 && localPlayer.Battalion == value.Battalion))
			{
				continue;
			}
			DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(item.Position, ref vector3, ref flag);
			bool isDead = value.Character.IsDead;
			if (!flag)
			{
				ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = new GameIconLookup(value.JobIcon, false, true, (ClientLanguage?)null);
				if (obj.TryGetFromGameIcon(ref val, ref val2))
				{
					float textLineHeight = ImGui.GetTextLineHeight();
					float num = 2f * Globals.GlobalUIScale;
					Vector2 vector4 = zVFlc9JGAQ(vector3, ((ImGuiViewportPtr)(ref mainViewport)).Pos, ((ImGuiViewportPtr)(ref mainViewport)).Size, textLineHeight * 0.5f + num + 8f * Globals.GlobalUIScale) - new Vector2(textLineHeight * 0.5f);
					Vector2 vector5 = vector4 + new Vector2(textLineHeight);
					float num2 = (isDead ? 0.5f : 1f);
					float num3 = 4f * Globals.GlobalUIScale;
					ImDrawListPtr foregroundDrawList = ImGui.GetForegroundDrawList();
					Vector2 vector6 = vector4 - new Vector2(num);
					Vector2 vector7 = vector5 + new Vector2(num);
					Vector4 color = KnownColor.Black.ToVector4();
					color.W = 0.72f * num2;
					((ImDrawListPtr)(ref foregroundDrawList)).AddRectFilled(vector6, vector7, color.ToUInt(), num3);
					ImTextureID handle = val2.GetWrapOrEmpty().Handle;
					Vector2 zero = Vector2.Zero;
					Vector2 one = Vector2.One;
					color = Vector4.One;
					color.W = num2;
					((ImDrawListPtr)(ref foregroundDrawList)).AddImage(handle, vector4, vector5, zero, one, color.ToUInt());
					color = value.Color.ToVector4();
					color.W = num2;
					((ImDrawListPtr)(ref foregroundDrawList)).AddRect(vector6, vector7, color.ToUInt(), num3, (ImDrawFlags)0, 2f * Globals.GlobalUIScale);
				}
				continue;
			}
			vector3 = new Vector2((int)vector3.X, (int)vector3.Y);
			Vector3 vector8 = new Vector3(0f, ((ModelContainer)(&((BattleChara)ptr).ModelContainer)).CalculateHeight(), 0f);
			bool flag2 = !RaycastHelper.HasLineOfSight(localPlayer.Position + vector2, item.Position + vector8);
			bool flag3 = isDead || flag2;
			ImGuiOm.CircleOutlined(vector3, config.DotRadius, value.Color, 4278190080u, 1.5f, flag3 ? 0.5f : 1f);
			if (config.OnlyDisplayDot)
			{
				continue;
			}
			Vector2 vector9 = ImGui.CalcTextSize(ImU8String.op_Implicit(value.Name), false, -1f);
			float num4 = (float)new List<uint> { value.JobIcon, value.MarkerIcon }.Count((uint x) => x != 0) * textLineHeightWithSpacing;
			ImGui.SetNextWindowPos(new Vector2(vector3.X - (8f + num4 + vector9.X / 2f), vector3.Y - (8f + textLineHeightWithSpacing + vector9.Y + y)));
			ImU8String val3 = new ImU8String(15, 1);
			((ImU8String)(ref val3)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x398723DD ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_56ae6e3f2cdf4dd8a743f4d493ce49a5));
			((ImU8String)(ref val3)).AppendFormatted<ulong>(contentId);
			if (!ImGui.Begin(val3, (ImGuiWindowFlags)2896863))
			{
				continue;
			}
			StyleDisposable val4 = ImRaii.PushStyle((ImGuiStyleVar)0, 0.5f, flag3);
			try
			{
				GroupDisposable val5 = ImRaii.Group();
				try
				{
					ImGuiOm.ScaledDummy(12f);
					if (value.ByWi2Ipmln(out var currentCastTime, out var totalCastTime))
					{
						float num5 = currentCastTime / totalCastTime;
						Vector2 vector10 = new Vector2(Math.Max(vector.X, vector9.X) + textLineHeightWithSpacing + 8f, textLineHeightWithSpacing);
						ImU8String val6 = new ImU8String(0, 1);
						((ImU8String)(ref val6)).AppendFormatted<float>(totalCastTime - currentCastTime, LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0xF207F13 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_fea4ad1b13de4c4bbc22f571ceda7372));
						ImGui.ProgressBar(num5, vector10, val6);
					}
					ITextureProvider obj2 = DalamudServiceExtension.Instance<ITextureProvider>();
					GameIconLookup val = new GameIconLookup(value.JobIcon, false, true, (ClientLanguage?)null);
					if (obj2.TryGetFromGameIcon(ref val, ref val7))
					{
						ImGui.Image(val7.GetWrapOrEmpty().Handle, new Vector2(textLineHeightWithSpacing));
					}
					if (value.MarkerIcon != 0)
					{
						ITextureProvider obj3 = DalamudServiceExtension.Instance<ITextureProvider>();
						val = new GameIconLookup(value.MarkerIcon, false, true, (ClientLanguage?)null);
						if (obj3.TryGetFromGameIcon(ref val, ref val8))
						{
							ImGui.SameLine();
							ImGui.Image(val8.GetWrapOrEmpty().Handle, new Vector2(textLineHeightWithSpacing));
						}
					}
					ImGui.SameLine();
					Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
					ImGuiOm.TextOutlined(value.Color, value.Name);
					Vector2 vector11 = vector9;
					vector11.Y = 0f;
					ImGui.SetCursorScreenPos(cursorScreenPos + vector11);
				}
				finally
				{
					((GroupDisposable)(ref val5)).Dispose();
				}
			}
			finally
			{
				((IDisposable)val4)?.Dispose();
			}
			ImGui.End();
		}
		Vector2 vector12 = default(Vector2);
		ISharedImmediateTexture val10 = default(ISharedImmediateTexture);
		foreach (KeyValuePair<uint, Vector3> fieldMarker in fieldMarkers)
		{
			if (!DalamudServiceExtension.Instance<IGameGui>().WorldToScreen(fieldMarker.Value, ref vector12))
			{
				continue;
			}
			vector12 = new Vector2((int)vector12.X, (int)vector12.Y);
			ImGui.SetNextWindowPos(vector12 - new Vector2(48f));
			ImU8String val9 = new ImU8String(27, 1);
			((ImU8String)(ref val9)).AppendLiteral(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D75B27A ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_f8b51f8019ce49018b38e06104a97fc1));
			((ImU8String)(ref val9)).AppendFormatted<uint>(fieldMarker.Key);
			if (ImGui.Begin(val9, (ImGuiWindowFlags)2896863))
			{
				ITextureProvider obj4 = DalamudServiceExtension.Instance<ITextureProvider>();
				GameIconLookup val = new GameIconLookup(cdilrq1gLR(fieldMarker.Key), false, true, (ClientLanguage?)null);
				if (obj4.TryGetFromGameIcon(ref val, ref val10))
				{
					ImGui.Image(val10.GetWrapOrEmpty().Handle, new Vector2(textLineHeightWithSpacing * 3f));
					ImGui.End();
				}
			}
		}
	}

	private static Vector2 zVFlc9JGAQ(Vector2 projectedPosition, Vector2 viewportPosition, Vector2 viewportSize, float viewportInset)
	{
		Vector2 vector = viewportPosition + viewportSize * 0.5f;
		Vector2 vector2 = projectedPosition - vector;
		if (vector2.LengthSquared() < 0.001f)
		{
			vector2 = Vector2.UnitY;
		}
		float num = Math.Max(viewportSize.X * 0.5f - viewportInset, 1f);
		float num2 = Math.Max(viewportSize.Y * 0.5f - viewportInset, 1f);
		float x = ((MathF.Abs(vector2.X) > float.Epsilon) ? (num / MathF.Abs(vector2.X)) : float.MaxValue);
		float y = ((MathF.Abs(vector2.Y) > float.Epsilon) ? (num2 / MathF.Abs(vector2.Y)) : float.MaxValue);
		return vector + vector2 * MathF.Min(x, y);
	}

	private unsafe void cEvlAL7hWT()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		if (!Throttler.Shared.Throttle(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x3F36086C ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_7a7485368e28474eb6d61987c51729a2)))
		{
			return;
		}
		targetMarkers.Clear();
		Span<GameObjectId> markers = ((MarkingController)MarkingController.Instance()).Markers;
		for (int i = 0; i < markers.Length; i++)
		{
			ulong num = GameObjectId.op_Implicit(markers[i]);
			if ((num != 0L && num != 3758096384u) || 1 == 0)
			{
				targetMarkers[num] = (uint)i;
			}
		}
		fieldMarkers.Clear();
		Span<FieldMarker> span = ((MarkingController)MarkingController.Instance()).FieldMarkers;
		for (int j = 0; j < span.Length; j++)
		{
			FieldMarker val = span[j];
			if (val.Active)
			{
				fieldMarkers[(uint)j] = val.Position;
			}
		}
		foreach (IPlayerCharacter item in ManagerBase<PlayersManager>.Instance().PlayersAround)
		{
			KaymSqi6x8cncKDt1bh kaymSqi6x8cncKDt1bh = KaymSqi6x8cncKDt1bh.MIyikZrfZe(this, item);
			if (!(kaymSqi6x8cncKDt1bh == null))
			{
				playerInfos[((BattleChara)item.ToStruct()).ContentId] = kaymSqi6x8cncKDt1bh;
			}
		}
	}

	private static KnownColor DrGlQTQPSX(BattalionFlags battalion)
	{
		if (!GameMain.IsInPvPInstance())
		{
			return KnownColor.White;
		}
		return battalion switch
		{
			BattalionFlags.FriendOrMaelstrom => KnownColor.Red, 
			BattalionFlags.TwinAdder => KnownColor.Goldenrod, 
			BattalionFlags.ImmortalFlames => KnownColor.DeepSkyBlue, 
			_ => KnownColor.White, 
		};
	}

	private static uint TJWlJ8Rm22(uint index)
	{
		if (!LuminaGetter.TryGetRow<Marker>(index + 1, out Marker item))
		{
			return 0u;
		}
		return (uint)((Marker)(ref item)).Icon;
	}

	private static uint cdilrq1gLR(uint index)
	{
		if (!LuminaGetter.TryGetRow<FieldMarker>(index + 1, out FieldMarker item))
		{
			return 0u;
		}
		return ((FieldMarker)(ref item)).UiIcon;
	}

	public FrontlinePlayerRadar()
	{
		UFJD4QhsjGpSQPTjrhC.Tx2oW8x8le();
		Info = new ModuleInfo
		{
			Title = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x4D9C2CF7 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_3badc14f3c904cf4809aa58db382c8ee)),
			Description = LanguageManager.Get(LtU3X6hjLS3wYDisTg5.hvWhKq6xaR(0x317EC55 ^ _003CModule_003E_007B99f5dfe7_002D88a0_002D47d1_002Dba58_002D46f29e7c6779_007D.m_9e3ce226ea9c47708b8c3accfba96232.m_99910d0c1e594d99b604e0aab1195273)),
			Category = ModuleCategory.Duty
		};
		Permission = new ModulePermission
		{
			NeedAuth = true
		};
		targetMarkers = new Dictionary<ulong, uint>();
		fieldMarkers = new Dictionary<uint, Vector3>();
		playerInfos = new Dictionary<ulong, KaymSqi6x8cncKDt1bh>();
		base._002Ector();
	}
}
