using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Newtonsoft.Json;
using acXh6NyXo8Om6HO9yLt;
using ycTZ7OSV787Z3a0g7JT;

namespace Sem6j6yhpQ5uEW1MJxL;

internal class S3DJJ3yPZjLumXepvvO : NRm8GoyuwLfFLeEjeU6
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCheckBlacklistState_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<bool> _003C_003Et__builder;

		public string credential;

		private TaskAwaiter<string?> _003C_003Eu__1;

		private void MoveNext()
		{
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result: false);
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

	private readonly CancellationTokenSource cancelSource;

	internal bool IsBlacklisted
	{
		[CompilerGenerated]
		get
		{
			return false;
		}
		[CompilerGenerated]
		private set
		{
		}
	}

	internal bool IsPremium { get; private set; }

	internal override void Init()
	{
	}

	internal void pHWyov3R0N(bool isBlacklisted, bool isPremium)
	{
		IsBlacklisted = isBlacklisted;
		IsPremium = isPremium;
	}

	[AsyncStateMachine(typeof(_003CCheckBlacklistState_003Ed__11))]
	internal static Task<bool> AiWygRpDL0(string credential)
	{
		_003CCheckBlacklistState_003Ed__11 stateMachine = default(_003CCheckBlacklistState_003Ed__11);
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.credential = credential;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	internal async Task<bool> PjeywpIMOJ(string credential)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(PluginConfig.Instance().PremiumCredential) || string.IsNullOrWhiteSpace(credential))
			{
				return false;
			}
			return JsonConvert.DeserializeObject<bool>(await PqtKlnSSKxdRRHSb7mo.xXNSfmClyl(InternalAPIRequestType.RequestPremiumCredential, new
			{
				p_token = PluginConfig.Instance().PremiumCredential,
				p_credential = credential
			}));
		}
		catch (Exception)
		{
		}
		return false;
	}

	internal async Task ifvyE0fWeW(string credential, ulong contentID)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(credential) && contentID != 0L)
			{
				await PqtKlnSSKxdRRHSb7mo.PostAsync(InternalAPIRequestType.ReportCredentialCID, new
				{
					p_credential = credential,
					p_contentid = contentID.ToString()
				});
			}
		}
		catch (Exception)
		{
		}
	}

	internal override void Uninit()
	{
		pHWyov3R0N(isBlacklisted: false, isPremium: false);
		cancelSource.Cancel();
		cancelSource.Dispose();
	}

	public S3DJJ3yPZjLumXepvvO()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		cancelSource = new CancellationTokenSource();
		base._002Ector();
	}
}
