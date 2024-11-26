using UnityEngine;

public class SyringeInteraction : MonoBehaviour
{
    public Animator syringeAnimator; // �ֻ�� Animator
    public Transform syringeTip; // �ֻ�� �� �κ� (��ġ ������)
    public Transform patientMouth; // ȯ�� �� ��ġ
    public float activationDistance = 0.1f; // �ֻ� Ȱ��ȭ �Ÿ�

    private bool isInjected = false; // �ֻ簡 �Ϸ�Ǿ����� Ȯ��

    private void Update()
    {
        // �ֻ�Ⱑ ȯ�� �� ��ó�� �ְ� �ֻ簡 ������� �ʾ��� ���
        if (IsSyringeNearMouth() && !isInjected)
        {
            StartInjection();
        }
    }

    private bool IsSyringeNearMouth()
    {
        // �ֻ�� ���� ȯ�� �� ������ �Ÿ� ���
        if (syringeTip != null && patientMouth != null)
        {
            float distance = Vector3.Distance(syringeTip.position, patientMouth.position);
            return distance < activationDistance; // Ư�� �Ÿ� �̳��� ��� true ��ȯ
        }
        return false;
    }

    public void StartInjection()
    {
        if (!isInjected)
        {
            isInjected = true; // �ֻ� �Ϸ� ���� ����

            // �ִϸ��̼� Ʈ���� ����
            if (syringeAnimator != null)
            {
                syringeAnimator.SetTrigger("StartInjection");
            }

            Debug.Log("�ֻ� �ִϸ��̼��� ����Ǿ����ϴ�.");
        }
    }
}
