using NoiseCrimeStudios.Rendering.OBBProjectionProbe;
using UnityEditor;
using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionEditor
{
	[CustomEditor( typeof( OOBBSceneGlobalProbeBase ) )]
    public class OOBBGlobalProbeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            OOBBSceneGlobalProbeBase controller = ( OOBBSceneGlobalProbeBase )target;

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Update Shader Global Reflection Matrix" ) )
                controller.RefreshShaderGlobals();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Log Details To Console" ) )
                controller.LogDetailsToConsole();
            GUILayout.EndHorizontal();
        }
    }
}