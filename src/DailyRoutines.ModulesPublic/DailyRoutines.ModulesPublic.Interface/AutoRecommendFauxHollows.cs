using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using DailyRoutines.Common.Interface.Windows;
using DailyRoutines.Common.Module.Abstractions;
using DailyRoutines.Common.Module.Enums;
using DailyRoutines.Common.Module.Models;
using DailyRoutines.Extensions;
using DailyRoutines.Manager;
using Dalamud.Bindings.ImGui;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Extensions;
using OmenTools.Info.Game.Data;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace DailyRoutines.ModulesPublic.Interface;

public class AutoRecommendFauxHollows : ModuleBase
{
	private enum WeeklyPuzzleTexture
	{
		Hidden = 5,
		Blank = 6,
		Blocked = 9
	}

	private enum WeeklyPuzzlePrizeTexture
	{
		TinyBox,
		TinySwords,
		TinyChest,
		TinyCommander,
		BoxTL,
		BoxTR,
		BoxBL,
		BoxBR,
		ChestTL,
		ChestTR,
		ChestBL,
		ChestBR,
		SwordsTL,
		SwordsTR,
		SwordsML,
		SwordsMR,
		SwordsBL,
		SwordsBR,
		Commander
	}

	private class Config : ModuleConfig
	{
		public bool PrioritizeSwords;
	}

	private struct BitMask(ulong raw = 0uL)
	{
		public ulong Raw = raw;

		public bool this[int index]
		{
			get
			{
				return (Raw & MaskForBit(index)) != 0;
			}
			set
			{
				if (value)
				{
					Set(index);
				}
				else
				{
					Clear(index);
				}
			}
		}

		public static BitMask Build(params int[] bits)
		{
			BitMask result = default(BitMask);
			foreach (int index in bits)
			{
				result.Set(index);
			}
			return result;
		}

		public void Reset()
		{
			Raw = 0uL;
		}

		public bool Any()
		{
			return Raw != 0;
		}

		public bool None()
		{
			return Raw == 0;
		}

		public void Set(int index)
		{
			Raw |= MaskForBit(index);
		}

		public void Clear(int index)
		{
			Raw &= ~MaskForBit(index);
		}

		public void Toggle(int index)
		{
			Raw ^= MaskForBit(index);
		}

		public int NumSetBits()
		{
			return BitOperations.PopCount(Raw);
		}

		public int LowestSetBit()
		{
			return BitOperations.TrailingZeroCount(Raw);
		}

		public int HighestSetBit()
		{
			return 63 - BitOperations.LeadingZeroCount(Raw);
		}

		public static BitMask operator ~(BitMask a)
		{
			return new BitMask(~a.Raw);
		}

		public static BitMask operator &(BitMask a, BitMask b)
		{
			return new BitMask(a.Raw & b.Raw);
		}

		public static BitMask operator |(BitMask a, BitMask b)
		{
			return new BitMask(a.Raw | b.Raw);
		}

		public static BitMask operator ^(BitMask a, BitMask b)
		{
			return new BitMask(a.Raw ^ b.Raw);
		}

		public IEnumerable<int> SetBits()
		{
			ulong v = Raw;
			while (v != 0L)
			{
				int index = BitOperations.TrailingZeroCount(v);
				yield return index;
				v &= (ulong)(~(1L << index));
			}
		}

		private readonly ulong MaskForBit(int index)
		{
			if ((uint)index >= 64u)
			{
				return 0uL;
			}
			return (ulong)(1L << index);
		}
	}

	private class BoardState
	{
		[Flags]
		public enum Tile
		{
			Unknown = 0,
			Hidden = 1,
			Blocked = 2,
			Empty = 4,
			BoxTL = 8,
			BoxTR = 0x10,
			BoxBL = 0x20,
			BoxBR = 0x40,
			ChestTL = 0x80,
			ChestTR = 0x100,
			ChestBL = 0x200,
			ChestBR = 0x400,
			SwordsTL = 0x800,
			SwordsTR = 0x1000,
			SwordsML = 0x2000,
			SwordsMR = 0x4000,
			SwordsBL = 0x8000,
			SwordsBR = 0x10000,
			Commander = 0x20000,
			RotatedL = 0x40000,
			RotatedR = 0x80000,
			RotatedEither = 0xC0000,
			Box = 0xC0078,
			Chest = 0xC0780,
			BoxChest = 0xC07F8,
			Swords = 0xDF800
		}

