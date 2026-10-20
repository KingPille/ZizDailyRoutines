using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using OmenTools.Dalamud;
using OmenTools.Extensions;

namespace DailyRoutines.Common.Module.Compilers;

public sealed class ModuleCompiler
{
	private static readonly CSharpCompilationOptions CompilationOptions;

	private static readonly CSharpParseOptions ParseOptions;

	private readonly ConcurrentDictionary<string, Lazy<byte[]>> compiledAssemblyCache = new ConcurrentDictionary<string, Lazy<byte[]>>(StringComparer.Ordinal);

	private readonly ConcurrentDictionary<string, Lazy<Assembly?>> loadedAssemblyCache = new ConcurrentDictionary<string, Lazy<Assembly>>(StringComparer.Ordinal);

	private readonly ConcurrentBag<ModuleLoadContext> loadedContexts = new ConcurrentBag<ModuleLoadContext>();

	private readonly Lazy<ImmutableArray<MetadataReference>> referenceListLazy = new Lazy<ImmutableArray<MetadataReference>>(BuildReferenceList, LazyThreadSafetyMode.ExecutionAndPublication);

	private ImmutableArray<MetadataReference> ReferenceList => referenceListLazy.Value;

	public void UnloadAll()
	{
		long timestamp = Stopwatch.GetTimestamp();
		int count = compiledAssemblyCache.Count;
		int count2 = loadedAssemblyCache.Count;
		int count3 = loadedContexts.Count;
		DLog.Debug($"[ModuleCompiler] 开始清理编译器缓存, 已编译缓存: {count}, 已加载缓存: {count2}, 加载上下文: {count3}");
		compiledAssemblyCache.Clear();
		loadedAssemblyCache.Clear();
		int num = 0;
		ModuleLoadContext result;
		while (loadedContexts.TryTake(out result))
		{
			num++;
			TryUnloadContext(result);
		}
		DLog.Debug($"[ModuleCompiler] 编译器缓存清理完成, 已卸载上下文: {num}, 耗时: {Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F2} ms");
	}

	public Assembly? Load(string code)
	{
		if (string.IsNullOrWhiteSpace(code))
		{
			DLog.Debug("[ModuleCompiler] 接收到空白源码, 跳过模块加载");
			return null;
		}
		DLog.Debug($"[ModuleCompiler] 接收到模块源码加载请求, 源码长度: {code.Length} 字符");
		byte[] array = Compile(code);
		if (array.Length == 0)
		{
			DLog.Warning("[ModuleCompiler] 编译结果为空, 未执行程序集加载");
			return null;
		}
		if (array.Length == 0)
		{
			return null;
		}
		return Load(array);
	}

	public Assembly? Load(byte[] assembly)
	{
		if (assembly.Length == 0)
		{
			DLog.Debug("[ModuleCompiler] 接收到空程序集字节数组, 跳过加载");
			return null;
		}
		string assemblyHash = ComputeSHA256Hash(assembly);
		string text = FormatHash(assemblyHash);
		Lazy<Assembly> lazy = new Lazy<Assembly>(() => LoadCore(assembly, assemblyHash), LazyThreadSafetyMode.ExecutionAndPublication);
		Lazy<Assembly> orAdd = loadedAssemblyCache.GetOrAdd(assemblyHash, lazy);
		DLog.Debug((orAdd == lazy) ? $"[ModuleCompiler] 未命中程序集加载缓存, 准备创建新的加载任务, 哈希: {text}, 大小: {assembly.Length} 字节" : $"[ModuleCompiler] 命中程序集加载缓存, 哈希: {text}, 大小: {assembly.Length} 字节");
		try
		{
			Assembly value = orAdd.Value;
			if (value != null)
			{
				DLog.Debug("[ModuleCompiler] 程序集加载完成, 哈希: " + text + ", 程序集: " + (value.FullName ?? "<未知>"));
				return value;
			}
			DLog.Warning("[ModuleCompiler] 程序集加载结果为空, 准备移除缓存项, 哈希: " + text);
		}
		catch (Exception ex)
		{
			DLog.Error("[ModuleCompiler] 读取程序集加载缓存时发生异常, 哈希: " + text, ex);
		}
		loadedAssemblyCache.TryRemove(assemblyHash, out Lazy<Assembly> _);
		DLog.Debug("[ModuleCompiler] 已移除失效的程序集加载缓存项, 哈希: " + text);
		return null;
	}

