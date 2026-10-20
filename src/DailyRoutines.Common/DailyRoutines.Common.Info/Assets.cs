using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Game.Text;
using Dalamud.Interface.Textures;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text.ReadOnly;
using OmenTools.Extensions;
using OmenTools.Info.Dalamud;

namespace DailyRoutines.Common.Info;

public static class Assets
{
	[CompilerGenerated]
	private static ReadOnlySeString _003CBoxedLetterR_003Ek__BackingField;

	[CompilerGenerated]
	private static ReadOnlySeString _003CBoxedLetterD_003Ek__BackingField;

	[CompilerGenerated]
	private static ReadOnlySeString _003CBoxedLettersDR_003Ek__BackingField;

	[CompilerGenerated]
	private static ReadOnlySeString _003CBracketDailyRoutines_003Ek__BackingField;

	[CompilerGenerated]
	private static ReadOnlySeString _003CBracketDR_003Ek__BackingField;

	public static ISharedImmediateTexture Icon { get; } = DalamudServiceExtension.Instance<ITextureProvider>().GetFromFile(Path.Join(DalamudServiceExtension.Instance().AssemblyLocation.DirectoryName, "Assets", "icon.png"));

	public static ISharedImmediateTexture BackgroundTexture { get; } = DalamudServiceExtension.Instance<ITextureProvider>().GetFromFile(Path.Join(DalamudServiceExtension.Instance().AssemblyLocation.DirectoryName, "Assets", "background.png"));

	public static List<StyleInfo> Styles { get; }

