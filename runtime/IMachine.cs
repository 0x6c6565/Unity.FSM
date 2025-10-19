// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Unity.FSM
{
    public partial interface IMachine
    {
		bool isPaused { get; }

		float timeInState { get; }

        bool hasCurrent { get; }

        void AddStates(in IEnumerable<IState> states);

        void AddState(in IState state);

        void SetInitialState<T>() where T : IState;

        void Pause(bool value);

        bool IsCurrent<T>() where T : IState;

        IState GetCurrent();

        bool TryGetCurrent(out IState state);

        bool Contains<T>() where T : IState;

        void Push<T>() where T : IState;

        void Pop();

        void Change<T>() where T : IState;

        void OnStateEnter();

        void OnStateUpdate(float delta);

		void OnStateFixedUpdate(float delta);

		void OnStateExit();
    }
}
