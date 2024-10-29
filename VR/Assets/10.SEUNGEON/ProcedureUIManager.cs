// ProcedureSystem/UI/ProcedureUIManager.cs
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Oculus.Interaction;

public class ProcedureUIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPrefab;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float uiDistance = 2f;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private GameObject currentUI;
    private UnityAction currentCallback;

    public void ShowUI(ProcedureUIEvent uiEvent, UnityAction onButtonClick)
    {
        if (currentUI != null)
        {
            Destroy(currentUI);
        }

        if (!uiEvent.showUI) return;

        currentUI = Instantiate(uiPrefab);
        currentCallback = onButtonClick;
        PositionUIInFrontOfPlayer(currentUI, uiEvent.position);

        var uiController = currentUI.GetComponent<ProcedureUIController>();
        if (uiController != null)
        {
            uiController.SetContent(uiEvent.buttonText);
        }

        var eventWrapper = currentUI.GetComponent<IPointableCanvas>();
        if (eventWrapper != null)
        {
            //eventWrapper._whenSelectedHovered.AddListener(() => {
              //  currentCallback?.Invoke();
           // });
        }
    }

    private void PositionUIInFrontOfPlayer(GameObject ui, string position)
    {
        Vector3 targetPos = playerCamera.position + playerCamera.forward * uiDistance;

        switch (position.ToLower())
        {
            case "front":
                ui.transform.position = targetPos;
                break;
            case "side":
                ui.transform.position = targetPos + playerCamera.right * 0.5f;
                break;
            default:
                ui.transform.position = targetPos;
                break;
        }

        ui.transform.LookAt(2 * ui.transform.position - playerCamera.position);
    }

    public void HideUI()
    {
        if (currentUI != null)
        {
            var eventWrapper = currentUI.GetComponent<PointableCanvasUnityEventWrapper>();
            if (eventWrapper != null)
            {
              //  eventWrapper._whenSelectedHovered.RemoveAllListeners();
            }

            Destroy(currentUI);
            currentUI = null;
        }
        currentCallback = null;
    }
}