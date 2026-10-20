using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DailyRoutines.Common.Module.Enums;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.Common.Module.Models;

public sealed class ModuleInfo
{
	public class LinkInfo
	{
		[CompilerGenerated]
		private string _003CIconLink_003Ek__BackingField;

		public string GitHubLink { get; set; } = string.Empty;

		public string SupportLink { get; set; } = string.Empty;

		public string IconLink
		{
			get
			{
				if (!string.IsNullOrEmpty(_003CIconLink_003Ek__BackingField))
				{
					return _003CIconLink_003Ek__BackingField;
				}
				if (string.IsNullOrWhiteSpace(GitHubLink))
				{
					return string.Empty;
				}
				_003CIconLink_003Ek__BackingField = GitHubLink.Replace("hxxps://github.com", "hxxps://gh.atmoomen.top/avatars.githubusercontent.com");
				return _003CIconLink_003Ek__BackingField;
			}
		}
	}

	[CompilerGenerated]
	private readonly string[] _003CAuthor_003Ek__BackingField;

	private const string AUTHOR_SUPPORT_LINKS_BASE_URL = "hxxps://gh.atmoomen.top/raw.githubusercontent.com/Dalamud-DailyRoutines/DailyRoutines.Info/master/assets/AuthorSupportLinks.json";

	public required string Title { get; init; }

	public required string Description { get; init; }

	public required ModuleCategory Category { get; init; }

	public string[] Author
	{
		[CompilerGenerated]
		get
		{
			return _003CAuthor_003Ek__BackingField;
		}
		[CompilerGenerated]
		init
		{
		}
	}

	public string ReportURL { get; init; }

	public string[] ModulesPrerequisite { get; init; }

	public string[] ModulesPair { get; init; }

	public string[] ModulesConflict { get; init; }

	public string[] PreviewImageURL { get; init; }

	public string[] SupportUrls => (from x in Author
		select (!AuthorSupportLinks.TryGetValue(x, out LinkInfo value)) ? string.Empty : value.SupportLink into x
		where !string.IsNullOrWhiteSpace(x)
		select x).ToArray();

	public static Dictionary<string, LinkInfo> AuthorSupportLinks { get; private set; }

	public override string ToString()
	{
		return Title;
	}

	static ModuleInfo()
	{
		AuthorSupportLinks = new Dictionary<string, LinkInfo>();
		using SharedCancellationLease sharedCancellationLease = OmenServiceBase<HTTPClientHelper>.Instance().AcquireSharedCancellation();
		CancellationToken token = sharedCancellationLease.Token;
		Task.Run(async delegate
		{
			try
			{
				OmenServiceBase<HTTPClientHelper>.Instance().Get();
				_ = token;
				throw null;
			}
			catch
			{
			}
		}, token);
	}

	public ModuleInfo()
	{
		Author = new string[1] { "ziz" };
		ReportURL = "hxxps://discord.com/channels/1258981591124938762/1464230937653940316";
		ModulesPrerequisite = Array.Empty<string>();
		ModulesPair = Array.Empty<string>();
		ModulesConflict = Array.Empty<string>();
		PreviewImageURL = Array.Empty<string>();
		base._002Ector();
	}
}
