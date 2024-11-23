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
        // ���� ���� �ε�
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

        // ID �������� ���� ���� ����
        commonProcedures = commonProcedures.OrderBy(p => p.id).ToList();

        // ���� ���� �ε�
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

        // �ʱ� �ܰ� ����
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
        // ���� ��ȣ�ۿ� ������Ʈ ��Ȱ��ȭ
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
            stepDescriptionText.text = $"�⺻ ���� {currentCommonProcedureIndex + 1}/{commonProcedures.Count}\n�ܰ� {currentStepIndex + 1}/{steps.Count}\n����: {nextStep.description}";
            progressText.text = $"�⺻ ���� ���൵: {currentCommonProcedureIndex + 1}/{commonProcedures.Count} - �ܰ�: {currentStepIndex + 1}/{steps.Count}";
        }
        else
        {
            stepDescriptionText.text = $"�ܰ� {currentStepIndex + 1}/{steps.Count}\n����: {nextStep.description}";
            progressText.text = $"���൵: {currentStepIndex + 1}/{steps.Count}";
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
        stepNameText.text = "���� �Ϸ�!";
        stepDescriptionText.text = "��� �ܰ踦 �Ϸ��߽��ϴ�.";
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