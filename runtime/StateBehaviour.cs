// Copyright (c) 2024 lee wood
//
// This file is licensed under the MIT License.

using System;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Unity.FSM
{
	/// <summary>
	/// <see cref="MachineBehaviour"/>s use discrete IState implementing types as states. This class provides a base
	/// from which to specialize. However, this isn't strictly necessary, as the only requirement is the implementation
	/// of the <see cref="IState"/> interface.
	/// 
	/// One benefit of deriving from StateBehaviour is there implmentation of <see cref="onEnter"/> and <see cref="onExit"/>.
	/// These two serialized events allow for customization from within editor without requiring scripting.
	/// </summary>
	public abstract class StateBehaviour : MonoBehaviour, IState
	{
		/// <summary>On editor-exposed event that is executed when the IState is entered.</summary>
		[field: SerializeField] public MachineEvent onEnter { get; protected internal set; } = new MachineEvent();

		/// <summary>On editor-exposed event that is executed when the IState exits.</summary>
		[field: SerializeField] public MachineEvent onExit { get; protected internal set; } = new MachineEvent();

		/// <summary>Called by the IMachine when the state is entered.</summary>
		/// <param name="machine">The machine managing the state.</param>
		public virtual void OnStateEnter(in IMachine machine) { onEnter?.Invoke(machine); }

		/// <summary>Called by the IMachine when the state calls Update() for the first time.</summary>
		/// <param name="machine">The machine managing the state.</param>
		/// <param name="delta">The elapsed time between this and the previous call to Update().</param>
		public virtual void OnStateFirstUpdate(in IMachine machine, float delta) { OnStateUpdate(machine, delta); }
		/// <summary>Called by the IMachine when the state calls Update() after the first time.</summary>
		/// <param name="machine">The machine managing the state.</param>
		/// <param name="delta">The elapsed time between this and the previous call to Update().</param>
		public virtual void OnStateUpdate(in IMachine machine, float delta) { }

		/// <summary>Called by the IMachine when the state calls FixedUpdate() for the first time.</summary>
		/// <param name="machine">The machine managing the state.</param>
		/// <param name="delta">The elapsed time between this and the previous call to FixedUpdate().</param>
		public virtual void OnStateFirstFixedUpdate(in IMachine machine, float delta) { OnStateFixedUpdate(machine, delta); }
		/// <summary>Called by the IMachine when the state calls FixedUpdate() after the first time.</summary>
		/// <param name="machine">The machine managing the state.</param>
		/// <param name="delta">The elapsed time between this and the previous call to FixedUpdate().</param>
		public virtual void OnStateFixedUpdate(in IMachine machine, float delta) { }

		/// <summary>Called by the IMachine when the state exist.</summary>
		/// <param name="machine">The machine managing the state.</param>
		public virtual void OnStateExit(in IMachine machine) { onExit?.Invoke(machine); }

		/// <summary>
		/// Log handles two keywords {this} and {name} (curly braces are required).
		/// {this} => is replaced by gameObject.name
		/// {name} => is replaced by GetType().Name on the calling StateBehaviour.
		/// </summary>
		/// <param name="message">The message to send to <see cref="Debug.Log(object)"/>.</param>
		public void Log(string message)
		{
			Debug.Log(Regex.Replace(message, @"(\{this\})|(\{name\})",
				match => match.Value switch
				{
					"{this}" => gameObject.name,
					"{name}" => GetType().Name,
					_ => ""
				}), this);
		}
	}
}
