// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Collections.Generic;

namespace Unity.FSM
{
	/// <summary>
	/// Holder for references to states within the <see cref="IMachine.GetCurrent()"/> container.
	/// </summary>
	public readonly struct StateRef : IEquatable<StateRef>
	{
		/// <summary>The reference to the State being tracked.</summary>
		public IState state { get; init; }

		/// <summary>The current step in the State's lifecycle (for this particular machine implementation.)</summary>
		public IState.Phase phase { get; init; }

		/// <summary>Default c'tor.</summary>
		/// <param name="state">The State to track.</param>
		/// <param name="phase">The current point in the State's lifecycle (<see cref="IState.Phase.None"/> by default.)</param>
		public StateRef(IState state, IState.Phase phase = IState.Phase.None)
		{
			this.state = state;
			this.phase = phase;
		}

		/// <summary>Equator impl for StateRef.</summary>
		/// <param name="obj">The object to compare.</param>
		/// <returns>True, if equal; false otherwise.</returns>
		public override bool Equals(object obj) => obj is StateRef @ref && Equals(@ref);
		/// <summary>Equality comparer impl for StateRef comparisons to other StateRefs.</summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(StateRef other) => EqualityComparer<IState>.Default.Equals(state, other.state);

		/// <summary>Hasher for state only (through interface) as phase is not tracked.</summary>
		/// <returns>The hash code of the State.</returns>
		public override int GetHashCode() => state.GetHashCode();

		/// <summary>Equality op impl.</summary>
		/// <param name="left">The StateRef to compare against.</param>
		/// <param name="right">The StateRef to compare.</param>
		/// <returns>True, if equal; false otherwise.</returns>
		public static bool operator ==(StateRef left, StateRef right) => left.Equals(right);
		/// <summary>Inequality op impl <see cref="StateRef.!="/>.</summary>
		/// <param name="left">The StateRef to compare against.</param>
		/// <param name="right">The StateRef to compare.</param>
		/// <returns>False, if equal; true otherwise.</returns>
		public static bool operator !=(StateRef left, StateRef right) => !(left == right);
	}
}
