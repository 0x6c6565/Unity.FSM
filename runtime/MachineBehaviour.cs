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

		[field: SerializeField] public string initialClassType { get; protected internal set; } = string.Empty;

		protected Stack<StateRef> current { get; init; } = new Stack<StateRef>();

		[field: SerializeField] public float timeInState { get; protected internal set; } = 0.0F;

		[field: SerializeField] public bool isPaused { get; protected internal set; } = false;

		[field: SerializeField] public bool autoLoadStateBehaviours { get; protected internal set; } = true;

		[field: SerializeField] public IMachine.ExecuteMode executeMode { get; protected internal set; } 
			= IMachine.ExecuteMode.Update;

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
			if (!string.IsNullOrEmpty(initialClassType))
			{
				Type type = Type.GetType(initialClassType);
				if (null != type)
				{
					Push(type);
				}
			}

			switch (executeMode)
			{
				case IMachine.ExecuteMode.Update:
					{
						gameObject.AddComponent<IMachine.UpdateExecutor>();
					} break;
				case IMachine.ExecuteMode.FixedUpdate:
					{
						gameObject.AddComponent<IMachine.FixedUpdateExecutor>();
					} break;
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

		public void Enter()
		{
			if (hasCurrent && HasNoPhase(current.Peek()))
			{
				timeInState = 0.0F;

				StateRef stateRef = current.Pop();
				current.Push(new StateRef(stateRef.state, IState.Phase.Entered));

				current.Peek().state.Enter(this);
			}
		}

		public void Execute(float delta) 
		{
			if (hasCurrent && HasEnteredOrExecuted(current.Peek()) && !HasExited(current.Peek()))
			{
				timeInState += delta;

				if (!HasExecuted(current.Peek()))
				{
					StateRef stateRef = current.Pop();
					current.Push(new StateRef(stateRef.state, IState.Phase.Executed));

					current.Peek().state.FirstExecute(this, delta);
				}
				else
				{
					current.Peek().state.Execute(this, delta);
				}
			}
		}

		public void Exit() 
		{
			if (hasCurrent && HasEnteredOrExecuted(current.Peek()) && !HasExecuted(current.Peek()))
			{
				StateRef stateRef = current.Pop();
				current.Push(new StateRef(stateRef.state, IState.Phase.Exited));

				current.Peek().state.Exit(this);
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
				Exit();

				current.Pop();
			}
		}

		public void Push<T>() where T : IState { }
		public void Push(in Type type)
		{
			if (!IsCurrent(type) && states.ContainsKey(type))
			{
				current.Push(new StateRef(states[type]));

				Enter();
			}
		}

		public void SetInitialState<T>() where T : IState => SetInitialState(typeof(T));
		public void SetInitialState(in Type type)
		{
			if (type?.GetInterfaces().Contains(typeof(IState)) ?? false)
			{
				initialClassType = type.Name;
			}
		}

		public bool TryGetCurrent(out IState state) 
		{
			state = hasCurrent ? current.Peek().state : null; 
			
			return null != state;
		}

		bool HasNoPhase(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.None);
		bool HasEntered(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Entered);
		bool HasExecuted(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Executed);
		bool HasEnteredOrExecuted(in StateRef stateRef) => HasEntered(stateRef) || HasExecuted(stateRef);
		bool HasExited(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Exited);
		bool ComparePhase(in StateRef stateRef, IState.Phase Phase) => stateRef.phase == Phase;
	}
}
