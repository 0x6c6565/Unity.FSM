// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;

namespace Unity.FSM
{
	/// <summary>
	/// IState is the core interface for defining states within the FSM.
	/// </summary>
    public partial interface IState
    {
		/// <summary>Invoked when the <see cref="IMachine"/> enters this state.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
        void OnStateEnter(in IMachine machine);

		/// <summary>Invoked when on <see cref="IMachine.UpdateExecutor.Update()"/> is first called for this state.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
		/// <param name="delta">The elapsed seconds between this and the previous Update() call.</param>
		void OnStateFirstUpdate(in IMachine machine, float delta);

		/// <summary>Invoked when on <see cref="IMachine.UpdateExecutor.Update()"/> is called after the first time.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
		/// <param name="delta">The elapsed seconds between this and the previous Update() call.</param>
		void OnStateUpdate(in IMachine machine, float delta);

		/// <summary>Invoked when on <see cref="IMachine.FixedUpdateExecutor.FixedUpdate()"/> is first called for this state.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
		/// <param name="delta">The elapsed seconds between this and the previous FixedUpdate() call.</param>
		void OnStateFirstFixedUpdate(in IMachine machine, float delta);

		/// <summary>Invoked when on <see cref="IMachine.FixedUpdateExecutor.FixedUpdate()"/> is called after the first time.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
		/// <param name="delta">The elapsed seconds between this and the previous FixedUpdate() call.</param>
		void OnStateFixedUpdate(in IMachine machine, float delta);

		/// <summary>Invoked when the <see cref="IMachine"/> exits this state.</summary>
		/// <param name="machine">The interface of the machine running this state.</param>
		void OnStateExit(in IMachine machine);
	}
}
