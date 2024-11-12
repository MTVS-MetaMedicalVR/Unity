using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System.Collections;
using UnityEngine.Networking;
using TMPro;  // TMPro ���ӽ����̽� �߰�

public class QuickTestManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform categoryPanel;
    [SerializeField] private Transform procedurePanel;
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject procedureButtonPrefab;

    [Header("Scene Settings")]
    [SerializeField] private string inGameSceneName = "InGameScene";

    private Toggle lastSelectedCategoryToggle;    // ������ ���õ� ī�װ� ���
    private Toggle lastSelectedProcedureToggle;   // ������ ���õ� ���� ���

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
        Toggle toggle = buttonObj.GetComponent<Toggle>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = categoryName;
        }

        if (toggle != null)
        {
            toggle.group = categoryPanel.GetComponent<ToggleGroup>();
            toggle.onValueChanged.AddListener((bool isOn) => {
                if (isOn)
                {
                    if (lastSelectedCategoryToggle != null && lastSelectedCategoryToggle != toggle)
                    {
                        lastSelectedCategoryToggle.isOn = false;
                    }
                    lastSelectedCategoryToggle = toggle;
                    ShowProcedures(categoryPath, categoryName);
                }
            });
        }
    }

    void ShowProcedures(string categoryPath, string categoryName)
    {
        foreach (Transform child in procedurePanel)
        {
            Destroy(child.gameObject);
        }

        foreach (string procedureFolder in Directory.GetDirectories(categoryPath))
        {
            string jsonPath = Path.Combine(procedureFolder, Paths.PROCEDURE_INFO);
            string thumbnailPath = Path.Combine(procedureFolder, Paths.THUMBNAIL);

            if (File.Exists(jsonPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(jsonPath);
                    Procedure procedure = JsonUtility.FromJson<Procedure>(jsonContent);

                    GameObject buttonObj = Instantiate(procedureButtonPrefab, procedurePanel);

                    if (File.Exists(thumbnailPath))
                    {
                        Image buttonImage = buttonObj.GetComponent<Image>();
                        if (buttonImage != null)
                        {
                            StartCoroutine(LoadThumbnail(thumbnailPath, buttonImage));
                        }
                    }

                    TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = $"{procedure.name}\n{procedure.description}";
                    }

                    Toggle toggle = buttonObj.GetComponent<Toggle>();
                    if (toggle != null)
                    {
                        toggle.group = procedurePanel.GetComponent<ToggleGroup>();
                        toggle.onValueChanged.AddListener((bool isOn) => {
                            if (isOn)
                            {
                                if (lastSelectedProcedureToggle != null && lastSelectedProcedureToggle != toggle)
                                {
                                    lastSelectedProcedureToggle.isOn = false;
                                }
                                lastSelectedProcedureToggle = toggle;
                                StartProcedure(categoryName, procedure);
                            }
                        });
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
        ProcedureSceneManager.Instance.SetProcedure(category, procedure.id);
        SceneManager.LoadScene(inGameSceneName);
    }
}