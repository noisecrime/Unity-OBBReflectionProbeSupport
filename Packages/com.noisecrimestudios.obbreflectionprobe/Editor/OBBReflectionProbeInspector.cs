using NoiseCrimeStudios.Rendering.OBBProjectionProbe;
using UnityEditor;
using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionEditor
{
    [DisallowMultipleComponent]
	[CustomEditor( typeof( OBBReflectionProbe ) )]
    public class OBBReflectionProbeInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            OBBReflectionProbe obbReflectionProbe = ( OBBReflectionProbe )target;

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Update Shader Global Reflection Matrix" ) )
                obbReflectionProbe.RefreshShaderGlobals();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space( EditorGUIUtility.labelWidth );
            if ( GUILayout.Button( "Log Details To Console" ) )
                obbReflectionProbe.LogDetailsToConsole();
            GUILayout.EndHorizontal();
        }
    }
}