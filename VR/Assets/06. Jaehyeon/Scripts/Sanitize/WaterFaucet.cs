using UnityEngine;

public class TurnOnWater : MonoBehaviour
{
    public string stepId = "turn_on_water";
    public Animator waterAnimator;
    public ProcedureManager procedureManager;

    private void OnMouseDown()  // Ŭ������ �� Ʋ��
    {
        Debug.Log("���� Ƶ�ϴ�.");
        waterAnimator.SetTrigger("TurnOn");
        procedureManager.CompleteStep(stepId);
    }
}

