using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Text.Json;
using DailyRoutines.Common.Module.Abstractions;
using Dalamud.Bindings.ImGui;
using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using Lumina.Excel.Sheets;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Models;
using OmenTools.OmenService;

namespace DailyRoutines.AnimationLockOptions;

[SupportedOSPlatform("windows7.0")]
public static class Runtime
{
	private delegate nint LimitBreakDelegate(nint input);

	public sealed class Options
	{
		public bool AdvanceMode { get; set; } = true;

		public bool RemoveLBlock { get; set; }
	}

	private sealed class State : IDisposable
	{
		internal readonly object Module;

		internal readonly string Path;

		internal readonly FieldInfo ConfigField;

		internal readonly FieldInfo LockField;

		internal Options Options;

		internal bool Eligible = true;

		internal bool HasLocalAction;

		internal uint LastActionId;

		internal byte LastActionType;

		internal bool Disposed;

		internal Hook<LimitBreakDelegate>? LimitBreakHook;

		internal Action<ModuleBase>? OriginalCleanup;

		internal float ConfiguredLock => (float)LockField.GetValue(ConfigField.GetValue(Module));

		internal State(object module)
		{
			Module = module;
			Path = System.IO.Path.Combine(((ModuleBase)module).ConfigDirectoryPath, "AnimationLockOptions.json");
			ConfigField = module.GetType().GetField("config", BindingFlags.Instance | BindingFlags.NonPublic);
			LockField = ConfigField.FieldType.GetField("AnimationLock");
			Options = new Options();
			if (File.Exists(Path))
			{
				try
				{
					Options = JsonSerializer.Deserialize<Options>(File.ReadAllText(Path)) ?? new Options();
				}
				catch (JsonException)
				{
				}
			}
		}

