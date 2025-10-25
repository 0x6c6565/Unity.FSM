// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;
using UnityEngine.Events;

namespace Unity.FSM
{
	/// <summary>An editor-exposed event that passes an <see cref="IMachine"/>.</summary>
	[Serializable] public class MachineEvent : UnityEvent<IMachine> { }

	/// <summary>An editor-exposed event that passes an <see cref="IMachine"/> and a float.</summary>
	[Serializable] public class MachineDeltaEvent : UnityEvent<IMachine, float> { }
}
