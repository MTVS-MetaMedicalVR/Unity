
// 1. ProcedureSceneManager.cs - 씬 간 데이터 전달용
using UnityEngine;

public class ProcedureSceneManager : MonoBehaviour
{
    private static ProcedureSceneManager instance;
    public static ProcedureSceneManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ProcedureSceneManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("ProcedureSceneManager");
                    instance = go.AddComponent<ProcedureSceneManager>();
                }
            }
            return instance;
        }
    }

    public string CurrentCategory { get; private set; }
    public string CurrentProcedureId { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetProcedure(string category, string procedureId)
    {
        CurrentCategory = category;
        CurrentProcedureId = procedureId;
    }

    public void ClearProcedure()
    {
        CurrentCategory = null;
        CurrentProcedureId = null;
    }
}