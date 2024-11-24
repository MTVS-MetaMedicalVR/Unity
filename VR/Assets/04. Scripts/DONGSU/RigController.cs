using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigController : MonoBehaviour
{
    public UnitChairController unitChairController; // UnitChairController 연결

    [SerializeField] private Rig rig; // MultiAimConstraint가 적용된 Rig
    [SerializeField] private float weightChangeSpeed = 10f; // Weight 변경 속도

    public float minWeight = 0.5f; // 최소 Weight
    public float maxWeight = 1.0f; // 최대 Weight

    private float currentTime = 0f; // Weight 변경을 위한 타이머
    private float currentWeight = 1f; // 현재 Weight
    private float targetWeight = 0f; // 목표 Weight
    private float weightChangeTime = 2f; // Weight 변경 주기

    void Start()
    {
        // 초기 Weight 설정
        if (rig != null)
        {
            rig.weight = currentWeight;
        }
    }

    void Update()
    {
        if (!unitChairController.isBack)
        {
            // isBack이 false일 때만 Rig의 Weight를 조정
            currentTime += Time.deltaTime;
            if (currentTime > weightChangeTime)
            {
                currentTime = 0f;
                targetWeight = Random.Range(minWeight, maxWeight); // 랜덤 목표 Weight 설정
            }

            // 현재 Weight를 목표 Weight로 점진적으로 변화
            currentWeight = Mathf.Lerp(currentWeight, targetWeight, Time.deltaTime * weightChangeSpeed);
            rig.weight = currentWeight; // Rig Weight 업데이트
        }
        else
        {
            // isBack이 true일 때 Rig의 Weight를 0으로 설정하여 MultiAimConstraint의 영향을 끔
            currentWeight = Mathf.Lerp(currentWeight, 0f, Time.deltaTime * weightChangeSpeed);
            rig.weight = currentWeight;
        }
    }
}
