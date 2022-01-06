using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	public class AutoRotate : MonoBehaviour
    {
        void Update()
        {
            transform.Rotate( 0.2f, -0.3f, 0.4f);
        }
    }
}