		public const int WIDTH = 6;

		public const int HEIGHT = 6;

		public BitMask Blockers;

		public int BoxChestTL = -1;

		public bool SwordsHorizontal;

		public int SwordsTL = -1;

		public Tile[] Tiles = new Tile[36];

		public bool Update(Tile[] tiles)
		{
			(BitMask, int, bool, int)? tuple = AnalyzeBoard(tiles);
			if (tuple.HasValue)
			{
				Tiles = tiles;
				(Blockers, SwordsTL, SwordsHorizontal, BoxChestTL) = tuple.Value;
				return true;
			}
			Array.Fill(Tiles, Tile.Unknown);
			Blockers.Reset();
			SwordsTL = -1;
			SwordsHorizontal = false;
			BoxChestTL = -1;
			return false;
		}

		private static (BitMask blockers, int swordsTL, bool swordsHoriz, int boxChestTL)? AnalyzeBoard(Tile[] tiles)
		{
			BitMask item = default(BitMask);
			int num = -1;
			bool flag = false;
			int num2 = -1;
			for (int i = 0; i < tiles.Length; i++)
			{
				Tile tile = tiles[i];
				if (tile.HasFlag(Tile.Blocked))
				{
					item.Set(i);
				}
				else if ((tile & Tile.Swords) != Tile.Unknown)
				{
					int num3 = i - TLOffsetSwords(tile);
					bool flag2 = (tile & Tile.RotatedEither) != 0;
					if (num3 > i || (num != -1 && (num != num3 || flag != flag2)))
					{
						return null;
					}
					num = num3;
					flag = flag2;
				}
				else if ((tile & Tile.BoxChest) != Tile.Unknown)
				{
					int num4 = i - TLOffsetBoxChest(tile);
					if (num4 > i || (num2 != -1 && num2 != num4))
					{
						return null;
					}
					num2 = num4;
				}
			}
			return (item, num, flag, num2);
		}

		private static int TLOffsetSwords(Tile t)
		{
			return t switch
			{
				Tile.SwordsTL => 0, 
				Tile.SwordsTR => 1, 
				Tile.SwordsML => 6, 
				Tile.SwordsMR => 7, 
				Tile.SwordsBL => 12, 
				Tile.SwordsBR => 13, 
				Tile.SwordsTL | Tile.RotatedL => 6, 
				Tile.SwordsTR | Tile.RotatedL => 0, 
				Tile.SwordsML | Tile.RotatedL => 7, 
				Tile.SwordsMR | Tile.RotatedL => 1, 
				Tile.SwordsBL | Tile.RotatedL => 8, 
				Tile.SwordsBR | Tile.RotatedL => 2, 
				Tile.SwordsTL | Tile.RotatedR => 2, 
				Tile.SwordsTR | Tile.RotatedR => 8, 
				Tile.SwordsML | Tile.RotatedR => 1, 
				Tile.SwordsMR | Tile.RotatedR => 7, 
				Tile.SwordsBL | Tile.RotatedR => 0, 
				Tile.SwordsBR | Tile.RotatedR => 6, 
				_ => -1, 
			};
		}

