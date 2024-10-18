using System.Collections;
using UnityEngine;
using TMPro; // TextMeshPro�� ����ϱ� ���� ���ӽ����̽�

public class UIManager : MonoBehaviour
{
    // UI ���

    // ���� �޴� �г�
    public GameObject mainMenuPanel;
    // ���� �ȳ� �г�
    public GameObject procedurePanel;
    // ���� ���� �ܰ� �ȳ� �ؽ�Ʈ
    public TextMeshProUGUI stepText;
    // �ǵ�� �ؽ�Ʈ
    public TextMeshProUGUI feedbackText;
    // ���� �Ϸ� �� ǥ�õ� �г�
    public GameObject completionPanel; 

    private void Start()
    {
        // ���� ���� �� ���� �޴��� ǥ��
        ShowMainMenu(); 
    }

    // ���� �޴� ǥ��
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        procedurePanel.SetActive(false);
        // �ǵ�� �ؽ�Ʈ �ʱ�ȭ
        feedbackText.text = ""; 
        // �Ϸ� �г� ��Ȱ��ȭ
        completionPanel.SetActive(false); 
    }

    // ���� �г� ǥ��
    public void ShowProcedurePanel()
    {
        mainMenuPanel.SetActive(false);
        procedurePanel.SetActive(true);
        // �ǵ�� �ʱ�ȭ
        feedbackText.text = ""; 
    }

    // ���� �ܰ� �ؽ�Ʈ ������Ʈ
    public void UpdateStepText(string stepDescription)
    {
        stepText.text = stepDescription;
    }

    // �ǵ�� ǥ��
    public void ShowFeedback(string feedback)
    {
        // �ǵ�� �ؽ�Ʈ ������Ʈ
        feedbackText.text = feedback;
        // ���� �ð� �Ŀ� �ǵ���� ����� ���� �ڷ�ƾ ����
        StartCoroutine(HideFeedbackAfterDelay()); 
    }

    // �ǵ���� ���� �ð� �Ŀ� ����� ���� �ڷ�ƾ
    private IEnumerator HideFeedbackAfterDelay()
    {
        // 2�� �Ŀ� �ǵ�� �ؽ�Ʈ�� �ʱ�ȭ
        yield return new WaitForSeconds(2.0f); 
        feedbackText.text = "";
    }

    // ���� �Ϸ� ó��
    public void ShowCompletion()
    {
        // ���� �г� ��Ȱ��ȭ
        procedurePanel.SetActive(false);
        // �Ϸ� �г� Ȱ��ȭ
        completionPanel.SetActive(true); 
    }
}
