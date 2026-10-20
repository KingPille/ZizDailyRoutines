using System.Threading;
using System.Threading.Tasks;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;

namespace OmenTools.Dalamud;

public static class EdgeTTSIPC
{
	public const string INTERNAL_NAME = "EdgeTTS.Dalamud";

	[IPCSubscriber("EdgeTTS.Speak")]
	private static IPCSubscriber<string, object>? SpeakSubscriber;

	[IPCSubscriber("EdgeTTS.SpeakWithOptions")]
	private static IPCSubscriber<string, int?, int?, int?, object>? SpeakWithOptionsSubscriber;

	[IPCSubscriber("EdgeTTS.SpeakAsync")]
	private static IPCSubscriber<string, CancellationToken, Task>? SpeakAsyncSubscriber;

	[IPCSubscriber("EdgeTTS.SpeakWithOptionsAsync")]
	private static IPCSubscriber<string, int?, int?, int?, CancellationToken, Task>? SpeakWithOptionsAsyncSubscriber;

	[IPCSubscriber("EdgeTTS.Synthesize")]
	private static IPCSubscriber<string, object>? SynthesizeSubscriber;

	[IPCSubscriber("EdgeTTS.SynthesizeWithOptions")]
	private static IPCSubscriber<string, int?, int?, int?, object>? SynthesizeWithOptionsSubscriber;

	[IPCSubscriber("EdgeTTS.SynthesizeAsync")]
	private static IPCSubscriber<string, CancellationToken, Task>? SynthesizeAsyncSubscriber;

	[IPCSubscriber("EdgeTTS.SynthesizeWithOptionsAsync")]
	private static IPCSubscriber<string, int?, int?, int?, CancellationToken, Task>? SynthesizeWithOptionsAsyncSubscriber;

	public static bool IsPluginEnabled()
	{
		return DalamudServiceExtension.Instance().IsPluginEnabled("EdgeTTS.Dalamud");
	}

	public static void Speak(string text)
	{
		SpeakSubscriber?.InvokeAction(text);
	}

	public static void Speak(string text, int? speed = null, int? pitch = null, int? volume = null)
	{
		SpeakWithOptionsSubscriber?.InvokeAction(text, speed, pitch, volume);
	}

	public static Task SpeakAsync(string text, CancellationToken cancellationToken)
	{
		return SpeakAsyncSubscriber?.InvokeFunc(text, cancellationToken) ?? Task.CompletedTask;
	}

	public static Task SpeakAsync(string text, int? speed = null, int? pitch = null, int? volume = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return SpeakWithOptionsAsyncSubscriber?.InvokeFunc(text, speed, pitch, volume, cancellationToken) ?? Task.CompletedTask;
	}

	public static void Synthesize(string text)
	{
		SynthesizeSubscriber?.InvokeAction(text);
	}

	public static void Synthesize(string text, int? speed = null, int? pitch = null, int? volume = null)
	{
		SynthesizeWithOptionsSubscriber?.InvokeAction(text, speed, pitch, volume);
	}

	public static Task SynthesizeAsync(string text, CancellationToken cancellationToken)
	{
		return SynthesizeAsyncSubscriber?.InvokeFunc(text, cancellationToken) ?? Task.CompletedTask;
	}

	public static Task SynthesizeAsync(string text, int? speed = null, int? pitch = null, int? volume = null, CancellationToken cancellationToken = default(CancellationToken))
	{
		return SynthesizeWithOptionsAsyncSubscriber?.InvokeFunc(text, speed, pitch, volume, cancellationToken) ?? Task.CompletedTask;
	}
}
