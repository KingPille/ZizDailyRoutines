using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.DataUploader.Abstractions;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.Manager.Attributes;

namespace swBjtDH3URWiJCmRm95;

[ManagerInitOrder(16)]
internal sealed class FefQ2WHphcrBKNTyisX : ManagerBase<FefQ2WHphcrBKNTyisX>
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CInit_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncTaskMethodBuilder _003C_003Et__builder;

		public FefQ2WHphcrBKNTyisX _003C_003E4__this;

		private List<DataUploaderBase>.Enumerator _003C_003E7__wrap1;

		private TaskAwaiter _003C_003Eu__1;

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			FefQ2WHphcrBKNTyisX fefQ2WHphcrBKNTyisX = _003C_003E4__this;
			try
			{
				if (num != 0)
				{
					fefQ2WHphcrBKNTyisX.Uploaders = (from t in Assembly.GetExecutingAssembly().GetTypes()
						where typeof(DataUploaderBase).IsAssignableFrom(t) && !t.IsAbstract
						select (DataUploaderBase)Activator.CreateInstance(t) into x
						where x != null
						select x).OfType<DataUploaderBase>().ToList();
					_003C_003E7__wrap1 = fefQ2WHphcrBKNTyisX.Uploaders.GetEnumerator();
				}
				try
				{
					if (num != 0)
					{
						goto IL_012f;
					}
					TaskAwaiter awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter);
					num = (_003C_003E1__state = -1);
					goto IL_0128;
					IL_0128:
					awaiter.GetResult();
					goto IL_012f;
					IL_012f:
					if (_003C_003E7__wrap1.MoveNext())
					{
						awaiter = _003C_003E7__wrap1.Current.PublicInitAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_0128;
					}
				}
				finally
				{
					if (num < 0)
					{
						((IDisposable)_003C_003E7__wrap1/*cast due to constrained. prefix*/).Dispose();
					}
				}
				_003C_003E7__wrap1 = default(List<DataUploaderBase>.Enumerator);
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

	private List<DataUploaderBase> Uploaders { get; set; }

	[AsyncStateMachine(typeof(_003CInit_003Ed__4))]
	protected override Task Init()
	{
		return Task.CompletedTask;
	}

	protected override async Task Uninit()
	{
		foreach (DataUploaderBase uploader in Uploaders)
		{
			await uploader.PublicUninitAsync();
		}
		Uploaders.Clear();
	}

	public FefQ2WHphcrBKNTyisX()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Uploaders = new List<DataUploaderBase>();
		base._002Ector();
	}
}
