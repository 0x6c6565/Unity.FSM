// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	public partial interface IMachine
	{
		/// <summary>
		/// The default supported execution points.
		/// </summary>
		[Flags]
		public enum ExecutionModes
		{
			/// <summary>If this is the only ExecutionMode then processing must occur manually.</summary>
			None = 0,
			/// <summary>If this is set then the IMachine should route through an <see cref="UpdateExecutor"/>.</summary>
			Update = 0x1,
			/// <summary>If this is set then the IMachine should route through a <see cref="FixedUpdateExecutor"/>.</summary>
			FixedUpdate = 0x2,
		}
	}
}
