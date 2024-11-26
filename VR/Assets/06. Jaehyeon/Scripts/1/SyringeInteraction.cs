using UnityEngine;

public class SyringeInteraction : MonoBehaviour
{
    public Animator syringeAnimator; // �ֻ�� Animator ����
    private bool hasInjected = false; // �ֻ簡 �̹� ����Ǿ����� Ȯ��

    private void OnTriggerEnter(Collider other)
    {
        // "Gum" �±׸� ���� ������Ʈ�� �浹 Ȯ��
        if (other.CompareTag("Gum") && !hasInjected)
        {
            hasInjected = true; // �ֻ� ���� �÷��� ����
            syringeAnimator.SetTrigger("StartInjesction"); // �ִϸ��̼� ����
            Debug.Log("�ֻ�� �ִϸ��̼� ����");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �ʿ� �� TriggerExit ó��
        if (other.CompareTag("Gum"))
        {
            Debug.Log("�ֻ�Ⱑ �Կ��� ������");
        }
    }
}
