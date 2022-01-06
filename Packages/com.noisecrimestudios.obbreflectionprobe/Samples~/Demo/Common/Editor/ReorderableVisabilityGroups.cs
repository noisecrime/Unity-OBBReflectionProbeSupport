using System.Collections.Generic;
using NoiseCrimeStudios.Examples.OBBProjection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// ReorderableList
// https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
// https://pastebin.com/WhfRgcdC
// GUIUtility.ExitGUI();

namespace NoiseCrimeStudios.Examples.OBBProjectionEditor
{
	public class ReorderableVisabilityGroups
    {     
		private static float		singleLineOffset		= EditorGUIUtility.singleLineHeight + 4f;
		private static string       defaultCollectionName   = "New Collection";

		private NewItem				newItem					= null;
		private SerializedProperty  collectionsProperty;
		private SerializedProperty  collectionOfProperty;
		private ReorderableList     collectionsList;
	//	private CollectionOfTransformVisabilty collectionOfTransformVisabilty;

		public class NewItem
		{
			public string					name = defaultCollectionName;
			public Transform				root = null;
			public List<Transform>			children;
			public bool[]					selected;			
			public Dictionary<int, bool>	selection; // <unityID, selected>

			public void BuildSelectionFromRoot( int levels )
			{
				selection = new Dictionary<int, bool>();

				BuildSelectionRecurse( root, levels );
			}

			public void BuildSelectionRecurse( Transform parent, int levels )
			{
				levels--;

				for ( int i = 0; i < parent.childCount; i++ )
				{
					Transform child = parent.GetChild( i );
					
					int id = child.GetInstanceID();

					if ( selection.ContainsKey( id ) )
						continue;

					selection.Add( id, false );

					if ( levels > 0 &&  child.childCount > 0 )
						BuildSelectionRecurse( child, levels );
				}
			}

			public void DisplayHiearchyRecurseGUI( Transform parent, int levels )
			{
				levels--;
				using ( new EditorGUI.IndentLevelScope() )
				{
					for ( int i = 0; i < parent.childCount; i++ )
					{
						Transform child = parent.GetChild( i );

						int id = child.GetInstanceID();

						if ( selection.ContainsKey( id ) )
						{
							selection[ id ] = EditorGUILayout.Toggle( child.name, selection[ id ] );

							if ( levels > 0 && child.childCount > 0 )
								DisplayHiearchyRecurseGUI( child, levels );
						}
						else
						{
							GUI.enabled = false;
							EditorGUILayout.LabelField( child.name );
							GUI.enabled = true;							
						}
					}
				}
			}

			public void PopulateSelectedTransforms( Transform parent, int levels, List<Transform> transforms )
			{
				levels--;
				using ( new EditorGUI.IndentLevelScope() )
				{
					for ( int i = 0; i < parent.childCount; i++ )
					{
						Transform child = parent.GetChild( i );

						int id = child.GetInstanceID();

						if ( selection.ContainsKey( id ) && selection[ id ] == true )
						{
							transforms.Add( child );

							if ( levels > 0 && child.childCount > 0 )
								PopulateSelectedTransforms( child, levels, transforms );
						}
					}
				}
			}
		}

		public ReorderableVisabilityGroups( SerializedObject serializedObject  )
		{
			collectionOfProperty	= serializedObject.FindProperty( "visabilityCollection" );
			collectionsProperty		= collectionOfProperty.FindPropertyRelative("collection");     
			collectionsList			= new ReorderableList( serializedObject, collectionsProperty, true, true, true, true );

			collectionsList.drawElementCallback		+= DrawListItems;
			collectionsList.drawHeaderCallback		+= DrawHeader;
			collectionsList.elementHeightCallback	+= ElementHeightCallback;
			collectionsList.onAddCallback			+= OnAddCallback;
#if !USE_FOLDOUT_UI
			collectionsList.onSelectCallback		+= OnSelectCallback;
#endif			
		}
		~ReorderableVisabilityGroups()
		{
			collectionsList.drawHeaderCallback		-= DrawHeader;
			collectionsList.drawElementCallback		-= DrawListItems;
			collectionsList.elementHeightCallback	-= ElementHeightCallback;	
			collectionsList.onAddCallback			-= OnAddCallback;
#if !USE_FOLDOUT_UI
			collectionsList.onSelectCallback		-= OnSelectCallback;
#endif
		}

