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
	public class ReorderableCameraPlacement 
	{
		private static float		singleLineOffset		= EditorGUIUtility.singleLineHeight + 4f;
		
		private NewItem				newItem					= null;
		private SerializedProperty  collectionsProperty;
		private ReorderableList     collectionsList;

		public class NewItem
		{
			public CollectionOfCameraPlacement.Placement	placement = CollectionOfCameraPlacement.Placement.None;
			public Transform					transform = null;
		}

		public ReorderableCameraPlacement( SerializedObject serializedObject)
		{
			collectionsProperty = serializedObject.FindProperty( "cameraCollection" ).FindPropertyRelative("collection");      
			collectionsList		= new ReorderableList( serializedObject, collectionsProperty, true, true, true, true );

			collectionsList.drawElementCallback		+= DrawElementCallback;
			collectionsList.drawHeaderCallback		+= DrawHeaderCallback;
			collectionsList.elementHeightCallback	+= ElementHeightCallback;
			collectionsList.onAddCallback			+= OnAddCallback;
		}

		~ReorderableCameraPlacement()
		{
			collectionsList.drawHeaderCallback		-= DrawHeaderCallback;
			collectionsList.drawElementCallback		-= DrawElementCallback;
			collectionsList.elementHeightCallback	-= ElementHeightCallback;	
			collectionsList.onAddCallback			-= OnAddCallback;
		}

		public void InspectorGUI( CollectionOfCameraPlacement cameraCollection )
		{						
			GUILayout.Space( 4f );

			collectionsList.DoLayoutList();		
		
			if ( null != newItem )
			{							
				GUILayout.Space( 12f );
				CreateNewItemGUI( cameraCollection );
			}
		}

		private void CreateNewItemGUI( CollectionOfCameraPlacement cameraCollection )
		{
			float oldLabelWidth = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 60f;

			GUILayout.Label( "Create New CameraPlacement", EditorStyles.boldLabel );
			GUILayout.Space( 4f );

			using ( new EditorGUI.IndentLevelScope() )
			{
				newItem.transform = ( Transform )EditorGUILayout.ObjectField( "Camera", newItem.transform, typeof( Transform ), true );

				if ( newItem.transform != null )
					newItem.placement = ( CollectionOfCameraPlacement.Placement )EditorGUILayout.EnumPopup( "Placement", newItem.placement );
			}

			GUILayout.Space( 4f );

			GUILayout.BeginHorizontal();

			GUI.enabled = newItem.transform != null;

			if ( GUILayout.Button( "Create" ) )
			{
				cameraCollection.AddCameraToTransforms( newItem.transform, newItem.placement );
				newItem = null;
			}

			GUI.enabled = true;

			if ( GUILayout.Button( "Cancel" ) )
				newItem = null;

			GUILayout.EndHorizontal();

			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
		
		#region Collection Reorderable List	
		void DrawHeaderCallback( Rect rect )
		{
			string name = "Camera Collection";
			EditorGUI.LabelField( rect, name );
		}	

		void OnAddCallback( ReorderableList list )
		{
			newItem = new NewItem();
		}

		float ElementHeightCallback( int index )
		{
			return singleLineOffset * 3 + 2f;
		}

		// https://stackoverflow.com/questions/54814067/how-to-make-vector3-fields-behave-like-the-ones-from-transform-in-a-customeditor
		// See TransformRotationGUI && EditorGUIUtility.wideMode

		void DrawElementCallback( Rect rect, int index, bool isActive, bool isFocused )
		{
			SerializedProperty element = collectionsList.serializedProperty.GetArrayElementAtIndex(index);
			
			float oldLabelWidth		= EditorGUIUtility.labelWidth;
			bool  oldWideMode		= EditorGUIUtility.wideMode;
					
			EditorGUIUtility.wideMode   = true;
			EditorGUIUtility.labelWidth = 88f; // EditorGUIUtility.currentViewWidth - 212;
			
			float offsetVertical	= rect.y + 2f;

			EditorGUI.PropertyField(
				new Rect( rect.x, offsetVertical, rect.width, EditorGUIUtility.singleLineHeight ),
				element.FindPropertyRelative( "_placement" ),
				GUIContent.none
			);

			offsetVertical += singleLineOffset;

			EditorGUI.PropertyField(
				new Rect( rect.x, offsetVertical, rect.width , EditorGUIUtility.singleLineHeight ),
				element.FindPropertyRelative( "_localPosition" ),
				new GUIContent("Local Position")
			);

			offsetVertical += singleLineOffset;

			// This is probably very wrong - check out TransformRotationGUI in Unity C# Source Code!
			Vector3 rotation =  element.FindPropertyRelative( "_localRotation" ).quaternionValue.eulerAngles;
						
            EditorGUI.BeginChangeCheck();

			EditorGUI.Vector3Field( 
				new Rect( rect.x, offsetVertical, rect.width , EditorGUIUtility.singleLineHeight ),
				"Local Rotation",
				rotation
				);

			if (EditorGUI.EndChangeCheck())
				element.FindPropertyRelative( "_localRotation" ).quaternionValue = Quaternion.Euler( rotation );

			/*
			EditorGUI.PropertyField(
				new Rect( rect.x, offsetVertical, rect.width , EditorGUIUtility.singleLineHeight ),
				element.FindPropertyRelative( "_localRotation" ),
				new GUIContent("Rotation"), true
			);*/

			EditorGUIUtility.wideMode	= oldWideMode;
			EditorGUIUtility.labelWidth = oldLabelWidth;
		}
		#endregion
	}
}