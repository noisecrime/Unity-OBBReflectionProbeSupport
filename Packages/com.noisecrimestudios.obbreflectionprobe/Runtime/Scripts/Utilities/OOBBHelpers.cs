using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	public static class OOBBHelpers
	{
		public static string MatrixLongString( Matrix4x4 matrix )
		{
			return string.Format( "{0:F4}, {1:F4}, {2:F4}, {3:F4},  {4:F4}, {5:F4}, {6:F4}, {7:F4},  {8:F4}, {9:F4}, {10:F4}, {11:F4},  {12:F4}, {13:F4}, {14:F4}, {15:F4}",
				matrix.m00, matrix.m01, matrix.m02, matrix.m03, matrix.m10, matrix.m11, matrix.m12, matrix.m13, matrix.m20, matrix.m21, matrix.m22, matrix.m23, matrix.m30, matrix.m31, matrix.m32, matrix.m33 );
		}
	}
}