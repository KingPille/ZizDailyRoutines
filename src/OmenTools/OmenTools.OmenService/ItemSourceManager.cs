using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lumina.Data;
using Microsoft.Win32.SafeHandles;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.Info.Collections;
using OmenTools.Info.Game.ItemSource;
using OmenTools.Info.Game.ItemSource.Enums;
using OmenTools.Info.Game.ItemSource.Models;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public sealed class ItemSourceManager : OmenServiceBase<ItemSourceManager>
{
	private enum RepositoryStatus
	{
		Building,
		Ready,
		Failed
	}

	private readonly record struct ItemOffset(uint ItemID, long Offset, int Length);

	private readonly record struct IndexEntry(uint ItemID, int Offset, int Length);

	private readonly record struct LocationKey(float X, float Y, uint TerritoryID, uint MapID);

	private readonly record struct SectionInfo(long Offset, int Length, uint Checksum);

	private readonly record struct SnapshotHeader(uint Magic, int FormatVersion, string ClientVersion, Language ClientLanguage, SectionInfo StringSection, SectionInfo LocationSection, SectionInfo IndexSection, SectionInfo ReverseIndexSection, SectionInfo DataSection)
	{
		[CompilerGenerated]
		public void Deconstruct(out uint Magic, out int FormatVersion, out string ClientVersion, out Language ClientLanguage, out SectionInfo StringSection, out SectionInfo LocationSection, out SectionInfo IndexSection, out SectionInfo ReverseIndexSection, out SectionInfo DataSection)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected I4, but got Unknown
			Magic = this.Magic;
			FormatVersion = this.FormatVersion;
			ClientVersion = this.ClientVersion;
			ClientLanguage = (Language)(int)this.ClientLanguage;
			StringSection = this.StringSection;
			LocationSection = this.LocationSection;
			IndexSection = this.IndexSection;
			ReverseIndexSection = this.ReverseIndexSection;
			DataSection = this.DataSection;
		}
	}

	private static readonly FrozenDictionary<uint, ItemOffset> EmptyOffsets = new Dictionary<uint, ItemOffset>().ToFrozenDictionary();

	private static readonly FrozenDictionary<uint, uint[]> EmptyReverseLookup = new Dictionary<uint, uint[]>().ToFrozenDictionary();

	private readonly Lock snapshotGate = new Lock();

	private readonly LRUCache<uint, ItemSourceQueryResult> hotCache = new LRUCache<uint, ItemSourceQueryResult>(30);

	private readonly LRUCache<uint, ExchangeItemsQueryResult> reverseHotCache = new LRUCache<uint, ExchangeItemsQueryResult>(30);

	private FrozenDictionary<uint, ItemOffset> itemOffsets = EmptyOffsets;

	private FrozenDictionary<uint, uint[]> reverseItemLookup = EmptyReverseLookup;

	private string[] stringTable = Array.Empty<string>();

	private ShopNPCLocation[] locationTable = Array.Empty<ShopNPCLocation>();

	private FileStream? snapshotStream;

	private RepositoryStatus status;

	private int buildStarted;

	private const int SNAPSHOT_FORMAT_VERSION = 5;

	private const uint SNAPSHOT_MAGIC = 1230197315u;

	private const int HOT_CACHE_CAPACITY = 30;

	public ItemSourceQueryResult Query(uint itemID)
	{
		if (itemID == 0)
		{
			return ItemSourceQueryResult.NotFound;
		}
		RepositoryStatus repositoryStatus = status;
		FrozenDictionary<uint, ItemOffset> frozenDictionary;
		string[] currentStrings;
		ShopNPCLocation[] currentLocations;
		SafeFileHandle currentHandle;
		using (snapshotGate.EnterScope())
		{
			frozenDictionary = itemOffsets;
			currentStrings = stringTable;
			currentLocations = locationTable;
			currentHandle = snapshotStream?.SafeFileHandle;
		}
		if (frozenDictionary.TryGetValue(itemID, out var itemOffset) && currentHandle != null)
		{
			return hotCache.GetOrAdd(itemID, (uint _) => DecodeItem(currentHandle, itemOffset, currentStrings, currentLocations.ToList()));
		}
		return repositoryStatus switch
		{
			RepositoryStatus.Ready => ItemSourceQueryResult.NotFound, 
			RepositoryStatus.Failed => ItemSourceQueryResult.Failed, 
			_ => ItemSourceQueryResult.Building, 
		};
	}

	public ExchangeItemsQueryResult QueryExchangeItems(uint costItemID)
	{
		if (costItemID == 0)
		{
			return ExchangeItemsQueryResult.NotFound;
		}
		RepositoryStatus repositoryStatus = status;
		FrozenDictionary<uint, ItemOffset> currentOffsets;
		FrozenDictionary<uint, uint[]> frozenDictionary;
		string[] currentStrings;
		ShopNPCLocation[] currentLocations;
		SafeFileHandle currentHandle;
		using (snapshotGate.EnterScope())
		{
			currentOffsets = itemOffsets;
			frozenDictionary = reverseItemLookup;
			currentStrings = stringTable;
			currentLocations = locationTable;
			currentHandle = snapshotStream?.SafeFileHandle;
		}
		if (currentHandle != null && frozenDictionary.TryGetValue(costItemID, out var targetItemIDs))
		{
			return reverseHotCache.GetOrAdd(costItemID, (uint _) => BuildExchangeQueryResult(costItemID, targetItemIDs, currentHandle, currentOffsets, currentStrings, currentLocations));
		}
		return repositoryStatus switch
		{
			RepositoryStatus.Ready => ExchangeItemsQueryResult.NotFound, 
			RepositoryStatus.Failed => ExchangeItemsQueryResult.Failed, 
			_ => ExchangeItemsQueryResult.Building, 
		};
	}

	protected override void Init()
	{
		if (TryLoadSnapshot(out string _))
		{
			status = RepositoryStatus.Ready;
			return;
		}
		status = RepositoryStatus.Building;
		EnsureBackgroundBuild();
	}

	protected override void Uninit()
	{
		hotCache.Dispose();
		reverseHotCache.Dispose();
		using (snapshotGate.EnterScope())
		{
			snapshotStream?.Dispose();
			snapshotStream = null;
			itemOffsets = EmptyOffsets;
			reverseItemLookup = EmptyReverseLookup;
			stringTable = Array.Empty<string>();
			locationTable = Array.Empty<ShopNPCLocation>();
		}
	}

	private void EnsureBackgroundBuild()
	{
		if (Interlocked.CompareExchange(ref buildStarted, 1, 0) != 0)
		{
			return;
		}
		Task.Run(async delegate
		{
			_ = 1;
			try
			{
				status = RepositoryStatus.Building;
				Stopwatch versionWaitStopwatch = Stopwatch.StartNew();
				while (string.IsNullOrEmpty(GameState.ClientVersion) && versionWaitStopwatch.ElapsedMilliseconds < 30000)
				{
					await Task.Delay(500).ConfigureAwait(continueOnCapturedContext: false);
				}
				if (string.IsNullOrEmpty(GameState.ClientVersion))
				{
					status = RepositoryStatus.Failed;
					DLog.Warning("[ItemSourceRepository] 等待游戏版本超时, 放弃构建快照");
				}
				else
				{
					string snapshotPath = GetSnapshotPath();
					Dictionary<uint, ItemSourceInfo> items = ItemSourceInfo.BuildAllItems();
					await WriteSnapshotAsync(snapshotPath, items).ConfigureAwait(continueOnCapturedContext: false);
					if (!TryLoadSnapshot(out string _))
					{
						throw new InvalidOperationException("快照已写入, 但重新装载失败");
					}
					hotCache.ClearAll(includePermanentItems: true);
					reverseHotCache.ClearAll(includePermanentItems: true);
					status = RepositoryStatus.Ready;
					DLog.Debug($"[ItemSourceRepository] 快照已就绪, 条目数: {itemOffsets.Count}");
				}
			}
			catch (Exception ex)
			{
				status = RepositoryStatus.Failed;
				DLog.Error("[ItemSourceRepository] 构建物品来源快照失败", ex);
			}
			finally
			{
				Interlocked.Exchange(ref buildStarted, 0);
			}
		});
	}

	private bool TryLoadSnapshot(out string? failureReason)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		failureReason = null;
		string snapshotPath = GetSnapshotPath();
		if (!File.Exists(snapshotPath))
		{
			failureReason = "快照文件不存在";
			return false;
		}
		try
		{
			byte[] array = File.ReadAllBytes(snapshotPath);
			using MemoryStream input = new MemoryStream(array, writable: false);
			using BinaryReader reader = new BinaryReader(input, Encoding.UTF8, leaveOpen: true);
			SnapshotHeader header = ReadHeader(reader);
			ValidateHeader(header, array.Length);
			if (string.IsNullOrEmpty(GameState.ClientVersion))
			{
				failureReason = "游戏版本尚未就绪";
				return false;
			}
			if (!string.Equals(header.ClientVersion, GameState.ClientVersion, StringComparison.Ordinal))
			{
				throw new InvalidDataException("客户端版本不匹配, 当前: " + GameState.ClientVersion + ", 快照: " + header.ClientVersion);
			}
			if (header.ClientLanguage != GameState.ClientLanguage)
			{
				throw new InvalidDataException($"客户端语言不匹配, 当前: {GameState.ClientLanguage}, 快照: {header.ClientLanguage}");
			}
			ValidateChecksum(array, header.StringSection);
			ValidateChecksum(array, header.LocationSection);
			ValidateChecksum(array, header.IndexSection);
			ValidateChecksum(array, header.DataSection);
			ValidateChecksum(array, header.ReverseIndexSection);
			string[] array2 = ReadStringSection(array, header.StringSection);
			ShopNPCLocation[] array3 = ReadLocationSection(array, header.LocationSection);
			FrozenDictionary<uint, ItemOffset> frozenDictionary = ReadIndexSection(array, header.IndexSection, header.DataSection.Offset);
			FrozenDictionary<uint, uint[]> frozenDictionary2 = ReadReverseIndexSection(array, header.ReverseIndexSection);
			FileStream fileStream = new FileStream(snapshotPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.RandomAccess | FileOptions.SequentialScan);
			using (snapshotGate.EnterScope())
			{
				snapshotStream?.Dispose();
				snapshotStream = fileStream;
				stringTable = array2;
				locationTable = array3;
				itemOffsets = frozenDictionary;
				reverseItemLookup = frozenDictionary2;
			}
			hotCache.ClearAll(includePermanentItems: true);
			reverseHotCache.ClearAll(includePermanentItems: true);
			return true;
		}
		catch (Exception ex)
		{
			failureReason = ex.Message;
			DLog.Warning("[ItemSourceRepository] 现有快照不可用, 将后台重建: " + failureReason);
			TryDeleteSnapshot(snapshotPath);
			return false;
		}
	}

	private static ItemSourceQueryResult DecodeItem(SafeFileHandle handle, ItemOffset itemOffset, IReadOnlyList<string> strings, List<ShopNPCLocation> locations)
	{
		try
		{
			byte[] array = GC.AllocateUninitializedArray<byte>(itemOffset.Length);
			int num = RandomAccess.Read(handle, array, itemOffset.Offset);
			if (num != itemOffset.Length)
			{
				throw new EndOfStreamException($"预期读取 {itemOffset.Length} 字节, 实际读取 {num} 字节");
			}
			using MemoryStream input = new MemoryStream(array, writable: false);
			using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8, leaveOpen: false);
			ItemShopType shopType = (ItemShopType)binaryReader.ReadInt32();
			string achievementDescription = ReadOptionalString(strings, binaryReader.ReadInt32()) ?? string.Empty;
			int num2 = binaryReader.ReadInt32();
			List<ShopNPCInfos> list = new List<ShopNPCInfos>(num2);
			for (int i = 0; i < num2; i++)
			{
				uint iD = binaryReader.ReadUInt32();
				string name = ReadRequiredString(strings, binaryReader.ReadInt32());
				string shopName = ReadOptionalString(strings, binaryReader.ReadInt32());
				int num3 = binaryReader.ReadInt32();
				int num4 = binaryReader.ReadInt32();
				List<ShopItemCostInfo> list2 = new List<ShopItemCostInfo>(num4);
				for (int j = 0; j < num4; j++)
				{
					uint cost = binaryReader.ReadUInt32();
					uint itemID = binaryReader.ReadUInt32();
					bool flag = binaryReader.ReadBoolean();
					uint value = binaryReader.ReadUInt32();
					list2.Add(new ShopItemCostInfo(cost, itemID, flag ? new uint?(value) : ((uint?)null)));
				}
				list.Add(new ShopNPCInfos
				{
					ID = iD,
					Name = name,
					ShopName = shopName,
					CostInfos = list2,
					Location = ((num3 >= 0) ? locations[num3] : null)
				});
			}
			return ItemSourceQueryResult.Ready(new ItemSourceInfo
			{
				ItemID = itemOffset.ItemID,
				ShopType = shopType,
				AchievementDescription = achievementDescription,
				NPCInfos = list
			});
		}
		catch (Exception ex)
		{
			DLog.Error($"[ItemSourceRepository] 解码物品来源条目失败, ItemID: {itemOffset.ItemID}", ex);
			return ItemSourceQueryResult.Failed;
		}
	}

	private static ExchangeItemsQueryResult BuildExchangeQueryResult(uint costItemID, IReadOnlyList<uint> targetItemIDs, SafeFileHandle handle, FrozenDictionary<uint, ItemOffset> offsets, IReadOnlyList<string> strings, IReadOnlyList<ShopNPCLocation> locations)
	{
		List<ExchangeItemInfo> list = new List<ExchangeItemInfo>(targetItemIDs.Count);
		foreach (uint targetItemID in targetItemIDs)
		{
			if (!offsets.TryGetValue(targetItemID, out var value))
			{
				return ExchangeItemsQueryResult.Failed;
			}
			ItemSourceQueryResult itemSourceQueryResult = DecodeItem(handle, value, strings, locations.ToList());
			if (itemSourceQueryResult.State != ItemSourceQueryState.Ready || itemSourceQueryResult.Data == null)
			{
				return ExchangeItemsQueryResult.Failed;
			}
			ExchangeItemInfo exchangeItemInfo = BuildReverseItemInfo(itemSourceQueryResult.Data, costItemID);
			if (exchangeItemInfo == null)
			{
				return ExchangeItemsQueryResult.Failed;
			}
			list.Add(exchangeItemInfo);
		}
		list = list.Where((ExchangeItemInfo x) => !string.IsNullOrWhiteSpace(x.GetItemName())).ToList();
		if (list.Count == 0)
		{
			return ExchangeItemsQueryResult.NotFound;
		}
		return ExchangeItemsQueryResult.Ready(new ExchangeItemsInfo
		{
			CostItemID = costItemID,
			Items = list
		});
	}

	private static ExchangeItemInfo? BuildReverseItemInfo(ItemSourceInfo itemInfo, uint costItemID)
	{
		List<ExchangeItemNPCInfo> list = new List<ExchangeItemNPCInfo>();
		foreach (ShopNPCInfos nPCInfo in itemInfo.NPCInfos)
		{
			List<ShopItemCostInfo> list2 = nPCInfo.CostInfos.Where((ShopItemCostInfo costInfo) => costInfo.ItemID == costItemID).ToList();
			if (list2.Count != 0)
			{
				list.Add(new ExchangeItemNPCInfo
				{
					ID = nPCInfo.ID,
					Name = nPCInfo.Name,
					ShopName = nPCInfo.ShopName,
					Location = nPCInfo.Location,
					CostInfos = list2
				});
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return new ExchangeItemInfo
		{
			ItemID = itemInfo.ItemID,
			ShopType = itemInfo.ShopType,
			AchievementDescription = itemInfo.AchievementDescription,
			NPCInfos = list
		};
	}

	private static async Task WriteSnapshotAsync(string path, Dictionary<uint, ItemSourceInfo> items)
	{
		Dictionary<string, int> indices = new Dictionary<string, int>(StringComparer.Ordinal);
		List<string> stringTableBuilder = new List<string>();
		Dictionary<LocationKey, int> indices2 = new Dictionary<LocationKey, int>();
		List<ShopNPCLocation> locationTableBuilder = new List<ShopNPCLocation>();
		List<IndexEntry> indexEntries = new List<IndexEntry>(items.Count);
		Dictionary<uint, HashSet<uint>> reverseIndexBuilder = new Dictionary<uint, HashSet<uint>>();
		MemoryStream dataStream = new MemoryStream();
		await using (BinaryWriter binaryWriter = new BinaryWriter(dataStream, Encoding.UTF8, leaveOpen: true))
		{
			foreach (KeyValuePair<uint, ItemSourceInfo> item in items.OrderBy<KeyValuePair<uint, ItemSourceInfo>, uint>((KeyValuePair<uint, ItemSourceInfo> x) => x.Key))
			{
				item.Deconstruct(out var key, out var value);
				uint itemID = key;
				ItemSourceInfo itemSourceInfo = value;
				int num = checked((int)dataStream.Position);
				binaryWriter.Write((int)itemSourceInfo.ShopType);
				binaryWriter.Write(GetOptionalStringID(itemSourceInfo.AchievementDescription, indices, stringTableBuilder));
				binaryWriter.Write(itemSourceInfo.NPCInfos.Count);
				foreach (ShopNPCInfos nPCInfo in itemSourceInfo.NPCInfos)
				{
					binaryWriter.Write(nPCInfo.ID);
					binaryWriter.Write(GetRequiredStringID(nPCInfo.Name, indices, stringTableBuilder));
					binaryWriter.Write(GetOptionalStringID(nPCInfo.ShopName, indices, stringTableBuilder));
					binaryWriter.Write(GetLocationID(nPCInfo.Location, indices2, locationTableBuilder));
					binaryWriter.Write(nPCInfo.CostInfos.Count);
					foreach (ShopItemCostInfo costInfo in nPCInfo.CostInfos)
					{
						binaryWriter.Write(costInfo.Cost);
						binaryWriter.Write(costInfo.ItemID);
						binaryWriter.Write(costInfo.Collectablity.HasValue);
						binaryWriter.Write(costInfo.Collectablity.GetValueOrDefault());
						AddReverseIndexEntry(reverseIndexBuilder, costInfo.ItemID, itemID);
					}
				}
				int length = checked((int)dataStream.Position - num);
				indexEntries.Add(new IndexEntry(itemID, num, length));
			}
		}
		byte[] bytes = BuildStringSection(stringTableBuilder);
		byte[] bytes2 = BuildLocationSection(locationTableBuilder);
		byte[] bytes3 = BuildIndexSection(indexEntries);
		byte[] bytes4 = BuildReverseIndexSection(reverseIndexBuilder);
		byte[] bytes5 = dataStream.ToArray();
		using MemoryStream outputStream = new MemoryStream();
		await using BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8, leaveOpen: true);
		int headerSizeHint = GetHeaderSizeHint();
		writer.Write(new byte[headerSizeHint]);
		SectionInfo stringSection = WriteSection(outputStream, bytes);
		SectionInfo locationSection = WriteSection(outputStream, bytes2);
		SectionInfo indexSection = WriteSection(outputStream, bytes3);
		SectionInfo reverseIndexSection = WriteSection(outputStream, bytes4);
		SectionInfo dataSection = WriteSection(outputStream, bytes5);
		outputStream.Position = 0L;
		WriteHeader(writer, new SnapshotHeader(1230197315u, 5, GameState.ClientVersion, GameState.ClientLanguage, stringSection, locationSection, indexSection, reverseIndexSection, dataSection));
		await WriteAllBytesAtomicallyAsync(path, outputStream.ToArray()).ConfigureAwait(continueOnCapturedContext: false);
	}

	private static byte[] BuildStringSection(List<string> strings)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
		binaryWriter.Write(strings.Count);
		foreach (string @string in strings)
		{
			binaryWriter.Write(@string);
		}
		return memoryStream.ToArray();
	}

	private static byte[] BuildLocationSection(List<ShopNPCLocation> locations)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
		binaryWriter.Write(locations.Count);
		foreach (ShopNPCLocation location in locations)
		{
			binaryWriter.Write(location.X);
			binaryWriter.Write(location.Y);
			binaryWriter.Write(location.TerritoryID);
			binaryWriter.Write(location.MapID);
		}
		return memoryStream.ToArray();
	}

	private static byte[] BuildIndexSection(IReadOnlyList<IndexEntry> entries)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
		binaryWriter.Write(entries.Count);
		foreach (IndexEntry entry in entries)
		{
			binaryWriter.Write(entry.ItemID);
			binaryWriter.Write(entry.Offset);
			binaryWriter.Write(entry.Length);
		}
		return memoryStream.ToArray();
	}

	private static byte[] BuildReverseIndexSection(Dictionary<uint, HashSet<uint>> reverseIndex)
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true);
		binaryWriter.Write(reverseIndex.Count);
		foreach (var (value, hashSet2) in reverseIndex.OrderBy<KeyValuePair<uint, HashSet<uint>>, uint>((KeyValuePair<uint, HashSet<uint>> x) => x.Key))
		{
			binaryWriter.Write(value);
			binaryWriter.Write(hashSet2.Count);
			foreach (uint item in hashSet2.OrderBy((uint x) => x))
			{
				binaryWriter.Write(item);
			}
		}
		return memoryStream.ToArray();
	}

	private static SectionInfo WriteSection(Stream stream, byte[] bytes)
	{
		long position = stream.Position;
		stream.Write(bytes);
		return new SectionInfo(position, bytes.Length, ComputeChecksum(bytes));
	}

	private static SnapshotHeader ReadHeader(BinaryReader reader)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		uint magic = reader.ReadUInt32();
		int formatVersion = reader.ReadInt32();
		string clientVersion = reader.ReadString();
		Language clientLanguage = (Language)(ushort)reader.ReadInt32();
		SectionInfo stringSection = ReadSectionInfo(reader);
		SectionInfo locationSection = ReadSectionInfo(reader);
		SectionInfo indexSection = ReadSectionInfo(reader);
		SectionInfo reverseIndexSection = ReadSectionInfo(reader);
		SectionInfo dataSection = ReadSectionInfo(reader);
		return new SnapshotHeader(magic, formatVersion, clientVersion, clientLanguage, stringSection, locationSection, indexSection, reverseIndexSection, dataSection);
	}

	private static void WriteHeader(BinaryWriter writer, SnapshotHeader header)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected I4, but got Unknown
		writer.Write(header.Magic);
		writer.Write(header.FormatVersion);
		writer.Write(header.ClientVersion);
		writer.Write((int)header.ClientLanguage);
		WriteSectionInfo(writer, header.StringSection);
		WriteSectionInfo(writer, header.LocationSection);
		WriteSectionInfo(writer, header.IndexSection);
		WriteSectionInfo(writer, header.ReverseIndexSection);
		WriteSectionInfo(writer, header.DataSection);
	}

	private static void ValidateHeader(SnapshotHeader header, int fileLength)
	{
		if (header.Magic != 1230197315)
		{
			throw new InvalidDataException($"快照魔数不匹配: {header.Magic}");
		}
		if (header.FormatVersion != 5)
		{
			throw new InvalidDataException($"快照格式版本不匹配: {header.FormatVersion}");
		}
		ValidateSection(header.StringSection, fileLength);
		ValidateSection(header.LocationSection, fileLength);
		ValidateSection(header.IndexSection, fileLength);
		ValidateSection(header.ReverseIndexSection, fileLength);
		ValidateSection(header.DataSection, fileLength);
	}

	private static void ValidateSection(SectionInfo section, int fileLength)
	{
		if (section.Offset < 0 || section.Length < 0)
		{
			throw new InvalidDataException("快照区段目录存在非法负值");
		}
		if (section.Offset + section.Length > fileLength)
		{
			throw new InvalidDataException("快照区段越界");
		}
	}

	private static void ValidateChecksum(byte[] bytes, SectionInfo section)
	{
		if (ComputeChecksum(bytes.AsSpan((int)section.Offset, section.Length)) != section.Checksum)
		{
			throw new InvalidDataException($"快照区段校验失败, Offset: {section.Offset}, Length: {section.Length}");
		}
	}

	private static string[] ReadStringSection(byte[] bytes, SectionInfo section)
	{
		using MemoryStream input = new MemoryStream(bytes, (int)section.Offset, section.Length, writable: false);
		using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8, leaveOpen: false);
		int num = binaryReader.ReadInt32();
		string[] array = new string[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = binaryReader.ReadString();
		}
		return array;
	}

	private static ShopNPCLocation[] ReadLocationSection(byte[] bytes, SectionInfo section)
	{
		using MemoryStream input = new MemoryStream(bytes, (int)section.Offset, section.Length, writable: false);
		using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8, leaveOpen: false);
		int num = binaryReader.ReadInt32();
		ShopNPCLocation[] array = new ShopNPCLocation[num];
		for (int i = 0; i < num; i++)
		{
			float x = binaryReader.ReadSingle();
			float y = binaryReader.ReadSingle();
			uint territoryID = binaryReader.ReadUInt32();
			uint value = binaryReader.ReadUInt32();
			array[i] = new ShopNPCLocation(x, y, territoryID, value);
		}
		return array;
	}

	private static FrozenDictionary<uint, ItemOffset> ReadIndexSection(byte[] bytes, SectionInfo section, long dataSectionOffset)
	{
		using MemoryStream input = new MemoryStream(bytes, (int)section.Offset, section.Length, writable: false);
		using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8, leaveOpen: false);
		int num = binaryReader.ReadInt32();
		Dictionary<uint, ItemOffset> dictionary = new Dictionary<uint, ItemOffset>(num);
		for (int i = 0; i < num; i++)
		{
			uint num2 = binaryReader.ReadUInt32();
			int num3 = binaryReader.ReadInt32();
			int length = binaryReader.ReadInt32();
			dictionary[num2] = new ItemOffset(num2, dataSectionOffset + num3, length);
		}
		return dictionary.ToFrozenDictionary();
	}

	private static FrozenDictionary<uint, uint[]> ReadReverseIndexSection(byte[] bytes, SectionInfo section)
	{
		using MemoryStream input = new MemoryStream(bytes, (int)section.Offset, section.Length, writable: false);
		using BinaryReader binaryReader = new BinaryReader(input, Encoding.UTF8, leaveOpen: false);
		int num = binaryReader.ReadInt32();
		Dictionary<uint, uint[]> dictionary = new Dictionary<uint, uint[]>(num);
		for (int i = 0; i < num; i++)
		{
			uint key = binaryReader.ReadUInt32();
			int num2 = binaryReader.ReadInt32();
			uint[] array = new uint[num2];
			for (int j = 0; j < num2; j++)
			{
				array[j] = binaryReader.ReadUInt32();
			}
			dictionary[key] = array;
		}
		return dictionary.ToFrozenDictionary();
	}

	private static int GetHeaderSizeHint()
	{
		return 512;
	}

	private static SectionInfo ReadSectionInfo(BinaryReader reader)
	{
		return new SectionInfo(reader.ReadInt64(), reader.ReadInt32(), reader.ReadUInt32());
	}

	private static void WriteSectionInfo(BinaryWriter writer, SectionInfo info)
	{
		writer.Write(info.Offset);
		writer.Write(info.Length);
		writer.Write(info.Checksum);
	}

	private static uint ComputeChecksum(ReadOnlySpan<byte> data)
	{
		uint num = 2166136261u;
		ReadOnlySpan<byte> readOnlySpan = data;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			byte b = readOnlySpan[i];
			num ^= b;
			num *= 16777619;
		}
		return num;
	}

	private static uint ComputeChecksum(byte[] bytes)
	{
		return ComputeChecksum(bytes.AsSpan());
	}

	private static string ReadRequiredString(IReadOnlyList<string> strings, int index)
	{
		if ((uint)index >= strings.Count)
		{
			throw new InvalidDataException($"字符串索引越界: {index}");
		}
		return strings[index];
	}

	private static string? ReadOptionalString(IReadOnlyList<string> strings, int index)
	{
		if (index >= 0)
		{
			return ReadRequiredString(strings, index);
		}
		return null;
	}

	private static int GetRequiredStringID(string value, IDictionary<string, int> indices, ICollection<string> table)
	{
		if (indices.TryGetValue(value, out var value2))
		{
			return value2;
		}
		value2 = (indices[value] = table.Count);
		table.Add(value);
		return value2;
	}

	private static int GetOptionalStringID(string? value, IDictionary<string, int> indices, ICollection<string> table)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return GetRequiredStringID(value, indices, table);
		}
		return -1;
	}

	private static int GetLocationID(ShopNPCLocation? location, IDictionary<LocationKey, int> indices, List<ShopNPCLocation> table)
	{
		if (location == null)
		{
			return -1;
		}
		LocationKey key = new LocationKey(location.X, location.Y, location.TerritoryID, location.MapID);
		if (indices.TryGetValue(key, out var value))
		{
			return value;
		}
		value = (indices[key] = table.Count);
		table.Add(location);
		return value;
	}

	private static void AddReverseIndexEntry(Dictionary<uint, HashSet<uint>> reverseIndex, uint costItemID, uint itemID)
	{
		bool exists;
		ref HashSet<uint> valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(reverseIndex, costItemID, out exists);
		if (!exists || valueRefOrAddDefault == null)
		{
			valueRefOrAddDefault = new HashSet<uint>();
		}
		valueRefOrAddDefault.Add(itemID);
	}

	private static async Task WriteAllBytesAtomicallyAsync(string path, byte[] bytes)
	{
		string directoryName = Path.GetDirectoryName(path);
		if (!string.IsNullOrEmpty(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
		string tempPath = $"{path}.{Guid.NewGuid():N}.tmp";
		try
		{
			await File.WriteAllBytesAsync(tempPath, bytes).ConfigureAwait(continueOnCapturedContext: false);
			if (File.Exists(path))
			{
				File.Replace(tempPath, path, null);
			}
			else
			{
				File.Move(tempPath, path);
			}
		}
		finally
		{
			if (File.Exists(tempPath))
			{
				File.Delete(tempPath);
			}
		}
	}

	private static void TryDeleteSnapshot(string path)
	{
		try
		{
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		catch (Exception ex)
		{
			DLog.Warning("[ItemSourceRepository] 删除旧快照失败: " + path + ", " + ex.Message);
		}
	}

	private static string GetSnapshotPath()
	{
		return Path.Join(DalamudServiceExtension.Instance().GetPluginConfigDirectory(), "OmenTools", "Cache", "ItemSourceSnapshot.bin");
	}
}
