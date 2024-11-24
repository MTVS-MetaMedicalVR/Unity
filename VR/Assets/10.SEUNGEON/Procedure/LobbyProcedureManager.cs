using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class LobbyProcedureManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform categoryPanel;
    [SerializeField] private Transform procedurePanel;
    [SerializeField] private GameObject categoryButtonPrefab;
    [SerializeField] private GameObject procedureButtonPrefab;

    [Header("Scene Settings")]
    [SerializeField] private string inGameSceneName = "InGameScene";

    private Toggle lastSelectedCategoryToggle;
    private Toggle lastSelectedProcedureToggle;

    private static class Paths
    {
        public static readonly string PROCEDURES_FOLDER = "ProcedureData";
        public static readonly string COMMON_FOLDER = "Common";
        public static readonly string PROCEDURE_INFO = "procedure";
        public static readonly string THUMBNAIL = "thumbnail";
    }

    void Start()
    {
        ScanCategories();
    }

    void ScanCategories()
    {
        // Resources.LoadAll을 사용하여 모든 카테고리 폴더 로드
        Object[] categories = Resources.LoadAll(Paths.PROCEDURES_FOLDER, typeof(Object));
        var categoryFolders = categories
            .Select(c => c.name)
            .Distinct()
            .Where(name => !name.Equals(Paths.COMMON_FOLDER));

        foreach (string categoryName in categoryFolders)
        {
            CreateCategoryButton(categoryName);
        }
    }

    void CreateCategoryButton(string categoryName)
    {
        GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryPanel);
        Toggle toggle = buttonObj.GetComponentInChildren<Toggle>();
        TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = categoryName;
        }

        if (toggle != null)
        {
            toggle.group = categoryPanel.GetComponentInChildren<ToggleGroup>();
            toggle.onValueChanged.AddListener((bool isOn) => {
                if (isOn)
                {
                    if (lastSelectedCategoryToggle != null && lastSelectedCategoryToggle != toggle)
                    {
                        lastSelectedCategoryToggle.isOn = false;
                    }
                    lastSelectedCategoryToggle = toggle;
                    ShowProcedures(categoryName);
                }
            });
        }
    }

    void ShowProcedures(string categoryName)
    {
        foreach (Transform child in procedurePanel)
        {
            Destroy(child.gameObject);
        }

        string categoryPath = $"{Paths.PROCEDURES_FOLDER}/{categoryName}";
        TextAsset[] procedureInfos = Resources.LoadAll<TextAsset>(categoryPath);

        foreach (TextAsset procedureInfo in procedureInfos)
        {
            if (procedureInfo.name.EndsWith(Paths.PROCEDURE_INFO))
            {
                try
                {
                    Procedure procedure = JsonUtility.FromJson<Procedure>(procedureInfo.text);
                    GameObject buttonObj = Instantiate(procedureButtonPrefab, procedurePanel);

                    // 썸네일 로드
                    string thumbnailPath = $"{categoryPath}/{procedure.id}/{Paths.THUMBNAIL}";
                    Sprite thumbnailSprite = Resources.Load<Sprite>(thumbnailPath);

                    Image buttonImage = buttonObj.GetComponentInChildren<Image>();
                    if (buttonImage != null && thumbnailSprite != null)
                    {
                        buttonImage.sprite = thumbnailSprite;
                    }

                    TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (buttonText != null)
                    {
                        buttonText.text = $"{procedure.name}\n{procedure.description}";
                    }

                    Toggle toggle = buttonObj.GetComponentInChildren<Toggle>();
                    if (toggle != null)
                    {
                        toggle.group = procedurePanel.GetComponentInChildren<ToggleGroup>();
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
                    Debug.LogError($"Error loading procedure from {procedureInfo.name}: {e.Message}");
                }
            }
        }
    }

    void StartProcedure(string category, Procedure procedure)
    {
        ProcedureSceneManager.Instance.SetProcedure(category, procedure.id);
        SceneManager.LoadScene(inGameSceneName);
    }
}