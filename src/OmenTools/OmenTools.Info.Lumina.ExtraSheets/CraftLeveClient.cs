using System;
using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace OmenTools.Info.Lumina.ExtraSheets;

[Sheet("leve/CraftLeveClient")]
public readonly struct CraftLeveClient(ExcelPage page, uint offset, uint row) : IExcelRow<CraftLeveClient>
{
	public ExcelPage ExcelPage => page;

	public uint RowOffset => offset;

	public uint RowId => row;

	public ReadOnlySeString Name => page.ReadString((UIntPtr)offset, (UIntPtr)offset);

	public ReadOnlySeString Text => page.ReadString((UIntPtr)(offset + 4), (UIntPtr)offset);

	static CraftLeveClient IExcelRow<CraftLeveClient>.Create(ExcelPage page, uint offset, uint row)
	{
		return new CraftLeveClient(page, offset, row);
	}
}
