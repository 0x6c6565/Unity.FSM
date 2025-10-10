// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

using UnityEngine;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		[DisallowMultipleComponent]
		public abstract class Executor : MonoBehaviour 
		{
			public IMachine machine { get; init; } = null;
		}

		public class UpdateExecutor : Executor
		{
			void Update()
			{
				if (null != machine && !machine.isPaused)
				{
					machine.Execute(Time.deltaTime);
				}
			}
		}

		public class FixedUpdateExecutor : Executor
		{
			void FixedUpdate()
			{
				if (null != machine && !machine.isPaused)
				{
					machine.Execute(Time.fixedDeltaTime);
				}
			}
		}
	}
}
