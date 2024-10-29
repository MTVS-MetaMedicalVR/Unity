using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// ProcedureTypes.cs - 프로시저 타입 정의
public enum ProcedureInteractionType
{
    Movement,      // 특정 영역으로 이동
    Outline,       // 아웃라인이 있는 오브젝트와 상호작용
    Animation,     // 애니메이션 실행
    ParticleTimer  // 파티클 효과와 타이머
}

[System.Serializable]
public class TempProcedure
{
    public string id;
    public string title;
    public string description;
}



[System.Serializable]
public class TempProcedureData
{
    public List<TempProcedure> datas;
}


// TempProcedureManager.cs - 프로시저 관리자
public class TempProcedureManager : MonoBehaviour
{
    public static TempProcedureManager Instance { get; private set; }

    public TempProcedureData procedureData;
    public static readonly string fileName = "handwashing_procedure.json";

    [Header("UI")]
    public Canvas procedureUI;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Toggle confirmButton;

    [SerializeField]
    private List<TempProcedureObjectBase> orderedProcedureObjects = new List<TempProcedureObjectBase>();
    private int currentProcedureIndex = 0;
    private bool isWaitingForConfirm = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        procedureData = LoadFromStreamingAssets(fileName);
        ArrangeProcedureObjects();
    }

    void Start()
    {
        confirmButton?.onValueChanged.AddListener(OnConfirmButtonClick);
        StartNextProcedure();
    }

    private void ArrangeProcedureObjects()
    {
        var allProcedureObjects = FindObjectsOfType<TempProcedureObjectBase>();
        orderedProcedureObjects.Clear();

        foreach (var procedure in procedureData.datas)
        {
            var matchingObject = System.Array.Find(allProcedureObjects,
                obj => obj.procedureId == procedure.id);

            if (matchingObject != null)
            {
                matchingObject.tempProcedure = procedure;
                orderedProcedureObjects.Add(matchingObject);
            }
            else
            {
                Debug.LogWarning($"Procedure object with ID {procedure.id} not found in scene");
            }
        }
    }

    void Update()
    {
        if (!isWaitingForConfirm && currentProcedureIndex < orderedProcedureObjects.Count)
        {
            if (orderedProcedureObjects[currentProcedureIndex].isDone)
            {
                CompleteCurrentProcedure();
            }
        }
    }

    public string GetCurrentProcedureId()
    {
        if (currentProcedureIndex < orderedProcedureObjects.Count)
        {
            return orderedProcedureObjects[currentProcedureIndex].procedureId;
        }
        return string.Empty;
    }

    void StartNextProcedure()
    {
        if (currentProcedureIndex >= orderedProcedureObjects.Count)
        {
            Debug.Log("모든 절차를 완료했습니다.");
            return;
        }

        var currentProcedure = orderedProcedureObjects[currentProcedureIndex].tempProcedure;
        ShowProcedureUI(currentProcedure.title, currentProcedure.description);
        orderedProcedureObjects[currentProcedureIndex].gameObject.SetActive(true);
        isWaitingForConfirm = true;
    }

    void ShowProcedureUI(string title, string description)
    {
        if (procedureUI != null)
        {
            procedureUI.gameObject.SetActive(true);
            confirmButton.isOn = false;
            titleText.text = title;
            descriptionText.text = description;
        }
    }

    void OnConfirmButtonClick(bool isOn)
    {
        if (!isWaitingForConfirm) return;

        procedureUI.gameObject.SetActive(false);
        isWaitingForConfirm = false;
    }

    void CompleteCurrentProcedure()
    {
        currentProcedureIndex++;
        StartNextProcedure();
    }

    public TempProcedureData LoadFromStreamingAssets(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"JSON file not found at path: {filePath}");
            return null;
        }
        string jsonContent = File.ReadAllText(filePath);
        return JsonUtility.FromJson<TempProcedureData>(jsonContent);
    }
}