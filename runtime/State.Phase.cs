// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	public partial interface IState
	{
		[Flags]
		public enum Phase
		{
			None			= 0x0,
			Entered			= 0x1,
			Updated			= 0x2,
			FixedUpdated	= 0x4,
			Exited			= 0x8
		}
	}
}
