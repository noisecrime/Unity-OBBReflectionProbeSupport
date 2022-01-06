using UnityEngine;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	[ExecuteAlways]
	public class SetTitleCard : MonoBehaviour
    {
		[SerializeField]
		string			title = null;

		[SerializeField, TextArea(1,8)]
		string			notes;

		private void Awake()
		{
			if ( null == title ) 
				title = this.name;
		}

		private void OnEnable()
		{
			CanavsController controller = FindObjectOfType<CanavsController>();
			
			if ( null != controller )
				controller.UpdateContents( title, notes );
		}
	}
}