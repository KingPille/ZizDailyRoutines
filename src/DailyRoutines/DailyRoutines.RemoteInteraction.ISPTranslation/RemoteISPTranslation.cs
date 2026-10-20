using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Common.Manager.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Abstractions;
using DailyRoutines.Common.RemoteInteraction.Models;
using DailyRoutines.Manager;
using DailyRoutines.RemoteInteraction.ISPTranslation.Handlers;
using DailyRoutines.RemoteInteraction.ISPTranslation.Models;
using DailyRoutines.RemoteInteraction.ISPTranslation.Models.Responses;

namespace DailyRoutines.RemoteInteraction.ISPTranslation;

public class RemoteISPTranslation : RemoteInteractionBase
{
	public static ISPTranslationKey CreateKey(string text, CultureInfo? culture = null)
	{
		if (culture == null)
		{
			culture = CultureInfo.CurrentUICulture;
		}
		return new ISPTranslationKey(text.Trim(), culture.Name);
	}

	public static RemoteSnapshot<ISPTranslatorResponse> GetOrRequest(string text, CultureInfo? culture = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().GetOrRequest<ISPTranslationQueryHandler, ISPTranslationKey, ISPTranslatorResponse>(CreateKey(text, culture));
	}

	public static bool TryGet(string text, out ISPTranslatorResponse value, CultureInfo? culture = null)
	{
		return ManagerBase<RemoteInteractionManager>.Instance().TryGet<ISPTranslationQueryHandler, ISPTranslationKey, ISPTranslatorResponse>(CreateKey(text, culture), out value);
	}

	public static ValueTask<ISPTranslatorResponse> GetFreshAsync(string text, CultureInfo? culture = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return new ValueTask<ISPTranslatorResponse>((ISPTranslatorResponse)null);
	}

	public RemoteISPTranslation()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
	}
}
