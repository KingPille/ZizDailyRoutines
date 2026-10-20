using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Extensions;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Lumina.Text.ReadOnly;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.ImGuiOm;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using OmenTools.Threading.TaskHelper;
using OmenTools.Threading.TaskHelper.Enums;

namespace DailyRoutines.ModulesPublic;

public class AutoBackupGameConfig : ModuleBase
{
	private class GistFile
	{
		[JsonProperty("raw_url")]
		public string? RawURL { get; set; }

		public string? Content { get; set; }
	}

	private class Gist
	{
		public string? ID { get; set; }

		public string? Description { get; set; }

		public Dictionary<string, GistFile>? Files { get; set; }
	}

	private class GistManifest
	{
		public Dictionary<string, string> Files = new Dictionary<string, string>();

		public string Machine = string.Empty;
	}

	private class Config : ModuleConfig
	{
		public string DataFolderPath = string.Empty;

		public string Token = string.Empty;

		public string GistID = string.Empty;

		public DateTimeOffset LastBackupTime = DateTimeOffset.MinValue;
	}

	private Config config;

	private HttpClient httpClient;

	private bool isConflictPending;

	private bool isPopupOpen;

	private const string GIST_API = "hxxps://api.github.com/gists";

	private const string GIST_DESCRIPTION = "FFXIV Game Config Backup (DailyRoutines)";

