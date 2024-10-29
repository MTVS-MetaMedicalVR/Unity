using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// ProcedureTypes.cs - ���ν��� Ÿ�� ����
public enum ProcedureInteractionType
{
    Movement,      // Ư�� �������� �̵�
    Outline,       // �ƿ������� �ִ� ������Ʈ�� ��ȣ�ۿ�
    Animation,     // �ִϸ��̼� ����
    ParticleTimer  // ��ƼŬ ȿ���� Ÿ�̸�
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


// TempProcedureManager.cs - ���ν��� ������
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
            Debug.Log("��� ������ �Ϸ��߽��ϴ�.");
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