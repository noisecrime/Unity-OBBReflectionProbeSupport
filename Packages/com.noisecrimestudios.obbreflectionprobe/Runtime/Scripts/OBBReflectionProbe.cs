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
			Gizmos.color = new Color( 1f, 0.5f, 0f, 0.75F );
			Gizmos.DrawWireCube( probe.center, probe.size );
			Gizmos.matrix = Matrix4x4.identity;
			Gizmos.color = oldColor;
		}


		/*
		[ContextMenu( "Update WorldToLocalMatrix " )]
		private void UpdateWorldToLocalMatrix()
		{
			RefreshShaderGlobals( Probe );
		}
		
		[ContextMenu( "Log Matrices To Console" )]
		*/

		public void LogDetailsToConsole()
		{
			string results =
			$"Probe: {probe.name} {probe.center} {probe.size}  Bounds: {probe.bounds.center}  {probe.bounds.size}\n" +
			$"Local: {transform.localPosition} {transform.localRotation.eulerAngles} {transform.localScale}\n" +
			$"World: {transform.position} {transform.rotation.eulerAngles} {transform.lossyScale}\n" +
			"worldToLocalMatrix\n" + transform.worldToLocalMatrix + "\n" +
			"localToWorldMatrix\n" + transform.localToWorldMatrix ;

			Debug.Log( results );
		}
	}
}