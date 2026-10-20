using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.AnimationLockOptions;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using itAnDoW4mgYVlrmtMor;
using nXyRYE1uBjK6gmU4ln;

namespace DailyRoutines;

public sealed class Plugin : IAsyncDalamudPlugin, IAsyncDisposable
{
	private bool isNotAllowed;

	public static Version Version { get; private set; }

	private IFramework Framework { get; init; }

	private IDalamudPluginInterface PI { get; init; }

	public Plugin(IDalamudPluginInterface pi, IFramework framework)
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		if (false)
		{
			isNotAllowed = true;
			throw new InvalidOperationException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x204C2967 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_062a9cfe150f4dc6a9fdd7b6d00434af));
		}
		if (1 == 0)
		{
			isNotAllowed = true;
			throw new SecurityException(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x533A0C01 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
		}
		Version? version = Assembly.GetExecutingAssembly().GetName().Version;
		Version assemblyVersion = pi.Manifest.AssemblyVersion;
		if (version != new Version(assemblyVersion.Major, assemblyVersion.Minor, 0, 0))
		{
			isNotAllowed = true;
			throw new InvalidDataException("Plugin version does not match manifest.");
		}
		Version = version;
		Framework = framework;
		PI = pi;
	}

	public async Task LoadAsync(CancellationToken cancellationToken)
	{
		if (!isNotAllowed)
		{
			await Framework.RunOnTick((Func<Task>)async delegate
			{
				await Y5SWncTBsGUDX5l2wY.giQ4vv6A4(PI, Framework);
			}, default(TimeSpan), 1, cancellationToken);
		}
	}

	public async ValueTask DisposeAsync()
	{
		if (!isNotAllowed)
		{
			await Framework.RunOnTick((Func<Task>)([AsyncStateMachine(typeof(_003C_003Ec.dFlvlUtmeopsPGW7PxB))] () =>
			{
				Runtime.ShutdownConfigViewer();
				_003C_003Ec.dFlvlUtmeopsPGW7PxB stateMachine = default(_003C_003Ec.dFlvlUtmeopsPGW7PxB);
				stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder.Create();
				stateMachine._003C_003E1__state = -1;
				stateMachine._003C_003Et__builder.Start(ref stateMachine);
				return stateMachine._003C_003Et__builder.Task;
			}), default(TimeSpan), 1, default(CancellationToken));
		}
	}

	[CompilerGenerated]
	private async Task z2JWZEQGN()
	{
		await Y5SWncTBsGUDX5l2wY.giQ4vv6A4(PI, Framework);
	}
}