	private const string USER_AGENT = "DailyRoutines-AutoBackupGameConfig";

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoBackupGameConfigTitle"),
		Description = LanguageManager.Get("AutoBackupGameConfigDescription"),
		Category = ModuleCategory.System
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		base.TaskHelper = new TaskHelper
		{
			TimeoutMS = 300000
		};
		httpClient = OmenServiceBase<HTTPClientHelper>.Instance().Get("AutoBackupGameConfig.Gist");
		OmenServiceBase<WindowManager>.Instance().PostDraw += OnDraw;
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
	}

	protected override void Uninit()
	{
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		OmenServiceBase<WindowManager>.Instance().PostDraw -= OnDraw;
	}

	protected override void ConfigUI()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		using (ImRaiiExtension.Heading1(LanguageManager.Get("AutoBackupGameConfig-DataFolderPath")))
		{
			ImGui.SetNextItemWidth(400f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###AutoBackupGameConfig-DataFolderInput"), ref config.DataFolderPath, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoBackupGameConfig-DataFolderHelp"), 40f, (FontAwesomeIcon)61530);
			bool flag = IsDataFolderValid();
			ImGui.TextColored(flag ? KnownColor.LawnGreen.ToUInt() : KnownColor.Red.ToUInt(), ImU8String.op_Implicit(flag ? LanguageManager.Get("Valid") : LanguageManager.Get("Invalid")));
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1("GitHub Personal Acess Token (Classic)"))
		{
			ImGui.SetNextItemWidth(400f * Globals.GlobalUIScale);
			ImGui.InputText(ImU8String.op_Implicit("###AutoBackupGameConfig-TokenInput"), ref config.Token, 128, (ImGuiInputTextFlags)32768, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
			ImGuiOm.HelpMarker(LanguageManager.Get("AutoBackupGameConfig-TokenHelp"), 40f, (FontAwesomeIcon)61530);
		}
		ImGui.NewLine();
		using (ImRaiiExtension.Heading1(LanguageManager.Get("AutoBackupGameConfig-ManualOperate")))
		{
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoBackupGameConfig-Backup")), default(Vector2)))
			{
				EnqueueUpload();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoBackupGameConfig-Restore")), default(Vector2)))
			{
				EnqueueRestore();
			}
		}
		if (!string.IsNullOrEmpty(config.GistID))
		{
			ImGui.NewLine();
			ImU8String val = default(ImU8String);
			((ImU8String)(ref val))._002Ector(8, 1);
			((ImU8String)(ref val)).AppendLiteral("Gist ID：");
			((ImU8String)(ref val)).AppendFormatted<string>(config.GistID);
			ImGui.TextDisabled(val);
			if (config.LastBackupTime != DateTimeOffset.MinValue)
			{
				ImU8String val2 = default(ImU8String);
				((ImU8String)(ref val2))._002Ector(1, 2);
				((ImU8String)(ref val2)).AppendFormatted<string>(LanguageManager.Get("AutoBackupGameConfig-LastBackupTime"));
				((ImU8String)(ref val2)).AppendLiteral("：");
				((ImU8String)(ref val2)).AppendFormatted<DateTimeOffset>(config.LastBackupTime.ToLocalTime());
				ImGui.TextDisabled(val2);
			}
		}
	}

	private void OnDraw()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		if (isConflictPending)
		{
			isPopupOpen = true;
			isConflictPending = false;
		}
		using ImGuiOm.ModalPopupDisposable modalPopupDisposable = ImGuiOm.PopupModal(LanguageManager.Get("AutoBackupGameConfig-Conflict-Title"), ref isPopupOpen, (ImGuiWindowFlags)0);
		if (!(!modalPopupDisposable))
		{
			ImGui.TextWrapped(ImU8String.op_Implicit(LanguageManager.Get("AutoBackupGameConfig-Conflict-Message")));
			ImGui.Spacing();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoBackupGameConfig-Backup")), default(Vector2)))
			{
				isPopupOpen = false;
				EnqueueUpload();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("AutoBackupGameConfig-Restore")), default(Vector2)))
			{
				isPopupOpen = false;
				EnqueueRestore();
			}
			ImGui.SameLine();
			if (ImGui.Button(ImU8String.op_Implicit(LanguageManager.Get("Cancel")), default(Vector2)))
			{
				isPopupOpen = false;
			}
		}
	}

	private void OnLogin()
	{
		base.TaskHelper.Abort();
		base.TaskHelper.EnqueueAsync((Func<CancellationToken, Task>)CheckAndBackupAsync, "登录后备份检查", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void EnqueueUpload()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string text = LanguageManager.Get("AutoBackupGameConfig-Notification-StartOperation");
		NotifyHelper.Toast(text);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
		base.TaskHelper.Abort();
		base.TaskHelper.EnqueueAsync((Func<CancellationToken, Task>)UploadAsync, "上传覆盖云端", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private void EnqueueRestore()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		string text = LanguageManager.Get("AutoBackupGameConfig-Notification-StartOperation");
		NotifyHelper.Toast(text);
		OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
		base.TaskHelper.Abort();
		base.TaskHelper.EnqueueAsync((Func<CancellationToken, Task>)RestoreAsync, "从云端恢复", 0, (TaskAbortBehaviour?)null, (TaskAbortBehaviour?)null, (Action?)null, (Action?)null, 0);
	}

	private async Task<string?> EnsureGistIDAsync(CancellationToken ct)
	{
		if (!string.IsNullOrEmpty(config.GistID))
		{
			return config.GistID;
		}
		string text = await FindGistAsync(ct);
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		config.GistID = text;
		config.Save(this);
		return text;
	}

	private async Task CheckAndBackupAsync(CancellationToken ct)
	{
		_ = 4;
		try
		{
			if (!IsDataFolderValid() || string.IsNullOrWhiteSpace(config.Token))
			{
				return;
			}
			Dictionary<string, string> localManifest = BuildManifest(config.DataFolderPath);
			if (localManifest.Count == 0)
			{
				return;
			}
			if (await EnsureGistIDAsync(ct) == null)
			{
				await UploadAsync(ct);
				return;
			}
			GistManifest gistManifest = await DownloadManifestAsync(ct);
			int diffCount;
			if (gistManifest == null)
			{
				await UploadAsync(ct);
			}
			else if (RequiresDecision(localManifest, gistManifest, Environment.MachineName, out diffCount))
			{
				isConflictPending = true;
			}
			else if (diffCount > 0)
			{
				await UploadAsync(ct);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			HandleError("登录后备份检查失败", ex2);
		}
	}

	private async Task UploadAsync(CancellationToken ct)
	{
		try
		{
			if (IsDataFolderValid())
			{
				Dictionary<string, string> dictionary = BuildManifest(config.DataFolderPath);
				if (dictionary.Count != 0)
				{
					byte[] inArray = CreateBackupZip(config.DataFolderPath, dictionary);
					Dictionary<string, object> files = new Dictionary<string, object>
					{
						["manifest.json"] = new
						{
							content = JsonConvert.SerializeObject((object)new GistManifest
							{
								Files = dictionary,
								Machine = Environment.MachineName
							}, (Formatting)1)
						},
						["backup.zip"] = new
						{
							content = Convert.ToBase64String(inArray)
						}
					};
					string payload = JsonConvert.SerializeObject((object)new
					{
						description = GetDescription(),
						@public = false,
						files = files
					});
					await SaveGistAsync(payload, ct);
					config.LastBackupTime = OmenServiceBase<StandardTimeManager>.Instance().UTCNowOffset;
					config.Save(this);
					string text = LanguageManager.Get("AutoBackupGameConfig-Notification-UploadSuccess");
					OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
					NotifyHelper.Toast(text);
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			HandleError("备份至 GitHub Gist 失败", ex2);
		}
	}

	private async Task RestoreAsync(CancellationToken ct)
	{
		_ = 6;
		try
		{
			if (!IsDataFolderValid())
			{
				string text = LanguageManager.Get("AutoBackupGameConfig-Notification-InvalidFolder");
				OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
				NotifyHelper.ToastError(text);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(config.Token))
				{
					return;
				}
				if (await EnsureGistIDAsync(ct) == null)
				{
					string text2 = LanguageManager.Get("AutoBackupGameConfig-Notification-NoGist");
					OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text2));
					NotifyHelper.ToastError(text2);
					return;
				}
				using MemoryStream memoryStream = new MemoryStream(await DownloadBackupBytesAsync(ct));
				await using ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
				string folderFull = Path.GetFullPath(config.DataFolderPath);
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					if (entry.FullName.EndsWith('/'))
					{
						continue;
					}
					string entryPath = Path.GetFullPath(Path.Join(folderFull, entry.FullName.Replace('/', Path.DirectorySeparatorChar)));
					if (Path.GetRelativePath(folderFull, entryPath).StartsWith(".."))
					{
						continue;
					}
					Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
					await using Stream entryStream = await entry.OpenAsync(ct);
					await using FileStream fileStream = new FileStream(entryPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
					await entryStream.CopyToAsync(fileStream, ct);
				}
				string text3 = LanguageManager.Get("AutoBackupGameConfig-Notification-RestoreSuccess");
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text3));
				NotifyHelper.Toast(text3);
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex2)
		{
			HandleError("从云端恢复失败", ex2);
		}
	}

	private async Task SaveGistAsync(string payload, CancellationToken ct)
	{
		if (string.IsNullOrEmpty(config.GistID))
		{
			await CreateGistAsync(payload, ct);
			return;
		}
		HttpResponseMessage httpResponseMessage = await SendGistRequestAsync(HttpMethod.Patch, "https://api.github.com/gists/" + config.GistID, payload, ct);
		if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
		{
			config.GistID = string.Empty;
			config.Save(this);
			await CreateGistAsync(payload, ct);
		}
		else if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error", $"更新失败（HTTP {(int)httpResponseMessage.StatusCode}）"));
		}
	}

	private async Task<HttpResponseMessage> SendGistRequestAsync(HttpMethod method, string url, string? payload, CancellationToken ct)
	{
		using HttpRequestMessage request = CreateGistRequest(method, url, config.Token, payload);
		return await httpClient.SendAsync(request, ct);
	}

	private async Task CreateGistAsync(string payload, CancellationToken ct)
	{
		using HttpRequestMessage request = CreateGistRequest(HttpMethod.Post, "https://api.github.com/gists", config.Token, payload);
		using HttpResponseMessage response = await httpClient.SendAsync(request, ct);
		if (!response.IsSuccessStatusCode)
		{
			throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error", $"创建失败（HTTP {(int)response.StatusCode}）"));
		}
		Gist gist = JsonConvert.DeserializeObject<Gist>(await response.Content.ReadAsStringAsync(ct));
		config.GistID = gist?.ID ?? string.Empty;
		config.Save(this);
	}

	private async Task<GistManifest?> DownloadManifestAsync(CancellationToken ct)
	{
		Gist gist = await GetGistAsync("https://api.github.com/gists/" + config.GistID, ct);
		if (gist == null)
		{
			return null;
		}
		string text = gist.Files?["manifest.json"]?.Content;
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		return JsonConvert.DeserializeObject<GistManifest>(text);
	}

	private async Task<string?> FindGistAsync(CancellationToken ct)
	{
		int page = 1;
		while (true)
		{
			HttpResponseMessage httpResponseMessage = await SendGistRequestAsync(HttpMethod.Get, $"{"https://api.github.com/gists"}?per_page=100&page={page}", null, ct);
			if (!httpResponseMessage.IsSuccessStatusCode)
			{
				throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error", $"获取列表失败（HTTP {(int)httpResponseMessage.StatusCode}）"));
			}
			List<Gist> list = JsonConvert.DeserializeObject<List<Gist>>(await httpResponseMessage.Content.ReadAsStringAsync(ct)) ?? new List<Gist>();
			string text = list.FirstOrDefault((Gist gist) => gist.Description == GetDescription())?.ID;
			if (text != null)
			{
				return text;
			}
			if (list.Count == 0)
			{
				break;
			}
			page++;
		}
		return null;
	}

	private async Task<Gist?> GetGistAsync(string url, CancellationToken ct)
	{
		HttpResponseMessage httpResponseMessage = await SendGistRequestAsync(HttpMethod.Get, url, null, ct);
		if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
		{
			return null;
		}
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error", $"连接失败（HTTP {(int)httpResponseMessage.StatusCode}）"));
		}
		return JsonConvert.DeserializeObject<Gist>(await httpResponseMessage.Content.ReadAsStringAsync(ct));
	}

	private async Task<byte[]> DownloadBackupBytesAsync(CancellationToken ct)
	{
		string text = ((await GetGistAsync("https://api.github.com/gists/" + config.GistID, ct)) ?? throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error-NoData"))).Files?["backup.zip"]?.RawURL;
		if (string.IsNullOrEmpty(text))
		{
			throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error-NoData"));
		}
		HttpResponseMessage httpResponseMessage = await SendGistRequestAsync(HttpMethod.Get, text, null, ct);
		if (!httpResponseMessage.IsSuccessStatusCode)
		{
			throw new Exception(LanguageManager.Get("AutoBackupGameConfig-Error", $"下载失败（HTTP {(int)httpResponseMessage.StatusCode}）"));
		}
		return Convert.FromBase64String(await httpResponseMessage.Content.ReadAsStringAsync(ct));
	}

	private static HttpRequestMessage CreateGistRequest(HttpMethod method, string url, string token, string? json = null)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage(method, url);
		if (json != null)
		{
			httpRequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
		}
		httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
		httpRequestMessage.Headers.UserAgent.TryParseAdd("DailyRoutines-AutoBackupGameConfig");
		return httpRequestMessage;
	}

	private static Dictionary<string, string> BuildManifest(string folder)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.Ordinal);
		foreach (var item3 in EnumerateBackupFiles(folder))
		{
			string item = item3.RelativePath;
			string item2 = item3.FullPath;
			dictionary[item] = ComputeFileHash(item2);
		}
		return dictionary;
	}

	private static IEnumerable<(string RelativePath, string FullPath)> EnumerateBackupFiles(string folder)
	{
		foreach (string item in Directory.EnumerateFiles(folder))
		{
			string fileName = Path.GetFileName(item);
			if (!fileName.EndsWith(".old", StringComparison.OrdinalIgnoreCase))
			{
				bool flag = ((fileName == "FFXIV.cfg" || fileName == "MACROSYS.dat") ? true : false);
				if (flag || (fileName.StartsWith("FFXIV_CHARA_") && fileName.EndsWith(".dat")))
				{
					yield return (RelativePath: fileName, FullPath: item);
				}
			}
		}
		foreach (string item2 in Directory.EnumerateDirectories(folder))
		{
			if (!Path.GetFileName(item2).StartsWith("FFXIV_CHR"))
			{
				continue;
			}
			foreach (string item3 in Directory.EnumerateFiles(item2, "*", SearchOption.AllDirectories))
			{
				string relativePath = Path.GetRelativePath(folder, item3);
				if (!ContainsLogDirectory(relativePath) && !Path.GetFileName(relativePath).EndsWith(".old", StringComparison.OrdinalIgnoreCase))
				{
					yield return (RelativePath: relativePath.Replace('\\', '/'), FullPath: item3);
				}
			}
		}
	}

	private static bool ContainsLogDirectory(string relativePath)
	{
		InlineArray2<char> buffer = default(InlineArray2<char>);
		buffer[0] = Path.DirectorySeparatorChar;
		buffer[1] = Path.AltDirectorySeparatorChar;
		string[] array = relativePath.Split(buffer);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].Equals("log", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	private static string ComputeFileHash(string filePath)
	{
		using FileStream source = File.OpenRead(filePath);
		return Convert.ToHexString(SHA256.HashData(source));
	}

	private static byte[] CreateBackupZip(string folder, Dictionary<string, string> manifest)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using (ZipArchive zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create))
		{
			foreach (var (text3, _) in manifest)
			{
				using Stream destination = zipArchive.CreateEntry(text3, CompressionLevel.Optimal).Open();
				using FileStream fileStream = File.OpenRead(Path.Join(folder, text3));
				fileStream.CopyTo(destination);
			}
		}
		return memoryStream.ToArray();
	}

	private static bool RequiresDecision(Dictionary<string, string> localManifest, GistManifest cloudManifest, string currentMachine, out int diffCount)
	{
		diffCount = 0;
		bool flag = !cloudManifest.Machine.Equals(currentMachine, StringComparison.Ordinal);
		bool result = false;
		foreach (var (key, text3) in localManifest)
		{
			if (!cloudManifest.Files.TryGetValue(key, out string value) || !(value == text3))
			{
				diffCount++;
				if (flag)
				{
					result = true;
				}
			}
		}
		foreach (string key2 in cloudManifest.Files.Keys)
		{
			if (!localManifest.ContainsKey(key2))
			{
				diffCount++;
				result = true;
			}
		}
		return result;
	}

	private static void HandleError(string logMessage, Exception ex)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		DLog.Error(logMessage, ex);
		string text = LanguageManager.Get("AutoBackupGameConfig-Notification-Failed");
		OmenServiceBase<NotifyHelper>.Instance().ChatError(ReadOnlySeString.op_Implicit(text));
		NotifyHelper.ToastError(text);
	}

	private bool IsDataFolderValid()
	{
		if (!string.IsNullOrWhiteSpace(config.DataFolderPath) && Directory.Exists(config.DataFolderPath))
		{
			return File.Exists(Path.Join(config.DataFolderPath, "FFXIV.cfg"));
		}
		return false;
	}

	private static string GetDescription()
	{
		string text = "GL";
		if (GameState.IsCN)
		{
			text = "CN";
		}
		if (GameState.IsKR)
		{
			text = "KR";
		}
		if (GameState.IsTC)
		{
			text = "TC";
		}
		return "FFXIV Game Config Backup (DailyRoutines) - " + text;
	}
}
