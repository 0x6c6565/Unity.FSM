using System;
using System.Reflection;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.UIElements;

namespace Unity.FSM.Editor
{
    [CustomEditor(typeof(MachineBehaviour), true)]
    public sealed class MachineBehaviourInspector : UnityEditor.Editor
    {
		const string INITIAL_CLASS_TYPE_NAME = "<initialClassTypeName>k__BackingField";
		const string INITIAL_CLASS_TYPE_BUTTON = "InitialClassTypeButton";

		[field: SerializeField] public VisualTreeAsset document { get; private set; }

		Button initialClassTypeButton { get; set; }


		public override VisualElement CreateInspectorGUI()
		{
			VisualElement root = new VisualElement();

			document.CloneTree(root);

			initialClassTypeButton = root.Q<Button>(INITIAL_CLASS_TYPE_BUTTON);
			if (null != initialClassTypeButton)
			{
				SerializedProperty initialClassTypeNameProperty = serializedObject.FindProperty(INITIAL_CLASS_TYPE_NAME);
				if (null != initialClassTypeNameProperty)
				{
					if (!string.IsNullOrEmpty(initialClassTypeNameProperty.stringValue))
					{
						initialClassTypeButton.text = Type.GetType(initialClassTypeNameProperty.stringValue).Name;
					}
					else
					{
						Debug.LogWarning("Type is empty!");
					}
				}
				else
				{
					Debug.LogWarning("Not found!");
				}

				initialClassTypeButton.RegisterCallback<ClickEvent>(OnInitialClassTypeClicked);
			}

			return root; 
		}

		void OnInitialClassTypeClicked(ClickEvent @event)
		{
			if (null != initialClassTypeButton)
			{
				GenericDropdownMenu menu = new GenericDropdownMenu();
				menu.DropDown(initialClassTypeButton.worldBound, initialClassTypeButton, true);

				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (!type.IsAbstract && type.IsSubclassOf(typeof(StateBehaviour)))
						{
							menu.AddItem(type.Name, false, () => OnSelectInitialClassType(type));
						}
					}
				}
			}
		}

		void OnSelectInitialClassType(Type type)
		{
			if (null != type)
			{
				if (null != initialClassTypeButton)
				{
					initialClassTypeButton.text = type.Name;
				}

				SerializedProperty initialClassTypeNameProperty = serializedObject.FindProperty(INITIAL_CLASS_TYPE_NAME);
				if (null != initialClassTypeNameProperty)
				{
					initialClassTypeNameProperty.stringValue = type.AssemblyQualifiedName;
					initialClassTypeNameProperty.serializedObject.ApplyModifiedProperties();
				}
			}
		}
	}
}
