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
    [SerializeField] private TMP_Text warningText;  // 경고 텍스트 (오브젝트가 없을 때 표시)

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
        // Common 폴더 복사
        CopyFolderToPersistent("ProcedureData/Common");

        // 현재 카테고리 폴더 복사
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        CopyFolderToPersistent($"ProcedureData/{category}/{procedureId}");
    }

    private async void CopyFolderToPersistent(string relativePath)
    {
        string srcPath = Path.Combine(Application.streamingAssetsPath, relativePath);
        string destPath = Path.Combine(Application.persistentDataPath, relativePath);

        // 안드로이드에서는 StreamingAssets이 jar 내부에 있어서 UnityWebRequest를 사용
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
            // PC에서는 직접 파일 복사 가능
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
        // 안드로이드에서는 persistentDataPath 사용, 그 외에서는 streamingAssetsPath 사용
        string basePath = Application.platform == RuntimePlatform.Android ?
            Application.persistentDataPath : Application.streamingAssetsPath;
        return Path.Combine(basePath, relativePath);
    }

    private void LoadProcedures()
    {
        // Common 폴더의 기본 절차들 로드
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

        // 실제 절차 로드
        string category = ProcedureSceneManager.Instance.CurrentCategory;
        string procedureId = ProcedureSceneManager.Instance.CurrentProcedureId;
        string path = GetProperPath($"ProcedureData/{category}/{procedureId}/procedure.json");

        if (File.Exists(path))
        {
            string jsonContent = File.ReadAllText(path);
            currentProcedure = JsonUtility.FromJson<Procedure>(jsonContent);
        }

        // 초기 steps 설정
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
                // 다음 Common 절차로 이동
                currentCommonProcedureIndex++;
                if (currentCommonProcedureIndex < commonProcedures.Count)
                {
                    // 다음 Common 절차 시작
                    currentStepIndex = -1;
                    steps = commonProcedures[currentCommonProcedureIndex].steps;
                    ShowNextStep();
                    return;
                }
                else
                {
                    // Common 절차들이 모두 끝나면 실제 절차로 전환
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

        // UI 업데이트
        stepNameText.text = currentStep.name;
        if (isExecutingCommonProcedures)
        {
            stepDescriptionText.text = $"기본 절차 {currentCommonProcedureIndex + 1}/{commonProcedures.Count}\n단계 {currentStepIndex + 1}/{steps.Count}\n설명: {currentStep.description}";
            progressText.text = $"기본 절차 진행도: {currentCommonProcedureIndex + 1}/{commonProcedures.Count} - 단계: {currentStepIndex + 1}/{steps.Count}";
        }
        else
        {
            stepDescriptionText.text = $"단계 {currentStepIndex + 1}/{steps.Count}\n설명: {currentStep.description}";
            progressText.text = $"진행도: {currentStepIndex + 1}/{steps.Count}";
        }

        // 해당 ID를 가진 오브젝트가 있는지 확인하고 경고 메시지 표시
        var targetObject = procedureObjects.Find(obj => obj.ProcedureId == currentStep.targetName);

        // 알림 패널 표시
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
            // 오브젝트가 없더라도 바로 다음 단계로 진행
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