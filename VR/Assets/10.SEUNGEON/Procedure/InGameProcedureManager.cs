using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InGameProcedureManager : MonoBehaviour
{
    public static InGameProcedureManager Instance { get; private set; }

    [Header("Step UI")]
    [SerializeField] private GameObject stepNotificationPanel;
    [SerializeField] private TMP_Text stepNameText;
    [SerializeField] private TMP_Text stepDescriptionText;
    [SerializeField] private Toggle confirmToggle;
    [SerializeField] private TMP_Text warningText;

    [Header("Progress")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private List<ProcedureObjectBase> procedureObjects;

    private static class Paths
    {
        public const string PROCEDURES_BASE = "ProcedureData";
        public const string COMMON_FOLDER = "Common";
        public const string PROCEDURE_FILE = "procedure";
    }

    private Procedure currentProcedure;
    private List<Step> steps;
    private int currentStepIndex = -1;
    private List<Procedure> commonProcedures = new List<Procedure>();
    private int currentCommonProcedureIndex = -1;
    private bool isExecutingCommonProcedures = true;

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
        LoadProcedures();
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
            confirmToggle.isOn = false;
        }
    }

    private void LoadProcedures()
    {
        // 공통 절차 로드
        TextAsset[] commonProcedureAssets = Resources.LoadAll<TextAsset>($"{Paths.PROCEDURES_BASE}/{Paths.COMMON_FOLDER}");

        foreach (var asset in commonProcedureAssets)
        {
            if (asset.name.EndsWith(Paths.PROCEDURE_FILE))
            {
                try
                {
                    Procedure commonProcedure = JsonUtility.FromJson<Procedure>(asset.text);
                    commonProcedures.Add(commonProcedure);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error parsing common procedure {asset.name}: {e.Message}");
                }
            }
        }

        // ID 기준으로 공통 절차 정렬
        commonProcedures = commonProcedures.OrderBy(p => p.id).ToList();

        // 현재 절차 로드
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        string procedurePath = $"{Paths.PROCEDURES_BASE}/{category}/{procedureId}/{Paths.PROCEDURE_FILE}";

        TextAsset procedureAsset = Resources.Load<TextAsset>(procedurePath);
        if (procedureAsset != null)
        {
            currentProcedure = JsonUtility.FromJson<Procedure>(procedureAsset.text);
        }
        else
        {
            Debug.LogError($"Failed to load procedure at path: {procedurePath}");
        }

        // 초기 단계 설정
        if (commonProcedures.Count > 0)
        {
            currentCommonProcedureIndex = 0;
            steps = commonProcedures[0].steps;
        }
        else
        {
            isExecutingCommonProcedures = false;
            steps = currentProcedure.steps;
        }
    }

    private void ShowNextStep()
    {
        // 현재 상호작용 오브젝트 비활성화
        if (currentStepIndex >= 0 && currentStepIndex < steps.Count)
        {
            Step currentStep = steps[currentStepIndex];
            var currentObject = procedureObjects.Find(obj => obj.ProcedureId == currentStep.targetName);
            if (currentObject != null)
            {
                currentObject.DisableInteraction();
            }
        }

        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            if (isExecutingCommonProcedures)
            {
                currentCommonProcedureIndex++;
                if (currentCommonProcedureIndex < commonProcedures.Count)
                {
                    currentStepIndex = -1;
                    steps = commonProcedures[currentCommonProcedureIndex].steps;
                    ShowNextStep();
                    return;
                }
                else
                {
                    isExecutingCommonProcedures = false;
                    currentStepIndex = -1;
                    steps = currentProcedure.steps;
                    ShowNextStep();
                    return;
                }
            }
            else
            {
                CompleteProcedure();
                return;
            }
        }

        Step nextStep = steps[currentStepIndex];

        stepNameText.text = nextStep.name;
        if (isExecutingCommonProcedures)
        {
            stepDescriptionText.text = $"기본 절차 {currentCommonProcedureIndex + 1}/{commonProcedures.Count}\n단계 {currentStepIndex + 1}/{steps.Count}\n설명: {nextStep.description}";
            progressText.text = $"기본 절차 진행도: {currentCommonProcedureIndex + 1}/{commonProcedures.Count} - 단계: {currentStepIndex + 1}/{steps.Count}";
        }
        else
        {
            stepDescriptionText.text = $"단계 {currentStepIndex + 1}/{steps.Count}\n설명: {nextStep.description}";
            progressText.text = $"진행도: {currentStepIndex + 1}/{steps.Count}";
        }

        var targetObject = procedureObjects.Find(obj => obj.ProcedureId == nextStep.targetName);
        if (targetObject != null)
        {
            targetObject.Initialize();
            targetObject.EnableInteraction();
        }

        stepNotificationPanel.SetActive(true);
    }

    private void OnStepConfirmed()
    {
        stepNotificationPanel.SetActive(false);

        Step currentStep = steps[currentStepIndex];
        var targetObject = procedureObjects.Find(obj => obj.ProcedureId == currentStep.targetName);

        if (targetObject != null)
        {
            targetObject.Initialize();
        }
        else
        {
            CompleteCurrentStep();
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
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
        confirmToggle.onValueChanged.RemoveAllListeners();
        confirmToggle.onValueChanged.AddListener((bool isOn) => {
            if (isOn)
            {
                stepNotificationPanel.SetActive(false);
            }
        });
        stepNotificationPanel.SetActive(true);
    }

    public string GetCurrentProcedureId()
    {
        if (isExecutingCommonProcedures && currentCommonProcedureIndex >= 0 && currentCommonProcedureIndex < commonProcedures.Count)
        {
            return commonProcedures[currentCommonProcedureIndex].id;
        }
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