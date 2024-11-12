// 2. DataStructures.cs - ������ ���� ����
using System.Collections.Generic;
using UnityEngine;

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

// 3. ProcedureObjectBase.cs - �⺻ ��ȣ�ۿ� Ŭ����
// ProcedureObjectBase.cs
public abstract class ProcedureObjectBase : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] protected InteractionConfig interactionConfig;
    protected bool isDone;

    public virtual void Initialize()
    {
        isDone = false;
        gameObject.SetActive(true);
    }

    protected virtual void CompleteInteraction()
    {
        isDone = true;
        if (InGameProcedureManager.Instance != null)
        {
            InGameProcedureManager.Instance.CompleteCurrentStep();
        }
    }
}