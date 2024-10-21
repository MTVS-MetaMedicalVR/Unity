using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    // 로비 메뉴 UI를 포함하고 있는 캔버스
    public GameObject lobbyMenuUI;

    // 콘텐츠 선택 UI
    public GameObject contentSelectionUI;

    // 세팅 메뉴 UI
    public GameObject settingsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Play Button 눌렀을 때 호출할 함수
    public void OnPlayButtonClick()
    {
        // 로비 메뉴 UI 비활성화
        lobbyMenuUI.SetActive(false);

        // 콘텐츠 선택 UI 활성화
        contentSelectionUI.SetActive(true);
        
    }
    // Settings Button 눌렀을 때 호출할 함수
    public void OnSettingsButtonClick()
    {
        // 로비 메뉴 UI 비활성화
        lobbyMenuUI.SetActive(false);

        // 세팅 메뉴 UI 활성화
        settingsMenuUI.SetActive(true);

        Debug.Log("세팅 메뉴 짠 !");
    }
    // Quit Button 눌렀을 때 호출할 함수
    public void OnQuitButtonClick()
    {
        Debug.Log("시뮬레이션을 종료합니다.");

        // 유니티 에디터에서 플레이 종료 (빌드하기 전에 삭제해야 함)
        EditorApplication.isPlaying = false;
        
        // 게임 종료 >>> 빌드 후에 작동함
        Application.Quit();
    }

    public void OnBackButtonClick()
    {
        // 세팅 메뉴 UI 비활성화
        settingsMenuUI.SetActive(false);

        // 로비 메뉴 UI 활성화
        lobbyMenuUI.SetActive(true);
    }
}
