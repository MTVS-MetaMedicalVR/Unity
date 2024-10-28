using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureM : MonoBehaviour
{
    public Text stepDescriptionText;  // UI에 표시할 단계 설명
    public GameObject sink;  // 싱크대 오브젝트
    public GameObject faucet;  // 수도꼭지 오브젝트
    public GameObject soapPump;  // 비누 펌프 오브젝트
    public GameObject tissue;  // 티슈 오브젝트
    public Transform player;  // 플레이어의 Transform

    public FaucetController faucetController;  // FaucetController 참조
    public SoapPumpController soapPumpController;
    public HandWashController handWashController;  // HandWashController 참조
    public HandGestureController handGestureController;  // HandGestureController 참조

    private void Start()
    {
        DisplayMessage("VR 환경에 오신 것을 환영합니다!");
    }

    private void Update()
    {
        // 플레이어가 싱크대 근처에 도달했을 때
        if (Vector3.Distance(player.position, sink.transform.position) < 0.3f)
        {
            DisplayMessage("싱크대에 도착했습니다.");
        }
    }

    public void DisplayMessage(string message)
    {
        stepDescriptionText.text = message;
        Debug.Log(message);
    }

    public void TriggerFaucet()
    {
        // 수도꼭지 돌리기와 물 파티클 켜기
        DisplayMessage("물을 트세요.");
        faucetController.TurnOnWater();
    }

    public void TriggerSoapPump()
    {
        // 비누 펌프 사용
        DisplayMessage("비누를 펌프하세요.");
        soapPumpController.PumpSoap();
    }

    public void TriggerHandWash()
    {
        // 손 씻기 30초 실행
        DisplayMessage("손을 30초 동안 씻으세요.");
        handWashController.StartHandWash();
    }

    public void TriggerHandDry()
    {
        // 손 말리기 시작
        DisplayMessage("손을 말리세요.");
        handGestureController.StartDrying();
    }

    public void TurnOffFaucet()
    {
        // 물 끄기
        DisplayMessage("물을 끄세요.");
        faucetController.RequestTurnOffWater();
    }
}
