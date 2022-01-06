using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe.Experimental
{
	/// <summary>
	/// Mangement System - Work In Progress.
	/// </summary>
	/// <remarks>
	/// Exploring how a management level system might work for assigning OBB Reflection Probe Matrices to Renderers.
	/// Problem is this approach is highly dependant upon the structure and approach of the Project it is used within.
	/// For example a Project that instantiates geometry say from prefabs is not going to work well.
	/// </remarks>
    [AddComponentMenu( "Experimental/Rendering/Exp - OOBB Manager" )]
    [System.Obsolete("Not Obsolete but experimental & work in progress - DO NOT USE")]
	public class OOBBRenderManagement : MonoBehaviour
    {
        private static List<Renderer> staticRenderers;
        private static List<Renderer> dynamicRenderers;

     
        // Note: in 2020.1 you can use FindObjectsByType( include inactive)

        private void FindRenderers()
        {
            staticRenderers = new List<Renderer>();
            dynamicRenderers = new List<Renderer>();

            Scene scene                         = SceneManager.GetActiveScene();
            List<GameObject> rootGameObjects    = new List<GameObject>(scene.rootCount);
            scene.GetRootGameObjects( rootGameObjects );

            foreach ( var root in rootGameObjects )
            {
                Renderer[] results = root.GetComponentsInChildren<Renderer>( true );

                foreach ( Renderer r in results )
                {
                    
                    if ( r.gameObject.isStatic )
                        staticRenderers.Add( r );
                    else
                        dynamicRenderers.Add( r );
                }
            }
        }
    }
}

/* Plan
 * Static renderers will not change so we can assign MPB once at stat up.
 * We can check for blended ones - blending OBB 2 OBB or AABB 2 AABB is easy but OBB 2 AABB???
 */