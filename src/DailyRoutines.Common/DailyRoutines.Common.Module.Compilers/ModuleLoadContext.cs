using System;
using System.Reflection;
using System.Runtime.Loader;

namespace DailyRoutines.Common.Module.Compilers;

public class ModuleLoadContext(AssemblyLoadContext parentLoadContext, string name) : AssemblyLoadContext(name, isCollectible: true)
{
	protected override Assembly Load(AssemblyName assemblyName)
	{
		return parentLoadContext.LoadFromAssemblyName(assemblyName);
	}

	protected override nint LoadUnmanagedDll(string unmanagedDLLName)
	{
		return IntPtr.Zero;
	}
}
