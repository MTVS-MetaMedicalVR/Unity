using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectWithTool : MonoBehaviour
{
    private GameObject currentObject = null; // 현재 잡고 있는 오브젝트
    public Collider toolCollider;           // 도구의 Collider
    public string releaseZoneTag = "ReleaseZone"; // ReleaseZone 태그
    private Transform grabPoint;            // 오브젝트가 Grab될 위치
    private bool recentlyReleased = false;  // Release 방지 플래그
    private ConfigurableJoint joint;        // ConfigurableJoint 추가
    private Rigidbody currentRigidbody;     // 현재 잡은 오브젝트의 Rigidbody
    public float extractionForce = 50f;     // 발치 힘 (스프링 값)
    public float extractionDistance = 0.3f; // 발치 거리
    public float extractionSpeed = 2f;      // 발치 속도

    void Start()
    {
        grabPoint = transform.Find("GrabPoint");
        if (grabPoint == null)
        {
            Debug.LogError("GrabPoint를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recentlyReleased) return;

        // 잡을 수 있는 오브젝트와 닿았을 때
        if (other.CompareTag("Grabbable") && currentObject == null)
        {
            GrabObject(other.gameObject);
            Debug.Log("물체를 잡았습니다!");
        }

        // ReleaseZone에 닿았을 때 오브젝트를 놓자.
        else if (other.CompareTag(releaseZoneTag) && currentObject != null)
        {
            ReleaseObject();
            Debug.Log("물체를 놓았습니다!");
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

        currentObject = obj;
        currentRigidbody = currentObject.GetComponent<Rigidbody>();

        if (currentRigidbody != null)
        {
            // ConfigurableJoint 추가
            joint = currentObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = Vector3.zero;

            // Joint 설정: 이동 제어
            JointDrive drive = new JointDrive
            {
                positionSpring = extractionForce,
                positionDamper = 5f,
                maximumForce = Mathf.Infinity
            };

            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;

            // 발치 거리 제한
            joint.linearLimit = new SoftJointLimit { limit = extractionDistance };

            // 회전 잠금
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            // 점진적으로 기구 방향으로 이동
            StartCoroutine(MoveToGrabPoint());
        }
    }

    private IEnumerator MoveToGrabPoint()
    {
        if (currentObject == null || grabPoint == null) yield break;

        // 치아가 점진적으로 기구 방향으로 이동
        while (Vector3.Distance(currentObject.transform.position, grabPoint.position) > 0.01f)
        {
            currentObject.transform.position = Vector3.Lerp(
                currentObject.transform.position,
                grabPoint.position,
                Time.deltaTime * extractionSpeed
            );

            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("발치 완료!");
    }

    private void ReleaseObject()
    {
        if (currentObject != null)
        {
            // ConfigurableJoint 삭제
            if (joint != null)
            {
                Destroy(joint);
            }

            // Rigidbody 설정
            if (currentRigidbody != null)
            {
                currentRigidbody.isKinematic = false;
            }

            StartCoroutine(PreventImmediateGrab());

            // currentObject 초기화
            currentObject = null;
            currentRigidbody = null;
        }
    }
}
