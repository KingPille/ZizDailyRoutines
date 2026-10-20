using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using OmenTools.Info.Collections;
using TinyPinyin;

namespace OmenTools.Utils.FuzzyMatcher;

public sealed class FuzzyMatcher<T> : IDisposable
{
	private readonly record struct SearchEntry(T Item, SearchKey[] Keys);

	private sealed class SearchKey
	{
		public SegmentKey[] Segments { get; }

		public FuzzySearchWeight Weight { get; }

		public SearchKey(IEnumerable<string?> primarySegments, FuzzySearchWeight weight, IPinyinProvider pinyinProvider)
		{
			Weight = weight;
			List<SegmentKey> list = new List<SegmentKey>();
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (string primarySegment in primarySegments)
			{
				string text = FuzzyMatcher<T>.NormalizeText(primarySegment);
				if (!string.IsNullOrWhiteSpace(text) && hashSet.Add(text))
				{
					list.Add(new SegmentKey(text, pinyinProvider));
				}
			}
			Segments = list.ToArray();
		}
	}

	private sealed class SegmentKey
	{
		public string Original { get; }

		public string Compact { get; }

		public string[] PinyinCombinations { get; }

		public string[] PinyinInitialisms { get; }

		public string[] Initialisms { get; }

		public CharMask Mask { get; }

		public SegmentKey(string original, IPinyinProvider pinyinProvider)
		{
			Original = original;
			Compact = FuzzyMatcher<T>.Compact(original);
			bool flag = original.Any(FuzzyMatcher<T>.IsCjk);
			PinyinCombinations = (flag ? pinyinProvider.GetPinyinCombinations(original) : Array.Empty<string>());
			PinyinInitialisms = (flag ? ((DefaultPinyinProvider)pinyinProvider).GetPinyinInitialismCombinations(original) : Array.Empty<string>());
			StringBuilder stringBuilder = new StringBuilder();
			FuzzyMatcher<T>.AppendAsciiInitialism(stringBuilder, original);
			string text = stringBuilder.ToString().ToLowerInvariant();
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
			list.AddRange(PinyinInitialisms);
			Initialisms = list.Distinct().ToArray();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(Original);
			stringBuilder2.Append(Compact);
			string[] pinyinCombinations = PinyinCombinations;
			foreach (string value in pinyinCombinations)
			{
				stringBuilder2.Append(value);
			}
			pinyinCombinations = Initialisms;
			foreach (string value2 in pinyinCombinations)
			{
				stringBuilder2.Append(value2);
			}
			Mask = new CharMask(stringBuilder2.ToString());
		}
	}

	private readonly record struct SearchTerm(string Original, string Compact, string[] PinyinCombinations, bool IsCjk, bool IsExact, bool IsNegative, CharMask Mask);

	private readonly struct CharMask
	{
		private readonly ulong mask;

		public CharMask(string text)
		{
			ulong num = 0uL;
			for (int i = 0; i < text.Length; i++)
			{
				char c = char.ToLowerInvariant(text[i]);
				switch (c)
				{
				case 'a':
				case 'b':
				case 'c':
				case 'd':
				case 'e':
				case 'f':
				case 'g':
				case 'h':
				case 'i':
				case 'j':
				case 'k':
				case 'l':
				case 'm':
				case 'n':
				case 'o':
				case 'p':
				case 'q':
				case 'r':
				case 's':
				case 't':
				case 'u':
				case 'v':
				case 'w':
				case 'x':
				case 'y':
				case 'z':
					num |= (ulong)(1L << c - 97);
					continue;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					num |= (ulong)(1L << 26 + (c - 48));
					continue;
				}
				if (FuzzyMatcher<T>.IsCjk(c))
				{
					num |= 0x8000000000000000uL;
				}
			}
			mask = num;
		}

		public bool IsSubsetOf(CharMask other)
		{
			return (mask & other.mask) == mask;
		}
	}

	private interface IPinyinProvider
	{
		string GetPinyin(string text, string separator);

		string[] GetPinyinCombinations(string segment);
	}

	private sealed class DefaultPinyinProvider : IPinyinProvider
	{
		public string GetPinyin(string text, string separator)
		{
			return PinyinHelper.GetPinyin(text, separator);
		}

		public string[] GetPinyinCombinations(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
			{
				return Array.Empty<string>();
			}
			List<StringBuilder> list = new List<StringBuilder>
			{
				new StringBuilder()
			};
			for (int i = 0; i < segment.Length; i++)
			{
				char c = segment[i];
				List<string> list2 = new List<string>();
				string text = PinyinHelper.GetPinyin(c.ToString(), string.Empty).ToLowerInvariant();
				list2.Add((!string.IsNullOrWhiteSpace(text)) ? text : c.ToString().ToLowerInvariant());
				List<StringBuilder> list3 = new List<StringBuilder>();
				foreach (StringBuilder item in list)
				{
					foreach (string item2 in list2)
					{
						StringBuilder stringBuilder = new StringBuilder(item.ToString());
						stringBuilder.Append(item2);
						list3.Add(stringBuilder);
					}
				}
				list = list3;
				if (list.Count > 8)
				{
					list = list.Take(8).ToList();
				}
			}
			return list.Select((StringBuilder sb) => sb.ToString()).Distinct().ToArray();
		}

