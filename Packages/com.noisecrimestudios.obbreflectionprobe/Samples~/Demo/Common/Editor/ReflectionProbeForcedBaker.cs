using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

namespace NoiseCrimeStudios.Examples.OBBProjectionEditor
{
	/// <summary>
	/// Editor method to force a reflectionProbe to be baked.
	/// </summary>
	/// <remarks>
	/// https://forum.unity.com/threads/how-to-baking-light-probes-only.408490/#post-2662381
	/// 
	/// Runtime Editor Usage
	/// If you have never baked your custom reflection probe before and don't have any texture set on it,
	/// when you exit Play Mode you will loose the change on your reflection probe because in Play Mode
	/// a temporal copy of the scene is used.
	/// </remarks>
	public static class ReflectionProbeForcedBaker
    {
        public static void Bake( ReflectionProbe reflectionProbe, int resolutionIndex )
        {
            if ( resolutionIndex >= 0 && resolutionIndex < 8 )
                reflectionProbe.resolution = (int)Mathf.Pow( 2, resolutionIndex + 4 );

            string path = null;
            string targetExtension = reflectionProbe.hdr ? ".exr" : ".png";

            // Check if it is a customBakedTexture.
            if ( null != reflectionProbe.customBakedTexture )
                path = AssetDatabase.GetAssetPath( reflectionProbe.customBakedTexture );
            else if ( null != reflectionProbe.bakedTexture )
                path = AssetDatabase.GetAssetPath( reflectionProbe.bakedTexture ); 
            else
                Debug.LogWarning( $"No existing Reflection Probe file for bakedTexture or customBakedTexture." );


            if ( string.IsNullOrEmpty( path ) )
            {
                // We use the path of the active scene as the target path
                string sceneFilename    = SceneManager.GetActiveScene().path;
                string targetPath;

                if ( string.IsNullOrEmpty( sceneFilename ) )
                    targetPath = "Assets";
                else
                {
                    string scenePath    = Path.GetDirectoryName( sceneFilename );
                    string sceneName    = Path.GetFileNameWithoutExtension( sceneFilename );
                    targetPath          = Path.Combine( scenePath, sceneName );
                    Debug.Log( $"targetPath: {targetPath}\nsceneFilename: {sceneFilename}\nscenePath: {scenePath}\nsceneName: {sceneName}" );
                }

                if ( Directory.Exists( targetPath ) == false )
                    Directory.CreateDirectory( targetPath );

                string fileName = reflectionProbe.name + (reflectionProbe.hdr ? "-reflectionHDR" : "-reflection") + targetExtension;
                // string fileName = "ReflectionProbe-";

                path = AssetDatabase.GenerateUniqueAssetPath( Path.Combine( targetPath, fileName ) );
            }
            else if ( Path.GetExtension( path ) != targetExtension )
            {
                Debug.LogWarning( $"ReflectionProbe texture extension has changed from {Path.GetExtension( path )} to {targetExtension}" );
                path = Path.GetFileNameWithoutExtension( path ) + targetExtension;
            }

            if ( !string.IsNullOrEmpty( path ) )
            {
                Lightmapping.BakeReflectionProbe( reflectionProbe, path );
                Debug.Log( "ReflectionProbeForcedBaker: " + path );
            }
        }
    }
}