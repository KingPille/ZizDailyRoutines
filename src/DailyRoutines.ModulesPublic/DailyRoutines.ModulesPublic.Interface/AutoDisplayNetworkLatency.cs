using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.LocalFixes;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ISPTranslation;
using DailyRoutines.RemoteInteraction.ISPTranslation.Models.Responses;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImPlot;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Application.Network;
using FFXIVClientStructs.FFXIV.Client.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;
using FFXIVClientStructs.FFXIV.Client.System.String;
using Newtonsoft.Json;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Global;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoDisplayNetworkLatency : ModuleBase
{
	private class Config : ModuleConfig
	{
		public string Format = LanguageManager.Get("AutoDisplayNetworkLatency-DefaultFormat");
	}

	private class ServerPingMonitor : IDisposable
	{
		private readonly record struct ZoneEndpoint(string Host, ushort Port);

		[StructLayout(LayoutKind.Auto)]
		[CompilerGenerated]
		private struct _003CMeasureLatencyAsync_003Ed__46 : IAsyncStateMachine
		{
			public int _003C_003E1__state;

			public AsyncTaskMethodBuilder<long> _003C_003Et__builder;

			public ServerPingMonitor _003C_003E4__this;

			public CancellationToken cancellationToken;

			private Socket _003Csocket_003E5__2;

			private CancellationTokenSource _003CtimeoutSource_003E5__3;

			private long _003Ctimestamp_003E5__4;

			private ValueTaskAwaiter _003C_003Eu__1;

			private void MoveNext()
			{
				int num = _003C_003E1__state;
				ServerPingMonitor serverPingMonitor = _003C_003E4__this;
				long result;
				try
				{
					if (num != 0)
					{
						_003Csocket_003E5__2 = new Socket(serverPingMonitor.ServerAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					}
					try
					{
						if (num != 0)
						{
							_003CtimeoutSource_003E5__3 = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
						}
						try
						{
							if (num != 0)
							{
								_003CtimeoutSource_003E5__3.CancelAfter(TimeSpan.FromSeconds(1L));
								if (serverPingMonitor.bypassTun)
								{
									IPAddress iPAddress = TryGetPhysicalNicIP();
									if (iPAddress != null)
									{
										try
										{
											_003Csocket_003E5__2.Bind(new IPEndPoint(iPAddress, 0));
										}
										catch (SocketException)
										{
										}
									}
								}
								_003Ctimestamp_003E5__4 = Stopwatch.GetTimestamp();
							}
							try
							{
								ValueTaskAwaiter awaiter;
								if (num != 0)
								{
									awaiter = _003Csocket_003E5__2.ConnectAsync(new IPEndPoint(serverPingMonitor.ServerAddress, serverPingMonitor.ServerPort), _003CtimeoutSource_003E5__3.Token).GetAwaiter();
									if (!awaiter.IsCompleted)
									{
										num = (_003C_003E1__state = 0);
										_003C_003Eu__1 = awaiter;
										_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
										return;
									}
								}
								else
								{
									awaiter = _003C_003Eu__1;
									_003C_003Eu__1 = default(ValueTaskAwaiter);
									num = (_003C_003E1__state = -1);
								}
								awaiter.GetResult();
								long num2 = (long)Math.Round(Stopwatch.GetElapsedTime(_003Ctimestamp_003E5__4).TotalMilliseconds);
								if (!serverPingMonitor.bypassTun && !serverPingMonitor.bypassTunAttempted && num2 > 0 && num2 < 5)
								{
									serverPingMonitor.consecutiveLowPings++;
									if (serverPingMonitor.consecutiveLowPings >= 5 && TryGetPhysicalNicIP() != null)
									{
										serverPingMonitor.bypassTun = true;
									}
								}
								else if (num2 > 20)
								{
									serverPingMonitor.bypassTunAttempted = true;
								}
								result = num2;
							}
							catch (OperationCanceledException)
							{
								result = -1L;
							}
							catch (SocketException)
							{
								result = -1L;
							}
						}
						finally
						{
							if (num < 0 && _003CtimeoutSource_003E5__3 != null)
							{
								((IDisposable)_003CtimeoutSource_003E5__3).Dispose();
							}
						}
					}
					finally
					{
						if (num < 0 && _003Csocket_003E5__2 != null)
						{
							((IDisposable)_003Csocket_003E5__2).Dispose();
						}
					}
				}
				catch (Exception exception)
				{
					_003C_003E1__state = -2;
					_003Csocket_003E5__2 = null;
					_003CtimeoutSource_003E5__3 = null;
					_003C_003Et__builder.SetException(exception);
					return;
				}
				_003C_003E1__state = -2;
				_003Csocket_003E5__2 = null;
				_003CtimeoutSource_003E5__3 = null;
				_003C_003Et__builder.SetResult(result);
			}

			void IAsyncStateMachine.MoveNext()
			{
				this.MoveNext();
			}

			[DebuggerHidden]
			private void SetStateMachine(IAsyncStateMachine stateMachine)
			{
				_003C_003Et__builder.SetStateMachine(stateMachine);
			}

			void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
			{
				this.SetStateMachine(stateMachine);
			}
		}

		private const string TARGET_IP_QUERY_API = "hxxp://ip-api.com/json/{0}?lang={1}";

		private const int TUN_DETECTION_THRESHOLD_MS = 5;

		private const int TUN_DETECTION_CONSECUTIVE_COUNT = 5;

		private CancellationTokenSource? ipInfoCancelSource;

		private ZoneEndpoint? endpoint;

		private bool bypassTun;

		private int consecutiveLowPings;

		private bool bypassTunAttempted;

		private static IPAddress? CachedPhysicalNicIP;

		private static bool PhysicalNicResolved;

		public IPAddress ServerAddress { get; private set; } = IPAddress.Loopback;

		public ushort ServerPort { get; private set; }

		public IPLocationDTO? AddressInfo { get; private set; }

		public ISPTranslatorResponse? ISPInfo { get; private set; }

		public long LastPing { get; private set; } = -1L;

		public float[] History { get; } = new float[100];

		public int HistoryIndex { get; private set; }

		public int FilledCount { get; private set; }

		public void Dispose()
		{
			ipInfoCancelSource?.Cancel();
			ipInfoCancelSource?.Dispose();
		}

		public async Task UpdateAsync(CancellationToken cancellationToken)
		{
			_ = 1;
			try
			{
				if (await UpdateServerEndpointAsync(cancellationToken))
				{
					bypassTun = false;
					consecutiveLowPings = 0;
					bypassTunAttempted = false;
					if (ServerPort != 0)
					{
						RefreshIPInfo(ServerAddress);
					}
					else
					{
						ResetAddressInfo();
					}
				}
				long lastPing = ((ServerPort != 0) ? (await MeasureLatencyAsync(cancellationToken)) : (-1));
				LastPing = lastPing;
			}
			catch (Exception)
			{
				LastPing = -1L;
			}
			History[HistoryIndex] = ((LastPing == -1) ? 0 : LastPing);
			HistoryIndex = (HistoryIndex + 1) % History.Length;
			if (FilledCount < History.Length)
			{
				FilledCount++;
			}
		}

		private async Task<bool> UpdateServerEndpointAsync(CancellationToken cancellationToken)
		{
			ZoneEndpoint? zoneEndpoint = await GetZoneEndpointAsync(cancellationToken);
			if (!zoneEndpoint.HasValue || !IPAddress.TryParse(zoneEndpoint.Value.Host, out IPAddress address))
			{
				return ResetServerEndpoint();
			}
			if (endpoint == zoneEndpoint)
			{
				return false;
			}
			bool result = !address.Equals(ServerAddress) || zoneEndpoint.Value.Port != ServerPort;
			endpoint = zoneEndpoint;
			ServerAddress = address;
			ServerPort = zoneEndpoint.Value.Port;
			return result;
		}

		private unsafe static ZoneEndpoint? GetZoneEndpoint()
		{
			Framework* ptr = Framework.Instance();
			if (ptr == null)
			{
				return null;
			}
			NetworkModuleProxy* networkModuleProxy = ((Framework)ptr).GetNetworkModuleProxy();
			if (networkModuleProxy == null || ((NetworkModuleProxy)networkModuleProxy).NetworkModule == null)
			{
				return null;
			}
			ZoneClient* ptr2 = *(ZoneClient**)((byte*)((NetworkModuleProxy)networkModuleProxy).NetworkModule + 2672);
			if (ptr2 == null)
			{
				return null;
			}
			string text = ((object)(*(Utf8String*)(&((ZoneClient)ptr2).Host))/*cast due to constrained. prefix*/).ToString();
			if (!string.IsNullOrWhiteSpace(text) && ((ZoneClient)ptr2).Port != 0)
			{
				return new ZoneEndpoint(text, ((ZoneClient)ptr2).Port);
			}
			return null;
		}

		private static async Task<ZoneEndpoint?> GetZoneEndpointAsync(CancellationToken cancellationToken)
		{
			ZoneEndpoint? endpoint = null;
			await DalamudServiceExtension.Instance<IFramework>().RunOnTick<ZoneEndpoint?>((Func<ZoneEndpoint?>)(() => endpoint = GetZoneEndpoint()), default(TimeSpan), 0, cancellationToken);
			return endpoint;
		}

		[AsyncStateMachine(typeof(_003CMeasureLatencyAsync_003Ed__46))]
		private Task<long> MeasureLatencyAsync(CancellationToken cancellationToken)
		{
			return ServerLatency.MeasureAsync(ServerAddress, cancellationToken);
		}

		private void RefreshIPInfo(IPAddress address)
		{
			ResetAddressInfo();
			if (RuntimeInformationExtension.IsWine())
			{
				return;
			}
			ipInfoCancelSource = new CancellationTokenSource();
			CancellationToken token = ipInfoCancelSource.Token;
			Task.Run(async delegate
			{
				_ = 2;
				try
				{
					HttpClient httpClient = OmenServiceBase<HTTPClientHelper>.Instance().Get();
					if (httpClient != null)
					{
						string text = await httpClient.GetStringAsync($"http://ip-api.com/json/{address}?lang={CultureInfo.CurrentUICulture}", token);
						if (!token.IsCancellationRequested)
						{
							IPLocationDTO newInfo = JsonConvert.DeserializeObject<IPLocationDTO>(text);
							if (newInfo != null && !token.IsCancellationRequested)
							{
								ISPTranslatorResponse iSPInfo = await RemoteISPTranslation.GetFreshAsync(newInfo.InternetServiceProvider, null, token);
								ISPInfo = iSPInfo;
								ISPTranslatorResponse iSPTranslatorResponse = await RemoteISPTranslation.GetFreshAsync(newInfo.CityName, null, token);
								if (iSPTranslatorResponse != null)
								{
									newInfo.CityName = iSPTranslatorResponse.Translated;
								}
								AddressInfo = newInfo;
							}
						}
					}
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception)
				{
					AddressInfo = null;
					ISPInfo = null;
				}
			}, token);
		}

		private bool ResetServerEndpoint()
		{
			bool result = !ServerAddress.Equals(IPAddress.Loopback) || ServerPort != 0;
			endpoint = null;
			ServerAddress = IPAddress.Loopback;
			ServerPort = 0;
			return result;
		}

		private static IPAddress? TryGetPhysicalNicIP()
		{
			if (PhysicalNicResolved)
			{
				return CachedPhysicalNicIP;
			}
			PhysicalNicResolved = true;
			try
			{
				CachedPhysicalNicIP = (from n in (from n in NetworkInterface.GetAllNetworkInterfaces()
						where n.OperationalStatus == OperationalStatus.Up
						select n).Where(delegate(NetworkInterface n)
					{
						NetworkInterfaceType networkInterfaceType = n.NetworkInterfaceType;
						return (networkInterfaceType == NetworkInterfaceType.Ethernet || networkInterfaceType == NetworkInterfaceType.Wireless80211 || networkInterfaceType == NetworkInterfaceType.GigabitEthernet) ? true : false;
					})
					where !IsVirtualNic(n.Description) && !IsVirtualNic(n.Name)
					select new
					{
						Nic = n,
						HasGateway = (n.GetIPProperties().GatewayAddresses.Count > 0)
					} into x
					orderby x.HasGateway descending, x.Nic.Speed descending
					select x.Nic).FirstOrDefault()?.GetIPProperties().UnicastAddresses.FirstOrDefault((UnicastIPAddressInformation a) => a.Address.AddressFamily == AddressFamily.InterNetwork)?.Address;
			}
			catch
			{
			}
			return CachedPhysicalNicIP;
		}

		private static bool IsVirtualNic(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return false;
			}
			string text2 = text.ToLowerInvariant();
			if (!text2.Contains("tun") && !text2.Contains("tap") && !text2.Contains("vpn") && !text2.Contains("virtual") && !text2.Contains("hyper-v") && !text2.Contains("wsl") && !text2.Contains("docker") && !text2.Contains("loopback") && !text2.Contains("tunnel") && !text2.Contains("pseudo") && !text2.Contains("bluetooth"))
			{
				return text2.Contains("wwan");
			}
			return true;
		}

		private void ResetAddressInfo()
		{
			ipInfoCancelSource?.Cancel();
			ipInfoCancelSource?.Dispose();
			ipInfoCancelSource = null;
			AddressInfo = null;
			ISPInfo = null;
		}
	}

	private class IPLocationDTO
	{
		[JsonProperty("country")]
		public string? CountryName { get; set; }

		[JsonProperty("city")]
		public string? CityName { get; set; }

		[JsonProperty("isp")]
		public string? InternetServiceProvider { get; set; }
	}

	private Config config;

	private ServerPingMonitor? monitor;

	private IDtrBarEntry? entry;

	private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoDisplayNetworkLatencyTitle"),
		Description = LanguageManager.Get("AutoDisplayNetworkLatencyDescription"),
		Category = ModuleCategory.Interface,
		PreviewImageURL = new string[1] { "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines/main/Resources/Modules/AutoDisplayNetworkLatency/preview-1.png" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected override void Init()
	{
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		if (monitor == null)
		{
			monitor = new ServerPingMonitor();
		}
		if (entry == null)
		{
			entry = DalamudServiceExtension.Instance<IDtrBar>().Get("DailyRoutines-AutoDisplayNetworkLatency", (SeString)null);
		}
		entry.OnClick = delegate
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (base.Overlay == null)
			{
				base.Overlay = new Overlay(this);
				Overlay? overlay = base.Overlay;
				((Window)overlay).Flags = (ImGuiWindowFlags)(((Window)overlay).Flags & -65);
				Overlay? overlay2 = base.Overlay;
				WindowSizeConstraints value = default(WindowSizeConstraints);
				((WindowSizeConstraints)(ref value))._002Ector();
				((WindowSizeConstraints)(ref value)).MinimumSize = Globals.ScaledVector2(300f, 200f);
				((Window)overlay2).SizeConstraints = value;
			}
			((Window)base.Overlay).Toggle();
		};
		Task.Run((Func<Task?>)MainLoop, cancelSource.Token);
	}

	protected override void Uninit()
	{
		cancelSource.Cancel();
		cancelSource.Dispose();
		monitor?.Dispose();
		monitor = null;
		IDtrBarEntry? obj = entry;
		if (obj != null)
		{
			obj.Remove();
		}
		entry = null;
	}

	protected override void ConfigUI()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector4 vector = KnownColor.Chocolate.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("Format")));
		IndentDisposable val = ImRaii.PushIndent(1, true);
		try
		{
			ImGui.InputText(ImU8String.op_Implicit("##FormatInput"), ref config.Format, 512, (ImGuiInputTextFlags)0, (ImGuiInputTextCallbackDelegate)null);
			if (ImGui.IsItemDeactivatedAfterEdit())
			{
				config.Save(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected unsafe override void OverlayUI()
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		if (monitor == null)
		{
			return;
		}
		if (!ImGui.IsWindowFocused((ImGuiFocusedFlags)3))
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		float num = 9999f;
		float num2 = 0f;
		float num3 = 0f;
		int num4 = 0;
		int num5 = 0;
		int filledCount = monitor.FilledCount;
		for (int i = 0; i < filledCount; i++)
		{
			float num6 = monitor.History[i];
			if (num6 <= 0.1f)
			{
				num5++;
				continue;
			}
			if (num6 < num)
			{
				num = num6;
			}
			if (num6 > num2)
			{
				num2 = num6;
			}
			num3 += num6;
			num4++;
		}
		float num7 = ((num4 > 0) ? (num3 / (float)num4) : 0f);
		float num8 = ((filledCount > 0) ? ((float)num5 / (float)filledCount) : 0f);
		if (num == 9999f)
		{
			num = 0f;
		}
		long lastPing = monitor.LastPing;
		Vector4 vector = GetPingColor(lastPing);
		using (OmenServiceBase<FontManager>.Instance().UIFont160.Push())
		{
			ImU8String val = new ImU8String(0, 1);
			((ImU8String)(ref val)).AppendFormatted<long>(lastPing);
			ImGui.TextColored(ref vector, val);
		}
		ImGui.SameLine();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit("ms"));
		ImGui.SameLine();
		string obj = $"{monitor.ServerAddress}:{monitor.ServerPort}";
		Vector2 vector2 = ImGui.CalcTextSize(ImU8String.op_Implicit(obj), false, -1f);
		float x = ImGui.GetContentRegionAvail().X;
		if (x > vector2.X)
		{
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + x - vector2.X);
		}
		ImGui.TextDisabled(ImU8String.op_Implicit(obj));
		Vector2 itemRectMax = ImGui.GetItemRectMax();
		IPLocationDTO addressInfo = monitor.AddressInfo;
		if (addressInfo != null)
		{
			using (OmenServiceBase<FontManager>.Instance().UIFont80.Push())
			{
				string text = addressInfo.CountryName + " - " + addressInfo.CityName;
				ISPTranslatorResponse iSPInfo = monitor.ISPInfo;
				if (iSPInfo != null)
				{
					text = text + " / " + iSPInfo.Translated;
				}
				if (!string.IsNullOrWhiteSpace(text))
				{
					Vector2 vector3 = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f);
					Vector2 windowPos = ImGui.GetWindowPos();
					float scrollY = ImGui.GetScrollY();
					ImGui.SetCursorPosY(itemRectMax.Y - windowPos.Y + scrollY);
					float x2 = ImGui.GetContentRegionAvail().X;
					if (x2 > vector3.X)
					{
						ImGui.SetCursorPosX(ImGui.GetCursorPosX() + x2 - vector3.X);
					}
					ImGui.TextDisabled(ImU8String.op_Implicit(text));
				}
			}
		}
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.Spacing();
		ImGui.SameLine();
		TableDisposable val2 = ImRaii.Table(ImU8String.op_Implicit("##StatsTable"), 4, (ImGuiTableFlags)24576);
		try
		{
			if (TableDisposable.op_Implicit(val2))
			{
				DrawStatColumn(LanguageManager.Get("AutoDisplayNetworkLatencyTitle-Stat-Avg"), $"{num7:F0}", GetPingColor(num7));
				DrawStatColumn(LanguageManager.Get("AutoDisplayNetworkLatencyTitle-Stat-Min"), $"{num:F0}", GetPingColor(num));
				DrawStatColumn(LanguageManager.Get("AutoDisplayNetworkLatencyTitle-Stat-Max"), $"{num2:F0}", GetPingColor(num2));
				Vector4 vector4 = ((num8 == 0f) ? KnownColor.SpringGreen.ToVector4() : ((!(num8 < 0.05f)) ? KnownColor.Red.ToVector4() : KnownColor.Orange.ToVector4()));
				Vector4 color = vector4;
				DrawStatColumn(LanguageManager.Get("AutoDisplayNetworkLatencyTitle-Stat-Loss"), $"{num8:P0}", color);
			}
		}
		finally
		{
			((TableDisposable)(ref val2)).Dispose();
		}
		PlotColorDisposable val3 = ImRaii.PushColor((ImPlotCol)16, new Vector4(0.05f), true);
		try
		{
			PlotColorDisposable val4 = ImRaii.PushColor((ImPlotCol)5, Vector4.Zero, true);
			try
			{
				PlotColorDisposable val5 = ImRaii.PushColor((ImPlotCol)14, new Vector4(1f, 1f, 1f, 0.05f), true);
				try
				{
					PlotStyleDisposable val6 = ImRaii.PushStyle((ImPlotStyleVar)4, 0.25f, true);
					try
					{
						PlotStyleDisposable val7 = ImRaii.PushStyle((ImPlotStyleVar)0, 2f, true);
						try
						{
							PlotDisposable val8 = ImRaii.Plot("##LatencyPlot", new Vector2(-1f), (ImPlotFlags)55);
							try
							{
								if (!val8)
								{
									return;
								}
								ImPlot.SetupAxes((byte*)null, (byte*)null, (ImPlotAxisFlags)9, (ImPlotAxisFlags)9);
								float num9 = MathF.Max(num2 * 1.25f, 100f);
								ImPlot.SetupAxesLimits(0.0, (double)monitor.History.Length, 0.0, (double)num9, (ImPlotCond)1);
								ImPlot.SetupAxisTicks((ImAxis)0, 0.0, (double)monitor.History.Length, 51);
								ImPlot.SetupAxisTicks((ImAxis)3, 0.0, (double)num9, 21);
								PlotColorDisposable val9 = ImRaii.PushColor((ImPlotCol)0, vector, true).Push((ImPlotCol)1, vector, true);
								try
								{
									ImPlot.PlotLine("##Ping", ref monitor.History[0], monitor.History.Length, 1.0, 0.0, (ImPlotLineFlags)16384, monitor.HistoryIndex);
								}
								finally
								{
									((IDisposable)val9)?.Dispose();
								}
								if (num7 <= 0f)
								{
									return;
								}
								Vector4 vector4 = KnownColor.White.ToVector4();
								vector4.W = 0.6f;
								Vector4 vector5 = vector4;
								val9 = ImRaii.PushColor((ImPlotCol)0, vector5, true);
								try
								{
									double[] array = new double[2]
									{
										0.0,
										monitor.History.Length
									};
									double[] array2 = new double[2] { num7, num7 };
									ImPlot.PlotLine("##Avg", ref array[0], ref array2[0], 2);
								}
								finally
								{
									((IDisposable)val9)?.Dispose();
								}
							}
							finally
							{
								((PlotDisposable)(ref val8)).Dispose();
							}
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
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
		static void DrawStatColumn(string label, string value, Vector4 vector6)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			ImGui.TableNextColumn();
			ImGui.Spacing();
			ImGui.TextDisabled(ImU8String.op_Implicit(label));
			ImGui.SameLine(0f, 8f * Globals.GlobalUIScale);
			float cursorPosY = ImGui.GetCursorPosY();
			ImGuiStylePtr style = ImGui.GetStyle();
			ImGui.SetCursorPosY(cursorPosY - ((ImGuiStylePtr)(ref style)).FramePadding.Y);
			using (OmenServiceBase<FontManager>.Instance().UIFont140.Push())
			{
				ImGui.TextColored(ref vector6, ImU8String.op_Implicit(value));
			}
		}
		static Vector4 GetPingColor(float ping)
		{
			if (ping < 0f)
			{
				return KnownColor.Gray.ToVector4();
			}
			if (ping < 100f)
			{
				return KnownColor.SpringGreen.ToVector4();
			}
			if (ping < 200f)
			{
				return KnownColor.Orange.ToVector4();
			}
			return KnownColor.Red.ToVector4();
		}
	}

	private async Task MainLoop()
	{
		try
		{
			CancellationToken cancellationToken = cancelSource.Token;
			long lastPing = -1L;
			while (!cancellationToken.IsCancellationRequested)
			{
				ServerPingMonitor currentMonitor = monitor;
				IDtrBarEntry currentEntry = entry;
				if (currentMonitor == null || currentEntry == null)
				{
					break;
				}
				if (!GameState.IsLoggedIn)
				{
					await Task.Delay(3000, cancellationToken);
					continue;
				}
				await currentMonitor.UpdateAsync(cancellationToken);
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				long currentPing = currentMonitor.LastPing;
				IPAddress address = currentMonitor.ServerAddress;
				ushort port = currentMonitor.ServerPort;
				IFramework obj = DalamudServiceExtension.Instance<IFramework>();
				Action action = delegate
				{
					//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
					if (!cancellationToken.IsCancellationRequested && entry == currentEntry)
					{
						currentEntry.Shown = true;
						if (lastPing != currentPing)
						{
							currentEntry.Text = SeString.op_Implicit(SafeFormatting.Latency(config.Format, currentPing, LanguageManager.Get("AutoDisplayNetworkLatency-DefaultFormat")));
							lastPing = currentPing;
						}
						SeStringBuilder val = new SeStringBuilder().AddIcon((BitmapFontIcon)126).AddText($"{address}:{port}");
						IPLocationDTO addressInfo = currentMonitor.AddressInfo;
						if (addressInfo != null)
						{
							val.AddText($"（{addressInfo.CountryName} - {addressInfo.CityName}）");
						}
						currentEntry.Tooltip = val.Build();
					}
				};
				CancellationToken cancellationToken2 = cancellationToken;
				await obj.RunOnTick(action, default(TimeSpan), 0, cancellationToken2);
				await Task.Delay(1000, cancellationToken);
			}
		}
		catch (OperationCanceledException) when (cancelSource.IsCancellationRequested)
		{
		}
		catch (Exception ex2)
		{
			DLog.Error("更新网络延迟时发生错误", ex2);
		}
	}
}
