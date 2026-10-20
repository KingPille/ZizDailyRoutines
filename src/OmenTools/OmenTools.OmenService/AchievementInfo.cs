using Dalamud.Game;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;
using OmenTools.Extensions;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.OmenService;

public record AchievementInfo(uint ID, uint Current, uint Max)
{
	public string Name
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Achievement data = GetData();
			return ((object)((Achievement)(ref data)).Name/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
		}
	}

	public string Description
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			Achievement data = GetData();
			return ((object)((Achievement)(ref data)).Description/*cast due to constrained. prefix*/).ToString() ?? string.Empty;
		}
	}

	public uint Icon
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Achievement data = GetData();
			return ((Achievement)(ref data)).Icon;
		}
	}

	public IDalamudTextureWrap IconTexture
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			ITextureProvider obj = DalamudServiceExtension.Instance<ITextureProvider>();
			GameIconLookup val = new GameIconLookup(Icon, false, true, (ClientLanguage?)null);
			return obj.GetFromGameIcon(ref val).GetWrapOrEmpty();
		}
	}

	public bool IsFinished => Current == Max;

	public Achievement GetData()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return LuminaGetter.GetRow<Achievement>(ID).GetValueOrDefault();
	}
}
