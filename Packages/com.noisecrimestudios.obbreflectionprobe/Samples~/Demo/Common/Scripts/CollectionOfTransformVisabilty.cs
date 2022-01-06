using System.Collections.Generic;
using UnityEngine;
using System;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
    /// <summary>
    /// Custom Collection of Transforms for Toggling Scene Visability.
    /// </summary>
    [Serializable]
    public class CollectionOfTransformVisabilty
    {
        [Serializable]
        public struct VisabilityGroup
        {
            public string           _name;
            public bool             _isExpanded;    // For reorderableList.
            public Transform        _root;          // All transforms below root will be toggles off.
            public List<Transform>  _items;         // List of trasnforms to toggle on.
            public int              _levels;        // Number of child level to recruse
        }
        
        public List<VisabilityGroup>  collection = new List<VisabilityGroup>();

        public int hiearchyLevels       = 3;
        private int activeGroupIndex    = -1;
       
        public void AddCollection( string name, bool isExpanded, int levels, Transform root, List<Transform> items )
        {
            collection.Add( new VisabilityGroup()
            {
                _name        = name,
                _isExpanded  = isExpanded,
                _root        = root,
                _items       = items,
                _levels      = levels
            } );
        }

        public Component GetComponentFromGroup( VisabilityGroup group, Type type )
		{
            // Debug.Log( "GetComponentFromGroup: " + group._name );

            foreach ( Transform obj in group._items )
            {
                Component c = obj.GetComponentInChildren( type );

                if ( null != c )
                    return c;
            }

            return null;
		}

        public Component GetComponentFromActiveGroup( Type type )
		{
            if ( activeGroupIndex < 0 || activeGroupIndex >= collection.Count )
                return null;

            VisabilityGroup group = collection[activeGroupIndex];

            return GetComponentFromGroup( group, type );
		}
        

		public void HideAllCollections()
        {
            foreach ( VisabilityGroup collection in collection )
			{
                if ( null != collection._root )
                {
                    HideRecrusive( collection._root, collection._levels );

                   // for ( int i = 0; i < collection._root.childCount; i++ )
                   //    collection._root.GetChild( i ).gameObject.SetActive( false );

                    collection._root.gameObject.SetActive( false );
                }
			}
        }

        public void HideRecrusive( Transform parent, int levels )
        {
            levels--;

            for ( int i = 0; i < parent.childCount; i++ )
            {
                Transform child = parent.GetChild( i );
                             
                if ( levels > 0 && child.childCount > 0 )
                    HideRecrusive( child, levels );
                
                child.gameObject.SetActive( false );
            }
        }


        public void ShowCollection( VisabilityGroup group )
		{
            if ( null == group._root )
            {
                Debug.LogWarning( $"ShowCollection: Invalid Root in Collection {group._name}");
                return;
            }

            HideAllCollections();

            group._root.gameObject.SetActive( true );

            foreach ( Transform obj in group._items )
            {
                if ( null != obj )
                    obj.gameObject.SetActive( true );
                else
                    Debug.LogWarning( $"ShowCollection: Invalid Transform in Collection {group._name} of {group._root}" );
            }
		}

        public void ShowCollection( int index )
		{           
            if ( index < 0 || index >= collection.Count )
                return;

            activeGroupIndex = index;

            ShowCollection( collection[index] );
		}
    }
}