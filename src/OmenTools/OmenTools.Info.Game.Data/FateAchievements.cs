using System.Collections.Frozen;
using System.Collections.Generic;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.Data;

public static class FateAchievements
{
	public static uint VersionCount { get; } = (uint)(LuminaGetter.Get<ExVersion>().Count - 1);

	public static FrozenDictionary<uint, uint> AchievementToZone { get; } = new Dictionary<uint, uint>
	{
		[2343u] = 813u,
		[2345u] = 815u,
		[2346u] = 816u,
		[2344u] = 814u,
		[2347u] = 817u,
		[2348u] = 818u,
		[3022u] = 956u,
		[3023u] = 957u,
		[3024u] = 958u,
		[3025u] = 959u,
		[3026u] = 961u,
		[3027u] = 960u,
		[3559u] = 1187u,
		[3560u] = 1188u,
		[3561u] = 1189u,
		[3562u] = 1190u,
		[3563u] = 1191u,
		[3564u] = 1192u
	}.ToFrozenDictionary();

	public static FrozenDictionary<uint, uint> ZoneToAchievement { get; } = AchievementToZone.ToFrozenDictionary((KeyValuePair<uint, uint> x) => x.Value, (KeyValuePair<uint, uint> x) => x.Key);
}
