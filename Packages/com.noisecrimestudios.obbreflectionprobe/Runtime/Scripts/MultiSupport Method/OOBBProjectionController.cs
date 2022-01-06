using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	/// <summary>
	/// Provides functionality for Per Renderer OBB Reflection Probes.
	/// </summary>
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[RequireComponent( typeof( ReflectionProbe ) )]
	[AddComponentMenu( "Rendering/OOBB Probe Controller" )]
    public class OOBBProjectionController : MonoBehaviour
    {
		private static readonly int		obbProbeWorldToLocalID = Shader.PropertyToID( "_OBBProbeWorldToLocal" );

		#region Profiling
		private static ProfilerMarker	markerCalculateProbeMatrix  = new ProfilerMarker( "CalculateProbeMatrix" );
		private static ProfilerMarker	markerProbeMatrixToMaterial = new ProfilerMarker( "ProbeMatrixToMaterial" );
		#endregion


        [SerializeField, Tooltip("Accounts for box offset in matrix - always required?") ]
		private bool					includeBoxOffset		= true;			// ToDo: Remove this if always needed.
		[SerializeField, HideInInspector, Tooltip("Forces MaterialPropertyBlock to always update on every call - useful for performance debugging only")] 
		private	bool					forceAlwaysUpdate		= false;
				
		[SerializeField, HideInInspector]
		private Matrix4x4				probeWorldToLocalMatrix;
	
		private ReflectionProbe			probe;
		private	MaterialPropertyBlock	materialPropertyBlock;
       
		// Stores lastFrameCount the matrix was calculated. 
		// Used to avoid re-calculating the matrix/materialPropertyBlock multiple times per frame.
		private int						lastFrameCalculation = -1;

		private int						numCallsPerFrame = 0;

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

		/*
		// This has potential to avoid manual recalculation by the developer.
		// However as an Update method it adds some unnecessary overhead even to just the editor.
		// It would also need to send an update to all renderers - which will be slow.
		// Not even sure this is a good idea for say dynamic reflection Probes, as we have no reliiable method
		// to cache references to renderers that need to be updated.
#if UNITY_EDITOR && OOBB_EDITOR_REFRESH
		private void Update()
		{
			if ( transform.hasChanged )
				GetMaterialPropertyBlock( true );
		}
#endif
		*/
#endregion

	
		/// <summary>
		/// Calculates Probes worldToLocal Matrix if its not been cached this frame.
		/// </summary>
		/// <remarks>
		/// Could improve this with more fine grain checks against mode type and slicing.
		/// Notes:
		/// Cannot rely on OnEnable() as the call order between this class and OBBRenderer's in the scene is arbitrary!
		/// Therefore trying to set a bool or value to force recalculation say from a assembly domain reload is a point of failure.
		/// Luckily materialPropertyBlock is a class which means we can catch it being null after an assembly domain reload!
		/// </remarks>
		public MaterialPropertyBlock GetMaterialPropertyBlock( bool forceUpdate )
		{
			using ( markerProbeMatrixToMaterial.Auto() )
			{
				bool recalculate = 
					null == materialPropertyBlock || forceAlwaysUpdate || 
					( ( forceUpdate || Probe.mode != ReflectionProbeMode.Baked) && lastFrameCalculation != Time.frameCount );

				if ( OOBBProjectionSettings.IsLoggingEnabled )
				{
					Debug.Log(
						$"[{Time.frameCount}] OBBReflectionProbeController [{name}] ProbeMaterialPropertyBlock  {( recalculate ? "Calculate" : "Use Cache" )}\n" +
						$"lastFrameCalculation: {lastFrameCalculation}  forceUpdatesPerFrame: {forceAlwaysUpdate}  Probe.mode: {Probe.mode}\n" +
						$"probeWorldToLocalMatrix: {OOBBHelpers.MatrixLongString( probeWorldToLocalMatrix )}\n" +
						$"materialPropertyBlock: { ( null == materialPropertyBlock ? "Is Null" : OOBBHelpers.MatrixLongString( materialPropertyBlock.GetMatrix( obbProbeWorldToLocalID ) ) )}", this );
				}

				if ( recalculate )
				{
					CalculateProbeWorldToLocalMatrix();
					lastFrameCalculation = Time.frameCount;
					OOBBProjectionSettings.NumCallsToProbes = 0;
				}

				if ( lastFrameCalculation == Time.frameCount )
					OOBBProjectionSettings.NumCallsToProbes++;
			}

			return materialPropertyBlock;
		}


		private void CalculateProbeWorldToLocalMatrix()
		{
			using ( markerCalculateProbeMatrix.Auto() )
			{
				Vector3 position = transform.position;
				Vector3 scale    = Probe.size * 0.5f;

				if ( includeBoxOffset )
					position = transform.position + ( Vector3 )( transform.localToWorldMatrix * Probe.center );

				probeWorldToLocalMatrix = Matrix4x4.TRS( position, transform.rotation, scale ).inverse;

				if ( null == materialPropertyBlock )
					materialPropertyBlock = new MaterialPropertyBlock();

				materialPropertyBlock.SetMatrix( obbProbeWorldToLocalID, probeWorldToLocalMatrix );
			}

			if ( OOBBProjectionSettings.IsLoggingEnabled )
				Debug.Log( 
					$"[{Time.frameCount}] OBBReflectionProbeController [{name}] CalculateProbeWorldToLocalMatrix\n" +
					$"lastFrameCalculation: {lastFrameCalculation}  forceUpdatesPerFrame: {forceAlwaysUpdate}  Probe.mode: {Probe.mode}\n" +
					$"probeWorldToLocalMatrix: {OOBBHelpers.MatrixLongString(probeWorldToLocalMatrix)}\n" +
					$"materialPropertyBlock: { ( null == materialPropertyBlock ? "Is Null" : OOBBHelpers.MatrixLongString(materialPropertyBlock.GetMatrix(obbProbeWorldToLocalID)) )}", this);
		}				



		public void LogDetailsToConsole()
		{	
			string results = $"{name} - Information\n";

			results += $"numCallsPerFrame: {numCallsPerFrame}\n";
	
			Debug.Log( results );
		}		
    }
}