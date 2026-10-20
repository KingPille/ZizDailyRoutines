using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Lumina.Excel;
using OmenTools.Threading;
using TinyPinyin;

namespace OmenTools.Interop.Game.Lumina;

public class LuminaSearcher<T> where T : struct, IExcelRow<T>
{
	private readonly Throttler<Guid> searchThrottler = new Throttler<Guid>();

	private readonly int resultLimit;

	private readonly uint throttleInterval;

	private readonly int retryInterval;

	private readonly int maxRetries;

	private readonly string[][] preprocessedData;

	private readonly ConcurrentDictionary<string, List<int>> cache = new ConcurrentDictionary<string, List<int>>();

	public Guid GUID { get; init; }

	public IReadOnlyList<T> Data { get; init; }

	public List<T> SearchResult { get; private set; }

	public LuminaSearcher(IEnumerable<T> data, Func<T, string>[] searchFuncs, Func<IQueryable<T>, IOrderedQueryable<T>>? orderFunc = null, int resultLimit = 100, uint throttleInterval = 100u, int retryInterval = 50, int maxRetries = 3)
	{
		LuminaSearcher<T> luminaSearcher = this;
		GUID = Guid.NewGuid();
		if (orderFunc != null)
		{
			IQueryable<T> arg = data.AsQueryable();
			Data = orderFunc(arg).ThenBy((T x) => ((IExcelRow<T>)x).RowId).ToList();
		}
		else
		{
			Data = data.OrderBy((T x) => ((IExcelRow<T>)x).RowId).ToList();
		}
		SearchResult = Data.Take(resultLimit).ToList();
		this.resultLimit = resultLimit;
		this.throttleInterval = throttleInterval;
		this.retryInterval = retryInterval;
		this.maxRetries = maxRetries;
		int count = Data.Count;
		preprocessedData = new string[count][];
		Parallel.For(0, count, delegate(int i)
		{
			T arg2 = luminaSearcher.Data[i];
			string[] array = new string[searchFuncs.Length];
			for (int j = 0; j < searchFuncs.Length; j++)
			{
				string text = searchFuncs[j](arg2);
				array[j] = text + "_" + PinyinHelper.GetPinyin(text, string.Empty);
			}
			luminaSearcher.preprocessedData[i] = array;
		});
	}

	public void Search(string keyword, bool isIgnoreCase = true, bool isRegex = true)
	{
		string key = keyword + "_" + isIgnoreCase + "_" + isRegex;
		if (cache.TryGetValue(key, out List<int> value))
		{
			SearchResult = (from index in value.Take(resultLimit)
				select Data[index]).ToList();
		}
		else if (!searchThrottler.Throttle(GUID, throttleInterval))
		{
			RetrySearch(keyword, isIgnoreCase, isRegex, 0);
		}
		else
		{
			ExecuteSearch(keyword, isIgnoreCase, isRegex);
		}
	}

	private void RetrySearch(string keyword, bool isIgnoreCase, bool isRegex, int attempt)
	{
		if (attempt >= maxRetries)
		{
			return;
		}
		Task.Delay(retryInterval).ContinueWith(delegate
		{
			if (!searchThrottler.Throttle(GUID, throttleInterval))
			{
				RetrySearch(keyword, isIgnoreCase, isRegex, attempt + 1);
			}
			else
			{
				ExecuteSearch(keyword, isIgnoreCase, isRegex);
			}
		});
	}

	private void ExecuteSearch(string keyword, bool isIgnoreCase, bool isRegex)
	{
		StringComparison comparison = (isIgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		List<int> finalResults = new List<int>();
		Lock syncLock = new Lock();
		RegexOptions options = (isIgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
		Regex regex = null;
		if (isRegex)
		{
			try
			{
				regex = new Regex(keyword, options);
			}
			catch
			{
				isRegex = false;
			}
		}
		Parallel.For(0, preprocessedData.Length, () => new List<int>(), delegate(int i, ParallelLoopState _, List<int> localList)
		{
			string[] array = preprocessedData[i];
			bool flag = false;
			if (isRegex && regex != null)
			{
				string[] array2 = array;
				foreach (string input in array2)
				{
					if (regex.IsMatch(input))
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				string[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j].Contains(keyword, comparison))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				localList.Add(i);
			}
			return localList;
		}, delegate(List<int> localList)
		{
			if (localList.Count > 0)
			{
				using (syncLock.EnterScope())
				{
					finalResults.AddRange(localList);
				}
			}
		});
		finalResults.Sort();
		List<int> list = finalResults.Take(resultLimit).ToList();
		SearchResult = list.Select((int index) => Data[index]).ToList();
		cache[keyword + "_" + isIgnoreCase + "_" + isRegex] = list;
	}
}
