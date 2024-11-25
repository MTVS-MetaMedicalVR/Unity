using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuctionTool : MonoBehaviour
{
    public AudioSource suctionSound; // Suction AudioSource
    public Transform suctionTip; // ���� ������ ���κ� ��ġ
    public Transform targetArea; // ȯ���� Ư�� ���� Transform
    public float detectionRadius = 0.1f; // Ư�� ���� �ݰ�

    private bool isSuctionActive = false;

    void Update()
    {
        // ���� ���� ���κа� Ư�� ���� ������ �Ÿ� ���
        float distanceToTarget = Vector3.Distance(suctionTip.position, targetArea.position);

        // Ư�� ���� �ȿ� ������ �� Suction Sound ���
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
            // Ư�� �������� ����� Suction Sound ����
            if (isSuctionActive)
            {
                suctionSound.Stop();
                isSuctionActive = false;
                Debug.Log("Suction stopped.");
            }
        }
    }
}
