using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace OmenTools.Utils;

public sealed class LodestoneSearcher(HttpClient client, CancellationToken cancellationToken)
{
	private ImmutableDictionary<(string Name, string World), (Task<string?> Task, DateTimeOffset Expires)> queries = ImmutableDictionary<(string, string), (Task<string>, DateTimeOffset)>.Empty;

	public async Task<string?> GetCharacterIDAsync(string name, string world)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name, "name");
		ArgumentException.ThrowIfNullOrWhiteSpace(world, "world");
		cancellationToken.ThrowIfCancellationRequested();
		(string, string) key = (name.ToUpperInvariant(), world.ToUpperInvariant());
		TaskCompletionSource<string?> completion = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
		ImmutableDictionary<(string, string), (Task<string>, DateTimeOffset)> immutableDictionary;
		ImmutableDictionary<(string, string), (Task<string>, DateTimeOffset)> value2;
		do
		{
			immutableDictionary = Volatile.Read(in queries);
			if (immutableDictionary.TryGetValue(key, out var value) && (!value.Item1.IsCompleted || value.Item2 > DateTimeOffset.UtcNow))
			{
				return await value.Item1.ConfigureAwait(continueOnCapturedContext: false);
			}
			DateTimeOffset now = DateTimeOffset.UtcNow;
			value2 = immutableDictionary.RemoveRange(from x in immutableDictionary
				where x.Value.Task.IsCompleted && x.Value.Expires <= now
				select x.Key).SetItem(key, (completion.Task, now.AddMinutes(5.0)));
		}
		while (Interlocked.CompareExchange(ref queries, value2, immutableDictionary) != immutableDictionary);
		string characterID = null;
		try
		{
			characterID = await SearchCharacterIDAsync(name, world).ConfigureAwait(continueOnCapturedContext: false);
			completion.SetResult(characterID);
		}
		catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
		{
			completion.SetCanceled(cancellationToken);
		}
		catch (Exception exception)
		{
			completion.SetException(exception);
		}
		finally
		{
			if (characterID == null)
			{
				ImmutableInterlocked.Update<ImmutableDictionary<(string, string), (Task<string>, DateTimeOffset)>>(ref queries, (ImmutableDictionary<(string Name, string World), (Task<string> Task, DateTimeOffset Expires)> current) => (!current.TryGetValue(key, out (Task<string>, DateTimeOffset) value3) || value3.Item1 != completion.Task) ? current : current.Remove(key));
			}
		}
		return await completion.Task.ConfigureAwait(continueOnCapturedContext: false);
	}

	private async Task<string?> SearchCharacterIDAsync(string name, string world)
	{
		string searchURL = "https://na.finalfantasyxiv.com/lodestone/character/?q=" + Uri.EscapeDataString(name) + "&worldname=" + Uri.EscapeDataString(world);
		int page = 1;
		while (true)
		{
			string source = await client.GetStringAsync($"{searchURL}&page={page}", cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			using (IHtmlDocument document = await new HtmlParser().ParseDocumentAsync(source, cancellationToken).ConfigureAwait(continueOnCapturedContext: false))
			{
				if (document.QuerySelector(".parts__total") == null)
				{
					throw new InvalidDataException("Lodestone returned an unexpected character search page.");
				}
				foreach (IElement item in document.QuerySelectorAll(".entry > a.entry__link"))
				{
					string a = item.QuerySelector(".entry__name")?.TextContent.Trim();
					IElement? element = item.QuerySelector(".entry__world");
					string a2 = ((element != null) ? element.TextContent.Split('[')[0].Trim() : null);
					if (!string.Equals(a, name, StringComparison.OrdinalIgnoreCase) || !string.Equals(a2, world, StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					string[] array = item.GetAttribute("href")?.Split('/', StringSplitOptions.RemoveEmptyEntries);
					if (array != null && array.Length == 3 && array[0] == "lodestone" && array[1] == "character")
					{
						string text = array[2];
						if (uint.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var result) && result != 0)
						{
							return text;
						}
					}
					throw new InvalidDataException("Lodestone returned an invalid character ID.");
				}
				if (document.QuerySelector("a.btn__pager__next:not(.btn__pager__no)") == null)
				{
					return null;
				}
				await Task.Delay(1000, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
			}
			page++;
		}
	}
}
