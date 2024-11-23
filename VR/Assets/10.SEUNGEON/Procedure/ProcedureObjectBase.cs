// 2. DataStructures.cs - 데이터 구조 정의
using Oculus.Interaction.HandGrab;
using Oculus.Interaction;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

[System.Serializable]
public class Step
{
    public string name;
    public string description;
    public string type;
    public float duration = -1f;
    public string targetName;
}

[System.Serializable]
public class Procedure
{
    public string id;
    public string name;
    public string description;
    public string preRequisite;
    public List<Step> steps = new List<Step>();
}

[System.Serializable]
public class InteractionConfig
{
    public string animationTriggerName;
    public ParticleSystem particleEffect;
    public float timerDuration;
}

public abstract class ProcedureObjectBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected InteractionConfig interactionConfig;
    [SerializeField] private string procedureId;
    [SerializeField, Interface(typeof(HandGrabInteractable))]
    private UnityEngine.Object _handGrabInteractable;
    protected HandGrabInteractable HandGrabInteractable { get; private set; }
    protected bool isDone;
    protected bool isInteractionEnabled;
    public string ProcedureId => procedureId;

    protected virtual void Awake()
    {
        HandGrabInteractable = _handGrabInteractable as HandGrabInteractable;
        isInteractionEnabled = false;
    }

    protected virtual void OnHandGrabStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover && !isDone && isInteractionEnabled)
        {
            HandleInteraction();
        }
    }

    protected virtual void HandleInteraction()
    {
        // 각 오브젝트에서 구현
    }

    public virtual void Initialize()
    {
        isDone = false;
        DisableInteraction();
    }

    public virtual void EnableInteraction()
    {
        isInteractionEnabled = true;
        if (HandGrabInteractable != null)
        {
            HandGrabInteractable.WhenStateChanged += OnHandGrabStateChanged;
        }
    }

    public virtual void DisableInteraction()
    {
        isInteractionEnabled = false;
        if (HandGrabInteractable != null)
        {
            HandGrabInteractable.WhenStateChanged -= OnHandGrabStateChanged;
        }
    }

    protected virtual void CompleteInteraction()
    {
        isDone = true;
        DisableInteraction();
        if (InGameProcedureManager.Instance != null)
        {
            InGameProcedureManager.Instance.CompleteCurrentStep();
        }
    }
}