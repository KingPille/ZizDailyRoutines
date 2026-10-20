using System;
using System.Globalization;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using OmenTools.Extensions;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.ImGuiOm.Widgets;

public class DatePicker
{
	private const float NAVIGATION_BUTTON_SPACING = 2f;

	private CultureInfo cultureInfo;

	private bool isYearView;

	private DateTime viewDate = OmenServiceBase<StandardTimeManager>.Instance().Now;

	private string[] weekDays = Array.Empty<string>();

	public string DateFormat { get; set; } = "yyyy.MM";

	public float Width { get; set; }

	public DatePicker(CultureInfo? culture = null)
	{
		cultureInfo = culture ?? CultureInfo.CurrentCulture;
		UpdateLocalizedStrings();
	}

	public bool Draw(string label, ref DateTime currentDate, ImGuiSelectableFlags flags = (ImGuiSelectableFlags)0)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		float num = ResolvePickerWidth();
		IdDisposable val = ImRaii.PushId(ImU8String.op_Implicit(label), true);
		try
		{
			CenterWithinAvailableWidth(num);
			GroupDisposable val2 = ImRaii.Group();
			try
			{
				DrawHeader(num);
				ImGui.Spacing();
				if (isYearView)
				{
					flag |= DrawYearPicker(num);
				}
				else
				{
					TableDisposable val3 = ImRaii.Table(ImU8String.op_Implicit("CalendarBody"), 7, (ImGuiTableFlags)34816, new Vector2(num, 0f));
					try
					{
						if (TableDisposable.op_Implicit(val3))
						{
							DrawWeekDays();
							flag |= DrawDays(ref currentDate, flags);
						}
					}
					finally
					{
						((TableDisposable)(ref val3)).Dispose();
					}
				}
				return flag;
			}
			finally
			{
				((GroupDisposable)(ref val2)).Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DrawHeader(float pickerWidth)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		GroupDisposable val = ImRaii.Group();
		try
		{
			Vector2 buttonSize = new Vector2(ImGui.GetFrameHeight());
			float navigationGroupWidth = GetNavigationGroupWidth(buttonSize.X);
			float cursorPosX = ImGui.GetCursorPosX();
			if (ImGuiOm.ButtonIcon("##PrevYear", (FontAwesomeIcon)61696, buttonSize))
			{
				viewDate = viewDate.AddYears(-1);
			}
			ImGui.SameLine(0f, 2f);
			if (ImGuiOm.ButtonIcon("##PrevMonth", (FontAwesomeIcon)61700, buttonSize))
			{
				viewDate = viewDate.AddMonths(-1);
			}
			string text = viewDate.ToString(DateFormat, cultureInfo);
			float x = ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X;
			float num = MathF.Max(0f, pickerWidth - navigationGroupWidth * 2f);
			float cursorPosX2 = cursorPosX + navigationGroupWidth + MathF.Max(0f, (num - x) / 2f);
			ImGui.SameLine();
			ImGui.SetCursorPosX(cursorPosX2);
			if (ImGui.Selectable(ImU8String.op_Implicit(text), isYearView, (ImGuiSelectableFlags)1, new Vector2(x, 0f)))
			{
				isYearView = !isYearView;
			}
			ImGui.SameLine();
			ImGui.SetCursorPosX(cursorPosX + pickerWidth - navigationGroupWidth);
			if (ImGuiOm.ButtonIcon("##NextMonth", (FontAwesomeIcon)61701, buttonSize))
			{
				viewDate = viewDate.AddMonths(1);
			}
			ImGui.SameLine(0f, 2f);
			if (ImGuiOm.ButtonIcon("##NextYear", (FontAwesomeIcon)61697, buttonSize))
			{
				viewDate = viewDate.AddYears(1);
			}
		}
		finally
		{
			((GroupDisposable)(ref val)).Dispose();
		}
	}

	private void DrawWeekDays()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		FontDisposable val = ImRaii.PushFont(UiBuilder.DefaultFont, true);
		try
		{
			ImGuiStylePtr style = ImGui.GetStyle();
			ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)0, ((ImGuiStylePtr)(ref style)).Colors[1].ToUInt(), true);
			try
			{
				string[] array = weekDays;
				foreach (string text in array)
				{
					ImGui.TableNextColumn();
					ImGuiOm.TextCentered(text);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private bool DrawDays(ref DateTime currentDate, ImGuiSelectableFlags flags)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		DateTime dateTime = new DateTime(viewDate.Year, viewDate.Month, 1);
		int num = DateTime.DaysInMonth(viewDate.Year, viewDate.Month);
		int firstDayOfWeek = (int)cultureInfo.DateTimeFormat.FirstDayOfWeek;
		int num2 = (int)(dateTime.DayOfWeek - firstDayOfWeek + 7) % 7;
		for (int i = 0; i < num2; i++)
		{
			ImGui.TableNextColumn();
		}
		for (int j = 1; j <= num; j++)
		{
			ImGui.TableNextColumn();
			DateTime dateTime2 = new DateTime(viewDate.Year, viewDate.Month, j);
			bool flag = dateTime2.Date == currentDate.Date;
			bool flag2 = dateTime2.Date == OmenServiceBase<StandardTimeManager>.Instance().Now.Date;
			ImGuiStylePtr style = ImGui.GetStyle();
			ColorDisposable val = ImRaii.PushColor((ImGuiCol)0, ((ImGuiStylePtr)(ref style)).Colors[18].ToUInt(), flag2 && !flag);
			try
			{
				if (ImGuiOm.SelectableTextCentered(j.ToString(), flag, (ImGuiSelectableFlags)(flags | 1)))
				{
					currentDate = dateTime2;
					result = true;
				}
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		return result;
	}

	private bool DrawYearPicker(float pickerWidth)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		int year = viewDate.Year;
		int num = year - 5;
		TableDisposable val = ImRaii.Table(ImU8String.op_Implicit("YearPicker"), 4, (ImGuiTableFlags)34816, new Vector2(pickerWidth, 0f));
		try
		{
			if (!val)
			{
				return false;
			}
			for (int i = 0; i < 12; i++)
			{
				ImGui.TableNextColumn();
				int num2 = num + i;
				bool flag = num2 == year;
				ImGuiStylePtr style = ImGui.GetStyle();
				ColorDisposable val2 = ImRaii.PushColor((ImGuiCol)21, ((ImGuiStylePtr)(ref style)).Colors[24].ToUInt(), flag);
				try
				{
					ImU8String val3 = new ImU8String(0, 1);
					((ImU8String)(ref val3)).AppendFormatted<int>(num2);
					if (ImGui.Button(val3, new Vector2(-1f, 0f)))
					{
						viewDate = new DateTime(num2, viewDate.Month, viewDate.Day);
						isYearView = false;
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			ImGui.TableNextRow();
			ImGui.TableSetColumnIndex(0);
			if (ImGui.Button(ImU8String.op_Implicit("<<"), new Vector2(-1f, 0f)))
			{
				viewDate = viewDate.AddYears(-10);
			}
			ImGui.TableSetColumnIndex(3);
			if (ImGui.Button(ImU8String.op_Implicit(">>"), new Vector2(-1f, 0f)))
			{
				viewDate = viewDate.AddYears(10);
			}
			return false;
		}
		finally
		{
			((TableDisposable)(ref val)).Dispose();
		}
	}

	private void UpdateLocalizedStrings()
	{
		weekDays = cultureInfo.DateTimeFormat.AbbreviatedDayNames;
		int firstDayOfWeek = (int)cultureInfo.DateTimeFormat.FirstDayOfWeek;
		if (firstDayOfWeek != 0)
		{
			string[] array = new string[7];
			for (int i = 0; i < 7; i++)
			{
				array[i] = weekDays[(i + firstDayOfWeek) % 7];
			}
			weekDays = array;
		}
	}

	private float ResolvePickerWidth()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (Width > 0f)
		{
			return Width;
		}
		float x = ImGui.CalcTextSize(ImU8String.op_Implicit(viewDate.ToString(DateFormat, cultureInfo)), false, -1f).X;
		float frameHeight = ImGui.GetFrameHeight();
		return MathF.Max(y: GetCalendarCellWidth() * 7f, x: x + GetNavigationGroupWidth(frameHeight) * 2f);
	}

	private float GetCalendarCellWidth()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		float x = ImGui.CalcTextSize(ImU8String.op_Implicit("0000"), false, -1f).X;
		string[] array = weekDays;
		foreach (string text in array)
		{
			x = MathF.Max(x, ImGui.CalcTextSize(ImU8String.op_Implicit(text), false, -1f).X);
		}
		x = MathF.Max(x, ImGui.CalcTextSize(ImU8String.op_Implicit("30"), false, -1f).X);
		ImGuiStylePtr style = ImGui.GetStyle();
		return x + ((ImGuiStylePtr)(ref style)).FramePadding.X * 2f + ((ImGuiStylePtr)(ref style)).CellPadding.X * 2f;
	}

	private static float GetNavigationGroupWidth(float buttonWidth)
	{
		return buttonWidth * 2f + 2f;
	}

	private static void CenterWithinAvailableWidth(float itemWidth)
	{
		float num = ImGui.GetContentRegionAvail().X - itemWidth;
		if (num > 0f)
		{
			ImGui.SetCursorPosX(ImGui.GetCursorPosX() + num / 2f);
		}
	}
}