		private static int TLOffsetBoxChest(Tile t)
		{
			switch (t)
			{
			case Tile.BoxTL:
			case Tile.ChestTL:
				return 0;
			case Tile.BoxTR:
			case Tile.ChestTR:
				return 1;
			case Tile.BoxBL:
			case Tile.ChestBL:
				return 6;
			case Tile.BoxBR:
			case Tile.ChestBR:
				return 7;
			case Tile.BoxTL | Tile.RotatedL:
			case Tile.ChestTL | Tile.RotatedL:
				return 6;
			case Tile.BoxTR | Tile.RotatedL:
			case Tile.ChestTR | Tile.RotatedL:
				return 0;
			case Tile.BoxBL | Tile.RotatedL:
			case Tile.ChestBL | Tile.RotatedL:
				return 7;
			case Tile.BoxBR | Tile.RotatedL:
			case Tile.ChestBR | Tile.RotatedL:
				return 1;
			case Tile.BoxTL | Tile.RotatedR:
			case Tile.ChestTL | Tile.RotatedR:
				return 1;
			case Tile.BoxTR | Tile.RotatedR:
			case Tile.ChestTR | Tile.RotatedR:
				return 7;
			case Tile.BoxBL | Tile.RotatedR:
			case Tile.ChestBL | Tile.RotatedR:
				return 0;
			case Tile.BoxBR | Tile.RotatedR:
			case Tile.ChestBR | Tile.RotatedR:
				return 6;
			default:
				return -1;
			}
		}
	}

	private class Solver
	{
		public const int CONFIRMED_SWORD = -2;

		public const int CONFIRMED_BOX_CHEST = -3;

		public const int POTENTIAL_FOX = -4;

		public readonly Patterns PatternDB = new Patterns();

		public bool FindSwordsFirst;

		public int[] Solve(BoardState board)
		{
			int[] array = new int[36];
			Patterns.Sheet sheet = MatchingSheet(board);
			if (sheet != null)
			{
				HashSet<(int, bool)> hashSet = new HashSet<(int, bool)>();
				HashSet<int> hashSet2 = new HashSet<int>();
				HashSet<ulong> hashSet3 = new HashSet<ulong>();
				foreach (var (row, cell) in MatchingCells(board, sheet))
				{
					hashSet.Add((row.SwordsTL, row.SwordsHorizontal));
					hashSet2.Add(cell.ChestTL);
					hashSet3.Add(cell.Foxes.Raw);
				}
				int num = ((hashSet.Count == 1) ? (-2) : 10);
				int num2 = ((hashSet2.Count == 1) ? (-3) : ((!FindSwordsFirst || hashSet.Count <= 1) ? 10 : 0));
				foreach (var (tl, horiz) in hashSet)
				{
					foreach (int item in SwordIndices(tl, horiz))
					{
						array[item] += num;
					}
				}
				foreach (int item2 in hashSet2)
				{
					foreach (int item3 in BoxChestIndices(item2))
					{
						array[item3] += num2;
					}
				}
				foreach (ulong item4 in hashSet3)
				{
					foreach (int item5 in new BitMask(item4).SetBits())
					{
						if (hashSet3.Count == 1)
						{
							array[item5] = -4;
						}
						else
						{
							array[item5]++;
						}
					}
				}
			}
			return array;
		}

		public bool MatchesSheet(BoardState board, Patterns.Sheet sheet)
		{
			return sheet.Blockers.Raw == board.Blockers.Raw;
		}

		public bool MatchesRow(BoardState board, Patterns.Row row)
		{
			if (board.SwordsTL == -1)
			{
				return AllHidden(SwordIndices(row.SwordsTL, row.SwordsHorizontal), board);
			}
			if (board.SwordsTL == row.SwordsTL)
			{
				return board.SwordsHorizontal == row.SwordsHorizontal;
			}
			return false;
		}

		public bool MatchesCell(BoardState board, Patterns.Cell cell)
		{
			if (board.BoxChestTL == -1)
			{
				return AllHidden(BoxChestIndices(cell.ChestTL), board);
			}
			return board.BoxChestTL == cell.ChestTL;
		}

		public Patterns.Sheet? MatchingSheet(BoardState board)
		{
			return PatternDB.KnownPatterns.Find((Patterns.Sheet s) => MatchesSheet(board, s));
		}

		public IEnumerable<Patterns.Row> MatchingRows(BoardState board, Patterns.Sheet sheet)
		{
			return sheet.Rows.Where((Patterns.Row r) => MatchesRow(board, r));
		}

