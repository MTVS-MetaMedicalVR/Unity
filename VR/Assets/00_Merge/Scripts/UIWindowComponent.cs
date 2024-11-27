// 별도의 UIWindow 컴포넌트 
using UnityEngine;

public class UIWindowComponent : MonoBehaviour
{
    public string windowId;
    public GameObject windowObject;

    void Awake()
    {
        if (windowObject == null)
            windowObject = gameObject;
    }
}