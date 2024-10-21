using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentOVRCameraRig : MonoBehaviour
{
    private static PersistentOVRCameraRig instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �ߺ� ���� ����
            Destroy(gameObject);
        }
    }
}