		public IEnumerable<Patterns.Cell> MatchingCells(BoardState board, Patterns.Row row)
		{
			return row.Cells.Where((Patterns.Cell c) => MatchesCell(board, c));
		}

		public IEnumerable<(Patterns.Row row, Patterns.Cell cell)> MatchingCells(BoardState board, Patterns.Sheet sheet)
		{
			foreach (Patterns.Row r in MatchingRows(board, sheet))
			{
				foreach (Patterns.Cell item in MatchingCells(board, r))
				{
					yield return (row: r, cell: item);
				}
			}
		}

		public static IEnumerable<int> RectIndices(int tl, int w, int h)
		{
			int sx = tl % 6;
			int sy = tl / 6;
			int y = 0;
			while (y < h)
			{
				int num;
				for (int x = 0; x < w; x = num)
				{
					yield return (sy + y) * 6 + sx + x;
					num = x + 1;
				}
				num = y + 1;
				y = num;
			}
		}

		public static IEnumerable<int> SwordIndices(int tl, bool horiz)
		{
			if (!horiz)
			{
				return RectIndices(tl, 2, 3);
			}
			return RectIndices(tl, 3, 2);
		}

		public static IEnumerable<int> BoxChestIndices(int tl)
		{
			return RectIndices(tl, 2, 2);
		}

		private bool AllHidden(IEnumerable<int> indices, BoardState board)
		{
			return indices.All((int i) => board.Tiles[i] == BoardState.Tile.Hidden);
		}
	}

	private class Patterns
	{
		public class Cell(int chestTl, BitMask foxes)
		{
			public readonly int ChestTL = chestTl;

			public BitMask Foxes = foxes;
		}

		public class Row(int swordsTl, bool swordsHorizontal, Cell[] cells)
		{
			public readonly Cell[] Cells = cells;

			public readonly bool SwordsHorizontal = swordsHorizontal;

			public readonly int SwordsTL = swordsTl;
		}

		public class Sheet(BitMask blockers, Row[] rows)
		{
			public readonly Row[] Rows = rows;

			public BitMask Blockers = blockers;
		}

