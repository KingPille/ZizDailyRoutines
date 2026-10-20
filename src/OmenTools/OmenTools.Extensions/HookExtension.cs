using System;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud.Helpers;

namespace OmenTools.Extensions;

public static class HookExtension
{
	extension(IGameInteropProvider hook)
	{
		public Hook<T> HookFromMemberFunction<T>(Type memberFunctions, string name, T detour) where T : Delegate
		{
			return hook.HookFromAddress<T>((IntPtr)DalamudReflector.GetMemberFuncByName(memberFunctions, name), detour, (HookBackend)0);
		}

		public unsafe Hook<T> HookFromVirtualTable<T, TVTable>(TVTable* vTable, string name, T detour) where T : Delegate where TVTable : unmanaged
		{
			return hook.HookFromAddress<T>((IntPtr)ReflectionExtension.GetVFuncByName(ref *vTable, name), detour, (HookBackend)0);
		}
	}

	extension<T>(Hook<T>? hook) where T : Delegate
	{
		public void Toggle(bool? isEnabled = null)
		{
			if (hook == null || hook.IsDisposed)
			{
				return;
			}
			if (!isEnabled.HasValue)
			{
				if (hook.IsEnabled)
				{
					hook.Disable();
				}
				else
				{
					hook.Enable();
				}
			}
			else if (isEnabled.Value)
			{
				hook.Enable();
			}
			else
			{
				hook.Disable();
			}
		}
	}
}
