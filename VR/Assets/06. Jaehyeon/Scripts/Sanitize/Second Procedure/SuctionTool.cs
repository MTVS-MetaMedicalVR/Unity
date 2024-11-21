using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuctionTool : MonoBehaviour
{
    public AudioSource suctionSound; // Suction AudioSource
    public Transform suctionTip; // 석션 도구의 끝부분 위치
    public Transform targetArea; // 환자의 특정 영역 Transform
    public float detectionRadius = 0.1f; // 특정 영역 반경

    private bool isSuctionActive = false;

    void Update()
    {
        // 석션 도구 끝부분과 특정 영역 사이의 거리 계산
        float distanceToTarget = Vector3.Distance(suctionTip.position, targetArea.position);

        // 특정 영역 안에 들어왔을 때 Suction Sound 재생
        if (distanceToTarget <= detectionRadius)
        {
            if (!isSuctionActive)
            {
                suctionSound.Play();
                isSuctionActive = true;
                Debug.Log("Suction started.");
            }
        }
        else
        {
            // 특정 영역에서 벗어나면 Suction Sound 종료
            if (isSuctionActive)
            {
                suctionSound.Stop();
                isSuctionActive = false;
                Debug.Log("Suction stopped.");
            }
        }
    }
}
