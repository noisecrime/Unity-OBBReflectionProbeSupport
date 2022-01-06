using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe.Experimental
{
	[System.Obsolete("Not Obsolete but experimental & work in progress - DO NOT USE")]
	public static class ReflectionProbeExtensions
	{
		#region OOBB Extension
		// Testing out not needing a class for OOBBReflectionProbe and not neeing to add it to scene.
		// This would be best used for a realtime probe that is updating every frame.
		public struct OOBBProbeData
		{
			public Matrix4x4        worldToLocal;
			public int              lastFrameCount;
		}

		private static Dictionary<int, OOBBProbeData> oobbProbeMatrices;
				
		public static Matrix4x4 TrackOBBWorldToLocalMatrix( this ReflectionProbe probe, bool includeBoxOffset )
		{
			if ( null == oobbProbeMatrices )
				oobbProbeMatrices = new Dictionary<int, OOBBProbeData>();

			int id = probe.GetInstanceID();

			// Check for cached version
			if ( oobbProbeMatrices.TryGetValue( id, out OOBBProbeData data ) )
			{
				if ( probe.mode == ReflectionProbeMode.Baked || Time.frameCount == data.lastFrameCount )	
					return data.worldToLocal;
			}
			
			// Update cached version.		
			Vector3 position = probe.transform.position;
			Vector3 scale    = probe.size * 0.5f;

			if ( includeBoxOffset )
				position = probe.transform.position + ( Vector3 )( probe.transform.localToWorldMatrix * probe.center );

			data.worldToLocal	= Matrix4x4.TRS( position, probe.transform.rotation, scale ).inverse;			
			data.lastFrameCount = Time.frameCount;

			oobbProbeMatrices.Add( id, data );

			return data.worldToLocal;
		}

		
		public static void LogOBBReflectionProbes()
		{
			string results = "";

			foreach ( KeyValuePair<int, OOBBProbeData> kvp in oobbProbeMatrices )
			{
				string probeName = "Unknown";

#if UNITY_EDITOR
				Object obj = UnityEditor.EditorUtility.InstanceIDToObject(kvp.Key);

				if ( null != obj )
					probeName = obj.name;
#endif

				results += $"ID: [{kvp.Key}] [{probeName}] Frame: [{kvp.Value.lastFrameCount}]  {OOBBHelpers.MatrixLongString( kvp.Value.worldToLocal )}\n";
			}

			Debug.Log( results );
		}
		#endregion



		/// <summary>
		/// Extension: Returns the Probe OBB WorldToLocal Matrix.
		/// </summary>
		public static Matrix4x4 GetOBBWorldToLocalMatrix( this ReflectionProbe probe, bool includeBoxOffset )
		{
			Vector3 position = probe.transform.position;
			Vector3 scale    = probe.size * 0.5f;

			if ( includeBoxOffset )
				position = probe.transform.position + ( Vector3 )( probe.transform.localToWorldMatrix * probe.center );

			return Matrix4x4.TRS( position, probe.transform.rotation, scale ).inverse;
		}


	}
}