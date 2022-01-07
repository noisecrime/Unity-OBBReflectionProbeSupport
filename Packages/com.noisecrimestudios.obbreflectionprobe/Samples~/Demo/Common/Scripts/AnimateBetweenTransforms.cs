using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	public class AnimateBetweenTransforms : MonoBehaviour
    {
		[SerializeField] float			durationBetwenPoints	= 2f;
		[SerializeField] bool			useDeltaFrame			= true;
		[SerializeField] Transform[]	points;


		private int		pointIndex		= 0;
		private float	lastTimeLoop	= 0;
		private float	period			= 0;


		private void Update()
		{
			if ( useDeltaFrame )
			{
				period += Time.deltaTime;

				if ( period > durationBetwenPoints )
				{
					period %= durationBetwenPoints;					
					pointIndex = ( pointIndex + 1 ) % points.Length;
				}

				transform.position = Vector3.Lerp( points[ pointIndex ].position, points[ ( pointIndex + 1 ) % points.Length ].position, period/durationBetwenPoints );
			}
			else
			{
				float ratio = ( Time.realtimeSinceStartup % durationBetwenPoints ) / durationBetwenPoints;

				if ( Time.realtimeSinceStartup / durationBetwenPoints > lastTimeLoop )
				{
					lastTimeLoop++;
					pointIndex = ( pointIndex + 1 ) % points.Length;
				}

				transform.position = Vector3.Lerp( points[ pointIndex ].position, points[ ( pointIndex + 1 ) % points.Length ].position, ratio );
			}
		}

	}
}