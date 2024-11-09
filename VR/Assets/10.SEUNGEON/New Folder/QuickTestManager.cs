using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

[System.Serializable]
public class Step
{
    public string name;
    public string type;
    public float duration = -1f;
    public string targetName;
}

[System.Serializable]
public class Procedure
{
    public string id;
    public string name;
    public string description;
    public string preRequisite;
    public List<Step> steps = new List<Step>();
}

public class QuickTestManager : MonoBehaviour
{
    private static class Paths
    {
        public static readonly string PROCEDURES_FOLDER = "ProcedureData";
        public static readonly string COMMON_FOLDER = "Common";
        public static readonly string PROCEDURE_INFO = "procedure.json";
        public static readonly string THUMBNAIL = "thumbnail.png";
    }

    private static class ProcedureType
    {
        public static readonly string ANIMATION = "animation";
        public static readonly string GESTURE = "gesture";
        public static readonly string TRIGGER = "trigger";
        public static readonly string NOTICE = "notice";
        public static readonly string TOOL = "tool";
    }

    [Header("UI References")]
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject procedureButtonPrefab;
    [SerializeField] private GameObject stepButtonPrefab;
    [SerializeField] private Transform categoryPanel;
    [SerializeField] private Transform procedurePanel;
    [SerializeField] private Transform stepPanel;

    private Dictionary<Button, Coroutine> buttonCoroutines = new Dictionary<Button, Coroutine>();
    private List<Button> stepButtons = new List<Button>();
    private int currentStepIndex = -1;
    private Dictionary<string, Procedure> commonProcedures = new Dictionary<string, Procedure>();
    private Procedure currentProcedure;

    void Start()
    {
        string proceduresPath = Path.Combine(Application.streamingAssetsPath, Paths.PROCEDURES_FOLDER);
        if (Directory.Exists(proceduresPath))
        {
            LoadCommonProcedures(proceduresPath);
            ScanProcedureFolders(proceduresPath);
        }
        else
        {
            Debug.LogError($"Procedures folder not found at: {proceduresPath}");
        }
    }