		private static readonly Sheet[] KnownBaseSheets = new Sheet[4]
		{
			new Sheet(BitMask.Build(8, 10, 13, 26, 35), new Row[7]
			{
				new Row(16, swordsHorizontal: false, new Cell[2]
				{
					new Cell(0, BitMask.Build(3, 4, 12, 30)),
					new Cell(14, BitMask.Build(3, 4, 12, 30))
				}),
				new Row(15, swordsHorizontal: false, new Cell[2]
				{
					new Cell(0, BitMask.Build(9, 17, 33, 34)),
					new Cell(18, BitMask.Build(9, 17, 33, 34))
				}),
				new Row(21, swordsHorizontal: false, new Cell[2]
				{
					new Cell(0, BitMask.Build(5, 15, 16, 20)),
					new Cell(18, BitMask.Build(5, 15, 16, 20))
				}),
				new Row(15, swordsHorizontal: true, new Cell[2]
				{
					new Cell(18, BitMask.Build(3, 4, 12, 30)),
					new Cell(27, BitMask.Build(3, 4, 12, 30))
				}),
				new Row(21, swordsHorizontal: true, new Cell[2]
				{
					new Cell(0, BitMask.Build(5, 15, 16, 20)),
					new Cell(24, BitMask.Build(5, 15, 16, 20))
				}),
				new Row(14, swordsHorizontal: true, new Cell[2]
				{
					new Cell(0, BitMask.Build(2, 11, 29, 32)),
					new Cell(24, BitMask.Build(2, 11, 29, 32))
				}),
				new Row(18, swordsHorizontal: false, new Cell[4]
				{
					new Cell(27, BitMask.Build(2, 11, 29, 32)),
					new Cell(16, BitMask.Build(2, 11, 29, 32)),
					new Cell(22, BitMask.Build(9, 17, 33, 34)),
					new Cell(14, BitMask.Build(9, 17, 33, 34))
				})
			}),
			new Sheet(BitMask.Build(3, 13, 16, 21, 32), new Row[5]
			{
				new Row(0, swordsHorizontal: true, new Cell[4]
				{
					new Cell(4, BitMask.Build(15, 18, 26, 33)),
					new Cell(28, BitMask.Build(15, 18, 26, 33)),
					new Cell(24, BitMask.Build(10, 14, 20, 22)),
					new Cell(27, BitMask.Build(10, 14, 20, 22))
				}),
				new Row(22, swordsHorizontal: false, new Cell[4]
				{
					new Cell(24, BitMask.Build(15, 18, 26, 33)),
					new Cell(1, BitMask.Build(15, 18, 26, 33)),
					new Cell(19, BitMask.Build(1, 6, 8, 17)),
					new Cell(4, BitMask.Build(1, 6, 8, 17))
				}),
				new Row(27, swordsHorizontal: true, new Cell[2]
				{
					new Cell(4, BitMask.Build(1, 6, 8, 17)),
					new Cell(24, BitMask.Build(1, 6, 8, 17))
				}),
				new Row(18, swordsHorizontal: false, new Cell[4]
				{
					new Cell(0, BitMask.Build(10, 14, 20, 22)),
					new Cell(28, BitMask.Build(10, 14, 20, 22)),
					new Cell(8, BitMask.Build(2, 5, 12, 35)),
					new Cell(22, BitMask.Build(2, 5, 12, 35))
				}),
				new Row(18, swordsHorizontal: true, new Cell[2]
				{
					new Cell(0, BitMask.Build(2, 5, 12, 35)),
					new Cell(27, BitMask.Build(2, 5, 12, 35))
				})
			}),
			new Sheet(BitMask.Build(4, 7, 15, 25, 33), new Row[6]
			{
				new Row(10, swordsHorizontal: false, new Cell[2]
				{
					new Cell(12, BitMask.Build(0, 21, 27, 31)),
					new Cell(28, BitMask.Build(0, 21, 27, 31))
				}),
				new Row(16, swordsHorizontal: false, new Cell[2]
				{
					new Cell(12, BitMask.Build(8, 24, 34, 35)),
					new Cell(20, BitMask.Build(8, 24, 34, 35))
				}),
				new Row(22, swordsHorizontal: false, new Cell[4]
				{
					new Cell(2, BitMask.Build(6, 10, 17, 26)),
					new Cell(13, BitMask.Build(6, 10, 17, 26)),
					new Cell(20, BitMask.Build(1, 5, 14, 30)),
					new Cell(12, BitMask.Build(1, 5, 14, 30))
				}),
				new Row(21, swordsHorizontal: true, new Cell[4]
				{
					new Cell(2, BitMask.Build(6, 10, 17, 26)),
					new Cell(12, BitMask.Build(6, 10, 17, 26)),
					new Cell(13, BitMask.Build(8, 24, 34, 35)),
					new Cell(10, BitMask.Build(8, 24, 34, 35))
				}),
				new Row(20, swordsHorizontal: true, new Cell[2]
				{
					new Cell(2, BitMask.Build(1, 5, 14, 30)),
					new Cell(10, BitMask.Build(1, 5, 14, 30))
				}),
				new Row(12, swordsHorizontal: true, new Cell[1]
				{
					new Cell(2, BitMask.Build(0, 21, 27, 31))
				})
			}),
			new Sheet(BitMask.Build(7, 16, 18, 27, 32), new Row[7]
			{
				new Row(2, swordsHorizontal: true, new Cell[2]
				{
					new Cell(14, BitMask.Build(11, 24, 31, 34)),
					new Cell(19, BitMask.Build(11, 24, 31, 34))
				}),
				new Row(3, swordsHorizontal: true, new Cell[2]
				{
					new Cell(22, BitMask.Build(0, 15, 21, 35)),
					new Cell(24, BitMask.Build(0, 15, 21, 35))
				}),
				new Row(2, swordsHorizontal: false, new Cell[2]
				{
					new Cell(4, BitMask.Build(1, 13, 17, 26)),
					new Cell(24, BitMask.Build(1, 13, 17, 26))
				}),
				new Row(8, swordsHorizontal: false, new Cell[2]
				{
					new Cell(28, BitMask.Build(2, 3, 23, 33)),
					new Cell(24, BitMask.Build(2, 3, 23, 33))
				}),
				new Row(22, swordsHorizontal: false, new Cell[2]
				{
					new Cell(2, BitMask.Build(1, 13, 17, 26)),
					new Cell(4, BitMask.Build(1, 13, 17, 26))
				}),
				new Row(13, swordsHorizontal: false, new Cell[2]
				{
					new Cell(2, BitMask.Build(0, 15, 21, 35)),
					new Cell(22, BitMask.Build(0, 15, 21, 35))
				}),
				new Row(13, swordsHorizontal: true, new Cell[4]
				{
					new Cell(2, BitMask.Build(11, 24, 31, 34)),
					new Cell(22, BitMask.Build(11, 24, 31, 34)),
					new Cell(4, BitMask.Build(2, 3, 23, 33)),
					new Cell(28, BitMask.Build(2, 3, 23, 33))
				})
			})
		};

