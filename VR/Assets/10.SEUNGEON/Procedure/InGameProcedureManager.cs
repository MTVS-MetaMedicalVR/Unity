using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;

public class InGameProcedureManager : MonoBehaviour
{
    public static InGameProcedureManager Instance { get; private set; }

    [Header("Step UI")]
    [SerializeField] private GameObject stepNotificationPanel;    // 단계 알림 패널
    [SerializeField] private TMP_Text stepNameText;              // 단계 이름 텍스트
    [SerializeField] private TMP_Text stepDescriptionText;       // 단계 설명 텍스트
    [SerializeField] private Toggle confirmToggle;               // 확인 토글 (버튼 대신)

    [Header("Progress")]
    [SerializeField] private TMP_Text progressText;              // 진행상황 텍스트
    [SerializeField] private List<ProcedureObjectBase> procedureObjects;  // 절차 오브젝트들

    private Procedure currentProcedure;
    private List<Step> steps;
    private int currentStepIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadProcedure();
        SetupToggleListener();
        ShowNextStep();
    }

    private void SetupToggleListener()
    {
        if (confirmToggle != null)
        {
            confirmToggle.onValueChanged.AddListener(OnToggleValueChanged);
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (isOn)
        {
            OnStepConfirmed();
            // 토글 상태 리셋
            confirmToggle.isOn = false;
        }
    }

    private void LoadProcedure()
    {
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;

        string path = Path.Combine(Application.streamingAssetsPath, "ProcedureData",
            category, procedureId, "procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            currentProcedure = JsonUtility.FromJson<Procedure>(jsonContent);
            steps = currentProcedure.steps;
        }
    }

    private void ShowNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            CompleteProcedure();
            return;
        }

        Step currentStep = steps[currentStepIndex];

        // UI 업데이트
        stepNameText.text = currentStep.name;
        stepDescriptionText.text = $"단계 {currentStepIndex + 1}/{steps.Count}\n설명: {currentStep.description}";
        progressText.text = $"진행도: {currentStepIndex + 1}/{steps.Count}";

        // 알림 패널 표시
        stepNotificationPanel.SetActive(true);

        // 이전 오브젝트들 비활성화
        foreach (var obj in procedureObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void OnStepConfirmed()
    {
        // 알림 패널 숨기기
        stepNotificationPanel.SetActive(false);

        // 현재 스텝의 오브젝트 활성화
        Step currentStep = steps[currentStepIndex];
        var targetObject = procedureObjects.Find(obj => obj.gameObject.name == currentStep.targetName);

        if (targetObject != null)
        {
            targetObject.gameObject.SetActive(true);
            targetObject.Initialize();
        }
    }

    public void CompleteCurrentStep()
    {
        ShowNextStep();
    }

    private void CompleteProcedure()
    {
        stepNameText.text = "절차 완료!";
        stepDescriptionText.text = "모든 단계를 완료했습니다.";
        confirmToggle.onValueChanged.RemoveAllListeners();
        confirmToggle.onValueChanged.AddListener((bool isOn) => {
            if (isOn)
            {
                SceneManager.LoadScene("MenuScene");
            }
        });
        stepNotificationPanel.SetActive(true);
    }

    public string GetCurrentProcedureId()
    {
        return currentProcedure?.id;
    }

    private void OnDestroy()
    {
        if (confirmToggle != null)
        {
            confirmToggle.onValueChanged.RemoveAllListeners();
        }
    }
}