using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float forceMultiplier = 1000f;

    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
        Debug.Log("���������� �ʱ�ȭ �Ϸ�");
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

                // ���� ������ �������� �ణ ����
                force += Vector3.up * force.magnitude * 0.5f;

                if (force.magnitude > 0.01f)
                {
                    Vector3 contactPoint = other.ClosestPoint(transform.position);
                    tooth.ApplyElevatorForce(force, contactPoint);
                    Debug.Log($"���������� �� ����: {force.magnitude}");
                }
            }
        }
        lastPosition = transform.position;
    }
}