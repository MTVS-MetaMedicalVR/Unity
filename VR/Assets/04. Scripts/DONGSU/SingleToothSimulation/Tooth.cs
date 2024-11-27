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
        // 초기 위치 저장
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;

        // Rigidbody 설정
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.mass = 1f;
            rb.drag = 10f;
            rb.angularDrag = 10f;
        }
        //승언 수정(에러 임시 수정)
        Rigidbody tempRigidBody  = this.transform.parent.GetComponent<Rigidbody>();
        // Joint 설정
        SetupSimpleJoint(tempRigidBody);
    }

    void SetupSimpleJoint(Rigidbody parentRb)
    {
        if (joint != null) Destroy(joint);
        joint = gameObject.AddComponent<ConfigurableJoint>();

        // 기본 설정
        joint.connectedBody = parentRb;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedAnchor = initialPosition;

        // 모든 움직임을 Locked로 시작
        joint.xMotion = joint.yMotion = joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = joint.angularYMotion = joint.angularZMotion = ConfigurableJointMotion.Locked;

        // 더 강한 Drive 설정
        var drive = new JointDrive
        {
            positionSpring = springForce,
            positionDamper = damperForce,
            maximumForce = maxForce
        };

        joint.xDrive = joint.yDrive = joint.zDrive = drive;
        joint.angularXDrive = joint.angularYZDrive = drive;
        joint.rotationDriveMode = RotationDriveMode.XYAndZ;

        // 0.5초 후에 움직임 제한 해제
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

            // 매우 작은 제한 설정
            var linearLimit = new SoftJointLimit { limit = 0.001f };
            joint.linearLimit = linearLimit;

            var angularLimit = new SoftJointLimit { limit = 1f };
            joint.lowAngularXLimit = joint.highAngularXLimit = joint.angularYLimit = joint.angularZLimit = angularLimit;
        }
    }

    public void ApplyElevatorForce(Vector3 force, Vector3 contactPoint)
    {
        if (isLoosened || rb == null) return;

        // 힘 스케일링 및 적용
        Vector3 scaledForce = force * elevatorForceMultiplier;
        rb.AddForceAtPosition(scaledForce, contactPoint, ForceMode.Impulse);

        // 진행도 업데이트
        float forceMagnitude = scaledForce.magnitude;
        looseningProgress += forceMagnitude * Time.fixedDeltaTime * looseningSpeed;
        looseningProgress = Mathf.Clamp01(looseningProgress);

        if (looseningProgress >= looseningThreshold && !isLoosened)
        {
            OnToothLoosened();
        }

        Debug.Log($"치아에 힘 적용: {forceMagnitude}, 진행도: {looseningProgress}");
    }

    void OnToothLoosened()
    {
        isLoosened = true;
        Debug.Log("치아가 충분히 흔들립니다!");

        if (joint != null)
        {
            // Joint 제한 완화
            var linearLimit = new SoftJointLimit { limit = 0.01f };
            joint.linearLimit = linearLimit;

            var angularLimit = new SoftJointLimit { limit = 5f };
            joint.lowAngularXLimit = joint.highAngularXLimit = joint.angularYLimit = joint.angularZLimit = angularLimit;

            // Drive 힘 감소
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
            Debug.Log("아직 치아가 충분히 흔들리지 않았습니다.");
            return;
        }

        if (isBeingExtracted)
        {
            Debug.Log("이미 발치가 진행 중입니다.");
            return;
        }

        isBeingExtracted = true;
        StartCoroutine(ExtractingProcess(forceps));
        Debug.Log("발치 시작");
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

            // 위쪽 방향의 움직임만 고려
            if (upwardMovement > 0)
            {
                float resistance = Mathf.Lerp(0.8f, 0.2f, extractionProgress);
                extractionProgress += upwardMovement * Time.deltaTime * extractionSpeed * resistance;
                extractionProgress = Mathf.Clamp01(extractionProgress);

                // 위치 업데이트
                transform.position = Vector3.Lerp(
                    startPos,
                    startPos + Vector3.up * 0.1f,
                    extractionProgress
                );

                // 약간의 흔들림 효과
                float wobbleAmount = (1f - extractionProgress) * 0.5f;
                transform.rotation = initialRotation * Quaternion.Euler(
                    Mathf.Sin(Time.time * 10f) * wobbleAmount,
                    Mathf.Sin(Time.time * 8f) * wobbleAmount,
                    Mathf.Sin(Time.time * 12f) * wobbleAmount
                );
            }

            Debug.Log($"발치 진행도: {extractionProgress:F2}");
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
            Debug.Log("발치 중단");
        }
    }

    void OnExtractionComplete()
    {
        Debug.Log("발치 완료!");
        if (joint != null)
        {
            Destroy(joint);
        }

        // 필요한 경우 여기에 추가 효과 구현
        // 예: 파티클 효과, 사운드 등
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            // 치아의 현재 상태 시각화
            Gizmos.color = Color.Lerp(Color.green, Color.red, looseningProgress);
            Gizmos.DrawWireSphere(transform.position, 0.05f);

            // 힘의 방향 표시
            if (rb != null && rb.velocity.magnitude > 0.01f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, transform.position + rb.velocity.normalized * 0.1f);
            }
        }
    }
}