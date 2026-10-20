using DailyRoutines.Common.Info;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Utility;

namespace DailyRoutines.Common.Extensions;

public static class SeStringExtension
{
	extension(SeString text)
	{
		public SeString WithDRPrefix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return new SeStringBuilder().Append(SeStringExtensions.ToDalamudString(Assets.BoxedLettersDR)).AddText(" ").Append(text)
				.Build();
		}

		public SeString WithDPrefix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return new SeStringBuilder().Append(SeStringExtensions.ToDalamudString(Assets.BoxedLetterD)).Append(" ").Append(text)
				.Build();
		}

		public SeString WithRPrefix()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return new SeStringBuilder().Append(SeStringExtensions.ToDalamudString(Assets.BoxedLetterR)).Append(" ").Append(text)
				.Build();
		}
	}
}
