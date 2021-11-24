/*
 * Copyright (C) 2021 Jerry Verhoeven - All Rights Reserved
 * You may use, distribute and modify this code under the terms of the MIT license.
 */

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace GrinningPickle.EditorTools
{
	public class GameObjectRenamerWindow : EditorWindow
	{
		#region Properties
		//-----------------------------------------------------------------

		//-----------------------------------------------------------------
		#endregion Properties

		#region Members
		//-----------------------------------------------------------------

		private string m_Pattern;
		private string m_NewName;
		private bool m_IsPrefix = false;
		private bool m_IsPostFix = false;

		private Vector2 m_ScrollPosition = Vector2.zero;
		private Texture2D m_Icon = null;
		private bool m_SelectionChanged = false;

		//-----------------------------------------------------------------
		#endregion Members

		#region Unity Overloads
		//-----------------------------------------------------------------

		[MenuItem("Tools/GameObject Renaming...", false, 0)]
		private static void Init()
		{
			EditorWindow.GetWindow<GameObjectRenamerWindow>(false, "Renamer", true);
		}

		private void OnSelectionChange()
		{
			m_SelectionChanged = true;
			this.Repaint();
		}

		private void OnGUI()
		{
			if (!this)
				return;

			if (m_Icon == null)
			{
				m_Icon = EditorGUIUtility.Load("icons/UnityEditor.HierarchyWindow.png") as Texture2D;
				this.titleContent = new GUIContent("Renamer", m_Icon);
			}

			EditorGUILayout.BeginVertical();
			{
				List<Transform> selectedTransformsList = Selection.gameObjects.Select(x => x.transform).ToList();

				//Auto-fill New Name field with name of first selected object.
				if ((string.IsNullOrEmpty(m_NewName) || m_SelectionChanged) && selectedTransformsList.Count > 0)
				{
					m_NewName = selectedTransformsList[0].name;
				}

				EditorGUILayout.BeginHorizontal();
				{
					m_Pattern = EditorGUILayout.TextField(new GUIContent("Pattern", "Regex pattern, this can just be a few letters, Regex will pick it up as a pattern."), m_Pattern);

					if (GUILayout.Button("clear", EditorStyles.miniButton, GUILayout.MaxWidth(40)))
					{
						m_Pattern = "";
					}
				}
				EditorGUILayout.EndHorizontal();

				m_NewName = EditorGUILayout.TextField(new GUIContent("New Name", "New Name, Regex replacement, prefix or postfix string"), m_NewName);
				m_IsPrefix = EditorGUILayout.Toggle(new GUIContent("Is Prefix", "Will use New Name as a prefix instead of a full replacement."), m_IsPrefix);
				m_IsPostFix = EditorGUILayout.Toggle(new GUIContent("Is Postfix", "Will use New Name as a postfix instead of a full replacement."), m_IsPostFix);

				EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
				{
					EditorGUILayout.LabelField("Selected Objects: " + selectedTransformsList.Count, GUILayout.ExpandWidth(true), GUILayout.MinWidth(0));
				}
				EditorGUILayout.EndHorizontal();

				m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
				{
					foreach (Transform obj in selectedTransformsList)
					{
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(obj.name, GUILayout.MaxWidth(position.width / 2));

							string prefix = m_IsPrefix ? m_NewName : "";
							string postfix = m_IsPostFix ? m_NewName : "";

							string newName = m_NewName;

							if (!string.IsNullOrEmpty(m_Pattern))
							{
								newName = Regex.Replace(obj.name, m_Pattern, m_NewName);
							}

							if (m_IsPrefix || m_IsPostFix)
							{
								newName = prefix + obj.name + postfix;
							}

							EditorGUILayout.LabelField(newName, GUILayout.MaxWidth(position.width / 2));
						}
						EditorGUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();

				if (selectedTransformsList.Count <= 0)
				{
					GUI.enabled = false;
				}

				if (GUILayout.Button("Rename"))
				{
					if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to rename these " + selectedTransformsList.Count + " objects?", "Yes", "Cancel"))
					{
						foreach (Transform obj in selectedTransformsList)
						{
							string prefix = m_IsPrefix ? m_NewName : "";
							string postfix = m_IsPostFix ? m_NewName : "";

							string newName = m_NewName;

							if (!string.IsNullOrEmpty(m_Pattern))
							{
								newName = Regex.Replace(obj.name, m_Pattern, m_NewName);
							}

							if (m_IsPrefix || m_IsPostFix)
							{
								newName = prefix + obj.name + postfix;
							}

							Undo.RecordObject(obj.gameObject, "Rename");
							obj.name = newName;

							if (PrefabUtility.IsPartOfAnyPrefab(obj.gameObject))
							{
								EditorUtility.SetDirty(obj.gameObject);
								PrefabUtility.RecordPrefabInstancePropertyModifications(obj.gameObject);
							}
						}

						Undo.FlushUndoRecordObjects();
					}
				}

				if (selectedTransformsList.Count <= 0)
				{
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndVertical();

			m_SelectionChanged = false;
		}

		//-----------------------------------------------------------------
		#endregion Unity Overloads

		#region Public Interface
		//-----------------------------------------------------------------
		//public void MyFunction()
		//{
		//
		//}
		//-----------------------------------------------------------------
		#endregion Public Interface

		#region Private Interface
		//-----------------------------------------------------------------

		//-----------------------------------------------------------------
		#endregion Private Interface
	}
}