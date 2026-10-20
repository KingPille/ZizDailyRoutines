using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace OmenTools.Extensions;

public static class CommonExtension
{
	extension<T>(T config) where T : class
	{
		public string ToJSONBase64()
		{
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(config.ToJSON()));
		}

		public string ToJSON()
		{
			return JsonConvert.SerializeObject((object)config, JsonSerializerSettingsExtension.GetShared());
		}
	}

	extension(string textData)
	{
		public T? FromJSON<T>() where T : class
		{
			if (string.IsNullOrEmpty(textData))
			{
				return null;
			}
			return JsonConvert.DeserializeObject<T>(textData, JsonSerializerSettingsExtension.GetShared());
		}

		public T? FromJSONBase64<T>() where T : class
		{
			if (string.IsNullOrEmpty(textData))
			{
				return null;
			}
			return Encoding.UTF8.GetString(Convert.FromBase64String(textData)).FromJSON<T>();
		}
	}

	extension(byte[] haystack)
	{
		public bool TryFindBytes(byte[] needle, out int pos)
		{
			int num = needle.Length;
			int num2 = haystack.Length - num;
			for (int i = 0; i <= num2; i++)
			{
				int j;
				for (j = 0; j < num && needle[j] == haystack[i + j]; j++)
				{
				}
				if (j == num)
				{
					pos = i;
					return true;
				}
			}
			pos = 0;
			return false;
		}

		public bool TryFindBytes(string needle, out int pos)
		{
			return haystack.TryFindBytes((from x in needle.Split(" ")
				select byte.Parse(x, NumberStyles.HexNumber)).ToArray(), out pos);
		}
	}
}