		internal void Save()
		{
			Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));
			string text = Path + "." + Guid.NewGuid().ToString("N") + ".tmp";
			try
			{
				using (FileStream fileStream = new FileStream(text, FileMode.CreateNew, FileAccess.Write, FileShare.None))
				{
					JsonSerializer.Serialize(fileStream, Options, new JsonSerializerOptions
					{
						WriteIndented = true
					});
					fileStream.Flush(flushToDisk: true);
				}
				File.Move(text, Path, overwrite: true);
			}
			finally
			{
				if (File.Exists(text))
				{
					File.Delete(text);
				}
			}
		}

		internal void SetLimitBreakEnabled()
		{
			if (Options.RemoveLBlock)
			{
				if (LimitBreakHook == null)
				{
					LimitBreakHook = new CompSig("48 ?? ?? ?? 48 ?? ?? ?? 45 ?? ?? 33 ?? E8 ?? ?? ?? ?? 84 ?? 74").GetHook<LimitBreakDelegate>(KeepInput);
				}
				LimitBreakHook.Enable();
			}
			else
			{
				LimitBreakHook?.Disable();
			}
		}

		public void Dispose()
		{
			if (!Disposed)
			{
				LimitBreakHook?.Dispose();
				LimitBreakHook = null;
				Disposed = true;
			}
		}
	}

	private const string LimitBreakSignature = "48 ?? ?? ?? 48 ?? ?? ?? 45 ?? ?? 33 ?? E8 ?? ?? ?? ?? 84 ?? 74";

	private static readonly ConditionalWeakTable<object, State> States = new ConditionalWeakTable<object, State>();

	private static readonly FieldInfo CleanupField = typeof(ModuleBase).GetField("cleanupAction", BindingFlags.Instance | BindingFlags.NonPublic);

	private static bool ConfigViewerOpen;

	private static bool ConfigViewerRegistered;

	private static string ConfigViewerRoot = "";

	private static string[] ConfigViewerFiles = Array.Empty<string>();

	private static int ConfigViewerSelection = -1;

	private static string ConfigViewerText = "";

	private static string ConfigViewerError = "";

	private static nint KeepInput(nint input)
	{
		return input;
	}

	public static void Initialize(object module)
	{
		if (States.TryGetValue(module, out State value))
		{
			value.Dispose();
			CleanupField.SetValue(module, value.OriginalCleanup);
			States.Remove(module);
		}
		State state = new State(module);
		States.Add(module, state);
		state.SetLimitBreakEnabled();
		Action<ModuleBase> original = (Action<ModuleBase>)CleanupField.GetValue(module);
		state.OriginalCleanup = original;
		Action<ModuleBase> value2 = delegate(ModuleBase owner)
		{
			try
			{
				original?.Invoke(owner);
			}
			finally
			{
				state.Dispose();
			}
		};
		CleanupField.SetValue(module, value2);
	}

	public static void Draw(object module)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		if (!States.TryGetValue(module, out State value) || value.Disposed)
		{
			return;
		}
		bool advanceMode = value.Options.AdvanceMode;
		if (ImGui.Checkbox(ImU8String.op_Implicit("Effective on food / potion / movement action"), ref advanceMode))
		{
			value.Options.AdvanceMode = advanceMode;
			if (value.HasLocalAction)
			{
				value.Eligible = IsEligible(value.LastActionId, value.LastActionType, advanceMode);
			}
			value.Save();
		}
		bool removeLBlock = value.Options.RemoveLBlock;
		if (ImGui.Checkbox(ImU8String.op_Implicit("Removed Limit Break position lock"), ref removeLBlock))
		{
			bool removeLBlock2 = value.Options.RemoveLBlock;
			value.Options.RemoveLBlock = removeLBlock;
			try
			{
				value.SetLimitBreakEnabled();
			}
			catch
			{
				value.Options.RemoveLBlock = removeLBlock2;
				throw;
			}
			value.Save();
		}
	}

	public static float Clamp(object module, float current)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		ClassJob classJobData = LocalPlayerState.ClassJobData;
		if (((ClassJob)(ref classJobData)).DohDolJobIndex != -1)
		{
			return MathF.Min(current, 400f);
		}
		if (!States.TryGetValue(module, out State value) || value.Disposed || !value.Eligible)
		{
			return current;
		}
		return MathF.Min(current, value.ConfiguredLock);
	}

	public unsafe static void AfterPacket(object module, uint sourceId, nint header)
	{
		if (header == 0 || !States.TryGetValue(module, out State value) || value.Disposed)
		{
			return;
		}
		IPlayerCharacter localPlayer = DalamudServiceExtension.Instance<IObjectTable>().LocalPlayer;
		if (localPlayer == null || ((IGameObject)localPlayer).EntityId != sourceId)
		{
			return;
		}
		value.HasLocalAction = true;
		value.LastActionId = ((Header)header).ActionId;
		value.LastActionType = ((Header)header).ActionType;
		value.Eligible = IsEligible(((Header)header).ActionId, ((Header)header).ActionType, value.Options.AdvanceMode);
		if (value.Eligible && !(((Header)header).AnimationLock <= value.ConfiguredLock))
		{
			ActionManager* ptr = ActionManager.Instance();
			if (ptr != null)
			{
				((ActionManager)ptr).AnimationLock = value.ConfiguredLock;
			}
		}
	}

	private static bool IsEligible(uint actionId, byte actionType, bool advanced)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (actionId - 7 <= 1)
		{
			return false;
		}
		switch (actionType)
		{
		case 2:
			return advanced;
		default:
			return false;
		case 1:
		{
			Action? rowOrDefault = DalamudServiceExtension.Instance<IDataManager>().GetExcelSheet<Action>((ClientLanguage?)null, (string)null).GetRowOrDefault(actionId);
			if (!rowOrDefault.HasValue)
			{
				return false;
			}
			Action value = rowOrDefault.Value;
			uint rowId = ((Action)(ref value)).ActionCategory.RowId;
			bool flag = ((rowId == 9 || rowId == 15) ? true : false);
			return !flag && (!((Action)(ref value)).AffectsPosition || advanced);
		}
		}
	}

	public static void OpenConfigDirectory(string directory)
	{
		ConfigViewerError = "";
		ConfigViewerSelection = -1;
		ConfigViewerFiles = Array.Empty<string>();
		ConfigViewerText = "";
		try
		{
			ConfigViewerRoot = Path.GetFullPath(directory);
			RequireLocalFileSystemPath(ConfigViewerRoot);
			if (!Directory.Exists(ConfigViewerRoot))
			{
				throw new IOException("Configuration folder does not exist.");
			}
			List<string> list = new List<string>();
			Stack<string> stack = new Stack<string>();
			stack.Push(ConfigViewerRoot);
			while (stack.Count > 0 && list.Count < 4096)
			{
				string path = stack.Pop();
				RequireLocalFileSystemPath(path);
				foreach (string item in Directory.EnumerateFileSystemEntries(path))
				{
					if ((File.GetAttributes(item) & FileAttributes.ReparsePoint) == 0)
					{
						RequireLocalFileSystemPath(item);
						if (Directory.Exists(item))
						{
							stack.Push(item);
						}
						else if (Path.GetExtension(item).Equals(".json", StringComparison.OrdinalIgnoreCase))
						{
							list.Add(item);
						}
						if (list.Count >= 4096)
						{
							break;
						}
					}
				}
			}
			ConfigViewerFiles = list.OrderBy<string, string>((string p) => p, StringComparer.OrdinalIgnoreCase).ToArray();
			ConfigViewerSelection = -1;
			ConfigViewerText = "";
		}
		catch (Exception ex)
		{
			ConfigViewerFiles = Array.Empty<string>();
			ConfigViewerSelection = -1;
			ConfigViewerText = "";
			ConfigViewerError = ex.Message;
		}
		ConfigViewerOpen = true;
		if (!ConfigViewerRegistered)
		{
			DalamudLeftServiceExtension.Instance().Draw += DrawConfigViewer;
			ConfigViewerRegistered = true;
		}
	}

	private static void RequireLocalFileSystemPath(string path)
	{
		string fullPath = Path.GetFullPath(path);
		if (fullPath.Length < 3 || !char.IsAsciiLetter(fullPath[0]) || fullPath[1] != ':' || fullPath[2] != '\\')
		{
			throw new IOException("Only a local drive path is allowed.");
		}
		string pathRoot = Path.GetPathRoot(fullPath);
		if (new DriveInfo(pathRoot).DriveType != DriveType.Fixed)
		{
			throw new IOException("Network and non-local drives are blocked.");
		}
		string text = pathRoot;
		if ((File.GetAttributes(text) & FileAttributes.ReparsePoint) != FileAttributes.None)
		{
			throw new IOException("Filesystem links and junctions are blocked.");
		}
		string[] array = fullPath.Substring(pathRoot.Length).Split('\\', StringSplitOptions.RemoveEmptyEntries);
		foreach (string path2 in array)
		{
			text = Path.Combine(text, path2);
			if ((File.GetAttributes(text) & FileAttributes.ReparsePoint) != FileAttributes.None)
			{
				throw new IOException("Filesystem links and junctions are blocked.");
			}
		}
	}

	public static void ShutdownConfigViewer()
	{
		try
		{
			if (ConfigViewerRegistered)
			{
				DalamudLeftServiceExtension.Instance().Draw -= DrawConfigViewer;
			}
		}
		catch (Exception ex)
		{
			DLog.Error("Failed to close the local module config viewer.", ex);
		}
		finally
		{
			ConfigViewerRegistered = false;
			ConfigViewerOpen = false;
			ConfigViewerRoot = "";
			ConfigViewerFiles = Array.Empty<string>();
			ConfigViewerSelection = -1;
			ConfigViewerText = "";
			ConfigViewerError = "";
		}
	}

	private static void DrawConfigViewer()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ImGui.SetNextWindowSize(new Vector2(760f, 520f), (ImGuiCond)4);
			bool flag = ImGui.Begin(ImU8String.op_Implicit("Module Config###DailyRoutinesLocalConfigViewer"), ref ConfigViewerOpen, (ImGuiWindowFlags)0);
			try
			{
				if (!flag)
				{
					return;
				}
				ImGui.TextWrapped(ImU8String.op_Implicit(ConfigViewerRoot));
				if (ConfigViewerError.Length > 0)
				{
					ImGui.TextWrapped(ImU8String.op_Implicit(ConfigViewerError));
				}
				string text = ((ConfigViewerSelection >= 0 && ConfigViewerSelection < ConfigViewerFiles.Length) ? Path.GetRelativePath(ConfigViewerRoot, ConfigViewerFiles[ConfigViewerSelection]) : "Select a configuration file");
				if (ImGui.BeginCombo(ImU8String.op_Implicit("File"), ImU8String.op_Implicit(text), (ImGuiComboFlags)0))
				{
					try
					{
						for (int i = 0; i < ConfigViewerFiles.Length; i++)
						{
							if (!ImGui.Selectable(ImU8String.op_Implicit(Path.GetRelativePath(ConfigViewerRoot, ConfigViewerFiles[i])), i == ConfigViewerSelection, (ImGuiSelectableFlags)0, default(Vector2)))
							{
								continue;
							}
							ConfigViewerSelection = i;
							try
							{
								RequireLocalFileSystemPath(ConfigViewerFiles[i]);
								FileInfo fileInfo = new FileInfo(ConfigViewerFiles[i]);
								if (fileInfo.Length > 1048576)
								{
									throw new IOException("File exceeds the viewer's 1 MB limit.");
								}
								ConfigViewerText = File.ReadAllText(fileInfo.FullName);
								ConfigViewerError = "";
							}
							catch (Exception ex)
							{
								ConfigViewerText = "";
								ConfigViewerError = ex.Message;
							}
						}
					}
					finally
					{
						ImGui.EndCombo();
					}
				}
				if (ConfigViewerFiles.Length == 0 && ConfigViewerError.Length == 0)
				{
					ImGui.TextUnformatted(ImU8String.op_Implicit("No JSON configuration files found."));
				}
				ImGui.Separator();
				ImGui.TextUnformatted(ImU8String.op_Implicit(ConfigViewerText));
			}
			finally
			{
				ImGui.End();
			}
		}
		finally
		{
			if (!ConfigViewerOpen && ConfigViewerRegistered)
			{
				DalamudLeftServiceExtension.Instance().Draw -= DrawConfigViewer;
				ConfigViewerRegistered = false;
			}
		}
	}
}