	public unsafe static ReadOnlySeString BoxedLetterR
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!((ReadOnlySeString)(ref _003CBoxedLetterR_003Ek__BackingField)).IsEmpty)
			{
				return _003CBoxedLetterR_003Ek__BackingField;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.PushColorType(34u).Append(SeIconCharExtensions.ToIconString((SeIconChar)57474)).PopColorType();
				return _003CBoxedLetterR_003Ek__BackingField = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public unsafe static ReadOnlySeString BoxedLetterD
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!((ReadOnlySeString)(ref _003CBoxedLetterD_003Ek__BackingField)).IsEmpty)
			{
				return _003CBoxedLetterD_003Ek__BackingField;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.PushColorType(34u).Append(SeIconCharExtensions.ToIconString((SeIconChar)57460)).PopColorType();
				return _003CBoxedLetterD_003Ek__BackingField = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public unsafe static ReadOnlySeString BoxedLettersDR
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			if (!((ReadOnlySeString)(ref _003CBoxedLettersDR_003Ek__BackingField)).IsEmpty)
			{
				return _003CBoxedLettersDR_003Ek__BackingField;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.PushColorType(34u).Append(SeIconCharExtensions.ToIconString((SeIconChar)57460)).Append(SeIconCharExtensions.ToIconString((SeIconChar)57474))
					.PopColorType();
				return _003CBoxedLettersDR_003Ek__BackingField = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public unsafe static ReadOnlySeString BracketDailyRoutines
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (!((ReadOnlySeString)(ref _003CBracketDailyRoutines_003Ek__BackingField)).IsEmpty)
			{
				return _003CBracketDailyRoutines_003Ek__BackingField;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.PushColorType(34u).Append("[ziz]").PopColorType();
				return _003CBracketDailyRoutines_003Ek__BackingField = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	public unsafe static ReadOnlySeString BracketDR
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			if (!((ReadOnlySeString)(ref _003CBracketDR_003Ek__BackingField)).IsEmpty)
			{
				return _003CBracketDR_003Ek__BackingField;
			}
			RentedSeStringBuilder val = default(RentedSeStringBuilder);
			((RentedSeStringBuilder)(ref val))._002Ector();
			try
			{
				((RentedSeStringBuilder)(ref val)).Builder.PushColorType(34u).Append("[DR]").PopColorType();
				return _003CBracketDR_003Ek__BackingField = ((RentedSeStringBuilder)(ref val)).Builder.ToReadOnlySeString();
			}
			finally
			{
				((IDisposable)(*(RentedSeStringBuilder*)(&val))/*cast due to constrained. prefix*/).Dispose();
			}
		}
	}

	static Assets()
	{
		int num = 4;
		List<StyleInfo> list = new List<StyleInfo>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<StyleInfo> span = CollectionsMarshal.AsSpan(list);
		span[0] = new StyleInfo("Crimson Veil Night", "DS1H4sIAAAAAAACCo1XTXObMBD9Kx2dPRmQAINv+WpySDqZxp20vclYwcQYXBk7H53+90qgBSEI6GTA70lvV2+X5S+iaOGeOTO0Qou/6Ke4wfLul7hwzpx/MxSjRTBDa7Rw5HOmUM6ZX4HEr8A8C/QMJRVyo5BpdUdX6vYFlnfU8kG1+hYtvBnKFGinQGEHkw9Ti0HwHsAQRfX0D1rgis3Fk0BeHCp5Jax6hIsTWhD5+1oJe1PC3gfD/mieOtpTStXjoCOMxopO1w3NDRTPFen7XV2ITZ+qFUNCoigMJbNNOiZA8IEQNQTXmYcekYTnnjBHwR1Ay5MtMolbsreywUeuIoRzxQhrRa4gSORVeqCrjLUxBB7EHiqGvFCMpzRfF68XSasnBEGwvhM16MtNmq01sNtEC8FiJX8u0A/F/rjX0X4vmb6CR5HAXxR8zXgDJ1jBsULjuUIHfoN+3FChHzg1oYbXWAH8yumO6TLmIIOAjKCJUIFvixPjWg5xw4lAjGdyzuMyPbVOIGAdDNvgdptlWma6JpnjbtZdE2ssj3tBhCbjssgyuj9oYYxtc8/y4wXlFqcr0Y8xF6uvOvhR6zSEG05XA2fsKQqZD1PME/EgvQROxHeHmUbi5lBAHpQDcTWDs3h7T/m2hUdm9egKs1R4sBNRABF5TURkAG+KGtvl4liWRd5GDmnG0GIwNrBmsgKImbh9+9aUz9zrhv0iuWVUL1QMq7tet+e1WFMQ8c0YCDYohiAPGAQM30bwyPaU07JoJUHLw+BEAm3b83WCqetTc9QdreHZHp/oznK/7+yQfrAbnu6n/StEdxi9s4Sq9CHbAbxhulvZ+l78JYlLzcQDLVLrLgPFOHI4y57ZRw9fwH/kz0V81PsWSHcaM7qDBLNHjhnzqoi3aZ48cHZK2Ws7Pnhm/QaQJuK3tOvdvny37X0PWVHepTk7TB9iB95z51iXkKzb9FAWiXgdtRuF5sngTyi9zcCgfnNKvp70jNV1qiVhNN8VRb20S17kiUWx+n6XeJcmm3L6pSydBcTv+nQzOB+0sPOsnI6lrk85aT2yjMUl0weiEfd40jycJle82C8pT1hp0z7kXPSNnm5F4Fkn+KlyFqR6thNm7bPHdJrsq3Q3lsG6LO6LNc1qSgd/5oCNHLCRM286qvx2ELMTWqDr3YrxLzRff3nIjjskvmfq2ZdaOLmalVcW8y58Lk2OEBVwPWnQGscsZvMK+GwLTCyKsAJubGNJLXpIBXyxzffWVmNmq3Fne4K5xSzX+QAd67edb9JJjX9st+a2eTzY5rG01Xicnq7rbnSyNcXr9OxZr/hmm5532xU/bPNI6fTsWi+pzTkTKaexnUyZSzkPO//+A3eegbGuEQAA                                                                                ");
		span[1] = new StyleInfo("Violet Dusk", "DS1H4sIAAAAAAACCo1XXXObOhD9Kx2ePRlAgMFvTdPbPDR3Mo3vpL1vMlYwNQZXxs5Hp/+9+lohBAE92ZbPkc6uzi7Lbw97q+DKX3gbb/Xb++6tUv7jh/j8s/Byb5UsvK238vkyURhfYfyrmGGe2A4LrxDIndqtFL/wRmF/KmKiiJHYfM++LLxKYQ4KEylMLDC1tSqZjbUaitXjKPYX+1tookybEHAS4lr2Uxx8VoyL2GfhPQtZL0rWq445NmJ+G2Qi4MsYq3Wk1pGQgHPFx1vNCxIgsuT9L76wUx/FlilCWZamnNmlPET6JCBkmhD4yzRCnPA0UOYruA9ofq9NxXFr8tJqfBYoQrpUjFQqChiBI2/KE95UpIshiSAlqWLwL4rxWNbb5vm66PSkIAj29zON/rQrq60BDnS0EGyo5C8Z+r45no8mOh4kM1bwLGP464ZuCdVwFCp4qNDhUqGTWKMfdpjpB44kSLjEMuA/FB+IKWMJMhDISHSECnzbXAg1chhqTgZiIpvzMW/LS+cEBNYJ4ZiwO2ZdtpWpiee4n/XAxlrbh4MgUpvxqakqfDwZYUwdc0fq8zWmDrfL0Q85ZbtvevhJ62jCF4o3I3ccKQpajlPsG4kgvQhuJA7GmVbillBAEZQDCgyDk3x/h+m+g2d29ZgKq5J5sBdRAhFFOiI0grdFTZ1yfW7bpu4ihzSH0GLC0MLayUogZhQM7Ssp77k3SIdFckuwWagh7B5E/Z7XYW1BKLZjQKFFsQRFwEBg+C6CB3LEFLdNJwlaXghORNC2o9gk2LreNYfsaJrnen2sO/PzvpFT+Ua+0PI4718muscY3CVUZQzZTuAJ0z/K1ffsL05cGyYeaZFGdxkpxonLWQ/MPnn5DP5f/dTkZ7NvgXRfmzEYJdg9csqYN02+L+vinpJLSZ67+SGy6zeBNKG4o30+HNtX1953XzXt17Imp/lL7MEH7pzqEpx1W57apmCPo+6g1L6Z8B3K4DAwaKxvKTaTXhFZp0YSJvMtKOqh3dKmLhyKNY77xK9lsWvnH8rcWUD8Zk43o/NBB/tYtfOxyPrkk9YDqUjeEnMgmnBPxM1DcXFDm+Ma04K0Lu2Dz0X/4sstC7zqBT9XzowkZztm1iF7SqfNvikPUxmUZXHXbHElKT38lQ828sFG/lJ3VP7qwGYnb+V9PmwI/YDr7Yf76nzw2NuMnH2xg5PFrLxxmHfhZWl2hBDA7axBJY44zOYC+OQKLByKUAB3rrGUDj1EAH+65nvvqrFy1XhwvcHaYZbrvYBO9dveO+msxl+uR1PXPJ5c89i6ajzPT9eyG11cTfE8P3vKHV9c0/PquuObax4xnp9d5ZbGnDOTcpy7yeS55POw/+cvhRjeRqoRAAA=                                                                                                                                                                                                                                                                                                                                                                    ");
		span[2] = new StyleInfo("Obsidian Rose", "DS1H4sIAAAAAAACCo1XTXObMBD9Kx3OngxIgMG3fDU5JJ1M407a3mSsYGIMroydj07/eyWhBSEI6GTA70lvV2+X5a9DnIV35s6clbP46/x0FpG4+SV//82cxFmEM2ftLFzxmCqMqzDuWcAxz3yFmZNK5Eb9mck7slK3L4oYKqIvF9/yi5mTK8xOYfwOphh8WhpPkXy6V09R5+kffiEjZFynFHCQ4ip+Kzc+qnVO6vdVynpTst6bmAMt5o9eJsRWhAyqJYmik3VD80LF83jufssLvumTXCnCOI6jSDDbjCMMhAAIcUPw3HnkY0F47gtTcBfQ4ljLXOCW9K1q8LGnCNFcMaJakccJAnmVHcgqp20MoQ8ZiRRDXCjGU1asy9eLtNUTgSBY340b9OUmy9ca2GuihWCRki/S8lDuj3sdHfSSGSh4HHP8RcnWlDVwjBQcKTSaK3QYNOjHDeH6gVMTaniN5cCvjOyoLmMOMjDICJsIFfi2PFGm5RA1nBjE+CbnPKmyU+sEDNZBsA1qt1lmVa5rEjnuZt0zscbyqBdEZDIuyzwn+4MWxtg297Q4XhBmcboC/Zgwvvqqgx+1TkO4YWQ1cMa+ouD5MMU8ER/Si+FEAm+YaSRuDgXkQzlgTzM4Tbb3hG1beGxWj64wz7gHOxGFEJHfRIQH8KaosV0ujlVVFm3kkGYELQYhA2smK4SYsde3b035zL1e1C+SW0r0QkWwuud3e16LNQXhwIwBI4NiCPKBgcHwbQSPdE8YqcpWErQ8BE7E0Lb9QCeYuj41B/9L59keH2+EYr/v9JB90BuW7af9y0V3GL2zhKoMINshvGG6W9n6nv8liEvNxAMtUusuA8U4cjjLntlHD5/DfxTPZXLU+xZIdxszeoMEs0eOGfOqTLZZkT4wesroazs++Gb9hpAmHLS0692+erftfQ95Wd1lBT1MH2IH3nPnWJcQrNvsUJUpfx21G0XmyaBPKL3NwKBBc0qBnvSc1nWqJWE035KiXtoVK4vUoliDoEu8y9JNNf1SFs4C4nd9uhmcD1rYeV5Nx1LXp5i0HmlOk4rqA9GIe3xhHkbSK1bul4SltLJpH2Iu+kZOtzzwvBP8VDlzUj3bcbP22WM6TfZVthvLYF0W9+Wa5DWlgz9zwUYu2MidNx1VfDnw2clZONe7FWVfSLH+8pAfdw7/mKlnX2LhZDkrryzmXfhWmhwhJHA9adAaRy1mcwl8tgWmFkUogRvbWDKLHiKBL7b53tpqzG017mxPsLCY5Trfn2P9tvNJOqnxj+3WzDaPB9s8VrYaj9PTdd2NTrameJ2ePesV32zT82674odtHgmZnl3rJbU5ZyLlJLGTKXIp5mH333+x7z+EqREAAA==                                                                                                                                                                                                                    ");
		span[3] = new StyleInfo("Crimson Ash", "DS1H4sIAAAAAAACCo1Xy3KbMBT9lQxrTwYkwOBd07TJIulkEnfSdidjBRNjcGTsPDr990pIF4QgoJUBn3N17pPLX4c4C+/cnTkrZ/HX+eUsInHzu/79N3MSZxHOnLWzcMVjqjCuwrjnAcc8cQszJ62RG2Utq+/ISmGfDeN+bXzLL2ZOrjA7hfE7mEI9DTtPy8Gn+0ELL84CIfGEcW31xaEWV/Hb+uCj0nVSzNda1puS9d74HGg+fwxGghD1WJ7zW16IPxJlgKwbohcqpsej96e+4Mc+1jYjjOM4igSzjTnCQAiAEDcEz51HPhaEp540V8FdQIvElrnALelb1eBjTxGiuWJEUpHHCQJ5mR3IKqetD6EP3keKIS4U4zEr1uXrRdrqiUAQ2HfjBv11k+VrDew13oKzSMmfi1DflfvjXocHvWgGCh/HHH9RsjVlDRwjBUcKjeYKHQYN+mFDuAPAkQQJl1gO/M7Ijuoy5iADg4ywcVGBr8sTZVoQUcOJQYxvcr4kVXZqSwFD7SA4BrXHLLMq1zWJIHfD7plYwzzqORGZjK9lnpP9QXNj7JhbWhwvCLNIr0A/JIxbX3Xwo7XTEK4YWQ3k2FcUPB+mmBnxIbwYMhJ4w0wjcHPoIB/6AXtahdNke0vYtoXHZvvoCvOM12DHoxA88huP8ADeFDV2ysWxqsqi9RzCjGDGIGRgzWCF4DP2+uUrKZ9Vrxf1m+SaEr1REVj3/O7Qa7GmIByYPmBkUAxBPjAwFHzrwQPdE0aqspUEMw9BJWKY236gE0xdnxYH/0vn2aaPj2dx3j09ZB/0imX76frlojuMXi6hKwOIdgivmO5RtnWvpvVSK+KBEalNl4FmHEnOslfso8nn8J/FU5kc9bkF0t2mGL1Bgjkjxwrzsky2WZHeMXrK6Gu7Qfhm/4YQJhy0tG+7ffVuO/vu8rK6yQp6mE5iB96rzrEpIVjX2aEqU/46ag+KzMygTyi9w6BAgyZLgR70nMo+1YIwGu+aol7aFSuL1KJZg6BLvMnSTTX9UhaVBcR7fb0Z3A9a2Je8mvZF9qdYtR5oTpOK6hvRSPX4ongYSS9ZuV8SltLKZnyIvegHOV1zx/OO81PtzElyuePF2meP6TTZl9luLIKyLW7LNcklpYM/d6GMXCgjd95MVPHxwHcnZ+F8260oOyPF+uwuP+4c/j0jl19iUcn1sryyWHjhc2lyhaiB68kClThqsZzXwCdbYGrRhDVwY+tLZjFDauCzbby3thpzW4072wwWFrtc5xN0bN52vkonNb7YHs1s43iwjWNlq/E4vV3LaXSyLYrX6d1TWnyzDc+7rcUP2zgSMr27SpPanjMRcpLYyRSxFPuw++8/H1ff1qwRAAA=                                                                                                                                                                                                                                                                                                                                                                                                                        ");
		Styles = list;
	}
}
