using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaterProcedureManager : MonoBehaviour
{
    public Transform player;
    public float proximityThreshold = 1.5f;
    public Text stepDescriptionText;

    private int currentStepIndex = 0;
    private string[] steps = { "turn_on_water", "use_soap", "turn_off_water" };
    private bool isNearSink = false;

    private void Update()
    {
        // 매 프레임마다 플레이어와의 거리를 확인
        CheckPlayerProximity();

        // 모든 절차 완료 시 처리
        if (currentStepIndex >= steps.Length)
        {
            Debug.Log("모든 절차가 완료되었습니다.");
            return;
        }

        // 현재 단계 실행 여부 확인
        if (isNearSink && Input.GetKeyDown(KeyCode.Space)) // Space 키로 상호작용 트리거
        {
            ExecuteCurrentStep();
        }
    }

    // 플레이어가 싱크대 근처에 있는지 확인하는 메서드
    private void CheckPlayerProximity()
    {
        //float distance = Vector3.Distance(player.position, transform.position);
        //isNearSink = distance <= proximityThreshold;
    }

    // 현재 단계를 실행하는 메서드
    private void ExecuteCurrentStep()
    {
        string step = steps[currentStepIndex];
        Debug.Log($"{step} 단계가 실행되었습니다.");
        UpdateStepDescription($"{step} 단계 진행 중...");

        // 각 단계에 맞는 동작 실행
        switch (step)
        {
            case "turn_on_water":
                TurnOnWater();
                break;
            case "use_soap":
                UseSoap();
                break;
            case "turn_off_water":
                TurnOffWater();
                break;
        }

        currentStepIndex++; // 다음 단계로 이동
    }

    // 단계 설명 업데이트
    private void UpdateStepDescription(string description)
    {
        if (stepDescriptionText != null)
        {
            stepDescriptionText.text = description;
        }
    }

    // 물 트기
    private void TurnOnWater()
    {
        Debug.Log("물을 틀었습니다.");
    }

    // 비누 사용
    private void UseSoap()
    {
        Debug.Log("비누를 사용했습니다.");
    }

    // 물 끄기
    private void TurnOffWater()
    {
        Debug.Log("물을 껐습니다.");
    }
}
