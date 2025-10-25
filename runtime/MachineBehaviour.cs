// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace Unity.FSM
{
	/// <summary>
	/// An implementation of <see cref="IMachine"/> for use with <see cref="GameObject"/>s as state machines.
	/// Implements a state stack that executes the currently active top state in the stack.
	/// </summary>
	[DisallowMultipleComponent]
	public partial class MachineBehaviour : MonoBehaviour, IMachine
	{
		/// <summary>Container for the valid states for the machine.</summary>
		protected Dictionary<Type, IState> states { get; init; } = new Dictionary<Type, IState>();

		/// <summary>Unity-serializable holder for the starting state class type.</summary>
		[field: SerializeField] public string initialClassTypeName { get; protected internal set; } = string.Empty;

		/// <summary>The stack of current states.</summary>
		protected Stack<StateRef> current { get; init; } = new Stack<StateRef>();

		/// <summary>The elapsed seconds in the current state.</summary>
		[field: SerializeField] public float timeInState { get; protected internal set; } = 0.0F;

		/// <summary>Paused state identifier of the machine.</summary>
		[field: SerializeField] public bool isPaused { get; protected internal set; } = false;

		/// <summary>If true, this machine will attempt to load all states on the current <see cref="GameObject"/>.</summary>
		[field: SerializeField] public bool autoLoadStateBehaviours { get; protected internal set; } = true;

		/// <summary>The supported execution modes of the machine.</summary>
		[field: SerializeField] public IMachine.ExecutionModes executionMode { get; protected internal set; } 
			= IMachine.ExecutionModes.Update;

		/// <summary>Whether or not the machine has at least one current state.</summary>
		public bool hasCurrent => 0 < current?.Count;

		/// <summary>
		/// Awake handles collecting auto loaded states and initialization of variables.
		/// </summary>
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

		/// <summary>
		/// Start sets the initial state, if applicable and instantiates <see cref="IMachine.Executor">s.
		/// </summary>
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

		/// <summary>Adds a collection of states.</summary>
		/// <param name="states">The collection of State interfaces to add to the machine.</param>
		public void AddStates(in IEnumerable<IState> states) 
		{
			foreach (IState state in states)
			{
				AddState(state);
			}
		}

		/// <summary>Adds a state to the machine.</summary>
		/// <param name="state">The state to add.</param>
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

		/// <summary>Change the current state to the new state type.</summary>
		/// <param name="stateClassType">The class of the new state.</param>
		public void Change(string stateClassType)
		{
			try
			{
				Change(Type.GetType(stateClassType));
			}
			catch (Exception exception)
			{
				Debug.LogError($"ChangeStateException: {exception.Message}");
			}
		}
		/// <summary>Change the current state to the new state type.</summary>
		/// <typeparam name="T">The class of the new state.</typeparam>
		public void Change<T>() where T : IState => Change(typeof(T));
		/// <summary>Change the current state to the new state type.</summary>
		/// <param name="type">The class of the new state.</param>
		public void Change(in Type type)
		{
			if (Contains(type) && !IsCurrent(type))
			{
				Pop();

				Push(type);
			}
		}

		/// <summary>Does the machine contain a state of that class type?</summary>
		/// <param name="stateClassType">The class of the state for which to look.</param>
		/// <returns>True if the state is valid for the machine.</returns>
		public bool Contains(string stateClassType)
		{
			try
			{
				return Contains(Type.GetType(stateClassType));
			}
			catch (Exception exception)
			{
				Debug.LogError($"ContainsStateException: {exception.Message}");

				return false;
			}
		}
		/// <summary>Does the machine contain a state of that class type?</summary>
		/// <typeparam name="T">The type of state class.</typeparam>
		/// <returns>True if the state is valid for the machine.</returns>
		public bool Contains<T>() where T : IState => Contains(typeof(T));
		/// <summary>Does the machine contain a state of that class type?</summary>
		/// <param name="type">The type of state class.</param>
		/// <returns>True if the state is valid for the machine.</returns>
		public bool Contains(in Type type)
		{
			return null != type && states.ContainsKey(type);
		}

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateEnter(in IMachine)">.</summary>
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

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateUpdate(in IMachine, float)">.</summary>
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

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateFixedUpdate(in IMachine, float)">.</summary>
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

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateExit(in IMachine)">.</summary>
		public void OnStateExit() 
		{
			if (hasCurrent && HasEntered(current.Peek()) && !HasExited(current.Peek()))
			{
				StateRef stateRef = current.Pop();
				current.Push(new StateRef(stateRef.state, IState.Phase.Exited));

				current.Peek().state.OnStateExit(this);
			}
		}

		/// <summary>Returns the current state by its interface.</summary>
		/// <returns>The current state.</returns>
		public IState GetCurrent() => hasCurrent ? current.Peek().state : null;

		/// <summary>Is the state class the currently active state?</summary>
		/// <param name="stateClassType">The state class.</param>
		/// <returns>True, if the class is the same as the current state; false otherwise.</returns>
		public bool IsCurrent(string stateClassType)
		{
			try
			{
				return IsCurrent(Type.GetType(stateClassType));
			}
			catch (Exception exception)
			{
				Debug.LogError($"IsCurrentStateException: {exception.Message}");

				return false;
			}
		}
		/// <summary>Is the state class the currently active state?</summary>
		/// <typeparam name="T">The state class</typeparam>
		/// <returns>True, if the class is the same as the current state; false otherwise.</returns>
		public bool IsCurrent<T>() where T : IState => IsCurrent(typeof(T));
		/// <summary>Is the state class the currently active state?</summary>
		/// <param name="type">The state class.</param>
		/// <returns>True, if the class is the same as the current state; false otherwise.</returns>
		public bool IsCurrent(in Type type) => hasCurrent && current.Peek().state.GetType() == type;

		/// <summary>Instruct the machine to pause itself.</summary>
		/// <param name="value">True will pause the machine; false will resume.</param>
		public void Pause(bool value) 
		{
			if (isPaused != value)
			{
				isPaused = value;

				// Invoke an event here...
			}
		}

		/// <summary>Enables support for state stacking and removes the current state on top of the stack.</summary>
		public void Pop() 
		{
			if (hasCurrent)
			{
				OnStateExit();

				current.Pop(); // No need to set to phase to None.
			}
		}

		/// <summary>Enables support for state stacking. Pushes a new state onto the stack.</summary>
		/// <param name="stateClassType">The new state class type.</param>
		public void Push(string stateClassType)
		{
			try
			{
				Push(Type.GetType(stateClassType));
			}
			catch (Exception exception)
			{
				Debug.LogError($"PushStateException: {exception.Message}");
			}
		}
		/// <summary>Enables support for state stacking. Pushes a new state onto the stack.</summary>
		/// <typeparam name="T">The state class to push on top of the current state collections.</typeparam>
		public void Push<T>() where T : IState { }
		/// <summary>Enables support for state stacking. Pushes a new state onto the stack.</summary>
		/// <param name="type">The new state class type.</param>
		public void Push(in Type type)
		{
			if (!IsCurrent(type) && states.ContainsKey(type))
			{
				current.Push(new StateRef(states[type]));

				OnStateEnter();
			}
		}

		/// <summary>Assigns a valid state to the machine to begin.</summary>
		/// <param name="stateClassType">The state class.</param>
		public void SetInitialState(string stateClassType)
		{
			try
			{
				SetInitialState(Type.GetType(stateClassType));
			}
			catch (Exception exception)
			{
				Debug.LogError($"SetInitialStateException: {exception.Message}");
			}
		}
		/// <summary>Assigns a valid state to the machine to begin.</summary>
		/// <typeparam name="T">The state class to assign.</typeparam>
		public void SetInitialState<T>() where T : IState => SetInitialState(typeof(T));
		/// <summary>Assigns a valid state to the machine to begin.</summary>
		/// <param name="type">The state class.</param>
		public void SetInitialState(in Type type)
		{
			if (type?.GetInterfaces().Contains(typeof(IState)) ?? false)
			{
				initialClassTypeName = type.Name;
			}
		}

		/// <summary>Encapsulates <see cref="hasCurrent"/> and <see cref="GetCurrent"/>().</summary>
		/// <param name="state">The currently active state.</param>
		/// <returns>True if there is a currently active state.</returns>
		public bool TryGetCurrent(out IState state) 
		{
			state = hasCurrent ? current.Peek().state : null; 
			
			return null != state;
		}

		/// <summary>Phase check helper for no phase.</summary>
		/// <param name="stateRef">The state to check.</param>
		/// <returns>True if state is in titular phase.</returns>
		bool HasNoPhase(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.None);
		/// <summary>Phase check helper for entered phase.</summary>
		/// <param name="stateRef">The state to check.</param>
		/// <returns>True if state is in titular phase.</returns>
		bool HasEntered(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Entered);
		/// <summary>Phase check helper for updated phase.</summary>
		/// <param name="stateRef">The state to check.</param>
		/// <returns>True if state is in titular phase.</returns>
		bool HasUpdated(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Updated);
		/// <summary>Phase check helper for fixedUpdated phase.</summary>
		/// <param name="stateRef">The state to check.</param>
		/// <returns>True if state is in titular phase.</returns>
		bool HasFixedUpdated(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.FixedUpdated);
		/// <summary>Phase check helper for exited phase.</summary>
		/// <param name="stateRef">The state to check.</param>
		/// <returns>True if state is in titular phase.</returns>
		bool HasExited(in StateRef stateRef) => ComparePhase(stateRef, IState.Phase.Exited);
		/// <summary>Compare a StateRef.phase to a Phase value.</summary>
		/// <param name="stateRef">The ref to compare.</param>
		/// <param name="Phase">The phase to compare.</param>
		/// <returns>True, if matching; false otherwise.</returns>
		bool ComparePhase(in StateRef stateRef, IState.Phase Phase) => Phase == (stateRef.phase & Phase);
	}
}
