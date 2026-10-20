using System;
using Dalamud.Interface.Style;

namespace OmenTools.Info.Dalamud;

public struct StyleInfo(string name, string data)
{
	private class StyleModelWrapper(StyleModel styleModel) : IDisposable
	{
		public void Dispose()
		{
			styleModel.Pop();
		}
	}

	public string Name = name;

	public string Data = data;

	private readonly StyleModel built = StyleModel.Deserialize(data);

	private readonly StyleModelWrapper wrapper = new StyleModelWrapper(built);

	public IDisposable Push()
	{
		built.Push();
		return wrapper;
	}
}
