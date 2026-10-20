using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Dalamud.Plugin;
using Dalamud.Plugin.Ipc;
using OmenTools.Dalamud.Abstractions;
using OmenTools.Dalamud.Attributes;
using OmenTools.Extensions;

namespace OmenTools.Dalamud;

public static class IPCAttributeRegistry
{
	private const string REGISTER_ACTION_METHOD_NAME = "RegisterAction";

	private const string REGISTER_FUNC_METHOD_NAME = "RegisterFunc";

	private static readonly ConditionalWeakTable<object, List<ICallGateProvider>> RegisteredProviders = new ConditionalWeakTable<object, List<ICallGateProvider>>();

	private static readonly Dictionary<int, MethodInfo> GetIPCProviderMethods = (from m in typeof(IDalamudPluginInterface).GetMethods(BindingFlags.Instance | BindingFlags.Public)
		where (object)m != null && m.Name == "GetIpcProvider" && m.IsGenericMethodDefinition
		group m by m.GetGenericArguments().Length).ToDictionary((IGrouping<int, MethodInfo> group) => group.Key, (IGrouping<int, MethodInfo> group) => group.First());

	private static readonly Type[] FuncDelegateTypes = new Type[9]
	{
		typeof(Func<>),
		typeof(Func<, >),
		typeof(Func<, , >),
		typeof(Func<, , , >),
		typeof(Func<, , , , >),
		typeof(Func<, , , , , >),
		typeof(Func<, , , , , , >),
		typeof(Func<, , , , , , , >),
		typeof(Func<, , , , , , , , >)
	};

	private static readonly Type[] ActionDelegateTypes = new Type[10]
	{
		typeof(Action),
		typeof(Action<>),
		typeof(Action<, >),
		typeof(Action<, , >),
		typeof(Action<, , , >),
		typeof(Action<, , , , >),
		typeof(Action<, , , , , >),
		typeof(Action<, , , , , , >),
		typeof(Action<, , , , , , , >),
		typeof(Action<, , , , , , , , >)
	};

	private static readonly MethodInfo CreateValueProviderMethod = typeof(IPCAttributeRegistry).GetMethod("CreateValueProviderCore", BindingFlags.Static | BindingFlags.NonPublic);

	private static readonly MethodInfo CreateDefaultValueFactoryMethod = typeof(IPCAttributeRegistry).GetMethod("CreateDefaultValueFactoryCore", BindingFlags.Static | BindingFlags.NonPublic);

