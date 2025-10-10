// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;

namespace Unity.FSM
{
	public abstract class StateBehaviour : MonoBehaviour, IState
	{
		public abstract void Enter(in IMachine machine);

		public abstract void Execute(in IMachine machine, float delta);

		public abstract void Exit(in IMachine machine);

		public abstract void FirstExecute(in IMachine machine, float delta);
	}
}