		public string[] GetPinyinInitialismCombinations(string segment)
		{
			if (string.IsNullOrWhiteSpace(segment))
			{
				return Array.Empty<string>();
			}
			List<StringBuilder> list = new List<StringBuilder>
			{
				new StringBuilder()
			};
			for (int i = 0; i < segment.Length; i++)
			{
				char c = segment[i];
				List<char> list2 = new List<char>();
				string text = PinyinHelper.GetPinyin(c.ToString(), string.Empty).ToLowerInvariant();
				list2.Add((!string.IsNullOrWhiteSpace(text)) ? text[0] : char.ToLowerInvariant(c));
				List<StringBuilder> list3 = new List<StringBuilder>();
				foreach (StringBuilder item in list)
				{
					foreach (char item2 in list2.Distinct())
					{
						StringBuilder stringBuilder = new StringBuilder(item.ToString());
						stringBuilder.Append(item2);
						list3.Add(stringBuilder);
					}
				}
				list = list3;
				if (list.Count > 8)
				{
					list = list.Take(8).ToList();
				}
			}
			return list.Select((StringBuilder sb) => sb.ToString()).Distinct().ToArray();
		}
	}

	private readonly struct CacheKey(string query, int limit, Comparison<T>? tieBreaker) : IEquatable<CacheKey>
	{
		public string Query { get; } = query;

		public int Limit { get; } = limit;

		public Comparison<T>? TieBreaker { get; } = tieBreaker;

		public bool Equals(CacheKey other)
		{
			if (string.Equals(Query, other.Query, StringComparison.Ordinal) && Limit == other.Limit)
			{
				return object.Equals(TieBreaker, other.TieBreaker);
			}
			return false;
		}

		public override bool Equals(object? obj)
		{
			if (obj is CacheKey other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Query, Limit, TieBreaker);
		}
	}

	private readonly struct MatchPriority(int score, T item, Comparison<T>? tieBreaker) : IComparable<MatchPriority>
	{
		public int Score { get; } = score;

		public T Item { get; } = item;

		public int CompareTo(MatchPriority other)
		{
			int num = Score.CompareTo(other.Score);
			if (num != 0)
			{
				return num;
			}
			if (tieBreaker != null)
			{
				return -tieBreaker(Item, other.Item);
			}
			return 0;
		}
	}

	[CompilerGenerated]
	private sealed class _003CSearchAsync_003Ed__8 : IAsyncEnumerable<T>, IAsyncEnumerator<T>, IAsyncDisposable, IValueTaskSource<bool>, IValueTaskSource, IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncIteratorMethodBuilder _003C_003Et__builder;

		public ManualResetValueTaskSourceCore<bool> _003C_003Ev__promiseOfValueOrEnd;

		private T _003C_003E2__current;

		private bool _003C_003Ew__disposeMode;

		private CancellationTokenSource _003C_003Ex__combinedTokens;

		private int _003C_003El__initialThreadId;

		private CancellationToken cancellationToken;

		public CancellationToken _003C_003E3__cancellationToken;

		public FuzzyMatcher<T> _003C_003E4__this;

		private string query;

		public string _003C_003E3__query;

		private Comparison<T> tieBreaker;

		public Comparison<T> _003C_003E3__tieBreaker;

		private T[] _003C_003E7__wrap1;

		private int _003C_003E7__wrap2;

		private YieldAwaitable.YieldAwaiter _003C_003Eu__1;

		T IAsyncEnumerator<T>.Current
		{
			[DebuggerHidden]
			get
			{
				return _003C_003E2__current;
			}
		}

		[DebuggerHidden]
		public _003CSearchAsync_003Ed__8(int _003C_003E1__state)
		{
			_003C_003Et__builder = AsyncIteratorMethodBuilder.Create();
			this._003C_003E1__state = _003C_003E1__state;
			_003C_003El__initialThreadId = Environment.CurrentManagedThreadId;
		}

