using UnityEngine;

public class TurnOnWater : MonoBehaviour
{
    public string stepId = "turn_on_water";
    public Animator waterAnimator;
    public ProcedureManager procedureManager;

    private void OnMouseDown()  // 클릭으로 물 틀기
    {
        Debug.Log("물을 틉니다.");
        waterAnimator.SetTrigger("TurnOn");
        procedureManager.CompleteStep(stepId);
    }
}

