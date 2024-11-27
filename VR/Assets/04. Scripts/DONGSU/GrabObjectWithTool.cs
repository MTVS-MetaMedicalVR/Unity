using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObjectWithTool : MonoBehaviour
{
    // ���� ��� �ִ� ������Ʈ
    private GameObject currentObject = null;

    // �ɼ�, ������ ���� ������ Collider
    public Collider toolCollider;

    // ������Ʈ�� ���� �� �ִ� Ʈ����(Collider)�� ����
    public string releaseZoneTag = "ReleaseZone";

    // ������Ʈ�� Grab�� ��ġ
    private Transform grabPoint;

    // Release ���� Grab ���� �������� bool�� ��ȯ
    private bool recentlyReleased = false;

    // ��ư��(��)�� �Կ� �������� ȯ�� �� �ݰ��ϱ� ���� ��ũ��Ʈ
    JawFollow jawFollow;

    // Ư�� ������ ���� ������Ʈ�� ��� ���� ��� Ȱ��ȭ
    // Start is called before the first frame update
    void Start()
    {
        // GrabPoint ������Ʈ�� �ڽ� �߿��� ã��.
        grabPoint = transform.Find("GrabPoint");
        if (grabPoint == null)
        {
            Debug.LogError("GrabPoint�� ã�� �� �����ϴ�.");
        }

        // jawFollow ������Ʈ�� ��������.
        jawFollow = GameObject.Find("JawSource A").GetComponent<JawFollow>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (recentlyReleased) return;
        // ���� �� �ִ� ������Ʈ�� ����� ��
        if (other.CompareTag("Grabbable") && currentObject == null)
        {
            GrabObject(other.gameObject);
            Debug.Log("��ü�� ��ҽ��ϴ� !");
        }

        // ReleaseZone�� ����� �� ������Ʈ�� ����.
        else if (other.CompareTag(releaseZoneTag) && currentObject != null)
        {
            ReleaseObject();
            Debug.Log("��ü�� ���ҽ��ϴ� !");
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

        // ������Ʈ�� currentObject�� �����ϰ�, �� ��ġ�� ������ ����
        currentObject = obj;
        // Tool�� �ڽ����� ����
        currentObject.transform.SetParent(grabPoint);
        currentObject.transform.localPosition = Vector3.zero;
        currentObject.transform.localRotation = Quaternion.identity;

        // ������Ʈ�� Rigidbody �Ӽ��� ������ �ʿ��� ���� ����
        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // ������ ������ ���� �ʵ��� ��������.
            rb.isKinematic = true;
        }
    }

    private void ReleaseObject()
    {
        // currentObject�� �θ� ���� ����
        if (currentObject != null)
        {
            currentObject.transform.SetParent(null);

            // Rigidbody�� ����Ͽ� �߷°� ����ȿ���� ����ǵ��� ��������.
            Rigidbody rb = currentObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ������ ȿ�� �ٽ� Ȱ��ȭ����.
                rb.isKinematic = false;
            }

            StartCoroutine(PreventImmediateGrab());

            // currentObject �ʱ�ȭ
            currentObject = null;
        }

        jawFollow.isOpen = false;
    }
}