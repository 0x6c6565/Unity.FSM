// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Unity.FSM
{
    /// <summary>
    /// Core interface for implementing state machines.
    /// Machines track State interfaces which describe actions the state exists in.
    /// <see cref="MachineBehaviour"/> for an implementation.
    /// </summary>
    public partial interface IMachine
    {
        /// <summary>Is the machine paused.</summary>
		bool isPaused { get; }

        /// <summary>Elapsed seconds in current state.</summary>
		float timeInState { get; }

        /// <summary>Does the machine have a currently active state.</summary>
        bool hasCurrent { get; }

        /// <summary>Adds a collection of states.</summary>
        /// <param name="states">The collection of State interfaces to add to the machine.</param>
        void AddStates(in IEnumerable<IState> states);

        /// <summary>Adds a state to the machine.</summary>
        /// <param name="state">The state to add.</param>
        void AddState(in IState state);

		/// <summary>Assigns a valid state to the machine to begin.</summary>
		/// <param name="stateClassType">The state class.</param>
		void SetInitialState(string stateClassType);
        /// <summary>Assigns a valid state to the machine to begin.</summary>
        /// <typeparam name="T">The state class to assign.</typeparam>
        void SetInitialState<T>() where T : IState;
		/// <summary>Assigns a valid state to the machine to begin.</summary>
		/// <param name="type">The state class.</param>
		void SetInitialState(in Type type);

        /// <summary>Instruct the machine to pause itself.</summary>
        /// <param name="value">True will pause the machine; false will resume.</param>
        void Pause(bool value);

		/// <summary>Is the state class the currently active state?</summary>
		/// <param name="stateClassType">The state class.</param>
		/// <returns>True, if the class is the same as the current state; false otherwise.</returns>
		bool IsCurrent(string stateClassType);
        /// <summary>Is the state class the currently active state?</summary>
        /// <typeparam name="T">The state class</typeparam>
        /// <returns>True, if the class is the same as the current state; false otherwise.</returns>
        bool IsCurrent<T>() where T : IState;
		/// <summary>Is the state class the currently active state?</summary>
		/// <param name="type">The state class.</param>
		/// <returns>True, if the class is the same as the current state; false otherwise.</returns>
		bool IsCurrent(in Type type);

        /// <summary>Returns the current state by its interface.</summary>
        /// <returns>The current state.</returns>
        IState GetCurrent();

        /// <summary>Encapsulates <see cref="hasCurrent"/> and <see cref="GetCurrent"/>().</summary>
        /// <param name="state">The currently active state.</param>
        /// <returns>True if there is a currently active state.</returns>
        bool TryGetCurrent(out IState state);

        /// <summary>Does the machine contain a state of that class type?</summary>
        /// <param name="stateClassType">The class of the state for which to look.</param>
        /// <returns>True if the state is valid for the machine.</returns>
        bool Contains(string stateClassType);
		/// <summary>Does the machine contain a state of that class type?</summary>
		/// <typeparam name="T">The type of state class.</typeparam>
		/// <returns>True if the state is valid for the machine.</returns>
		bool Contains<T>() where T : IState;
		/// <summary>Does the machine contain a state of that class type?</summary>
		/// <param name="type">The type of state class.</param>
		/// <returns>True if the state is valid for the machine.</returns>
		bool Contains(in Type type);

        /// <summary>Enables support for state stacking. Pushes a new state onto the stack.</summary>
        /// <param name="stateClassType">The new state class type.</param>
        void Push(string stateClassType);
		/// <summary>Enables support for state stacking.</summary>
		/// <typeparam name="T">The state class to push on top of the current state collections.</typeparam>
		void Push<T>() where T : IState;
        /// <summary>Enables support for state stacking. Pushes a new state onto the stack.</summary>
        /// <param name="type">The new state class type.</param>
        void Push(in Type type);

		/// <summary>Enables support for state stacking and removes the current state on top of the stack.</summary>
		void Pop();

        /// <summary>Change the current state to the new state type.</summary>
        /// <param name="stateClassType">The class of the new state.</param>
        void Change(string stateClassType);
		/// <summary>Change the current state to the new state type.</summary>
		/// <typeparam name="T">The class of the new state.</typeparam>
		void Change<T>() where T : IState;
		/// <summary>Change the current state to the new state type.</summary>
		/// <param name="type">The class of the new state.</param>
		public void Change(in Type type);

        /// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateEnter(in IMachine)">.</summary>
		void OnStateEnter();

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateUpdate(in IMachine, float)">.</summary>
		void OnStateUpdate(float delta);

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateFixedUpdate(in IMachine, float)">.</summary>
		void OnStateFixedUpdate(float delta);

		/// <summary>Exposed helper managing machines externally. Handles <see cref="IState.OnStateExit(in IMachine)">.</summary>
		void OnStateExit();
    }
}
