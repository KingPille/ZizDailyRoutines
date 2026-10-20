using System;

namespace OmenTools.Dalamud.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IPCSubscriberAttribute(string ipcName) : Attribute
{
	public string IPCName { get; } = ipcName;

	public bool AutoInitialize { get; set; } = true;

	public string? DefaultValue { get; set; }

	public (bool IsValid, string? ErrorMessage) Validate()
	{
		if (string.IsNullOrWhiteSpace(IPCName))
		{
			return (IsValid: false, ErrorMessage: "IPC 名称不能为空");
		}
		return (IsValid: true, ErrorMessage: null);
	}

	public T? GetTypedDefaultValue<T>()
	{
		if (string.IsNullOrEmpty(DefaultValue))
		{
			return default(T);
		}
		try
		{
			Type type = typeof(T);
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			return type.Name switch
			{
				"Boolean" => (T)(object)bool.Parse(DefaultValue), 
				"Int32" => (T)(object)int.Parse(DefaultValue), 
				"Int64" => (T)(object)long.Parse(DefaultValue), 
				"Single" => (T)(object)float.Parse(DefaultValue), 
				"Double" => (T)(object)double.Parse(DefaultValue), 
				"String" => (T)(object)DefaultValue, 
				_ => (T)Convert.ChangeType(DefaultValue, type), 
			};
		}
		catch
		{
			return default(T);
		}
	}
}
