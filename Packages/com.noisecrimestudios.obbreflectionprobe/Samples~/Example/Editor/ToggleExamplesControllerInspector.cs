// #define USE_FOLDOUT_UI

using NoiseCrimeStudios.Examples.OBBProjection;
using UnityEditor;
using UnityEngine;

// ReorderableList
// https://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/
// https://pastebin.com/WhfRgcdC
// GUIUtility.ExitGUI();

namespace NoiseCrimeStudios.Examples.OBBProjectionEditor
{
	[DisallowMultipleComponent]
	[CustomEditor( typeof( ExampleSetupController ) )]
	public class ToggleExamplesControllerInspector : Editor
	{	
		static string[] resolutionOptions = new string[]{ "16", "32", "64", "128", "256", "512", "1024", "2048" };
		private int bakeResolution = 5;		

		private ReorderableVisabilityGroups reorderableVisabilityGroups;
		private ReorderableCameraPlacement	reorderableCameraCollection;
		

		private void OnEnable()
		{
			reorderableVisabilityGroups = new ReorderableVisabilityGroups( serializedObject );
			reorderableCameraCollection = new ReorderableCameraPlacement( serializedObject );
		}

		private void OnDisable()
		{
			if ( null != reorderableCameraCollection)
				reorderableCameraCollection = null;

			if ( null != reorderableVisabilityGroups)
				reorderableVisabilityGroups = null;
		}

		public override void OnInspectorGUI()
		{			
			ExampleSetupController controller = ( ExampleSetupController )target;

			serializedObject.Update();

			GUILayout.Space( 4f );

			bakeResolution = GUILayout.Toolbar( bakeResolution, resolutionOptions );

			GUILayout.BeginHorizontal();

			if ( GUILayout.Button( "Bake Active Group Probe" ) )
				ForceBakeReflection( controller.visabilityCollection, bakeResolution );
			
			if ( GUILayout.Button( "Bake All Group Probes" ) )			
				ForceBakeAllReflection( controller.visabilityCollection, bakeResolution );

			GUILayout.EndHorizontal();

			reorderableVisabilityGroups.InspectorGUI( controller.visabilityCollection );
			reorderableCameraCollection.InspectorGUI( controller.cameraCollection );

			serializedObject.ApplyModifiedProperties();
		}

		public void ForceBakeReflection( CollectionOfTransformVisabilty visabilityCollection, int resolutionIndex )
		{
			// Search for a ReflectionProbe in group
			Component c = visabilityCollection.GetComponentFromActiveGroup( typeof(ReflectionProbe ) );

			// If Found force it to bake
			if ( null != c )			
				ReflectionProbeForcedBaker.Bake( ( ReflectionProbe )c, resolutionIndex );			
		}

		public void ForceBakeAllReflection( CollectionOfTransformVisabilty visabilityCollection, int resolutionIndex )
		{
			foreach ( CollectionOfTransformVisabilty.VisabilityGroup group in visabilityCollection.collection )
			{
				visabilityCollection.ShowCollection( group );

				// Search for a ReflectionProbe in group
				Component c = visabilityCollection.GetComponentFromGroup( group, typeof(ReflectionProbe ) );

				// need to yeild!

				// If Found force it to bake
				if ( null != c )
					ReflectionProbeForcedBaker.Bake( ( ReflectionProbe )c, resolutionIndex );
			}		
		}

	}
}