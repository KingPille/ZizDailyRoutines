using System;
using System.Collections.Generic;
using System.Linq;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Models;
using OmenTools.Utils.FuzzyMatcher;

namespace DailyRoutines.Common.Module;

public sealed class ModuleSearcher
{
	private static readonly FuzzySearchWeight TitleWeight = new FuzzySearchWeight(400, 24, 18, 16, 200, 3);

	private static readonly FuzzySearchWeight NameWeight = new FuzzySearchWeight(320, 20, 14, 12, 160, 4);

	private static readonly FuzzySearchWeight MetaWeight = new FuzzySearchWeight(200, 14, 10, 8, 100, 5);

	private readonly FuzzyMatcher<ModuleBase> matcher;

	public ModuleSearcher(IEnumerable<ModuleBase> modules, Func<ModuleBase, IEnumerable<string?>?>? extraSegmentsSelector = null)
	{
		ArgumentNullException.ThrowIfNull(modules, "modules");
		matcher = new FuzzyMatcher<ModuleBase>(modules.Where((ModuleBase m) => m != null), delegate(ModuleBase module)
		{
			ModuleInfo info = module.Info;
			Type type = module.GetType();
			IEnumerable<string> enumerable = extraSegmentsSelector?.Invoke(module);
			(IEnumerable<string>, FuzzySearchWeight)[] obj = new(IEnumerable<string>, FuzzySearchWeight)[3]
			{
				(new global::_003C_003Ez__ReadOnlySingleElementList<string>(info.Title), TitleWeight),
				(new global::_003C_003Ez__ReadOnlyArray<string>(new string[2] { module.ModuleName, type.Name }), NameWeight),
				default((IEnumerable<string>, FuzzySearchWeight))
			};
			List<string> list = new List<string>();
			list.Add(string.Join(" / ", info.Author));
			list.Add(info.Description);
			list.Add(string.Join(" / ", info.ModulesPrerequisite));
			list.Add(string.Join(" / ", info.ModulesPair));
			list.Add(string.Join(" / ", info.ModulesConflict));
			list.Add(string.Join(" / ", module.PrecedingModules?.Select((ModuleBase x) => x.ModuleName) ?? Array.Empty<string>()));
			list.Add(string.Join(" / ", module.PairModules?.Select((ModuleBase x) => x.ModuleName) ?? Array.Empty<string>()));
			list.Add(string.Join(" / ", module.ConflictModules?.Select((ModuleBase x) => x.ModuleName) ?? Array.Empty<string>()));
			list.Add(info.ReportURL);
			list.Add(string.Join(" / ", info.SupportUrls));
			list.Add(string.Join(" / ", info.PreviewImageURL));
			list.AddRange(enumerable ?? Array.Empty<string>());
			obj[2] = (new _003C_003Ez__ReadOnlyList<string>(list), MetaWeight);
			return new global::_003C_003Ez__ReadOnlyArray<(IEnumerable<string>, FuzzySearchWeight)>(obj);
		});
	}

	public ModuleBase[] Search(string query)
	{
		return matcher.Search(query, (ModuleBase left, ModuleBase right) => string.Compare(left.Info.Title, right.Info.Title, StringComparison.OrdinalIgnoreCase));
	}
}
