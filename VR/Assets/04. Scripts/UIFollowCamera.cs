using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollowCamera : MonoBehaviour
{
    // Follw�� ī�޶�
    public Camera targetCamera;
    // ī�޶�� UI ���� �Ÿ�
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
            // UI�� ��ġ�� ī�޶� ��ġ + ���������� ����
            transform.position = targetCamera.transform.position + targetCamera.transform.forward * offset.z
                                 + targetCamera.transform.up * offset.y
                                 + targetCamera.transform.right * offset.x;

            // UI�� �׻� ī�޶� ���ϵ��� ȸ��, �������� ������ ���ϱ� ���� Axis ���� ó��
            Vector3 lookDirection = transform.position - targetCamera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
