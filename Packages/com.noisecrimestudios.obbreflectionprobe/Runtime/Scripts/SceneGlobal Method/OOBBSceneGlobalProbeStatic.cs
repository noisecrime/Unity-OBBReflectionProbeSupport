using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Passes probe worldToLocal matrix as global Shader Property - Static (OnEnable).
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent( typeof( ReflectionProbe ) )]
	[AddComponentMenu( "Rendering/OOBB Probe Scene Global Static" )]
	public class OOBBSceneGlobalProbeStatic : OOBBSceneGlobalProbeBase
	{
		private void OnEnable()
		{
			RefreshShaderGlobals( Probe );	
		}	
	}
	
}
