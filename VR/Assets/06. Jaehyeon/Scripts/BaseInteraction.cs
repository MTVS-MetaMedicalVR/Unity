using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public string stepId;  // JSON ���Ͽ��� �ܰ� ID�� �޾ƿ�
    protected Animator objectAnimator;  // ���� �ִϸ��̼� ������Ʈ
    public Transform player;
    public float proximityThreshold = 1.5f;

    protected virtual void Awake()
    {
        // ������Ʈ�� Animator�� ������ �Ҵ�
        objectAnimator = GetComponent<Animator>();
    }

    // ��ȣ�ۿ��� �����ϴ� ���� �޼���
    public virtual void PerformInteraction()
    {
        //Debug.Log($"{stepId} �ܰ� ���� ��...");
        ProcedureManager.Instance.CompleteStep(stepId);  // ���� �Ϸ�
        Debug.Log($"{stepId} �ܰ� ���� ��...");
    }

    private void Update()
    {
        // �� �����Ӹ��� �÷��̾���� �Ÿ��� ���
        if (IsPlayerNearby(player, proximityThreshold))
        {
            OnPlayerInteraction();  // ��ȣ�ۿ� �߻�
        }
    }


    // �÷��̾���� �Ÿ� Ȯ���� ���� ���� �޼���
    protected bool IsPlayerNearby(Transform player, float requiredDistance)
    {
        return Vector3.Distance(player.position, transform.position) <= requiredDistance;
    }

    // ��ȣ�ۿ� �� ȣ��� �߻� �޼��� (�ڽ� Ŭ�������� ����)
    protected abstract void OnPlayerInteraction();
}

