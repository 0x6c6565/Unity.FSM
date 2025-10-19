// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		[Flags]
		public enum ExecutionModes
		{
			None = 0,
			Update = 0x1,
			FixedUpdate = 0x2,
		}
	}
}
