using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Module;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using DailyRoutines.Verification;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using GO3ln7f4pExIcYCK2q3;
using Lumina.Data;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using P67IJbfls2Hy3Xtb45R;
using itAnDoW4mgYVlrmtMor;

namespace cQWxCafyFL1ywAxpwbN;

internal sealed class ewljh3fn17ZPhD3x9jM : ExINkNfv21TTK3yJ2Cx
{
	private sealed class nmcGqRKJ9UFwNwyeLnh
	{
		public required int SourceIndex { get; init; }

		public required ModuleBase Module { get; init; }

		public required ModulePermission Permission { get; init; }

		public required string ModuleName { get; init; }

		public required string Title { get; init; }

		public required string Category { get; init; }

		public required string Author { get; init; }

		public required string Description { get; init; }

		public required string PrecedingModules { get; init; }

		public required string RecommendModules { get; init; }

		public required string ConflictModules { get; init; }

		public required string PreviewImageUrls { get; init; }

		public required string ReportURL { get; init; }

		public required string SupportUrls { get; init; }

		public required string ModuleGUID { get; init; }

		public required string ModuleIdentifier { get; init; }

		public nmcGqRKJ9UFwNwyeLnh()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}
	}

	private const int TableColumnCount = 35;

	private const ImGuiTableFlags TableFlags = (ImGuiTableFlags)50333637;

	private string searchInput;

	private List<nmcGqRKJ9UFwNwyeLnh> rowCache;

	private List<int> filteredRowIndex;

	private ModuleSearcher moduleSearcher;

	private List<ModuleBase>? cachedModules;

	public override string SectionID => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4747 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0);

	public override string Title => GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28284327 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184);

	protected override void DrawContent(VejoYhfM1yiPckJhcdm context)
	{
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		if (ImGuiOm.ButtonIconWithText((FontAwesomeIcon)62830, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F170C5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a)))
		{
			var dictionary = ManagerBase<ModuleManager>.Instance().Modules.ToDictionary((ModuleBase x) => x.ModuleName, (ModuleBase x) => new
			{
				Author = x.Info.Author,
				Category = x.Info.Category,
				CNOnly = x.Permission.CNOnly,
				CNPremium = x.Permission.CNPremium,
				Description = x.Info.Description,
				GlobalPremium = x.Permission.GLPremium,
				NeedAuth = x.Permission.NeedAuth,
				Title = x.Info.Title
			});
			ImGui.SetClipboardText(ImU8String.op_Implicit(dictionary.ToJSON()));
			NotifyHelper notifyHelper = OmenServiceBase<NotifyHelper>.Instance();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D154E39 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
			defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
			defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A3EEA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32));
			defaultInterpolatedStringHandler.AppendFormatted<Language>(PluginConfig.Instance().UILanguage);
			notifyHelper.NotificationSuccess(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		ImGui.Separator();
		icffLKIMd0();
		bxafHuV2BR();
		ImGui.Separator();
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1830585184 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3)), 34, (ImGuiTableFlags)50333637);
		try
		{
			if (!val)
			{
				return;
			}
			ImGui.TableSetupScrollFreeze(2, 1);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7D2C6A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571), 64f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C483A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32), 220f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F1776B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a), 220f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47337FE7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA78467 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff), 440f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4619AD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), 84f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53686BAA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), 84f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A20520 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079), 84f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816AA26 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd), 84f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9069080 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x392212B4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C28B23 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4043 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4BE40 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1ED50 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B2125EC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1ED78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C28B9B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D204B0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1), 96f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9F5E7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x768FD31B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_417cae18312d413c80505172551e7b46), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BA932D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191209 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x41191219 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52), 120f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEE93F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b), 108f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65408FC8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb), 108f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x611669B6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402), 220f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339C8956 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de), 220f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD419B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), 220f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4BFB8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3), 280f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B1201 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), 300f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E461815 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), 300f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD4149 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), 280f);
			arOffGbBJQ(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B12EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), 440f);
			ImGui.TableHeadersRow();
			foreach (int item in filteredRowIndex)
			{
				nmcGqRKJ9UFwNwyeLnh obj = rowCache[item];
				ModuleBase module = obj.Module;
				ModulePermission permission = obj.Permission;
				ImGui.TableNextRow();
				fjEfj1c1SQ((obj.SourceIndex + 1).ToString());
				fjEfj1c1SQ(obj.ModuleName, copyable: true);
				fjEfj1c1SQ(obj.Title, copyable: true);
				fjEfj1c1SQ(obj.Category);
				fjEfj1c1SQ(obj.Description, copyable: true);
				u7RfCEMrpW(module.IsEnabled);
				u7RfCEMrpW(module.IsInitialized);
				u7RfCEMrpW(module.IsDisposed);
				u7RfCEMrpW(module.WithConfig);
				u7RfCEMrpW(module.WithConfigUI);
				u7RfCEMrpW(permission.NeedAuth);
				u7RfCEMrpW(permission.CNOnly);
				u7RfCEMrpW(permission.GLOnly);
				u7RfCEMrpW(permission.TCOnly);
				u7RfCEMrpW(permission.KROnly);
				u7RfCEMrpW(permission.CNPremium);
				u7RfCEMrpW(permission.GLPremium);
				u7RfCEMrpW(permission.TCPremium);
				u7RfCEMrpW(permission.KRPremium);
				u7RfCEMrpW(permission.AllDefaultEnabled);
				u7RfCEMrpW(permission.CNDefaultEnabled);
				u7RfCEMrpW(permission.GLDefaultEnabled);
				u7RfCEMrpW(permission.TCDefaultEnabled);
				u7RfCEMrpW(permission.KRDefaultEnabled);
				u7RfCEMrpW(permission.IsSkip());
				u7RfCEMrpW(permission.IsHide(AuthState.IsAuth, AuthState.IsPremium));
				fjEfj1c1SQ(obj.PrecedingModules, copyable: true);
				fjEfj1c1SQ(obj.RecommendModules, copyable: true);
				fjEfj1c1SQ(obj.ConflictModules, copyable: true);
				fjEfj1c1SQ(obj.PreviewImageUrls, copyable: true);
				fjEfj1c1SQ(obj.ReportURL, copyable: true);
				fjEfj1c1SQ(obj.SupportUrls, copyable: true);
				fjEfj1c1SQ(obj.ModuleGUID, copyable: true);
				fjEfj1c1SQ(obj.ModuleIdentifier, copyable: true);
			}
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void icffLKIMd0()
	{
		List<ModuleBase> modules = ManagerBase<ModuleManager>.Instance().Modules;
		if (cachedModules != modules)
		{
			cachedModules = modules;
			rowCache.Clear();
			filteredRowIndex.Clear();
			for (int i = 0; i < modules.Count; i++)
			{
				ModuleBase moduleBase = modules[i];
				ModuleInfo info = moduleBase.Info;
				ModulePermission permission = moduleBase.Permission;
				string moduleName = moduleBase.ModuleName;
				string title = info.Title;
				string category = info.Category.ToString();
				string author = jYMfOReeNW(info.Author);
				string description = info.Description;
				string precedingModules = AcsfKDcQp3(moduleBase.PrecedingModules);
				string recommendModules = AcsfKDcQp3(moduleBase.PairModules);
				string conflictModules = AcsfKDcQp3(moduleBase.ConflictModules);
				string previewImageUrls = jYMfOReeNW(info.PreviewImageURL);
				string reportURL = info.ReportURL;
				string supportUrls = jYMfOReeNW(info.SupportUrls);
				string moduleGUID = moduleBase.ModuleGUID;
				string moduleIdentifier = moduleBase.ModuleIdentifier;
				rowCache.Add(new nmcGqRKJ9UFwNwyeLnh
				{
					SourceIndex = i,
					Module = moduleBase,
					Permission = permission,
					ModuleName = moduleName,
					Title = title,
					Category = category,
					Author = author,
					Description = description,
					PrecedingModules = precedingModules,
					RecommendModules = recommendModules,
					ConflictModules = conflictModules,
					PreviewImageUrls = previewImageUrls,
					ReportURL = reportURL,
					SupportUrls = supportUrls,
					ModuleGUID = moduleGUID,
					ModuleIdentifier = moduleIdentifier
				});
				filteredRowIndex.Add(i);
			}
			moduleSearcher = TmofVnb2XS(modules);
		}
	}

	private void bxafHuV2BR()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		ImGui.SetNextItemWidth(MathF.Max(240f * Globals.GlobalUIScale, ImGui.GetContentRegionAvail().X - 220f * Globals.GlobalUIScale));
		ImGui.InputTextWithHint(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47337E6D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)), ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5583AD26 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5603c9a2cf17495cb5da1dda759bb08d)), ref searchInput, 256, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
		ImGui.SameLine();
		DisabledDisposable val = ImRaii.Disabled(string.IsNullOrWhiteSpace(searchInput));
		try
		{
			if (ImGui.Button(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A20482 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079)), default(Vector2)))
			{
				searchInput = string.Empty;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		KXNfSFkypQ();
		ImGui.SameLine();
		ImU8String val2 = default(ImU8String);
		((ImU8String)(ref val2))._002Ector(10, 2);
		((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x719D0D23 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5437ef93780b442a895af045b06fabbc));
		((ImU8String)(ref val2)).AppendFormatted<int>(filteredRowIndex.Count);
		((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9066BA2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3));
		((ImU8String)(ref val2)).AppendFormatted<int>(rowCache.Count);
		((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA78655 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
		ImGui.TextDisabled(val2);
	}

	private void KXNfSFkypQ()
	{
		string text = searchInput.Trim();
		filteredRowIndex.Clear();
		if (string.IsNullOrWhiteSpace(text))
		{
			for (int i = 0; i < rowCache.Count; i++)
			{
				filteredRowIndex.Add(i);
			}
			return;
		}
		ModuleBase[] array = moduleSearcher.Search(text);
		if (array.Length == 0)
		{
			return;
		}
		for (int j = 0; j < rowCache.Count; j++)
		{
			if (wNRf9xGscG(array, rowCache[j].Module))
			{
				filteredRowIndex.Add(j);
			}
		}
	}

	private static ModuleSearcher TmofVnb2XS(IEnumerable<ModuleBase> modules)
	{
		return new ModuleSearcher(modules);
	}

	private static bool wNRf9xGscG(IReadOnlyList<ModuleBase> modules, ModuleBase targetModule)
	{
		foreach (ModuleBase module in modules)
		{
			if (module == targetModule)
			{
				return true;
			}
		}
		return false;
	}

	private static void arOffGbBJQ(string label, float width)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableSetupColumn(ImU8String.op_Implicit(label), (ImGuiTableColumnFlags)8, width * Globals.GlobalUIScale, 0u);
	}

	private static void u7RfCEMrpW(bool value)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (value)
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329CB6AC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)));
		}
		else
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA77AC9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff)));
		}
	}

	private static void fjEfj1c1SQ(string? text, bool copyable = false)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TableNextColumn();
		if (string.IsNullOrWhiteSpace(text))
		{
			ImGui.TextDisabled(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C27821 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0)));
			return;
		}
		ImGui.TextUnformatted(ImU8String.op_Implicit(text));
		if (copyable)
		{
			ImGuiOmExtension.ClickToCopyAndNotify(text);
		}
	}

	private static string jYMfOReeNW(IEnumerable<string>? values)
	{
		if (values != null)
		{
			return string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DC4BB8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f), values.Where((string x) => !string.IsNullOrWhiteSpace(x)));
		}
		return string.Empty;
	}

	private static string AcsfKDcQp3(IEnumerable<ModuleBase>? modules)
	{
		if (modules != null)
		{
			return string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DB697F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), modules.Select((ModuleBase x) => x.ModuleName));
		}
		return string.Empty;
	}

	public ewljh3fn17ZPhD3x9jM()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		searchInput = string.Empty;
		rowCache = new List<nmcGqRKJ9UFwNwyeLnh>();
		filteredRowIndex = new List<int>();
		moduleSearcher = new ModuleSearcher(Array.Empty<ModuleBase>());
		base._002Ector();
	}
}
