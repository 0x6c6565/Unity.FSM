// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;

namespace Unity.FSM
{
	public abstract class StateBehaviour : MonoBehaviour, IState
	{
		public virtual void OnStateEnter(in IMachine machine) { }

		public virtual void OnStateFirstUpdate(in IMachine machine, float delta) { OnStateUpdate(machine, delta); }
		public virtual void OnStateUpdate(in IMachine machine, float delta) { }

		public virtual void OnStateFirstFixedUpdate(in IMachine machine, float delta) { OnStateFixedUpdate(machine, delta); }
		public virtual void OnStateFixedUpdate(in IMachine machine, float delta) { }

		public virtual void OnStateExit(in IMachine machine) { }
	}
}
