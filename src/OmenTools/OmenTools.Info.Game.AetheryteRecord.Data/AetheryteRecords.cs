using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Info.Game.AetheryteRecord.Data;

public static class AetheryteRecords
{
	internal static readonly FrozenDictionary<uint, List<Vector3>> CosmicExplorationPositions;

	internal static readonly FrozenDictionary<uint, Vector3> FirmamentPositions;

	internal static readonly FrozenDictionary<uint, uint> CosmicPlaceNameOffsets;

	internal static readonly FrozenDictionary<uint, Vector3> NormalPositions;

	internal static readonly Dictionary<uint, byte> NormalVersions;

	public static FrozenSet<byte> AethernetGroups { get; }

	static AetheryteRecords()
	{
		Dictionary<uint, List<Vector3>> dictionary = new Dictionary<uint, List<Vector3>>();
		int num = 5;
		List<Vector3> list = new List<Vector3>(num);
		CollectionsMarshal.SetCount(list, num);
		Span<Vector3> span = CollectionsMarshal.AsSpan(list);
		span[0] = new Vector3(-1f, 1.6f, -28.9f);
		span[1] = new Vector3(-541.2f, 59.5f, -530.1f);
		span[2] = new Vector3(429.1f, 45.5f, 499.8f);
		span[3] = new Vector3(-621.2f, 50.5f, 396.2f);
		span[4] = new Vector3(632f, -73.9f, -575.2f);
		dictionary[1237u] = list;
		num = 5;
		List<Vector3> list2 = new List<Vector3>(num);
		CollectionsMarshal.SetCount(list2, num);
		Span<Vector3> span2 = CollectionsMarshal.AsSpan(list2);
		span2[0] = new Vector3(338.2f, 52.6f, -384.8f);
		span2[1] = new Vector3(-154.3f, 29f, 313.8f);
		span2[2] = new Vector3(-643.3f, -1.5f, -630.4f);
		span2[3] = new Vector3(675.4f, -242f, 425.5f);
		span2[4] = new Vector3(-594.7f, 28f, 721.3f);
		dictionary[1291u] = list2;
		num = 5;
		List<Vector3> list3 = new List<Vector3>(num);
		CollectionsMarshal.SetCount(list3, num);
		Span<Vector3> span3 = CollectionsMarshal.AsSpan(list3);
		span3[0] = new Vector3(-165.9f, 0.5f, 78.9f);
		span3[1] = new Vector3(-140f, -74.9f, -587.2f);
		span3[2] = new Vector3(-451.4f, 102.1f, 763f);
		span3[3] = new Vector3(731.3f, 218.8f, -102f);
		span3[4] = new Vector3(-121f, -193.7f, -796.9f);
		dictionary[1310u] = list3;
		num = 5;
		List<Vector3> list4 = new List<Vector3>(num);
		CollectionsMarshal.SetCount(list4, num);
		Span<Vector3> span4 = CollectionsMarshal.AsSpan(list4);
		span4[0] = new Vector3(263.4f, 205.7f, 353.6f);
		span4[1] = new Vector3(-226.2f, 145f, -556f);
		span4[2] = new Vector3(-244.2f, 168.1f, 317.5f);
		span4[3] = new Vector3(-599.7f, 206.6f, -371.4f);
		span4[4] = new Vector3(-676.6f, 115f, 626.7f);
		dictionary[1319u] = list4;
		CosmicExplorationPositions = dictionary.ToFrozenDictionary();
		FirmamentPositions = new Dictionary<uint, Vector3>
		{
			[0u] = new Vector3(-92.8f, -0f, -116.3f),
			[1u] = new Vector3(111.9f, -20f, -107.7f),
			[2u] = new Vector3(77.7f, -18.5f, 6.4f),
			[3u] = new Vector3(204.1f, -40f, -25.7f),
			[4u] = new Vector3(147.1f, -50f, 95.7f),
			[5u] = new Vector3(-77.9f, 0f, 79.6f),
			[6u] = new Vector3(20.8f, -16f, 168.1f),
			[7u] = new Vector3(-135f, 10f, -11.6f)
		}.ToFrozenDictionary();
		CosmicPlaceNameOffsets = new Dictionary<uint, uint>
		{
			[1237u] = 5446u,
			[1291u] = 5456u,
			[1310u] = 5451u,
			[1319u] = 5546u
		}.ToFrozenDictionary();
		NormalPositions = new Dictionary<uint, Vector3>
		{
			[2u] = new Vector3(37.22f, 1.3f, 36.52f),
			[3u] = new Vector3(19.34f, -0.99f, 34.33f),
			[4u] = new Vector3(-195.63f, 3.38f, 292.4f),
			[5u] = new Vector3(186.77f, 9.16f, -72.96f),
			[6u] = new Vector3(-224.02f, 21.56f, 348.84f),
			[7u] = new Vector3(-52.5f, -40.77f, 234.68f),
			[8u] = new Vector3(-98.15f, 18.7f, 3.51f),
			[9u] = new Vector3(-144.2f, -3.19f, -160.56f),
			[10u] = new Vector3(167.41f, 14.2f, 670.66f),
			[11u] = new Vector3(483.18f, 17.36f, 460.26f),
			[12u] = new Vector3(-11.61f, 70.6f, 9.73f),
			[13u] = new Vector3(659f, 9f, 509f),
			[14u] = new Vector3(260f, -22.75f, 227f),
			[15u] = new Vector3(438f, 4.2f, 105f),
			[16u] = new Vector3(-111.15f, 64.37f, -219.82f),
			[17u] = new Vector3(61.36f, 45.1f, -222.16f),
			[18u] = new Vector3(-386.31f, -58.9f, 135.49f),
			[19u] = new Vector3(-161.19f, 26.24f, -408.68f),
			[20u] = new Vector3(-326.85f, 8.36f, 400.22f),
			[21u] = new Vector3(11.82f, 7.08f, 453.58f),
			[22u] = new Vector3(-24.73f, 48.41f, -25.22f),
			[23u] = new Vector3(214.98f, 312.1f, -229.02f),
			[24u] = new Vector3(43.7f, 20.6f, -658.71f),
			[25u] = new Vector3(168f, -2.6f, 82.7f),
			[26u] = new Vector3(102.2f, 8.5f, -108.5f),
			[27u] = new Vector3(117.6f, 11.6f, -230.9f),
			[28u] = new Vector3(-146f, 4f, -14f),
			[29u] = new Vector3(-307.7f, 7.1f, -174.1f),
			[30u] = new Vector3(-70.5f, 7f, -135.5f),
			[31u] = new Vector3(128.9f, 24.7f, -301.8f),
			[32u] = new Vector3(447.9f, -1.8f, 198.3f),
			[33u] = new Vector3(66f, 4f, -118f),
			[34u] = new Vector3(-155.32f, 14.11f, 71.12f),
			[35u] = new Vector3(-52f, 10f, 11f),
			[36u] = new Vector3(32f, 12f, 109f),
			[37u] = new Vector3(-98.96f, 41.1f, 92.26f),
			[38u] = new Vector3(450.5f, 93.9f, 158.1f),
			[39u] = new Vector3(-110.9f, 15.7f, 317.7f),
			[40u] = new Vector3(35.1f, 15.9f, 570.2f),
			[41u] = new Vector3(18.3f, 40f, 70.5f),
			[42u] = new Vector3(-56f, 42f, -129.5f),
			[43u] = new Vector3(-333f, 12f, 55f),
			[44u] = new Vector3(-180.5f, 4f, 182.5f),
			[45u] = new Vector3(-3.1f, 41.9f, 146.5f),
			[46u] = new Vector3(-29.8f, 70.9f, 117.4f),
			[47u] = new Vector3(92.88f, 12.1f, 60.58f),
			[48u] = new Vector3(-2.8f, 44f, -217.8f),
			[49u] = new Vector3(-213.5f, 16f, 49.5f),
			[50u] = new Vector3(-20f, 14f, 75f),
			[51u] = new Vector3(5.73f, 30.1f, -21.77f),
			[52u] = new Vector3(232.6f, 112.96f, -260.52f),
			[53u] = new Vector3(-16.07f, -1.78f, -170.62f),
			[54u] = new Vector3(-304.6f, 63.4f, -279.1f),
			[55u] = new Vector3(38.6f, 5.7f, -4.41f),
			[62u] = new Vector3(5.72f, 1.14f, 0.79f),
			[63u] = new Vector3(-61.2f, 0f, 50.9f),
			[64u] = new Vector3(57f, 21f, 64f),
			[65u] = new Vector3(0f, 21f, 57f),
			[66u] = new Vector3(94.6f, -5f, -67.4f),
			[67u] = new Vector3(113f, 13f, -38f),
			[68u] = new Vector3(-24.4f, 3.3f, -81.4f),
			[69u] = new Vector3(-11.8f, 0f, -35.1f),
			[70u] = new Vector3(-71.27f, 8.21f, 37.4f),
			[71u] = new Vector3(483.15f, 212.68f, 719.42f),
			[72u] = new Vector3(-608f, -122.5f, 543f),
			[73u] = new Vector3(-621.5f, -50.95f, -426.14f),
			[74u] = new Vector3(-714.2f, -186.86f, -591.05f),
			[75u] = new Vector3(79.78f, 207.64f, -28.42f),
			[76u] = new Vector3(535.24f, -51.18f, 40.62f),
			[77u] = new Vector3(-295.26f, -21.03f, 33.77f),
			[78u] = new Vector3(266.34f, -42.01f, 586.48f),
			[79u] = new Vector3(-577.84f, 49.87f, 316.73f),
			[80u] = new Vector3(49.4f, 24f, -2.5f),
			[81u] = new Vector3(-106.3f, 15.1f, -32.7f),
			[82u] = new Vector3(50.9f, -12f, 70.2f),
			[83u] = new Vector3(140.3f, -9.2f, -62.9f),
			[84u] = new Vector3(-139f, -12.5f, -19f),
			[85u] = new Vector3(-84.9f, 10.1f, -121f),
			[86u] = new Vector3(79.5f, 10f, -122.5f),
			[87u] = new Vector3(1.6f, 16f, -39.3f),
			[88u] = new Vector3(-163f, 304f, -321f),
			[89u] = new Vector3(51f, -0f, 26.1f),
			[90u] = new Vector3(-70f, 208.5f, -23.5f),
			[91u] = new Vector3(-529.4f, 152.8f, -486f),
			[92u] = new Vector3(-219.9f, 104.3f, -600.8f),
			[93u] = new Vector3(-11.5f, 91.5f, -12.5f),
			[94u] = new Vector3(24.5f, -19f, 97f),
			[95u] = new Vector3(-17f, 83f, -8f),
			[98u] = new Vector3(-625f, 130f, -504f),
			[99u] = new Vector3(414f, 114.3f, 240f),
			[100u] = new Vector3(108.55f, 118.16f, -739.3f),
			[101u] = new Vector3(-266.02f, 257.87f, 743.39f),
			[102u] = new Vector3(-641.97f, 50f, -15.28f),
			[103u] = new Vector3(607.16f, 81f, 646.32f),
			[104u] = new Vector3(83.04f, 0.1f, 96.82f),
			[105u] = new Vector3(360.6f, -119.19f, -271.93f),
			[106u] = new Vector3(94.58f, 3.03f, -576.47f),
			[107u] = new Vector3(438.09f, 68.56f, -95.61f),
			[108u] = new Vector3(250.15f, 4.9f, -395.3f),
			[109u] = new Vector3(546.14f, -19.51f, 337.31f),
			[110u] = new Vector3(74.06f, 114.9f, 25.74f),
			[111u] = new Vector3(41f, 4.5f, -40f),
			[112u] = new Vector3(-76f, -5f, -80f),
			[113u] = new Vector3(-113f, -5f, 151.5f),
			[114u] = new Vector3(36f, 8f, 147f),
			[115u] = new Vector3(27f, 4f, 67f),
			[116u] = new Vector3(-77.1f, 18f, -166.5f),
			[117u] = new Vector3(128f, 12f, 84f),
			[118u] = new Vector3(120f, 12f, -88f),
			[119u] = new Vector3(26f, 6f, -148f),
			[120u] = new Vector3(843.5f, 5.9f, 845.5f),
			[121u] = new Vector3(-78f, 0.1f, 9f),
			[122u] = new Vector3(97f, 2f, -112.5f),
			[123u] = new Vector3(429f, 61.3f, -531.8f),
			[124u] = new Vector3(-644f, 50.9f, -788.8f),
			[125u] = new Vector3(132f, 4f, -35f),
			[126u] = new Vector3(-55.6f, 79.1f, 44.5f),
			[127u] = new Vector3(50.3f, 0.2f, -23.7f),
			[128u] = new Vector3(-745.06f, 127.92f, 115.91f),
			[129u] = new Vector3(12.4f, -0f, -102.5f),
			[130u] = new Vector3(-59.9f, 0f, 86.2f),
			[131u] = new Vector3(-486.1f, 1.2f, 537.9f),
			[132u] = new Vector3(744.98f, 22.7f, -24.9f),
			[133u] = new Vector3(-58f, 2.94f, 6.41f),
			[134u] = new Vector3(-1.31f, 82.15f, -10.61f),
			[135u] = new Vector3(70.1f, -10.4f, 65.2f),
			[136u] = new Vector3(-724.09f, 51.81f, -227.63f),
			[137u] = new Vector3(660.93f, 28.22f, 286.76f),
			[138u] = new Vector3(-234.06f, 19.38f, 384.91f),
			[139u] = new Vector3(-424.13f, 417.27f, -614.18f),
			[140u] = new Vector3(237.36f, 10.43f, -229.03f),
			[141u] = new Vector3(-505.89f, 45.88f, -219.83f),
			[142u] = new Vector3(-98.67f, -19.08f, 287.99f),
			[143u] = new Vector3(373.95f, 20.4f, -186.91f),
			[144u] = new Vector3(-334.39f, 48.09f, 508.24f),
			[145u] = new Vector3(-73.89f, 102.28f, -847.18f),
			[146u] = new Vector3(371.57f, 86.7f, -690.32f),
			[147u] = new Vector3(549.73f, 349.21f, -199.71f),
			[148u] = new Vector3(-156.63f, -281.39f, 223.24f),
			[149u] = new Vector3(-9f, -7.7f, 144.5f),
			[150u] = new Vector3(-109.3f, 0f, -60.1f),
			[151u] = new Vector3(62.3f, -0f, -17.9f),
			[152u] = new Vector3(34.5f, 0.1f, 217.5f),
			[153u] = new Vector3(67.2f, 36f, -133.5f),
			[154u] = new Vector3(-51f, 20f, -170f),
			[155u] = new Vector3(-56.9f, -37.7f, -238.9f),
			[156u] = new Vector3(826.6f, 36f, 407.2f),
			[157u] = new Vector3(12f, 36f, -5.5f),
			[158u] = new Vector3(-56.8f, -0.8f, 51.9f),
			[159u] = new Vector3(5.5f, 5.9f, -60f),
			[160u] = new Vector3(174.1f, 56.9f, 851.9f),
			[161u] = new Vector3(396.12f, -28.26f, 298.42f),
			[162u] = new Vector3(99.4f, -4.2f, 78.9f),
			[163u] = new Vector3(-26.82f, -0.5f, -36.69f),
			[166u] = new Vector3(444.76f, 169.33f, -471.26f),
			[167u] = new Vector3(7.8f, -28.62f, -43.67f),
			[168u] = new Vector3(-724.67f, -28.62f, 301.98f),
			[169u] = new Vector3(186.73f, 5.9f, 625.49f),
			[170u] = new Vector3(-518.29f, 2.69f, 42.37f),
			[171u] = new Vector3(398.14f, 3.91f, -243.42f),
			[172u] = new Vector3(-402.74f, 22.99f, 474.02f),
			[173u] = new Vector3(511.9f, -35.94f, -177.17f),
			[174u] = new Vector3(-591.5f, 132.5f, 663.9f),
			[175u] = new Vector3(-5.31f, -131.03f, -503.98f),
			[176u] = new Vector3(166.54f, 10.97f, 131.2f),
			[177u] = new Vector3(-625.1f, -21.29f, 537.43f),
			[178u] = new Vector3(-532.23f, 159.86f, -212.08f),
			[179u] = new Vector3(-539.66f, 71.89f, 260.54f),
			[180u] = new Vector3(55.53f, 269.17f, -650.63f),
			[181u] = new Vector3(487.32f, 437.1f, 326.23f),
			[182u] = new Vector3(-0.13f, 3.37f, 5.74f),
			[183u] = new Vector3(36.28f, 0.2f, -25.58f),
			[184u] = new Vector3(-286.7f, 20f, -74.9f),
			[185u] = new Vector3(-92f, 2.6f, 34.9f),
			[186u] = new Vector3(-31.5f, 41.3f, -157.5f),
			[187u] = new Vector3(208.9f, 21.8f, -121f),
			[188u] = new Vector3(210.5f, 1.8f, 18.5f),
			[189u] = new Vector3(14.1f, -16.2f, 129.9f),
			[190u] = new Vector3(-28.8f, 179.7f, -812.4f),
			[191u] = new Vector3(-367.4f, 45f, -26.4f),
			[192u] = new Vector3(-160.6f, 36f, 27.7f),
			[193u] = new Vector3(-146.7f, 28f, 197.7f),
			[194u] = new Vector3(7.8f, -2f, 107.4f),
			[195u] = new Vector3(-137.4f, 4f, -95.1f),
			[196u] = new Vector3(-47.4f, -0f, -197.2f),
			[197u] = new Vector3(520.3f, 55.3f, -624.6f),
			[198u] = new Vector3(135.5f, 27f, 15.1f),
			[199u] = new Vector3(60.4f, -24.7f, -212.8f),
			[200u] = new Vector3(338.7f, -159.29f, -424.42f),
			[201u] = new Vector3(464.81f, 115.78f, 625.07f),
			[202u] = new Vector3(-164.62f, 6.43f, -468.4f),
			[203u] = new Vector3(535.02f, 116.6f, 196.34f),
			[204u] = new Vector3(-468.06f, 122.74f, 309.88f),
			[205u] = new Vector3(-388.14f, 24.19f, -430.33f),
			[206u] = new Vector3(713.43f, -132.64f, 519.47f),
			[207u] = new Vector3(387.98f, 0.06f, 460.88f),
			[208u] = new Vector3(-284.37f, 19.74f, -108.54f),
			[209u] = new Vector3(303.08f, -13.69f, -561.89f),
			[210u] = new Vector3(512.34f, 146.3f, 197.36f),
			[211u] = new Vector3(-215.15f, 30.3f, -575.72f),
			[212u] = new Vector3(-218.94f, 31.1f, 111f),
			[213u] = new Vector3(-1.89f, 53.86f, 785.46f),
			[214u] = new Vector3(651.75f, 25.27f, -274.49f),
			[215u] = new Vector3(-244.59f, 54.46f, -399.3f),
			[216u] = new Vector3(-33f, 0f, 10.8f),
			[217u] = new Vector3(6.8f, 8.5f, 7f),
			[218u] = new Vector3(-418.2f, 3f, -47.8f),
			[219u] = new Vector3(-184.5f, 39.9f, 2.9f),
			[220u] = new Vector3(-146.9f, -15f, 194.7f),
			[221u] = new Vector3(-13.3f, -10f, 140f),
			[222u] = new Vector3(-96.3f, 100.8f, -220f),
			[223u] = new Vector3(173.6f, -18f, 32.6f),
			[224u] = new Vector3(67.3f, 47f, -331f),
			[225u] = new Vector3(650.5f, -120f, -444.2f),
			[226u] = new Vector3(-283.7f, 13f, -814.9f),
			[227u] = new Vector3(744f, 114.6f, 564.1f),
			[228u] = new Vector3(18f, 8.2f, -666.2f),
			[229u] = new Vector3(283.6f, 16f, 771.9f),
			[230u] = new Vector3(-31.3f, -6.1f, 214f),
			[231u] = new Vector3(383.6f, 60f, 72.4f),
			[232u] = new Vector3(261.8f, 50.8f, 145.5f),
			[233u] = new Vector3(369.8f, 60.1f, 324.8f),
			[234u] = new Vector3(-28.6f, 38.1f, -342f),
			[235u] = new Vector3(-156.94f, 0.1f, 20.7f),
			[236u] = new Vector3(-373.9f, 14f, 138.7f),
			[237u] = new Vector3(-293f, 45.9f, -831.8f),
			[238u] = new Vector3(780.25f, 13.71f, -232.22f),
			[239u] = new Vector3(43.1f, 0f, -2f)
		}.ToFrozenDictionary();
		AethernetGroups = ((IEnumerable<Aetheryte>)LuminaGetter.Get<Aetheryte>()).Select((Aetheryte x) => ((Aetheryte)(ref x)).AethernetGroup).ToFrozenSet();
		NormalVersions = new Dictionary<uint, byte>
		{
			[88u] = 1,
			[163u] = 2
		};
	}
}