	public static void RegObjectIPCs(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		RegisterIPCs(instance, instance.GetType(), instance, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public static void UnregObjectIPCs(object instance)
	{
		ArgumentNullException.ThrowIfNull(instance, "instance");
		UnregisterIPCs(instance);
	}

	public static void RegStaticIPCs(Type staticType)
	{
		ArgumentNullException.ThrowIfNull(staticType, "staticType");
		RegisterIPCs(staticType, staticType, null, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public static void UnregStaticIPCs(Type staticType)
	{
		ArgumentNullException.ThrowIfNull(staticType, "staticType");
		UnregisterIPCs(staticType);
	}

	private static void RegisterIPCs(object key, Type declaringType, object? instance, BindingFlags flags)
	{
		UnregisterIPCs(key);
		List<ICallGateProvider> list = new List<ICallGateProvider>();
		MemberInfo[] members = declaringType.GetMembers(flags);
		foreach (MemberInfo memberInfo in members)
		{
			IPCProviderAttribute customAttribute = memberInfo.GetCustomAttribute<IPCProviderAttribute>();
			if (customAttribute != null)
			{
				ICallGateProvider val = CreateProvider(instance, memberInfo, customAttribute);
				if (val != null)
				{
					list.Add(val);
				}
			}
			IPCSubscriberAttribute customAttribute2 = memberInfo.GetCustomAttribute<IPCSubscriberAttribute>();
			if (customAttribute2 != null)
			{
				CreateSubscriber(instance, memberInfo, customAttribute2);
			}
		}
		if (list.Count != 0)
		{
			RegisteredProviders.Add(key, list);
		}
	}

	private static void UnregisterIPCs(object key)
	{
		if (!RegisteredProviders.TryGetValue(key, out List<ICallGateProvider> value))
		{
			return;
		}
		foreach (ICallGateProvider item in value)
		{
			try
			{
				item.UnregisterAction();
				item.UnregisterFunc();
			}
			catch (Exception ex)
			{
				DLog.Error("卸载 IPC Provider 时发生错误", ex);
			}
		}
		RegisteredProviders.Remove(key);
	}

	private static ICallGateProvider? CreateProvider(object? instance, MemberInfo member, IPCProviderAttribute attr)
	{
		try
		{
			ICallGateProvider result;
			if (!(member is MethodInfo method))
			{
				if (member is PropertyInfo propertyInfo)
				{
					if (!propertyInfo.CanRead)
					{
						goto IL_0061;
					}
					result = CreateValueProvider(instance, propertyInfo, propertyInfo.PropertyType, attr.IPCName);
				}
				else
				{
					if (!(member is FieldInfo fieldInfo))
					{
						goto IL_0061;
					}
					result = CreateValueProvider(instance, fieldInfo, fieldInfo.FieldType, attr.IPCName);
				}
			}
			else
			{
				result = CreateMethodProvider(instance, method, attr);
			}
			goto IL_0063;
			IL_0061:
			result = null;
			goto IL_0063;
			IL_0063:
			return result;
		}
		catch (Exception ex)
		{
			DLog.Error("创建 Provider 失败: " + attr.IPCName, ex);
			return null;
		}
	}

	private static ICallGateProvider? CreateMethodProvider(object? instance, MethodInfo method, IPCProviderAttribute attr)
	{
		Type[] array = (from parameter in method.GetParameters()
			select parameter.ParameterType).ToArray();
		bool flag = method.ReturnType == typeof(void);
		Type[] genericTypes;
		Type[] genericTypes2;
		if (flag)
		{
			Type[] array2 = array;
			int num = 0;
			Type[] array3 = new Type[1 + array2.Length];
			ReadOnlySpan<Type> readOnlySpan = new ReadOnlySpan<Type>(array2);
			readOnlySpan.CopyTo(new Span<Type>(array3).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array3[num] = typeof(object);
			genericTypes = array3;
			genericTypes2 = array;
		}
		else
		{
			Type[] array3 = array;
			int num = 0;
			Type[] array2 = new Type[1 + array3.Length];
			ReadOnlySpan<Type> readOnlySpan = new ReadOnlySpan<Type>(array3);
			readOnlySpan.CopyTo(new Span<Type>(array2).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array2[num] = method.ReturnType;
			genericTypes = array2;
			array2 = array;
			num = 0;
			array3 = new Type[1 + array2.Length];
			readOnlySpan = new ReadOnlySpan<Type>(array2);
			readOnlySpan.CopyTo(new Span<Type>(array3).Slice(num, readOnlySpan.Length));
			num += readOnlySpan.Length;
			array3[num] = method.ReturnType;
			genericTypes2 = array3;
		}
		object obj = CreateProviderInstance(attr.IPCName, genericTypes);
		ICallGateProvider val = (ICallGateProvider)((obj is ICallGateProvider) ? obj : null);
		if (val == null)
		{
			return null;
		}
		Type type = CreateDelegateType(flag, genericTypes2);
		if (type == null)
		{
			DLog.Error("创建 Provider 失败: " + attr.IPCName + " - 当前仅支持最多 8 个参数的委托");
			return null;
		}
		Delegate obj2 = CreateMethodDelegate(method, instance, type);
		if ((object)obj2 == null)
		{
			return null;
		}
		RegisterProviderCallback(obj, flag ? "RegisterAction" : "RegisterFunc", obj2, attr.IPCName);
		return val;
	}

	private static object? CreateProviderInstance(string ipcName, Type[] genericTypes)
	{
		if (!GetIPCProviderMethods.TryGetValue(genericTypes.Length, out MethodInfo value))
		{
			DLog.Error($"创建 Provider 失败: {ipcName} - 找不到具有 {genericTypes.Length} 个泛型参数的 GetIpcProvider 方法");
			return null;
		}
		return value.MakeGenericMethod(genericTypes).Invoke(DalamudServiceExtension.Instance(), new object[1] { ipcName });
	}

	private static Type? CreateDelegateType(bool isAction, Type[] genericTypes)
	{
		if (isAction)
		{
			if (genericTypes.Length >= ActionDelegateTypes.Length)
			{
				return null;
			}
			if (genericTypes.Length != 0)
			{
				return ActionDelegateTypes[genericTypes.Length].MakeGenericType(genericTypes);
			}
			return ActionDelegateTypes[0];
		}
		if (genericTypes.Length == 0 || genericTypes.Length > FuncDelegateTypes.Length)
		{
			return null;
		}
		return FuncDelegateTypes[genericTypes.Length - 1].MakeGenericType(genericTypes);
	}

	private static Delegate? CreateMethodDelegate(MethodInfo method, object? instance, Type delegateType)
	{
		if (!method.IsStatic && instance == null)
		{
			DLog.Error($"创建委托失败: {method.DeclaringType?.FullName}.{method.Name} 需要实例对象");
			return null;
		}
		try
		{
			return method.IsStatic ? method.CreateDelegate(delegateType) : method.CreateDelegate(delegateType, instance);
		}
		catch (Exception ex)
		{
			DLog.Error("创建方法委托失败: " + method.DeclaringType?.FullName + "." + method.Name, ex);
			return null;
		}
	}

	private static void RegisterProviderCallback(object provider, string methodName, Delegate callback, string ipcName)
	{
		MethodInfo method = provider.GetType().GetMethod(methodName);
		if (method == null)
		{
			throw new MissingMethodException(provider.GetType().FullName, methodName);
		}
		try
		{
			method.Invoke(provider, new object[1] { callback });
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException($"注册 IPC {ipcName} 的 {methodName} 回调失败", innerException);
		}
	}

	private static ICallGateProvider? CreateValueProvider(object? instance, MemberInfo member, Type valueType, string ipcName)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		try
		{
			return (ICallGateProvider)CreateValueProviderMethod.MakeGenericMethod(valueType).Invoke(null, new object[3] { instance, member, ipcName });
		}
		catch (Exception ex)
		{
			DLog.Error("创建值 Provider 失败: " + ipcName, ex);
			return null;
		}
	}

	private static ICallGateProvider CreateValueProviderCore<T>(object? instance, MemberInfo member, string ipcName)
	{
		ICallGateProvider<T> ipcProvider = DalamudServiceExtension.Instance().GetIpcProvider<T>(ipcName);
		ipcProvider.RegisterFunc(CreateMemberGetter<T>(member, instance));
		return (ICallGateProvider)(object)ipcProvider;
	}

	private static Func<T> CreateMemberGetter<T>(MemberInfo member, object? instance)
	{
		PropertyInfo propertyInfo = member as PropertyInfo;
		if ((object)propertyInfo == null)
		{
			FieldInfo fieldInfo = member as FieldInfo;
			if ((object)fieldInfo != null)
			{
				return () => (T)fieldInfo.GetValue(instance);
			}
			throw new ArgumentOutOfRangeException("member", "仅支持字段或属性作为值 Provider");
		}
		return () => (T)propertyInfo.GetValue(instance);
	}

	private static bool CreateSubscriber(object? instance, MemberInfo member, IPCSubscriberAttribute attr)
	{
		var (flag, text) = attr.Validate();
		if (!flag)
		{
			DLog.Error("创建 Subscriber 失败: " + text);
			return false;
		}
		try
		{
			return member is FieldInfo field && CreateFieldSubscriber(instance, field, attr);
		}
		catch (Exception ex)
		{
			DLog.Error("创建 Subscriber 失败: " + attr.IPCName, ex);
			return false;
		}
	}

	private static bool CreateFieldSubscriber(object? instance, FieldInfo field, IPCSubscriberAttribute attr)
	{
		if (!IsIPCSubscriberField(field.FieldType))
		{
			return false;
		}
		object obj = CreateDefaultValueFactory(field.FieldType, attr);
		object obj2 = Activator.CreateInstance(field.FieldType, attr.IPCName, obj);
		if (!(obj2 is IPCSubscriberBase iPCSubscriberBase))
		{
			return false;
		}
		iPCSubscriberBase.AutoInitialize = attr.AutoInitialize;
		field.SetValue(instance, obj2);
		if (attr.AutoInitialize)
		{
			iPCSubscriberBase.Initialize();
		}
		return true;
	}

	private static bool IsIPCSubscriberField(Type fieldType)
	{
		if (!fieldType.IsGenericType)
		{
			return false;
		}
		Type genericTypeDefinition = fieldType.GetGenericTypeDefinition();
		if (genericTypeDefinition.Namespace == typeof(IPCSubscriber<int>).Namespace)
		{
			return genericTypeDefinition.Name.StartsWith("IPCSubscriber`", StringComparison.Ordinal);
		}
		return false;
	}

	private static object? CreateDefaultValueFactory(Type subscriberType, IPCSubscriberAttribute attr)
	{
		if (attr.DefaultValue == null || !subscriberType.IsGenericType)
		{
			return null;
		}
		Type type = subscriberType.GetGenericArguments()[^1];
		try
		{
			return CreateDefaultValueFactoryMethod.MakeGenericMethod(type).Invoke(null, new object[2] { attr.DefaultValue, attr.IPCName });
		}
		catch (TargetInvocationException ex)
		{
			DLog.Error($"解析默认值失败: DefaultValue='{attr.DefaultValue}', IPCName='{attr.IPCName}'", ex.InnerException ?? ex);
			return null;
		}
		catch (Exception ex2)
		{
			DLog.Error($"解析默认值失败: DefaultValue='{attr.DefaultValue}', IPCName='{attr.IPCName}'", ex2);
			return null;
		}
	}

	private static Func<T> CreateDefaultValueFactoryCore<T>(string rawValue, string ipcName)
	{
		T defaultValue = ConvertDefaultValue<T>(rawValue, ipcName);
		return () => defaultValue;
	}

	private static T ConvertDefaultValue<T>(string rawValue, string ipcName)
	{
		try
		{
			Type typeFromHandle = typeof(T);
			Type? underlyingType = Nullable.GetUnderlyingType(typeFromHandle);
			Type type = underlyingType ?? typeFromHandle;
			object obj = ((type == typeof(string)) ? rawValue : ((type == typeof(bool)) ? ((object)bool.Parse(rawValue)) : ((type == typeof(Guid)) ? ((object)Guid.Parse(rawValue)) : ((type == typeof(TimeSpan)) ? ((object)TimeSpan.Parse(rawValue, CultureInfo.InvariantCulture)) : ((type == typeof(DateTime)) ? ((object)DateTime.Parse(rawValue, CultureInfo.InvariantCulture)) : ((type == typeof(DateOnly)) ? ((object)DateOnly.Parse(rawValue, CultureInfo.InvariantCulture)) : ((type == typeof(TimeOnly)) ? ((object)TimeOnly.Parse(rawValue, CultureInfo.InvariantCulture)) : ((type == typeof(Version)) ? Version.Parse(rawValue) : ((type == typeof(Uri)) ? new Uri(rawValue, UriKind.RelativeOrAbsolute) : ((type == typeof(object)) ? rawValue : ((!type.IsEnum) ? Convert.ChangeType(rawValue, type, CultureInfo.InvariantCulture) : Enum.Parse(type, rawValue, ignoreCase: true))))))))))));
			object obj2 = obj;
			if (underlyingType != null)
			{
				return (T)Activator.CreateInstance(typeFromHandle, obj2);
			}
			return (T)obj2;
		}
		catch (Exception innerException)
		{
			throw new InvalidOperationException($"IPC {ipcName} 的默认值 '{rawValue}' 无法转换为 {typeof(T).Name}", innerException);
		}
	}
}
