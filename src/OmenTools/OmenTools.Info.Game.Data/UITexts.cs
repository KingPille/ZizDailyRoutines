using Lumina.Data;
using OmenTools.OmenService;

namespace OmenTools.Info.Game.Data;

public static class UITexts
{
	public static bool IsFullWidthLanguage { get; }

	public static string Colon { get; }

	public static char Space { get; }

	public static char LeftSquareBracket { get; }

	public static char RightSquareBracket { get; }

	static UITexts()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		Language clientLanguage = GameState.ClientLanguage;
		bool flag = (((int)clientLanguage == 1 || clientLanguage - 5 <= 3) ? true : false);
		IsFullWidthLanguage = flag;
		clientLanguage = GameState.ClientLanguage;
		flag = ((clientLanguage - 5 <= 1 || (int)clientLanguage == 8) ? true : false);
		Colon = (flag ? "：" : ": ");
		Space = (IsFullWidthLanguage ? '\u3000' : ' ');
		LeftSquareBracket = (IsFullWidthLanguage ? '［' : '[');
		RightSquareBracket = (IsFullWidthLanguage ? '］' : ']');
	}
}
