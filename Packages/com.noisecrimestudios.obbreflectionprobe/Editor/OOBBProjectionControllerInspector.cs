using System.Collections.Generic;
using NoiseCrimeStudios.Rendering.OBBProjectionProbe;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoiseCrimeStudios.Rendering.OBBProjectionEditor
{
	[CustomEditor( typeof( OOBBProjectionController ) )]
    public class OOBBProjectionControllerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            OOBBProjectionController controller = ( OOBBProjectionController )target;

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Update All Renderers" ) )
            {
                controller.GetMaterialPropertyBlock( true );
                UpdateAllRenderers();
              //  SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Log Details To Console" ) )
                controller.LogDetailsToConsole();
            GUILayout.EndHorizontal();
        }

        private void UpdateAllRenderers()
        {
            Scene scene                         = SceneManager.GetActiveScene();
            List<GameObject> rootGameObjects    = new List<GameObject>(scene.rootCount);
            scene.GetRootGameObjects( rootGameObjects );

            foreach ( var root in rootGameObjects )
            {
                OOBBRenderBase[] results = root.GetComponentsInChildren<OOBBRenderBase>( true );

                foreach ( OOBBRenderBase r in results )
                    r.RequestProbeMatrix( OOBBRenderBase.RequstPurpose.UpdateAll, false );             
            }
        }
    }
}