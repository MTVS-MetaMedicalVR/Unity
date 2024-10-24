using UnityEngine;
using UnityEngine.EventSystems;  // 이벤트 시스템
using UnityEngine.UI;            // UI 컴포넌트 사용

public class HandRaycast : MonoBehaviour
{
    public Camera mainCamera;  // Ray를 쏠 카메라 지정

    void Update()
    {
        // 마우스 클릭 또는 터치가 감지되었을 때 실행
        if (Input.GetMouseButtonDown(0))  // VR 컨트롤러에서는 버튼 입력을 이와 연결할 수 있음
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);  // 마우스 위치로부터 Ray 생성

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Ray가 충돌한 오브젝트에 Button 컴포넌트가 있는지 확인
                Button button = hit.collider.GetComponent<Button>();

                if (button != null)
                {
                    button.onClick.Invoke();  // 버튼 클릭 이벤트 실행
                    Debug.Log($"{button.name} 버튼이 클릭되었습니다.");
                }
            }
        }
    }
}
