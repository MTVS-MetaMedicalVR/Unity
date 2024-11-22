using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHaptic : MonoBehaviour
{
    // 기본 진동이 2.5초이기 때문에 duration을 추가하고 Coroutine을 이용해서 조절한다.
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
            // 선택할 때 진동 이벤트 설정
            print(rayInteractable.gameObject);
            rayInteractable.WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action;

            // rayInteractable.WhenPointerEventRaised += RayInteractable_WhenPointerEventRaised;
        }

        //rayInteractables[0].WhenSelectingInteractorAdded.Action += WhenSelectingInteractorAdded_Action(rayInteractables.);
    }

   

    //// RayInteractable이 Hover 될 때 진동
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
        // 컨트롤러에 진동이 오고,
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        // 내가 원하는 시간만큼 진동을 받은 다음
        yield return new WaitForSeconds(duration);
        // 주파수와 진폭을 0으로 만들어서 끝내자.
        OVRInput.SetControllerVibration(0, 0, controller);
    }
}
