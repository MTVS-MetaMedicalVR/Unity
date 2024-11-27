using System.Collections.Generic;
using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    [System.Serializable]
    public class UIWindow : MonoBehaviour  // MonoBehaviour ��� �߰�
    {
        public GameObject windowObject;
        public string windowId;
    }

    [Header("Windows")]
    [SerializeField] private UIWindow mainMenuWindow;
    [SerializeField] private UIWindow procedureSelectWindow;
    [SerializeField] private UIWindow roomCreateWindow;
    [SerializeField] private UIWindow roomJoinWindow;
    [SerializeField] private UIWindow practiceSpectateWindow;

    [Header("References")]
    [SerializeField] private QuickTestManager quickTestManager;
    [SerializeField] private PhotonNetworkManager networkManager;

    private Stack<UIWindow> windowStack = new Stack<UIWindow>();
    private UIWindow currentWindow;  // currentWindow ���� �߰�
    private string selectedCategory;
    private string selectedProcedureId;

    void Start()
    {
        ShowWindow(mainMenuWindow);
    }

    // ���� �޴� -> ���� ����
    public void OnProcedureSelectClicked()
    {
        ShowWindow(procedureSelectWindow);
    }

    // ���� ���� ��
    public void OnProcedureSelected(string category, string procedureId)
    {
        selectedCategory = category;
        selectedProcedureId = procedureId;
        ShowWindow(roomCreateWindow);
    }

    // �� ���� ��
    public void OnCreateRoom(string roomName)
    {
        // ���� ���� ����
        ProcedureSceneManager.Instance.SetProcedure(selectedCategory, selectedProcedureId);
        // �� ����
        networkManager.CreateSession(roomName);
        // �ǽ��Ƿ� �̵�
        ShowWindow(practiceSpectateWindow);
    }

    // �� ���� ��
    public void OnJoinRoom()
    {
        ShowWindow(roomJoinWindow);
    }

    // �ǽ���/������ ����
    public void OnJoinRoomMode(string roomName, bool isPracticeMode)
    {
        SceneType sceneType = isPracticeMode ? SceneType.PracticeRoom : SceneType.SpectatorRoom;
        networkManager.JoinSession(roomName, sceneType);
    }

    // �ڷΰ���
    public void OnBackPressed()
    {
        if (windowStack.Count > 0)
        {
            var previousWindow = windowStack.Pop();
            SwitchToWindow(previousWindow);
        }
    }



    private void ShowWindow(UIWindow window)
    {
        if (currentWindow != null)
        {
            currentWindow.windowObject.SetActive(false);
            windowStack.Push(currentWindow);
        }
        SwitchToWindow(window);
    }

    private void SwitchToWindow(UIWindow window)
    {
        var windows = FindObjectsOfType<UIWindow>();  // UIWindow�� MonoBehaviour�� ����ϹǷ� ����
        foreach (var w in windows)
        {
            if (w.windowObject != null)
                w.windowObject.SetActive(false);
        }
        window.windowObject.SetActive(true);
        currentWindow = window;
    }
}