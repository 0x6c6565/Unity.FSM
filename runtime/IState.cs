// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
    public partial interface IState
    {
        void Enter(in IMachine machine);

        void FirstExecute(in IMachine machine, float delta);

		void Execute(in IMachine machine, float delta);

        void Exit(in IMachine machine);
	}
}
