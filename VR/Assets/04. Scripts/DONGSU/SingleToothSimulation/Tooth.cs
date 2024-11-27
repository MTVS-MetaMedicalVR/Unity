using UnityEngine;
using System.Collections;

public class Tooth : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody rb;
    private ConfigurableJoint joint;

    [Header("Physics Settings")]
    public float springForce = 10000f;
    public float damperForce = 1000f;
    public float maxForce = 10000f;

    [Header("Extraction Settings")]
    public float looseningThreshold = 0.3f;
    public float looseningSpeed = 0.1f;
    public float elevatorForceMultiplier = 1f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float looseningProgress = 0f;
    private bool isLoosened = false;
    private bool isBeingExtracted = false;

    void Start()
    {
        // �ʱ� ��ġ ����
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Rigidbody ����
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.mass = 1f;
            rb.drag = 10f;
            rb.angularDrag = 10f;
        }
        //�¾� ����(���� �ӽ� ����)
        Rigidbody tempRigidBody  = this.transform.parent.GetComponent<Rigidbody>();
        // Joint ����
        SetupSimpleJoint(tempRigidBody);
    }

    void SetupSimpleJoint(Rigidbody parentRb)
    {
        if (joint != null) Destroy(joint);
        joint = gameObject.AddComponent<ConfigurableJoint>();

        // �⺻ ����
        joint.connectedBody = parentRb;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = initialPosition;

        // ��� �������� Locked�� ����
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;

        // �� ���� Drive ����
        var drive = new JointDrive
        {
            positionSpring = springForce,
            positionDamper = damperForce,
            maximumForce = maxForce
        };

        joint.xDrive = joint.yDrive = joint.zDrive = drive;
        joint.angularXDrive = joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;

        // 0.5�� �Ŀ� ������ ���� ����
        StartCoroutine(EnableMovementAfterDelay());
    }

    IEnumerator EnableMovementAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
        }

        if (joint != null)
        {
            joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Limited;

            // �ſ� ���� ���� ����
            var linearLimit = new SoftJointLimit { limit = 0.001f };
            joint.linearLimit = linearLimit;

            var angularLimit = new SoftJointLimit { limit = 1f };
            joint.lowAngularXLimit = joint.highAngularXLimit = joint.angularYLimit = joint.angularZLimit = angularLimit;
        }
    }

    public void ApplyElevatorForce(Vector3 force, Vector3 contactPoint)
    {
        if (isLoosened || rb == null) return;

        // �� �����ϸ� �� ����
        Vector3 scaledForce = force * elevatorForceMultiplier;
        rb.AddForceAtPosition(scaledForce, contactPoint, ForceMode.Impulse);

        // ���൵ ������Ʈ
        float forceMagnitude = scaledForce.magnitude;
        looseningProgress += forceMagnitude * Time.fixedDeltaTime * looseningSpeed;
        looseningProgress = Mathf.Clamp01(looseningProgress);

        if (looseningProgress >= looseningThreshold && !isLoosened)
        {
            OnToothLoosened();
        }

        Debug.Log($"ġ�ƿ� �� ����: {forceMagnitude}, ���൵: {looseningProgress}");
    }

    void OnToothLoosened()
    {
        isLoosened = true;
        Debug.Log("ġ�ư� ����� ��鸳�ϴ�!");

        if (joint != null)
        {
            // Joint ���� ��ȭ
            var linearLimit = new SoftJointLimit { limit = 0.01f };
            joint.linearLimit = linearLimit;

            var angularLimit = new SoftJointLimit { limit = 5f };
            joint.lowAngularXLimit = joint.highAngularXLimit = joint.angularYLimit = joint.angularZLimit = angularLimit;

            // Drive �� ����
            var drive = new JointDrive
            {
                positionSpring = springForce * 0.5f,
                positionDamper = damperForce * 0.5f,
                maximumForce = maxForce * 0.5f
            };

            joint.xDrive = joint.yDrive = joint.zDrive = drive;
            joint.angularXDrive = joint.angularYZDrive = drive;
        }
    }

    public void StartExtraction(Transform forceps)
    {
        if (!isLoosened)
        {
            Debug.Log("���� ġ�ư� ����� ��鸮�� �ʾҽ��ϴ�.");
            return;
        }

        if (isBeingExtracted)
        {
            Debug.Log("�̹� ��ġ�� ���� ���Դϴ�.");
            return;
        }

        isBeingExtracted = true;
        StartCoroutine(ExtractingProcess(forceps));
        Debug.Log("��ġ ����");
    }

    IEnumerator ExtractingProcess(Transform forceps)
    {
        Vector3 startPos = transform.position;
        float extractionProgress = 0f;
        float extractionSpeed = 0.5f;

        while (extractionProgress < 1f && isBeingExtracted)
        {
            Vector3 forcepsMovement = forceps.position - startPos;
            float upwardMovement = Vector3.Dot(forcepsMovement, Vector3.up);

            // ���� ������ �����Ӹ� ���
            if (upwardMovement > 0)
            {
                float resistance = Mathf.Lerp(0.8f, 0.2f, extractionProgress);
                extractionProgress += upwardMovement * Time.deltaTime * extractionSpeed * resistance;
                extractionProgress = Mathf.Clamp01(extractionProgress);

                // ��ġ ������Ʈ
                transform.position = Vector3.Lerp(
                    startPos,
                    startPos + Vector3.up * 0.1f,
                    extractionProgress
                );

                // �ణ�� ��鸲 ȿ��
                float wobbleAmount = (1f - extractionProgress) * 0.5f;
                transform.rotation = initialRotation * Quaternion.Euler(
                    Mathf.Sin(Time.time * 10f) * wobbleAmount,
                    Mathf.Sin(Time.time * 8f) * wobbleAmount,
                    Mathf.Sin(Time.time * 12f) * wobbleAmount
                );
            }

            Debug.Log($"��ġ ���൵: {extractionProgress:F2}");
            yield return null;
        }

        if (extractionProgress >= 1f)
        {
            OnExtractionComplete();
        }
    }

    public void StopExtraction()
    {
        if (isBeingExtracted)
        {
            isBeingExtracted = false;
            transform.localPosition = initialPosition;
            transform.localRotation = initialRotation;
            Debug.Log("��ġ �ߴ�");
        }
    }

    void OnExtractionComplete()
    {
        Debug.Log("��ġ �Ϸ�!");
        if (joint != null)
        {
            Destroy(joint);
        }

        // �ʿ��� ��� ���⿡ �߰� ȿ�� ����
        // ��: ��ƼŬ ȿ��, ���� ��
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // ġ���� ���� ���� �ð�ȭ
            Gizmos.color = Color.Lerp(Color.green, Color.red, looseningProgress);
            Gizmos.DrawWireSphere(transform.position, 0.05f);

            // ���� ���� ǥ��
            if (rb != null && rb.velocity.magnitude > 0.01f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + rb.velocity.normalized * 0.1f);
            }
        }
    }
}