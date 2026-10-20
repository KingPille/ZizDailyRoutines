using System;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.System.Resource;
using FFXIVClientStructs.FFXIV.Client.System.Resource.Handle;
using InteropGenerator.Runtime;
using OmenTools.Dalamud;
using OmenTools.Extensions;
using OmenTools.OmenService.Abstractions;

namespace OmenTools.OmenService;

public class GameResourceManager : OmenServiceBase<GameResourceManager>
{
	public class GameResourceManagerConfig : OmenServiceConfig
	{
		public bool ShowGameResourceManagerLog;

		public string GameResourceManagerKeyword = string.Empty;

		public void Save()
		{
			this.Save(OmenServiceBase<GameResourceManager>.Instance());
		}
	}

	private class CRC32
	{
		private const uint POLY = 3988292384u;

		private static readonly uint[] Table = CreateSlicingBy8Tables();

		private uint crc32 = uint.MaxValue;

		public uint Checksum => ~crc32;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Init()
		{
			crc32 = uint.MaxValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
		public void Update(ReadOnlySpan<byte> data)
		{
			uint num = crc32;
			if (data.Length >= 8)
			{
				ref byte reference = ref MemoryMarshal.GetReference(data);
				nint num2 = 0;
				nint num3 = data.Length;
				for (nint num4 = num3 - 8; num2 <= num4; num2 += 8)
				{
					ulong num5 = Unsafe.ReadUnaligned<ulong>(in Unsafe.Add(ref reference, num2));
					if (!BitConverter.IsLittleEndian)
					{
						num5 = BinaryPrimitives.ReverseEndianness(num5);
					}
					uint num6 = (uint)(int)num5 ^ num;
					uint num7 = (uint)(num5 >> 32);
					num = Table[1792 + (byte)(num7 >> 24)] ^ Table[1536 + (byte)(num7 >> 16)] ^ Table[1280 + (byte)(num7 >> 8)] ^ Table[1024 + (byte)num7] ^ Table[768 + (byte)(num6 >> 24)] ^ Table[512 + (byte)(num6 >> 16)] ^ Table[256 + (byte)(num6 >> 8)] ^ Table[(byte)num6];
				}
				for (; num2 < num3; num2++)
				{
					byte b = Unsafe.Add(ref reference, num2);
					num = Table[(num ^ b) & 0xFF] ^ (num >> 8);
				}
				crc32 = num;
			}
			else
			{
				ReadOnlySpan<byte> readOnlySpan = data;
				for (int i = 0; i < readOnlySpan.Length; i++)
				{
					byte b2 = readOnlySpan[i];
					num = Table[(num ^ b2) & 0xFF] ^ (num >> 8);
				}
				crc32 = num;
			}
		}

		private static uint[] CreateSlicingBy8Tables()
		{
			uint[] array = new uint[2048];
			for (uint num = 0u; num < 256; num++)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					num2 = (((num2 & 1) != 0) ? ((num2 >> 1) ^ 0xEDB88320u) : (num2 >> 1));
				}
				array[num] = num2;
			}
			for (int j = 1; j < 8; j++)
			{
				int num3 = (j - 1) * 256;
				int num4 = j * 256;
				for (int k = 0; k < 256; k++)
				{
					uint num5 = array[num3 + k];
					array[num4 + k] = array[num5 & 0xFF] ^ (num5 >> 8);
				}
			}
			return array;
		}
	}

	private Hook<GetResourceSync>? GetResourceSyncHook;

	private Hook<GetResourceAsync>? GetResourceAsyncHook;

	private readonly CRC32 hashGenerator = new CRC32();

	private readonly ConcurrentDictionary<Type, ImmutableList<string>> registeredPaths = new ConcurrentDictionary<Type, ImmutableList<string>>();

	private ConcurrentDictionary<string, byte> blacklistedPaths = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

	public GameResourceManagerConfig Config { get; private set; }

	public void AddToBlacklist(Type source, params string[] paths)
	{
		registeredPaths.AddOrUpdate(source, (Type _) => ImmutableList.CreateRange(paths), (Type _, ImmutableList<string> currentList) => currentList.AddRange(paths));
		RegeneratePaths();
	}

	public void AddToBlacklist(Type source, IEnumerable<string> paths)
	{
		string[] items = (paths as string[]) ?? paths.ToArray();
		if (items.Length != 0)
		{
			registeredPaths.AddOrUpdate(source, (Type _) => ImmutableList.CreateRange(items), (Type _, ImmutableList<string> currentList) => currentList.AddRange(items));
			RegeneratePaths();
		}
	}

	public void RemoveFromBlacklist(Type source, params string[] paths)
	{
		if (paths == null || paths.Length <= 0)
		{
			return;
		}
		ImmutableList<string> value;
		while (registeredPaths.TryGetValue(source, out value))
		{
			ImmutableList<string> immutableList = value.RemoveRange(paths);
			if (immutableList == value)
			{
				return;
			}
			if (immutableList.IsEmpty)
			{
				KeyValuePair<Type, ImmutableList<string>> item = new KeyValuePair<Type, ImmutableList<string>>(source, value);
				if (((ICollection<KeyValuePair<Type, ImmutableList<string>>>)registeredPaths).Remove(item))
				{
					return;
				}
			}
			else if (registeredPaths.TryUpdate(source, immutableList, value))
			{
				return;
			}
		}
		RegeneratePaths();
	}

	public void RemoveFromBlacklist(Type source, IEnumerable<string> paths)
	{
		string[] array = (paths as string[]) ?? paths.ToArray();
		if (array.Length == 0)
		{
			return;
		}
		ImmutableList<string> value;
		while (registeredPaths.TryGetValue(source, out value))
		{
			ImmutableList<string> immutableList = value.RemoveRange(array);
			if (immutableList == value)
			{
				return;
			}
			if (immutableList.IsEmpty)
			{
				KeyValuePair<Type, ImmutableList<string>> item = new KeyValuePair<Type, ImmutableList<string>>(source, value);
				if (((ICollection<KeyValuePair<Type, ImmutableList<string>>>)registeredPaths).Remove(item))
				{
					break;
				}
			}
			else if (registeredPaths.TryUpdate(source, immutableList, value))
			{
				break;
			}
		}
		RegeneratePaths();
	}

	protected unsafe override void Init()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		Config = LoadConfig<GameResourceManagerConfig>() ?? new GameResourceManagerConfig();
		if (GetResourceSyncHook == null)
		{
			GetResourceSyncHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<GetResourceSync>(typeof(MemberFunctionPointers), "GetResourceSync", new GetResourceSync(GetResourceSyncDetour));
		}
		if (GetResourceAsyncHook == null)
		{
			GetResourceAsyncHook = DalamudServiceExtension.Instance<IGameInteropProvider>().HookFromMemberFunction<GetResourceAsync>(typeof(MemberFunctionPointers), "GetResourceAsync", new GetResourceAsync(GetResourceAsyncDetour));
		}
		OmenServiceBase<GameState>.Instance().Login += OnLogin;
		OmenServiceBase<GameState>.Instance().Logout += OnLogout;
		if (GameState.IsLoggedIn)
		{
			Toggle(isEnabled: true);
		}
	}

	protected override void Uninit()
	{
		OmenServiceBase<GameState>.Instance().Login -= OnLogin;
		OmenServiceBase<GameState>.Instance().Logout -= OnLogout;
		Toggle(isEnabled: false);
		GetResourceSyncHook?.Dispose();
		GetResourceSyncHook = null;
		GetResourceAsyncHook?.Dispose();
		GetResourceAsyncHook = null;
		blacklistedPaths.Clear();
		registeredPaths.Clear();
	}

	private void OnLogin()
	{
		Toggle(isEnabled: true);
	}

	private void OnLogout()
	{
		Toggle(isEnabled: false);
	}

	private unsafe ResourceHandle* GetResourceSyncDetour(ResourceManager* manager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path, void* unknown, void* unkDebugPtr, uint unkDebugInt)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetResourceHandler(isSync: true, manager, category, type, hash, path, unknown, isUnknown: false, unkDebugPtr, unkDebugInt);
	}

	private unsafe ResourceHandle* GetResourceAsyncDetour(ResourceManager* manager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path, void* unknown, bool isUnknown, void* unkDebugPtr, uint unkDebugInt)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return GetResourceHandler(isSync: false, manager, category, type, hash, path, unknown, isUnknown, unkDebugPtr, unkDebugInt);
	}

	private unsafe ResourceHandle* GetResourceHandler(bool isSync, ResourceManager* manager, ResourceCategory* category, uint* type, uint* hash, CStringPointer path, void* unknown, bool isUnknown, void* unkDebugPtr, uint unkDebugInt)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		if (!((CStringPointer)(ref path)).HasValue)
		{
			return InvokeOriginal();
		}
		string text = ((object)(*(CStringPointer*)(&path))/*cast due to constrained. prefix*/).ToString().Trim();
		if (string.IsNullOrEmpty(text))
		{
			return InvokeOriginal();
		}
		if (Config.ShowGameResourceManagerLog && (string.IsNullOrWhiteSpace(Config.GameResourceManagerKeyword) || text.Contains(Config.GameResourceManagerKeyword)))
		{
			DLog.Debug("[Game Resource Manager]\n资源路径:" + text);
		}
		text = text.ToLowerInvariant();
		if (blacklistedPaths.ContainsKey(text))
		{
			ReadOnlySpan<byte> data = "vfx/path/nothing.avfx"u8;
			Span<byte> destination = stackalloc byte[data.Length + 1];
			data.CopyTo(destination);
			path = CStringPointer.op_Implicit((byte*)Unsafe.AsPointer(in destination.GetPinnableReference()));
			hashGenerator.Init();
			hashGenerator.Update(data);
			*hash = hashGenerator.Checksum;
		}
		return InvokeOriginal();
		unsafe ResourceHandle* InvokeOriginal()
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (!isSync)
			{
				return GetResourceAsyncHook.Original.Invoke(manager, category, type, hash, path, unknown, isUnknown, unkDebugPtr, unkDebugInt);
			}
			return GetResourceSyncHook.Original.Invoke(manager, category, type, hash, path, unknown, unkDebugPtr, unkDebugInt);
		}
	}

	private void Toggle(bool isEnabled)
	{
		GetResourceSyncHook?.Toggle<GetResourceSync>((bool?)isEnabled);
		GetResourceAsyncHook?.Toggle<GetResourceAsync>((bool?)isEnabled);
	}

	private void RegeneratePaths()
	{
		blacklistedPaths = (from x in registeredPaths.SelectMany<KeyValuePair<Type, ImmutableList<string>>, string>((KeyValuePair<Type, ImmutableList<string>> x) => x.Value)
			select x.Trim().ToLowerInvariant()).ToConcurrentDictionary((Func<string, string>)((string x) => x), (Func<string, byte>)((string _) => 0));
	}
}
