using Dalamud.Game.Agent;
using Dalamud.Game.NativeWrapper;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using OmenTools.Interop.Game.Models;

namespace OmenTools.Extensions;

public static class AgentExtension
{
	extension(AgentId agentID)
	{
		public unsafe AtkValue* SendEvent(ulong eventKind, params object[] eventParams)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return ((AgentId)agentID).SendEvent(eventKind, eventParams);
		}

		public unsafe AtkValue* SendEvent(ulong eventKind, scoped in AtkValueArray valueArray)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			return ((AgentId)agentID).SendEvent(eventKind, in valueArray);
		}
	}

	extension(AgentId agentID)
	{
		public unsafe AtkValue* SendEvent(ulong eventKind, params object[] eventParams)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId(agentID);
			if (agentByInternalId != null)
			{
				return (*agentByInternalId).SendEvent(eventKind, eventParams);
			}
			return null;
		}

		public unsafe AtkValue* SendEvent(ulong eventKind, scoped in AtkValueArray valueArray)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			AgentInterface* agentByInternalId = ((AgentModule)AgentModule.Instance()).GetAgentByInternalId(agentID);
			if (agentByInternalId != null)
			{
				return (*agentByInternalId).SendEvent(eventKind, in valueArray);
			}
			return null;
		}
	}

	extension(scoped ref AgentInterface agentInterface)
	{
		public unsafe AtkValue* SendEvent(ulong eventKind, params object[] eventParams)
		{
			AtkValue* returnValue = (AtkValue*)stackalloc AtkValue[1];
			return agentInterface.SendEvent(returnValue, eventKind, eventParams);
		}

		public unsafe AtkValue* SendEvent(ulong eventKind, scoped in AtkValueArray valueArray)
		{
			AtkValue* returnValue = (AtkValue*)stackalloc AtkValue[1];
			return agentInterface.SendEvent(returnValue, eventKind, in valueArray);
		}

		public unsafe AtkValue* SendEvent(AtkValue* returnValue, ulong eventKind, params object[] eventParams)
		{
			using AtkValueArray atkValueArray = new AtkValueArray(eventParams);
			((AgentInterface)(ref agentInterface)).ReceiveEvent(returnValue, (AtkValue*)atkValueArray, (uint)eventParams.Length, eventKind);
			return returnValue;
		}

		public unsafe AtkValue* SendEvent(AtkValue* returnValue, ulong eventKind, scoped in AtkValueArray valueArray)
		{
			((AgentInterface)(ref agentInterface)).ReceiveEvent(returnValue, (AtkValue*)valueArray, (uint)valueArray.Length, eventKind);
			return returnValue;
		}
	}

	extension(scoped in AgentInterfacePtr agent)
	{
		public unsafe AgentInterface* ToStruct()
		{
			return (AgentInterface*)agent.Address;
		}

		public unsafe T* ToStruct<T>() where T : unmanaged
		{
			return (T*)agent.Address;
		}
	}
}
