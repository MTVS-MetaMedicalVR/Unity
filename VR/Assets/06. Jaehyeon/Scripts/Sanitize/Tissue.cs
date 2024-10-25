using UnityEngine;

public class Tissue : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            Debug.Log("���� Ƽ���� �۰� �ֽ��ϴ�.");
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Dry");  // �� �۱� �ִϸ��̼� ����
            }

            ProcedureManager.Instance.CompleteStep("dry_hands");  // ���� �Ϸ� �˸�
        }
    }
}
