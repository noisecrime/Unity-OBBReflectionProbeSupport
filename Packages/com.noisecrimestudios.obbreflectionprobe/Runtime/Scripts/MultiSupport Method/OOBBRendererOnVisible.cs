using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Renderer supporting OBB Reflection Probe - Static (BecameVisible)
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu( "Rendering/OOBB Probe Renderer BecameVisible" )]
	public class OOBBRendererOnVisible : OOBBRenderBase
	{
		/// <summary>
		/// Request Probe Matrix update when the renderer becomes visible in scene.
		/// </summary>
		private void OnBecameVisible()
		{
			RequestProbeMatrix( RequstPurpose.BecameVisible );
		}
	}
}