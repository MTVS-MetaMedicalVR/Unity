// ProcedureSystem/UI/ProcedureUIController.cs
using UnityEngine;
using TMPro;

public class ProcedureUIController : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI titleText;
	[SerializeField] private TextMeshProUGUI descriptionText;

	public void SetContent(string buttonText)
	{
		if (titleText != null)
		{
			titleText.text = buttonText;
		}
	}
}