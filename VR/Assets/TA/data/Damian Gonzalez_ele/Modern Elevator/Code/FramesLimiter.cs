using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramesLimiter : MonoBehaviour
{
    [SerializeField] private int desiredFrameRate = 30;
    private int oldValue=0;
    void OnEnable()
    {
        InvokeRepeating(nameof(SlowUpdate), 0f, .5f);
    }

    void OnDisable() {
        Application.targetFrameRate = 0;
        CancelInvoke(nameof(SlowUpdate));
    }

    void SlowUpdate() {
        if (oldValue != desiredFrameRate) {
            Application.targetFrameRate = desiredFrameRate;
            QualitySettings.vSyncCount = 0;
            oldValue = desiredFrameRate;
        }
    }

}
