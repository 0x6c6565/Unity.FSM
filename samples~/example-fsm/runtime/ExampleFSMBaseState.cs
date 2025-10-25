using System;

using UnityEngine;

namespace Unity.FSM.Samples
{
	/// <summary>
	/// An abstract specialization of StateBehaviour thatadds an editor-exposed Jump event.
	/// </summary>
	public abstract class ExampleFSMBaseState : StateBehaviour
	{
		/// <summary>An event that is called when the machine received the Jump InputAction.</summary>
		[field: SerializeField] public MachineEvent onJump { get; protected internal set; } = new MachineEvent();

		/// <summary>The IMachine-invoked method when a Jump is initiated.</summary>
		/// <param name="machine"></param>
		protected internal virtual void OnJump(IMachine machine)
		{
			Debug.Log(typeof(ExampleFSMOffState).AssemblyQualifiedName);
			onJump?.Invoke(machine);
		}
	}
}
