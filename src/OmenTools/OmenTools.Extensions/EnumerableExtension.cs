using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OmenTools.Extensions;

public static class EnumerableExtension
{
	extension<T>(ICollection<T> collection)
	{
		public void AddRange(params T[] values)
		{
			foreach (T item in values)
			{
				collection.Add(item);
			}
		}

		public void RemoveRange(params T[] values)
		{
			foreach (T item in values)
			{
				collection.Remove(item);
			}
		}

		public void RemoveRange(IEnumerable<T> values)
		{
			foreach (T value in values)
			{
				collection.Remove(value);
			}
		}

		public void Add(params T[] values)
		{
			collection.AddRange(values);
		}

		public void Remove(params T[] values)
		{
			collection.RemoveRange(values);
		}

		public void AddRange(IEnumerable<T> items)
		{
			foreach (T item in items)
			{
				collection.Add(item);
			}
		}
	}

	extension<T>(ConcurrentBag<T> bag)
	{
		public void AddRange(IEnumerable<T> items)
		{
			if (bag == null)
			{
				throw new ArgumentNullException("bag");
			}
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (T item in items)
			{
				bag.Add(item);
			}
		}

		public void ForEach(Action<T> action)
		{
			if (bag == null)
			{
				throw new ArgumentNullException("bag");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in bag)
			{
				action(item);
			}
		}
	}

	extension<T>(IEnumerable<T> source)
	{
		public ConcurrentBag<T> ToConcurrentBag()
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			return new ConcurrentBag<T>(source);
		}

