using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using UnityEngine.Networking;

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
        CopyStreamingAssetsToPersistent();
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
    private void CopyStreamingAssetsToPersistent()
    {
        // Common ���� ����
        CopyFolderToPersistent("ProcedureData/Common");

        // ���� ī�װ� ���� ����
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        CopyFolderToPersistent($"ProcedureData/{category}/{procedureId}");
    }

    private async void CopyFolderToPersistent(string relativePath)
    {
        string srcPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string destPath = Path.Combine(Application.persistentDataPath, relativePath);

        // �ȵ���̵忡���� StreamingAssets�� jar ���ο� �־ UnityWebRequest�� ���
        if (Application.platform == RuntimePlatform.Android)
        {
            string wwwPath = "jar:file://" + srcPath;
            using (UnityWebRequest www = UnityWebRequest.Get(wwwPath))
            {
                var operation = www.SendWebRequest();
                while (!operation.isDone)
                {
                    await System.Threading.Tasks.Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    if (!Directory.Exists(destPath))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    }
                    File.WriteAllBytes(destPath, www.downloadHandler.data);
                }
                else
                {
                    Debug.LogError($"Failed to copy file: {www.error}");
                }
            }
        }
        else
        {
            // PC������ ���� ���� ���� ����
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }

            foreach (string dirPath in Directory.GetDirectories(srcPath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(srcPath, destPath));
            }

            foreach (string filePath in Directory.GetFiles(srcPath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(filePath, filePath.Replace(srcPath, destPath), true);
            }
        }
    }
    private string GetProperPath(string relativePath)
    {
        // �ȵ���̵忡���� persistentDataPath ���, �� �ܿ����� streamingAssetsPath ���
        string basePath = Application.platform == RuntimePlatform.Android ?
            Application.persistentDataPath : Application.streamingAssetsPath;
        return Path.Combine(basePath, relativePath);
    }

    private void LoadProcedures()
    {
        // Common ������ �⺻ ������ �ε�
        string commonPath = GetProperPath("ProcedureData/Common");
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

        // ���� ���� �ε�
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        string path = GetProperPath($"ProcedureData/{category}/{procedureId}/procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            currentProcedure = JsonUtility.FromJson<Procedure>(jsonContent);
        }

        // �ʱ� steps ����
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