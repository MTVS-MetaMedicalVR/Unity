using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    // Follw할 카메라
    public Camera targetCamera;
    // 카메라와 UI 사이 거리
    public Vector3 offset = new Vector3(0, 0, 0.5f);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCamera != null)
        {
            // UI의 위치를 카메라 위치 + 오프셋으로 서정
            transform.position = targetCamera.transform.position + targetCamera.transform.forward * offset.z
                                 + targetCamera.transform.up * offset.y
                                 + targetCamera.transform.right * offset.x;

            // UI가 항상 카메라를 향하도록 회전, 뒤집히는 문제를 피하기 위해 Axis 반전 처리
            Vector3 lookDirection = transform.position - targetCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
