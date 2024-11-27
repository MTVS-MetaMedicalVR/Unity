using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float forceMultiplier = 1000f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        Debug.Log("엘리베이터 초기화 완료");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Tooth"))
        {
            Tooth tooth = other.GetComponent<Tooth>();
            if (tooth != null)
            {
                Vector3 movement = (transform.position - lastPosition);
                Vector3 force = movement * forceMultiplier;

                // 힘의 방향을 위쪽으로 약간 편향
                force += Vector3.up * force.magnitude * 0.5f;

                if (force.magnitude > 0.01f)
                {
                    Vector3 contactPoint = other.ClosestPoint(transform.position);
                    tooth.ApplyElevatorForce(force, contactPoint);
                    Debug.Log($"엘리베이터 힘 적용: {force.magnitude}");
                }
            }
        }
        lastPosition = transform.position;
    }
}