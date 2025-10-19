// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Unity.FSM
{
	[DisallowMultipleComponent]
	public partial class MachineBehaviour : MonoBehaviour, IMachine
	{
		protected Dictionary<Type, IState> states { get; init; } = new Dictionary<Type, IState>();

		[field: SerializeField] public string initialClassTypeName { get; protected internal set; } = string.Empty;

		protected Stack<StateRef> current { get; init; } = new Stack<StateRef>();

		[field: SerializeField] public float timeInState { get; protected internal set; } = 0.0F;

		[field: SerializeField] public bool isPaused { get; protected internal set; } = false;

		[field: SerializeField] public bool autoLoadStateBehaviours { get; protected internal set; } = true;

		[field: SerializeField] public IMachine.ExecutionModes executionMode { get; protected internal set; } 
			= IMachine.ExecutionModes.Update;

		public bool hasCurrent => 0 < current?.Count;

		protected virtual void Awake()
		{
			states.Clear();
			current.Clear();

			timeInState = 0.0F;

			if (autoLoadStateBehaviours)
			{
				foreach (IState state in GetComponents<IState>())
				{
					if (states.ContainsKey(state.GetType()))
					{
						Debug.LogWarning($"{name}.states already contains an instance of {state.GetType().Name}", this);
					}

					states.Add(state.GetType(), state);
				}
			}
		}

		protected virtual void Start()
		{
			if (!string.IsNullOrEmpty(initialClassTypeName))
			{
				Type type = Type.GetType(initialClassTypeName);
				if (null != type)
				{
					Push(type);
				}
			}

			foreach (var flag in Enum.GetValues(typeof(IMachine.ExecutionModes)))
			{
				if ((IMachine.ExecutionModes)flag == (executionMode & (IMachine.ExecutionModes)flag))
				{
					switch (flag)
					{
						case IMachine.ExecutionModes.Update:
							{
								IMachine.Executor executor = gameObject.AddComponent<IMachine.UpdateExecutor>();
								executor.machine = this;

							} break;
						case IMachine.ExecutionModes.FixedUpdate:
							{
								IMachine.Executor executor = gameObject.AddComponent<IMachine.FixedUpdateExecutor>();
								executor.machine = this;
							} break;
					}
				}
			}
		}

		public void AddStates(in IEnumerable<IState> states) 
		{
			foreach (IState state in states)
			{
				AddState(state);
			}
		}

		public void AddState(in IState state) 
		{
			if (null != state)
			{
				if (!states.ContainsKey(state.GetType()))
				{
					states.Add(state.GetType(), state);
				}
			}
		}

		public void Change<T>() where T : IState => Change(typeof(T));
		public void Change(in Type type)
		{
			if (Contains(type) && !IsCurrent(type))
			{
				Pop();

				Push(type);
			}
		}

		public bool Contains<T>() where T : IState => Contains(typeof(T));
		public bool Contains(in Type type)
		{
			return null != type && states.ContainsKey(type);
		}

		public void OnStateEnter()
		{
			if (hasCurrent && HasNoPhase(current.Peek()))
			{
				timeInState = 0.0F;

				StateRef stateRef = current.Pop();
				current.Push(new StateRef(stateRef.state, IState.Phase.Entered));

				current.Peek().state.OnStateEnter(this);
			}
		}

		public void OnStateUpdate(float delta) 
		{
			if (hasCurrent && HasEntered(current.Peek()) && !HasExited(current.Peek()))
			{
				timeInState += delta;

				if (!HasUpdated(current.Peek()))
				{
					StateRef stateRef = current.Pop();
					current.Push(new StateRef(stateRef.state, stateRef.phase | IState.Phase.Updated));

					current.Peek().state.OnStateFirstUpdate(this, delta);
				}
				else
				{
					current.Peek().state.OnStateUpdate(this, delta);
				}
			}
		}

		public void OnStateFixedUpdate(float delta)
		{
			if (hasCurrent && HasEntered(current.Peek()) && !HasExited(current.Peek()))
			{
				timeInState += delta;

				if (!HasFixedUpdated(current.Peek()))
				{
					StateRef stateRef = current.Pop();
					current.Push(new StateRef(stateRef.state, stateRef.phase | IState.Phase.FixedUpdated));

					current.Peek().state.OnStateFirstFixedUpdate(this, delta);
				}
				else
				{
					current.Peek().state.OnStateFixedUpdate(this, delta);
				}
			}
		}

		public void OnStateExit() 
		{
			if (hasCurrent && HasEntered(current.Peek()) && !HasExited(current.Peek()))
			{
				StateRef stateRef = current.Pop();
				current.Push(new StateRef(stateRef.state, IState.Phase.Exited));

				current.Peek().state.OnStateExit(this);
			}
		}

		public IState GetCurrent() => hasCurrent ? current.Peek().state : null;

		public bool IsCurrent<T>() where T : IState => IsCurrent(typeof(T));
		public bool IsCurrent(in Type type) => hasCurrent && current.Peek().state.GetType() == type;

		public void Pause(bool value) 
		{
			if (isPaused != value)
			{
				isPaused = value;

				// Invoke an event here...
			}
		}

		public void Pop() 
		{
			if (hasCurrent)
			{
				OnStateExit();

				current.Pop(); // No need to set to phase to None.
			}
		}

		public void Push<T>() where T : IState { }
		public void Push(in Type type)
		{
			if (!IsCurrent(type) && states.ContainsKey(type))
			{
				current.Push(new StateRef(states[type]));

				OnStateEnter();
			}
		}

		public void SetInitialState<T>() where T : IState => SetInitialState(typeof(T));
		public void SetInitialState(in Type type)
		{
			if (type?.GetInterfaces().Contains(typeof(IState)) ?? false)
			{
				initialClassTypeName = type.Name;
			}
		}

		public bool TryGetCurrent(out IState state) 
		{
			state = hasCurrent ? current.Peek().state : null; 
			
			return null != state;
		}

		bool HasNoPhase(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.None);
		bool HasEntered(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Entered);
		bool HasUpdated(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Updated);
		bool HasFixedUpdated(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.FixedUpdated);
		bool HasExited(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Exited);
		bool ComparePhase(in StateRef stateRef, IState.Phase Phase) => Phase == (stateRef.phase & Phase);
	}
}