		public void InspectorGUI( CollectionOfTransformVisabilty visabilityCollection )
		{						
			GUILayout.Space( 4f );

			collectionsList.DoLayoutList();		
		
			if ( null != newItem )
			{							
				GUILayout.Space( 12f );
				CreateNewItemGUI( visabilityCollection );
			}			
		}

		private void CreateNewItemGUI( CollectionOfTransformVisabilty visabilityCollection )
		{
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 60f;

			GUILayout.Label( "Create New Collection", EditorStyles.boldLabel );
			GUILayout.Space( 4f );

			GUI.enabled = null != newItem.root;
			visabilityCollection.hiearchyLevels = EditorGUILayout.IntSlider( visabilityCollection.hiearchyLevels, 1, 5 );		
			GUI.enabled = true;

			GUILayout.Space( 4f );

			using ( new EditorGUI.IndentLevelScope() )
			{				
				EditorGUI.BeginChangeCheck();

				newItem.root = ( Transform )EditorGUILayout.ObjectField( "Root", newItem.root, typeof( Transform ), true );

				if ( EditorGUI.EndChangeCheck() )
				{
					// newItem.hiearchyLevels = visabilityCollection.hiearchyLevels;
					newItem.name = newItem.root.name;
					newItem.BuildSelectionFromRoot( visabilityCollection.hiearchyLevels );
				}

				if ( newItem.root != null )
				{
					newItem.name = EditorGUILayout.TextField( "Name", newItem.name );

					if ( newItem.name != defaultCollectionName )
					{
						// if ( null == newItem.selected || newItem.selected.Length != newItem.root.childCount )
						//	newItem.selected = new bool[ newItem.root.childCount ];

						EditorGUIUtility.labelWidth = oldLabelWidth * 2;

						newItem.DisplayHiearchyRecurseGUI( newItem.root, visabilityCollection.hiearchyLevels );

					//	for ( int i = 0; i < newItem.root.childCount; i++ )
					//		newItem.selected[ i ] = EditorGUILayout.Toggle( newItem.root.GetChild( i ).name, newItem.selected[ i ] );
					}
				}
			}

			GUILayout.Space( 4f );

			GUILayout.BeginHorizontal();

			GUI.enabled = newItem.root != null && newItem.name != defaultCollectionName;

			if ( GUILayout.Button( "Create" ) )
			{
				// Build list of transforms to add to collection from selected items
				List<Transform> transforms = new List<Transform>();

				newItem.PopulateSelectedTransforms( newItem.root, visabilityCollection.hiearchyLevels, transforms );

				/*
				for ( int i = 0; i < newItem.root.childCount; i++ )
					if ( newItem.selected[ i ] )
						transforms.Add( newItem.root.GetChild( i ) );
				*/

				if ( transforms.Count > 0 )
					visabilityCollection.AddCollection( newItem.name, false, visabilityCollection.hiearchyLevels, newItem.root, transforms );

				newItem = null;
			}

			GUI.enabled = true;

			if ( GUILayout.Button( "Cancel" ) )
				newItem = null;

			GUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}

		#region Collection Reorderable List		
		void DrawHeader( Rect rect )
		{
			string name = "Visability Collections";
			EditorGUI.LabelField( rect, name );
		}

		void OnSelectCallback( ReorderableList list )
		{
			SerializedProperty isExpanded = list.serializedProperty.GetArrayElementAtIndex(list.index).FindPropertyRelative( "_isExpanded" );
			isExpanded.boolValue = !isExpanded.boolValue;
		}

