namespace OmenTools.OmenService.ZoneIndicator;

public interface IZoneIndicatorMutable
{
	uint? RenderRadius { get; set; }

	bool HiddenWhenBlocked { get; set; }

	bool HideTextLabel { get; set; }

	ZoneIndicatorSurrounding? Surrounding { get; set; }
}