	public static IReadOnlyList<Type> GetDerivedTypes<T>(Assembly? assembly) where T : class
	{
		if (assembly == null)
		{
			DLog.Debug("[ModuleCompiler] 程序集为空, 无法查找派生自 " + typeof(T).FullName + " 的类型");
			return Array.Empty<Type>();
		}
		Type typeFromHandle = typeof(T);
		List<Type> list = new List<Type>();
		DLog.Debug("[ModuleCompiler] 开始扫描派生类型, 目标基类: " + typeFromHandle.FullName + ", 程序集: " + (assembly.FullName ?? "<未知>"));
		try
		{
			Type[] types = assembly.GetTypes();
			foreach (Type type in types)
			{
				if (!type.IsAbstract && typeFromHandle.IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
				{
					list.Add(type);
				}
			}
			DLog.Debug($"[ModuleCompiler] 派生类型扫描完成, 目标基类: {typeFromHandle.FullName}, 匹配数量: {list.Count}");
		}
		catch (ReflectionTypeLoadException ex)
		{
			Type[] types = ex.Types;
			foreach (Type type2 in types)
			{
				if (!(type2 == null) && !type2.IsAbstract && typeFromHandle.IsAssignableFrom(type2) && !(type2.GetConstructor(Type.EmptyTypes) == null))
				{
					list.Add(type2);
				}
			}
			DLog.Error("[ModuleCompiler] 解析模块类型时发生部分加载失败", ex);
			DLog.Warning($"[ModuleCompiler] 已从部分可用类型中恢复结果, 目标基类: {typeFromHandle.FullName}, 匹配数量: {list.Count}, 加载器异常数量: {ex.LoaderExceptions.Length}");
		}
		return list;
	}

	private byte[] Compile(string sourceCode)
	{
		string codeHash = ComputeSHA256Hash(sourceCode);
		string text = FormatHash(codeHash);
		Lazy<byte[]> lazy = new Lazy<byte[]>(() => CompileCore(sourceCode, codeHash), LazyThreadSafetyMode.ExecutionAndPublication);
		Lazy<byte[]> orAdd = compiledAssemblyCache.GetOrAdd(codeHash, lazy);
		DLog.Debug((orAdd == lazy) ? $"[ModuleCompiler] 未命中编译缓存, 开始创建新的编译任务, 源码哈希: {text}, 源码长度: {sourceCode.Length} 字符" : $"[ModuleCompiler] 命中编译缓存, 源码哈希: {text}, 源码长度: {sourceCode.Length} 字符");
		try
		{
			byte[] value = orAdd.Value;
			DLog.Debug($"[ModuleCompiler] 编译结果已就绪, 源码哈希: {text}, 程序集大小: {value.Length} 字节");
			return value;
		}
		catch (Exception ex)
		{
			compiledAssemblyCache.TryRemove(codeHash, out Lazy<byte[]> _);
			DLog.Error("[ModuleCompiler] 读取编译缓存时发生异常, 已移除缓存项, 源码哈希: " + text, ex);
			throw;
		}
	}

	private byte[] CompileCore(string sourceCode, string codeHash)
	{
		long timestamp = Stopwatch.GetTimestamp();
		string text = FormatHash(codeHash);
		DLog.Debug("[ModuleCompiler] 开始执行模块编译, 源码哈希: " + text);
		using MemoryStream memoryStream = new MemoryStream();
		EmitResult emitResult = CreateCompilation(sourceCode, codeHash).Emit(memoryStream);
		if (!emitResult.Success)
		{
			ThrowCompilationException(emitResult, codeHash);
		}
		byte[] array = memoryStream.ToArray();
		DLog.Debug($"[ModuleCompiler] 模块编译成功, 源码哈希: {text}, 程序集大小: {array.Length} 字节, 耗时: {Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F2} ms");
		return array;
	}

	private CSharpCompilation CreateCompilation(string sourceCode, string codeHash)
	{
		string text = $"DailyRoutinesModule-{Guid.NewGuid():N}";
		SyntaxTree item = SyntaxFactory.ParseSyntaxTree(SourceText.From(sourceCode, Encoding.UTF8), ParseOptions);
		ImmutableArray<MetadataReference> referenceList = ReferenceList;
		DLog.Verbose($"[ModuleCompiler] 正在创建 Roslyn 编译实例, 源码哈希: {FormatHash(codeHash)}, 动态程序集名: {text}, 引用数量: {referenceList.Length}");
		return CSharpCompilation.Create(text, new global::_003C_003Ez__ReadOnlySingleElementList<SyntaxTree>(item), referenceList, CompilationOptions);
	}

	private static void ThrowCompilationException(EmitResult emitResult, string codeHash)
	{
		Diagnostic[] array = emitResult.Diagnostics.Where((Diagnostic diagnostic) => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error).ToArray();
		DLog.Error($"[ModuleCompiler] 模块编译失败, 源码哈希: {FormatHash(codeHash)}, 错误数量: {array.Length}");
		for (int num = 0; num < array.Length; num++)
		{
			DLog.Error($"[ModuleCompiler] 编译诊断 {num + 1}/{array.Length}: {FormatDiagnostic(array[num])}");
		}
		throw new InvalidOperationException($"模块编译失败, 共 {array.Length} 个错误");
	}

	private Assembly? LoadCore(byte[] assembly, string assemblyHash)
	{
		long timestamp = Stopwatch.GetTimestamp();
		AssemblyLoadContext loadContext = AssemblyLoadContext.GetLoadContext(typeof(ModuleCompiler).Assembly);
		string text = $"DailyRoutinesModule-{Guid.NewGuid():N}";
		if (loadContext == null)
		{
			DLog.Error("[ModuleCompiler] 无法获取父程序集加载上下文, 程序集哈希: " + FormatHash(assemblyHash));
			return null;
		}
		using MemoryStream assembly2 = new MemoryStream(assembly, writable: false);
		ModuleLoadContext moduleLoadContext = new ModuleLoadContext(loadContext, text);
		DLog.Debug($"[ModuleCompiler] 开始加载动态程序集, 哈希: {FormatHash(assemblyHash)}, 上下文: {text}, 大小: {assembly.Length} 字节");
		try
		{
			Assembly assembly3 = moduleLoadContext.LoadFromStream(assembly2);
			loadedContexts.Add(moduleLoadContext);
			DLog.Debug($"[ModuleCompiler] 动态程序集加载成功, 哈希: {FormatHash(assemblyHash)}, 程序集: {assembly3.FullName ?? "<未知>"}, 上下文: {text}, 耗时: {Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F2} ms");
			return assembly3;
		}
		catch (Exception ex)
		{
			TryUnloadContext(moduleLoadContext);
			DLog.Error("[ModuleCompiler] 加载动态程序集时发生异常, 哈希: " + FormatHash(assemblyHash) + ", 上下文: " + text, ex);
			return null;
		}
	}

	private static ImmutableArray<MetadataReference> BuildReferenceList()
	{
		long timestamp = Stopwatch.GetTimestamp();
		ImmutableArray<MetadataReference>.Builder builder = ImmutableArray.CreateBuilder<MetadataReference>();
		HashSet<string> seenFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		DLog.Debug("[ModuleCompiler] 开始构建 Roslyn 引用列表");
		AddReferences(builder, seenFileNames, Path.GetDirectoryName(typeof(object).Assembly.Location), "系统基础程序集目录", SearchOption.TopDirectoryOnly);
		AddReferences(builder, seenFileNames, Path.GetDirectoryName(typeof(Form).Assembly.Location), "Windows Forms 程序集目录", SearchOption.TopDirectoryOnly);
		AddReferences(builder, seenFileNames, DalamudServiceExtension.Instance().AssemblyLocation.DirectoryName, "插件程序集目录", SearchOption.AllDirectories);
		AddReferences(builder, seenFileNames, Path.GetDirectoryName(((object)DalamudServiceExtension.Instance()).GetType().Assembly.Location), "插件运行时类型目录", SearchOption.AllDirectories);
		DLog.Debug($"[ModuleCompiler] Roslyn 引用列表构建完成, 引用数量: {builder.Count}, 耗时: {Stopwatch.GetElapsedTime(timestamp).TotalMilliseconds:F2} ms");
		return builder.ToImmutable();
	}

	private static void AddReferences(ImmutableArray<MetadataReference>.Builder builder, HashSet<string> seenFileNames, string? directory, string sourceName, SearchOption searchOption)
	{
		if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
		{
			DLog.Warning("[ModuleCompiler] 引用目录不可用, 来源: " + sourceName + ", 目录: " + (directory ?? "<空>"));
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		foreach (string item in EnumerateAssemblyFiles(directory, searchOption))
		{
			num++;
			string fileName = Path.GetFileName(item);
			if (!seenFileNames.Add(fileName))
			{
				num2++;
				continue;
			}
			if (!IsValidAssembly(item))
			{
				num3++;
				continue;
			}
			builder.Add(MetadataReference.CreateFromFile(item));
			num4++;
		}
		DLog.Debug($"[ModuleCompiler] 引用目录扫描完成, 来源: {sourceName}, 目录: {directory}, 发现文件: {num}, 新增引用: {num4}, 重复文件名: {num2}, 无效程序集: {num3}");
	}

	private static IEnumerable<string> EnumerateAssemblyFiles(string directory, SearchOption searchOption)
	{
		foreach (string item in Directory.EnumerateFiles(directory, "*.dll", searchOption))
		{
			yield return item;
		}
		foreach (string item2 in Directory.EnumerateFiles(directory, "*.exe", searchOption))
		{
			yield return item2;
		}
	}

	private static bool IsValidAssembly(string path)
	{
		try
		{
			AssemblyName.GetAssemblyName(path);
			return true;
		}
		catch (BadImageFormatException)
		{
			return false;
		}
		catch (FileLoadException)
		{
			return false;
		}
		catch (IOException)
		{
			return false;
		}
		catch (UnauthorizedAccessException)
		{
			return false;
		}
	}

	private static string ComputeSHA256Hash(string data)
	{
		return ComputeSHA256Hash(Encoding.UTF8.GetBytes(data));
	}

	private static string ComputeSHA256Hash(byte[] data)
	{
		return ComputeSHA256Hash(data.AsSpan());
	}

	private static string ComputeSHA256Hash(ReadOnlySpan<byte> data)
	{
		return Convert.ToHexString(SHA256.HashData(data));
	}

	private static string FormatHash(string hash)
	{
		if (hash.Length > 12)
		{
			return hash.Substring(0, 12);
		}
		return hash;
	}

	private static string FormatDiagnostic(Diagnostic diagnostic)
	{
		FileLinePositionSpan lineSpan = diagnostic.Location.GetLineSpan();
		if (!lineSpan.IsValid)
		{
			return diagnostic.Id + ", 消息: " + diagnostic.GetMessage();
		}
		int value = lineSpan.StartLinePosition.Line + 1;
		int value2 = lineSpan.StartLinePosition.Character + 1;
		return $"{diagnostic.Id}, 位置: 第 {value} 行, 第 {value2} 列, 消息: {diagnostic.GetMessage()}";
	}

	private static void TryUnloadContext(ModuleLoadContext context)
	{
		string text = context.Name ?? "<未命名>";
		DLog.Verbose("[ModuleCompiler] 开始卸载动态程序集上下文: " + text);
		try
		{
			context.Unload();
			DLog.Verbose("[ModuleCompiler] 已发出动态程序集上下文卸载请求: " + text);
		}
		catch (Exception ex)
		{
			DLog.Error("[ModuleCompiler] 卸载动态程序集上下文时发生异常, 上下文: " + text, ex);
		}
	}

	static ModuleCompiler()
	{
		AssemblyIdentityComparer assemblyIdentityComparer = DesktopAssemblyIdentityComparer.Default;
		CompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, reportSuppressedDiagnostics: false, null, null, null, null, OptimizationLevel.Release, checkOverflow: false, allowUnsafe: true, null, null, default(ImmutableArray<byte>), null, Platform.AnyCpu, ReportDiagnostic.Default, 4, null, concurrentBuild: true, deterministic: false, null, null, null, assemblyIdentityComparer);
		ParseOptions = CSharpParseOptions.Default;
	}
}
