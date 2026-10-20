using Dalamud.Game.Text.SeStringHandling;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Extensions;

public static class BitMapFontIconExtension
{
	extension(BitmapFontIcon icon)
	{
		public ClassJob ToClassJob()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Expected I4, but got Unknown
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			int num = (int)icon;
			switch (num)
			{
			case 128:
			case 129:
			case 130:
			case 131:
			case 132:
			case 133:
			case 134:
			case 135:
			case 136:
			case 137:
			case 138:
			case 139:
			case 140:
			case 141:
			case 142:
			case 143:
			case 144:
			case 145:
			case 146:
			case 147:
			case 148:
			case 149:
			case 150:
			case 151:
			case 152:
			case 153:
			case 154:
			case 155:
			case 156:
			case 157:
			case 158:
			case 159:
			case 160:
			case 161:
			case 162:
			case 163:
			case 164:
			case 165:
			case 166:
			case 167:
			{
				uint num2 = (uint)(num - 127);
				if (num2 >= 1 && num2 <= 40)
				{
					return LuminaGetter.GetRowOrDefault<ClassJob>(num2);
				}
				break;
			}
			case 170:
				return LuminaGetter.GetRowOrDefault<ClassJob>(41u);
			case 171:
				return LuminaGetter.GetRowOrDefault<ClassJob>(42u);
			case 183:
				return LuminaGetter.GetRowOrDefault<ClassJob>(43u);
			}
			return LuminaGetter.GetRowOrDefault<ClassJob>(0u);
		}
	}
}
