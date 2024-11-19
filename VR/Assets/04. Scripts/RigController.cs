using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    public UnitChairController unitChairController; // UnitChairController ����

    [SerializeField] private Rig rig; // MultiAimConstraint�� ����� Rig
    [SerializeField] private float weightChangeSpeed = 10f; // Weight ���� �ӵ�

    public float minWeight = 0.5f; // �ּ� Weight
    public float maxWeight = 1.0f; // �ִ� Weight

    private float currentTime = 0f; // Weight ������ ���� Ÿ�̸�
    private float currentWeight = 1f; // ���� Weight
    private float targetWeight = 0f; // ��ǥ Weight
    private float weightChangeTime = 2f; // Weight ���� �ֱ�

    void Start()
    {
        // �ʱ� Weight ����
        if (rig != null)
        {
            rig.weight = currentWeight;
        }
    }

    void Update()
    {
        if (!unitChairController.isBack)
        {
            // isBack�� false�� ���� Rig�� Weight�� ����
            currentTime += Time.deltaTime;
            if (currentTime > weightChangeTime)
            {
                currentTime = 0f;
                targetWeight = Random.Range(minWeight, maxWeight); // ���� ��ǥ Weight ����
            }

            // ���� Weight�� ��ǥ Weight�� ���������� ��ȭ
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * weightChangeSpeed);
            rig.weight = currentWeight; // Rig Weight ������Ʈ
        }
        else
        {
            // isBack�� true�� �� Rig�� Weight�� 0���� �����Ͽ� MultiAimConstraint�� ������ ��
            currentWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * weightChangeSpeed);
            rig.weight = currentWeight;
        }
    }
}
