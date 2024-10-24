using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseInteraction : MonoBehaviour
{
    public string stepId;  // JSON ���Ͽ��� �ܰ� ID�� �޾ƿ�
    protected Animator objectAnimator;
    public Transform player;  // �÷��̾� ��ġ ����
    public float proximityThreshold = 1.5f;  // ���� �Ÿ� �Ӱ谪

    private bool interactionTriggered = false;  // �ߺ� ���� �÷���
    private bool interactionCompleted = false;  // ��ȣ�ۿ� �Ϸ� ����

    protected virtual void Awake()
    {
        // ������Ʈ�� Animator ������Ʈ�� �ִ��� Ȯ��
        objectAnimator = GetComponent<Animator>();
        if (objectAnimator == null)
        {
            Debug.LogError($"{gameObject.name}�� Animator�� �����ϴ�.");
        }
    }

    // ��ȣ�ۿ� ����
    public virtual void PerformInteraction()
    {
        if (interactionCompleted) return;  // �̹� �Ϸ�� ��ȣ�ۿ��̸� ����

        string currentStepId = ProcedureManager.Instance.GetCurrentStepId();
        Debug.Log($"���� �ܰ�: {currentStepId}, ��û�� �ܰ�: {stepId}");

        if (currentStepId == stepId)
        {
            ProcedureManager.Instance.CompleteStep(stepId);  // ���� �Ϸ�
            interactionCompleted = true;  // ��ȣ�ۿ� �Ϸ�
            Debug.Log($"{stepId} �ܰ� �Ϸ�.");
        }
        else
        {
            Debug.LogError($"'{stepId}'�� ���� �ܰ�� ��ġ���� �ʽ��ϴ�.");
        }
    }

    private void Update()
    {
        // �� �����Ӹ��� �÷��̾���� �Ÿ��� ���
        if (!interactionTriggered && IsPlayerNearby(player, proximityThreshold))
        {
            interactionTriggered = true;  // �ߺ� ����
            OnPlayerInteraction();  // ��ȣ�ۿ� ����
        }
    }

    // �÷��̾���� �Ÿ� Ȯ��
    protected bool IsPlayerNearby(Transform player, float requiredDistance)
    {
        return Vector3.Distance(player.position, transform.position) <= requiredDistance;
    }

    // ��ȣ�ۿ� �� ȣ��� �߻� �޼��� (������ �ڽ� Ŭ�������� ����)
    protected abstract void OnPlayerInteraction();
}
