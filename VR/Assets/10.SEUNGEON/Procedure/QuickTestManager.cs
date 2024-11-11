using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;

public class QuickTestManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform categoryPanel;  // 목차 패널
    [SerializeField] private Transform procedurePanel; // 절차 리스트 패널
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject procedureButtonPrefab;

    [Header("Scene Settings")]
    [SerializeField] private string inGameSceneName = "InGameScene";

    private static class Paths
    {
        public static readonly string PROCEDURES_FOLDER = "ProcedureData";
        public static readonly string COMMON_FOLDER = "Common";
        public static readonly string PROCEDURE_INFO = "procedure.json";
        public static readonly string THUMBNAIL = "thumbnail.png";
    }

    void Start()
    {
        ScanCategoryFolders();
    }

    void ScanCategoryFolders()
    {
        string basePath = Path.Combine(Application.streamingAssetsPath, Paths.PROCEDURES_FOLDER);

        if (!Directory.Exists(basePath))
        {
            Debug.LogError($"Base procedures folder not found at: {basePath}");
            return;
        }

        // Common 폴더를 제외한 모든 카테고리 폴더 스캔
        var directories = Directory.GetDirectories(basePath)
                                 .Where(d => !Path.GetFileName(d).Equals(Paths.COMMON_FOLDER));

        foreach (string categoryPath in directories)
        {
            string categoryName = Path.GetFileName(categoryPath);
            CreateCategoryButton(categoryName, categoryPath);
        }
    }

    void CreateCategoryButton(string categoryName, string categoryPath)
    {
        GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryPanel);
        Button button = buttonObj.GetComponent<Button>();
        Text buttonText = buttonObj.GetComponentInChildren<Text>();

        // 버튼 텍스트 설정
        if (buttonText != null)
        {
            buttonText.text = categoryName;
        }

        // 클릭 이벤트 설정
        if (button != null)
        {
            button.onClick.AddListener(() => ShowProcedures(categoryPath, categoryName));
        }
    }

    void ShowProcedures(string categoryPath, string categoryName)
    {
        // 기존 절차 버튼들 제거
        foreach (Transform child in procedurePanel)
        {
            Destroy(child.gameObject);
        }

        // 해당 카테고리의 모든 절차 폴더 스캔
        foreach (string procedureFolder in Directory.GetDirectories(categoryPath))
        {
            string jsonPath = Path.Combine(procedureFolder, Paths.PROCEDURE_INFO);
            string thumbnailPath = Path.Combine(procedureFolder, Paths.THUMBNAIL);

            if (File.Exists(jsonPath))
            {
                try
                {
                    // 절차 정보 로드
                    string jsonContent = File.ReadAllText(jsonPath);
                    Procedure procedure = JsonUtility.FromJson<Procedure>(jsonContent);

                    // 절차 버튼 생성
                    GameObject buttonObj = Instantiate(procedureButtonPrefab, procedurePanel);

                    // 썸네일 이미지 설정
                    if (File.Exists(thumbnailPath))
                    {
                        Image buttonImage = buttonObj.GetComponent<Image>();
                        if (buttonImage != null)
                        {
                            StartCoroutine(LoadThumbnail(thumbnailPath, buttonImage));
                        }
                    }

                    // 버튼 텍스트 설정
                    Text buttonText = buttonObj.GetComponentInChildren<Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = $"{procedure.name}\n{procedure.description}";
                    }

                    // 클릭 이벤트 설정
                    Button button = buttonObj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => StartProcedure(categoryName, procedure));
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error loading procedure from {jsonPath}: {e.Message}");
                }
            }
        }
    }

    IEnumerator LoadThumbnail(string path, Image targetImage)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture("file://" + path))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                targetImage.sprite = Sprite.Create(texture,
                    new Rect(0, 0, texture.width, texture.height),
                    Vector2.one * 0.5f);
            }
        }
    }

    void StartProcedure(string category, Procedure procedure)
    {
        // 선택한 절차 정보 저장
        ProcedureSceneManager.Instance.SetProcedure(category, procedure.id);

        // 인게임 씬으로 전환
        SceneManager.LoadScene(inGameSceneName);
    }
}