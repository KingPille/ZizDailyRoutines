using System;
using System.Collections.Generic;
using OmenTools.OmenService.Abstractions;

namespace OmenTools;

public sealed class DServiceInitOptions
{
	private readonly HashSet<Type> disabledServices = new HashSet<Type>();

	public DServiceInitOptions Disable<TService>() where TService : OmenServiceBase
	{
		disabledServices.Add(typeof(TService));
		return this;
	}

	internal bool IsDisabled(Type serviceType)
	{
		return disabledServices.Contains(serviceType);
	}
}
