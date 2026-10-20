using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;
using DailyRoutines.Manager;
using LqiFr6AUanXXiJKotr;
using Microsoft.CSharp.RuntimeBinder;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using hcXTsMLIXdeaKdE7xb7;
using itAnDoW4mgYVlrmtMor;

namespace ycTZ7OSV787Z3a0g7JT;

[ManagerInitOrder(1)]
internal sealed class PqtKlnSSKxdRRHSb7mo : ManagerBase<PqtKlnSSKxdRRHSb7mo>
{
	private enum APISourceType
	{
		CindyMaster,
		HSS,
		Haku,
		ISP,
		CECrowdsource,
		ChaoticUptime
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass7_0
	{
		public APISourceType type;

		public _003C_003Ec__DisplayClass7_0()
		{
			n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
			base._002Ector();
		}

		internal void Nu6OjwJeRh(HttpClient client)
		{
			client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39223040 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F359316 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
			client.Timeout = TimeSpan.FromSeconds(10L);
			if (ClientFactories.TryGetValue(type, out Action<HttpClient> value))
			{
				value(client);
			}
		}
	}

	[CompilerGenerated]
	private static class _003C_003Eo__8
	{
		public static CallSite<Func<CallSite, Type, object, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, Type, object, Encoding, string, StringContent>> _003C_003Ep__1;
	}

	[CompilerGenerated]
	private static class _003C_003Eo__9
	{
		public static CallSite<Func<CallSite, Type, InternalAPIRequestType, object, CancellationToken, object>> _003C_003Ep__0;

		public static CallSite<Func<CallSite, object, object, object>> _003C_003Ep__1;

		public static CallSite<Func<CallSite, object, bool>> _003C_003Ep__2;

		public static CallSite<Func<CallSite, object, object>> _003C_003Ep__3;

		public static CallSite<Func<CallSite, object, CancellationToken, object>> _003C_003Ep__4;

		public static CallSite<Func<CallSite, object, string>> _003C_003Ep__5;
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetAsync_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<HttpResponseMessage> _003C_003Et__builder;

		public InternalAPIRequestType type;

		public Dictionary<string, string> queryParams;

		public CancellationToken cancellationToken;