		public ConcurrentDictionary<TKey, TValue> ToConcurrentDictionary<TKey, TValue>(Func<T, TKey> keySelector, Func<T, TValue> valueSelector) where TKey : notnull
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (keySelector == null)
			{
				throw new ArgumentNullException("keySelector");
			}
			if (valueSelector == null)
			{
				throw new ArgumentNullException("valueSelector");
			}
			return new ConcurrentDictionary<TKey, TValue>(source.ToDictionary(keySelector, valueSelector));
		}

		public ConcurrentDictionary<TKey, T> ToConcurrentDictionary<TKey>(Func<T, TKey> keySelector) where TKey : notnull
		{
			return source.ToConcurrentDictionary(keySelector, (T item) => item);
		}

		public void ForEach(Action<T> action)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (T item in source)
			{
				action(item);
			}
		}

		public T FirstOr0(Func<T, bool> predicate)
		{
			T[] array = (source as T[]) ?? source.ToArray();
			T[] array2 = array;
			foreach (T val in array2)
			{
				if (predicate(val))
				{
					return val;
				}
			}
			return array.First();
		}

		public IEnumerable<TR> SelectMulti<TR>(params Func<T, TR>[] funcs)
		{
			return from v in source
				from x in funcs
				select x(v);
		}

		public bool ContainsAll(IEnumerable<T> values)
		{
			return values.All(source.Contains<T>);
		}

		public bool ContainsAny(params T[] values)
		{
			T[] enumerable = (source as T[]) ?? source.ToArray();
			return values.Any((T x) => enumerable.Contains(x, null));
		}

		public bool ContainsAny(IEnumerable<T> values)
		{
			T[] enumerable = (source as T[]) ?? source.ToArray();
			return values.Any((T x) => enumerable.Contains(x, null));
		}

		public int IndexOf(Predicate<T> predicate)
		{
			int num = -1;
			foreach (T item in source)
			{
				num++;
				if (predicate(item))
				{
					return num;
				}
			}
			return -1;
		}
	}

	extension<T>(IList<T> list)
	{
		public void Swap(int index1, int index2)
		{
			if (index1 < 0 || index1 >= list.Count || index2 < 0 || index2 >= list.Count)
			{
				throw new IndexOutOfRangeException("无法交换元素, 因为其中一个索引无效");
			}
			T value = list[index2];
			T value2 = list[index1];
			list[index1] = value;
			list[index2] = value2;
		}

		public bool TryDequeue(out T? result)
		{
			if (list.Count > 0)
			{
				result = list[0];
				list.RemoveAt(0);
				return true;
			}
			result = default(T);
			return false;
		}

		public T? Dequeue()
		{
			if (list.TryDequeue(out T result))
			{
				return result;
			}
			throw new InvalidOperationException("Sequence contains no elements");
		}

		public bool TryDequeueLast(out T? result)
		{
			if (list.Count > 0)
			{
				result = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				return true;
			}
			result = default(T);
			return false;
		}

		public T? DequeueLast()
		{
			if (list.TryDequeueLast(out T result))
			{
				return result;
			}
			throw new InvalidOperationException("序列中未包含任何有效元素");
		}

		public T? DequeueOrDefault()
		{
			if (list.Count <= 0)
			{
				return default(T);
			}
			T result = list[0];
			list.RemoveAt(0);
			return result;
		}
	}

	extension<TKey, TValue>(ConcurrentDictionary<TKey, TValue> dictionary) where TKey : notnull
	{
		public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (items == null)
			{
				throw new ArgumentNullException("items");
			}
			foreach (KeyValuePair<TKey, TValue> item in items)
			{
				dictionary.TryAdd(item.Key, item.Value);
			}
		}

		public void ForEach(Action<TKey, TValue> action)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException("dictionary");
			}
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			foreach (KeyValuePair<TKey, TValue> item in dictionary)
			{
				action(item.Key, item.Value);
			}
		}
	}

	extension<TK, TV>(IDictionary<TK, TV> dictionary)
	{
		public IEnumerable<TK> FindKeysByValue(TV value)
		{
			return dictionary.Where(delegate(KeyValuePair<TK, TV> x)
			{
				if (value != null)
				{
					ref TV reference = ref value;
					TV val = default(TV);
					if (val == null)
					{
						val = reference;
						reference = ref val;
					}
					KeyValuePair<TK, TV> keyValuePair = x;
					object obj = keyValuePair.Value;
					return reference.Equals(obj);
				}
				return false;
			}).Select(delegate(KeyValuePair<TK, TV> x)
			{
				KeyValuePair<TK, TV> keyValuePair = x;
				return keyValuePair.Key;
			});
		}

		public bool TryGetFirst(Func<KeyValuePair<TK, TV>, bool> predicate, out KeyValuePair<TK, TV> keyValuePair)
		{
			try
			{
				keyValuePair = dictionary.First(predicate);
				return true;
			}
			catch
			{
				keyValuePair = default(KeyValuePair<TK, TV>);
				return false;
			}
		}

		public TV? GetOrDefault(TK key)
		{
			if (!dictionary.TryGetValue(key, out var value))
			{
				return default(TV);
			}
			return value;
		}

		public TV GetOrAdd(TK key, Func<TK, TV> valueFactory)
		{
			if (dictionary.TryGetValue(key, out var value))
			{
				return value;
			}
			return dictionary[key] = valueFactory(key);
		}
	}

	public static T? DequeueOrDefault<T>(this Queue<T?> queue)
	{
		if (queue.Count <= 0)
		{
			return default(T);
		}
		return queue.Dequeue();
	}

	public static bool Toggle<T>(this HashSet<T> hashSet, T value)
	{
		if (!hashSet.Add(value))
		{
			hashSet.Remove(value);
			return false;
		}
		return true;
	}

	public static bool TryGetFirst<TSource>(this IEnumerable<TSource>? source, out TSource? value)
	{
		if (source != null)
		{
			if (source is IList<TSource> list)
			{
				if (list.Count > 0)
				{
					value = list[0];
					return true;
				}
			}
			else
			{
				using IEnumerator<TSource> enumerator = source.GetEnumerator();
				if (enumerator.MoveNext())
				{
					value = enumerator.Current;
					return true;
				}
			}
		}
		value = default(TSource);
		return false;
	}

	public static bool TryGetFirst<TSource>(this IEnumerable<TSource?>? source, Func<TSource, bool>? predicate, out TSource? value)
	{
		if (source == null || predicate == null)
		{
			value = default(TSource);
			return false;
		}
		foreach (TSource item in source)
		{
			if (item != null && predicate(item))
			{
				value = item;
				return true;
			}
		}
		value = default(TSource);
		return false;
	}

	public static bool TryGetLast<TK>(this IEnumerable<TK?> enumerable, Func<TK?, bool> predicate, out TK? value)
	{
		try
		{
			value = enumerable.Last(predicate);
			return true;
		}
		catch
		{
			value = default(TK);
			return false;
		}
	}

	public static bool Toggle<T>(this List<T> list, T value)
	{
		if (list.Contains(value))
		{
			list.RemoveAll((T x) => x?.Equals(value) ?? false);
			return false;
		}
		list.Add(value);
		return true;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int IncrementOrSet<TK>(this IDictionary<TK, int> dic, TK key, int increment = 1)
	{
		if (!dic.TryAdd(key, increment))
		{
			dic[key] += increment;
		}
		return dic[key];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TV? GetSafe<TK, TV>(this IDictionary<TK, TV>? dic, TK key, TV? defaultValue = default(TV?))
	{
		if (dic == null || !dic.TryGetValue(key, out TV value))
		{
			return defaultValue;
		}
		return value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TV GetOrCreate<TK, TV>(this IDictionary<TK, TV> dictionary, TK key) where TV : new()
	{
		if (dictionary.TryGetValue(key, out TV value))
		{
			return value;
		}
		TV val = new TV();
		dictionary.Add(key, val);
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool EqualsAny<T>(this T obj, IEnumerable<T> values)
	{
		return values.Any((T x) => x?.Equals(obj) ?? false);
	}

	public static T[] Together<T>(this T[] array, params T[] additionalValues)
	{
		return array.Union(additionalValues).ToArray();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T[] CreateArray<T>(this T o, uint repeatCount)
	{
		T[] array = new T[repeatCount];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = o;
		}
		return array;
	}
}
