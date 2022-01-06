using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NoiseCrimeStudios.Rendering.OBBProjectionProbe;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe.Experimental
{
    /// <summary>
    /// WIP - Controller for calculating and caching a Reflection Probe OOBB WorldToLocal Matrix.
    /// </summary>
	[ExecuteInEditMode]
	[RequireComponent( typeof( ReflectionProbe ) )]
	[AddComponentMenu( "Experimental/Rendering/Exp - OOBB Controller" )]
	[System.Obsolete("Not Obsolete but experimental & work in progress - DO NOT USE")]
    public class OOBBReflectionProbeController : MonoBehaviour
    {
#pragma warning disable 0414
		[SerializeField, Tooltip("Accounts for box offset in matrix - always required?") ]
		private bool					includeBoxOffset		= true;
		[SerializeField] 
		private	bool					forceUpdatesPerFrame	= false;
        	
        [SerializeField, HideInInspector]
		private Matrix4x4				probeWorldToLocalMatrix;

        [SerializeField, HideInInspector]
        private ReflectionProbe			probe;
		
#pragma warning restore 0414


		#region Getters & Setters
		/// <summary>OnDemand Reference to the reflection Probe.</summary>
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



		#region Callbacks & Events
		private void OnSetDefaultReflection(Cubemap cubemap)
        {	
			if ( OOBBProjectionSettings.IsLoggingEnabled )
				Debug.Log( $"[{Time.frameCount}] OBBReflectionProbeController [{name}] Default reflection cubemap was changed", this );
        }
		#endregion


		#region Unity Methods
		void Start()
        {	
			if ( OOBBProjectionSettings.IsLoggingEnabled )
				Debug.Log( $"[{Time.frameCount}] OBBReflectionProbeController [{name}] Start", this );
		
            ReflectionProbe.defaultReflectionSet += OnSetDefaultReflection;
        }

        void OnDestroy()
        {	
			if ( OOBBProjectionSettings.IsLoggingEnabled )
				Debug.Log( $"[{Time.frameCount}] OBBReflectionProbeController [{name}] Destroy", this );
		
            ReflectionProbe.defaultReflectionSet -= OnSetDefaultReflection;
        }
		#endregion

	}
}