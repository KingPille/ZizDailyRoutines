using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Common.Info.Models;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Manager;
using FFXIVClientStructs.FFXIV.Client.Game.Character;
using OmenTools.Dalamud.Services.Game.Object.Abstractions.ObjectKinds;
using OmenTools.Interop.Game.Lumina;
using OmenTools.Threading;
using itAnDoW4mgYVlrmtMor;
using ycTZ7OSV787Z3a0g7JT;

namespace MUl8XNCnLvHE3xrDCCw;

internal class OyqY4DfzvfDhyCNWyHy : DataUploaderBase
{
	private record cqhDGZtVX15rOdkZvDG
	{
		public ulong ContentID { get; init; }

		public string Name { get; init; }

		public uint HomeWorldID { get; init; }

		public string HomeWorldName { get; init; }

		public uint CurrentWorldID { get; init; }

		public string CurrentWorldName { get; init; }

		public string Region { get; init; }

		public cqhDGZtVX15rOdkZvDG(ulong ContentID, string Name, uint HomeWorldID, string HomeWorldName, uint CurrentWorldID, string CurrentWorldName, string Region)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			this.ContentID = ContentID;
			this.Name = Name;
			this.HomeWorldID = HomeWorldID;
			this.HomeWorldName = HomeWorldName;
			this.CurrentWorldID = CurrentWorldID;
			this.CurrentWorldName = CurrentWorldName;
			this.Region = Region;
			base._002Ector();
		}

