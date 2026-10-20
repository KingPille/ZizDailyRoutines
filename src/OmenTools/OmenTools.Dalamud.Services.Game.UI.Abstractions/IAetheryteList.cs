using System.Collections;
using System.Collections.Generic;

namespace OmenTools.Dalamud.Services.Game.UI.Abstractions;

public interface IAetheryteList : IReadOnlyCollection<IAetheryteEntry>, IEnumerable<IAetheryteEntry>, IEnumerable, IOmenDalamudService<IAetheryteList>, IOmenDalamudService
{
	int Length { get; }

	IAetheryteEntry? this[int index] { get; }
}
