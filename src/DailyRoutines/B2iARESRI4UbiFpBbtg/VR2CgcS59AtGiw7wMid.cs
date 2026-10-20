using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using OmenTools.Extensions;
using itAnDoW4mgYVlrmtMor;

namespace B2iARESRI4UbiFpBbtg;

[ManagerInitOrder(1)]
internal sealed class VR2CgcS59AtGiw7wMid : ManagerBase<VR2CgcS59AtGiw7wMid>
{
	private static class AmhdKxOr0e0f3JSPFwS
	{
		public static async Task XAGOGyqIkv(string sourceDir, string destinationDir, CancellationToken cancellationToken)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDir);
			if (!directoryInfo.Exists)
			{
				return;
			}
			Directory.CreateDirectory(destinationDir);
			foreach (FileInfo item in directoryInfo.EnumerateFiles(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A24DD6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079), SearchOption.AllDirectories))
			{
				cancellationToken.ThrowIfCancellationRequested();
				string relativePath = Path.GetRelativePath(sourceDir, item.FullName);
				string text = Path.Combine(destinationDir, relativePath);
				string directoryName = Path.GetDirectoryName(text);
				if (directoryName != null)
				{
					Directory.CreateDirectory(directoryName);
				}
				await DyOO6gxvUm(item.FullName, text, cancellationToken);
			}
		}

		private static async Task DyOO6gxvUm(string sourceFile, string destinationFile, CancellationToken cancellationToken)
		{
			for (int i = 0; i < 5; i++)
			{
				try
				{
					await using FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, FileOptions.Asynchronous);
					await using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None, 81920, FileOptions.Asynchronous))
					{
						await sourceStream.CopyToAsync(destinationStream, 81920, cancellationToken);
					}
					break;
				}
				catch (IOException ex) when (ex.HResult == -2147024864)
				{
					await Task.Delay(200 * (i + 1), cancellationToken);
				}
			}
		}
	}

	private readonly CancellationTokenSource cancellationTokenSource;

	private Task? copyTask;

	private static string DevSourceFolder { get; }

	private static string DevTargetFolder
	{
		get
		{
			string text = Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1540990964 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a86a125fa3c94b2ca72995b652fdcc2e));
			Directory.CreateDirectory(text);
			return text;
		}
	}

	protected override Task Init()
	{
		if (string.IsNullOrEmpty(DevSourceFolder) || !Directory.Exists(DevSourceFolder))
		{
			return Task.CompletedTask;
		}
		copyTask = Task.Run(() => AmhdKxOr0e0f3JSPFwS.XAGOGyqIkv(DevSourceFolder, DevTargetFolder, cancellationTokenSource.Token));
		return Task.CompletedTask;
	}

	protected override async Task Stop()
	{
		await cancellationTokenSource.CancelAsync();
		if (copyTask == null)
		{
			return;
		}
		try
		{
			await copyTask;
		}
		catch (OperationCanceledException) when (cancellationTokenSource.IsCancellationRequested)
		{
		}
	}

	protected override Task Uninit()
	{
		cancellationTokenSource.Dispose();
		return Task.CompletedTask;
	}

	public VR2CgcS59AtGiw7wMid()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		cancellationTokenSource = new CancellationTokenSource();
		base._002Ector();
	}

	static VR2CgcS59AtGiw7wMid()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		DevSourceFolder = Path.GetDirectoryName(DalamudServiceExtension.Instance().AssemblyLocation.FullName) ?? string.Empty;
	}

	[CompilerGenerated]
	private Task? SxwSDyhM09()
	{
		return AmhdKxOr0e0f3JSPFwS.XAGOGyqIkv(DevSourceFolder, DevTargetFolder, cancellationTokenSource.Token);
	}
}
