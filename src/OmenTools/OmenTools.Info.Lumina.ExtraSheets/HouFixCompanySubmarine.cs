using System;
using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace OmenTools.Info.Lumina.ExtraSheets;

[Sheet("custom/004/HouFixCompanySubmarine_00447")]
public readonly struct HouFixCompanySubmarine(ExcelPage page, uint offset, uint row) : IExcelRow<HouFixCompanySubmarine>
{
	public ExcelPage ExcelPage => page;

	public uint RowOffset => offset;

	public uint RowId => row;

	public ReadOnlySeString Identifier => page.ReadString((UIntPtr)offset, (UIntPtr)offset);

	public ReadOnlySeString Text => page.ReadString((UIntPtr)(offset + 4), (UIntPtr)offset);

	static HouFixCompanySubmarine IExcelRow<HouFixCompanySubmarine>.Create(ExcelPage page, uint offset, uint row)
	{
		return new HouFixCompanySubmarine(page, offset, row);
	}
}
