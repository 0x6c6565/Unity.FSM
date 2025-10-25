// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	public partial interface IState
	{
		/// <summary>Identifiers of a State's lifecycle stages.</summary>
		[Flags]
		public enum Phase
		{
			None			= 0x0,	// This state is at the beginning of its lifecycle.
			Entered			= 0x1,	// The state has been entered.
			Updated			= 0x2,	// An executor has run an Update on the state.
			FixedUpdated	= 0x4,  // An executor has run a FixedUpdate on the state.
			Exited			= 0x8	// The state has run its Exit logic.
		}
	}
}
