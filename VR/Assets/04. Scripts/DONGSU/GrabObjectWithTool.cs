using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectWithTool : MonoBehaviour
{
    // 현재 잡고 있는 오브젝트
    private GameObject currentObject = null;

    // 핀셋, 포셉과 같은 도구의 Collider
    public Collider toolCollider;

    // 오브젝트를 놓을 수 있는 트리거(Collider)를 지정
    public string releaseZoneTag = "ReleaseZone";

    // 오브젝트가 Grab될 위치
    private Transform grabPoint;

    // Release 직후 Grab 방지 목적으로 bool값 반환
    private bool recentlyReleased = false;

    // 코튼롤(솜)을 입에 가져오면 환자 입 닫게하기 위한 스크립트
    JawFollow jawFollow;

    // 특정 도구를 통해 오브젝트를 잡고 놓는 기능 활성화
    // Start is called before the first frame update
    void Start()
    {
        // GrabPoint 오브젝트를 자식 중에서 찾자.
        grabPoint = transform.Find("GrabPoint");
        if (grabPoint == null)
        {
            Debug.LogError("GrabPoint를 찾을 수 없습니다.");
        }

        // jawFollow 컴포넌트를 가져오자.
        jawFollow = GameObject.Find("JawSource A").GetComponent<JawFollow>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (recentlyReleased) return;
        // 잡을 수 있는 오브젝트와 닿았을 때
        if (other.CompareTag("Grabbable") && currentObject == null)
        {
            GrabObject(other.gameObject);
            Debug.Log("물체를 잡았습니다 !");
        }

        // ReleaseZone에 닿았을 때 오브젝트를 놓자.
        else if (other.CompareTag(releaseZoneTag) && currentObject != null)
        {
            ReleaseObject();
            Debug.Log("물체를 놓았습니다 !");
        }
    }

    private IEnumerator PreventImmediateGrab()
    {
        recentlyReleased = true;
        yield return new WaitForSeconds(1f);
        recentlyReleased = false;
    }
    private void GrabObject(GameObject obj)
    {
        if (grabPoint == null) return;

        // 오브젝트를 currentObject로 설정하고, 그 위치를 도구에 고정
        currentObject = obj;
        // Tool의 자식으로 설정
        currentObject.transform.SetParent(grabPoint);
        currentObject.transform.localPosition = Vector3.zero;
        currentObject.transform.localRotation = Quaternion.identity;

        // 오브젝트의 Rigidbody 속성을 가져와 필요한 설정 변경
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 물리적 영향을 받지 않도록 설정하자.
            rb.isKinematic = true;
        }
    }

    private void ReleaseObject()
    {
        // currentObject의 부모 관계 해제
        if (currentObject != null)
        {
            currentObject.transform.SetParent(null);

            // Rigidbody를 사용하여 중력과 물리효과가 적용되도록 설정하자.
            Rigidbody rb = currentObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // 물리적 효과 다시 활성화하자.
                rb.isKinematic = false;
            }

            StartCoroutine(PreventImmediateGrab());

            // currentObject 초기화
            currentObject = null;
        }

        jawFollow.isOpen = false;
    }
}