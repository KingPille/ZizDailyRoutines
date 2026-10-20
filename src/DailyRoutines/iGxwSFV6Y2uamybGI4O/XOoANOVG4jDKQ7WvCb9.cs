using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines;
using DailyRoutines.Common.Info;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Dalamud.Utility.Numerics;
using Lumina.Data;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.ImGuiOm.Markdown;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;
using nXyRYE1uBjK6gmU4ln;
using wUQLZCVUi4RNloOxE8c;

namespace iGxwSFV6Y2uamybGI4O;

internal class XOoANOVG4jDKQ7WvCb9 : Window, IDisposable
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass20_0
	{
		public float cellPaddingX;

		public _003C_003Ec__DisplayClass20_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void GkCO0lO9w2(float contentWidth)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			float num = 180f * Globals.GlobalUIScale;
			float num2 = num + cellPaddingX * 2f;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1C768 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), 2, (ImGuiTableFlags)0, new Vector2(contentWidth, 0f));
			try
			{
				if (TableDisposable.op_Implicit(val))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DC9A3C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C62E6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)), (ImGuiTableColumnFlags)8, num2, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val2 = new ImU8String(2, 2);
					((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61489));
					((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC0260E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
					((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBB8DB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)));
					ImGui.TextUnformatted(val2);
					ImGui.TableNextColumn();
					float num3 = OmenServiceBase<FontManager>.Instance().Config.FontSize;
					ImGui.SetNextItemWidth(num);
					if (ImGui.InputFloat(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x61164208 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)), ref num3, 0.5f, 1f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4119EA33 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)), (ImGuiInputTextFlags)0))
					{
						num3 = Math.Clamp(num3, 8f, 48f);
					}
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						OmenServiceBase<FontManager>.Instance().Config.FontSize = num3;
						OmenServiceBase<FontManager>.Instance().Config.Save();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
			ImGui.Spacing();
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B210E60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a))));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}

		internal void teRO5uuvdU(float contentWidth)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			float num = MathF.Ceiling(ImGui.GetTextLineHeightWithSpacing() * 1.875f) + 2f + cellPaddingX * 2f;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA26D7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)), 2, (ImGuiTableFlags)0, new Vector2(contentWidth, 0f));
			try
			{
				if (TableDisposable.op_Implicit(val))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B306A28 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x226783D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)), (ImGuiTableColumnFlags)8, num, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val2 = new ImU8String(2, 2);
					((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61726));
					((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BF160 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
					((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A885 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09)));
					ImGui.TextUnformatted(val2);
					ImGui.TableNextColumn();
					bool value = PluginConfig.Instance().DefaultEnableModules;
					if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38734B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997), ref value))
					{
						PluginConfig.Instance().DefaultEnableModules = value;
						PluginConfig.Instance().Save();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
			ImGui.Spacing();
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7B071 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff))));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
	}

	private const string CHANGELOG_CN = "## 本体修改\r\n\r\n- 修复了部分仅适用于国服的逻辑仍会在国际服环境下被尝试清理导致扔出空引用异常的问题。\r\n- 修复了所有自制原生界面的标题无法在国际服环境下显示的问题。\r\n- 现在支持点击主界面左侧边栏窗口的版本号文本以再次打开更新日志窗口。\r\n- 更新日志现在会在窗口中被正确渲染为 Markdown。\r\n- 右键菜单解析现在支持“出售品列表”界面。\r\n- 修复了输出的错误消息外观与原生错误消息外观不符的问题。\r\n- 优化了“道具帮助”与“技能帮助”界面相关修改的显示效果。\r\n- 优化了 IPv6 网络环境下难以连接在线验证服务的问题。\r\n\r\n### 关于集中调整通知功能的告知\r\n\r\n（以下变更可能需要历经多个版本才能调整完毕。）\r\n\r\n- 因“文本转语音”功能已独立为单独的 `EdgeTTS.Dalamud` 插件，且该功能表现上侵入感较强。现在仅会在发送“Windows 托盘通知”并检测到该插件启用时一并朗读托盘通知的消息内容。\r\n- “聊天消息”类通知将会划分为两种。\r\n    - 有“DR”前缀：内容明确为插件提供的功能才可能能产生的通知消息。\r\n      示例：内容为“批量制作完成”的通知消息；内容为“市场某一物品价格低于设定值”的通知消息；内容为“潜水艇均已返航”，但是可以交互消息以自动前往收取的通知消息。\r\n    - 无“DR”前缀：内容为游戏本身也可以产生，仅为未外显或明确通知的通知消息。\r\n      示例：内容为“进入了移动速度检测区域”的通知消息；内容为“成功登录了游戏”的通知消息、内容为“遭遇了击退效果”的通知消息。\r\n- 除“聊天信息”类通知外，其他所有类型通知都不包含“DR”前缀。\r\n- “聊天消息”类通知的发送频道将从“调试”调整为“系统信息”。今后可以选择在“消息过滤”窗口中的“通知”分页下取消勾选“系统信息”来过滤相关消息。（并非所有“聊天消息”类通知都会经由“系统消息”频道发送。）\r\n- “Dalamud 通知”类通知将会仅用于操作 ImGui 绘制的界面时产生的反馈中。\r\n- “聊天消息”类通知则将会集中用于操作游戏原生界面（含插件自制的原生界面）、原生系统机制时产生的反馈中。\r\n- “副本机制”类通知仅用于极少数需要特别强调（含事先明确告知的娱乐类）的反馈中。\r\n- “Windows 托盘通知”类通知将会用于长时间自动化或特别紧急乃至需要后台通知的情况中。该通知也仅会在游戏位于后台时或游戏已 15 秒未收到任何按键输入时发送。一般还会有其他类型的通知一并发送相同的内容。\r\n- “浮动提示”类通知适用情况与“聊天消息”类通知大体一致，但具体使用会大量参考同类游戏原生系统机制如何使用并模拟，以更加类似于原生机制。\r\n- “Windows 系统提示音”类通知（含蜂鸣器）仅用于极少数需要特别强调（含事先明确告知的娱乐类）的反馈中。\r\n\r\n## 新增模块\r\n\r\n### 自动继续昔日重现 [系统]\r\n\r\n若有已有中断的存档，则在登录、退出副本后自动继续游玩昔日重现。\r\n\r\n### 无限制宏图标指定 [界面]\r\n\r\n*推荐模块*\r\n\r\n支持在宏内使用 /pdr micon <图标 ID> 指令以将其图标设置为任意图标。\r\n\r\n### “道具分解”界面优化 [界面]\r\n\r\n*推荐模块*\r\n\r\n优化“道具分解”界面，新增分解技能信息显示，支持批量分解道具。“道具帮助”界面内新增详细物品分解信息显示。手动分解道具时自动确认。\r\n\r\n![OptimizedSalvage-Addon](/assets/Changelog/1.0.0.0/OptimizedSalvage.png)\r\n\r\n## 移除模块\r\n\r\n### 自动道具分解 [界面]\r\n\r\n因功能迁移，请使用新模块“‘道具分解’界面优化”。\r\n\r\n### 自动通知特殊玩家出现 [通知]\r\n\r\n因实现方式过于陈旧，难以维护；加诸实际用途不明确，且功能可被其他模块、插件替代，故移除。\r\n\r\n## 模块修改\r\n\r\n### 自动重复点击 [系统]\r\n\r\n- 修复了“手柄模式”下无法正常使用的问题。\r\n- 新增了配置项“等待时间”，长按超过该时间后才会开始重复点击，为 0 时不等待。\r\n\r\n### 自动隐藏不必要弹出窗口 [界面]\r\n\r\n- 修复了部分窗口在模块关闭后会再次出现的问题。\r\n\r\n### 更好的自动跟随 [系统]\r\n\r\n- 新增了配置项“仅在副本中启用”。默认禁用。\r\n\r\n### 自动显示物品商店信息 [界面]\r\n\r\n- “商店交换去向”界面不再显示物品概述信息。\r\n- 修复了“商店交换去向”界面中无法点击“市场”按钮的问题。\r\n- 调整了翻页组件的外观样式与位置。\r\n- 优化了打开逻辑。\r\n- 物品图标现在均可打开右键菜单。因此也移除了“市场”按钮。\r\n- 因“市场”按钮移除，“商店交换去向”界面中原独立成行的花费详情现在与物品名同行，水平右对齐。\r\n- 调整了商店列表的交互逻辑，现在左键点击直接打开地图，右键打开菜单可选择“传送到距离最近的据点”。\r\n- 现在商店列表显示将会根据区域进行排序。\r\n- 修复了“道具帮助”界面显示的商店信息与打开界面内显示的商店信息不一致的问题。\r\n- 移除了全部的前置模块。\r\n- 修复了未在高品质道具的“道具保住”界面内显示商店信息的问题。\r\n- 现在会始终清除“道具帮助”界面内原始的“商店贩售”信息，以精简界面与避免重复。\r\n\r\n### 快捷传送面板 [界面]\r\n\r\n- 优化了任务物体的判断逻辑。\r\n- 修复了传送至任务物体时可能会被修正到更高位置的问题。\r\n- 移除了 vnavmesh 寻路逻辑。\r\n- 传送到确定的目标对象时不再会与目标重叠。\r\n- 交互任务对象时会自动选中。\r\n- 现在若启用了 vnavmesh，则所有可传送点的位置都会经由其修正。\r\n- 新增“游戏原生交互优化”模块为可搭配模块。若启用，则会调整可交互距离的判断。\r\n\r\n### 快捷聊天面板 [界面]\r\n\r\n- 修复了滚动条未自动隐藏的问题。\r\n\r\n### 快捷老主顾任务情报界面 [界面]\r\n\r\n- 调整了右键菜单实现方式。根据配置，现在菜单项可能会显示“DR”前缀。\r\n\r\n### 自动刷新市场搜索结果 [系统]\r\n\r\n- 现在在登出时会自动清空市场数据，以修复跨大区重新登录后可能出现的无法获取市场数据问题。\r\n- 修复了在免费试玩状态下仍然会反复重试请求市场数据的问题。\r\n\r\n### 更好的市场布告板 [界面]\r\n\r\n- 现在在免费试玩状态下不再请求本地市场数据，而是始终展示来自 Universalis 的数据。\r\n- 修复了手动切换区域后未自动切换服务器的问题。\r\n- 修复了“道具帮助”界面中显示的“日销量”数字未格式化的问题。\r\n\r\n### 快捷跨界传送 [界面]\r\n\r\n- （仅国服）调整了界面标题。\r\n\r\n### 制作笔记界面优化 [生产采集]\r\n\r\n- 修复了“配方步骤”界面内右键菜单无法在“制作笔记”界面关闭状态下打开的问题。\r\n\r\n### 自动消除钓场警惕 [生产采集]\r\n\r\n- “额外文本指令”现在会被包装成宏指令的形式执行。\r\n- 修正了“额外文本指令”中错误的帮助信息。\r\n- 调整了描述。\r\n- 调整了通知消息文本与外观样式。\r\n- 配置界面新增“先决条件”，指示了运行所需要完成的任务。\r\n\r\n### 自动雇员作业 [界面]\r\n\r\n- 调整了“出售时自动修改价格”配置项显示文本为“自动指定出售价格”，并添加了相关提示信息。\r\n- 修复了“单次上架数量”配置未生效的问题。\r\n- “雇员列表”界面旁的悬浮窗内将无法配置“自动指定出售价格”。\r\n- 新增配置项“自动确认上架”。上架商品时，将会自动点击出现的“价格调整”界面内的“确定”按钮。\r\n- “出售品列表”界面旁的悬浮窗内右上角新增“设置”按钮，点击可打开配置界面。原“清除价格缓存”文本按钮变为右上角的图标按钮。\r\n- 因右键菜单解析已支持“出售品列表”界面，现在改价相关自动操作完成后或“调整价格”界面关闭时将会自动关闭“更好的市场布告板”模块界面。\r\n- 修复了部分资源未在界面关闭时被清理的问题。\r\n\r\n### 自动跳过对话 [系统]\r\n\r\n- 完全重构了逻辑，使用了更加底层的实现方式，覆盖了更多场景，且更不容易在版本更新时失效。\r\n- 调整了描述。\r\n\r\n### 自动跳过过场动画 [系统]\r\n\r\n- 优化了兜底的跳过逻辑，采用了更加底层的实现方式。\r\n- 修复了自动点选观察型剧情从第二次进入相关状态时无法自动完成的问题。\r\n- 优化了配置界面绘制逻辑。\r\n\r\n### 自动退出副本 [副本]\r\n\r\n- 新增指令 `/pdr autoleaveduty` 用以手动指定当前副本任务完成时的自动退出行为。手动指定的行为的优先级高于配置。\r\n    - `/pdr autoleaveduty None`：清除手动指定的自动退出副本行为。\r\n    - `/pdr autoleaveduty InstantLeave`：完成副本任务时立刻自动退出。\r\n    - `/pdr autoleaveduty NoLeave`：完成副本任务时不自动退出。\r\n- 调整了描述文本。\r\n\r\n### 即刻登出 / 退出 [系统]\r\n\r\n- 登出时会立刻先行在本地登出，避免服务器回包过慢导致流程卡死。\r\n\r\n### 自动临危受命 [脚本]\r\n\r\n- 新增配置项“忽略远超等级的危命任务”。若危命任务等级超出当前玩家等级 8 级，则不会前往，避免无法完成或完成效率过低。默认启用。\r\n\r\n### 自动通知登录 [通知]\r\n\r\n- 移除了所有配置项。\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知战斗开始倒计时开始 [战斗]\r\n\r\n- 将分类由“通知”调整为“战斗”。\r\n- 移除了所有配置项。\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知额外奖励危命任务出现 [战斗]\r\n\r\n- 移除了除“打开地图”外的所有配置项。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 调整了名称与描述文本。\r\n\r\n### 自动通知准备确认 [战斗]\r\n\r\n- 优化了性能表现。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 调整了描述文本。\r\n\r\n### 自动通知云冠群岛特殊天气 [生产采集]\r\n\r\n- 优化了性能表现。\r\n- 将分类由“通知”调整为“生产采集”。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 调整了描述文本。\r\n\r\n### 自动通知诛灭战限时奖励提升 [副本]\r\n\r\n- 修复了因 API 端点变更导致的功能部分失效的问题。\r\n- 优化了性能表现。\r\n- 移除了除订阅大区外的所有配置项。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 调整了描述文本。\r\n\r\n### 自动屏蔽系统通告 [系统]\r\n\r\n- 完全重构了实现逻辑。现在仅屏蔽来自服务器下发的通告信息，不再会过滤掉同通道其他消息。\r\n- 调整了名称与描述文本。\r\n\r\n### 自动通知过场剧情结束 [副本]\r\n\r\n- 完全重构了实现逻辑，采取了更加稳定、准确的实现方式。\r\n- 移除了所有配置项。\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知任务出发准备 [副本]\r\n\r\n- 完全重构了实现逻辑，采取了更加稳定、准确的实现方式，不再会因界面开关而反复通知。\r\n- 调整了标题与描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知副本信息 [副本]\r\n\r\n- 调整了描述文本。\r\n- 移除了所有配置项。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 修正了部分副本显示的品级同步为错误数据的问题。\r\n\r\n### 自动通知副本开始 [副本]\r\n\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知“随机任务：指导者”成就进度 [副本]\r\n\r\n- 简化了实现逻辑。\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n\r\n### 自动通知新消息 [通知]\r\n\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 筛选掉了大量过于杂乱的消息类型。\r\n- 选择消息类型时，会显示对应的游戏文本而非枚举名。\r\n- 移除了配置项“仅当游戏位于后台时提醒”。\r\n\r\n### 自动通知理符限额刷新 [通知]\r\n\r\n- 调整了描述文本。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 移除了配置项“同时发送至聊天栏”。\r\n\r\n### 自动醒梦 [技能]\r\n\r\n- 调整了描述文本。\r\n- 移除了配置项“发送聊天信息”和“发送通知”。\r\n- 调整了通知消息外观样式与文本内容。\r\n- 调整了配置界面。";

	private const string CHANGELOG_EN = "## Base Modifications\r\n\r\n- Fixed an issue where logic intended only for the Chinese servers was still being cleaned up in the global client environment, causing null reference exceptions.\r\n- Fixed an issue where the titles of all custom native interfaces could not be displayed in the global client environment.\r\n- Clicking the version number text in the left sidebar window of the main interface now reopens the changelog window.\r\n- The changelog is now correctly rendered as Markdown inside the window.\r\n- Context menu parsing now supports the \"Markets\" interface.\r\n- Fixed an issue where the appearance of output error messages did not match that of native error messages.\r\n- Improved the display of modifications related to the \"Item Help\" and \"Action Help\" interfaces.\r\n- Improved the issue where connecting to the online verification service was difficult in IPv6 network environments.\r\n\r\n### Notice Regarding the Centralized Adjustment of Notification Features\r\n\r\n(The following changes may take several versions to be fully adjusted.)\r\n\r\n- As the \"text-to-speech\" feature has been separated into the standalone `EdgeTTS.Dalamud` plugin, and that feature feels quite intrusive in practice, the message content of a tray notification will now only be read aloud when a \"Windows tray notification\" is sent and that plugin is detected as enabled.\r\n- \"Chat message\" type notifications will be divided into two kinds.\r\n    - With the \"DR\" prefix: notifications whose content could clearly only be produced by features provided by the plugin.\r\n      Examples: a notification whose content is \"batch crafting complete\"; a notification whose content is \"an item's market price fell below the configured value\"; a notification whose content is \"all submersibles have returned\" but which can be interacted with to travel and collect automatically.\r\n    - Without the \"DR\" prefix: notifications whose content the game itself can also produce, and which merely are not displayed or not explicitly notified.\r\n      Examples: a notification whose content is \"entered a movement speed detection area\"; a notification whose content is \"successfully logged in to the game\"; a notification whose content is \"knockback effect incurred\".\r\n- Except for \"chat message\" type notifications, no other type of notification contains the \"DR\" prefix.\r\n- The sending channel for \"chat message\" type notifications will change from \"Debug\" to \"System Messages\". From now on, you can filter these messages by unchecking \"System Messages\" under the \"Notification\" tab in the \"Log Filters\" window. (Not all \"chat message\" type notifications are sent through the \"System Messages\" channel.)\r\n- \"Dalamud notification\" type notifications will be used only for feedback produced when operating interfaces drawn with ImGui.\r\n- \"Chat message\" type notifications will instead be concentrated on feedback produced when operating the game's native interfaces (including native-style interfaces made by the plugin) and native system mechanics.\r\n- \"Duty mechanic\" type notifications are used only for the very few pieces of feedback that require special emphasis (including entertainment-type ones announced in advance).\r\n- \"Windows tray notification\" type notifications will be used for long-running automation or particularly urgent cases, including those requiring background notification. Such notifications are also sent only when the game is in the background or when the game has not received any key input for 15 seconds. Generally, other types of notifications also send the same content.\r\n- The applicable situations for \"floating toast\" type notifications are broadly the same as those for \"chat message\" type notifications, but their specific use heavily references and mimics how similar native game system mechanics are used, so as to resemble native mechanics more closely.\r\n- \"Windows system sound\" type notifications (including buzzer) are used only for the very few pieces of feedback that require special emphasis (including entertainment-type ones announced in advance).\r\n\r\n## New Modules\r\n\r\n### Auto Resume New Game+ [System]\r\n\r\nAutomatically continue progress of New Game+ after logging in or leaving a duty if there is suspended progress available.\r\n\r\n### Unrestricted Macro Icon [Addon]\r\n\r\n*Recommended*\r\n\r\nAllow to assign any icon to a macro using the command /pdr micon <icon ID>.\r\n\r\n### Better Desynthesis [Addon]\r\n\r\n*Recommended*\r\n\r\nProvide a better Desynthesis interface, including desynthesis information, desynthesis skill information, batch desynthesis and auto confirmation while desynthesizing manually.\r\n\r\n![OptimizedSalvage-Addon](/assets/Changelog/1.0.0.0/OptimizedSalvage.png)\r\n\r\n## Removed Modules\r\n\r\n### Auto Item Desynthesis [Addon]\r\n\r\nDue to the feature migration, please use the new module \"Better Desynthesis\".\r\n\r\n### Auto Notify Special Players Appearance [Notification]\r\n\r\nBecause the implementation is too outdated and difficult to maintain, its practical use is unclear, and the feature can be replaced by other modules and plugins, it has been removed.\r\n\r\n## Module Changes\r\n\r\n### Auto Spam Key [System]\r\n\r\n- Fixed an issue where it did not work properly in \"gamepad mode\".\r\n- Added the \"Activation Delay\" configuration option; repeated clicking only starts after the key is held for longer than this duration, and no waiting occurs when it is 0.\r\n\r\n### Auto Block Unnecessary Pop-ups [Addon]\r\n\r\n- Fixed an issue where some windows would reappear after the module was disabled.\r\n\r\n### Better Following [System]\r\n\r\n- Added the \"Only In Duty\" configuration option. Disabled by default.\r\n\r\n### Auto Display Item Info At NPCs [Addon]\r\n\r\n- The \"Usage\" interface no longer displays item summary information.\r\n- Fixed an issue where the \"Market\" button could not be clicked in the \"Usage\" interface.\r\n- Adjusted the appearance and position of the paging component.\r\n- Improved the opening logic.\r\n- Item icons can now all open the context menu. The \"Market\" button has therefore been removed.\r\n- Because the \"Market\" button was removed, the cost details that were previously on their own line in the \"Usage\" interface now share the item name's line and are right-aligned horizontally.\r\n- Adjusted the interaction logic of the shop list; left-clicking now opens the map directly, while right-clicking opens a menu where \"Teleport to the nearest settlement\" can be selected.\r\n- The shop list is now sorted by region.\r\n- Fixed an issue where the shop information displayed in the \"Item Help\" interface did not match the shop information displayed in the opened interface.\r\n- Removed all prerequisite modules.\r\n- Fixed an issue where shop information was not displayed in the \"Keep Item\" interface for high-quality items.\r\n- The original \"Availability At NPCs\" information in the \"Item Help\" interface is now always cleared, to streamline the interface and avoid duplication.\r\n\r\n### Fast Teleportation Panel [Addon]\r\n\r\n- Improved the detection logic for quest objects.\r\n- Fixed an issue where teleporting to a quest object could be corrected to a higher position.\r\n- Removed the vnavmesh pathfinding logic.\r\n- Teleporting to a fixed target object no longer overlaps with the target.\r\n- Quest objects are now automatically selected when interacted with.\r\n- If vnavmesh is enabled, the positions of all teleportable points are now corrected through it.\r\n- Added the \"Game Interaction Optimization\" module as an optional companion module. If enabled, it adjusts the judgment of interactable distance.\r\n\r\n### Better Chat Panel [Addon]\r\n\r\n- Fixed an issue where the scrollbar did not hide automatically.\r\n\r\n### Better Custom Delivery Information [Addon]\r\n\r\n- Adjusted the implementation of the context menu. Depending on the configuration, menu items may now display the \"DR\" prefix.\r\n\r\n### Auto Refresh Market Search Results [System]\r\n\r\n- Market data is now automatically cleared on logout, fixing an issue where market data could not be obtained after logging back in across data centers.\r\n- Fixed an issue where market data requests were still retried repeatedly while in the free trial state.\r\n\r\n### Better Market Board [Addon]\r\n\r\n- While in the free trial state, local market data is no longer requested; data from Universalis is always displayed instead.\r\n- Fixed an issue where the server was not switched automatically after manually changing regions.\r\n- Fixed an issue where the \"Daily Sales\" figure displayed in the \"Item Help\" interface was not formatted.\r\n\r\n### Better World Visit [Addon]\r\n\r\n- (Chinese servers only) Adjusted the interface title.\r\n\r\n### Better Crafting Log [Craft & Gather]\r\n\r\n- Fixed an issue where the context menu in the \"Solution Steps\" interface could not be opened while the \"Crafting Log\" interface was closed.\r\n\r\n### Auto Reset Fishing Location Wariness [Craft & Gather]\r\n\r\n- \"Extra Command\" is now executed in the form of a wrapped macro command.\r\n- Fixed incorrect help information in \"Extra Command\".\r\n- Adjusted the description.\r\n- Adjusted the text and appearance of notification messages.\r\n- Added a \"Pre-condition\" entry to the configuration interface, indicating the quests that must be completed for it to run.\r\n\r\n### Auto Retainer Routines [Addon]\r\n\r\n- Changed the display text of the \"Auto-adjust price when listing\" configuration option to \"Auto Price\" and added related hint information.\r\n- Fixed an issue where the \"Quantity Per Listing\" configuration did not take effect.\r\n- \"Auto Price\" can no longer be configured in the floating window beside the \"Retainer List\" interface.\r\n- Added the \"Auto Confirm While Listing\" configuration option. When listing an item, the \"Confirm\" button in the \"Adjust Price\" interface that appears will be clicked automatically.\r\n- A \"Settings\" button was added to the top right of the floating window beside the \"Markets\" interface; clicking it opens the configuration interface. The former \"Clear Price Cache\" text button has become an icon button in the top right.\r\n- Because context menu parsing now supports the \"Markets\" interface, the \"Better Market Board\" module interface will now be closed automatically after price-change automation completes or when the \"Adjust Price\" interface is closed.\r\n- Fixed an issue where some resources were not cleaned up when the interface was closed.\r\n\r\n### Auto Skip Dialogs [System]\r\n\r\n- The logic was completely rebuilt, using a lower-level implementation that covers more scenarios and is less likely to break with version updates.\r\n- Adjusted the description.\r\n\r\n### Auto Skip Cutscenes [System]\r\n\r\n- Improved the fallback skip logic, adopting a lower-level implementation.\r\n- Fixed an issue where automatically choosing through observational cutscenes failed to complete automatically from the second time the relevant state was entered.\r\n- Improved the drawing logic of the configuration interface.\r\n\r\n### Auto Leave Duty [Duty]\r\n\r\n- Added the command `/pdr autoleaveduty` to manually specify the automatic leave behavior when the current duty's tasks are complete. Manually specified behavior takes priority over the configuration.\r\n    - `/pdr autoleaveduty None`: clears the manually specified auto-leave-duty behavior.\r\n    - `/pdr autoleaveduty InstantLeave`: leaves automatically immediately when the duty's tasks are complete.\r\n    - `/pdr autoleaveduty NoLeave`: does not leave automatically when the duty's tasks are complete.\r\n- Adjusted the description text.\r\n\r\n### Instant Logout Or Exit [System]\r\n\r\n- On logout, it now logs out locally first, avoiding the process getting stuck when the server response is slow.\r\n\r\n### Auto FATE [Script]\r\n\r\n- Added the \"Ignore FATEs Far Above Character Level\" configuration option. If a FATE's level exceeds the player's current level by 8, it will not be traveled to, avoiding failures or very low completion efficiency. Enabled by default.\r\n\r\n### Auto Login Notification [Notification]\r\n\r\n- Removed all configuration options.\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### Auto Countdown Notification [Combat]\r\n\r\n- Changed the category from \"Notification\" to \"Combat\".\r\n- Removed all configuration options.\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### *Auto Notify Bonus Reward FATE Appearance [Combat]\r\n\r\n- Removed all configuration options except \"Open Map\".\r\n- Adjusted the appearance and text content of notification messages.\r\n- Adjusted the name and description text.\r\n\r\n### Auto Ready Check Notification [Combat]\r\n\r\n- Improved performance.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Adjusted the description text.\r\n\r\n### Auto Diadem Weather Notification [Craft & Gather]\r\n\r\n- Improved performance.\r\n- Changed the category from \"Notification\" to \"Craft & Gather\".\r\n- Adjusted the appearance and text content of notification messages.\r\n- Adjusted the description text.\r\n\r\n### Auto Chaotic Raid Bonus Notification [Duty]\r\n\r\n- Fixed an issue where part of the functionality stopped working due to an API endpoint change.\r\n- Improved performance.\r\n- Removed all configuration options except the subscribed data center.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Adjusted the description text.\r\n\r\n### *Auto Block System Announcements [System]\r\n\r\n- The implementation logic was completely rebuilt. It now only blocks announcement information issued by the server, and no longer filters out other messages in the same channel.\r\n- Adjusted the name and description text.\r\n\r\n### Auto Cutscene End Notification [Duty]\r\n\r\n- The implementation logic was completely rebuilt, adopting a more stable and accurate implementation.\r\n- Removed all configuration options.\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### *Auto Notify Duty Departure Ready [Duty]\r\n\r\n- The implementation logic was completely rebuilt, adopting a more stable and accurate implementation; it no longer sends repeated notifications due to interface toggling.\r\n- Adjusted the title and description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### Auto Duty Information Notification [Duty]\r\n\r\n- Adjusted the description text.\r\n- Removed all configuration options.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Fixed an issue where the item level sync displayed for some duties showed incorrect data.\r\n\r\n### Auto Notify While Duty Starts [Duty]\r\n\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### Auto Mentor Roulette Progress Notification [Duty]\r\n\r\n- Simplified the implementation logic.\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n\r\n### Auto Unread Message Notification [Notification]\r\n\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Filtered out a large number of overly cluttered message types.\r\n- When selecting a message type, the corresponding game text is now displayed instead of the enum name.\r\n- Removed the \"Only Notify When Background\" configuration option.\r\n\r\n### Auto Leve Allowance Renewal Notification [Notification]\r\n\r\n- Adjusted the description text.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Removed the \"Also send to the chat log\" configuration option.\r\n\r\n### Auto Lucid Dreaming [Action]\r\n\r\n- Adjusted the description text.\r\n- Removed the \"Send Chat Message\" and \"Send Notification\" configuration options.\r\n- Adjusted the appearance and text content of notification messages.\r\n- Adjusted the configuration interface.";

	private const string CHANGELOG_JA = "## 本体の変更\r\n\r\n- 中国版にのみ適用される一部のロジックが、国際版環境でもクリーンアップを試行され、null 参照例外が発生する問題を修正しました。\r\n- すべての独自ネイティブインターフェースのタイトルが、国際版環境で表示されない問題を修正しました。\r\n- メイン画面左側のサイドバーウィンドウにあるバージョン番号テキストをクリックすることで、更新履歴ウィンドウを再度開けるようになりました。\r\n- 更新履歴がウィンドウ内で正しく Markdown としてレンダリングされるようになりました。\r\n- コンテキストメニューの解析が「ITEM LIST FOR MARKET」画面に対応しました。\r\n- 出力されるエラーメッセージの外観が、ネイティブのエラーメッセージの外観と一致しない問題を修正しました。\r\n- 「アイテム詳細」と「アクション詳細」画面に関連する改変の表示効果を最適化しました。\r\n- IPv6 ネットワーク環境でオンライン認証サービスに接続しにくい問題を改善しました。\r\n\r\n### 通知機能の一括調整に関するお知らせ\r\n\r\n（以下の変更は、調整が完了するまでに複数のバージョンを要する場合があります）\r\n\r\n- 「テキスト読み上げ」機能が独立した `EdgeTTS.Dalamud` プラグインに移行し、かつこの機能は動作上の侵入感が強いため、現在は「Windows トレイ通知」を送信し、当該プラグインの有効化を検出した場合にのみ、トレイ通知のメッセージ内容を併せて読み上げます。\r\n- 「チャットメッセージ」系の通知は、二種類に分割されます。\r\n    - 「DR」プレフィックスあり：内容が明確にプラグイン提供機能によってのみ発生し得る通知メッセージ。\r\n      例：内容が「まとめて製作完了」の通知メッセージ。内容が「マーケットのとあるアイテムの価格が設定値を下回った」通知メッセージ。内容が「潜水艦がすべて帰還した」でありながら、メッセージを操作すると自動で回収に向かえる通知メッセージ。\r\n    - 「DR」プレフィックスなし：内容がゲーム本体でも発生し得るもので、単に表示されていない、あるいは明確に通知されていない通知メッセージ。\r\n      例：内容が「移動速度検出エリアに進入した」通知メッセージ。内容が「ゲームに正常にログインした」通知メッセージ、内容が「ノックバック効果を受けた」通知メッセージ。\r\n- 「チャットメッセージ」系の通知を除き、その他のすべての種類の通知には「DR」プレフィックスが付きません。\r\n- 「チャットメッセージ」系通知の送信チャンネルは「デバッグ」から「システムメッセージ」に変更されます。今後は「ログフィルター」ウィンドウの「通知」タブで「システムメッセージ」のチェックを外すことで、該当メッセージをフィルタリングできます。（すべての「チャットメッセージ」系通知が「システムメッセージ」チャンネル経由で送信されるわけではありません。）\r\n- 「Dalamud 通知」系の通知は、ImGui で描画されたインターフェースを操作した際に発生するフィードバックにのみ使用されます。\r\n- 「チャットメッセージ」系通知は、ゲームのネイティブインターフェース（プラグインが独自に作成したネイティブインターフェースを含む）やネイティブシステムの仕組みを操作した際に発生するフィードバックに集約されます。\r\n- 「コンテンツギミック」系の通知は、特別な強調が必要なごく少数のフィードバック（事前に明示されたお遊び要素を含む）にのみ使用されます。\r\n- 「Windows トレイ通知」系の通知は、長時間の自動化や、特に緊急でバックグラウンド通知が必要な状況で使用されます。当該通知も、ゲームがバックグラウンドにある場合、またはゲームが 15 秒間いかなるキー入力も受け取っていない場合にのみ送信されます。通常は他の種類の通知でも同じ内容が併せて送信されます。\r\n- 「フローティングトースト」系の通知の適用状況は「チャットメッセージ」系の通知と概ね一致しますが、具体的な使用は同種のゲームネイティブシステムの仕組みがどう使われているかを大量に参照し模倣することで、よりネイティブな仕組みに近づけます。\r\n- 「Windows システムサウンド」系の通知（ブザーを含む）は、特別な強調が必要なごく少数のフィードバック（事前に明示されたお遊び要素を含む）にのみ使用されます。\r\n\r\n## 追加モジュール\r\n\r\n### 「つよくてニューゲーム」を自動再開 [システム]\r\n\r\n中断セーブデータが存在する場合、ログイン時およびコンテンツ退出後に「つよくてニューゲーム」を自動的に再開します。\r\n\r\n### マクロアイコンの自由指定 [インターフェイス]\r\n\r\n*おすすめモジュール*\r\n\r\nマクロ内で /pdr micon <アイコンID> コマンドを使用し、アイコンを任意のアイコンに変更することができます。\r\n\r\n### 「アイテム分解」画面の改善 [インターフェイス]\r\n\r\n*おすすめモジュール*\r\n\r\n「アイテム分解」画面を改善し、分解スキルの表示機能やまとめて分解する機能を追加しました。「アイテム詳細」画面にも詳しい分解情報を表示するように変更し、手動で分解する際の確認を自動で行うようにしました。\r\n\r\n![OptimizedSalvage-Addon](/assets/Changelog/1.0.0.0/OptimizedSalvage.png)\r\n\r\n## 削除されたモジュール\r\n\r\n### 自動アイテム分解 [インターフェイス]\r\n\r\n機能移行のため、新しいモジュール「「アイテム分解」画面の改善」をご利用ください。\r\n\r\n### *特殊なプレイヤーの出現を自動通知 [通知]\r\n\r\n実装方法があまりに旧式で保守が難しく、加えて実際の用途も不明確であり、他のモジュールやプラグインで代替可能なため削除しました。\r\n\r\n## モジュールの変更\r\n\r\n### ターボクリック機能 [システム]\r\n\r\n- 「ゲームパッドモード」で正常に使用できない問題を修正しました。\r\n- 設定項目「待機時間」を追加しました。この時間以上長押しすると連続クリックが開始され、0 の場合は待機しません。\r\n\r\n### 不要なポップアップウィンドウを自動的に隠す [インターフェイス]\r\n\r\n- モジュールを無効化した後に一部のウィンドウが再び表示される問題を修正しました。\r\n\r\n### 自動追尾の改善 [システム]\r\n\r\n- 設定項目「コンテンツ内でのみ有効化」を追加しました。デフォルトでは無効です。\r\n\r\n### 商品の商店情報を表示する [インターフェイス]\r\n\r\n- 「ショップ交換先一覧」画面にアイテム概要情報を表示しなくなりました。\r\n- 「ショップ交換先一覧」画面で「マーケット」ボタンをクリックできない問題を修正しました。\r\n- ページ送りコンポーネントの外観と位置を調整しました。\r\n- 開く処理のロジックを最適化しました。\r\n- アイテムアイコンはすべてコンテキストメニューを開けるようになりました。そのため「マーケット」ボタンも削除しました。\r\n- 「マーケット」ボタンの削除に伴い、「ショップ交換先一覧」画面で元々独立した行だった支払い詳細が、アイテム名と同じ行に水平右揃えで表示されるようになりました。\r\n- ショップリストの操作ロジックを調整し、左クリックでマップを直接開き、右クリックでメニューを開いて「最寄りの拠点へテレポ」を選択できるようになりました。\r\n- ショップリストの表示がエリアごとに並べ替えられるようになりました。\r\n- 「アイテム詳細」画面に表示されるショップ情報と、開いた画面内に表示されるショップ情報が一致しない問題を修正しました。\r\n- すべての前提モジュールを削除しました。\r\n- 高品質アイテムの「アイテムを残す」画面にショップ情報が表示されない問題を修正しました。\r\n- 「アイテム詳細」画面内の元の「NPC販売」情報を常に消去し、画面を簡素化して重複を避けるようにしました。\r\n\r\n### クイック転送パネル [インターフェイス]\r\n\r\n- クエストオブジェクトの判定ロジックを最適化しました。\r\n- クエストオブジェクトへ転送する際、より高い位置に補正されることがある問題を修正しました。\r\n- vnavmesh の経路探索ロジックを削除しました。\r\n- 確定した対象オブジェクトへ転送する際、対象と重ならなくなりました。\r\n- クエストオブジェクトを操作する際に自動で選択されます。\r\n- vnavmesh が有効な場合、すべての転送可能地点の位置がそれを経由して補正されます。\r\n- 「インタラクションロジックの最適化」モジュールを組み合わせ可能モジュールとして追加しました。有効にすると、操作可能距離の判定が調整されます。\r\n\r\n### クイックチャットパネル [インターフェイス]\r\n\r\n- スクロールバーが自動的に非表示にならない問題を修正しました。\r\n\r\n### クイックお得意様情報画面 [インターフェイス]\r\n\r\n- コンテキストメニューの実装方法を調整しました。設定に応じて、メニュー項目に「DR」プレフィックスが表示される場合があります。\r\n\r\n### マケボ検索結果自動更新 [システム]\r\n\r\n- ログアウト時にマーケットデータを自動的にクリアするようにし、データセンターをまたいで再ログインした際にマーケットデータを取得できなくなることがある問題を修正しました。\r\n- フリートライアル状態でもマーケットデータのリクエストを繰り返し再試行していた問題を修正しました。\r\n\r\n### どこでもマーケットボード [インターフェイス]\r\n\r\n- フリートライアル状態ではローカルマーケットデータを要求せず、常に Universalis からのデータを表示するようになりました。\r\n- 手動でエリアを切り替えた後、サーバーが自動で切り替わらない問題を修正しました。\r\n- 「アイテム詳細」画面に表示される「日当り販売数」の数値がフォーマットされていない問題を修正しました。\r\n\r\n### クイック・ワールド間テレポ [インターフェイス]\r\n\r\n- （中国版のみ）インターフェイスタイトルを調整しました。\r\n\r\n### 製作手帳UIの最適化 [製作・採集]\r\n\r\n- 「製作手順」画面内のコンテキストメニューが、「製作手帳」画面が閉じている状態で開けない問題を修正しました。\r\n\r\n### 自動的に釣り場の警戒を解除する [製作・採集]\r\n\r\n- 「追加テキストコマンド」がマクロコマンドの形式でラップされて実行されるようになりました。\r\n- 「追加テキストコマンド」内の誤ったヘルプ情報を修正しました。\r\n- 説明を調整しました。\r\n- 通知メッセージのテキストと外観スタイルを調整しました。\r\n- 設定画面に「前提条件」を追加し、動作に必要なクエストを示すようにしました。\r\n\r\n### リテイナーベンチャー自動化 [インターフェイス]\r\n\r\n- 設定項目「出品時に価格を自動変更する」の表示テキストを「出品価格自動設定」に変更し、関連するヒント情報を追加しました。\r\n- 設定項目「一度の出品数」が反映されない問題を修正しました。\r\n- 「リテイナーリスト」画面横のフローティングウィンドウ内では「出品価格自動設定」を設定できなくなりました。\r\n- 設定項目「出品の自動確定」を追加しました。アイテムを出品する際、表示される「価格の変更」画面内の「確定」ボタンを自動でクリックします。\r\n- 「ITEM LIST FOR MARKET」画面横のフローティングウィンドウ右上に「設定」ボタンを追加し、クリックで設定画面を開けるようにしました。元の「価格キャッシュをクリア」テキストボタンは右上のアイコンボタンになりました。\r\n- コンテキストメニューの解析が「ITEM LIST FOR MARKET」画面に対応したため、価格変更関連の自動操作完了後、または「価格の変更」画面が閉じた際に「どこでもマーケットボード」モジュールの画面が自動で閉じられるようになりました。\r\n- 画面を閉じた際に一部のリソースがクリーンアップされない問題を修正しました。\r\n\r\n### NPC会話自動スキップ [システム]\r\n\r\n- ロジックを全面的に再構築し、より低レイヤーの実装方式を採用してカバーするシナリオを増やし、バージョン更新時に機能しなくなる可能性を低減しました。\r\n- 説明を調整しました。\r\n\r\n### カットシーンスキップ [システム]\r\n\r\n- フォールバックのスキップロジックを最適化し、より低レイヤーの実装方式を採用しました。\r\n- 観察型のカットシーンを自動選択する際、該当状態に二度目に入った時点から自動完了できなくなる問題を修正しました。\r\n- 設定画面の描画ロジックを最適化しました。\r\n\r\n### コンテンツから自動退出する [コンテンツ]\r\n\r\n- 現在のコンテンツの任務完了時の自動退出挙動を手動で指定する `/pdr autoleaveduty` コマンドを追加しました。手動指定の挙動は設定より優先されます。\r\n    - `/pdr autoleaveduty None`：手動指定した自動退出挙動を解除します。\r\n    - `/pdr autoleaveduty InstantLeave`：コンテンツの任務完了時に即座に自動退出します。\r\n    - `/pdr autoleaveduty NoLeave`：コンテンツの任務完了時に自動退出しません。\r\n- 説明テキストを調整しました。\r\n\r\n### 即ログアウト・ゲーム終了 [システム]\r\n\r\n- ログアウト時にまずローカルでログアウトを行うようにし、サーバーの応答が遅いことで処理が停止するのを避けます。\r\n\r\n### 自動F.A.T.E. [スクリプト]\r\n\r\n- 設定項目「推奨レベルを大幅に超える F.A.T.E. を無視する」を追加しました。F.A.T.E. のレベルが現在のプレイヤーレベルを 8 上回る場合、クリアできない、あるいは完了効率が著しく低くなるのを避けるため、向かわなくなります。デフォルトで有効です。\r\n\r\n### ログイン時の通知機能 [通知]\r\n\r\n- すべての設定項目を削除しました。\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### 戦闘開始カウントダウンの開始通知 [戦闘]\r\n\r\n- カテゴリを「通知」から「戦闘」に変更しました。\r\n- すべての設定項目を削除しました。\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### 経験値ボーナス付き F.A.T.E. の発生を通知する [戦闘]\r\n\r\n- 「マップを開く」を除くすべての設定項目を削除しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 名称と説明テキストを調整しました。\r\n\r\n### レディチェック通知 [戦闘]\r\n\r\n- パフォーマンスを最適化しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 説明テキストを調整しました。\r\n\r\n### ディアデム諸島の特殊天候を自動通知 [製作・採集]\r\n\r\n- パフォーマンスを最適化しました。\r\n- カテゴリを「通知」から「製作・採集」に変更しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 説明テキストを調整しました。\r\n\r\n### 滅アライアンスレイドの報酬アップ通知 [コンテンツ]\r\n\r\n- API エンドポイントの変更により一部機能が機能しなくなっていた問題を修正しました。\r\n- パフォーマンスを最適化しました。\r\n- 購読データセンター以外のすべての設定項目を削除しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 説明テキストを調整しました。\r\n\r\n### システムアナウンスの非表示 [システム]\r\n\r\n- 実装ロジックを全面的に再構築しました。現在はサーバーから配信されるアナウンス情報のみを遮断し、同一チャンネルの他のメッセージをフィルタリングしなくなりました。\r\n- 名称と説明テキストを調整しました。\r\n\r\n### カットシーン終了の通知 [コンテンツ]\r\n\r\n- 実装ロジックを全面的に再構築し、より安定して正確な実装方式を採用しました。\r\n- すべての設定項目を削除しました。\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### コンテンツ突入準備完了の自動通知 [コンテンツ]\r\n\r\n- 実装ロジックを全面的に再構築し、より安定して正確な実装方式を採用しました。画面の開閉によって繰り返し通知されることがなくなりました。\r\n- タイトルと説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### コンテンツ情報の通知 [コンテンツ]\r\n\r\n- 説明テキストを調整しました。\r\n- すべての設定項目を削除しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 一部のコンテンツで表示されるアイテムレベルシンクが誤ったデータになっていた問題を修正しました。\r\n\r\n### コンテンツ開始通知 [コンテンツ]\r\n\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### 「コンテンツルーレット：メンター」アチーブメント進捗の自動通知 [コンテンツ]\r\n\r\n- 実装ロジックを簡素化しました。\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n\r\n### 新着メッセージ通知 [通知]\r\n\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 煩雑すぎる多数のメッセージタイプを除外しました。\r\n- メッセージタイプを選択する際、列挙名ではなく対応するゲームテキストが表示されるようになりました。\r\n- 設定項目「ゲームがバックグラウンドにある時のみ通知」を削除しました。\r\n\r\n### 自動リーヴ受注権リセット通知 [通知]\r\n\r\n- 説明テキストを調整しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 設定項目「チャット欄にも同時に送信する」を削除しました。\r\n\r\n### 自動ルーシッドドリーム [アクション]\r\n\r\n- 説明テキストを調整しました。\r\n- 設定項目「チャットメッセージを送信」と「通知を送信」を削除しました。\r\n- 通知メッセージの外観スタイルとテキスト内容を調整しました。\r\n- 設定画面を調整しました。";

	private const string CHANGELOG_KO = "## 본체 수정\r\n\r\n- 중국 서버에만 적용되는 일부 로직이 글로벌 서버 환경에서도 정리 처리를 시도하여 null 참조 예외가 발생하던 문제를 수정했습니다.\r\n- 모든 자체 제작 네이티브 인터페이스의 제목이 글로벌 서버 환경에서 표시되지 않던 문제를 수정했습니다.\r\n- 이제 메인 화면 왼쪽 사이드바 창의 버전 번호 텍스트를 클릭하여 업데이트 기록 창을 다시 열 수 있습니다.\r\n- 업데이트 기록이 창 안에서 Markdown으로 올바르게 렌더링됩니다.\r\n- 우클릭 메뉴 분석이 \"출품 목록\" 창을 지원합니다.\r\n- 출력되는 오류 메시지의 외형이 네이티브 오류 메시지의 외형과 일치하지 않던 문제를 수정했습니다.\r\n- \"아이템 상세 정보\" 및 \"기술 상세 정보\" 창 관련 수정의 표시 효과를 개선했습니다.\r\n- IPv6 네트워크 환경에서 온라인 인증 서비스에 연결하기 어려운 문제를 개선했습니다.\r\n\r\n### 알림 기능 일괄 조정에 관한 안내\r\n\r\n(다음 변경 사항은 여러 버전에 걸쳐 조정이 완료될 수 있습니다.)\r\n\r\n- \"텍스트 음성 변환\" 기능이 별도의 `EdgeTTS.Dalamud` 플러그인으로 분리되었고, 해당 기능이 동작상 침투감이 강하기 때문에, 이제는 \"Windows 트레이 알림\"을 보내고 해당 플러그인이 활성화된 것을 감지한 경우에만 트레이 알림의 메시지 내용을 함께 읽습니다.\r\n- \"채팅 메시지\" 계열 알림은 두 가지로 구분됩니다.\r\n    - \"DR\" 접두사 있음: 내용이 명확히 플러그인에서 제공하는 기능으로만 발생할 수 있는 알림 메시지.\r\n      예시: 내용이 \"일괄 제작 완료\"인 알림 메시지, 내용이 \"장터의 특정 아이템 가격이 설정값보다 낮음\"인 알림 메시지, 내용이 \"잠수함이 모두 귀환함\"이지만 메시지와 상호작용하여 자동으로 회수하러 갈 수 있는 알림 메시지.\r\n    - \"DR\" 접두사 없음: 내용이 게임 자체에서도 발생할 수 있으며, 단지 표시되지 않았거나 명확히 알림되지 않았던 알림 메시지.\r\n      예시: 내용이 \"이동 속도 감지 구역에 진입함\"인 알림 메시지, 내용이 \"게임에 정상적으로 로그인함\"인 알림 메시지, 내용이 \"밀려나기 효과를 받음\"인 알림 메시지.\r\n- \"채팅 메시지\" 계열 알림을 제외한 다른 모든 유형의 알림에는 \"DR\" 접두사가 붙지 않습니다.\r\n- \"채팅 메시지\" 계열 알림의 전송 채널이 \"디버그\"에서 \"시스템 메시지\"로 변경됩니다. 앞으로는 \"로그 필터\" 창의 \"알림\" 탭에서 \"시스템 메시지\" 체크를 해제하여 관련 메시지를 걸러낼 수 있습니다. (모든 \"채팅 메시지\" 계열 알림이 \"시스템 메시지\" 채널을 통해 전송되는 것은 아닙니다.)\r\n- \"Dalamud 알림\" 계열 알림은 ImGui로 그려진 인터페이스를 조작할 때 발생하는 피드백에만 사용됩니다.\r\n- \"채팅 메시지\" 계열 알림은 게임 네이티브 인터페이스(플러그인이 자체 제작한 네이티브 인터페이스 포함)와 네이티브 시스템 메커니즘을 조작할 때 발생하는 피드백에 집중적으로 사용됩니다.\r\n- \"콘텐츠 기믹\" 계열 알림은 특별한 강조가 필요한 극소수의 피드백(사전에 명확히 고지된 오락성 요소 포함)에만 사용됩니다.\r\n- \"Windows 트레이 알림\" 계열 알림은 장시간 자동화나 특별히 긴급하여 백그라운드 알림이 필요한 상황에 사용됩니다. 해당 알림 역시 게임이 백그라운드에 있거나 게임이 15초 동안 아무런 키 입력을 받지 못한 경우에만 전송됩니다. 일반적으로 다른 유형의 알림도 같은 내용을 함께 전송합니다.\r\n- \"플로팅 토스트\" 계열 알림의 적용 상황은 \"채팅 메시지\" 계열 알림과 대체로 일치하지만, 구체적인 사용은 같은 종류의 게임 네이티브 시스템 메커니즘이 어떻게 사용되는지를 많이 참고하여 모방함으로써 네이티브 메커니즘에 더 가깝게 만듭니다.\r\n- \"Windows 시스템 소리\" 계열 알림(부저 포함)은 특별한 강조가 필요한 극소수의 피드백(사전에 명확히 고지된 오락성 요소 포함)에만 사용됩니다.\r\n\r\n## 신규 모듈\r\n\r\n### 「임무 다시하기」 자동 재개 [시스템]\r\n\r\n저장 데이터가 있는 경우, 로그인 시 혹은 콘텐츠 퇴장 후 임무 다시하기을 자동으로 재개합니다.\r\n\r\n### 제한 없는 매크로 아이콘 [인터페이스]\r\n\r\n*추천 모듈*\r\n\r\n/pdr micon <아이콘 ID> 명령을 사용하여 매크로 아이콘을 원하는 아이콘으로 변경할 수 있습니다.\r\n\r\n### 분해 UI 최적화 [인터페이스]\r\n\r\n*추천 모듈*\r\n\r\n'분해' UI을 최적화하여 분해 시 숙련도 정보 표시 기능을 추가하고, 아이템 일괄 분해를 지원합니다. '아이템 도움말' 화면에 상세한 아이템 분해 정보 표시 기능을 추가합니다. 아이템을 수동으로 분해할 때 자동으로 확인됩니다.\r\n\r\n![OptimizedSalvage-Addon](/assets/Changelog/1.0.0.0/OptimizedSalvage.png)\r\n\r\n## 삭제된 모듈\r\n\r\n### *자동 아이템 분해 [인터페이스]\r\n\r\n기능이 이전되었으므로 새 모듈 \"'분해' UI 최적화\"를 사용하십시오.\r\n\r\n### *특수 플레이어 출현 자동 알림 [알림 기능]\r\n\r\n구현 방식이 너무 구식이라 유지보수가 어렵고, 실제 용도가 불분명하며, 다른 모듈이나 플러그인으로 대체할 수 있어 삭제했습니다.\r\n\r\n## 모듈 수정\r\n\r\n### 터보 클릭 [시스템]\r\n\r\n- \"게임패드 모드\"에서 정상적으로 사용할 수 없던 문제를 수정했습니다.\r\n- \"대기 시간\" 설정 항목을 추가했습니다. 이 시간 이상 길게 누르면 반복 클릭이 시작되며, 0이면 대기하지 않습니다.\r\n\r\n### 불필요한 팝업 자동 숨김 [인터페이스]\r\n\r\n- 모듈을 비활성화한 후 일부 창이 다시 나타나던 문제를 수정했습니다.\r\n\r\n### 자동 따라가기 개선 [시스템]\r\n\r\n- \"콘텐츠 내에서만 활성화\" 설정 항목을 추가했습니다. 기본값은 비활성화입니다.\r\n\r\n### 아이템의 NPC 상점 정보를 표시합니다 [인터페이스]\r\n\r\n- \"NPC 상점 [사용처]\" 창에 아이템 개요 정보를 더 이상 표시하지 않습니다.\r\n- \"NPC 상점 [사용처]\" 창에서 \"장터\" 버튼을 클릭할 수 없던 문제를 수정했습니다.\r\n- 페이지 넘김 컴포넌트의 외형과 위치를 조정했습니다.\r\n- 열기 로직을 최적화했습니다.\r\n- 아이템 아이콘 모두 우클릭 메뉴를 열 수 있게 되었습니다. 따라서 \"장터\" 버튼도 제거했습니다.\r\n- \"장터\" 버튼이 제거됨에 따라, \"NPC 상점 [사용처]\" 창에서 원래 독립된 행이었던 비용 상세가 아이템 이름과 같은 행에 수평 오른쪽 정렬로 표시됩니다.\r\n- 상점 목록의 상호작용 로직을 조정하여, 좌클릭하면 지도를 바로 열고 우클릭하면 메뉴가 열려 \"가장 가까운 거점으로 이동\"을 선택할 수 있습니다.\r\n- 이제 상점 목록 표시가 지역별로 정렬됩니다.\r\n- \"아이템 상세 정보\" 창에 표시되는 상점 정보와 열린 창 안에 표시되는 상점 정보가 일치하지 않던 문제를 수정했습니다.\r\n- 모든 전제 모듈을 제거했습니다.\r\n- 고품질 아이템의 \"아이템 유지\" 창에 상점 정보가 표시되지 않던 문제를 수정했습니다.\r\n- 이제 \"아이템 상세 정보\" 창 안의 원래 \"상점 판매\" 정보를 항상 삭제하여 창을 간소화하고 중복을 피합니다.\r\n\r\n### 빠른 전송 패널 [인터페이스]\r\n\r\n- 임무 오브젝트 판정 로직을 최적화했습니다.\r\n- 임무 오브젝트로 전송할 때 더 높은 위치로 보정될 수 있던 문제를 수정했습니다.\r\n- vnavmesh 경로 탐색 로직을 제거했습니다.\r\n- 확정된 대상 오브젝트로 전송할 때 대상과 겹치지 않게 되었습니다.\r\n- 임무 오브젝트와 상호작용할 때 자동으로 선택됩니다.\r\n- 이제 vnavmesh가 활성화되어 있으면 모든 전송 가능 지점의 위치가 이를 거쳐 보정됩니다.\r\n- \"상호작용 방식 최적화\" 모듈을 함께 사용할 수 있는 모듈로 추가했습니다. 활성화하면 상호작용 가능 거리 판정이 조정됩니다.\r\n\r\n### 퀵 채팅 패널 [인터페이스]\r\n\r\n- 스크롤바가 자동으로 숨겨지지 않던 문제를 수정했습니다.\r\n\r\n### 퀵 단골 거래 정보 화면 [인터페이스]\r\n\r\n- 우클릭 메뉴 구현 방식을 조정했습니다. 설정에 따라 메뉴 항목에 \"DR\" 접두사가 표시될 수 있습니다.\r\n\r\n### 장터 게시판 검색 결과 자동 새로고침 [시스템]\r\n\r\n- 이제 로그아웃 시 장터 데이터를 자동으로 비워, 데이터센터를 넘어 다시 로그인한 후 장터 데이터를 가져오지 못할 수 있던 문제를 수정했습니다.\r\n- 무료 체험 상태에서도 장터 데이터 요청을 반복 재시도하던 문제를 수정했습니다.\r\n\r\n### 어디서든 장터 게시판 [인터페이스]\r\n\r\n- 이제 무료 체험 상태에서는 로컬 장터 데이터를 요청하지 않고, 항상 Universalis에서 가져온 데이터를 표시합니다.\r\n- 수동으로 지역을 전환한 후 서버가 자동으로 전환되지 않던 문제를 수정했습니다.\r\n- \"아이템 상세 정보\" 창에 표시되는 \"일일 판매량\" 수치가 포맷되지 않던 문제를 수정했습니다.\r\n\r\n### 빠른 서버 텔레포트 명령어 [인터페이스]\r\n\r\n- (중국 서버 한정) 인터페이스 제목을 조정했습니다.\r\n\r\n### 제작 수첩 UI 최적화 [제작 및 채집]\r\n\r\n- \"제작 절차\" 창 안의 우클릭 메뉴가 \"제작 수첩\" 창이 닫힌 상태에서 열리지 않던 문제를 수정했습니다.\r\n\r\n### 낚시터 경계 자동 해제 [제작 및 채집]\r\n\r\n- \"추가 텍스트 명령어\"가 이제 매크로 명령 형태로 감싸져 실행됩니다.\r\n- \"추가 텍스트 명령어\"의 잘못된 도움말 정보를 수정했습니다.\r\n- 설명을 조정했습니다.\r\n- 알림 메시지 텍스트와 외형 스타일을 조정했습니다.\r\n- 설정 화면에 \"전제 조건\"을 추가하여 실행에 필요한 임무를 표시합니다.\r\n\r\n### 집사 수행 자동화 [인터페이스]\r\n\r\n- \"출품 시 가격 자동 변경\" 설정 항목의 표시 텍스트를 \"출품 가격 자동 설정\"으로 변경하고 관련 안내 정보를 추가했습니다.\r\n- \"한 번에 출품 수\" 설정이 적용되지 않던 문제를 수정했습니다.\r\n- \"집사 목록\" 창 옆의 플로팅 창에서는 \"출품 가격 자동 설정\"을 설정할 수 없게 되었습니다.\r\n- \"자동 출품\" 설정 항목을 추가했습니다. 아이템을 출품할 때 표시되는 \"가격 변경\" 창의 \"결정\" 버튼을 자동으로 클릭합니다.\r\n- \"출품 목록\" 창 옆의 플로팅 창 오른쪽 위에 \"설정\" 버튼을 추가하여 클릭하면 설정 창을 열 수 있습니다. 기존 \"가격 기록 초기화\" 텍스트 버튼은 오른쪽 위의 아이콘 버튼이 되었습니다.\r\n- 우클릭 메뉴 분석이 \"출품 목록\" 창을 지원하게 됨에 따라, 이제 가격 변경 관련 자동 작업이 완료된 후 또는 \"가격 변경\" 창이 닫힐 때 \"어디서든 장터 게시판\" 모듈 창이 자동으로 닫힙니다.\r\n- 창을 닫을 때 일부 리소스가 정리되지 않던 문제를 수정했습니다.\r\n\r\n### NPC 대화 자동 스킵 [시스템]\r\n\r\n- 로직을 완전히 재구성하여 더 저수준의 구현 방식을 사용했고, 더 많은 상황을 커버하며 버전 업데이트 시 무효화될 가능성이 낮아졌습니다.\r\n- 설명을 조정했습니다.\r\n\r\n### 스킵 제한 영상 강제 스킵 [시스템]\r\n\r\n- 폴백 스킵 로직을 최적화하여 더 저수준의 구현 방식을 채택했습니다.\r\n- 관찰형 컷신을 자동 선택할 때 해당 상태에 두 번째로 진입할 때부터 자동 완료되지 않던 문제를 수정했습니다.\r\n- 설정 화면 그리기 로직을 최적화했습니다.\r\n\r\n### 컨텐츠 자동 퇴장 [콘텐츠]\r\n\r\n- 현재 콘텐츠 임무 완료 시의 자동 퇴장 동작을 수동으로 지정하는 `/pdr autoleaveduty` 명령을 추가했습니다. 수동 지정 동작은 설정보다 우선합니다.\r\n    - `/pdr autoleaveduty None`: 수동 지정한 자동 퇴장 동작을 해제합니다.\r\n    - `/pdr autoleaveduty InstantLeave`: 콘텐츠 임무 완료 시 즉시 자동 퇴장합니다.\r\n    - `/pdr autoleaveduty NoLeave`: 콘텐츠 임무 완료 시 자동 퇴장하지 않습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n\r\n### 즉시 로그아웃 / 종료 [시스템]\r\n\r\n- 로그아웃 시 즉시 먼저 로컬에서 로그아웃하여, 서버 응답이 느려 절차가 멈추는 것을 방지합니다.\r\n\r\n### 자동 돌발 임무 [스크립트]\r\n\r\n- \"레벨을 훨씬 뛰어넘는 돌발 임무 무시\" 설정 항목을 추가했습니다. 돌발 임무 레벨이 현재 플레이어 레벨보다 8 이상 높으면 완료할 수 없거나 완료 효율이 지나치게 낮아지는 것을 피하기 위해 이동하지 않습니다. 기본값은 활성화입니다.\r\n\r\n### 자동 로그인 알림 [알림 기능]\r\n\r\n- 모든 설정 항목을 제거했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### 전투 시작 카운트다운 시작 알림 [전투]\r\n\r\n- 분류를 \"알림\"에서 \"전투\"로 변경했습니다.\r\n- 모든 설정 항목을 제거했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### *보너스 보상 돌발 임무 출현 자동 알림 [전투]\r\n\r\n- \"지도 열기\"를 제외한 모든 설정 항목을 제거했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 이름과 설명 텍스트를 조정했습니다.\r\n\r\n### 준비 확인 알림 [전투]\r\n\r\n- 성능을 최적화했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n\r\n### 디아뎀 제도 특수 날씨 자동 알림 [제작 및 채집]\r\n\r\n- 성능을 최적화했습니다.\r\n- 분류를 \"알림\"에서 \"제작 및 채집\"으로 변경했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n\r\n### 멸 어둠의 구름 격투전 보상 증가 알림 [콘텐츠]\r\n\r\n- API 엔드포인트 변경으로 기능 일부가 작동하지 않던 문제를 수정했습니다.\r\n- 성능을 최적화했습니다.\r\n- 구독 데이터센터를 제외한 모든 설정 항목을 제거했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n\r\n### *시스템 공지 자동 차단 [시스템]\r\n\r\n- 구현 로직을 완전히 재구성했습니다. 이제 서버에서 내려오는 공지 정보만 차단하며, 같은 채널의 다른 메시지를 걸러내지 않습니다.\r\n- 이름과 설명 텍스트를 조정했습니다.\r\n\r\n### 컷신 종료 자동 알림 [콘텐츠]\r\n\r\n- 구현 로직을 완전히 재구성하여 더 안정적이고 정확한 구현 방식을 채택했습니다.\r\n- 모든 설정 항목을 제거했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### *콘텐츠 출발 준비 완료 자동 알림 [콘텐츠]\r\n\r\n- 구현 로직을 완전히 재구성하여 더 안정적이고 정확한 구현 방식을 채택했으며, 이제 창을 여닫아도 반복 알림되지 않습니다.\r\n- 제목과 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### 던전 정보 알림 [콘텐츠]\r\n\r\n- 설명 텍스트를 조정했습니다.\r\n- 모든 설정 항목을 제거했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 일부 콘텐츠에서 표시되는 아이템 레벨 동기화가 잘못된 데이터로 표시되던 문제를 수정했습니다.\r\n\r\n### 콘텐츠 시작 알림 [콘텐츠]\r\n\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### “무작위 임무：멘토” 업적 진행 상황 자동 알림 [콘텐츠]\r\n\r\n- 구현 로직을 단순화했습니다.\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n\r\n### 새 메시지 알림 [알림 기능]\r\n\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 지나치게 복잡한 다수의 메시지 유형을 걸러냈습니다.\r\n- 메시지 유형을 선택할 때 열거형 이름이 아닌 해당 게임 텍스트가 표시됩니다.\r\n- \"게임이 백그라운드일 때만 알림\" 설정 항목을 제거했습니다.\r\n\r\n### 자동 수주권 리셋 시간 알림 [알림 기능]\r\n\r\n- 설명 텍스트를 조정했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- \"채팅창에도 함께 전송\" 설정 항목을 제거했습니다.\r\n\r\n### 자동 자각몽 [액션]\r\n\r\n- 설명 텍스트를 조정했습니다.\r\n- \"채팅 메세지 송신\"과 \"알림 송신\" 설정 항목을 제거했습니다.\r\n- 알림 메시지 외형 스타일과 텍스트 내용을 조정했습니다.\r\n- 설정 화면을 조정했습니다.";

	private int onboardingStep;

	private float headerTextGroupHeight;

	private const ImGuiWindowFlags WINDOW_FLAGS = (ImGuiWindowFlags)57;

	private static FrozenDictionary<Language, string> Changelogs;

	private const string SITE_URL = "hxxps://info.atmoomen.top";

	internal Version LastVersion { get; set; }

	private bool IsNewVersion
	{
		get
		{
			Version lastVersion = LastVersion;
			if ((object)lastVersion != null && lastVersion.Major == 0 && lastVersion.Minor == 0 && lastVersion.Build == 0)
			{
				return lastVersion.Revision == 0;
			}
			return false;
		}
	}

	public XOoANOVG4jDKQ7WvCb9()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		LastVersion = new Version(0, 0, 0, 0);
		((Window)this)._002Ector(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC02588 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
		((Window)this).Flags = (ImGuiWindowFlags)57;
		ImGuiViewportPtr mainViewport = ImGui.GetMainViewport();
		((Window)this).Size = ((ImGuiViewportPtr)(ref mainViewport)).WorkSize * 0.3f;
		((Window)this).SizeCondition = (ImGuiCond)4;
		((Window)this).Position = ((ImGuiViewportPtr)(ref mainViewport)).WorkPos + (((ImGuiViewportPtr)(ref mainViewport)).WorkSize - ((Window)this).Size.Value * 1.5f) / 2f;
		((Window)this).PositionCondition = (ImGuiCond)4;
	}

	public void Dispose()
	{
	}

	public override void Draw()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)1, Globals.ScaledVector2(18f, 16f), true);
		try
		{
			StyleDisposable val2 = ImRaii.PushStyle((ImGuiStyleVar)13, Globals.ScaledVector2(8f, 8f), true);
			try
			{
				lfWVkgjxUe();
				ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 2f * Globals.GlobalUIScale);
				float num = 32f * Globals.GlobalUIScale;
				float num2 = 10f * Globals.GlobalUIScale;
				float num3 = num2 + num;
				float y = ImGui.GetContentRegionAvail().Y;
				float num4 = y - num3;
				ImGuiStylePtr style = ImGui.GetStyle();
				float contentHeight = MathF.Max(100f, num4 - ((ImGuiStylePtr)(ref style)).ItemSpacing.Y - 4f * Globals.GlobalUIScale);
				if (IsNewVersion)
				{
					bndVNFwr7s(contentHeight);
					H75V7QakNq(num, num2);
				}
				else
				{
					bndVNFwr7s(contentHeight);
					H75V7QakNq(num, num2);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void lfWVkgjxUe()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		float value = ((headerTextGroupHeight > 0f) ? headerTextGroupHeight : (52f * Globals.GlobalUIScale));
		Vector2 vector = new Vector2(value);
		IDalamudTextureWrap wrapOrEmpty = Assets.Icon.GetWrapOrEmpty();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float num = 8f * Globals.GlobalUIScale;
		uint num2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.12f).ToUInt();
		uint num3 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.35f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + vector, num2, num);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, cursorScreenPos + vector, num3, num, (ImDrawFlags)0, 1.5f);
		ImGui.Image(wrapOrEmpty.Handle, vector);
		ImGui.SameLine(0f, 12f * Globals.GlobalUIScale);
		GroupDisposable val = ImRaii.Group();
		try
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit("ziz"));
			}
			ImGui.SameLine(0f, 10f * Globals.GlobalUIScale);
			xosV23Nhtb("1.0");
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				string text = "ziz Premium for free, fuck China.";
				ImGui.TextUnformatted(ImU8String.op_Implicit(text));
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		headerTextGroupHeight = ImGui.GetItemRectSize().Y;
	}

	private static void xosV23Nhtb(string badgeText)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
		{
			Vector2 vector = ImGui.CalcTextSize(ImU8String.op_Implicit(badgeText), false, -1f);
			Vector2 vector2 = Globals.ScaledVector2(8f, 2.5f);
			Vector2 vector3 = vector + vector2 * 2f;
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			cursorScreenPos.Y += 3f * Globals.GlobalUIScale;
			ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
			float num = 10f * Globals.GlobalUIScale;
			uint num2 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.2f).ToUInt();
			uint num3 = VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.6f).ToUInt();
			((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + vector3, num2, num);
			((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, cursorScreenPos + vector3, num3, num, (ImDrawFlags)0, 1f);
			Vector2 vector4 = cursorScreenPos + vector2;
			((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, KnownColor.Chocolate.ToVector4().ToUInt(), ImU8String.op_Implicit(badgeText));
			ImGui.Dummy(vector3);
		}
	}

	private void iJcVZ9R6mq(float contentHeight)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)3, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.02f), true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)5, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.06f), true);
			try
			{
				StyleDisposable val3 = ImRaii.PushStyle((ImGuiStyleVar)6, 8f * Globals.GlobalUIScale, true);
				try
				{
					StyleDisposable val4 = ImRaii.PushStyle((ImGuiStyleVar)7, 1f, true);
					try
					{
						StyleDisposable val5 = ImRaii.PushStyle((ImGuiStyleVar)1, Globals.ScaledVector2(16f, 14f), true);
						try
						{
							ChildDisposable val6 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5132D5EE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)), new Vector2(0f, contentHeight), true, (ImGuiWindowFlags)0);
							try
							{
								if (!(!val6))
								{
									switch (onboardingStep)
									{
									case 0:
										sHMVaWVU7N();
										break;
									case 1:
										leNV0QyDig();
										break;
									case 2:
										FSTV5dOrHD();
										break;
									}
								}
							}
							finally
							{
								((ChildDisposable)(ref val6)).Dispose();
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void sHMVaWVU7N()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		using (OmenServiceBase<FontManager>.Instance().UIFont120.Push())
		{
			ImGui.TextUnformatted(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EF21E7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b))));
		}
		if ((int)GameState.ClientLanguage != 2)
		{
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				ImGui.TextUnformatted(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x654144CA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb)));
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		ImGuiOm.ScaledDummy(0f, 6f);
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923D8AA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2)), 2, (ImGuiTableFlags)32768);
		try
		{
			if (!val2)
			{
				return;
			}
			Language uILanguage = PluginConfig.Instance().UILanguage;
			int num = 0;
			foreach (KeyValuePair<Language, string> availableLanguage in LanguageManager.AvailableLanguages)
			{
				ImGui.TableNextColumn();
				bool flag = uILanguage == availableLanguage.Key;
				float y = 36f * Globals.GlobalUIScale;
				Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
				float x = ImGui.GetContentRegionAvail().X;
				Vector2 vector = new Vector2(x, y);
				float num2 = 6f * Globals.GlobalUIScale;
				ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
				uint num3 = (flag ? VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.2f).ToUInt() : VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.04f).ToUInt());
				uint num4 = (flag ? VectorExtensions.WithW(KnownColor.Chocolate.ToVector4(), 0.7f).ToUInt() : VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.1f).ToUInt());
				((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + vector, num3, num2);
				((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, cursorScreenPos + vector, num4, num2, (ImDrawFlags)0, flag ? 1.5f : 1f);
				ImU8String val3 = new ImU8String(8, 1);
				((ImU8String)(ref val3)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BAD23 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
				((ImU8String)(ref val3)).AppendFormatted<Language>(availableLanguage.Key);
				if (ImGui.InvisibleButton(val3, vector, (ImGuiButtonFlags)0) && uILanguage != availableLanguage.Key)
				{
					PluginConfig.Instance().UILanguage = availableLanguage.Key;
					PluginConfig.Instance().Save();
					LanguageManager.LoadLanguage(availableLanguage.Key);
				}
				if (ImGui.IsItemHovered())
				{
					ImGui.SetMouseCursor((ImGuiMouseCursor)7);
				}
				Vector2 vector2 = cursorScreenPos + Globals.ScaledVector2(12f, 9.5f);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector2, flag ? KnownColor.Chocolate.ToVector4().ToUInt() : ImGui.GetColorU32((ImGuiCol)1), ImU8String.op_Implicit(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61612)));
				Vector2 vector3 = cursorScreenPos + Globals.ScaledVector2(36f, 9.5f);
				((ImDrawListPtr)(ref windowDrawList)).AddText(vector3, flag ? ImGui.GetColorU32((ImGuiCol)0) : ImGui.GetColorU32((ImGuiCol)0, 0.85f), ImU8String.op_Implicit(availableLanguage.Value));
				if (flag)
				{
					string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61452);
					Vector2 vector4 = cursorScreenPos + new Vector2(x - ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X - 14f * Globals.GlobalUIScale, 9.5f * Globals.GlobalUIScale);
					((ImDrawListPtr)(ref windowDrawList)).AddText(vector4, KnownColor.Chocolate.ToVector4().ToUInt(), ImU8String.op_Implicit(text));
				}
				num++;
				if (num <= 4)
				{
					ImGuiOm.ScaledDummy(0f, 4f);
				}
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
	}

	private static void leNV0QyDig()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78849A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00))));
		ImGuiOm.ScaledDummy(0f, 8f);
		ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6541442E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb))));
		ImGuiOm.ScaledDummy(0f, 12f);
		N03VRHeYAn(delegate
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, KnownColor.Goldenrod.ToVector4(), true);
			try
			{
				ImU8String val2 = new ImU8String(2, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61553));
				((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x28298F4D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184));
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x473355EF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5)));
				ImGui.TextUnformatted(val2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			ImGui.Spacing();
			ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37070C0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e))));
		}, VectorExtensions.WithW(KnownColor.Goldenrod.ToVector4(), 0.08f), VectorExtensions.WithW(KnownColor.Goldenrod.ToVector4(), 0.4f));
	}

	private static void FSTV5dOrHD()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		_003C_003Ec__DisplayClass20_0 obj = new _003C_003Ec__DisplayClass20_0();
		ImGuiStylePtr style = ImGui.GetStyle();
		obj.cellPaddingX = ((ImGuiStylePtr)(ref style)).CellPadding.X;
		N03VRHeYAn(delegate(float contentWidth)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_016b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0228: Unknown result type (might be due to invalid IL or missing references)
			float num = 180f * Globals.GlobalUIScale;
			float num2 = num + obj.cellPaddingX * 2f;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1C768 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83)), 2, (ImGuiTableFlags)0, new Vector2(contentWidth, 0f));
			try
			{
				if (TableDisposable.op_Implicit(val))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9DC9A3C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1cc312ba3ec946acba954139762d703f)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C62E6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32)), (ImGuiTableColumnFlags)8, num2, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val2 = new ImU8String(2, 2);
					((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61489));
					((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC0260E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83));
					((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBB8DB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2)));
					ImGui.TextUnformatted(val2);
					ImGui.TableNextColumn();
					float num3 = OmenServiceBase<FontManager>.Instance().Config.FontSize;
					ImGui.SetNextItemWidth(num);
					if (ImGui.InputFloat(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x61164208 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402)), ref num3, 0.5f, 1f, ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4119EA33 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52)), (ImGuiInputTextFlags)0))
					{
						num3 = Math.Clamp(num3, 8f, 48f);
					}
					if (ImGui.IsItemDeactivatedAfterEdit())
					{
						OmenServiceBase<FontManager>.Instance().Config.FontSize = num3;
						OmenServiceBase<FontManager>.Instance().Config.Save();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
			ImGui.Spacing();
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B210E60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a))));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		});
		ImGuiOm.ScaledDummy(0f, 10f);
		N03VRHeYAn(delegate(float contentWidth)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_013c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
			float num = MathF.Ceiling(ImGui.GetTextLineHeightWithSpacing() * 1.875f) + 2f + obj.cellPaddingX * 2f;
			TableDisposable val = ImRaii.Table(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA26D7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1)), 2, (ImGuiTableFlags)0, new Vector2(contentWidth, 0f));
			try
			{
				if (TableDisposable.op_Implicit(val))
				{
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B306A28 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117)), (ImGuiTableColumnFlags)4, 0f, 0u);
					ImGui.TableSetupColumn(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x226783D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44)), (ImGuiTableColumnFlags)8, num, 0u);
					ImGui.TableNextRow();
					ImGui.TableNextColumn();
					ImGui.AlignTextToFramePadding();
					ImU8String val2 = new ImU8String(2, 2);
					((ImU8String)(ref val2)).AppendFormatted<string>(FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61726));
					((ImU8String)(ref val2)).AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BF160 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
					((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A885 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09)));
					ImGui.TextUnformatted(val2);
					ImGui.TableNextColumn();
					bool value = PluginConfig.Instance().DefaultEnableModules;
					if (ImGuiOm.ToggleButton(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E38734B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997), ref value))
					{
						PluginConfig.Instance().DefaultEnableModules = value;
						PluginConfig.Instance().Save();
					}
				}
			}
			finally
			{
				((TableDisposable)(ref val)).Dispose();
			}
			ImGui.Spacing();
			ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)0, ImGui.GetColorU32((ImGuiCol)1), true);
			try
			{
				ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA7B071 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff))));
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		});
	}

	private static void N03VRHeYAn(Action<float> drawContent, Vector4? customBg = null, Vector4? customBorder = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		float x = ImGui.GetContentRegionAvail().X;
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		Vector2 vector = Globals.ScaledVector2(14f, 12f);
		float num = x - vector.X * 2f;
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSplit(2);
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(1);
		ImGui.SetCursorScreenPos(cursorScreenPos + vector);
		ImGui.PushTextWrapPos(ImGui.GetCursorPosX() + num - 2f * Globals.GlobalUIScale);
		GroupDisposable val = ImRaii.Group();
		try
		{
			drawContent(num);
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
		ImGui.PopTextWrapPos();
		float y = ImGui.GetItemRectSize().Y + vector.Y * 2f;
		Vector2 vector2 = new Vector2(x, y);
		((ImDrawListPtr)(ref windowDrawList)).ChannelsSetCurrent(0);
		uint num2 = (customBg ?? VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.025f)).ToUInt();
		uint num3 = (customBorder ?? VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.08f)).ToUInt();
		float num4 = 8f * Globals.GlobalUIScale;
		((ImDrawListPtr)(ref windowDrawList)).AddRectFilled(cursorScreenPos, cursorScreenPos + vector2, num2, num4);
		((ImDrawListPtr)(ref windowDrawList)).AddRect(cursorScreenPos, cursorScreenPos + vector2, num3, num4, (ImDrawFlags)0, 1f);
		((ImDrawListPtr)(ref windowDrawList)).ChannelsMerge();
		ImGui.SetCursorScreenPos(cursorScreenPos);
		ImGui.Dummy(vector2);
	}

	private void CuCVDoMded(float buttonHeight, float footerSpacing)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06de: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float x = ImGui.GetContentRegionAvail().X;
		uint num = VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.08f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddLine(cursorScreenPos, cursorScreenPos + new Vector2(x, 0f), num, 1f);
		ImGui.SetCursorPosY(ImGui.GetCursorPosY() + footerSpacing);
		Vector2 vector = new Vector2(buttonHeight);
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 8f * Globals.GlobalUIScale, true);
		try
		{
			if (onboardingStep > 0)
			{
				ImGui.SameLine(0f, 12f * Globals.GlobalUIScale);
				string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61536) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE5C96 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65414538 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb));
				float x2 = MathF.Max(100f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X + 24f * Globals.GlobalUIScale);
				if (ImGui.Button(ImU8String.op_Implicit(text), new Vector2(x2, buttonHeight)))
				{
					onboardingStep--;
				}
			}
			switch (onboardingStep)
			{
			case 0:
			{
				string text5 = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C789A32 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x281761BE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd) + FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61537);
				float num7 = MathF.Max(140f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(text5), false, -1f).X + 28f * Globals.GlobalUIScale);
				float cursorPosX3 = ImGui.GetWindowContentRegionMax().X - num7;
				ImGui.SameLine();
				ImGui.SetCursorPosX(cursorPosX3);
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, VectorExtensions.WithW(KnownColor.Magenta.ToVector4(), 0.85f), true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, KnownColor.DimGray.ToVector4(), true);
					try
					{
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, KnownColor.DarkRed.ToVector4(), true);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(text5), new Vector2(num7, buttonHeight)))
							{
								onboardingStep = 1;
							}
							break;
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			case 1:
			{
				string text3 = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6B3194A4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117));
				float num3 = MathF.Max(100f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(text3), false, -1f).X + 24f * Globals.GlobalUIScale);
				string text4 = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BB3EB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1)) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BF160 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a) + FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61537);
				float num4 = MathF.Max(120f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(text4), false, -1f).X + 28f * Globals.GlobalUIScale);
				float num5 = 8f * Globals.GlobalUIScale;
				float num6 = num3 + num5 + num4;
				float cursorPosX2 = ImGui.GetWindowContentRegionMax().X - num6;
				ImGui.SameLine();
				ImGui.SetCursorPosX(cursorPosX2);
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, VectorExtensions.WithW(KnownColor.Magenta.ToVector4(), 0.25f), true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, VectorExtensions.WithW(KnownColor.DimGray.ToVector4(), 0.5f), true);
					try
					{
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, VectorExtensions.WithW(KnownColor.DarkRed.ToVector4(), 0.75f), true);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(text3), new Vector2(num3, buttonHeight)))
							{
								OmenServiceBase<ChatManager>.Instance().SendMessage(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6CA6503F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86e8e55020514087b1fe097f879d6dff));
								((Window)this).IsOpen = false;
							}
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				ImGui.SameLine(0f, num5);
				val2 = ImRaii.PushColor((ImGuiCol)21, VectorExtensions.WithW(KnownColor.Magenta.ToVector4(), 0.85f), true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, KnownColor.DimGray.ToVector4(), true);
					try
					{
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, KnownColor.DarkRed.ToVector4(), true);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(text4), new Vector2(num4, buttonHeight)))
							{
								onboardingStep = 2;
							}
							break;
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			case 2:
			{
				string text2 = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61537) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6117A330 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_8883645b187042ce8990c01069a51402) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BEE08 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
				float num2 = MathF.Max(140f * Globals.GlobalUIScale, ImGui.CalcTextSize(ImU8String.op_Implicit(text2), false, -1f).X + 28f * Globals.GlobalUIScale);
				float cursorPosX = ImGui.GetWindowContentRegionMax().X - num2;
				ImGui.SameLine();
				ImGui.SetCursorPosX(cursorPosX);
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, VectorExtensions.WithW(KnownColor.Magenta.ToVector4(), 0.85f), true);
				try
				{
					ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, KnownColor.DimGray.ToVector4(), true);
					try
					{
						ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, KnownColor.DarkRed.ToVector4(), true);
						try
						{
							if (ImGui.Button(ImU8String.op_Implicit(text2), new Vector2(num2, buttonHeight)))
							{
								rTQVemoGbE();
							}
							break;
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void rTQVemoGbE()
	{
		PluginConfig.Instance().LastVersion = Plugin.Version.ToString();
		PluginConfig.Instance().Save();
		((Window)this).IsOpen = false;
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				await TaskExtension.WaitForConditionAsync(delegate
				{
					ModuleManager moduleManager = Y5SWncTBsGUDX5l2wY.S36MXGucI<ModuleManager>();
					return moduleManager != null && moduleManager.IsInitialized && !moduleManager.IsDisposed && !moduleManager.IsInitializingAllModules && !moduleManager.IsLoadingAllModules;
				}, TimeSpan.FromSeconds(180L), CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
				await ManagerBase<ModuleManager>.Instance().ReloadAllAsync(CancellationToken.None).ConfigureAwait(continueOnCapturedContext: false);
			}
			catch (Exception ex)
			{
				DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC1A570 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), ex);
			}
		});
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
			if (hfWYPNVIio8M9ncZnm != null)
			{
				((Window)hfWYPNVIio8M9ncZnm).IsOpen = true;
			}
		}, default(TimeSpan), 0, default(CancellationToken));
	}

	private static void bndVNFwr7s(float contentHeight)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		ColorDisposable val = ImRaii.PushColor((ImGuiCol)3, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.02f), true);
		try
		{
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)5, VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.06f), true);
			try
			{
				StyleDisposable val3 = ImRaii.PushStyle((ImGuiStyleVar)6, 8f * Globals.GlobalUIScale, true);
				try
				{
					StyleDisposable val4 = ImRaii.PushStyle((ImGuiStyleVar)7, 1f, true);
					try
					{
						StyleDisposable val5 = ImRaii.PushStyle((ImGuiStyleVar)1, Globals.ScaledVector2(16f, 14f), true);
						try
						{
							ChildDisposable val6 = ImRaii.Child(ImU8String.op_Implicit(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C789ACE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00)), new Vector2(0f, contentHeight), true, (ImGuiWindowFlags)0);
							try
							{
								if (!(!val6))
								{
									ImGuiMarkdown.Render("# You will own everything, and you will be happy.\n\n\u00a0\n\nziz\n\nFuck Square Enix\n\nFuck China\n\n\u00a0\n\n\u00a0\n\n\u00a0\n\n\u00a0\n\n\u00a0\n\n\u00a0\n\n\u00a0\n\nYour data is safe, this plugin connects to no network\n\nI will never retrieve your IP, Hardware ID, or Account ID", new MarkdownConfig());
								}
							}
							finally
							{
								((ChildDisposable)(ref val6)).Dispose();
							}
						}
						finally
						{
							((IDisposable)val5)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void H75V7QakNq(float buttonHeight, float footerSpacing)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		ImDrawListPtr windowDrawList = ImGui.GetWindowDrawList();
		Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
		float x = ImGui.GetContentRegionAvail().X;
		uint num = VectorExtensions.WithW(KnownColor.White.ToVector4(), 0.08f).ToUInt();
		((ImDrawListPtr)(ref windowDrawList)).AddLine(cursorScreenPos, cursorScreenPos + new Vector2(x, 0f), num, 1f);
		ImGui.SetCursorPosY(ImGui.GetCursorPosY() + footerSpacing);
		Vector2 vector = new Vector2(buttonHeight);
		StyleDisposable val = ImRaii.PushStyle((ImGuiStyleVar)11, 8f * Globals.GlobalUIScale, true);
		try
		{
			string text = FontAwesomeExtensions.ToIconString((FontAwesomeIcon)61537) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9075B22 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3) + LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329D078A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
			float x2 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
			float num2 = MathF.Max(140f * Globals.GlobalUIScale, x2 + 28f * Globals.GlobalUIScale);
			Vector2 vector2 = new Vector2(num2, buttonHeight);
			ImGui.SetCursorPosX(ImGui.GetWindowContentRegionMax().X - num2);
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, VectorExtensions.WithW(KnownColor.Magenta.ToVector4(), 0.85f), true);
			try
			{
				ColorDisposable val3 = ImRaii.PushColor((ImGuiCol)22, KnownColor.DimGray.ToVector4(), true);
				try
				{
					ColorDisposable val4 = ImRaii.PushColor((ImGuiCol)23, KnownColor.DarkRed.ToVector4(), true);
					try
					{
						if (ImGui.Button(ImU8String.op_Implicit(text), vector2))
						{
							VqoVd3MSJ4();
						}
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void VqoVd3MSJ4()
	{
		if (IsNewVersion)
		{
			rTQVemoGbE();
			return;
		}
		PluginConfig.Instance().LastVersion = Plugin.Version.ToString();
		PluginConfig.Instance().Save();
		((Window)this).IsOpen = false;
		DalamudServiceExtension.Instance<IFramework>().RunOnTick((Action)delegate
		{
			HfWYPNVIio8M9ncZnm0 hfWYPNVIio8M9ncZnm = OmenServiceBase<WindowManager>.Instance().Get<HfWYPNVIio8M9ncZnm0>();
			if (hfWYPNVIio8M9ncZnm != null)
			{
				((Window)hfWYPNVIio8M9ncZnm).IsOpen = true;
			}
		}, default(TimeSpan), 0, default(CancellationToken));
	}

	static XOoANOVG4jDKQ7WvCb9()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Changelogs = new Dictionary<Language, string>
		{
			[(Language)5] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B5F102 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3),
			[(Language)6] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D34BAE ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1),
			[(Language)8] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x46280A8F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_93e1cfc241b644019a4819c49327d6a7),
			[(Language)2] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7C0A10 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571),
			[(Language)1] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E3929D7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997),
			[(Language)7] = GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5133ABEC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b)
		}.ToFrozenDictionary();
	}
}
