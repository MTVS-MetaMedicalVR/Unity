using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseInteraction : MonoBehaviour
{
    public string stepId;  // JSON 파일에서 단계 ID를 받아옴
    protected Animator objectAnimator;
    public Transform player;  // 플레이어 위치 추적
    public float proximityThreshold = 1.5f;  // 근접 거리 임계값

    private bool interactionTriggered = false;  // 중복 방지 플래그
    private bool interactionCompleted = false;  // 상호작용 완료 여부

    protected virtual void Awake()
    {
        // 오브젝트에 Animator 컴포넌트가 있는지 확인
        objectAnimator = GetComponent<Animator>();
        if (objectAnimator == null)
        {
            Debug.LogError($"{gameObject.name}에 Animator가 없습니다.");
        }
    }

    // 상호작용 수행
    public virtual void PerformInteraction()
    {
        if (interactionCompleted) return;  // 이미 완료된 상호작용이면 무시

        string currentStepId = ProcedureManager.Instance.GetCurrentStepId();
        Debug.Log($"현재 단계: {currentStepId}, 요청된 단계: {stepId}");

        if (currentStepId == stepId)
        {
            ProcedureManager.Instance.CompleteStep(stepId);  // 절차 완료
            interactionCompleted = true;  // 상호작용 완료
            Debug.Log($"{stepId} 단계 완료.");
        }
        else
        {
            Debug.LogError($"'{stepId}'가 현재 단계와 일치하지 않습니다.");
        }
    }

    private void Update()
    {
        // 매 프레임마다 플레이어와의 거리를 계산
        if (!interactionTriggered && IsPlayerNearby(player, proximityThreshold))
        {
            interactionTriggered = true;  // 중복 방지
            OnPlayerInteraction();  // 상호작용 실행
        }
    }

    // 플레이어와의 거리 확인
    protected bool IsPlayerNearby(Transform player, float requiredDistance)
    {
        return Vector3.Distance(player.position, transform.position) <= requiredDistance;
    }

    // 상호작용 시 호출될 추상 메서드 (구현은 자식 클래스에서 수행)
    protected abstract void OnPlayerInteraction();
}
