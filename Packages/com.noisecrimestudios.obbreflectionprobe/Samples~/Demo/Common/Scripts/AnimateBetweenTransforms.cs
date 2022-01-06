using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	public class AnimateBetweenTransforms : MonoBehaviour
    {
		public Transform[]  points;
		public float		durationBetwenPoints = 2f;

		private int			pointIndex		= 0;
		private float		lastTimeLoop	= 0;

		private void Update()
		{
			float ratio = ( Time.fixedUnscaledTime % durationBetwenPoints ) / durationBetwenPoints;

			if ( Time.fixedUnscaledTime / durationBetwenPoints > lastTimeLoop )
			{
				lastTimeLoop++;
				pointIndex = ( pointIndex + 1 ) % points.Length;
			}

			transform.position = Vector3.Lerp( points[pointIndex].position, points[(pointIndex + 1 ) % points.Length].position, ratio );
		}

	}
}