using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using B2nED1Wmdu0xsDR7Tjv;
using DailyRoutines.Internal;
using DailyRoutines.Manager;
using Lumina.Text.ReadOnly;
using OmenTools.Interop.Runtime;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;
using itAnDoW4mgYVlrmtMor;

namespace Lhk7YYyZgDg4lZPGxVj;

internal abstract class XX0GFgy2GYwT9tTLeoM : IDisposable
{
	protected const string SALT = "NekoV#";

	[CompilerGenerated]
	private Action<XX0GFgy2GYwT9tTLeoM>? TokenChanged;

	protected abstract char Prefix { get; }

	public string Token { get; protected set; }

	public DateTime? ExpiredTime { get; protected set; }

	public bool IsEmpty { get; private set; }

	public bool IsActivated
	{
		get
		{
			if (ExpiredTime.HasValue)
			{
				return ExpiredTime > OmenServiceBase<StandardTimeManager>.Instance().Now;
			}
			return false;
		}
	}

	internal event Action<XX0GFgy2GYwT9tTLeoM>? imUyUSD2Ep
	{
		[CompilerGenerated]
		add
		{
			Action<XX0GFgy2GYwT9tTLeoM> action = TokenChanged;
			Action<XX0GFgy2GYwT9tTLeoM> action2;
			do
			{
				action2 = action;
				Action<XX0GFgy2GYwT9tTLeoM> value2 = (Action<XX0GFgy2GYwT9tTLeoM>)Delegate.Combine(action2, value);
				action = Interlocked.CompareExchange(ref TokenChanged, value2, action2);
			}
			while ((object)action != action2);
		}
		[CompilerGenerated]
		remove
		{
			Action<XX0GFgy2GYwT9tTLeoM> action = TokenChanged;
			Action<XX0GFgy2GYwT9tTLeoM> action2;
			do
			{
				action2 = action;
				Action<XX0GFgy2GYwT9tTLeoM> value2 = (Action<XX0GFgy2GYwT9tTLeoM>)Delegate.Remove(action2, value);
				action = Interlocked.CompareExchange(ref TokenChanged, value2, action2);
			}
			while ((object)action != action2);
		}
	}

	protected abstract string DMFWd84TNs();

	protected virtual void Dispose(bool disposing)
	{
	}

	protected static UInt128 Q6hyajaNPg(string input, string salt = "NekoV#")
	{
		if (!string.IsNullOrEmpty(input))
		{
			return StructMarshaller.FromBytes<UInt128>(MD5.HashData(Encoding.ASCII.GetBytes(salt + input)));
		}
		return UInt128.Zero;
	}

	protected void K5Py0vmcLH()
	{
		mQry7j62Yw(string.Empty);
		IsEmpty = true;
	}

	internal bool qnly5HnBCD(out UInt128 hash)
	{
		hash = Q6hyajaNPg(DMFWd84TNs(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x4118B473 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_55c846b516154967bf0c321596d2dc52));
		return hash != UInt128.Zero;
	}

	internal void BsXyRSJ0vD()
	{
		UInt128 uInt = Q6hyajaNPg(DMFWd84TNs(), GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x3B5B9DD0 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_1e38c41bbda54d9abc5b426283576a9a));
		if (uInt == UInt128.Zero || char.IsWhiteSpace(Prefix))
		{
			mQry7j62Yw(string.Empty);
			IsEmpty = true;
			return;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 2);
		defaultInterpolatedStringHandler.AppendFormatted(Prefix);
		defaultInterpolatedStringHandler.AppendFormatted(uInt, GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x70C32CD9 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_c041f06f3e304b758cab1109bed434d0));
		mQry7j62Yw(defaultInterpolatedStringHandler.ToStringAndClear());
		IsEmpty = false;
	}

	internal void YBuyDM9sP9(DateTime? expiredTime, bool notifyIfChanged = false)
	{
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		DateTime? expiredTime2 = ExpiredTime;
		ExpiredTime = expiredTime;
		if (notifyIfChanged && expiredTime.HasValue && !(expiredTime2 == expiredTime) && PluginConfig.Instance().SendOnlineAuthInfo)
		{
			string text = LanguageManager.Get(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x9073768 ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_444e42c3749640698bcaba67a1c5b5f3), Prefix, expiredTime.Value.ToString(GKXiHlWMTejsXms4kXL.ULtWsbI0Yb(0x34A3A2BC ^ _003CModule_003E_007Bb07c02b2_002D80c8_002D47e8_002Db3db_002Df13343a41fb5_007D.m_bceffe6711b141828cad1baee33acfbd.m_5e416caedcb6438f915024f390fd9079)), (expiredTime - OmenServiceBase<StandardTimeManager>.Instance().Now).Value.Days);
			if (GameState.IsLoggedIn)
			{
				OmenServiceBase<NotifyHelper>.Instance().Chat(ReadOnlySeString.op_Implicit(text));
			}
		}
	}

	public static ushort D4yyeUGbir(char prefix)
	{
		return prefix switch
		{
			'M' => 2, 
			'A' => 3, 
			'T' => 4, 
			'S' => 1, 
			_ => 0, 
		};
	}

	public static char OI8yNHAALU(ushort id)
	{
		return id switch
		{
			2 => 'M', 
			3 => 'A', 
			4 => 'T', 
			1 => 'S', 
			0 => 'C', 
			_ => 'M', 
		};
	}

	public void Dispose()
	{
		Dispose(disposing: true);
	}

	private void mQry7j62Yw(string token)
	{
		if (string.Equals(Token, token, StringComparison.Ordinal))
		{
			if (string.IsNullOrEmpty(token))
			{
				ExpiredTime = null;
			}
		}
		else
		{
			Token = token;
			ExpiredTime = null;
			TokenChanged?.Invoke(this);
		}
	}

	protected XX0GFgy2GYwT9tTLeoM()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		Token = string.Empty;
		IsEmpty = true;
		base._002Ector();
	}
}
