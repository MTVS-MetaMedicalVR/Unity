using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

public class InGameProcedureManager : MonoBehaviour
{
    public static InGameProcedureManager Instance { get; private set; }

    [Header("Step UI")]
    [SerializeField] private GameObject stepNotificationPanel;
    [SerializeField] private TMP_Text stepNameText;
    [SerializeField] private TMP_Text stepDescriptionText;
    [SerializeField] private Toggle confirmToggle;
    [SerializeField] private TMP_Text warningText;  // ��� �ؽ�Ʈ (������Ʈ�� ���� �� ǥ��)

    [Header("Progress")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private List<ProcedureObjectBase> procedureObjects;

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
        // 1. Common ������ �⺻ ������ �ε�
        string commonPath = Path.Combine(Application.streamingAssetsPath, "ProcedureData", "Common");
        if (Directory.Exists(commonPath))
        {
            var commonProcedureFolders = Directory.GetDirectories(commonPath);
            foreach (var folder in commonProcedureFolders.OrderBy(f => Path.GetFileName(f)))
            {
                string jsonPath = Path.Combine(folder, "procedure.json");
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    Procedure commonProcedure = JsonUtility.FromJson<Procedure>(jsonContent);
                    commonProcedures.Add(commonProcedure);
                }
            }
        }

        // 2. ���� ���� �ε�
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        string path = Path.Combine(Application.streamingAssetsPath, "ProcedureData",
            category, procedureId, "procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            currentProcedure = JsonUtility.FromJson<Procedure>(jsonContent);
        }

        // �ʱ� steps�� Common ������ ù ��° ������ ����
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
        currentStepIndex++;

        if (currentStepIndex >= steps.Count)
        {
            if (isExecutingCommonProcedures)
            {
                // ���� Common ������ �̵�
                currentCommonProcedureIndex++;
                if (currentCommonProcedureIndex < commonProcedures.Count)
                {
                    // ���� Common ���� ����
                    currentStepIndex = -1;
                    steps = commonProcedures[currentCommonProcedureIndex].steps;
                    ShowNextStep();
                    return;
                }
                else
                {
                    // Common �������� ��� ������ ���� ������ ��ȯ
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

        Step currentStep = steps[currentStepIndex];

        // UI ������Ʈ
        stepNameText.text = currentStep.name;
        if (isExecutingCommonProcedures)
        {
            stepDescriptionText.text = $"�⺻ ���� {currentCommonProcedureIndex + 1}/{commonProcedures.Count}\n�ܰ� {currentStepIndex + 1}/{steps.Count}\n����: {currentStep.description}";
            progressText.text = $"�⺻ ���� ���൵: {currentCommonProcedureIndex + 1}/{commonProcedures.Count} - �ܰ�: {currentStepIndex + 1}/{steps.Count}";
        }
        else
        {
            stepDescriptionText.text = $"�ܰ� {currentStepIndex + 1}/{steps.Count}\n����: {currentStep.description}";
            progressText.text = $"���൵: {currentStepIndex + 1}/{steps.Count}";
        }

        // �ش� ID�� ���� ������Ʈ�� �ִ��� Ȯ���ϰ� ��� �޽��� ǥ��
        var targetObject = procedureObjects.Find(obj => obj.ProcedureId == currentStep.targetName);

        // �˸� �г� ǥ��
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
            // ������Ʈ�� ������ �ٷ� ���� �ܰ�� ����
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
                //SceneManager.LoadScene("MenuScene");
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