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
        public static void Bake( ReflectionProbe reflectionProbe, int resolutionIndex, bool useDefaultBakedTexture = true)
        {
            if ( resolutionIndex >= 0 && resolutionIndex < 8 )
                reflectionProbe.resolution = (int)Mathf.Pow( 2, resolutionIndex + 4 );

            string path;

            if ( useDefaultBakedTexture )
                path = AssetDatabase.GetAssetPath(reflectionProbe.bakedTexture);
            else
                path = AssetDatabase.GetAssetPath(reflectionProbe.customBakedTexture);

            string targetExtension = reflectionProbe.hdr ? "exr" : "png";

            if ( string.IsNullOrEmpty( path ) || Path.GetExtension( path ) != "." + targetExtension )
            {
                // We use the path of the active scene as the target path
                string targetPath = Path.GetDirectoryName( SceneManager.GetActiveScene().path);

                if ( string.IsNullOrEmpty( targetPath ) )
                    targetPath = "Assets";
                else if ( Directory.Exists( targetPath ) == false )
                    Directory.CreateDirectory( targetPath );

                string fileName = reflectionProbe.name + (reflectionProbe.hdr ? "-reflectionHDR" : "-reflection") + "." + targetExtension;
                path = AssetDatabase.GenerateUniqueAssetPath( Path.Combine( targetPath, fileName ) );                
            }

            if ( !string.IsNullOrEmpty( path ) )
            {
                Lightmapping.BakeReflectionProbe( reflectionProbe, path );
                Debug.Log( "ReflectionProbeForcedBaker: " + path );
            }
        }

    }
}