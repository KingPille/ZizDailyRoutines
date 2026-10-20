using B2nED1Wmdu0xsDR7Tjv;
using Lhk7YYyZgDg4lZPGxVj;
using OmenTools.OmenService;
using OmenTools.OmenService.Abstractions;

namespace ThFWmWFIGyTX23kBtl;

internal class ksA3tkbnvUocY33ps0 : XX0GFgy2GYwT9tTLeoM
{
	protected override char Prefix => 'A';

	internal ksA3tkbnvUocY33ps0()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		OmenServiceBase<LocalPlayerState>.Instance().AccountIDChanged += d5Hq41qlW;
		BsXyRSJ0vD();
	}

	protected override void Dispose(bool disposing)
	{
		OmenServiceBase<LocalPlayerState>.Instance().AccountIDChanged -= d5Hq41qlW;
	}

	protected override string DMFWd84TNs()
	{
		if (LocalPlayerState.AccountID == 0L)
		{
			return string.Empty;
		}
		return LocalPlayerState.AccountID.ToString();
	}

	private void d5Hq41qlW(ulong _)
	{
		BsXyRSJ0vD();
	}
}
