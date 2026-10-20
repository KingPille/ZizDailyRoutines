using System;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using OmenTools.Dalamud;
using OmenTools.Extensions;

namespace OmenTools.Interop.Game.Models;

public record CompSig
{
	public string Signature { get; init; }

	public CompSig(string signature)
	{
		Signature = signature.Trim() ?? throw new ArgumentNullException("signature");
	}

	public string Get()
	{
		return Signature;
	}

	public nint ScanText()
	{
		try
		{
			return DalamudServiceExtension.Instance<ISigScanner>().ScanText(Signature);
		}
		catch (Exception ex)
		{
			DLog.Error("尝试 ScanText 时失败, 签名: " + Signature, ex);
		}
		return IntPtr.Zero;
	}

	public unsafe T* ScanText<T>() where T : unmanaged
	{
		return (T*)ScanText();
	}

	public nint GetStatic(int offset = 0)
	{
		try
		{
			return DalamudServiceExtension.Instance<ISigScanner>().GetStaticAddressFromSig(Signature, offset);
		}
		catch (Exception ex)
		{
			DLog.Error("尝试 GetStaticAddress 时失败, 签名: " + Signature, ex);
		}
		return IntPtr.Zero;
	}

	public unsafe T* GetStatic<T>(int offset = 0) where T : unmanaged
	{
		return (T*)GetStatic(offset);
	}

	public T GetDelegate<T>() where T : Delegate
	{
		return Marshal.GetDelegateForFunctionPointer<T>(ScanText());
	}

	public Hook<T> GetHook<T>(T detour) where T : Delegate
	{
		Hook<T> val = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromSignature<T>(Signature, detour, (HookBackend)0);
		DService.Instance().RegHook((IDalamudHook)(object)val, AssemblyLoadContext.GetLoadContext(typeof(T).Assembly));
		return val;
	}
}
