using System.Collections.Frozen;
using System.Collections.Generic;
using System.Numerics;
using OmenTools.Info.Models;

namespace OmenTools.Info.Game.Data;

public static class Positions
{
	public static FrozenDictionary<uint, Polygon[]> DivableAreas { get; } = new Dictionary<uint, Polygon[]>
	{
		[139u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[13]
			{
				new Vector2(-248f, -45f),
				new Vector2(-228f, 94f),
				new Vector2(-321f, 93f),
				new Vector2(-272f, 219f),
				new Vector2(-228f, 213f),
				new Vector2(-159f, 287f),
				new Vector2(226f, 273f),
				new Vector2(253f, 217f),
				new Vector2(269f, 99f),
				new Vector2(195f, 86f),
				new Vector2(286f, 5f),
				new Vector2(194f, -133f),
				new Vector2(-177f, -83f)
			}))
		},
		[621u] = new Polygon[3]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[55]
			{
				new Vector2(136f, -157f),
				new Vector2(140f, -259f),
				new Vector2(68f, -355f),
				new Vector2(-16f, -496f),
				new Vector2(-40f, -538f),
				new Vector2(-94f, -489f),
				new Vector2(-72f, -387f),
				new Vector2(-88f, -301f),
				new Vector2(-147f, -274f),
				new Vector2(-169f, -249f),
				new Vector2(-204f, -246f),
				new Vector2(-217f, -200f),
				new Vector2(-251f, -175f),
				new Vector2(-215f, -128f),
				new Vector2(-196f, -95f),
				new Vector2(-195f, -26f),
				new Vector2(-146f, 68f),
				new Vector2(-100f, 89f),
				new Vector2(-75f, 134f),
				new Vector2(-83f, 226f),
				new Vector2(-98f, 265f),
				new Vector2(-170f, 246f),
				new Vector2(-158f, 298f),
				new Vector2(-171f, 330f),
				new Vector2(-160f, 372f),
				new Vector2(-136f, 401f),
				new Vector2(-133f, 439f),
				new Vector2(-84f, 471f),
				new Vector2(-5f, 505f),
				new Vector2(41f, 477f),
				new Vector2(65f, 485f),
				new Vector2(134f, 491f),
				new Vector2(156f, 450f),
				new Vector2(232f, 426f),
				new Vector2(236f, 410f),
				new Vector2(250f, 398f),
				new Vector2(269f, 391f),
				new Vector2(270f, 325f),
				new Vector2(265f, 337f),
				new Vector2(243f, 338f),
				new Vector2(243f, 318f),
				new Vector2(270f, 313f),
				new Vector2(270f, 290f),
				new Vector2(250f, 298f),
				new Vector2(241f, 278f),
				new Vector2(266f, 260f),
				new Vector2(134f, 129f),
				new Vector2(125f, 99f),
				new Vector2(133f, 73f),
				new Vector2(147f, 42f),
				new Vector2(150f, 8f),
				new Vector2(152f, -81f),
				new Vector2(119f, -105f),
				new Vector2(121f, -119f),
				new Vector2(132f, -151f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[15]
			{
				new Vector2(143f, -31f),
				new Vector2(161f, -25f),
				new Vector2(161f, -20f),
				new Vector2(169f, -19f),
				new Vector2(190f, -18f),
				new Vector2(200f, -17f),
				new Vector2(247f, -20f),
				new Vector2(258f, -16f),
				new Vector2(255f, -9f),
				new Vector2(244f, -15f),
				new Vector2(169f, -9f),
				new Vector2(167f, -5f),
				new Vector2(162f, -5f),
				new Vector2(163f, 3f),
				new Vector2(143f, 5f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[11]
			{
				new Vector2(319f, 335f),
				new Vector2(356f, 358f),
				new Vector2(377f, 388f),
				new Vector2(449f, 432f),
				new Vector2(527f, 477f),
				new Vector2(515f, 493f),
				new Vector2(470f, 471f),
				new Vector2(427f, 447f),
				new Vector2(366f, 406f),
				new Vector2(336f, 371f),
				new Vector2(306f, 353f)
			}))
		},
		[613u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[4]
			{
				new Vector2(-1024f, -1024f),
				new Vector2(1024f, 1024f),
				new Vector2(1024f, -1024f),
				new Vector2(-1024f, 1024f)
			}))
		},
		[614u] = new Polygon[2]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[21]
			{
				new Vector2(-465f, -699f),
				new Vector2(-483f, -653f),
				new Vector2(-523f, -644f),
				new Vector2(-673f, -501f),
				new Vector2(-736f, -363f),
				new Vector2(-746f, -117f),
				new Vector2(-694f, -131f),
				new Vector2(-615f, -124f),
				new Vector2(-600f, -254f),
				new Vector2(-571f, -272f),
				new Vector2(-546f, -285f),
				new Vector2(-530f, -313f),
				new Vector2(-485f, -335f),
				new Vector2(-460f, -324f),
				new Vector2(-445f, -361f),
				new Vector2(-420f, -383f),
				new Vector2(-384f, -424f),
				new Vector2(-401f, -451f),
				new Vector2(-378f, -488f),
				new Vector2(-393f, -517f),
				new Vector2(-371f, -597f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[35]
			{
				new Vector2(-620f, 439f),
				new Vector2(-640f, 474f),
				new Vector2(-659f, 772f),
				new Vector2(-595f, 887f),
				new Vector2(722f, 898f),
				new Vector2(914f, 706f),
				new Vector2(914f, 640f),
				new Vector2(860f, 645f),
				new Vector2(738f, 580f),
				new Vector2(721f, 539f),
				new Vector2(560f, 476f),
				new Vector2(493f, 533f),
				new Vector2(522f, 565f),
				new Vector2(543f, 567f),
				new Vector2(550f, 590f),
				new Vector2(525f, 586f),
				new Vector2(530f, 620f),
				new Vector2(542f, 630f),
				new Vector2(525f, 645f),
				new Vector2(550f, 664f),
				new Vector2(561f, 726f),
				new Vector2(509f, 734f),
				new Vector2(510f, 788f),
				new Vector2(475f, 833f),
				new Vector2(431f, 836f),
				new Vector2(326f, 766f),
				new Vector2(252f, 772f),
				new Vector2(11f, 641f),
				new Vector2(-57f, 677f),
				new Vector2(-160f, 704f),
				new Vector2(-315f, 685f),
				new Vector2(-358f, 684f),
				new Vector2(-402f, 618f),
				new Vector2(-451f, 601f),
				new Vector2(-593f, 426f)
			}))
		},
		[622u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[14]
			{
				new Vector2(-195f, 110f),
				new Vector2(40f, 185f),
				new Vector2(1f, 214f),
				new Vector2(-40f, 200f),
				new Vector2(-85f, 196f),
				new Vector2(-155f, 225f),
				new Vector2(-192f, 183f),
				new Vector2(-229f, 153f),
				new Vector2(-224f, 73f),
				new Vector2(-214f, 5f),
				new Vector2(-96f, -29f),
				new Vector2(-38f, -95f),
				new Vector2(103f, -70f),
				new Vector2(185f, 2f)
			}))
		},
		[813u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[28]
			{
				new Vector2(-882f, 375f),
				new Vector2(-882f, 869f),
				new Vector2(-823f, 928f),
				new Vector2(34f, 928f),
				new Vector2(21f, 765f),
				new Vector2(-45f, 751f),
				new Vector2(-73f, 785f),
				new Vector2(-146f, 784f),
				new Vector2(-200f, 808f),
				new Vector2(-305f, 759f),
				new Vector2(-290f, 707f),
				new Vector2(-167f, 658f),
				new Vector2(-33f, 608f),
				new Vector2(33f, 636f),
				new Vector2(27f, 488f),
				new Vector2(-57f, 387f),
				new Vector2(-68f, 321f),
				new Vector2(-154f, 250f),
				new Vector2(-263f, 231f),
				new Vector2(-357f, 199f),
				new Vector2(-447f, 232f),
				new Vector2(-505f, 278f),
				new Vector2(-668f, 341f),
				new Vector2(-761f, 244f),
				new Vector2(-816f, 302f),
				new Vector2(-830f, 297f),
				new Vector2(-837f, 227f),
				new Vector2(-865f, 260f)
			}))
		},
		[817u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[21]
			{
				new Vector2(-889f, 103f),
				new Vector2(-899f, 444f),
				new Vector2(-838f, 454f),
				new Vector2(-792f, 442f),
				new Vector2(-708f, 465f),
				new Vector2(-649f, 429f),
				new Vector2(-623f, 443f),
				new Vector2(-602f, 494f),
				new Vector2(-634f, 539f),
				new Vector2(-569f, 560f),
				new Vector2(-545f, 510f),
				new Vector2(-489f, 483f),
				new Vector2(-525f, 334f),
				new Vector2(-548f, 302f),
				new Vector2(-573f, 230f),
				new Vector2(-645f, 234f),
				new Vector2(-699f, 270f),
				new Vector2(-746f, 254f),
				new Vector2(-749f, 207f),
				new Vector2(-725f, 176f),
				new Vector2(-775f, 116f)
			}))
		},
		[816u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[44]
			{
				new Vector2(269f, 873f),
				new Vector2(291f, 750f),
				new Vector2(256f, 662f),
				new Vector2(261f, 615f),
				new Vector2(254f, 520f),
				new Vector2(142f, 305f),
				new Vector2(40f, 238f),
				new Vector2(-170f, 156f),
				new Vector2(-224f, 100f),
				new Vector2(-268f, 91f),
				new Vector2(-278f, 63f),
				new Vector2(-337f, 14f),
				new Vector2(-396f, -115f),
				new Vector2(-452f, -149f),
				new Vector2(-485f, -197f),
				new Vector2(-496f, -240f),
				new Vector2(-371f, -388f),
				new Vector2(-246f, -451f),
				new Vector2(-217f, -503f),
				new Vector2(-155f, -520f),
				new Vector2(-124f, -502f),
				new Vector2(-84f, -510f),
				new Vector2(72f, -417f),
				new Vector2(116f, -245f),
				new Vector2(78f, -223f),
				new Vector2(64f, 32f),
				new Vector2(74f, 48f),
				new Vector2(166f, 72f),
				new Vector2(194f, 70f),
				new Vector2(246f, 44f),
				new Vector2(260f, -24f),
				new Vector2(283f, -44f),
				new Vector2(288f, -63f),
				new Vector2(312f, -67f),
				new Vector2(367f, 32f),
				new Vector2(349f, 82f),
				new Vector2(557f, 279f),
				new Vector2(540f, 329f),
				new Vector2(670f, 519f),
				new Vector2(804f, 579f),
				new Vector2(825f, 618f),
				new Vector2(488f, 960f),
				new Vector2(310f, 960f),
				new Vector2(332f, 908f)
			}))
		},
		[957u] = new Polygon[2]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[9]
			{
				new Vector2(-865f, 284f),
				new Vector2(-844f, 485f),
				new Vector2(-735f, 694f),
				new Vector2(-528f, 739f),
				new Vector2(-495f, 862f),
				new Vector2(-209f, 945f),
				new Vector2(-176f, 788f),
				new Vector2(-527f, 546f),
				new Vector2(-545f, 361f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[8]
			{
				new Vector2(81f, 222f),
				new Vector2(238f, 176f),
				new Vector2(219f, 103f),
				new Vector2(59f, 147f),
				new Vector2(64f, 162f),
				new Vector2(86f, 160f),
				new Vector2(93f, 193f),
				new Vector2(75f, 200f)
			}))
		},
		[1055u] = new Polygon[2]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[14]
			{
				new Vector2(663f, -2f),
				new Vector2(697f, -61f),
				new Vector2(754f, -81f),
				new Vector2(752f, -26f),
				new Vector2(772f, 14f),
				new Vector2(742f, 87f),
				new Vector2(761f, 120f),
				new Vector2(744f, 134f),
				new Vector2(729f, 137f),
				new Vector2(722f, 147f),
				new Vector2(725f, 164f),
				new Vector2(685f, 147f),
				new Vector2(687f, 103f),
				new Vector2(672f, 54f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[15]
			{
				new Vector2(-233f, -675f),
				new Vector2(-217f, -702f),
				new Vector2(-161f, -714f),
				new Vector2(-137f, -731f),
				new Vector2(-56f, -742f),
				new Vector2(55f, -717f),
				new Vector2(93f, -680f),
				new Vector2(87f, -653f),
				new Vector2(54f, -650f),
				new Vector2(13f, -650f),
				new Vector2(-6f, -655f),
				new Vector2(-30f, -672f),
				new Vector2(-64f, -692f),
				new Vector2(-138f, -650f),
				new Vector2(-171f, -658f)
			}))
		},
		[1190u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[10]
			{
				new Vector2(435f, -592f),
				new Vector2(535f, -637f),
				new Vector2(596f, -592f),
				new Vector2(587f, -378f),
				new Vector2(617f, -190f),
				new Vector2(495f, -171f),
				new Vector2(431f, -215f),
				new Vector2(389f, -337f),
				new Vector2(406f, -401f),
				new Vector2(430f, -576f)
			}))
		},
		[1187u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[5]
			{
				new Vector2(-800f, -240f),
				new Vector2(-631f, -200f),
				new Vector2(-646f, -18f),
				new Vector2(-762f, 63f),
				new Vector2(-836f, 9f)
			}))
		},
		[1188u] = new Polygon[1]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[32]
			{
				new Vector2(255f, 318f),
				new Vector2(251f, 332f),
				new Vector2(338f, 385f),
				new Vector2(359f, 405f),
				new Vector2(395f, 415f),
				new Vector2(406f, 530f),
				new Vector2(380f, 552f),
				new Vector2(321f, 586f),
				new Vector2(303f, 647f),
				new Vector2(254f, 670f),
				new Vector2(228f, 672f),
				new Vector2(193f, 664f),
				new Vector2(160f, 664f),
				new Vector2(116f, 620f),
				new Vector2(48f, 685f),
				new Vector2(35f, 699f),
				new Vector2(-39f, 716f),
				new Vector2(-77f, 684f),
				new Vector2(-84f, 650f),
				new Vector2(-130f, 635f),
				new Vector2(-144f, 583f),
				new Vector2(-105f, 509f),
				new Vector2(-41f, 480f),
				new Vector2(-15f, 504f),
				new Vector2(16f, 495f),
				new Vector2(41f, 475f),
				new Vector2(61f, 395f),
				new Vector2(99f, 402f),
				new Vector2(112f, 398f),
				new Vector2(135f, 359f),
				new Vector2(187f, 334f),
				new Vector2(206f, 335f)
			}))
		},
		[1189u] = new Polygon[2]
		{
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[10]
			{
				new Vector2(-626f, -852f),
				new Vector2(-564f, -455f),
				new Vector2(-670f, -420f),
				new Vector2(-698f, -437f),
				new Vector2(-841f, -461f),
				new Vector2(-844f, -570f),
				new Vector2(-896f, -734f),
				new Vector2(-867f, -750f),
				new Vector2(-863f, -820f),
				new Vector2(-608f, -854f)
			})),
			new Polygon(new global::_003C_003Ez__ReadOnlyArray<Vector2>(new Vector2[7]
			{
				new Vector2(240f, -887f),
				new Vector2(543f, -924f),
				new Vector2(738f, -799f),
				new Vector2(834f, -585f),
				new Vector2(483f, -606f),
				new Vector2(259f, -771f),
				new Vector2(261f, -899f)
			}))
		}
	}.ToFrozenDictionary();
}
