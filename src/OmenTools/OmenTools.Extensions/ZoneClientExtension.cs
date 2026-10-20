using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Application.Network;
using FFXIVClientStructs.FFXIV.Client.System.Framework;

namespace OmenTools.Extensions;

public static class ZoneClientExtension
{
	extension(ZoneClient client)
	{
		public unsafe static ZoneClient* Instance()
		{
			if (ZoneClientOffset == 0)
			{
				return null;
			}
			return (ZoneClient*)((byte*)((Framework)Framework.Instance()).NetworkModuleProxy + ZoneClientOffset);
		}
	}

	[CompilerGenerated]
	private static int _003CZoneClientOffset_003Ek__BackingField;

	private static int ZoneClientOffset
	{
		get
		{
			if (_003CZoneClientOffset_003Ek__BackingField != 0)
			{
				return _003CZoneClientOffset_003Ek__BackingField;
			}
			return _003CZoneClientOffset_003Ek__BackingField = GetFieldOffset("ZoneClient").GetValueOrDefault();
			static int? GetFieldOffset(string fieldName)
			{
				FieldInfo field = typeof(NetworkModule).GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				if (field == null)
				{
					return null;
				}
				return field.GetCustomAttribute<FieldOffsetAttribute>()?.Value;
			}
		}
	}
}
