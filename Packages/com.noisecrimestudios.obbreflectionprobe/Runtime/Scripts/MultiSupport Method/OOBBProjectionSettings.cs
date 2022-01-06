using System;
using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	[Serializable, ExcludeFromPresetAttribute]
    [CreateAssetMenu(fileName = "OOBB Projection Settings", menuName = "NoiseCrimeStudios/OOBB Projection Settings", order = 1)]
    public class OOBBProjectionSettings : ScriptableObject
    {
        private static readonly string defaultSettings = "OOBB Projection Settings";

        private static OOBBProjectionSettings s_Instance;

        #region Singleton

        /// <summary>
        /// Get a singleton instance of the settings class.
        /// </summary>
        public static OOBBProjectionSettings instance
        {
            get
            {
                if ( OOBBProjectionSettings.s_Instance == null )
                {
                    OOBBProjectionSettings.s_Instance = Resources.Load<OOBBProjectionSettings>( defaultSettings );

                    /*
                    #if UNITY_EDITOR
                    // Make sure TextMesh Pro UPM packages resources have been added to the user project
                    if (OBBReflectionProbeSettings.s_Instance == null)
                    {
                        // Open TMP Resources Importer
                        TMP_PackageResourceImporterWindow.ShowPackageImporterWindow();
                    }
                    #endif
                    */
                }

                return OOBBProjectionSettings.s_Instance;
            }
        }

        /// <summary>
        /// Static Function to load the OBBReflectionProbeSettings file.
        /// </summary>
        public static OOBBProjectionSettings LoadDefaultSettings()
        {
            if ( s_Instance == null )
            {
                // Load settings from TMP_Settings file
                OOBBProjectionSettings settings = Resources.Load<OOBBProjectionSettings>(defaultSettings);

                if ( settings != null )
                    s_Instance = settings;
            }

            return s_Instance;
        }

        /// <summary>
        /// Returns the OBBReflectionProbeSettings file.
        /// </summary>
        public static OOBBProjectionSettings GetSettings()
        {
            if ( OOBBProjectionSettings.instance == null )
                return null;

            return OOBBProjectionSettings.instance;
        }

        #endregion


        /// <summary>
        /// Returns the release version of the product.
        /// </summary>
        public static string version { get { return "0.0.5"; } }


        /// <summary>
        /// Controls whether OBBReflectionProbe components log debug information to console.
        /// </summary>
        public static bool IsLoggingEnabled
        {
            get { return instance.enableLogging; }
        }
        [SerializeField]
        private bool enableLogging = false;



		#region Debug Information
		public static int NumCallsToProbes
        {
            get { return instance.numCallsToProbes; }
            set { instance.numCallsToProbes = value; }
        }
        [SerializeField]
        private int numCallsToProbes = 0;
		#endregion
	}
}