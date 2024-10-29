// ProcedureSystem/VRProcedureManager.cs
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using Oculus.Interaction;
using System.Linq;

public class VRProcedureManager : MonoBehaviour
{
    [SerializeField] private TextAsset procedureJsonFile;
    [SerializeField] private ProceduralObject proceduralObject;
    [SerializeField] private ProcedureUIManager uiManager;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI conditionText;
    [SerializeField] private TextMeshProUGUI progressText;

    private ProcedureData procedureData;
    private ProcedureStep currentStep;
    private Dictionary<ProcedureCondition, float> conditionTimers = new Dictionary<ProcedureCondition, float>();
    private Dictionary<ProcedureCondition, IConditionChecker> conditionCheckers = new Dictionary<ProcedureCondition, IConditionChecker>();

    private ConditionCheckerFactory checkerFactory;
    private float procedureStartTime;
    private float stepStartTime;
    private bool canProgress;

    private void Awake()
    {
        // 서비스 초기화
        var objectService = new ObjectService();
        ServiceLocator.RegisterService<IObjectService>(objectService);

        foreach (var interactable in FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>())
        {
            objectService.RegisterObject(interactable);
        }

        foreach (var stateful in FindObjectsOfType<MonoBehaviour>().OfType<IStatefulObject>())
        {
            objectService.RegisterObject(stateful);
        }

        checkerFactory = new ConditionCheckerFactory(objectService, proceduralObject);
    }

    private void Start()
    {
        LoadProcedureData();
        if (procedureData.steps.Count > 0)
        {
            procedureStartTime = Time.time;
            SetCurrentStep(procedureData.steps[0]);
        }
    }

    private void Update()
    {
        if (currentStep == null || !canProgress) return;

        bool allConditionsMet = true;
        string conditionsStatus = "";
        float overallProgress = 0f;
        int totalConditions = currentStep.conditions.Length;

        foreach (var condition in currentStep.conditions)
        {
            if (!conditionCheckers.ContainsKey(condition))
            {
                conditionCheckers[condition] = checkerFactory.CreateChecker(condition);
            }

            var checker = conditionCheckers[condition];
            if (checker == null) continue;

            bool isConditionMet = checker.CheckCondition();

            if (isConditionMet)
            {
                if (!conditionTimers.ContainsKey(condition))
                {
                    conditionTimers[condition] = 0f;
                }

                conditionTimers[condition] += Time.deltaTime;

                if (conditionTimers[condition] < condition.holdDuration)
                {
                    allConditionsMet = false;
                }
                else
                {
                    overallProgress += 1f;
                }
            }
            else
            {
                conditionTimers[condition] = 0f;
                allConditionsMet = false;
            }

            string checkMark = isConditionMet ? "✓ " : "□ ";
            if (condition.holdDuration > 0)
            {
                float remainingTime = Mathf.Max(0, condition.holdDuration - (conditionTimers.ContainsKey(condition) ? conditionTimers[condition] : 0f));
                conditionsStatus += $"{checkMark}{checker.GetDescription()} ({remainingTime:F1}s)\n";
            }
            else
            {
                conditionsStatus += $"{checkMark}{checker.GetDescription()}\n";
            }
        }

        UpdateUI(conditionsStatus, overallProgress, totalConditions);

        if (allConditionsMet)
        {
            CompleteCurrentStep();
        }
    }

    private void UpdateUI(string conditionsStatus, float overallProgress, int totalConditions)
    {
        if (conditionText)
        {
            conditionText.text = conditionsStatus;
        }

        if (progressText && totalConditions > 0)
        {
            float progressPercentage = (overallProgress / totalConditions) * 100f;
            progressText.text = $"진행률: {progressPercentage:F0}%";
        }
    }

    private void CompleteCurrentStep()
    {
        float stepCompletionTime = Time.time - stepStartTime;
        EventManager.Publish(new ProcedureEvents.StepCompleted(currentStep, stepCompletionTime));
        currentStep.onStepComplete?.Invoke();
        ProceedToNextStep();
    }

    private void SetCurrentStep(ProcedureStep step)
    {
        currentStep = step;
        stepStartTime = Time.time;

        if (step.uiEvent != null && step.uiEvent.showUI)
        {
            uiManager.ShowUI(step.uiEvent, () => {
                if (step.uiEvent.blockProgress)
                {
                    canProgress = true;
                }
            });

            if (!step.uiEvent.blockProgress)
            {
                canProgress = true;
            }
        }
        else
        {
            canProgress = true;
        }

        foreach (var checker in conditionCheckers.Values)
        {
            checker.Reset();
        }

        conditionCheckers.Clear();
        conditionTimers.Clear();

        UpdateBaseUI();
        EventManager.Publish(new ProcedureEvents.StepStarted(step));
        step.onStepStart?.Invoke();
    }

    private void UpdateBaseUI()
    {
        if (titleText) titleText.text = currentStep.title;
        if (descriptionText) descriptionText.text = currentStep.description;
    }

    private void LoadProcedureData()
    {
        if (procedureJsonFile != null)
        {
            try
            {
                procedureData = JsonConvert.DeserializeObject<ProcedureData>(procedureJsonFile.text);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load procedure data: {ex.Message}");
                procedureData = new ProcedureData { steps = new List<ProcedureStep>() };
            }
        }
        else
        {
            Debug.LogError("Procedure JSON file is not assigned!");
            procedureData = new ProcedureData { steps = new List<ProcedureStep>() };
        }
    }

    private void ProceedToNextStep()
    {
        if (currentStep.nextStepId == null)
        {
            float totalTime = Time.time - procedureStartTime;
            EventManager.Publish(new ProcedureEvents.ProcedureCompleted(totalTime));
            return;
        }

        var nextStep = procedureData.steps.Find(s => s.id.ToString() == currentStep.nextStepId);
        if (nextStep != null)
        {
            SetCurrentStep(nextStep);
        }
    }

    private void OnDestroy()
    {
        uiManager?.HideUI();
    }
}