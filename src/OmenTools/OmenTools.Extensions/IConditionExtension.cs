using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Plugin.Services;

namespace OmenTools.Extensions;

public static class IConditionExtension
{
	extension(ICondition service)
	{
		public bool IsIdle
		{
			get
			{
				IReadOnlySet<ConditionFlag> first = service.AsReadOnlySet();
				ConditionFlag[] array = new ConditionFlag[5];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				return !first.Except(new global::_003C_003Ez__ReadOnlyArray<ConditionFlag>((ConditionFlag[])(object)array)).Any();
			}
		}

		public bool IsOccupiedInEvent
		{
			get
			{
				ConditionFlag[] array = new ConditionFlag[36];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				return service.Any((ConditionFlag[])(object)array);
			}
		}

		public bool IsBetweenAreas => service.Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)45,
			(ConditionFlag)51
		});

		public bool IsBoundByDuty
		{
			get
			{
				ConditionFlag[] array = new ConditionFlag[6];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				return service.Any((ConditionFlag[])(object)array);
			}
		}

		public bool IsWatchingCutscene => service.Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)58,
			(ConditionFlag)78
		});

		public bool IsCasting => service.Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)27,
			(ConditionFlag)87
		});

		public bool IsOnMount => service.Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)4,
			(ConditionFlag)10
		});

		public bool IsAbleToMount
		{
			get
			{
				ConditionFlag[] array = new ConditionFlag[4];
				RuntimeHelpers.InitializeArray(array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
				return !service.Any((ConditionFlag[])(object)array);
			}
		}

		public bool IsOnWorldTravel => service.Any((ConditionFlag[])(object)new ConditionFlag[2]
		{
			(ConditionFlag)93,
			(ConditionFlag)92
		});
	}
}
