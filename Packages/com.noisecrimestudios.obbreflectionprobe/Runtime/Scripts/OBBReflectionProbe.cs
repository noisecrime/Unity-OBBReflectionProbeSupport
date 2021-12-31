// #define INCLUDE_HEIRARCHY_SCALING

using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Passes our custom ObjectBoundingBox transform as global Shader Proeprty
	/// </summary>
	/// <remarks>
	/// No simple means to tie this into the reflection probe to update when its changed. 
	/// We can't derive as ReflectionProbe is sealed.
	/// Taken the decision to have to call RefreshShaderGlobals
	/// References:
	/// UnityCsReference-2019.4\Editor\Mono\Inspector\ReflectionProbeEditor.cs
	/// UnityCsReference-2019.4\Runtime\Export\Camera\ReflectionProbe.bindings.cs
	/// </remarks>
	[ExecuteInEditMode]
	[RequireComponent( typeof( ReflectionProbe ) )]
	public class OBBReflectionProbe : MonoBehaviour
	{		
		[SerializeField, Tooltip("Accounts for box offset in matrix - always required?") ]
		private bool            includeBoxOffset = true;

#if INCLUDE_HEIRARCHY_SCALING
		[SerializeField, Tooltip("Accounts for heirarchy scaling in matrix - not sure if this is useful or ever needed") ]
		private bool            includeHierarchyScaling = false;
#endif

		private readonly int    boxProbeWorldToLocalID = Shader.PropertyToID( "_BoxProbeWorldToLocal" );

		private ReflectionProbe probe;

		protected ReflectionProbe Probe
		{
			get
			{
				if ( null == probe )
					probe = GetComponent<ReflectionProbe>();
				return probe;
			}
		}

		/* 
		// Disabled 
		// OnReflectionProbeEvent only gets called when a Probe iscreated or destroyed so not useful.
		private void Start()
		{			
			ReflectionProbe.reflectionProbeChanged += OnReflectionProbeEvent;	
		}
		
		private void OnDestroy()
		{
			ReflectionProbe.reflectionProbeChanged -= OnReflectionProbeEvent;
		}
		*/

		private void OnEnable()
		{
			RefreshShaderGlobals( Probe );	
		}

		public void RefreshShaderGlobals()
		{
			RefreshShaderGlobals( Probe );
		}


		/// <summary>
		/// Calculate the worldToLocal matrix for the probe with respect to performing OBB Reflection projection.
		/// </summary>
		/// <remarks>
		/// Probe is localspace. Probe.bounds is worldspace.
		/// We cannot perform this in the shader as it will not have the Probes WorldToLocal matrix only the objects!
		/// 
		/// boxProbeWorldToLocalID can be built as
		/// Matrix4x4 scaleMatrixInverse	= Matrix4x4.Scale( probe.size * 0.5f ).inverse;
		/// Matrix4x4 trs					= Matrix4x4.TRS( position, transform.rotation, Vector3.one );
		/// Matrix4x4 trsInverse			= trs.inverse * scaleMatrixInverse;		/// 
		/// or
		/// Matrix4x4 boxProbeWorldToLocal  = transform.worldToLocalMatrix * Matrix4x4.Scale( probe.size * 0.5f ).inverse;
		/// 
		/// Where ScaleMatrix inverse is same as 
		/// Matrix4x4(1/_BoxProbeHalfSize.x, 0, 0, 0, 0, 1/_BoxProbeHalfSize.y, 0, 0, 0, 0, 1/_BoxProbeHalfSize.z, 0, 0, 0, 0, 1 );
        /// </remarks>
		private void RefreshShaderGlobals( ReflectionProbe probe )
		{			
			Vector3 position = transform.position;
			Vector3 scale	 = probe.size * 0.5f; // probe.bounds.size * 0.5f;
						
			if ( includeBoxOffset )
				position = transform.position + (Vector3)(transform.localToWorldMatrix * probe.center);

#if INCLUDE_HEIRARCHY_SCALING
			if ( includeHierarchyScaling ) // or parent worldMatrix * local matrix
				scale = new Vector3( transform.lossyScale.x * scale.x, transform.lossyScale.y * scale.y, transform.lossyScale.z * scale.z );
#endif

		//	Debug.Log( $"Probe: {probe.name} {probe.center} {probe.size}  Bounds: {probe.bounds.center}  {probe.bounds.size}");
		//	Debug.Log( $"Transform: {transform.position} LocalToWorld: {position}  includeBoxOffset: {transform.position + probe.center}");

			Matrix4x4 trs = Matrix4x4.TRS( position, transform.rotation, scale );
			Shader.SetGlobalMatrix( boxProbeWorldToLocalID, trs.inverse );
		}		



		/// <summary>
		/// Callback from Unity - would be useful but literally only called when Probe is added/removed from scene.
		/// </summary>
		private void OnReflectionProbeEvent( ReflectionProbe probe, ReflectionProbe.ReflectionProbeEvent eventType )
		{
			if ( eventType == ReflectionProbe.ReflectionProbeEvent.ReflectionProbeAdded )
			{
				Debug.Log( $"ReflectionProbe {probe.name} Added - Set WorldToLocal" );
				RefreshShaderGlobals( probe );
			}
		}
		

		private void OnDrawGizmosSelected()
		{
			Color oldColor = Gizmos.color;
			Gizmos.matrix = Matrix4x4.TRS( probe.transform.position, probe.transform.rotation, Vector3.one );
			Gizmos.color = new Color( 1f, 0.5f, 1f, 0.75F );
			Gizmos.DrawWireCube( probe.center, probe.size );
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = oldColor;
		}

		public void LogDetailsToConsole()
		{			
			Matrix4x4 boxProbeWorldToLocal0 = Shader.GetGlobalMatrix( boxProbeWorldToLocalID );		
			Matrix4x4 scaleMatrix			= Matrix4x4.Scale( probe.size * 0.5f );
			Matrix4x4 boxProbeWorldToLocal1	= transform.worldToLocalMatrix * scaleMatrix.inverse;

			string results =
				$"Probe: {probe.name} {probe.center} {probe.size}  Bounds: {probe.bounds.center}  {probe.bounds.size}\n" +	
				$"scaleMatrix:\n{ToLongString(scaleMatrix)}\n" +
				$"scaleMatrixInverse:\n{ToLongString(scaleMatrix.inverse)}\n" +
				$"boxProbeWorldToLocal1:\n{ToLongString(boxProbeWorldToLocal1)}\n" +
				$"boxProbeLocalToWorld1:\n{ToLongString(boxProbeWorldToLocal1.inverse)}\n" +				
				$"boxProbeWorldToLocal0:\n{ToLongString(boxProbeWorldToLocal0)}\n" +
				$"boxProbeLocalToWorld0:\n{ToLongString(boxProbeWorldToLocal0.inverse)}\n" +		
				$"worldToLocalMatrix:\n{ToLongString(transform.worldToLocalMatrix)}\n" +
				$"localToWorldMatrix:\n{ToLongString(transform.localToWorldMatrix)}\n" +
				$"Local: {transform.localPosition} {transform.localRotation.eulerAngles} {transform.localScale}\n" +
				$"World: {transform.position} {transform.rotation.eulerAngles} {transform.lossyScale}\n";
			
			Debug.Log( results );
			Debug.Log( boxProbeWorldToLocal0 == boxProbeWorldToLocal1 );
		}

		/// <summary>Returns a single line string of all matrix components</summary>
		public static string ToLongString(  Matrix4x4 matrix )
		{
			return string.Format( "{0:F4}, {1:F4}, {2:F4}, {3:F4},  {4:F4}, {5:F4}, {6:F4}, {7:F4},  {8:F4}, {9:F4}, {10:F4}, {11:F4},  {12:F4}, {13:F4}, {14:F4}, {15:F4}",
				matrix.m00, matrix.m01, matrix.m02, matrix.m03, matrix.m10, matrix.m11, matrix.m12, matrix.m13, matrix.m20, matrix.m21, matrix.m22, matrix.m23, matrix.m30, matrix.m31, matrix.m32, matrix.m33 );
		}
	}
}