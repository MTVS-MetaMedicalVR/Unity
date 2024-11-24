using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    // �� �̸��� ������ ����
    public string sceneToLoad;

    // ��ư�� Ŭ������ �� ȣ���� �޼���
    public void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("Scene name is not set.");
        }
    }
}
