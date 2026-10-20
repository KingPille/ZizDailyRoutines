using System;
using Lumina.Excel;
using Lumina.Text.ReadOnly;

namespace OmenTools.Info.Lumina.ExtraSheets;

[Sheet("custom/001/CmnDefHousingPersonalRoomEntrance_00178")]
public readonly struct HousingPersonalRoomEntrance(ExcelPage page, uint offset, uint row) : IExcelRow<HousingPersonalRoomEntrance>
{
	public ExcelPage ExcelPage => page;

	public uint RowOffset => offset;

	public uint RowId => row;

	public ReadOnlySeString Identifier => page.ReadString((UIntPtr)offset, (UIntPtr)offset);

	public ReadOnlySeString Text => page.ReadString((UIntPtr)(offset + 4), (UIntPtr)offset);

	static HousingPersonalRoomEntrance IExcelRow<HousingPersonalRoomEntrance>.Create(ExcelPage page, uint offset, uint row)
	{
		return new HousingPersonalRoomEntrance(page, offset, row);
	}
}
