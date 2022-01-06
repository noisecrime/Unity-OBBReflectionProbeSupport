using System.Collections.Generic;
using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
    /// <summary>
    /// Custom Collection of CameraPlacements.
    /// Assigns a transform (of a camera) to a specific position & Oreintation.
    /// </summary>
	[System.Serializable]
    public class CollectionOfCameraPlacement
    {
        public enum Placement 
        { 
            None,
            Straight,
            Angled
        }
        
        [System.Serializable]
        public class CameraPlacement
        {
            public Placement    _placement;
            public Vector3      _localPosition;
            public Quaternion   _localRotation;
        }
        
        public List<CameraPlacement> collection = new List<CameraPlacement>();
       
        /// <summary>
        /// Find Camera in collection by Placement name.
        /// </summary>
        public CameraPlacement FindCameraByPlacement( Placement placement)
		{
            foreach( CameraPlacement t in collection )
                if ( t._placement == placement ) return t;

            return null;
		}


        /// <summary>
        /// Given a list of Transforms - find first one with a Camera and set its transform to one from the collection.
        /// </summary>
        public void SetCollectionCameraPlacement( List<Transform> items, Placement placement  )
		{
            CameraPlacement cp = FindCameraByPlacement( placement );

            // Find in list of items one with a camera and set its transform
            foreach ( Transform t in items )
                if ( t.TryGetComponent<Camera>( out Camera camera ) )
                {
                    camera.transform.localPosition = cp._localPosition;
                    camera.transform.localRotation = cp._localRotation;
                    break;
                }
		}

        public void AddCameraToTransforms( GameObject obj, Placement placement )
		{
            if ( obj.GetComponent<Camera>() != null )
                collection.Add( new CameraPlacement()
                {
                    _placement      = placement,
                    _localPosition  = obj.transform.localPosition,
                    _localRotation  = obj.transform.localRotation
                } );
		}

        public void AddCameraToTransforms( Transform transform, Placement placement )
		{
            if ( transform.GetComponent<Camera>() != null )
                collection.Add( new CameraPlacement()
                {
                    _placement      = placement,
                    _localPosition  = transform.localPosition,
                    _localRotation  = transform.localRotation
                } );
		}
    }
}