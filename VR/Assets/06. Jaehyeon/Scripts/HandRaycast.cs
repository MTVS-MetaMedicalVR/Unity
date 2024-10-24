using UnityEngine;
using UnityEngine.EventSystems;  // �̺�Ʈ �ý���
using UnityEngine.UI;            // UI ������Ʈ ���

public class HandRaycast : MonoBehaviour
{
    public Camera mainCamera;  // Ray�� �� ī�޶� ����

    void Update()
    {
        // ���콺 Ŭ�� �Ǵ� ��ġ�� �����Ǿ��� �� ����
        if (Input.GetMouseButtonDown(0))  // VR ��Ʈ�ѷ������� ��ư �Է��� �̿� ������ �� ����
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // ���콺 ��ġ�κ��� Ray ����

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ray�� �浹�� ������Ʈ�� Button ������Ʈ�� �ִ��� Ȯ��
                Button button = hit.collider.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.Invoke();  // ��ư Ŭ�� �̺�Ʈ ����
                    Debug.Log($"{button.name} ��ư�� Ŭ���Ǿ����ϴ�.");
                }
            }
        }
    }
}
