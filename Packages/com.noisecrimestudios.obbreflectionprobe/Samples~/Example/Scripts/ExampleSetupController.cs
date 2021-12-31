using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	/// <summary>
	/// Behaviour that can be used to toggle various scene setups of active transforms/GameObjects.
	/// </summary>
	public class ExampleSetupController : MonoBehaviour
    {
        public CollectionOfCameraPlacement      cameraCollection = new CollectionOfCameraPlacement();
        public CollectionOfTransformVisabilty   visabilityCollection = new CollectionOfTransformVisabilty();
	}
}