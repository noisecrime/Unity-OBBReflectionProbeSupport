using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe.Experimental
{
	/// <summary>
	/// Base class for Renderer supporting OBB Reflection Probe - requests Probe Matrix for its material.
	/// </summary>
	/// <remarks>
	/// Supports multiple materials and sub materials.
	/// </remarks>
	[ExecuteInEditMode]
	[System.Obsolete("Not Obsolete but experimental & work in progress - DO NOT USE")]
	public class OBBRendererMPB : MonoBehaviour
    {
		// For debugging we track from where updating probe matrix requests originate.
		public enum RequstPurpose { None, WillRenderObject, BecameVisible, OnEnable };

		// Cached OBB Reflection Probe Matrix for use in build.
		[SerializeField] Matrix4x4				cachedProbeWorldToLocalMatrix;

		// Static list for storing in ClosestReflectionProbes results for all renderers.
		// This would be an issue if threaded! - Move this into ProbeController?
		static List<ReflectionProbeBlendInfo>	probes = new List<ReflectionProbeBlendInfo>();

		// Static Material Property Block for reuse with all instances
		static MaterialPropertyBlock			materialPropertyBlock;

		static List<Material>					materials = new List<Material>();

		static readonly string					obbProbeWorldToLocalStr = "_OBBProbeWorldToLocal";
		static readonly int						obbProbeWorldToLocalID	= Shader.PropertyToID( obbProbeWorldToLocalStr );


		private void OnEnable()
		{			
			AssignProbeMatrixToRenderer();
			// RequestProbeMatrix( RequstPurpose.OnEnable );
		}

		protected void AssignProbeMatrixToRenderer()
		{
			Renderer renderer = GetComponent<Renderer>();

			// Fetch all Materials to check which, if any use OBB Standard Shader.
			renderer.GetSharedMaterials( materials );

			for( int i = 0; i < materials.Count; i++ )
			{
				// Does Shader use out '_OBBProbeWorldToLocal' property?
				if ( materials[i].shader.FindPropertyIndex( obbProbeWorldToLocalStr ) < 0 )
					continue;

				// Re-use any existing materialPropertyBlock.
				if ( renderer.HasPropertyBlock() )
					renderer.GetPropertyBlock( materialPropertyBlock, i);
				else
					materialPropertyBlock = new MaterialPropertyBlock();
				
				// Add our cached matrix and '_OBBProbeWorldToLocal' property to the materialPropertyBlock.
				materialPropertyBlock.SetMatrix( obbProbeWorldToLocalID, cachedProbeWorldToLocalMatrix );
				
				// Set the renderer to use the updated materialPropertyBlock.
				renderer.SetPropertyBlock( materialPropertyBlock );
			}
		}

		// So this is storing cahced matrix with each renderer - yet can it just be stored with the Probe?


		/*
		protected void RequestProbeMatrix( RequstPurpose purpose )
		{
			// Debug.Log( $"[{Time.frameCount}] {name}: RequestProbeMatrix: {purpose}" );

			Renderer renderer = GetComponent<Renderer>();

			renderer.GetClosestReflectionProbes( probes );

			if ( probes.Count > 0 )
				probes[ 0 ].probe.GetComponent<OOBBReflectionProbeController>().AssignProbeMatrixToMaterial( renderer );
		}
	*/

		public void LogDetailsToConsole()
		{	
			string results = $"{name} - Information\n";

			Renderer renderer = GetComponent<Renderer>();
			renderer.GetClosestReflectionProbes( probes );

			foreach( ReflectionProbeBlendInfo info in probes )
				results += $"Probe: {info.probe.name}  [Weight: {info.weight}]\n";

			results += $"Renderer: localToWorldMatrix: {OOBBHelpers.MatrixLongString(renderer.localToWorldMatrix)}\n";
			results += $"Renderer: isVisible: {renderer.isVisible}\n";
			results += $"Renderer: isPartOfStaticBatch: {renderer.isPartOfStaticBatch}\n";
			results += $"Renderer: probeAnchor: {renderer.probeAnchor.name}\n";
			results += $"Renderer: reflectionProbeUsage: {renderer.reflectionProbeUsage}\n";
			
			Debug.Log( results );
		}
	}
}