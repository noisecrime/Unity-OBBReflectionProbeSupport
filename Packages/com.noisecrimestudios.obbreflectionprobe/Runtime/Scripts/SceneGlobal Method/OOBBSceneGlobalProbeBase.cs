using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Base class of passes probe worldToLocal matrix as global Shader Property.
	/// </summary>	
	[RequireComponent( typeof( ReflectionProbe ) )]   
	public class OOBBSceneGlobalProbeBase : MonoBehaviour
	{
		private static readonly int	probeWorldToLocalID = Shader.PropertyToID( "_OBBProbeWorldToLocal" );

		[SerializeField, Tooltip("Accounts for box offset in matrix - always required?") ]
		private bool				includeBoxOffset = true;
				
		protected ReflectionProbe	probe;


		#region Getters
		/// <summary>
		/// On demand access to the Relfection Probe reference.
		/// </summary>
		protected ReflectionProbe Probe
		{
			get
			{
				if ( null == probe )
					probe = GetComponent<ReflectionProbe>();
				return probe;
			}
		}
		#endregion



		#region Unity Methods		
		private void OnDrawGizmosSelected()
		{
			Color oldColor = Gizmos.color;
			Gizmos.matrix =  Matrix4x4.TRS( transform.position, transform.rotation, Vector3.one );
			Gizmos.color = new Color( 1f, 0.5f, 1f, 0.75f );
			Gizmos.DrawWireCube( Probe.center, Probe.size );
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = oldColor;
		}
		#endregion


		public void RefreshShaderGlobals()
		{
			RefreshShaderGlobals( Probe );
		}

		/// <summary>
		/// Calculate the worldToLocal matrix for the probe for performing OOBB projection.
		/// </summary>
		protected void RefreshShaderGlobals( ReflectionProbe probe )
		{			
			Vector3 position = transform.position;
			Vector3 scale	 = probe.size * 0.5f; // probe.bounds.size * 0.5f;
						
			if ( includeBoxOffset )
				position = transform.position + (Vector3)(transform.localToWorldMatrix * probe.center);

			Matrix4x4 trs = Matrix4x4.TRS( position, transform.rotation, scale );

			Shader.SetGlobalMatrix( probeWorldToLocalID, trs.inverse );

			// Tests for fixing deferred rendering.
			// Shader.SetGlobalMatrix( "_BoxProbeMatrix", transform.localToWorldMatrix); 
			// Shader.SetGlobalMatrix( "_BoxProbeMatrix", Matrix4x4.TRS( transform.position, transform.rotation, Vector3.one ) );
			// Shader.SetGlobalMatrix( "_BoxProbeMatrix", Matrix4x4.TRS( Vector3.zero, transform.rotation, Vector3.one ) );
			// Shader.SetGlobalMatrix( "_BoxProbeMatrix", Matrix4x4.TRS( Vector3.zero, Quaternion.identity, Vector3.one * 1.2f ) );
		}		


		public void LogDetailsToConsole()
		{			
			Matrix4x4 probeWorldToLocal0	= Shader.GetGlobalMatrix( probeWorldToLocalID );		
			Matrix4x4 scaleMatrix			= Matrix4x4.Scale( probe.size * 0.5f );
			Matrix4x4 probeWorldToLocal1	= transform.worldToLocalMatrix * scaleMatrix.inverse;

			string results =
				$"Probe: {probe.name} {probe.center} {probe.size}\n" + 
				$"Bound: {probe.bounds.center} {probe.bounds.size}\n" +	
				$"scaleMatrix:\t\n{OOBBHelpers.MatrixLongString(scaleMatrix)}\n" +
				$"scaleMatrixInverse:\n{OOBBHelpers.MatrixLongString(scaleMatrix.inverse)}\n" +
				$"probeWorldToLocal1:\n{OOBBHelpers.MatrixLongString(probeWorldToLocal1)}\n" +
				$"probeLocalToWorld1:\n{OOBBHelpers.MatrixLongString(probeWorldToLocal1.inverse)}\n" +				
				$"probeWorldToLocal0:\n{OOBBHelpers.MatrixLongString(probeWorldToLocal0)}\n" +
				$"probeLocalToWorld0:\n{OOBBHelpers.MatrixLongString(probeWorldToLocal0.inverse)}\n" +		
				$"worldToLocalMatrix:\n{OOBBHelpers.MatrixLongString(transform.worldToLocalMatrix)}\n" +
				$"localToWorldMatrix:\n{OOBBHelpers.MatrixLongString(transform.localToWorldMatrix)}\n" +
				$"Local: {transform.localPosition} {transform.localRotation.eulerAngles} {transform.localScale}\n" +
				$"World: {transform.position} {transform.rotation.eulerAngles} {transform.lossyScale}\n";
			
			Debug.Log( results );
		}
	}
}