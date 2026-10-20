using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OmenTools.Dalamud.DataShare.Attributes;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class DataShareManager : OmenServiceBase<DataShareManager>
{
	private string[] dataShareTags = Array.Empty<string>();

	protected override void Init()
	{
		dataShareTags = DiscoverDataShareTags();
	}

	protected override void Uninit()
	{
		string[] array = dataShareTags;
		foreach (string text in array)
		{
			DalamudServiceExtension.Instance().RelinquishData(text);
		}
		dataShareTags = Array.Empty<string>();
	}

	private static string[] DiscoverDataShareTags()
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		for (int i = 0; i < types.Length; i++)
		{
			FieldInfo[] fields = types[i].GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (fieldInfo.IsDefined(typeof(DataShareTagAttribute), inherit: false))
				{
					ValidateDataShareTagField(fieldInfo);
					hashSet.Add((fieldInfo.GetRawConstantValue() as string) ?? throw new InvalidOperationException("字段 " + GetFieldDisplayName(fieldInfo) + " 的 DataShare 标签值不能为空"));
				}
			}
		}
		return hashSet.Order<string>(StringComparer.Ordinal).ToArray();
	}

	private static void ValidateDataShareTagField(FieldInfo field)
	{
		if (!field.IsLiteral || field.IsInitOnly)
		{
			throw new InvalidOperationException("字段 " + GetFieldDisplayName(field) + " 标记了 [DataShareTag]，但它不是 const string");
		}
		if (field.FieldType != typeof(string))
		{
			throw new InvalidOperationException("字段 " + GetFieldDisplayName(field) + " 标记了 [DataShareTag]，但它不是 const string");
		}
	}

	private static string GetFieldDisplayName(FieldInfo field)
	{
		return (field.DeclaringType?.FullName ?? "<未知类型>") + "." + field.Name;
	}
}