		[CompilerGenerated]
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C0B74 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
			stringBuilder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32BC7AB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1));
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
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1E24FCE3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_86afda88005749b094dac2a7d6b27c25));
			builder.Append(ContentID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2816E920 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd));
			builder.Append((object?)Name);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAD0A3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503));
			builder.Append(HomeWorldID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7D9C9A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571));
			builder.Append((object?)HomeWorldName);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C0BF4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32));
			builder.Append(CurrentWorldID.ToString());
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x75EEABFF ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_11422b9d07f9498fb995b7dd8e417b5b));
			builder.Append((object?)CurrentWorldName);
			builder.Append(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3A440 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1));
			builder.Append((object?)Region);
			return true;
		}

		[CompilerGenerated]
		protected cqhDGZtVX15rOdkZvDG(cqhDGZtVX15rOdkZvDG original)
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
			ContentID = original.ContentID;
			Name = original.Name;
			HomeWorldID = original.HomeWorldID;
			HomeWorldName = original.HomeWorldName;
			CurrentWorldID = original.CurrentWorldID;
			CurrentWorldName = original.CurrentWorldName;
			Region = original.Region;
		}

		[CompilerGenerated]
		public void Wnut9Bllai(out ulong ContentID, out string Name, out uint HomeWorldID, out string HomeWorldName, out uint CurrentWorldID, out string CurrentWorldName, out string Region)
		{
			ContentID = this.ContentID;
			Name = this.Name;
			HomeWorldID = this.HomeWorldID;
			HomeWorldName = this.HomeWorldName;
			CurrentWorldID = this.CurrentWorldID;
			CurrentWorldName = this.CurrentWorldName;
			Region = this.Region;
		}
	}

	private readonly Throttler<ulong> playerUploadThrottler;

	private readonly SemaphoreSlim uploadSemaphore;

	private readonly System.Timers.Timer flushTimer;

	private readonly ConcurrentDictionary<ulong, byte> existingContentIDs;

	private readonly ConcurrentQueue<cqhDGZtVX15rOdkZvDG> uploadQueue;

	protected override Task Init()
	{
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround += ij4CLI0k59;
		ManagerBase<PlayersManager>.Instance().ReceivePlayerInfo += ehACH1S08w;
		flushTimer.Elapsed += VavCyvG1aw;
		flushTimer.Start();
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		ManagerBase<PlayersManager>.Instance().ReceivePlayersAround -= ij4CLI0k59;
		ManagerBase<PlayersManager>.Instance().ReceivePlayerInfo -= ehACH1S08w;
		flushTimer.Elapsed -= VavCyvG1aw;
		flushTimer.Stop();
		return Task.CompletedTask;
	}

	protected override Task Uninit()
	{
		flushTimer.Dispose();
		uploadSemaphore.Dispose();
		existingContentIDs.Clear();
		uploadQueue.Clear();
		return Task.CompletedTask;
	}

	private void VavCyvG1aw(object? sender, ElapsedEventArgs args)
	{
		RunBackground(vCECVD3YjT);
	}

	private unsafe void ij4CLI0k59(IReadOnlyList<IPlayerCharacter> characters)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		foreach (IPlayerCharacter character in characters)
		{
			WHQCSr5ljD(((BattleChara)character.ToStruct()).ContentId, character.Name, character.HomeWorld.RowId, character.CurrentWorld.RowId);
		}
	}

	private void ehACH1S08w(PlayerInfo data)
	{
		WHQCSr5ljD(data.ContentID, data.Name, data.HomeWorld, data.CurrentWorld);
	}

	private void WHQCSr5ljD(ulong contentID, string name, uint homeWorldID, uint currentWorldID)
	{
		try
		{
			if (contentID == 0L || !MTCCfTo6e8(contentID) || string.IsNullOrWhiteSpace(name))
			{
				return;
			}
			string text = wC9CCPe4lb(homeWorldID);
			string text2 = wC9CCPe4lb(currentWorldID);
			if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(text2) || text != text2)
			{
				return;
			}
			string worldName = LuminaWrapper.GetWorldName(homeWorldID);
			string worldName2 = LuminaWrapper.GetWorldName(currentWorldID);
			if (!string.IsNullOrWhiteSpace(worldName) && !string.IsNullOrWhiteSpace(worldName2) && existingContentIDs.TryAdd(contentID, 0))
			{
				uploadQueue.Enqueue(new cqhDGZtVX15rOdkZvDG(contentID, name, homeWorldID, worldName, currentWorldID, worldName2, text));
				if (uploadQueue.Count >= 100)
				{
					RunBackground(vCECVD3YjT);
				}
			}
		}
		catch
		{
			existingContentIDs.TryRemove(contentID, out var _);
		}
	}

	private async Task vCECVD3YjT(CancellationToken cancellationToken)
	{
		if (!(await uploadSemaphore.WaitAsync(0, cancellationToken).ConfigureAwait(continueOnCapturedContext: false)))
		{
			return;
		}
		try
		{
			await pdhC9ZVfxn(cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		finally
		{
			uploadSemaphore.Release();
		}
	}

	private async Task pdhC9ZVfxn(CancellationToken cancellationToken)
	{
		List<cqhDGZtVX15rOdkZvDG> batch = new List<cqhDGZtVX15rOdkZvDG>(200);
		try
		{
			cqhDGZtVX15rOdkZvDG result;
			while (batch.Count < 200 && uploadQueue.TryDequeue(out result))
			{
				batch.Add(result);
			}
			if (batch.Count == 0)
			{
				return;
			}
			var list = batch.Select((cqhDGZtVX15rOdkZvDG data) => new
			{
				content_id = data.ContentID.ToString(),
				name = data.Name.ToString(),
				world = data.HomeWorldName.ToString(),
				world_id = data.HomeWorldID.ToString(),
				current_world = data.CurrentWorldName.ToString(),
				current_world_id = data.CurrentWorldID.ToString(),
				region = data.Region.ToString()
			}).ToList();
			await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.ReportCID, new { list }, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			foreach (cqhDGZtVX15rOdkZvDG item in batch)
			{
				existingContentIDs.TryRemove(item.ContentID, out var _);
			}
		}
		catch
		{
		}
	}

	private bool MTCCfTo6e8(ulong contentID)
	{
		return playerUploadThrottler.Throttle(contentID, 900000u);
	}

	private static string wC9CCPe4lb(uint worldID)
	{
		if (worldID < 4000)
		{
			if (worldID < 2000)
			{
				if (worldID < 1000)
				{
					if (worldID == 0)
					{
						return string.Empty;
					}
					return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x536861E0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94);
				}
				return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A9FF2D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a);
			}
			if (worldID < 3000)
			{
				return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51331638 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b);
			}
			return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x6333BC1B ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2);
		}
		if (worldID < 5000)
		{
			return GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E4613B5 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e);
		}
		return string.Empty;
	}

	public OyqY4DfzvfDhyCNWyHy()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		playerUploadThrottler = new Throttler<ulong>();
		uploadSemaphore = new SemaphoreSlim(1);
		flushTimer = new System.Timers.Timer(60000.0);
		existingContentIDs = new ConcurrentDictionary<ulong, byte>();
		uploadQueue = new ConcurrentQueue<cqhDGZtVX15rOdkZvDG>();
		base._002Ector();
	}
}