		public readonly List<Sheet> KnownPatterns = new List<Sheet>();

		public Patterns()
		{
			Sheet[] knownBaseSheets = KnownBaseSheets;
			foreach (Sheet sheet in knownBaseSheets)
			{
				KnownPatterns.Add(sheet);
				Sheet sheet2 = RotateSheetLeft(sheet);
				KnownPatterns.Add(sheet2);
				sheet2 = RotateSheetLeft(sheet2);
				KnownPatterns.Add(sheet2);
				sheet2 = RotateSheetLeft(sheet2);
				KnownPatterns.Add(sheet2);
			}
		}

		private static int RotateCellIndexLeft(int cell)
		{
			int num = cell % 6;
			int num2 = cell / 6;
			int num3 = 5 - num2;
			return num * 6 + num3;
		}

		private static BitMask RotateCellMaskLeft(BitMask cells)
		{
			return BitMask.Build(cells.SetBits().Select(RotateCellIndexLeft).ToArray());
		}

		private static Cell RotateCellLeft(Cell cell)
		{
			return new Cell(RotateCellIndexLeft(cell.ChestTL) - 1, RotateCellMaskLeft(cell.Foxes));
		}

		private static Row RotateRowLeft(Row row)
		{
			return new Row(RotateCellIndexLeft(row.SwordsTL) - (row.SwordsHorizontal ? 1 : 2), !row.SwordsHorizontal, row.Cells.Select(RotateCellLeft).ToArray());
		}

		private static Sheet RotateSheetLeft(Sheet sheet)
		{
			return new Sheet(RotateCellMaskLeft(sheet.Blockers), sheet.Rows.Select(RotateRowLeft).ToArray());
		}
	}

	private Config config;

	private BoardState board;

	private Solver fauxSolver;

	public override ModuleInfo Info { get; } = new ModuleInfo
	{
		Title = LanguageManager.Get("AutoRecommendFauxHollowsTitle"),
		Description = LanguageManager.Get("AutoRecommendFauxHollowsDescription"),
		Category = ModuleCategory.Interface,
		Author = new string[1] { "Veever" }
	};

	public override ModulePermission Permission { get; } = new ModulePermission
	{
		AllDefaultEnabled = true
	};

