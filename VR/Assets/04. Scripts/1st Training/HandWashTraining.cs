using UnityEngine;

public class HandWashTraining : MonoBehaviour
{
    // 손 위생 절차 단계 정의
    public enum HandWashStage
    {
        MoveToSink,
        TurnOnWater,
        UseSoap,
        HandWashing,
        DryHandsWithTissue,
        TurnOffWater,
        Completed
    }

    // 현재 손 위생 실습 단계
    private HandWashStage currentStage;
    // 손 위생 애니메이션
    public Animator handWashAnimator;

    // 플레이어와 개수대 위치 확인용
    public Transform sinkTr;
    public Transform playerTr;
    private float sinkProximityThreshold = 1.5f; // 개수대 근처로 이동했다고 간주할 거리

    // 손 위생 타이머 및 영역 체크 변수
    private float washingTimer = 30.0f; // 손 씻기 타이머 (30초)
    private bool isInWashingZone = false; // 손이 손 씻기 영역에 있는지 여부
    private bool warningDisplayed = false; // 경고 UI가 표시되었는지 여부

    private void Start()
    {
        StartTraining(); // 실습 시작
    }

    // 손 위생 실습 시작
    public void StartTraining()
    {
        currentStage = HandWashStage.MoveToSink; // 초기 단계 설정
        HandleStage(); // 첫 단계 수행
    }

    private void Update()
    {
        // 각 단계에 따른 동작 처리
        if (currentStage == HandWashStage.MoveToSink)
        {
            CheckPlayerProximityToSink(); // 개수대 근처로 이동 여부 확인
        }
        else if (currentStage == HandWashStage.HandWashing)
        {
            TrackHandWashing(); // 손 씻기 타이머 트래킹
        }
    }

    // 단계별 안내 메시지 및 설정
    private void HandleStage()
    {
        switch (currentStage)
        {
            case HandWashStage.MoveToSink:
                Debug.Log("개수대로 이동하세요.");
                break;

            case HandWashStage.TurnOnWater:
                Debug.Log("물을 트세요.");
                break;

            case HandWashStage.UseSoap:
                Debug.Log("비누를 펌프하세요.");
                break;

            case HandWashStage.HandWashing:
                Debug.Log("손을 30초간 씻으세요.");
                washingTimer = 30.0f; // 손 씻기 타이머 초기화
                isInWashingZone = true;
                warningDisplayed = false;
                break;

            case HandWashStage.DryHandsWithTissue:
                Debug.Log("손을 티슈로 닦으세요.");
                break;

            case HandWashStage.TurnOffWater:
                Debug.Log("물을 끄세요.");
                break;

            case HandWashStage.Completed:
                CompleteHandWash(); // 실습 완료 처리
                break;
        }
    }

    // 다음 단계로 이동
    private void AdvanceStage()
    {
        currentStage++;
        HandleStage(); // 새로운 단계 처리
    }

    // 플레이어가 개수대 근처에 있는지 확인
    private void CheckPlayerProximityToSink()
    {
        float distance = Vector3.Distance(playerTr.position, sinkTr.position);
        if (distance <= sinkProximityThreshold)
        {
            Debug.Log("개수대 근처로 이동 완료");
            AdvanceStage(); // 다음 단계로 이동
        }
    }

    // 손 씻기 타이머 추적
    private void TrackHandWashing()
    {
        if (isInWashingZone) // 손이 씻기 영역 안에 있을 때
        {
            washingTimer -= Time.deltaTime; // 타이머 감소
            Debug.Log("남은 시간: " + washingTimer);

            if (washingTimer <= 0)
            {
                AdvanceStage(); // 손 씻기 완료 시 다음 단계로 이동
            }
        }
        else // 손이 씻기 영역 밖에 있을 때
        {
            if (!warningDisplayed)
            {
                ShowWarningUI(); // 경고 UI 표시
                ResetHandWashing(); // 타이머 초기화
                warningDisplayed = true;
            }
        }
    }

    // 손 씻기 영역에 있는지 확인
    private bool IsHandInWashingZone()
    {
        return isInWashingZone;
    }

    // 손 씻기 영역에서 벗어난 경고 UI 표시
    private void ShowWarningUI()
    {
        Debug.Log("손 씻기 영역에서 벗어났습니다. 다시 시작하세요.");
    }

    // 손 씻기 타이머 초기화
    private void ResetHandWashing()
    {
        washingTimer = 30.0f;
        Debug.Log("손 씻기 타이머가 초기화되었습니다.");
    }

    // 특정 태그 오브젝트와 손의 충돌만 처리하는 Trigger 이벤트
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            // 손 씻기 단계에서 손이 씻기 영역에 들어왔을 때
            if (currentStage == HandWashStage.HandWashing)
            {
                isInWashingZone = true; // 씻기 영역 안으로 진입 시 true로 설정
                warningDisplayed = false;
                Debug.Log("손이 손 씻기 영역에 들어왔습니다.");
            }
            // 물 트기 상호작용
            else if (currentStage == HandWashStage.TurnOnWater && other.gameObject.CompareTag("WaterFaucet"))
            {
                TurnOnWater();
            }
            // 비누 펌프 상호작용
            else if (currentStage == HandWashStage.UseSoap && other.gameObject.CompareTag("SoapPump"))
            {
                PumpSoap();
            }
            // 손 닦기 상호작용
            else if (currentStage == HandWashStage.DryHandsWithTissue && other.gameObject.CompareTag("Tissue"))
            {
                DryHands();
            }
            // 물 끄기 상호작용
            else if (currentStage == HandWashStage.TurnOffWater && other.gameObject.CompareTag("WaterFaucet"))
            {
                TurnOffWater();
            }
        }
    }

    // 손 씻기 영역에서 손이 벗어날 때 호출
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand") && currentStage == HandWashStage.HandWashing)
        {
            isInWashingZone = false; // 손이 씻기 영역을 벗어날 때 false로 설정
            Debug.Log("손이 손 씻기 영역에서 벗어났습니다.");
        }
    }

    // 물 트기 애니메이션 실행
    private void TurnOnWater()
    {
        Debug.Log("Water is now running.");
        handWashAnimator.SetTrigger("TurnOnWater");
        AdvanceStage(); // 다음 단계로 이동
    }

    // 비누 펌프 애니메이션 실행
    private void PumpSoap()
    {
        Debug.Log("Soap dispensed.");
        handWashAnimator.SetTrigger("PumpSoap");
        AdvanceStage(); // 다음 단계로 이동
    }

    // 손 닦기 애니메이션 실행
    private void DryHands()
    {
        Debug.Log("Hands dried with tissue.");
        handWashAnimator.SetTrigger("DryHands");
        AdvanceStage(); // 다음 단계로 이동
    }

    // 물 끄기 애니메이션 실행
    private void TurnOffWater()
    {
        Debug.Log("Water is now turned off.");
        handWashAnimator.SetTrigger("TurnOffWater");
        AdvanceStage(); // 다음 단계로 이동
    }

    // 손 위생 실습 완료 처리
    private void CompleteHandWash()
    {
        Debug.Log("손 위생 실습을 완료했습니다.");
    }
}
