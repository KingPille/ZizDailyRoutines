using System;

namespace OmenTools.Dalamud.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
public class IPCProviderAttribute(string ipcName) : Attribute
{
	public string IPCName { get; } = ipcName;

	public string? Description { get; set; }
}
