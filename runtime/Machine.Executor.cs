// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		public abstract class Executor : MonoBehaviour 
		{
			public IMachine machine { get; protected internal set; } = null;
		}

		[DisallowMultipleComponent]
		public class UpdateExecutor : Executor
		{
			void Update()
			{
				if (null != machine && !machine.isPaused)
				{
					machine.OnStateUpdate(Time.deltaTime);
				}
			}
		}

		[DisallowMultipleComponent]
		public class FixedUpdateExecutor : Executor
		{
			void FixedUpdate()
			{
				if (null != machine && !machine.isPaused)
				{
					machine.OnStateFixedUpdate(Time.fixedDeltaTime);
				}
			}
		}
	}
}
