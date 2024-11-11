using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class InGameProcedureManager : MonoBehaviour
{
    private static InGameProcedureManager instance;
    public static InGameProcedureManager Instance => instance;

    [Header("UI References")]
    [SerializeField] private Text currentStepText;
    [SerializeField] private Text progressText;
    [SerializeField] private Transform stepListPanel;
    [SerializeField] private GameObject stepTextPrefab;

    [Header("Procedure Objects")]
    [SerializeField] private List<ProcedureObjectBase> procedureObjects;

    [Header("Start UI")]
    [SerializeField] private GameObject procedureStartPanel;
    [SerializeField] private Text procedureNameText;
    [SerializeField] private Text procedureDescriptionText;
    [SerializeField] private Button startConfirmButton;

    [Header("Completion UI")]
    [SerializeField] private GameObject completionPanel;
    [SerializeField] private Text completionTitleText;
    [SerializeField] private Text completionDescriptionText;
    [SerializeField] private Text completionTimeText;
    [SerializeField] private Text completionScoreText;
    [SerializeField] private Transform completionStepListPanel;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private string menuSceneName = "MenuScene";

    private Procedure currentProcedure;
    private int currentStepIndex = -1;
    private List<Text> stepTexts = new List<Text>();
    private float procedureStartTime;
    private float procedureEndTime;
    private List<float> stepCompletionTimes = new List<float>();

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        string category = ProcedureSceneManager.Instance.CurrentCategory;

        if (completionPanel != null)
            completionPanel.SetActive(false);

        if (procedureStartPanel != null)
            procedureStartPanel.SetActive(false);

        if (returnToMenuButton != null)
            returnToMenuButton.onClick.AddListener(ReturnToMenu);

        if (startConfirmButton != null)
            startConfirmButton.onClick.AddListener(OnStartConfirmed);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartProcedure);

        LoadAndStartProcedure(category, procedureId);
    }

    void LoadAndStartProcedure(string category, string procedureId)
    {
        if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(procedureId))
        {
            Debug.LogError("Invalid category or procedure ID");
            return;
        }

        string path = Path.Combine(Application.streamingAssetsPath,
                                 "ProcedureData",
                                 category,
                                 procedureId,
                                 "procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            currentProcedure = JsonUtility.FromJson<Procedure>(jsonContent);

            if (!string.IsNullOrEmpty(currentProcedure.preRequisite))
            {
                LoadAndStartPreRequisite(currentProcedure.preRequisite);
            }
            else
            {
                StartProcedure(currentProcedure);
            }
        }
        else
        {
            Debug.LogError($"Procedure file not found at: {path}");
        }
    }

    void LoadAndStartPreRequisite(string preRequisiteId)
    {
        string path = Path.Combine(Application.streamingAssetsPath,
                                 "ProcedureData",
                                 "Common",
                                 preRequisiteId,
                                 "procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            Procedure preRequisite = JsonUtility.FromJson<Procedure>(jsonContent);
            StartProcedure(preRequisite, true);
        }
    }

    void StartProcedure(Procedure procedure, bool isPreRequisite = false)
    {
        currentProcedure = procedure;
        currentStepIndex = -1;

        foreach (var obj in procedureObjects)
        {
            if (obj != null)
                obj.gameObject.SetActive(false);
        }

        ClearStepList();
        CreateStepList();

        if (currentStepText != null)
            currentStepText.text = $"현재 절차: {procedure.name}";

        ShowStartUI();
    }

    void ShowStartUI()
    {
        if (stepListPanel != null)
            stepListPanel.gameObject.SetActive(false);

        if (procedureStartPanel != null)
        {
            procedureStartPanel.SetActive(true);

            if (procedureNameText != null)
                procedureNameText.text = $"<{currentProcedure.name}>";

            if (procedureDescriptionText != null)
                procedureDescriptionText.text = currentProcedure.description;
        }
    }

    void OnStartConfirmed()
    {
        if (procedureStartPanel != null)
            procedureStartPanel.SetActive(false);

        if (stepListPanel != null)
            stepListPanel.gameObject.SetActive(true);

        procedureStartTime = Time.time;
        stepCompletionTimes.Clear();

        StartNextStep();
    }

    void CreateStepList()
    {
        foreach (var step in currentProcedure.steps)
        {
            GameObject stepObj = Instantiate(stepTextPrefab, stepListPanel);
            Text stepText = stepObj.GetComponent<Text>();
            if (stepText != null)
            {
                stepText.text = step.name;
                stepText.color = Color.gray;
                stepTexts.Add(stepText);
            }
        }
    }

    void ClearStepList()
    {
        foreach (Transform child in stepListPanel)
        {
            Destroy(child.gameObject);
        }
        stepTexts.Clear();
    }

    public void StartNextStep()
    {
        currentStepIndex++;

        if (currentStepIndex >= currentProcedure.steps.Count)
        {
            OnProcedureComplete();
            return;
        }

        UpdateStepUI();
        ActivateCurrentStepObject();
    }

    void UpdateStepUI()
    {
        if (progressText != null)
        {
            float progress = ((float)(currentStepIndex + 1) / currentProcedure.steps.Count) * 100f;
            progressText.text = $"진행률: {progress:F1}%";
        }

        for (int i = 0; i < stepTexts.Count; i++)
        {
            if (stepTexts[i] != null)
            {
                if (i < currentStepIndex)
                    stepTexts[i].color = Color.green;
                else if (i == currentStepIndex)
                    stepTexts[i].color = Color.white;
                else
                    stepTexts[i].color = Color.gray;
            }
        }
    }

    void ActivateCurrentStepObject()
    {
        if (currentStepIndex >= 0 && currentStepIndex < procedureObjects.Count)
        {
            var currentObject = procedureObjects[currentStepIndex];
            if (currentObject != null)
            {
                currentObject.Initialize();
            }
        }
    }

    public string GetCurrentProcedureId()
    {
        return currentProcedure?.id;
    }

    public void CompleteCurrentStep()
    {
        stepCompletionTimes.Add(Time.time - procedureStartTime);

        if (currentStepIndex >= 0 && currentStepIndex < stepTexts.Count)
        {
            var stepText = stepTexts[currentStepIndex];
            if (stepText != null)
            {
                stepText.color = Color.green;
            }
        }

        StartNextStep();
    }

    void OnProcedureComplete()
    {
        procedureEndTime = Time.time;
        ShowCompletionUI();
    }

    void ShowCompletionUI()
    {
        if (completionPanel == null) return;

        if (stepListPanel != null)
            stepListPanel.gameObject.SetActive(false);

        completionPanel.SetActive(true);

        if (completionTitleText != null)
            completionTitleText.text = $"<{currentProcedure.name}>\n절차 완료!";

        if (completionDescriptionText != null)
            completionDescriptionText.text = currentProcedure.description;

        float totalTime = procedureEndTime - procedureStartTime;
        if (completionTimeText != null)
            completionTimeText.text = $"총 소요 시간: {totalTime:F1}초";

        ShowStepCompletionDetails();
        CalculateAndShowScore();
    }

    void ShowStepCompletionDetails()
    {
        if (completionStepListPanel == null) return;

        foreach (Transform child in completionStepListPanel)
            Destroy(child.gameObject);

        for (int i = 0; i < currentProcedure.steps.Count; i++)
        {
            GameObject stepObj = Instantiate(stepTextPrefab, completionStepListPanel);
            Text stepText = stepObj.GetComponent<Text>();
            if (stepText != null)
            {
                float stepTime = stepCompletionTimes[i];
                if (i > 0)
                    stepTime -= stepCompletionTimes[i - 1];

                string stepEvaluation = EvaluateStepTime(stepTime);
                stepText.text = $"{i + 1}. {currentProcedure.steps[i].name}\n" +
                               $"소요 시간: {stepTime:F1}초 ({stepEvaluation})";
                stepText.color = Color.white;
            }
        }
    }

    string EvaluateStepTime(float time)
    {
        if (time < 3.0f) return "매우 빠름";
        if (time < 5.0f) return "적절함";
        if (time < 10.0f) return "느림";
        return "매우 느림";
    }

    void CalculateAndShowScore()
    {
        if (completionScoreText == null) return;

        float totalTime = procedureEndTime - procedureStartTime;
        int baseScore = 100;
        int timeDeduction = Mathf.FloorToInt(totalTime / 10f) * 5;
        int finalScore = Mathf.Max(baseScore - timeDeduction, 0);

        string grade = finalScore >= 90 ? "A" :
                      finalScore >= 80 ? "B" :
                      finalScore >= 70 ? "C" :
                      finalScore >= 60 ? "D" : "F";

        completionScoreText.text = $"최종 점수: {finalScore}점 (등급: {grade})";
    }

    public void RestartProcedure()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ReturnToMenu()
    {
        ProcedureSceneManager.Instance.ClearProcedure();
        SceneManager.LoadScene(menuSceneName);
    }
}