		private void MoveNext()
		{
			int num = _003C_003E1__state;
			FuzzyMatcher<T> fuzzyMatcher = _003C_003E4__this;
			try
			{
				YieldAwaitable.YieldAwaiter awaiter;
				switch (num)
				{
				default:
					if (!_003C_003Ew__disposeMode)
					{
						num = (_003C_003E1__state = -1);
						cancellationToken.ThrowIfCancellationRequested();
						T[] array = fuzzyMatcher.Search(query, tieBreaker, int.MaxValue, cancellationToken);
						_003C_003E7__wrap1 = array;
						_003C_003E7__wrap2 = 0;
						break;
					}
					goto end_IL_000e;
				case -4:
					num = (_003C_003E1__state = -1);
					if (!_003C_003Ew__disposeMode)
					{
						_003C_003E2__current = default(T);
						awaiter = Task.Yield().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003CSearchAsync_003Ed__8 stateMachine = this;
							_003C_003Et__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
							return;
						}
						goto IL_0130;
					}
					goto end_IL_000e;
				case 0:
					{
						awaiter = _003C_003Eu__1;
						_003C_003Eu__1 = default(YieldAwaitable.YieldAwaiter);
						num = (_003C_003E1__state = -1);
						goto IL_0130;
					}
					IL_0130:
					awaiter.GetResult();
					_003C_003E7__wrap2++;
					break;
				}
				if (_003C_003E7__wrap2 < _003C_003E7__wrap1.Length)
				{
					T val = _003C_003E7__wrap1[_003C_003E7__wrap2];
					cancellationToken.ThrowIfCancellationRequested();
					_003C_003E2__current = val;
					num = (_003C_003E1__state = -4);
					goto IL_01ff;
				}
				_003C_003E7__wrap1 = null;
				end_IL_000e:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003E7__wrap1 = null;
				if (_003C_003Ex__combinedTokens != null)
				{
					_003C_003Ex__combinedTokens.Dispose();
					_003C_003Ex__combinedTokens = null;
				}
				_003C_003E2__current = default(T);
				_003C_003Et__builder.Complete();
				_003C_003Ev__promiseOfValueOrEnd.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003E7__wrap1 = null;
			if (_003C_003Ex__combinedTokens != null)
			{
				_003C_003Ex__combinedTokens.Dispose();
				_003C_003Ex__combinedTokens = null;
			}
			_003C_003E2__current = default(T);
			_003C_003Et__builder.Complete();
			_003C_003Ev__promiseOfValueOrEnd.SetResult(result: false);
			return;
			IL_01ff:
			_003C_003Ev__promiseOfValueOrEnd.SetResult(result: true);
		}

		void IAsyncStateMachine.MoveNext()
		{
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			this.SetStateMachine(stateMachine);
		}

