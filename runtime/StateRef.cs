// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Unity.FSM
{
	public readonly struct StateRef : IEquatable<StateRef>
	{
		public IState state { get; init; }

		public IState.Phase phase { get; init; }

		public StateRef(IState state, IState.Phase phase = IState.Phase.None)
		{
			this.state = state;
			this.phase = phase;
		}

		public override bool Equals(object obj) => obj is StateRef @ref && Equals(@ref);
		public bool Equals(StateRef other) => EqualityComparer<IState>.Default.Equals(state, other.state);

		public override int GetHashCode() => state.GetHashCode();

		public static bool operator ==(StateRef left, StateRef right) => left.Equals(right);
		public static bool operator !=(StateRef left, StateRef right) => !(left == right);
	}
}
