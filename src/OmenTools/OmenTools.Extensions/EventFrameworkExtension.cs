using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.Game.Event;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using FFXIVClientStructs.Interop;
using FFXIVClientStructs.STD;
using FFXIVClientStructs.STD.Helper;
using Lumina.Excel.Sheets;
using OmenTools.Interop.Game.Lumina;

namespace OmenTools.Extensions;

public static class EventFrameworkExtension
{
	extension(scoped ref EventFramework framework)
	{
		public unsafe bool TryGetAetheryteNearby(out uint eventID)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Invalid comparison between Unknown and I4
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			eventID = 0u;
			fixed (EventFramework* ptr = &framework)
			{
				if (ptr == null)
				{
					return false;
				}
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						if ((int)((EventId)(&((EventHandlerInfo)(&((EventHandler)current.Item2.Value).Info)).EventId)).ContentId != 5)
						{
							continue;
						}
						Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)current.Item2.Value).EventObjects.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Pointer<GameObject> current2 = enumerator2.Current;
								if (!(current2 == Pointer<GameObject>.op_Implicit((GameObject*)null)) && current2.Value != null)
								{
									string nameString = ((GameObject)current2.Value).NameString;
									Aetheryte rowOrDefault = LuminaGetter.GetRowOrDefault<Aetheryte>(0u);
									if (nameString == ((object)((Aetheryte)(ref rowOrDefault)).Singular/*cast due to constrained. prefix*/).ToString())
									{
										eventID = EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)current.Item2.Value).Info)).EventId);
										return true;
									}
								}
							}
						}
						finally
						{
							((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				return false;
			}
		}

		public unsafe bool IsEventIDNearby(EventId eventID)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventFramework* ptr = &framework)
			{
				if (ptr == null)
				{
					return false;
				}
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						if (((EventHandlerInfo)(&((EventHandler)current.Item2.Value).Info)).EventId == eventID && ((EventHandler)current.Item2.Value).EventObjects.Count != 0)
						{
							return true;
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				return false;
			}
		}

		public unsafe bool TryGetNearestEventID(Predicate<EventHandlerInfo> conditionInfo, Predicate<GameObject> conditionObj, Vector3 source, out EventId eventID)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventFramework* ptr = &framework)
			{
				eventID = EventId.op_Implicit(0u);
				if (ptr == null)
				{
					return false;
				}
				List<(Vector3, uint)> list = new List<(Vector3, uint)>();
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						EventHandler* value = current.Item2.Value;
						if (((EventHandler)value).EventObjects.Count == 0 || !conditionInfo(((EventHandler)value).Info))
						{
							continue;
						}
						Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)value).EventObjects.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Pointer<GameObject> current2 = enumerator2.Current;
								if (current2.Value != null && conditionObj(*current2.Value))
								{
									list.Add((Vector3.op_Implicit(((GameObject)current2.Value).Position), EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)value).Info)).EventId)));
								}
							}
						}
						finally
						{
							((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				if (list.Count == 0)
				{
					return false;
				}
				eventID = EventId.op_Implicit(list.OrderBy<(Vector3, uint), float>(((Vector3 Position, uint EventId) x) => Vector3.DistanceSquared(source, x.Position)).FirstOrDefault().Item2);
				return true;
			}
		}

		public unsafe bool TryGetNearestEvent(Predicate<EventHandlerInfo> conditionInfo, Predicate<GameObject> conditionObj, Vector3 source, out EventId eventID, out GameObjectId eventObjectID)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0171: Unknown result type (might be due to invalid IL or missing references)
			//IL_017f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventFramework* ptr = &framework)
			{
				eventID = EventId.op_Implicit(0u);
				eventObjectID = GameObjectId.op_Implicit(0uL);
				if (ptr == null)
				{
					return false;
				}
				List<(Vector3, uint, ulong)> list = new List<(Vector3, uint, ulong)>();
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						EventHandler* value = current.Item2.Value;
						if (((EventHandler)value).EventObjects.Count == 0 || !conditionInfo(((EventHandler)value).Info))
						{
							continue;
						}
						Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)value).EventObjects.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Pointer<GameObject> current2 = enumerator2.Current;
								if (current2.Value != null && conditionObj(*current2.Value))
								{
									list.Add((Vector3.op_Implicit(((GameObject)current2.Value).Position), EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)value).Info)).EventId), GameObjectId.op_Implicit(((GameObject)current2.Value).GetGameObjectId())));
								}
							}
						}
						finally
						{
							((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				if (list.Count == 0)
				{
					return false;
				}
				(Vector3, uint, ulong) tuple = list.OrderBy<(Vector3, uint, ulong), float>(((Vector3 Position, uint EventId, ulong ObjectID) x) => Vector3.DistanceSquared(source, x.Position)).FirstOrDefault();
				eventID = EventId.op_Implicit(tuple.Item2);
				eventObjectID = GameObjectId.op_Implicit(tuple.Item3);
				return true;
			}
		}

		public unsafe bool TryGetNearestEvent(Predicate<EventHandlerInfo> conditionInfo, Predicate<GameObject> conditionObj, Vector3 source, out EventId eventID, out GameObjectId eventObjectID, out Vector3 position, out float distance3D)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019d: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventFramework* ptr = &framework)
			{
				eventID = EventId.op_Implicit(0u);
				eventObjectID = GameObjectId.op_Implicit(0uL);
				distance3D = 0f;
				position = default(Vector3);
				if (ptr == null)
				{
					return false;
				}
				List<(Vector3, uint, ulong, float)> list = new List<(Vector3, uint, ulong, float)>();
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						EventHandler* value = current.Item2.Value;
						if (((EventHandler)value).EventObjects.Count == 0 || !conditionInfo(((EventHandler)value).Info))
						{
							continue;
						}
						Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)value).EventObjects.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Pointer<GameObject> current2 = enumerator2.Current;
								if (current2.Value != null && conditionObj(*current2.Value))
								{
									list.Add((Vector3.op_Implicit(((GameObject)current2.Value).Position), EventId.op_Implicit(((EventHandlerInfo)(&((EventHandler)value).Info)).EventId), GameObjectId.op_Implicit(((GameObject)current2.Value).GetGameObjectId()), Vector3.Distance(source, Vector3.op_Implicit(((GameObject)current2.Value).Position))));
								}
							}
						}
						finally
						{
							((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				if (list.Count == 0)
				{
					return false;
				}
				(Vector3, uint, ulong, float) tuple = list.OrderBy<(Vector3, uint, ulong, float), float>(((Vector3 Position, uint EventId, ulong ObjectID, float Distance3D) x) => x.Distance3D).FirstOrDefault();
				eventID = EventId.op_Implicit(tuple.Item2);
				eventObjectID = GameObjectId.op_Implicit(tuple.Item3);
				distance3D = tuple.Item4;
				(position, _, _, _) = tuple;
				return true;
			}
		}

		public unsafe bool TryGetEvents(Predicate<EventHandlerInfo> conditionInfo, Predicate<GameObject> conditionObj, Vector3 source, out List<(EventId EventID, GameObjectId EventObjectID, Vector3 Position)> result)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			fixed (EventFramework* ptr = &framework)
			{
				result = new List<(EventId, GameObjectId, Vector3)>();
				if (ptr == null)
				{
					return false;
				}
				List<(Vector3, EventId, GameObjectId)> list = new List<(Vector3, EventId, GameObjectId)>();
				Enumerator<StdPair<uint, Pointer<EventHandler>>, uint, PairKeyExtractor<uint, Pointer<EventHandler>>> enumerator = ((EventHandlerModule)(&((EventFramework)ptr).EventHandlerModule)).EventHandlerMap.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						StdPair<uint, Pointer<EventHandler>> current = enumerator.Current;
						EventHandler* value = current.Item2.Value;
						if (((EventHandler)value).EventObjects.Count == 0 || !conditionInfo(((EventHandler)value).Info))
						{
							continue;
						}
						Enumerator<Pointer<GameObject>, Pointer<GameObject>, DefaultKeyExtractor<Pointer<GameObject>>> enumerator2 = ((EventHandler)value).EventObjects.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Pointer<GameObject> current2 = enumerator2.Current;
								if (current2.Value != null && conditionObj(*current2.Value))
								{
									list.Add((Vector3.op_Implicit(((GameObject)current2.Value).Position), ((EventHandlerInfo)(&((EventHandler)value).Info)).EventId, ((GameObject)current2.Value).GetGameObjectId()));
								}
							}
						}
						finally
						{
							((IDisposable)enumerator2/*cast due to constrained. prefix*/).Dispose();
						}
					}
				}
				finally
				{
					((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
				}
				if (list.Count == 0)
				{
					return false;
				}
				result = (from x in list
					orderby Vector3.DistanceSquared(source, x.Position)
					select (EventId: x.EventId, ObjectID: x.ObjectID, Position: x.Position)).ToList();
				return true;
			}
		}
	}
}