		void OnAddCallback( ReorderableList list )
		{
			newItem = new NewItem();
		}

		float ElementHeightCallback( int index )
		{
			SerializedProperty element = collectionsList.serializedProperty.GetArrayElementAtIndex(index);

			if ( element.FindPropertyRelative("_isExpanded").boolValue )
				return singleLineOffset * 4 + singleLineOffset * element.FindPropertyRelative("_items").arraySize + 16f;

			return singleLineOffset + 2f;
			// Note: EditorGUI.GetPropertyHeight(element, true); doesn't appear to work reliably.
			// return element.FindPropertyRelative("_isExpanded").boolValue == false ? singleLineOffset + 2f : EditorGUI.GetPropertyHeight(element, true);
		}

		void DrawListItems( Rect rect, int index, bool isActive, bool isFocused )
		{
			SerializedProperty element = collectionsList.serializedProperty.GetArrayElementAtIndex(index);
			
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 40f;
			
			float foldoutWidth		= 0f;
			float offsetVertical	= rect.y + 2f;
			SerializedProperty isExpanded = element.FindPropertyRelative( "_isExpanded" );
						
#if USE_FOLDOUT_UI
			foldoutWidth = 16f;

			isExpanded.boolValue = EditorGUI.Foldout(				
				new Rect( rect.x + rect.width, offsetVertical, foldoutWidth, EditorGUIUtility.singleLineHeight ),
				isExpanded.boolValue, "");
#endif

			EditorGUI.PropertyField(
				new Rect( rect.x, offsetVertical, rect.width - 56f - foldoutWidth , EditorGUIUtility.singleLineHeight ),
				element.FindPropertyRelative( "_name" ),
				GUIContent.none
			);

			if ( GUI.Button( new Rect( rect.x + rect.width - 48f - foldoutWidth, offsetVertical, 48f, EditorGUIUtility.singleLineHeight ), "Show"))				
				((SceneObjectCollectionController)collectionOfProperty.serializedObject.targetObject).visabilityCollection.ShowCollection(index);
		
			if ( isExpanded.boolValue )
			{				
				float offsetX = rect.x + 8f;

				SerializedProperty items	= element.FindPropertyRelative( "_items" );
				SerializedProperty levels	= element.FindPropertyRelative( "_levels" );
				
				offsetVertical += singleLineOffset;

				GUI.Box( new Rect( rect.x, offsetVertical, rect.width, 8f + singleLineOffset * ( items.arraySize + 3)), "" );
				
				offsetVertical += 4f;

				EditorGUI.LabelField( new Rect( offsetX, offsetVertical, rect.width-offsetX-16f, EditorGUIUtility.singleLineHeight ), "Root" );
				EditorGUI.LabelField( new Rect( rect.width-64f, offsetVertical, 80f, EditorGUIUtility.singleLineHeight ), "Levels" , levels.intValue.ToString()  );
				
				offsetVertical += singleLineOffset;

				EditorGUI.PropertyField(
					new Rect( offsetX, offsetVertical, rect.width-16f, EditorGUIUtility.singleLineHeight ),
					element.FindPropertyRelative( "_root" ),
					GUIContent.none
				);


				offsetVertical += singleLineOffset;

				EditorGUI.LabelField( new Rect( offsetX, offsetVertical, rect.width-16f, EditorGUIUtility.singleLineHeight ), "Items" );

				offsetVertical += singleLineOffset;

				for ( int i = 0; i < items.arraySize; i++ )
				{
					EditorGUI.PropertyField(
						new Rect( offsetX, offsetVertical + i * singleLineOffset, rect.width-16f, EditorGUIUtility.singleLineHeight ),
						items.GetArrayElementAtIndex( i ),
						GUIContent.none
					);
				}
			}

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
		#endregion
    }
}