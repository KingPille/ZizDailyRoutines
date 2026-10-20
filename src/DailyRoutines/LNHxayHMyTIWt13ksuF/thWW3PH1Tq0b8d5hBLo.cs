using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Helpers;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace LNHxayHMyTIWt13ksuF;

[ManagerInitOrder(16)]
internal sealed class thWW3PH1Tq0b8d5hBLo : ManagerBase<thWW3PH1Tq0b8d5hBLo>
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003C_003COnUpdate_003Eb__10_0_003Ed : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public CancellationToken cancellationToken;

		public thWW3PH1Tq0b8d5hBLo _003C_003E4__this;

		private TaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			thWW3PH1Tq0b8d5hBLo thWW3PH1Tq0b8d5hBLo2 = _003C_003E4__this;
			try
			{
				try
				{
					TaskAwaiter awaiter;
					if (num == 0)
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(TaskAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0122;
					}
					cancellationToken.ThrowIfCancellationRequested();
					object obj = PSSHvfOcJt();
					if (obj != null)
					{
						DLog.Debug(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(-1487420814 ^ -1297592740 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2));
						MethodInfo methodInfo = PluginManager?.GetType().GetMethod(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E460243 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), BindingFlags.Instance | BindingFlags.Public);
						if (methodInfo != null)
						{
							awaiter = ((Task)methodInfo.Invoke(PluginManager, new object[3] { obj, false, false })).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
								return;
							}
							goto IL_0122;
						}
					}
					goto end_IL_0018;
					IL_0122:
					awaiter.GetResult();
					end_IL_0018:;
				}
				catch (OperationCanceledException)
				{
				}
				catch (Exception ex2)
				{
					DLog.Error(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD64CD ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), ex2);
				}
				finally
				{
					if (num < 0)
					{
						Volatile.Write(ref thWW3PH1Tq0b8d5hBLo2.isUpdating, 0);
					}
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult();
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

	private int isUpdating;

	private DateTime? nextReloadTime;

	private DateTime? nextUpdateTime;

	private static object? PluginManager { get; }

	protected override Task PostInit()
	{
		return Task.CompletedTask;
	}

	protected override Task Stop()
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		OmenServiceBase<GameState>.Instance().Login -= MsiH4upwum;
		OmenServiceBase<GameState>.Instance().Logout -= UQRHslne3S;
		OmenServiceBase<FrameworkManager>.Instance().Unreg(TKrH8KBwwO);
		iuTHlCTtub();
		return Task.CompletedTask;
	}

	private void MsiH4upwum()
	{
		TKrH8KBwwO(null);
	}

	private void UQRHslne3S()
	{
		iuTHlCTtub();
	}

	private void TKrH8KBwwO(object? _)
	{
	}

	private static object? PSSHvfOcJt()
	{
		try
		{
			return (PluginManager?.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4F70EE39 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_3782f2a028bd4046aa095423cd2526e0), BindingFlags.Instance | BindingFlags.Public)?.GetValue(PluginManager) as IEnumerable)?.Cast<object>().FirstOrDefault(delegate(object update)
			{
				object obj2 = update.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5A1EE0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a))?.GetValue(update);
				string? obj3 = obj2?.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A773 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09))?.GetValue(obj2)?.ToString();
				bool valueOrDefault = obj2?.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x27BAB7A9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7f9a8e3b1ea249bebe4aa86658ee9503))?.GetValue(obj2) as bool? == true;
				object obj4 = obj2?.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A79D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09))?.GetValue(obj2);
				bool valueOrDefault2 = obj4?.GetType().GetProperty(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47335B67 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5))?.GetValue(obj4) as bool? == true;
				return obj3 == DalamudServiceExtension.Instance().InternalName && !valueOrDefault && !valueOrDefault2;
			});
		}
		catch
		{
			return null;
		}
	}

	private void iuTHlCTtub()
	{
		nextReloadTime = null;
		nextUpdateTime = null;
	}

	public thWW3PH1Tq0b8d5hBLo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static thWW3PH1Tq0b8d5hBLo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		PluginManager = DalamudReflector.GetPluginManager();
	}

	[CompilerGenerated]
	[AsyncStateMachine(typeof(_003C_003COnUpdate_003Eb__10_0_003Ed))]
	private Task H0FHxb9MeG(CancellationToken cancellationToken)
	{
		return Task.CompletedTask;
	}
}
