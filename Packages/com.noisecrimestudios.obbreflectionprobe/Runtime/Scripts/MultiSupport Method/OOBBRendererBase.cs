using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Base class for Renderer supporting OBB Reflection Probe - requests Probe Matrix for its material.
	/// </summary>
	/// <remarks>
	/// Only supports single material, no sub materials.
	/// Base class for use with derived classes that specify when or how frequently to update matrix.
	/// </remarks>
	[DisallowMultipleComponent]
	public class OOBBRenderBase : MonoBehaviour
    {
		#region Profiling
		private static ProfilerMarker markerRequestProbeMatrix = new ProfilerMarker( "RequestProbeMatrix" );
		#endregion

		// For debugging we track from where updating probe matrix requests originate.
		public enum RequstPurpose { None, BecameVisible, OnEnable, OnValidate, Update, UpdateAll, WillRenderObject};

		// Static list for storing in ClosestReflectionProbes results for all renderers.
		// This might be an issue if threaded! - Move this into ProbeController?
		static List<ReflectionProbeBlendInfo>	probes = new List<ReflectionProbeBlendInfo>();
		


		#region Unity Methods

		// Update is used for performance testing only
		// private void Update()	 { RequestProbeMatrix( RequstPurpose.Update ); }

		// OnValidate doesn't seem to have any point here.
		// private void OnValidate() { RequestProbeMatrix( RequstPurpose.OnValidate ); }

		#endregion



		public void RequestProbeMatrix( RequstPurpose purpose, bool forceUpdate = false)
		{
			if ( OOBBProjectionSettings.IsLoggingEnabled )
				Debug.Log( $"[{Time.frameCount}] {name}: RequestProbeMatrix: {purpose}" );

			using ( markerRequestProbeMatrix.Auto() )
			{
				Renderer renderer = GetComponent<Renderer>();

				renderer.GetClosestReflectionProbes( probes );

				if ( probes.Count > 0 )
				{
					OOBBProjectionController controller = probes[ 0 ].probe.GetComponent<OOBBProjectionController>();

					if ( null != controller )
						renderer.SetPropertyBlock( controller.GetMaterialPropertyBlock( forceUpdate ) );
					else
						Debug.LogError( $"[{Time.frameCount}] RequestProbeMatrix: Cannot find OBBReflectionProbeController of probe [{probes[ 0 ].probe.name}] for [{name}] via {purpose}", this );
				}
			}
		}


		[ContextMenu("LogDetailsToConsole")]
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
			results += $"Renderer: probeAnchor: {(renderer.probeAnchor != null ? renderer.probeAnchor.name : "None")}\n";
			results += $"Renderer: reflectionProbeUsage: {renderer.reflectionProbeUsage}\n";
			
			Debug.Log( results );
		}
	}
}