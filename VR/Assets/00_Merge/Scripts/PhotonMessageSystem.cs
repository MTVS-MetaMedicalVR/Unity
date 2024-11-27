// PhotonMessageSystem.cs
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PhotonMessageSystem : MonoBehaviour
{
    public static PhotonMessageSystem Instance { get; private set; }

    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;
    [SerializeField] private float autoHideDelay = 2f; // 자동으로 숨길 시간

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        closeButton.onClick.AddListener(HideMessage);
        messagePanel.SetActive(false);
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), autoHideDelay);
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }
}
