using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Passes probe worldToLocal matrix as global Shader Property - Realtime.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent( typeof( ReflectionProbe ) )]
	[AddComponentMenu( "Rendering/OOBB Probe Scene Global Realtime" )]
	public class OOBBSceneGlobalProbeRealtime : OOBBSceneGlobalProbeBase
	{	
		private void OnEnable()
		{
			RefreshShaderGlobals( Probe );	
		}

		void Update()
		{
			if ( transform.hasChanged )
				RefreshShaderGlobals( Probe );
		}
	}
	
}
