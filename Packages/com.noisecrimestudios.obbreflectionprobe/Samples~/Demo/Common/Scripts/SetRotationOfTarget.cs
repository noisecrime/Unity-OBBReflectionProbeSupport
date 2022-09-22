using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	public class SetRotationOfTarget : MonoBehaviour
    {
        public Transform    target;
        public Vector3      rotation;

        void OnEnable()
        {
            if ( null != target )
                target.localRotation = Quaternion.Euler( rotation );
        }
    }
}