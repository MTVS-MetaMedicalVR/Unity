using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInteraction : MonoBehaviour
{
    public string stepId;  // JSON 파일에서 단계 ID를 받아옴
    protected Animator objectAnimator;  // 공통 애니메이션 컴포넌트
    public Transform player;
    public float proximityThreshold = 1.5f;

    protected virtual void Awake()
    {
        // 오브젝트에 Animator가 있으면 할당
        objectAnimator = GetComponent<Animator>();
    }

    // 상호작용을 수행하는 공통 메서드
    public virtual void PerformInteraction()
    {
        //Debug.Log($"{stepId} 단계 수행 중...");
        ProcedureManager.Instance.CompleteStep(stepId);  // 절차 완료
        Debug.Log($"{stepId} 단계 수행 중...");
    }

    private void Update()
    {
        // 매 프레임마다 플레이어와의 거리를 계산
        if (IsPlayerNearby(player, proximityThreshold))
        {
            OnPlayerInteraction();  // 상호작용 발생
        }
    }


    // 플레이어와의 거리 확인을 위한 공통 메서드
    protected bool IsPlayerNearby(Transform player, float requiredDistance)
    {
        return Vector3.Distance(player.position, transform.position) <= requiredDistance;
    }

    // 상호작용 시 호출될 추상 메서드 (자식 클래스에서 구현)
    protected abstract void OnPlayerInteraction();
}

