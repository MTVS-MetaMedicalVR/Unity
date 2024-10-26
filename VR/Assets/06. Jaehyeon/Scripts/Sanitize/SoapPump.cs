using UnityEngine;

public class UseSoap : MonoBehaviour
{
    public string stepId = "use_soap";
    public Animator soapAnimator;
    public ProcedureManager procedureManager;

    private void OnMouseDown()
    {
        Debug.Log("�񴩸� �����մϴ�.");
        soapAnimator.SetTrigger("Pump");
        procedureManager.CompleteStep(stepId);
    }
}
