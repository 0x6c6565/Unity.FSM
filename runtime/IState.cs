// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
    public partial interface IState
    {
        void OnStateEnter(in IMachine machine);

        void OnStateFirstUpdate(in IMachine machine, float delta);

		void OnStateUpdate(in IMachine machine, float delta);

		void OnStateFirstFixedUpdate(in IMachine machine, float delta);

		void OnStateFixedUpdate(in IMachine machine, float delta);

		void OnStateExit(in IMachine machine);
	}
}
