using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.Info.Collections;

public class LRUCache<TKey, TValue> : IDisposable where TKey : notnull
{
	private readonly record struct CacheItem(TKey Key, TValue Value, DateTime ExpirationTime, bool IsPermanent = false);

	private readonly Dictionary<TKey, LinkedListNode<CacheItem>> cache = new Dictionary<TKey, LinkedListNode<CacheItem>>();

	private readonly LinkedList<CacheItem> lruList = new LinkedList<CacheItem>();

	private readonly Lock gate = new Lock();

	private readonly int capacity;

	private readonly TimeSpan defaultExpiration;

	private readonly Timer cleanupTimer;

	private readonly TimeSpan cleanupInterval;

	private bool isDisposed;

	private long totalRequests;

	private long cacheHits;

	public int Count
	{
		get
		{
			using (gate.EnterScope())
			{
				return cache.Count;
			}
		}
	}

	public double HitRate
	{
		get
		{
			long num = Interlocked.Read(in totalRequests);
			if (num <= 0)
			{
				return 0.0;
			}
			return (double)Interlocked.Read(in cacheHits) / (double)num;
		}
	}

	public long TotalRequests => Interlocked.Read(in totalRequests);

	public long CacheHits => Interlocked.Read(in cacheHits);

	public LRUCache(int capacity, TimeSpan? defaultExpiration = null, TimeSpan? cleanupInterval = null)
	{
		if (capacity <= 0)
		{
			throw new ArgumentException("容量必须大于 0", "capacity");
		}
		this.capacity = capacity;
		this.defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30L);
		this.cleanupInterval = cleanupInterval ?? TimeSpan.FromMinutes(5L);
		cleanupTimer = new Timer(CleanupExpiredItems, null, this.cleanupInterval, this.cleanupInterval);
		RegisterMemoryPressureNotification();
	}

	public ImmutableList<KeyValuePair<TKey, TValue>> GetItems()
	{
		return GetSnapshotItems().ToImmutableList();
	}

	public KeyValuePair<TKey, TValue>[] GetSnapshotItems()
	{
		using (gate.EnterScope())
		{
			KeyValuePair<TKey, TValue>[] array = new KeyValuePair<TKey, TValue>[cache.Count];
			int num = 0;
			foreach (KeyValuePair<TKey, LinkedListNode<CacheItem>> item in cache)
			{
				array[num++] = new KeyValuePair<TKey, TValue>(item.Key, item.Value.Value.Value);
			}
			return array;
		}
	}

	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
	{
		return GetOrAdd(key, valueFactory, defaultExpiration);
	}

	public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory, TimeSpan expiration)
	{
		Interlocked.Increment(ref totalRequests);
		using (gate.EnterScope())
		{
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			if (TryGetNodeValueNoLock(key, uTCNow, out LinkedListNode<CacheItem> node, out TValue value))
			{
				MoveToFrontNoLock(node);
				Interlocked.Increment(ref cacheHits);
				return value;
			}
			value = valueFactory(key);
			AddNewNoLock(key, value, uTCNow.Add(expiration), isPermanent: false);
			return value;
		}
	}

	public TValue GetOrAddPermanent(TKey key, Func<TKey, TValue> valueFactory)
	{
		Interlocked.Increment(ref totalRequests);
		using (gate.EnterScope())
		{
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			if (TryGetNodeValueNoLock(key, uTCNow, out LinkedListNode<CacheItem> node, out TValue value))
			{
				MoveToFrontNoLock(node);
				Interlocked.Increment(ref cacheHits);
				return value;
			}
			value = valueFactory(key);
			AddNewNoLock(key, value, DateTime.MaxValue, isPermanent: true);
			return value;
		}
	}

	public bool TryGet(TKey key, out TValue value)
	{
		Interlocked.Increment(ref totalRequests);
		using (gate.EnterScope())
		{
			if (!TryGetNodeValueNoLock(key, OmenServiceBase<StandardTimeManager>.Instance().UTCNow, out LinkedListNode<CacheItem> node, out value))
			{
				return false;
			}
			MoveToFrontNoLock(node);
			Interlocked.Increment(ref cacheHits);
			return true;
		}
	}

	public bool TryUpdate(TKey key, TValue newValue, TimeSpan? expiration = null, bool? isPermanent = null)
	{
		using (gate.EnterScope())
		{
			if (!cache.TryGetValue(key, out LinkedListNode<CacheItem> value))
			{
				return false;
			}
			bool flag = isPermanent ?? value.Value.IsPermanent;
			DateTime expirationTime = (flag ? DateTime.MaxValue : OmenServiceBase<StandardTimeManager>.Instance().UTCNow.Add(expiration ?? defaultExpiration));
			value.Value = new CacheItem(key, newValue, expirationTime, flag);
			MoveToFrontNoLock(value);
			return true;
		}
	}

	public bool TryRemove(TKey key, out TValue value)
	{
		using (gate.EnterScope())
		{
			if (!cache.TryGetValue(key, out LinkedListNode<CacheItem> value2))
			{
				value = default(TValue);
				return false;
			}
			value = value2.Value.Value;
			RemoveNodeNoLock(value2);
			return true;
		}
	}

	public void Clear()
	{
		using (gate.EnterScope())
		{
			if (cache.Count == 0)
			{
				ResetStats();
				return;
			}
			List<CacheItem> list = new List<CacheItem>();
			foreach (CacheItem lru in lruList)
			{
				if (lru.IsPermanent)
				{
					list.Add(lru);
				}
			}
			cache.Clear();
			lruList.Clear();
			foreach (CacheItem item in list)
			{
				LinkedListNode<CacheItem> value = lruList.AddFirst(item);
				cache.Add(item.Key, value);
			}
			ResetStats();
		}
	}

	public void ClearAll(bool includePermanentItems = false)
	{
		if (includePermanentItems)
		{
			using (gate.EnterScope())
			{
				cache.Clear();
				lruList.Clear();
				ResetStats();
				return;
			}
		}
		Clear();
	}

	public void RemoveAll(IEnumerable<TKey> keys)
	{
		using (gate.EnterScope())
		{
			foreach (TKey key in keys)
			{
				if (cache.TryGetValue(key, out LinkedListNode<CacheItem> value))
				{
					RemoveNodeNoLock(value);
				}
			}
		}
	}

	public void AddOrUpdateAll(IEnumerable<KeyValuePair<TKey, TValue>> items, TimeSpan? expiration = null, bool isPermanent = false)
	{
		using (gate.EnterScope())
		{
			DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
			DateTime expirationTime = (isPermanent ? DateTime.MaxValue : uTCNow.Add(expiration ?? defaultExpiration));
			foreach (KeyValuePair<TKey, TValue> item in items)
			{
				if (cache.TryGetValue(item.Key, out LinkedListNode<CacheItem> value))
				{
					value.Value = new CacheItem(item.Key, item.Value, expirationTime, isPermanent);
					MoveToFrontNoLock(value);
				}
				else
				{
					AddNewNoLock(item.Key, item.Value, expirationTime, isPermanent);
				}
			}
		}
	}

	public void AddOrUpdatePermanent(TKey key, TValue value)
	{
		using (gate.EnterScope())
		{
			if (cache.TryGetValue(key, out LinkedListNode<CacheItem> value2))
			{
				value2.Value = new CacheItem(key, value, DateTime.MaxValue, IsPermanent: true);
				MoveToFrontNoLock(value2);
			}
			else
			{
				AddNewNoLock(key, value, DateTime.MaxValue, isPermanent: true);
			}
		}
	}

	public void ResetStats()
	{
		Interlocked.Exchange(ref totalRequests, 0L);
		Interlocked.Exchange(ref cacheHits, 0L);
	}

	public void Dispose()
	{
		if (!isDisposed)
		{
			isDisposed = true;
			ClearAll(includePermanentItems: true);
			cleanupTimer.Dispose();
		}
	}

	private bool TryGetNodeValueNoLock(TKey key, DateTime utcNow, out LinkedListNode<CacheItem> node, out TValue value)
	{
		if (!cache.TryGetValue(key, out node))
		{
			value = default(TValue);
			return false;
		}
		if (!node.Value.IsPermanent && utcNow > node.Value.ExpirationTime)
		{
			RemoveNodeNoLock(node);
			value = default(TValue);
			return false;
		}
		value = node.Value.Value;
		return true;
	}

	private void AddNewNoLock(TKey key, TValue value, DateTime expirationTime, bool isPermanent)
	{
		if (cache.Count >= capacity)
		{
			RemoveOldestNoLock();
		}
		LinkedListNode<CacheItem> value2 = lruList.AddFirst(new CacheItem(key, value, expirationTime, isPermanent));
		cache[key] = value2;
	}

	private void RemoveNodeNoLock(LinkedListNode<CacheItem> node)
	{
		lruList.Remove(node);
		cache.Remove(node.Value.Key);
	}

	private void MoveToFrontNoLock(LinkedListNode<CacheItem> node)
	{
		if (lruList.First != node)
		{
			lruList.Remove(node);
			lruList.AddFirst(node);
		}
	}

	private void RemoveOldestNoLock()
	{
		LinkedListNode<CacheItem> linkedListNode = lruList.Last;
		while (linkedListNode != null && linkedListNode.Value.IsPermanent)
		{
			linkedListNode = linkedListNode.Previous;
		}
		if (linkedListNode != null)
		{
			RemoveNodeNoLock(linkedListNode);
		}
		else if (lruList.Last != null)
		{
			RemoveNodeNoLock(lruList.Last);
		}
	}

	private void CleanupExpiredItems(object? state)
	{
		if (isDisposed)
		{
			return;
		}
		try
		{
			using (gate.EnterScope())
			{
				DateTime uTCNow = OmenServiceBase<StandardTimeManager>.Instance().UTCNow;
				LinkedListNode<CacheItem> linkedListNode = lruList.Last;
				while (linkedListNode != null)
				{
					LinkedListNode<CacheItem>? previous = linkedListNode.Previous;
					if (!linkedListNode.Value.IsPermanent && linkedListNode.Value.ExpirationTime < uTCNow)
					{
						RemoveNodeNoLock(linkedListNode);
					}
					linkedListNode = previous;
				}
			}
		}
		catch (ObjectDisposedException)
		{
		}
		catch (Exception)
		{
		}
	}

	private void RegisterMemoryPressureNotification()
	{
		Task.Run(async delegate
		{
			while (!isDisposed)
			{
				try
				{
					if (GC.GetTotalMemory(forceFullCollection: false) > 104857600)
					{
						TrimCache(0.5);
					}
					await Task.Delay(TimeSpan.FromMinutes(1L));
				}
				catch (Exception)
				{
				}
			}
		});
	}

	private void TrimCache(double percentage)
	{
		if ((percentage <= 0.0 || percentage > 1.0) ? true : false)
		{
			throw new ArgumentOutOfRangeException("percentage", "百分比必须在 0 到 1 之间");
		}
		using (gate.EnterScope())
		{
			int num = 0;
			foreach (CacheItem lru in lruList)
			{
				if (!lru.IsPermanent)
				{
					num++;
				}
			}
			int num2 = (int)((double)num * percentage);
			LinkedListNode<CacheItem> linkedListNode = lruList.Last;
			int num3 = 0;
			while (num3 < num2 && linkedListNode != null)
			{
				LinkedListNode<CacheItem>? previous = linkedListNode.Previous;
				if (!linkedListNode.Value.IsPermanent)
				{
					RemoveNodeNoLock(linkedListNode);
					num3++;
				}
				linkedListNode = previous;
			}
		}
	}
}
