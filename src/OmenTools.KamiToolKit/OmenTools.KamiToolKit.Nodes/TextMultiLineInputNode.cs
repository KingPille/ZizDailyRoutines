using System;
using System.Runtime.CompilerServices;
using FFXIVClientStructs.FFXIV.Client.System.Input;
using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Enums;
using KamiToolKit.Nodes;
using Lumina.Text.ReadOnly;

namespace OmenTools.KamiToolKit.Nodes;

public class TextMultiLineInputNode : TextInputNode
{
	public unsafe uint MaxLines
	{
		get
		{
			return ((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxLine;
		}
		set
		{
			((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxLine = value;
		}
	}

	public unsafe uint MaxBytes
	{
		get
		{
			return ((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxByte;
		}
		set
		{
			((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxByte = value;
		}
	}

	public override ReadOnlySeString String
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return base.String;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			base.String = value;
			base.PlaceholderTextNode.IsVisible = base.PlaceholderString != null && ((ReadOnlySeString)(ref value)).IsEmpty;
			UpdateHeightForContent();
		}
	}

	public override Action<ReadOnlySeString>? OnInputReceived
	{
		get
		{
			return base.OnInputReceived;
		}
		set
		{
			base.OnInputReceived = delegate
			{
				UpdateHeightForContent();
			};
			base.OnInputReceived = (Action<ReadOnlySeString>)Delegate.Combine(base.OnInputReceived, value);
		}
	}

	public bool AutoUpdateHeight { get; set; }

	public Action<float>? HeightChanged { get; set; }

	public unsafe TextMultiLineInputNode()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		base.TextLimitsNode.AlignmentType = (AlignmentType)8;
		TextNode currentTextNode = base.CurrentTextNode;
		currentTextNode.TextFlags = (TextFlags)(currentTextNode.TextFlags | 0x80);
		base.CurrentTextNode.LineSpacing = 14u;
		base.Flags |= TextInputFlags.MultiLine;
		AllowEnterToComplete = false;
		CollisionNode.AddEvent((AtkEventType)12, (Action)InputComplete);
		Unsafe.Write(&((AtkComponentTextInput)base.Component).InputSanitizationFlags, (AllowedEntities)639);
		((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxLine = 255u;
		((AtkUldComponentDataTextInput)(&((AtkComponentTextInput)base.Component).ComponentTextData)).MaxByte = 65535u;
	}

	public void UpdateHeightForContent()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (AutoUpdateHeight)
		{
			int num = Math.Max(1, ((object)String/*cast due to constrained. prefix*/).ToString().Split('\r', '\n').Length);
			uint lineSpacing = base.CurrentTextNode.LineSpacing;
			float num2 = Math.Max(Height, num * lineSpacing + 20);
			float height = Height;
			Height = num2;
			if (Math.Abs(num2 - height) > 0.1f)
			{
				HeightChanged?.Invoke(Height);
			}
		}
	}

	private unsafe void InputComplete()
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		if (((UIInputData)UIInputData.Instance()).IsKeyPressed((SeVirtualKey)13))
		{
			AtkComponentTextInput* asAtkComponentTextInput = ((AtkComponentNode)base.Node).GetAsAtkComponentTextInput();
			int cursorPos = ((AtkComponentTextInput)asAtkComponentTextInput).CursorPos;
			Utf8String val = default(Utf8String);
			((Utf8String)(ref val))._002Ector();
			try
			{
				((Utf8String)(ref val)).SetString("\r");
				((AtkComponentTextInput)asAtkComponentTextInput).WriteString(&val);
			}
			finally
			{
				((IDisposable)(*(Utf8String*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
			((AtkComponentTextInput)asAtkComponentTextInput).CursorPos = cursorPos + 1;
			((AtkComponentTextInput)asAtkComponentTextInput).SelectionStart = cursorPos + 1;
			((AtkComponentTextInput)asAtkComponentTextInput).SelectionEnd = cursorPos + 1;
		}
		base.OnInputComplete?.Invoke(ReadOnlySeString.op_Implicit(((Utf8String)(&((AtkComponentTextInput)base.Component).EvaluatedString)).AsSpan()));
	}
}
