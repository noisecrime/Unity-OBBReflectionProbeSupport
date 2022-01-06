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
	[CustomEditor( typeof( SceneObjectCollectionController ) )]
	public class SceneObjectCollectionControllerInspector : Editor
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
			SceneObjectCollectionController controller = ( SceneObjectCollectionController )target;

			serializedObject.Update();

			GUILayout.Space( 4f );

			bakeResolution = GUILayout.Toolbar( bakeResolution, resolutionOptions );

			GUILayout.BeginHorizontal();

			if ( GUILayout.Button( "Bake Active Group Probe" ) )
				ForceBakeReflection( controller.visabilityCollection, bakeResolution );
			
			if ( GUILayout.Button( "Bake All Group Probes" ) )			
				ForceBakeAllReflection( controller.visabilityCollection, bakeResolution );

			GUILayout.EndHorizontal();

			if ( GUILayout.Button( "Log Probe Details To Console" ) )			
				LogProbeDetailsToConsole( controller.visabilityCollection );

			reorderableVisabilityGroups.InspectorGUI( controller.visabilityCollection );
			reorderableCameraCollection.InspectorGUI( controller.cameraCollection );

			serializedObject.ApplyModifiedProperties();
		}

		private void ForceBakeReflection( CollectionOfTransformVisabilty visabilityCollection, int resolutionIndex )
		{
			// Search for a ReflectionProbe in group
			Component c = visabilityCollection.GetComponentFromActiveGroup( typeof(ReflectionProbe ) );

			// If Found force it to bake
			if ( null != c )			
				ReflectionProbeForcedBaker.Bake( ( ReflectionProbe )c, resolutionIndex );			
		}

		private void ForceBakeAllReflection( CollectionOfTransformVisabilty visabilityCollection, int resolutionIndex )
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

		private void LogProbeDetailsToConsole( CollectionOfTransformVisabilty visabilityCollection )
		{
			foreach ( CollectionOfTransformVisabilty.VisabilityGroup group in visabilityCollection.collection )
			{
				foreach ( Transform item in group._items )
				{
					ReflectionProbe[] probes = item.GetComponentsInChildren<ReflectionProbe>(true);

					foreach ( ReflectionProbe rp in probes )
					{
						string results = $"Probe {rp.name}  {rp.size}\n";
						results += $"bakedTexture: { FetchTextureInfo( rp.bakedTexture )}\n";
						results += $"customBakedTexture: { FetchTextureInfo( rp.customBakedTexture )}\n";

						Debug.Log( results );
					}
				}
			}
		}

		private string FetchTextureInfo( Texture texture )
		{
			if ( null == texture )
				return "None";

			return $"{texture.name}  [{AssetDatabase.GetAssetPath( texture )}]";
		}
	}
}