using System;
using System.Linq;
using System.Reflection;
using OmenTools.Dalamud;
using OmenTools.Dalamud.Attributes;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class IPCManager : OmenServiceBase<IPCManager>
{
	private static readonly Type[] StaticIPCTypes = typeof(IPCManager).Assembly.GetTypes().Where(HasIPCAttributeMember).ToArray();

	protected override void Init()
	{
		Type[] staticIPCTypes = StaticIPCTypes;
		for (int i = 0; i < staticIPCTypes.Length; i++)
		{
			IPCAttributeRegistry.RegStaticIPCs(staticIPCTypes[i]);
		}
	}

	protected override void Uninit()
	{
		Type[] staticIPCTypes = StaticIPCTypes;
		for (int i = 0; i < staticIPCTypes.Length; i++)
		{
			IPCAttributeRegistry.UnregStaticIPCs(staticIPCTypes[i]);
		}
	}

	private static bool HasIPCAttributeMember(Type type)
	{
		return type.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Any((MemberInfo member) => member.IsDefined(typeof(IPCSubscriberAttribute), inherit: false) || member.IsDefined(typeof(IPCProviderAttribute), inherit: false));
	}
}
