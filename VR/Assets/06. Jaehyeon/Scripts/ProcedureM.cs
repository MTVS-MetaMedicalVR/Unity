using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureM : MonoBehaviour
{
    public Text stepDescriptionText;  // UI에 표시할 단계 설명
    public GameObject sink;  // 싱크대 오브젝트
    public GameObject faucet;  // 수도꼭지 오브젝트
    public GameObject soapPump;  // 비누 펌프 오브젝트
    public GameObject tissue;  // 티슈 오브젝트
    public GameObject handModel;  // 손 모델 오브젝트
    public Transform player;  // 플레이어의 Transform
    public FaucetController faucetController;  // FaucetController 참조
    public SoapPumpController soapPumpController;
    public HandWashController handWashController;  // HandWashController 참조
    public HandGestureController handGestureController;  // HandGestureController 참조
    private int currentStep = 0;
    private bool stepInProgress = false;  // 현재 단계가 진행 중인지 확인


    private void Start()
    {
        StartProcedure();
    }

    public void StartProcedure()
    {
        currentStep = 0;
        stepInProgress = false;
        ExecuteStep();
    }

    public void ExecuteStep()
    {
        if (stepInProgress) return;  // 현재 단계가 완료되지 않았다면 중복 실행 방지

        stepInProgress = true;  // 단계 시작 표시

        switch (currentStep)
        {
            case 0:
                UpdateStep("개수대로 이동하세요.");
                break;
            case 1:
                UpdateStep("물을 트세요.");
                faucet.SetActive(true);
                faucetController.TurnOnWater();
                break;
            case 2:
                UpdateStep("비누를 펌프하세요.");
                soapPump.SetActive(true);
                break;
            case 3:
                UpdateStep("손을 30초간 씻으세요.");
                StartCoroutine(WashHands());
                break;
            case 4:
                UpdateStep("손을 말리세요.");
                handGestureController.enabled = true;  // 손 말리기 기능 활성화
                break;
            case 5:
                UpdateStep("물을 끄세요.");
                faucetController.RequestTurnOffWater();  // 물 끄기 요청
                //faucet.SetActive(true);
                break;
            default:
                CompleteProcedure();
                break;
        }
    }

    private void UpdateStep(string description)
    {
        stepDescriptionText.text = description;
        Debug.Log($"단계: {description}");
    }

    private IEnumerator WashHands()
    {
        yield return new WaitForSeconds(30);  // 30초 대기
        CompleteStep();
    }

    public void CompleteStep()
    {
        if (!stepInProgress) return;  // 이미 완료된 단계라면 중복 호출 방지

        Debug.Log($"'{currentStep}' 단계가 완료되었습니다.");
        stepInProgress = false;  // 단계 완료 상태 해제
        currentStep++;
        Invoke("ExecuteStep", 1.0f);  // 1초 후 다음 단계 실행
    }

    private void CompleteProcedure()
    {
        UpdateStep("손 씻기 절차가 완료되었습니다!");
        Debug.Log("모든 절차가 완료되었습니다.");
    }

    private void Update()
    {
        if (currentStep == 0 && Vector3.Distance(player.position, sink.transform.position) < 1f)
        {
            CompleteStep();  // 싱크대 근처에 도착하면 다음 단계로
        }
    }
}
