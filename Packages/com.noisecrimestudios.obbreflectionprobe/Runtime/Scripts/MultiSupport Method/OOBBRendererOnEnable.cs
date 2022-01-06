using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Renderer supporting OBB Reflection Probe - Static (OnEnable)
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu( "Rendering/OOBB Probe Renderer OnEnable" )]
	public class OOBBRendererOnEnable : OOBBRenderBase
    {
		#region Unity Methods

		private void OnEnable()
		{
			RequestProbeMatrix( RequstPurpose.OnEnable );
		}

		#endregion
	}
}