		[DebuggerHidden]
		IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken = default(CancellationToken))
		{
			_003CSearchAsync_003Ed__8 _003CSearchAsync_003Ed__9;
			if (_003C_003E1__state == -2 && _003C_003El__initialThreadId == Environment.CurrentManagedThreadId)
			{
				_003C_003E1__state = -3;
				_003C_003Et__builder = AsyncIteratorMethodBuilder.Create();
				_003C_003Ew__disposeMode = false;
				_003CSearchAsync_003Ed__9 = this;
			}
			else
			{
				_003CSearchAsync_003Ed__9 = new _003CSearchAsync_003Ed__8(-3)
				{
					_003C_003E4__this = _003C_003E4__this
				};
			}
			_003CSearchAsync_003Ed__9.query = _003C_003E3__query;
			_003CSearchAsync_003Ed__9.tieBreaker = _003C_003E3__tieBreaker;
			if (_003C_003E3__cancellationToken.Equals(default(CancellationToken)))
			{
				_003CSearchAsync_003Ed__9.cancellationToken = cancellationToken;
			}
			else if (cancellationToken.Equals(_003C_003E3__cancellationToken) || cancellationToken.Equals(default(CancellationToken)))
			{
				_003CSearchAsync_003Ed__9.cancellationToken = _003C_003E3__cancellationToken;
			}
			else
			{
				_003C_003Ex__combinedTokens = CancellationTokenSource.CreateLinkedTokenSource(_003C_003E3__cancellationToken, cancellationToken);
				_003CSearchAsync_003Ed__9.cancellationToken = _003C_003Ex__combinedTokens.Token;
			}
			return _003CSearchAsync_003Ed__9;
		}

		[DebuggerHidden]
		ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
		{
			if (_003C_003E1__state == -2)
			{
				return default(ValueTask<bool>);
			}
			_003C_003Ev__promiseOfValueOrEnd.Reset();
			_003CSearchAsync_003Ed__8 stateMachine = this;
			_003C_003Et__builder.MoveNext(ref stateMachine);
			short version = _003C_003Ev__promiseOfValueOrEnd.Version;
			if (_003C_003Ev__promiseOfValueOrEnd.GetStatus(version) == ValueTaskSourceStatus.Succeeded)
			{
				return new ValueTask<bool>(_003C_003Ev__promiseOfValueOrEnd.GetResult(version));
			}
			return new ValueTask<bool>(this, version);
		}

		[DebuggerHidden]
		bool IValueTaskSource<bool>.GetResult(short token)
		{
			return _003C_003Ev__promiseOfValueOrEnd.GetResult(token);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource<bool>.GetStatus(short token)
		{
			return _003C_003Ev__promiseOfValueOrEnd.GetStatus(token);
		}

		[DebuggerHidden]
		void IValueTaskSource<bool>.OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
		{
			_003C_003Ev__promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags);
		}

		[DebuggerHidden]
		void IValueTaskSource.GetResult(short token)
		{
			_003C_003Ev__promiseOfValueOrEnd.GetResult(token);
		}

		[DebuggerHidden]
		ValueTaskSourceStatus IValueTaskSource.GetStatus(short token)
		{
			return _003C_003Ev__promiseOfValueOrEnd.GetStatus(token);
		}

		[DebuggerHidden]
		void IValueTaskSource.OnCompleted(Action<object?> continuation, object? state, short token, ValueTaskSourceOnCompletedFlags flags)
		{
			_003C_003Ev__promiseOfValueOrEnd.OnCompleted(continuation, state, token, flags);
		}

		[DebuggerHidden]
		ValueTask IAsyncDisposable.DisposeAsync()
		{
			if (_003C_003E1__state >= -1)
			{
				throw new NotSupportedException();
			}
			if (_003C_003E1__state == -2)
			{
				return default(ValueTask);
			}
			_003C_003Ew__disposeMode = true;
			_003C_003Ev__promiseOfValueOrEnd.Reset();
			_003CSearchAsync_003Ed__8 stateMachine = this;
			_003C_003Et__builder.MoveNext(ref stateMachine);
			return new ValueTask(this, _003C_003Ev__promiseOfValueOrEnd.Version);
		}
	}

	private readonly Func<T, IEnumerable<(IEnumerable<string?> Texts, FuzzySearchWeight Weight)>>? defaultKeySelector;

	private readonly List<SearchEntry> entries = new List<SearchEntry>();

	private readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();

	private readonly LRUCache<CacheKey, T[]> cache = new LRUCache<CacheKey, T[]>(64);

	private readonly IPinyinProvider pinyinProvider = new DefaultPinyinProvider();

	private bool disposed;

	public FuzzyMatcher(IEnumerable<T> items, Func<T, IEnumerable<(IEnumerable<string?> Texts, FuzzySearchWeight Weight)>> keySelector)
	{
		FuzzyMatcher<T> fuzzyMatcher = this;
		ArgumentNullException.ThrowIfNull(items, "items");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		defaultKeySelector = keySelector;
		List<T> itemList = items.Where((T item) => item != null).ToList();
		if (itemList.Count == 0)
		{
			return;
		}
		SearchEntry[] builtEntries = new SearchEntry[itemList.Count];
		Parallel.For(0, itemList.Count, delegate(int i)
		{
			T val = itemList[i];
			SearchKey[] keys = (from pair in keySelector(val)
				select new SearchKey(pair.Texts, pair.Weight, fuzzyMatcher.pinyinProvider)).ToArray();
			builtEntries[i] = new SearchEntry(val, keys);
		});
		entries.AddRange(builtEntries);
	}

	public T[] Search(string query, Comparison<T>? tieBreaker = null, int limit = 30, CancellationToken cancellationToken = default(CancellationToken))
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			return Array.Empty<T>();
		}
		cancellationToken.ThrowIfCancellationRequested();
		string text = query.Trim();
		if (text.Any(IsCjk))
		{
			if (Compact(text).Length < 1)
			{
				return Array.Empty<T>();
			}
		}
		else if (Compact(text).Length < 2)
		{
			return Array.Empty<T>();
		}
		string query2 = NormalizeText(text);
		CacheKey key = new CacheKey(query2, limit, tieBreaker);
		lockSlim.EnterReadLock();
		try
		{
			if (cache.TryGet(key, out T[] value))
			{
				return value.ToArray();
			}
		}
		finally
		{
			lockSlim.ExitReadLock();
		}
		List<SearchTerm> terms = ParseTerms(text, pinyinProvider);
		if (terms.Count == 0)
		{
			return Array.Empty<T>();
		}
		List<(T, int)> list = new List<(T, int)>();
		lockSlim.EnterReadLock();
		try
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (entries.Count > 1000)
			{
				ConcurrentBag<(T Item, int Score)> concurrentMatches = new ConcurrentBag<(T, int)>();
				Parallel.ForEach(entries, new ParallelOptions
				{
					CancellationToken = cancellationToken
				}, delegate(SearchEntry entry)
				{
					if (TryScore(entry, terms, out var score2))
					{
						concurrentMatches.Add((entry.Item, score2));
					}
				});
				list.AddRange(concurrentMatches);
			}
			else
			{
				foreach (SearchEntry entry in entries)
				{
					cancellationToken.ThrowIfCancellationRequested();
					if (TryScore(entry, terms, out var score))
					{
						list.Add((entry.Item, score));
					}
				}
			}
		}
		finally
		{
			lockSlim.ExitReadLock();
		}
		cancellationToken.ThrowIfCancellationRequested();
		PriorityQueue<T, MatchPriority> priorityQueue = new PriorityQueue<T, MatchPriority>(list.Count);
		foreach (var item in list)
		{
			MatchPriority priority = new MatchPriority(item.Item2, item.Item1, tieBreaker);
			if (priorityQueue.Count < limit)
			{
				priorityQueue.Enqueue(item.Item1, priority);
				continue;
			}
			priorityQueue.TryPeek(out var _, out var priority2);
			if (priority.CompareTo(priority2) > 0)
			{
				priorityQueue.Dequeue();
				priorityQueue.Enqueue(item.Item1, priority);
			}
		}
		T[] array = new T[priorityQueue.Count];
		for (int num = array.Length - 1; num >= 0; num--)
		{
			array[num] = priorityQueue.Dequeue();
		}
		lockSlim.EnterWriteLock();
		try
		{
			cache.AddOrUpdateAll(new _003C_003Ez__ReadOnlySingleElementList<KeyValuePair<CacheKey, T[]>>(new KeyValuePair<CacheKey, T[]>(key, array)));
		}
		finally
		{
			lockSlim.ExitWriteLock();
		}
		return array.ToArray();
	}

	[AsyncIteratorStateMachine(typeof(FuzzyMatcher<>._003CSearchAsync_003Ed__8))]
	public IAsyncEnumerable<T> SearchAsync(string query, Comparison<T>? tieBreaker = null, [EnumeratorCancellation] CancellationToken cancellationToken = default(CancellationToken))
	{
		return new _003CSearchAsync_003Ed__8(-2)
		{
			_003C_003E4__this = this,
			_003C_003E3__query = query,
			_003C_003E3__tieBreaker = tieBreaker,
			_003C_003E3__cancellationToken = cancellationToken
		};
	}

	public void Add(T item)
	{
		if (defaultKeySelector == null)
		{
			throw new InvalidOperationException("No default key selector was provided in the constructor.");
		}
		Add(item, defaultKeySelector);
	}

	public void Add(T item, Func<T, IEnumerable<(IEnumerable<string?> Texts, FuzzySearchWeight Weight)>> keySelector)
	{
		if (item == null)
		{
			return;
		}
		lockSlim.EnterWriteLock();
		try
		{
			SearchKey[] keys = (from pair in keySelector(item)
				select new SearchKey(pair.Texts, pair.Weight, pinyinProvider)).ToArray();
			entries.Add(new SearchEntry(item, keys));
			cache.Clear();
		}
		finally
		{
			lockSlim.ExitWriteLock();
		}
	}

	public void AddRange(IEnumerable<T> items)
	{
		if (defaultKeySelector == null)
		{
			throw new InvalidOperationException("No default key selector was provided in the constructor.");
		}
		AddRange(items, defaultKeySelector);
	}

	public void AddRange(IEnumerable<T> items, Func<T, IEnumerable<(IEnumerable<string?> Texts, FuzzySearchWeight Weight)>> keySelector)
	{
		ArgumentNullException.ThrowIfNull(items, "items");
		ArgumentNullException.ThrowIfNull(keySelector, "keySelector");
		List<T> itemList = items.Where((T item) => item != null).ToList();
		if (itemList.Count == 0)
		{
			return;
		}
		SearchEntry[] builtEntries = new SearchEntry[itemList.Count];
		Parallel.For(0, itemList.Count, delegate(int i)
		{
			T val = itemList[i];
			SearchKey[] keys = (from pair in keySelector(val)
				select new SearchKey(pair.Texts, pair.Weight, pinyinProvider)).ToArray();
			builtEntries[i] = new SearchEntry(val, keys);
		});
		lockSlim.EnterWriteLock();
		try
		{
			entries.AddRange(builtEntries);
			cache.Clear();
		}
		finally
		{
			lockSlim.ExitWriteLock();
		}
	}

	public bool Remove(T item)
	{
		if (item == null)
		{
			return false;
		}
		lockSlim.EnterWriteLock();
		try
		{
			int num = entries.FindIndex((SearchEntry e) => EqualityComparer<T>.Default.Equals(e.Item, item));
			if (num >= 0)
			{
				entries.RemoveAt(num);
				cache.Clear();
				return true;
			}
			return false;
		}
		finally
		{
			lockSlim.ExitWriteLock();
		}
	}

	public void Clear()
	{
		lockSlim.EnterWriteLock();
		try
		{
			entries.Clear();
			cache.Clear();
		}
		finally
		{
			lockSlim.ExitWriteLock();
		}
	}

	private static bool TryScore(SearchEntry entry, IReadOnlyList<SearchTerm> terms, out int score)
	{
		score = 0;
		int num = 0;
		bool flag = false;
		SearchKey[] keys = entry.Keys;
		for (int i = 0; i < keys.Length; i++)
		{
			if (TryScoreKey(keys[i], terms, out var keyScore))
			{
				flag = true;
				num = Math.Max(num, keyScore);
			}
		}
		if (!flag)
		{
			return false;
		}
		score = num;
		return true;
	}

	private static bool TryScoreKey(SearchKey key, IReadOnlyList<SearchTerm> terms, out int keyScore)
	{
		keyScore = 0;
		foreach (SearchTerm term in terms)
		{
			if (term.IsNegative && AnySegmentMatches(key.Segments, term, key.Weight))
			{
				return false;
			}
		}
		int num = 0;
		int num2 = 0;
		foreach (SearchTerm term2 in terms)
		{
			if (!term2.IsNegative)
			{
				num++;
				int num3 = 0;
				SegmentKey[] segments = key.Segments;
				for (int i = 0; i < segments.Length; i++)
				{
					int val = ScoreSegment(segments[i], term2, key.Weight);
					num3 = Math.Max(num3, val);
				}
				if (num3 == 0)
				{
					return false;
				}
				num2 += num3;
			}
		}
		if (num == 0)
		{
			keyScore = key.Weight.BaseScore;
			return true;
		}
		keyScore = num2 + num * 24;
		return true;
	}

	private static bool AnySegmentMatches(SegmentKey[] segments, SearchTerm term, FuzzySearchWeight weight)
	{
		for (int i = 0; i < segments.Length; i++)
		{
			if (ScoreSegment(segments[i], term, weight) > 0)
			{
				return true;
			}
		}
		return false;
	}

	private static int ScoreSegment(SegmentKey segment, SearchTerm term, FuzzySearchWeight weight)
	{
		if (!term.Mask.IsSubsetOf(segment.Mask))
		{
			return 0;
		}
		if (term.IsExact)
		{
			if (segment.Original.Equals(term.Original, StringComparison.Ordinal))
			{
				return weight.BaseScore * 4 + term.Original.Length * 10;
			}
			if (segment.Original.Contains(term.Original, StringComparison.OrdinalIgnoreCase))
			{
				return weight.BaseScore * 2 + term.Original.Length * 5;
			}
			if (segment.Compact.Contains(term.Compact, StringComparison.Ordinal))
			{
				return weight.BaseScore * 2 + term.Compact.Length * 5;
			}
			return 0;
		}
		int val = 0;
		if (segment.Original.Equals(term.Original, StringComparison.Ordinal))
		{
			int val2 = weight.BaseScore * 4 + term.Original.Length * 10;
			val = Math.Max(val, val2);
		}
		if (segment.Original.StartsWith(term.Original, StringComparison.Ordinal))
		{
			int val3 = weight.BaseScore * 2 + term.Original.Length * 8;
			val = Math.Max(val, val3);
		}
		int val4 = FuzzyMatch(segment.Original, term.Original, weight);
		val = Math.Max(val, val4);
		int val5 = FuzzyMatch(segment.Compact, term.Compact, weight);
		val = Math.Max(val, val5);
		string[] initialisms = segment.Initialisms;
		for (int i = 0; i < initialisms.Length; i++)
		{
			int val6 = FuzzyMatch(initialisms[i], term.Compact, weight with
			{
				BaseScore = weight.InitialismBase
			});
			val = Math.Max(val, val6);
		}
		initialisms = segment.PinyinCombinations;
		for (int i = 0; i < initialisms.Length; i++)
		{
			int num = FuzzyMatch(initialisms[i], term.Compact, weight);
			val = Math.Max(val, (int)((double)num * 0.9));
		}
		if (val == 0 && term.Compact.Length >= 6)
		{
			int val7 = TryUnorderedMatch(segment.Compact, term.Compact, weight);
			val = Math.Max(val, val7);
		}
		if (val == 0 && term.Compact.Length >= 3)
		{
			int val8 = ApproximateMatch(segment.Compact, term.Compact, weight);
			val = Math.Max(val, val8);
		}
		return val;
	}

	private static int FuzzyMatch(string source, string pattern, FuzzySearchWeight weight)
	{
		if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
		{
			return 0;
		}
		if (pattern.Length > source.Length)
		{
			return 0;
		}
		int[] array = null;
		Span<int> span = ((pattern.Length > 256) ? ((Span<int>)(array = ArrayPool<int>.Shared.Rent(pattern.Length))) : stackalloc int[pattern.Length]);
		Span<int> positions = span;
		bool flag = false;
		int i = 0;
		flag = true;
		for (int j = 0; j < pattern.Length; j++)
		{
			for (; i < source.Length && source[i] != pattern[j]; i++)
			{
			}
			if (i >= source.Length)
			{
				flag = false;
				break;
			}
			positions[j] = i;
			i++;
		}
		if (!flag)
		{
			if (array != null)
			{
				ArrayPool<int>.Shared.Return(array);
			}
			return 0;
		}
		int[] array2 = null;
		span = ((pattern.Length > 256) ? ((Span<int>)(array2 = ArrayPool<int>.Shared.Rent(pattern.Length))) : stackalloc int[pattern.Length]);
		Span<int> positions2 = span;
		int num = source.Length - 1;
		for (int num2 = pattern.Length - 1; num2 >= 0; num2--)
		{
			while (num >= 0 && source[num] != pattern[num2])
			{
				num--;
			}
			positions2[num2] = num;
			num--;
		}
		int val = CalculateAlignmentScore(source, positions, pattern.Length, weight);
		int val2 = CalculateAlignmentScore(source, positions2, pattern.Length, weight);
		if (array != null)
		{
			ArrayPool<int>.Shared.Return(array);
		}
		if (array2 != null)
		{
			ArrayPool<int>.Shared.Return(array2);
		}
		return Math.Max(Math.Max(val, val2), 0);
	}

	private static int CalculateAlignmentScore(string source, Span<int> positions, int patternLength, FuzzySearchWeight weight)
	{
		int num = weight.BaseScore;
		for (int i = 0; i < patternLength; i++)
		{
			num += weight.ConsecutiveBonus;
			num += PositionBonus(source, positions[i], weight.BoundaryBonus);
			if (i > 0)
			{
				if (positions[i] == positions[i - 1] + 1)
				{
					num += weight.ConsecutiveBonus * 2;
				}
				int num2 = positions[i] - positions[i - 1] - 1;
				num -= num2 * weight.GapPenalty;
			}
		}
		return num + Math.Max(0, weight.BoundaryBonus - positions[0]);
	}

	private static int PositionBonus(string source, int pos, int boundaryBonus)
	{
		if (pos == 0)
		{
			return boundaryBonus;
		}
		char c = source[pos - 1];
		bool flag;
		switch (c)
		{
		case '\n':
		case ' ':
		case '-':
		case '.':
		case '/':
		case '_':
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			return (int)((double)boundaryBonus * 0.8);
		}
		if (char.IsLower(c) && char.IsUpper(source[pos]))
		{
			return (int)((double)boundaryBonus * 0.6);
		}
		if (char.IsDigit(c) ^ char.IsDigit(source[pos]))
		{
			return (int)((double)boundaryBonus * 0.6);
		}
		return 0;
	}

	private static int ApproximateMatch(string source, string pattern, FuzzySearchWeight weight)
	{
		if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(pattern))
		{
			return 0;
		}
		if (pattern.Length < 3)
		{
			return 0;
		}
		int num = ((pattern.Length <= 5) ? 1 : 2);
		int num2 = int.MaxValue;
		int num3 = source.Length - pattern.Length;
		int num4 = -1;
		for (int i = 0; i <= num3 + num; i++)
		{
			int num5 = Math.Max(0, i - num);
			if (num5 == num4)
			{
				continue;
			}
			num4 = num5;
			int num6 = Math.Min(source.Length, num5 + pattern.Length + num * 2);
			if (num5 < num6)
			{
				int num7 = DamerauLevenshteinDistance(source.AsSpan(num5, num6 - num5), pattern, num);
				if (num7 >= 0)
				{
					num2 = Math.Min(num2, num7);
				}
			}
		}
		if (num2 > num)
		{
			return 0;
		}
		return Math.Max(weight.ApproximateBase + pattern.Length * 4 - num2 * 40, 0);
	}

	private static int DamerauLevenshteinDistance(ReadOnlySpan<char> source, string pattern, int maxDistance)
	{
		int length = source.Length;
		int length2 = pattern.Length;
		int num = length2 + 1;
		int num2 = num * 3;
		int[] array = null;
		int num3;
		Span<int> span;
		if (num2 <= 256)
		{
			num3 = num2;
			span = stackalloc int[num3];
		}
		else
		{
			span = (array = ArrayPool<int>.Shared.Rent(num2));
		}
		Span<int> span2 = span;
		try
		{
			Span<int> span3 = span2.Slice(0, num);
			num3 = num;
			Span<int> span4 = span2.Slice(num3, num * 2 - num3);
			num3 = num * 2;
			Span<int> span5 = span2.Slice(num3, num2 - num3);
			for (int i = 0; i <= length2; i++)
			{
				span3[i] = i;
			}
			int num4 = 1;
			while (true)
			{
				if (num4 <= length)
				{
					span5[0] = num4;
					int num5 = span5[0];
					for (int j = 1; j <= length2; j++)
					{
						int num6 = ((source[num4 - 1] != pattern[j - 1]) ? 1 : 0);
						int val = span4[j] + 1;
						int val2 = span5[j - 1] + 1;
						int num7 = Math.Min(val2: span4[j - 1] + num6, val1: Math.Min(val, val2));
						if (num4 > 1 && j > 1 && source[num4 - 1] == pattern[j - 2] && source[num4 - 2] == pattern[j - 1])
						{
							num7 = Math.Min(num7, span3[j - 2] + 1);
						}
						span5[j] = num7;
						num5 = Math.Min(num5, num7);
					}
					if (num5 > maxDistance)
					{
						num3 = -1;
						break;
					}
					Span<int> span6 = span3;
					span3 = span4;
					span4 = span5;
					span5 = span6;
					num4++;
					continue;
				}
				int num8 = int.MaxValue;
				for (int k = 0; k <= length2; k++)
				{
					num8 = Math.Min(num8, span4[k]);
				}
				num3 = ((num8 <= maxDistance) ? num8 : (-1));
				break;
			}
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<int>.Shared.Return(array);
			}
		}
		return num3;
	}

	private static int TryUnorderedMatch(string source, string query, FuzzySearchWeight weight)
	{
		int num = 0;
		ReadOnlySpan<char> source2 = source.AsSpan();
		for (int i = 3; i <= query.Length - 3; i++)
		{
			ReadOnlySpan<char> pattern = query.AsSpan(0, i);
			ReadOnlySpan<char> pattern2 = query.AsSpan(i);
			if (QuickSubsequence(source2, pattern, out var endPos) && QuickSubsequence(source2.Slice(endPos), pattern2, out var endPos2))
			{
				int num2 = FuzzyMatch(source, pattern.ToString(), weight);
				int num3 = FuzzyMatch(source, pattern2.ToString(), weight);
				if (num2 > 0 && num3 > 0)
				{
					num = Math.Max(num, num2 + num3);
				}
			}
			if (QuickSubsequence(source2, pattern2, out var endPos3) && QuickSubsequence(source2.Slice(endPos3), pattern, out endPos2))
			{
				int num4 = FuzzyMatch(source, pattern2.ToString(), weight);
				int num5 = FuzzyMatch(source, pattern.ToString(), weight);
				if (num4 > 0 && num5 > 0)
				{
					num = Math.Max(num, num4 + num5);
				}
			}
		}
		return num;
	}

	private static bool QuickSubsequence(ReadOnlySpan<char> source, ReadOnlySpan<char> pattern, out int endPos)
	{
		endPos = 0;
		int i = 0;
		ReadOnlySpan<char> readOnlySpan = pattern;
		for (int j = 0; j < readOnlySpan.Length; j++)
		{
			for (char c = readOnlySpan[j]; i < source.Length && source[i] != c; i++)
			{
			}
			if (i >= source.Length)
			{
				return false;
			}
			i++;
		}
		endPos = i;
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static bool IsCjk(char ch)
	{
		if (ch >= '一')
		{
			return ch <= '鿿';
		}
		return false;
	}

	private static string NormalizeText(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		bool flag = false;
		string text2 = text.Trim();
		foreach (char c in text2)
		{
			if (char.IsWhiteSpace(c))
			{
				flag = stringBuilder.Length > 0;
				continue;
			}
			if (flag)
			{
				stringBuilder.Append(' ');
				flag = false;
			}
			stringBuilder.Append(char.ToLowerInvariant(c));
		}
		return stringBuilder.ToString();
	}

	private static string Compact(string? text)
	{
		if (string.IsNullOrWhiteSpace(text))
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder(text.Length);
		foreach (char c in text)
		{
			if (char.IsLetterOrDigit(c))
			{
				stringBuilder.Append(char.ToLowerInvariant(c));
			}
		}
		return stringBuilder.ToString();
	}

	private static List<string> SplitByCase(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return new List<string>();
		}
		List<string> list = new List<string>();
		int length = text.Length;
		int num = 0;
		for (int i = 0; i < length; i++)
		{
			if (i == length - 1)
			{
				int num2 = num;
				list.Add(text.Substring(num2, length - num2));
				break;
			}
			char c = text[i];
			char c2 = text[i + 1];
			bool flag = false;
			if (IsCjk(c) != IsCjk(c2))
			{
				flag = true;
			}
			else if (char.IsDigit(c) != char.IsDigit(c2))
			{
				flag = true;
			}
			else if (char.IsLower(c) && char.IsUpper(c2))
			{
				flag = true;
			}
			else if (char.IsUpper(c) && char.IsUpper(c2) && i + 2 < length && char.IsLower(text[i + 2]))
			{
				flag = true;
			}
			if (flag)
			{
				int num2 = num;
				list.Add(text.Substring(num2, i + 1 - num2));
				num = i + 1;
			}
		}
		return (from s in list
			select s.Trim() into s
			where s.Length > 0
			select s).ToList();
	}

	private static void AppendAsciiInitialism(StringBuilder builder, string text)
	{
		foreach (string item in SplitByCase(text))
		{
			if (item.Length == 0)
			{
				continue;
			}
			bool flag = true;
			string text2 = item;
			foreach (char c in text2)
			{
				if (char.IsLetter(c) && !char.IsUpper(c))
				{
					flag = false;
					break;
				}
			}
			if (flag && item.Length > 1)
			{
				text2 = item;
				foreach (char c2 in text2)
				{
					if (char.IsLetterOrDigit(c2))
					{
						builder.Append(char.ToLowerInvariant(c2));
					}
				}
			}
			else
			{
				char c3 = item[0];
				if (char.IsLetterOrDigit(c3))
				{
					builder.Append(char.ToLowerInvariant(c3));
				}
			}
		}
	}

	private static List<SearchTerm> ParseTerms(string rawQuery, IPinyinProvider pinyinProvider)
	{
		if (string.IsNullOrWhiteSpace(rawQuery))
		{
			return new List<SearchTerm>();
		}
		List<SearchTerm> list = new List<SearchTerm>();
		string text = rawQuery.Trim();
		while (true)
		{
			int num = text.IndexOf('"');
			if (num == -1)
			{
				break;
			}
			int num2 = text.IndexOf('"', num + 1);
			if (num2 == -1)
			{
				break;
			}
			string text2 = text.Substring(num + 1, num2 - num - 1).Trim();
			if (text2.Length > 0)
			{
				list.Add(CreateSearchTerm(text2, isExact: true, isNegative: false, pinyinProvider));
			}
			text = (text.Substring(0, num) + " " + text.Substring(num2 + 1)).Trim();
		}
		string[] array = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (string obj in array)
		{
			bool isNegative = false;
			string text3 = obj;
			if (text3.StartsWith('-') && text3.Length > 1)
			{
				isNegative = true;
				text3 = text3.Substring(1);
			}
			foreach (string item in SplitByCase(text3))
			{
				list.Add(CreateSearchTerm(item, isExact: false, isNegative, pinyinProvider));
			}
		}
		return list;
	}

	private static SearchTerm CreateSearchTerm(string text, bool isExact, bool isNegative, IPinyinProvider pinyinProvider)
	{
		string text2 = NormalizeText(text);
		string text3 = Compact(text);
		bool flag = text.Any(IsCjk);
		string[] pinyinCombinations = Array.Empty<string>();
		if (flag)
		{
			pinyinCombinations = pinyinProvider.GetPinyinCombinations(text2);
		}
		return new SearchTerm(text2, text3, pinyinCombinations, flag, isExact, isNegative, new CharMask(text3 + (flag ? "一" : "")));
	}

	public void Dispose()
	{
		if (!disposed)
		{
			cache.Dispose();
			lockSlim.Dispose();
			disposed = true;
		}
	}
}
