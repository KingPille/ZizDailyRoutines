using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using B2nED1Wmdu0xsDR7Tjv;
using TOm8ovfpYOVGJGuvn7b;

namespace ACnTQP9K7F3atyiqaA4;

internal sealed class tkGYLK9Ox1OpXu2BoEN
{
	public Dictionary<string, dvY7ShfxMmccDlr70u8> Sections { get; }

	public tkGYLK9Ox1OpXu2BoEN()
	{
		n8FuMZWBsAV4JCaJjUC.FJgTgZCr7W();
		base._002Ector();
		Sections = Re99WxrZuJ<dvY7ShfxMmccDlr70u8>().OrderBy<dvY7ShfxMmccDlr70u8, string>((dvY7ShfxMmccDlr70u8 section) => section.SectionID, StringComparer.Ordinal).ToDictionary((dvY7ShfxMmccDlr70u8 section) => section.SectionID, (dvY7ShfxMmccDlr70u8 section) => section);
	}

	public dvY7ShfxMmccDlr70u8? iJ49tMcdcM(string sectionID)
	{
		return Sections.GetValueOrDefault(sectionID);
	}

	private static List<T> Re99WxrZuJ<T>() where T : class
	{
		return (from type in Assembly.GetExecutingAssembly().GetTypes()
			where (object)type != null && !type.IsAbstract && !type.IsInterface && typeof(T).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null
			select type).Select(Activator.CreateInstance).OfType<T>().ToList();
	}
}
