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
    public Transform player;  // 플레이어의 Transform
    public FaucetController faucetController;  // FaucetController 참조

    private int currentStep = 0;

    private void Start()
    {
        StartProcedure();
    }

    public void StartProcedure()
    {
        currentStep = 0;
        ExecuteStep();
    }

    public void ExecuteStep()
    {
        switch (currentStep)
        {
            case 0:
                UpdateStep("개수대로 이동하세요.");
                break;
            case 1:
                UpdateStep("물을 트세요.");
                faucet.SetActive(true);
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
                UpdateStep("손을 티슈로 닦으세요.");
                tissue.SetActive(true);
                break;
            case 5:
                UpdateStep("물을 끄세요.");
                faucet.SetActive(true);
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
        Debug.Log($"'{currentStep}' 단계가 완료되었습니다.");
        currentStep++;
        ExecuteStep();
    }

    private void CompleteProcedure()
    {
        UpdateStep("손 씻기 절차가 완료되었습니다!");
        Debug.Log("모든 절차가 완료되었습니다.");
    }

    private void Update()
    {
        if (currentStep == 0 && Vector3.Distance(player.position, sink.transform.position) < 1.5f)
        {
            CompleteStep();  // 싱크대 근처에 도착하면 다음 단계로
        }
    }
}
