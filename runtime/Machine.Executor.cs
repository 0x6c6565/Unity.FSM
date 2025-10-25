// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		/// <summary>Base class for handling execution logic (abstract.)</summary>
		public abstract class Executor : MonoBehaviour 
		{
			public IMachine machine { get; protected internal set; } = null;
		}

		/// <summary>An executor that handles Update() calls.</summary>
		[DisallowMultipleComponent]
		public class UpdateExecutor : Executor
		{
			/// <summary>When this update executes it will notify the machine to invoke OnStateUpdate().</summary>
			void Update()
			{
				// Ignore if FSM is paused.
				if (null != machine && !machine.isPaused)
				{
					machine.OnStateUpdate(Time.deltaTime);
				}
			}
		}

		/// <summary>An executor that handles FixedUpdate() calls.</summary>
		[DisallowMultipleComponent]
		public class FixedUpdateExecutor : Executor
		{
			void FixedUpdate()
			{
				// Ignore if FSM is paused.
				if (null != machine && !machine.isPaused)
				{
					machine.OnStateFixedUpdate(Time.fixedDeltaTime);
				}
			}
		}
	}
}
