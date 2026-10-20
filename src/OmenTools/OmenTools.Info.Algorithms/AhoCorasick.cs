using System;
using System.Buffers;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace OmenTools.Info.Algorithms;

public class AhoCorasick
{
	private readonly record struct Node(FrozenDictionary<char, int> Transitions, int FailureState, int[] OutputIDs);

	private sealed class BuilderNode
	{
		public Dictionary<char, int> Transitions { get; } = new Dictionary<char, int>();

		public int FailureState { get; set; }

		public int[] OutputIDs { get; set; } = EmptyOutputIDs;
	}

	private static readonly FrozenDictionary<char, int> EmptyTransitions = new Dictionary<char, int>().ToFrozenDictionary();

	private static readonly int[] EmptyOutputIDs = Array.Empty<int>();

	private readonly Node[] states;

	private readonly string[] patterns;

	public AhoCorasick(IEnumerable<string> patterns)
	{
		ArgumentNullException.ThrowIfNull(patterns, "patterns");
		this.patterns = NormalizePatterns(patterns);
		states = BuildStates(this.patterns);
	}

	public bool ContainsAny(string text)
	{
		return ContainsAny(text.AsSpan());
	}

	public bool ContainsAny(ReadOnlySpan<char> text)
	{
		if (text.IsEmpty || patterns.Length == 0)
		{
			return false;
		}
		int num = 0;
		ReadOnlySpan<char> readOnlySpan = text;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = readOnlySpan[i];
			num = Advance(num, char.ToLowerInvariant(c));
			if (states[num].OutputIDs.Length != 0)
			{
				return true;
			}
		}
		return false;
	}

	public IEnumerable<string> FindAllMatches(string text)
	{
		return FindAllMatchesCore(text.AsSpan());
	}

	private string[] FindAllMatchesCore(ReadOnlySpan<char> text)
	{
		if (text.IsEmpty || patterns.Length == 0)
		{
			return Array.Empty<string>();
		}
		byte[] array = null;
		Span<byte> span = ((patterns.Length > 256) ? (array = ArrayPool<byte>.Shared.Rent(patterns.Length)).AsSpan(0, patterns.Length) : stackalloc byte[patterns.Length]);
		Span<byte> span2 = span;
		span2.Clear();
		List<string> list = new List<string>(Math.Min(patterns.Length, 8));
		int num = 0;
		try
		{
			ReadOnlySpan<char> readOnlySpan = text;
			for (int i = 0; i < readOnlySpan.Length; i++)
			{
				char c = readOnlySpan[i];
				num = Advance(num, char.ToLowerInvariant(c));
				int[] outputIDs = states[num].OutputIDs;
				if (outputIDs.Length == 0)
				{
					continue;
				}
				int[] array2 = outputIDs;
				foreach (int num2 in array2)
				{
					if (span2[num2] == 0)
					{
						span2[num2] = 1;
						list.Add(patterns[num2]);
					}
				}
			}
			return (list.Count == 0) ? Array.Empty<string>() : list.ToArray();
		}
		finally
		{
			if (array != null)
			{
				ArrayPool<byte>.Shared.Return(array, clearArray: true);
			}
		}
	}

	private static string[] NormalizePatterns(IEnumerable<string> source)
	{
		List<string> list = new List<string>();
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		foreach (string item in source)
		{
			if (!string.IsNullOrEmpty(item))
			{
				string text = item.ToLowerInvariant();
				if (text.Length != 0 && hashSet.Add(text))
				{
					list.Add(text);
				}
			}
		}
		return list.ToArray();
	}

	private static Node[] BuildStates(string[] patterns)
	{
		int num = 1;
		List<BuilderNode> list = new List<BuilderNode>(num);
		CollectionsMarshal.SetCount(list, num);
		CollectionsMarshal.AsSpan(list)[0] = new BuilderNode();
		List<BuilderNode> list2 = list;
		for (int i = 0; i < patterns.Length; i++)
		{
			Insert(list2, patterns[i], i);
		}
		BuildFailureLinks(list2);
		Node[] array = new Node[list2.Count];
		for (int j = 0; j < list2.Count; j++)
		{
			BuilderNode builderNode = list2[j];
			FrozenDictionary<char, int> transitions = ((builderNode.Transitions.Count == 0) ? EmptyTransitions : builderNode.Transitions.ToFrozenDictionary());
			array[j] = new Node(transitions, builderNode.FailureState, builderNode.OutputIDs);
		}
		return array;
	}

	private static void Insert(List<BuilderNode> states, string pattern, int patternID)
	{
		int index = 0;
		foreach (char key in pattern)
		{
			bool exists;
			ref int valueRefOrAddDefault = ref CollectionsMarshal.GetValueRefOrAddDefault(states[index].Transitions, key, out exists);
			if (!exists)
			{
				valueRefOrAddDefault = states.Count;
				states.Add(new BuilderNode());
			}
			index = valueRefOrAddDefault;
		}
		states[index].OutputIDs = new int[1] { patternID };
	}

	private static void BuildFailureLinks(List<BuilderNode> states)
	{
		if (states.Count == 1)
		{
			return;
		}
		Queue<int> queue = new Queue<int>(states.Count - 1);
		foreach (int value2 in states[0].Transitions.Values)
		{
			states[value2].FailureState = 0;
			queue.Enqueue(value2);
		}
		while (queue.Count > 0)
		{
			int index = queue.Dequeue();
			BuilderNode builderNode = states[index];
			foreach (KeyValuePair<char, int> transition in builderNode.Transitions)
			{
				transition.Deconstruct(out var key, out var value);
				char key2 = key;
				int num = value;
				int failureState = builderNode.FailureState;
				while (failureState != 0 && !states[failureState].Transitions.TryGetValue(key2, out value))
				{
					failureState = states[failureState].FailureState;
				}
				states[num].FailureState = states[failureState].Transitions.GetValueOrDefault(key2, 0);
				int[] outputIDs = states[states[num].FailureState].OutputIDs;
				if (outputIDs.Length != 0)
				{
					states[num].OutputIDs = MergeSortedUnique(states[num].OutputIDs, outputIDs);
				}
				queue.Enqueue(num);
			}
		}
	}

	private static int[] MergeSortedUnique(int[] current, int[] fallback)
	{
		if (current.Length == 0)
		{
			return fallback;
		}
		if (fallback.Length == 0)
		{
			return current;
		}
		int[] array = new int[current.Length + fallback.Length];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		bool flag = false;
		int num4 = 0;
		while (num < current.Length || num2 < fallback.Length)
		{
			int num5;
			if (num2 >= fallback.Length || (num < current.Length && current[num] < fallback[num2]))
			{
				num5 = current[num];
				num++;
			}
			else if (num >= current.Length || fallback[num2] < current[num])
			{
				num5 = fallback[num2];
				num2++;
			}
			else
			{
				num5 = current[num];
				num++;
				num2++;
			}
			if (!flag || num5 != num4)
			{
				array[num3] = num5;
				num4 = num5;
				flag = true;
				num3++;
			}
		}
		if (num3 != array.Length)
		{
			return array[..num3];
		}
		return array;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Advance(int state, char c)
	{
		while (true)
		{
			if (states[state].Transitions.TryGetValue(c, out var value))
			{
				return value;
			}
			if (state == 0)
			{
				break;
			}
			state = states[state].FailureState;
		}
		return 0;
	}
}