		private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			HttpResponseMessage result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<HttpResponseMessage>);
					num = (_003C_003E1__state = -1);
					goto IL_0134;
				}
				if (EndPointsInfo.TryGetValue(type, out (string, APISourceType) value))
				{
					HttpClient httpClient = q5gS9yZaXS(value.Item2);
					var (text, _) = value;
					if (queryParams != null && queryParams.Count > 0)
					{
						text = string.Concat(str2: string.Join(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227FA07 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44), queryParams.Select<KeyValuePair<string, string>, string>((KeyValuePair<string, string> kv) => Uri.EscapeDataString(kv.Key) + GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35B4AFDA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_042249f573d4431982cd93672ca35bd3) + Uri.EscapeDataString(kv.Value))), str0: text, str1: GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3AD8C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079));
					}
					awaiter = httpClient.GetAsync(text, cancellationToken).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0134;
				}
				result = null;
				goto end_IL_000e;
				IL_0134:
				result = awaiter.GetResult();
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CPostAsync_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<HttpResponseMessage> _003C_003Et__builder;

		public InternalAPIRequestType type;

		public object data;

		public CancellationToken cancellationToken;

		private TaskAwaiter<HttpResponseMessage> _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			HttpResponseMessage result;
			try
			{
				TaskAwaiter<HttpResponseMessage> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<HttpResponseMessage>);
					num = (_003C_003E1__state = -1);
					goto IL_01b9;
				}
				if (EndPointsInfo.TryGetValue(type, out (string, APISourceType) value))
				{
					HttpClient httpClient = q5gS9yZaXS(value.Item2);
					Type? typeFromHandle = typeof(StringContent);
					if (_003C_003Eo__8._003C_003Ep__0 == null)
					{
						_003C_003Eo__8._003C_003Ep__0 = CallSite<Func<CallSite, Type, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4CDA2F73 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_2765950fccc24d96a1162dd6779cbee1), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null)
						}));
					}
					StringContent content = (StringContent)/*Could not detect static type for DynamicInvokeConstructorInstruction*/;
					awaiter = httpClient.PostAsync(value.Item1, content, cancellationToken).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_01b9;
				}
				result = null;
				goto end_IL_000e;
				IL_01b9:
				result = awaiter.GetResult();
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CPostAsyncContent_003Ed__9 : IAsyncStateMachine
	{
		private static class DnIeZeth0hfl61KKjiB
		{
			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__0;

			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__1;

			public static CallSite<Func<CallSite, object, bool>> _003C_003Ep__2;

			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__3;

			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__4;

			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__5;

			public static CallSite<Func<CallSite, object, bool>> _003C_003Ep__6;

			public static CallSite<Func<CallSite, object, object>> _003C_003Ep__7;
		}

		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder<string> _003C_003Et__builder;

		public InternalAPIRequestType type;

		public object data;

		public CancellationToken cancellationToken;

		private object _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			string result;
			try
			{
				object obj;
				if (num != 0)
				{
					if (num == 1)
					{
						obj = _003C_003Eu__1;
						_003C_003Eu__1 = null;
						num = (_003C_003E1__state = -1);
						goto IL_05bf;
					}
					if (DnIeZeth0hfl61KKjiB._003C_003Ep__0 == null)
					{
						DnIeZeth0hfl61KKjiB._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D156AEB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, object> target = DnIeZeth0hfl61KKjiB._003C_003Ep__0.Target;
					CallSite<Func<CallSite, object, object>> _003C_003Ep__ = DnIeZeth0hfl61KKjiB._003C_003Ep__0;
					if (_003C_003Eo__9._003C_003Ep__0 == null)
					{
						_003C_003Eo__9._003C_003Ep__0 = CallSite<Func<CallSite, Type, InternalAPIRequestType, object, CancellationToken, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D156AD3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[4]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
						}));
					}
					obj = target(_003C_003Ep__, _003C_003Eo__9._003C_003Ep__0.Target(_003C_003Eo__9._003C_003Ep__0, typeof(PqtKlnSSKxdRRHSb7mo), type, data, cancellationToken));
					if (DnIeZeth0hfl61KKjiB._003C_003Ep__1 == null)
					{
						DnIeZeth0hfl61KKjiB._003C_003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DFC40 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					if (!(bool)(dynamic)DnIeZeth0hfl61KKjiB._003C_003Ep__1.Target(DnIeZeth0hfl61KKjiB._003C_003Ep__1, obj))
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = obj;
						ICriticalNotifyCompletion awaiter = obj as ICriticalNotifyCompletion;
						if (awaiter == null)
						{
							INotifyCompletion awaiter2 = (INotifyCompletion)obj;
							_003C_003Et__builder.AwaitOnCompleted(ref awaiter2, ref this);
							awaiter2 = null;
						}
						else
						{
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						}
						awaiter = null;
						return;
					}
				}
				else
				{
					obj = _003C_003Eu__1;
					_003C_003Eu__1 = null;
					num = (_003C_003E1__state = -1);
				}
				if (DnIeZeth0hfl61KKjiB._003C_003Ep__3 == null)
				{
					DnIeZeth0hfl61KKjiB._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C6B68 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				dynamic val = DnIeZeth0hfl61KKjiB._003C_003Ep__3.Target(DnIeZeth0hfl61KKjiB._003C_003Ep__3, obj);
				if (!((val == null) ? true : false))
				{
					if (DnIeZeth0hfl61KKjiB._003C_003Ep__4 == null)
					{
						DnIeZeth0hfl61KKjiB._003C_003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CBF60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					Func<CallSite, object, object> target2 = DnIeZeth0hfl61KKjiB._003C_003Ep__4.Target;
					CallSite<Func<CallSite, object, object>> _003C_003Ep__2 = DnIeZeth0hfl61KKjiB._003C_003Ep__4;
					if (_003C_003Eo__9._003C_003Ep__4 == null)
					{
						_003C_003Eo__9._003C_003Ep__4 = CallSite<Func<CallSite, object, CancellationToken, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51333F52 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[2]
						{
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
							CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
						}));
					}
					Func<CallSite, object, CancellationToken, object> target3 = _003C_003Eo__9._003C_003Ep__4.Target;
					CallSite<Func<CallSite, object, CancellationToken, object>> _003C_003Ep__3 = _003C_003Eo__9._003C_003Ep__4;
					if (_003C_003Eo__9._003C_003Ep__3 == null)
					{
						_003C_003Eo__9._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684870 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					obj = target2(_003C_003Ep__2, target3(_003C_003Ep__3, _003C_003Eo__9._003C_003Ep__3.Target(_003C_003Eo__9._003C_003Ep__3, (object)val), cancellationToken));
					if (DnIeZeth0hfl61KKjiB._003C_003Ep__5 == null)
					{
						DnIeZeth0hfl61KKjiB._003C_003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3037 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
					}
					if (!(bool)(dynamic)DnIeZeth0hfl61KKjiB._003C_003Ep__5.Target(DnIeZeth0hfl61KKjiB._003C_003Ep__5, obj))
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = obj;
						ICriticalNotifyCompletion awaiter = obj as ICriticalNotifyCompletion;
						if (awaiter == null)
						{
							INotifyCompletion awaiter2 = (INotifyCompletion)obj;
							_003C_003Et__builder.AwaitOnCompleted(ref awaiter2, ref this);
							awaiter2 = null;
						}
						else
						{
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						}
						awaiter = null;
						return;
					}
					goto IL_05bf;
				}
				result = null;
				goto end_IL_000e;
				IL_05bf:
				if (DnIeZeth0hfl61KKjiB._003C_003Ep__7 == null)
				{
					DnIeZeth0hfl61KKjiB._003C_003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x282867D1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
				}
				dynamic val2 = DnIeZeth0hfl61KKjiB._003C_003Ep__7.Target(DnIeZeth0hfl61KKjiB._003C_003Ep__7, obj);
				result = val2;
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
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

	private const string HAKU_KEY = "92d1da58-c223-4574-8051-4f9ddc9e5e41";

	private const string CINDY_MASTER_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InFtdmNmZnBla2Fjd3VneHZmcnRoIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDM4NDYwMjgsImV4cCI6MjA1OTQyMjAyOH0.tDjSQlf2THdPk4uuJ6ul_0FyCahdweMOjNAKQuUT-ys";

	private const string ISP_TRANSLATOR_KEY = "lKU4tZi8MvUB25tz9afwSkGGSWiY5jgD0pHhoNQ5soHgXsbveHEvKEEK0Oche98C";

	private const string CE_CROWDSOURCE_KEY = "cecrowd-v1-9ea81cd7bb5c4e82b19988dfed473340";

	private const string CHAOTIC_UPTIME_KEY = "Master287805831";

	private static readonly FrozenDictionary<APISourceType, Action<HttpClient>> ClientFactories;

	private static readonly FrozenDictionary<InternalAPIRequestType, (string Url, APISourceType ClientType)> EndPointsInfo;

	private static HttpClient q5gS9yZaXS(APISourceType type)
	{
		_003C_003Ec__DisplayClass7_0 CS_0024_003C_003E8__locals3 = new _003C_003Ec__DisplayClass7_0();
		CS_0024_003C_003E8__locals3.type = type;
		HTTPClientHelper hTTPClientHelper = OmenServiceBase<HTTPClientHelper>.Instance();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 1);
		defaultInterpolatedStringHandler.AppendLiteral(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6D217E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac));
		defaultInterpolatedStringHandler.AppendFormatted(CS_0024_003C_003E8__locals3.type);
		return hTTPClientHelper.Get(defaultInterpolatedStringHandler.ToStringAndClear(), delegate(HttpClient client)
		{
			client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x39223040 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F359316 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862));
			client.Timeout = TimeSpan.FromSeconds(10L);
			if (ClientFactories.TryGetValue(CS_0024_003C_003E8__locals3.type, out Action<HttpClient> value))
			{
				value(client);
			}
		});
	}

	[AsyncStateMachine(typeof(_003CPostAsync_003Ed__8))]
	internal static Task<HttpResponseMessage?> PostAsync(InternalAPIRequestType type, dynamic data, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (type != InternalAPIRequestType.RequestPlayerInfoByCID && type != InternalAPIRequestType.CreateBlacklist && type != InternalAPIRequestType.CreateSharedBlacklist && type != InternalAPIRequestType.UpdateBlacklist && type != InternalAPIRequestType.GetBlacklist && type != InternalAPIRequestType.RequestChaoticBonus && type != InternalAPIRequestType.RequestChaoticUptime && type != InternalAPIRequestType.RequestUsedNames && type != InternalAPIRequestType.GetBlacklistLogs)
		{
			throw null;
		}
		_003CPostAsync_003Ed__8 stateMachine = default(_003CPostAsync_003Ed__8);
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<HttpResponseMessage>.Create();
		stateMachine.type = type;
		stateMachine.data = data;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	internal unsafe static async Task<string?> xXNSfmClyl(InternalAPIRequestType type, dynamic data, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__0 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D156AEB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, object> target = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__0.Target;
		CallSite<Func<CallSite, object, object>> _003C_003Ep__ = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__0;
		if (_003C_003Eo__9._003C_003Ep__0 == null)
		{
			_003C_003Eo__9._003C_003Ep__0 = CallSite<Func<CallSite, Type, InternalAPIRequestType, object, CancellationToken, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D156AD3 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[4]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			}));
		}
		object obj = target(_003C_003Ep__, _003C_003Eo__9._003C_003Ep__0.Target(_003C_003Eo__9._003C_003Ep__0, typeof(PqtKlnSSKxdRRHSb7mo), type, (object)data, cancellationToken));
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__1 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3D7DFC40 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_cda76bbf50614e97b4ef38939106c571), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder = default(AsyncTaskMethodBuilder<string>);
		if (!(bool)(dynamic)_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__1.Target(_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__1, obj))
		{
			ICriticalNotifyCompletion awaiter = obj as ICriticalNotifyCompletion;
			if (awaiter == null)
			{
				INotifyCompletion awaiter2 = (INotifyCompletion)obj;
				asyncTaskMethodBuilder.AwaitOnCompleted(ref awaiter2, ref *(_003CPostAsyncContent_003Ed__9*)/*Error near IL_020b: stateMachine*/);
			}
			else
			{
				asyncTaskMethodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref *(_003CPostAsyncContent_003Ed__9*)/*Error near IL_0221: stateMachine*/);
			}
			/*Error near IL_022a: leave MoveNext - await not detected correctly*/;
		}
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__3 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x329C6B68 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_7042112693cd412186fee5da4a8f8e32), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		dynamic val = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__3.Target(_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__3, obj);
		if (val == null)
		{
			return null;
		}
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__4 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__4 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7C6CBF60 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d2f07ce64d464f30b1d7397ae7500cac), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		Func<CallSite, object, object> target2 = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__4.Target;
		CallSite<Func<CallSite, object, object>> _003C_003Ep__2 = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__4;
		if (_003C_003Eo__9._003C_003Ep__4 == null)
		{
			_003C_003Eo__9._003C_003Ep__4 = CallSite<Func<CallSite, object, CancellationToken, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51333F52 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[2]
			{
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null),
				CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, null)
			}));
		}
		Func<CallSite, object, CancellationToken, object> target3 = _003C_003Eo__9._003C_003Ep__4.Target;
		CallSite<Func<CallSite, object, CancellationToken, object>> _003C_003Ep__3 = _003C_003Eo__9._003C_003Ep__4;
		if (_003C_003Eo__9._003C_003Ep__3 == null)
		{
			_003C_003Eo__9._003C_003Ep__3 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684870 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		obj = target2(_003C_003Ep__2, target3(_003C_003Ep__3, _003C_003Eo__9._003C_003Ep__3.Target(_003C_003Eo__9._003C_003Ep__3, (object)val), cancellationToken));
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__5 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x702B3037 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ed1539c28915419fb405793dc0185c30), typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		if (!(bool)(dynamic)_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__5.Target(_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__5, obj))
		{
			ICriticalNotifyCompletion awaiter = obj as ICriticalNotifyCompletion;
			if (awaiter == null)
			{
				INotifyCompletion awaiter2 = (INotifyCompletion)obj;
				asyncTaskMethodBuilder.AwaitOnCompleted(ref awaiter2, ref *(_003CPostAsyncContent_003Ed__9*)/*Error near IL_0583: stateMachine*/);
			}
			else
			{
				asyncTaskMethodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref *(_003CPostAsyncContent_003Ed__9*)/*Error near IL_0599: stateMachine*/);
			}
			/*Error near IL_05a2: leave MoveNext - await not detected correctly*/;
		}
		if (_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__7 == null)
		{
			_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__7 = CallSite<Func<CallSite, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x282867D1 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95ac0b4adf764381bcdc75a9d241d184), null, typeof(PqtKlnSSKxdRRHSb7mo), new CSharpArgumentInfo[1] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
		}
		dynamic val2 = _003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__7.Target(_003CPostAsyncContent_003Ed__9.DnIeZeth0hfl61KKjiB._003C_003Ep__7, obj);
		return val2;
	}

	[AsyncStateMachine(typeof(_003CGetAsync_003Ed__10))]
	internal static Task<HttpResponseMessage?> s4hSCEmYC4(InternalAPIRequestType type, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (type != InternalAPIRequestType.RequestPlayerInfoByCID && type != InternalAPIRequestType.CreateBlacklist && type != InternalAPIRequestType.CreateSharedBlacklist && type != InternalAPIRequestType.UpdateBlacklist && type != InternalAPIRequestType.GetBlacklist && type != InternalAPIRequestType.RequestChaoticBonus && type != InternalAPIRequestType.RequestChaoticUptime && type != InternalAPIRequestType.RequestUsedNames && type != InternalAPIRequestType.GetBlacklistLogs)
		{
			throw null;
		}
		_003CGetAsync_003Ed__10 stateMachine = default(_003CGetAsync_003Ed__10);
		stateMachine._003C_003Et__builder = AsyncTaskMethodBuilder<HttpResponseMessage>.Create();
		stateMachine.type = type;
		stateMachine.queryParams = queryParams;
		stateMachine.cancellationToken = cancellationToken;
		stateMachine._003C_003E1__state = -1;
		stateMachine._003C_003Et__builder.Start(ref stateMachine);
		return stateMachine._003C_003Et__builder.Task;
	}

	internal static async Task<string?> ktqSjVMsCG(InternalAPIRequestType type, Dictionary<string, string>? queryParams = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		HttpResponseMessage httpResponseMessage = await s4hSCEmYC4(type, queryParams, cancellationToken);
		if (httpResponseMessage == null)
		{
			return null;
		}
		return await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
	}

	public PqtKlnSSKxdRRHSb7mo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}

	static PqtKlnSSKxdRRHSb7mo()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		ClientFactories = new Dictionary<APISourceType, Action<HttpClient>>
		{
			[APISourceType.CindyMaster] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A22066 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5379A635 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_019a6a2bbee24ddb9dd8d9a1d1e1ca09));
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FFB19E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684ECC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94));
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51333AF8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x47335931 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5f88650ee89f435aad988a8583bda5b5));
			},
			[APISourceType.HSS] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DBB297 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), ManagerBase<ILbWvcLd7xPWOTvckYC>.Instance().eVPLzA26sb<zTMwYPctmcDQxmkmbm>().Token);
			},
			[APISourceType.Haku] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3F359178 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_128c630676db4a8ab2ed83db4c2b1862), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CD601D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0));
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D1568DB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x51333D62 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc39b8b86ffd43198ab35059b444b39b));
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684A88 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), Plugin.Version.ToString());
			},
			[APISourceType.ISP] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x53684D30 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d4ce857df7b047629e7091bbc815dd94), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2F156F7 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_d964b0b1313049f490f2107d03a6ed3a));
			},
			[APISourceType.CECrowdsource] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--367535148 ^ 0x2041F319 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1F4A1B12 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_0d3e4cb7200c49c9943fa2d7c1024d32));
			},
			[APISourceType.ChaoticUptime] = delegate(HttpClient client)
			{
				client.DefaultRequestHeaders.Add(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x32A42BB ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_14783916c53948389577bd0e927537b1), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x7D156B87 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1dfb87339e314237a8eb0a6453e2a275));
			}
		}.ToFrozenDictionary();
		EndPointsInfo = new Dictionary<InternalAPIRequestType, (string, APISourceType)>
		{
			[InternalAPIRequestType.ReportLocationInfo] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC050D4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), APISourceType.HSS),
			[InternalAPIRequestType.ReportLocationInfos] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(--1317373447 ^ 0x25B47B63 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ac6250342b784829976646d28955b117), APISourceType.HSS),
			[InternalAPIRequestType.ReportModulesUsageByCID] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9072EE0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3), APISourceType.HSS),
			[InternalAPIRequestType.RequestZoneCoordinates] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3BB0C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079), APISourceType.HSS),
			[InternalAPIRequestType.GetDailyActiveUserCount] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x37CCFE1D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_90dae8e50e3a4314867cef21f0bb1bb0), APISourceType.HSS),
			[InternalAPIRequestType.ReportCID] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6E47A689 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_ad43e19aac164f32ab06e23cbc10bc4e), APISourceType.HSS),
			[InternalAPIRequestType.RequestRecommendModules] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1C78F156 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_365ea33385244ba8b1c5da69c8292f00), APISourceType.HSS),
			[InternalAPIRequestType.RequestPlayerInfoByCID] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2BF1ECF2 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_fc2313f9e2c04f099d4c47c5893e635d), APISourceType.CindyMaster),
			[InternalAPIRequestType.RequestBlacklistCredential] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x1FFA3629 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_a46716ea783442acb36e95ced1c935fc), APISourceType.CindyMaster),
			[InternalAPIRequestType.RequestPremiumCredential] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5E39E00D ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f9eb04af496747c5b32cddd51d0b3997), APISourceType.CindyMaster),
			[InternalAPIRequestType.ReportCredentialCID] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x654137E8 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb), APISourceType.CindyMaster),
			[InternalAPIRequestType.ReportChaoticBonus] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x339D3180 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c48859e0be41429f8474d6e2fecfa0de), APISourceType.CindyMaster),
			[InternalAPIRequestType.RequestChaoticBonus] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x281713B4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_dc626e3e84304b9d975c6d744befebcd), APISourceType.CindyMaster),
			[InternalAPIRequestType.RequestUsedNames] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3923A8CA ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f6778e21204a4d43ae5bbed939acf8a2), APISourceType.CindyMaster),
			[InternalAPIRequestType.ReportCosmoSpecialWeather] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B209FC6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a), APISourceType.CindyMaster),
			[InternalAPIRequestType.CreateBlacklist] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x227E823 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e3177f14377e4fe58ddaaaaf90163f44), APISourceType.CindyMaster),
			[InternalAPIRequestType.CreateSharedBlacklist] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x15FE2C78 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_e279476822c444f9955c257cab427fb2), APISourceType.CindyMaster),
			[InternalAPIRequestType.UpdateBlacklist] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x6541356E ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb), APISourceType.CindyMaster),
			[InternalAPIRequestType.GetBlacklist] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x65414AF0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_33b91e080b77482a8d7d3bd797a9d8fb), APISourceType.CindyMaster),
			[InternalAPIRequestType.GetBlacklistLogs] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x35A83161 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_95b251cdaa8148b7af54d7956ad9ed2a), APISourceType.CindyMaster),
			[InternalAPIRequestType.ReportDutyProgress] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x2CC0290C ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c02509fc64c64faaa7b59b967c4e5f83), APISourceType.Haku),
			[InternalAPIRequestType.RequestISPTranslator] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5BFFA4 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a), APISourceType.ISP),
			[InternalAPIRequestType.ReportCECrowdsource] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x76DA576F ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_f8c073ebdce5414baf147b1be2d2c2c2), APISourceType.CECrowdsource),
			[InternalAPIRequestType.RequestCECrowdsourceSampling] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x5B20E0F6 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_435f54c92c5b40a9ad2757dd36deed0a), APISourceType.CECrowdsource),
			[InternalAPIRequestType.RequestChaoticUptime] = (GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x38D3C25A ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_59cab3a5f3464d2f8ff563ddb3d8bad1), APISourceType.ChaoticUptime)
		}.ToFrozenDictionary();
	}
}
