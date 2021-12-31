using UnityEngine;

namespace NoiseCrimeStudios.Rendering.OBBProjectionProbe
{
	[ExecuteInEditMode]
	public class OBBProjectionProbe : MonoBehaviour
	{
		private Renderer m_renderer;

		protected Renderer ObjRenderer
		{
			get
			{
				if ( null == m_renderer )
					m_renderer = GetComponent<Renderer>();
				return m_renderer;
			}
		}

		protected Matrix4x4 WorldToLocalMatrix
		{
			get
			{
				if ( null == ObjRenderer )
					return transform.worldToLocalMatrix;
				else
					return ObjRenderer.worldToLocalMatrix;
			}
		}

		void Start()
		{
			RefreshShaderGlobals();
		}

		void RefreshShaderGlobals()
		{
			Shader.SetGlobalMatrix( "_BoxProbeWorldToLocal", WorldToLocalMatrix );
		}

		[ContextMenu( "Update WorldToLocalMatrix " )]
		private void UpdateWorldToLocalMatrix()
		{
			RefreshShaderGlobals();
		}

		[ContextMenu( "Log Matrices To Console" )]
		private void LogMatrices()
		{
			string results =
			$"{transform.localPosition} {transform.localRotation.eulerAngles} {transform.localScale}\n" +
			$"{transform.position} {transform.rotation.eulerAngles} {transform.lossyScale}\n" +
			"ObjRenderer.worldToLocalMatrix\n" + ObjRenderer.worldToLocalMatrix + "\n" +
			"Transform.worldToLocalMatrix\n" + transform.worldToLocalMatrix + "\n" +
			"Transform.localToWorldMatrix\n" + transform.localToWorldMatrix ;

			Debug.Log( results );
		}
	}
}