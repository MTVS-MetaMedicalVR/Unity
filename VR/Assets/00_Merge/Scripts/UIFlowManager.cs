using System.Collections.Generic;
using UnityEngine;

public class UIFlowManager : MonoBehaviour
{
    [System.Serializable]
    public class UIWindow : MonoBehaviour  // MonoBehaviour 상속 추가
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
    private UIWindow currentWindow;  // currentWindow 변수 추가
    private string selectedCategory;
    private string selectedProcedureId;

    void Start()
    {
        ShowWindow(mainMenuWindow);
    }

    // 메인 메뉴 -> 절차 선택
    public void OnProcedureSelectClicked()
    {
        ShowWindow(procedureSelectWindow);
    }

    // 절차 선택 시
    public void OnProcedureSelected(string category, string procedureId)
    {
        selectedCategory = category;
        selectedProcedureId = procedureId;
        ShowWindow(roomCreateWindow);
    }

    // 방 생성 시
    public void OnCreateRoom(string roomName)
    {
        // 절차 정보 설정
        ProcedureSceneManager.Instance.SetProcedure(selectedCategory, selectedProcedureId);
        // 방 생성
        networkManager.CreateSession(roomName);
        // 실습실로 이동
        ShowWindow(practiceSpectateWindow);
    }

    // 방 참가 시
    public void OnJoinRoom()
    {
        ShowWindow(roomJoinWindow);
    }

    // 실습실/관전실 선택
    public void OnJoinRoomMode(string roomName, bool isPracticeMode)
    {
        SceneType sceneType = isPracticeMode ? SceneType.PracticeRoom : SceneType.SpectatorRoom;
        networkManager.JoinSession(roomName, sceneType);
    }

    // 뒤로가기
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
        var windows = FindObjectsOfType<UIWindow>();  // UIWindow가 MonoBehaviour를 상속하므로 가능
        foreach (var w in windows)
        {
            if (w.windowObject != null)
                w.windowObject.SetActive(false);
        }
        window.windowObject.SetActive(true);
        currentWindow = window;
    }
}