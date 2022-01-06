using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Renderer supporting OBB Reflection Probe - Realtime (WillRenderObject)
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu( "Rendering/OOBB Probe Renderer WillRender" )]
	public class OOBBRendererWillRender : OOBBRenderBase
	{
		/// <summary>
		/// Request Probe Matrix update every frame. Ideal for realtime probes.
		/// </summary>
		private void OnWillRenderObject()
		{
			RequestProbeMatrix( RequstPurpose.WillRenderObject, true );
		}
	}
}