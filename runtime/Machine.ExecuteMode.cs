// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		public enum ExecuteMode
		{
			Update = 0,
			FixedUpdate,
			Manual
		}
	}
}
