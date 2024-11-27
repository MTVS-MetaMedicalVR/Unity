using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectWithTool : MonoBehaviour
{
    private GameObject currentObject = null; // ���� ��� �ִ� ������Ʈ
    public Collider toolCollider;           // ������ Collider
    public string releaseZoneTag = "ReleaseZone"; // ReleaseZone �±�
    private Transform grabPoint;            // ������Ʈ�� Grab�� ��ġ
    private bool recentlyReleased = false;  // Release ���� �÷���
    private ConfigurableJoint joint;        // ConfigurableJoint �߰�
    private Rigidbody currentRigidbody;     // ���� ���� ������Ʈ�� Rigidbody
    public float extractionForce = 50f;     // ��ġ �� (������ ��)
    public float extractionDistance = 0.3f; // ��ġ �Ÿ�
    public float extractionSpeed = 2f;      // ��ġ �ӵ�

    void Start()
    {
        grabPoint = transform.Find("GrabPoint");
        if (grabPoint == null)
        {
            Debug.LogError("GrabPoint�� ã�� �� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (recentlyReleased) return;

        // ���� �� �ִ� ������Ʈ�� ����� ��
        if (other.CompareTag("Grabbable") && currentObject == null)
        {
            GrabObject(other.gameObject);
            Debug.Log("��ü�� ��ҽ��ϴ�!");
        }

        // ReleaseZone�� ����� �� ������Ʈ�� ����.
        else if (other.CompareTag(releaseZoneTag) && currentObject != null)
        {
            ReleaseObject();
            Debug.Log("��ü�� ���ҽ��ϴ�!");
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
            // ConfigurableJoint �߰�
            joint = currentObject.AddComponent<ConfigurableJoint>();
            joint.connectedBody = grabPoint.GetComponentInParent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = Vector3.zero;

            // Joint ����: �̵� ����
            JointDrive drive = new JointDrive
            {
                positionSpring = extractionForce,
                positionDamper = 5f,
                maximumForce = Mathf.Infinity
            };

            joint.xDrive = drive;
            joint.yDrive = drive;
            joint.zDrive = drive;

            // ��ġ �Ÿ� ����
            joint.linearLimit = new SoftJointLimit { limit = extractionDistance };

            // ȸ�� ���
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;

            // ���������� �ⱸ �������� �̵�
            StartCoroutine(MoveToGrabPoint());
        }
    }

    private IEnumerator MoveToGrabPoint()
    {
        if (currentObject == null || grabPoint == null) yield break;

        // ġ�ư� ���������� �ⱸ �������� �̵�
        while (Vector3.Distance(currentObject.transform.position, grabPoint.position) > 0.01f)
        {
            currentObject.transform.position = Vector3.Lerp(
                currentObject.transform.position,
                grabPoint.position,
                Time.deltaTime * extractionSpeed
            );

            yield return null; // ���� �����ӱ��� ���
        }

        Debug.Log("��ġ �Ϸ�!");
    }

    private void ReleaseObject()
    {
        if (currentObject != null)
        {
            // ConfigurableJoint ����
            if (joint != null)
            {
                Destroy(joint);
            }

            // Rigidbody ����
            if (currentRigidbody != null)
            {
                currentRigidbody.isKinematic = false;
            }

            StartCoroutine(PreventImmediateGrab());

            // currentObject �ʱ�ȭ
            currentObject = null;
            currentRigidbody = null;
        }
    }
}
