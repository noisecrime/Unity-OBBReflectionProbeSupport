
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoiseCrimeStudios.Examples.OBBProjection
{
	// [ExecuteAlways]
	public class CanavsController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI	titleTmpro;
		[SerializeField] TextMeshProUGUI	notesTmpro;
		[SerializeField] GameObject			notesGroup;
		[SerializeField] bool				showNotes = true;

	
		public void UpdateContents( string titleCard, string notes )
		{
			titleTmpro.text = titleCard + "  [" + SceneManager.GetActiveScene().name + "]";
			notesTmpro.text	= notes;

			notesGroup.SetActive(showNotes && !string.IsNullOrEmpty(notes));
		}
	}
}