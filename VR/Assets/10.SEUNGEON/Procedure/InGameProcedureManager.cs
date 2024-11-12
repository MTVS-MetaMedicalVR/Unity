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
    [SerializeField] private GameObject stepNotificationPanel;    // �ܰ� �˸� �г�
    [SerializeField] private TMP_Text stepNameText;              // �ܰ� �̸� �ؽ�Ʈ
    [SerializeField] private TMP_Text stepDescriptionText;       // �ܰ� ���� �ؽ�Ʈ
    [SerializeField] private Toggle confirmToggle;               // Ȯ�� ��� (��ư ���)

    [Header("Progress")]
    [SerializeField] private TMP_Text progressText;              // �����Ȳ �ؽ�Ʈ
    [SerializeField] private List<ProcedureObjectBase> procedureObjects;  // ���� ������Ʈ��

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
            // ��� ���� ����
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

        // UI ������Ʈ
        stepNameText.text = currentStep.name;
        stepDescriptionText.text = $"�ܰ� {currentStepIndex + 1}/{steps.Count}\n����: {currentStep.description}";
        progressText.text = $"���൵: {currentStepIndex + 1}/{steps.Count}";

        // �˸� �г� ǥ��
        stepNotificationPanel.SetActive(true);

        // ���� ������Ʈ�� ��Ȱ��ȭ
        foreach (var obj in procedureObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }

    private void OnStepConfirmed()
    {
        // �˸� �г� �����
        stepNotificationPanel.SetActive(false);

        // ���� ������ ������Ʈ Ȱ��ȭ
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
        stepNameText.text = "���� �Ϸ�!";
        stepDescriptionText.text = "��� �ܰ踦 �Ϸ��߽��ϴ�.";
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