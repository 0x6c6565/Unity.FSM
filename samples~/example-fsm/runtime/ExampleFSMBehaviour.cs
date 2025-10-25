using System;

using UnityEngine;

using Unity.FSM;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif // ENABLE_INPUT_SYSTEM

namespace Unity.FSM.Samples
{
	/// <summary>
	/// MachineBehaviour implementation that adds an editor-exposed Jump event.
	/// </summary>
	public class ExampleFSMBehaviour : MachineBehaviour
	{
#if ENABLE_INPUT_SYSTEM
		
		/// <summary>When the Jump button is pressed this will call current state's OnJump method.</summary>
		/// <param name="context">The input context for the state of the InputAction.</param>
		public void OnJump(InputAction.CallbackContext context)
		{
			if (context.performed)
			{
				// Ensure at least one active state exists...
				if (hasCurrent)
				{
					// If the top state is a specialization of ExampleFSMBaseState then invoke its OnJump().
					(current.Peek().state as ExampleFSMBaseState)?.OnJump(this);
				}
			}
		}
#endif // ENABLE_INPUT_SYSTEM
	}
}
