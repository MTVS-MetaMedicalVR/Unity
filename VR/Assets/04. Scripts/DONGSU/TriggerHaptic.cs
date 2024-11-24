using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHaptic : MonoBehaviour
{
    // �⺻ ������ 2.5���̱� ������ duration�� �߰��ϰ� Coroutine�� �̿��ؼ� �����Ѵ�.
    [Range(0, 2.5f)]
    public float duration;
    [Range(0,1)]
    public float amplitude;
    [Range(0,1)]
    public float frequency;

    public List<RayInteractable> rayInteractables;
    //public RayInteractable rayInteractable;

    // Start is called before the first frame update
    void Start()
    {
        foreach (RayInteractable rayInteractable in rayInteractables)
        {
            // ������ �� ���� �̺�Ʈ ����
            print(rayInteractable.gameObject);
            rayInteractable.WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action;

            // rayInteractable.WhenPointerEventRaised += RayInteractable_WhenPointerEventRaised;
        }

        //rayInteractables[0].WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action(rayInteractables.);
    }

   

    //// RayInteractable�� Hover �� �� ����
    //private void RayInteractable_WhenPointerEventRaised(PointerEvent obj)
    //{
    //    if (obj.Type == PointerEventType.Hover)
    //    {
    //        TriggerHaptics(OVRInput.Controller.RTouch);
    //        TriggerHaptics(OVRInput.Controller.LTouch);
    //    }
    //}

    public void SelectHap()
    {

        StartCoroutine(TriggerHapticsRoutine(OVRInput.Controller.RTouch));

    }

    private void WhenSelectingInteractorAdded_Action(RayInteractor obj)
    {
        ControllerRef controllerRef = obj.GetComponent<ControllerRef>();
        if (controllerRef)
        {
            if (controllerRef.Handedness == Handedness.Right)
            {
                TriggerHaptics(OVRInput.Controller.RTouch);
            }
            else
            {
                TriggerHaptics(OVRInput.Controller.LTouch);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TriggerHaptics(OVRInput.Controller controller)
    {
        StartCoroutine(TriggerHapticsRoutine(controller));
    }

    public IEnumerator TriggerHapticsRoutine(OVRInput.Controller controller)
    {
        // ��Ʈ�ѷ��� ������ ����,
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        // ���� ���ϴ� �ð���ŭ ������ ���� ����
        yield return new WaitForSeconds(duration);
        // ���ļ��� ������ 0���� ���� ������.
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