	protected unsafe override void Init()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		config = ModuleConfigExtension.Load<Config>(this) ?? new Config();
		board = new BoardState();
		fauxSolver = new Solver();
		if (base.Overlay == null)
		{
			Overlay overlay = (base.Overlay = new Overlay(this));
		}
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)1, "WeeklyPuzzle", new AddonEventDelegate(OnWeeklyPuzzleEvent));
		DalamudServiceExtension.Instance<IAddonLifecycle>().RegisterListener((AddonEvent)6, "WeeklyPuzzle", new AddonEventDelegate(OnWeeklyPuzzleEvent));
		if (Addons.WeeklyPuzzle != null)
		{
			OnWeeklyPuzzleEvent((AddonEvent)1, null);
		}
	}

	protected override void Uninit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		DalamudServiceExtension.Instance<IAddonLifecycle>().UnregisterListener((AddonEventDelegate[])(object)new AddonEventDelegate[1] { OnWeeklyPuzzleEvent });
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
	}

	protected override void ConfigUI()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (ImGui.Checkbox(ImU8String.op_Implicit(LanguageManager.Get("AutoRecommendFauxHollows-PrioritizeSwords")), ref config.PrioritizeSwords))
		{
			fauxSolver.FindSwordsFirst = config.PrioritizeSwords;
			config.Save(this);
		}
	}

	protected unsafe override void OverlayUI()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (!(*Addons.WeeklyPuzzle).IsAddonAndNodesReady())
		{
			((Window)base.Overlay).IsOpen = false;
			return;
		}
		ImGui.SetWindowPos(new Vector2((float)((AtkUnitBase)Addons.WeeklyPuzzle).GetX() - ImGui.GetWindowSize().X, ((AtkUnitBase)Addons.WeeklyPuzzle).GetY() + 5), (ImGuiCond)0);
		Vector4 vector = KnownColor.Orange.ToVector4();
		ImGui.TextColored(ref vector, ImU8String.op_Implicit(LanguageManager.Get("AutoRecommendFauxHollowsTitle")));
		ImGui.Separator();
		ConfigUI();
	}

	private void OnWeeklyPuzzleEvent(AddonEvent type, AddonArgs? args)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		OmenServiceBase<FrameworkManager>.Instance().Unreg(OnUpdate);
		if ((int)type != 1)
		{
			if ((int)type == 6)
			{
				((Window)base.Overlay).IsOpen = false;
			}
		}
		else
		{
			((Window)base.Overlay).IsOpen = true;
			OmenServiceBase<FrameworkManager>.Instance().Reg(new OnUpdateDelegate(OnUpdate), 1000u);
		}
	}

	private unsafe void OnUpdate(IFramework framework)
	{
		AddonWeeklyPuzzle* weeklyPuzzle = (AddonWeeklyPuzzle*)Addons.WeeklyPuzzle;
		if (weeklyPuzzle != null && (*Addons.WeeklyPuzzle).IsAddonAndNodesReady())
		{
			BoardState.Tile[] tiles = ParseTileInformation(weeklyPuzzle);
			board.Update(tiles);
			int[] array = fauxSolver.Solve(board);
			int num = array.Max();
			if (num == 0)
			{
				num = -1;
			}
			UpdateAddonColors(weeklyPuzzle, array, num);
		}
	}

	private unsafe static BoardState.Tile[] ParseTileInformation(AddonWeeklyPuzzle* addon)
	{
		BoardState.Tile[] array = new BoardState.Tile[36];
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				ref BoardState.Tile reference = ref array[num++];
				AtkComponentButton* tileButton = GetTileButton(addon, j, i);
				AtkImageNode* backgroundImageNode = GetBackgroundImageNode(tileButton);
				AtkImageNode* iconImageNode = GetIconImageNode(tileButton);
				BoardState.Tile tile;
				switch ((WeeklyPuzzleTexture)((AtkImageNode)backgroundImageNode).PartId)
				{
				case WeeklyPuzzleTexture.Hidden:
					tile = BoardState.Tile.Hidden;
					break;
				case WeeklyPuzzleTexture.Blocked:
					tile = BoardState.Tile.Blocked;
					break;
				case WeeklyPuzzleTexture.Blank:
				{
					BoardState.Tile tile2 = (((AtkImageNode)iconImageNode).IsVisible() ? ((WeeklyPuzzlePrizeTexture)((AtkImageNode)iconImageNode).PartId switch
					{
						WeeklyPuzzlePrizeTexture.BoxTL => BoardState.Tile.BoxTL, 
						WeeklyPuzzlePrizeTexture.BoxTR => BoardState.Tile.BoxTR, 
						WeeklyPuzzlePrizeTexture.BoxBL => BoardState.Tile.BoxBL, 
						WeeklyPuzzlePrizeTexture.BoxBR => BoardState.Tile.BoxBR, 
						WeeklyPuzzlePrizeTexture.ChestTL => BoardState.Tile.ChestTL, 
						WeeklyPuzzlePrizeTexture.ChestTR => BoardState.Tile.ChestTR, 
						WeeklyPuzzlePrizeTexture.ChestBL => BoardState.Tile.ChestBL, 
						WeeklyPuzzlePrizeTexture.ChestBR => BoardState.Tile.ChestBR, 
						WeeklyPuzzlePrizeTexture.SwordsTL => BoardState.Tile.SwordsTL, 
						WeeklyPuzzlePrizeTexture.SwordsTR => BoardState.Tile.SwordsTR, 
						WeeklyPuzzlePrizeTexture.SwordsML => BoardState.Tile.SwordsML, 
						WeeklyPuzzlePrizeTexture.SwordsMR => BoardState.Tile.SwordsMR, 
						WeeklyPuzzlePrizeTexture.SwordsBL => BoardState.Tile.SwordsBL, 
						WeeklyPuzzlePrizeTexture.SwordsBR => BoardState.Tile.SwordsBR, 
						WeeklyPuzzlePrizeTexture.Commander => BoardState.Tile.Commander, 
						_ => BoardState.Tile.Unknown, 
					}) : BoardState.Tile.Empty);
					tile = tile2;
					break;
				}
				default:
					tile = BoardState.Tile.Unknown;
					break;
				}
				reference = tile;
				if (reference == BoardState.Tile.Unknown)
				{
					return array;
				}
				float rotation = ((AtkResNode)(&((AtkImageNode)iconImageNode).AtkResNode)).Rotation;
				if (rotation < 0f)
				{
					reference |= BoardState.Tile.RotatedL;
				}
				else if (rotation > 0f)
				{
					reference |= BoardState.Tile.RotatedR;
				}
			}
		}
		return array;
	}

	private unsafe static void UpdateAddonColors(AddonWeeklyPuzzle* addon, int[] solution, int bestScore)
	{
		int num = 0;
		for (int i = 0; i < 6; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				int num2 = solution[num++];
				AtkImageNode* backgroundImageNode = GetBackgroundImageNode(GetTileButton(addon, j, i));
				object obj = num2 switch
				{
					-2 => (31, 174, 186), 
					-3 => (255, 105, 180), 
					-4 => (255, 215, 0), 
					_ => (num2 == bestScore) ? (32, 143, 46) : (0, 0, 0), 
				};
				int item = ((ValueTuple<int, int, int>*)(&obj))->Item1;
				int item2 = ((ValueTuple<int, int, int>*)(&obj))->Item2;
				int item3 = ((ValueTuple<int, int, int>*)(&obj))->Item3;
				((AtkResNode)(&((AtkImageNode)backgroundImageNode).AtkResNode)).AddRed = (short)item;
				((AtkResNode)(&((AtkImageNode)backgroundImageNode).AtkResNode)).AddGreen = (short)item2;
				((AtkResNode)(&((AtkImageNode)backgroundImageNode).AtkResNode)).AddBlue = (short)item3;
			}
		}
	}

	private unsafe static AtkComponentButton* GetTileButton(AddonWeeklyPuzzle* addon, int x, int y)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		GameTileRow val = ((GameTileBoard)(&((AddonWeeklyPuzzle)addon).GameBoard))[y];
		return ((GameTileRow)(ref val))[x].Button;
	}

	private unsafe static AtkImageNode* GetBackgroundImageNode(AtkComponentButton* button)
	{
		return (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)(&((AtkComponentButton)button).AtkComponentBase)).UldManager)).NodeList[3];
	}

	private unsafe static AtkImageNode* GetIconImageNode(AtkComponentButton* button)
	{
		return (AtkImageNode*)((AtkUldManager)(&((AtkComponentBase)(&((AtkComponentButton)button).AtkComponentBase)).UldManager)).NodeList[6];
	}
}
