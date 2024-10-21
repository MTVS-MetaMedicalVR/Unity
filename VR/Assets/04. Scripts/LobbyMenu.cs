using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LobbyMenu : MonoBehaviour
{
    // �κ� �޴� UI�� �����ϰ� �ִ� ĵ����
    public GameObject lobbyMenuUI;

    // ������ ���� UI
    public GameObject contentSelectionUI;

    // ���� �޴� UI
    public GameObject settingsMenuUI;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // Play Button ������ �� ȣ���� �Լ�
    public void OnPlayButtonClick()
    {
        // �κ� �޴� UI ��Ȱ��ȭ
        lobbyMenuUI.SetActive(false);

        // ������ ���� UI Ȱ��ȭ
        contentSelectionUI.SetActive(true);
        
    }
    // Settings Button ������ �� ȣ���� �Լ�
    public void OnSettingsButtonClick()
    {
        // �κ� �޴� UI ��Ȱ��ȭ
        lobbyMenuUI.SetActive(false);

        // ���� �޴� UI Ȱ��ȭ
        settingsMenuUI.SetActive(true);

        Debug.Log("���� �޴� § !");
    }
    // Quit Button ������ �� ȣ���� �Լ�
    public void OnQuitButtonClick()
    {
        Debug.Log("�ùķ��̼��� �����մϴ�.");

        // ����Ƽ �����Ϳ��� �÷��� ���� (�����ϱ� ���� �����ؾ� ��)
        EditorApplication.isPlaying = false;
        
        // ���� ���� >>> ���� �Ŀ� �۵���
        Application.Quit();
    }

    public void OnBackButtonClick()
    {
        // ���� �޴� UI ��Ȱ��ȭ
        settingsMenuUI.SetActive(false);

        // �κ� �޴� UI Ȱ��ȭ
        lobbyMenuUI.SetActive(true);
    }
}