    void LoadCommonProcedures(string basePath)
    {
        string commonPath = Path.Combine(basePath, Paths.COMMON_FOLDER);
        if (Directory.Exists(commonPath))
        {
            foreach (string procedureFolder in Directory.GetDirectories(commonPath))
            {
                string jsonPath = Path.Combine(procedureFolder, Paths.PROCEDURE_INFO);
                if (File.Exists(jsonPath))
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    Procedure procedure = JsonUtility.FromJson<Procedure>(jsonContent);
                    if (!string.IsNullOrEmpty(procedure.id))
                    {
                        commonProcedures[procedure.id] = procedure;
                    }
                }
            }
        }
    }

    void ScanProcedureFolders(string basePath)
    {
        var directories = Directory.GetDirectories(basePath)
                                 .Where(d => !Path.GetFileName(d).Equals(Paths.COMMON_FOLDER));

        foreach (string dir in directories)
        {
            string categoryName = Path.GetFileName(dir);
            CreateCategoryButton(categoryName, dir);
        }
    }

    void CreateCategoryButton(string categoryName, string path)
    {
        GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryPanel);
        buttonObj.GetComponentInChildren<Text>().text = categoryName;
        buttonObj.GetComponent<Button>().onClick.AddListener(() => ShowProceduresInFolder(path));
    }

    void ShowProceduresInFolder(string folderPath)
    {
        ClearPanel(procedurePanel);
        ClearPanel(stepPanel);

        foreach (string procedureFolder in Directory.GetDirectories(folderPath))
        {
            string jsonPath = Path.Combine(procedureFolder, Paths.PROCEDURE_INFO);
            string thumbnailPath = Path.Combine(procedureFolder, Paths.THUMBNAIL);

            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath);
                Procedure procedure = JsonUtility.FromJson<Procedure>(jsonContent);

                GameObject buttonObj = Instantiate(procedureButtonPrefab, procedurePanel);

                // 썸네일 이미지 로드
                if (File.Exists(thumbnailPath))
                {
                    StartCoroutine(LoadThumbnail(thumbnailPath, buttonObj.GetComponent<Image>()));
                }

                // 절차 정보 설정
                Text buttonText = buttonObj.GetComponentInChildren<Text>();
                buttonText.text = $"{procedure.name}\n{procedure.description}";

                Button button = buttonObj.GetComponent<Button>();
                button.onClick.AddListener(() => StartProcedureWithPreRequisites(procedure));
            }
        }
    }

    IEnumerator LoadThumbnail(string path, Image targetImage)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            targetImage.sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                Vector2.one * 0.5f);
        }
        else
        {
            Debug.LogError($"Error loading thumbnail: {request.error}");
        }

        request.Dispose();
    }

    void StartProcedureWithPreRequisites(Procedure procedure)
    {
        currentProcedure = procedure;

        if (!string.IsNullOrEmpty(procedure.preRequisite) &&
            commonProcedures.ContainsKey(procedure.preRequisite))
        {
            ShowSteps(commonProcedures[procedure.preRequisite], true);
        }
        else
        {
            ShowSteps(procedure, false);
        }
    }

    void ShowSteps(Procedure procedure, bool isPreRequisite)
    {
        ClearPanel(stepPanel);
        stepButtons.Clear();
        currentStepIndex = -1;

        for (int i = 0; i < procedure.steps.Count; i++)
        {
            Step step = procedure.steps[i];
            GameObject buttonObj = Instantiate(stepButtonPrefab, stepPanel);
            Button stepButton = buttonObj.GetComponent<Button>();
            Text buttonText = buttonObj.GetComponentInChildren<Text>();

            string timeInfo = step.type.ToLower() == ProcedureType.ANIMATION ?
                $"[{step.duration}초]" : "";

            string typeInfo = step.type.ToLower() switch
            {
                var t when t == ProcedureType.ANIMATION => "애니메이션",
                var t when t == ProcedureType.GESTURE => $"제스처: {step.targetName}",
                var t when t == ProcedureType.TRIGGER => $"이동: {step.targetName}",
                var t when t == ProcedureType.TOOL => $"도구: {step.targetName}",
                var t when t == ProcedureType.NOTICE => "알림",
                _ => step.type
            };

            buttonText.text = $"{step.name}\n{typeInfo} {timeInfo}";
            stepButton.interactable = (i == 0);
            stepButtons.Add(stepButton);

            int stepIndex = i;
            if (step.type.ToLower() == ProcedureType.ANIMATION)
            {
                stepButton.onClick.AddListener(() => StartStepAnimation(stepButton, step, stepIndex, isPreRequisite));
            }
            else
            {
                stepButton.onClick.AddListener(() => ProcessStep(stepButton, step, stepIndex, isPreRequisite));
            }
        }
    }

    void ProcessStep(Button button, Step step, int index, bool isPreRequisite)
    {
        if (index != currentStepIndex + 1) return;

        Text buttonText = button.GetComponentInChildren<Text>();
        string originalText = buttonText.text;

        StartCoroutine(ShowStepCompletion(button, buttonText, originalText, index, isPreRequisite));
    }

    IEnumerator ShowStepCompletion(Button button, Text buttonText, string originalText,
        int index, bool isPreRequisite)
    {
        button.interactable = false;
        buttonText.text = $"{originalText}\n처리중...";
        yield return new WaitForSeconds(0.5f);
        buttonText.text = $"{originalText}\n완료!";
        CompleteStep(index, isPreRequisite);
    }

    void StartStepAnimation(Button button, Step step, int index, bool isPreRequisite)
    {
        if (buttonCoroutines.ContainsKey(button) && buttonCoroutines[button] != null)
        {
            StopCoroutine(buttonCoroutines[button]);
            buttonCoroutines.Remove(button);
        }

        buttonCoroutines[button] = StartCoroutine(AnimateStep(button, step, index, isPreRequisite));
    }

    IEnumerator AnimateStep(Button button, Step step, int index, bool isPreRequisite)
    {
        if (index != currentStepIndex + 1) yield break;

        Text buttonText = button.GetComponentInChildren<Text>();
        string originalText = buttonText.text;
        float duration = step.duration > 0 ? step.duration : 2f;
        float elapsed = 0f;

        button.interactable = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = (elapsed / duration) * 100f;
            float remainingTime = duration - elapsed;
            buttonText.text = $"{step.name}\n진행중: {progress:F1}%\n남은 시간: {remainingTime:F1}초";
            yield return null;
        }

        buttonText.text = $"{originalText}\n완료!";
        CompleteStep(index, isPreRequisite);
    }

    void CompleteStep(int index, bool isPreRequisite)
    {
        currentStepIndex = index;

        if (index + 1 < stepButtons.Count)
        {
            stepButtons[index + 1].interactable = true;
        }
        else if (isPreRequisite)
        {
            ShowSteps(currentProcedure, false);
        }
    }

    void ClearPanel(Transform panel)
    {
        foreach (Transform child in panel)
        {
            Button button = child.GetComponent<Button>();
            if (buttonCoroutines.ContainsKey(button))
            {
                if (buttonCoroutines[button] != null)
                {
                    StopCoroutine(buttonCoroutines[button]);
                }
                buttonCoroutines.Remove(button);
            }
            Destroy(child.gameObject);
        }
    }

    void OnDisable()
    {
        foreach (var coroutine in buttonCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        buttonCoroutines.Clear();
    }
}