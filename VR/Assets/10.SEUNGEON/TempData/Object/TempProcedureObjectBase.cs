
using System.Collections;
using UnityEngine;


// ProcedureInteractionConfig.cs - 상호작용 설정
[System.Serializable]
public class ProcedureInteractionConfig
{
    public ProcedureInteractionType interactionType;
    public float highlightBlinkRate = 1f;    // 깜빡임 속도
    public float timerDuration = 0f;         // 타이머 지속 시간
    public string animationTriggerName;      // 애니메이션 트리거 이름
    public Material highlightMaterial;        // 하이라이트 매터리얼
    public ParticleSystem particleEffect;     // 파티클 시스템
}

// TempProcedureObjectBase.cs - 기본 프로시저 클래스
public class TempProcedureObjectBase : MonoBehaviour
{
    public string procedureId;  // JSON의 id와 매칭
    public bool isDone = false;
    public TempProcedure tempProcedure;

    [SerializeField]
    protected ProcedureInteractionConfig interactionConfig;

    protected Material originalMaterial;
    protected MeshRenderer meshRenderer;
    protected Outline outlineComponent;
    protected Animator animator;
    protected bool isBlinking = false;
    protected Coroutine blinkCoroutine;

    protected virtual void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        outlineComponent = GetComponent<Outline>();
        animator = GetComponent<Animator>();

        if (meshRenderer)
            originalMaterial = meshRenderer.material;
    }

    protected virtual void OnEnable()
    {
        StartInteractionFeedback();
    }

    protected virtual void OnDisable()
    {
        StopInteractionFeedback();
    }

    protected virtual void StartInteractionFeedback()
    {
        switch (interactionConfig.interactionType)
        {
            case ProcedureInteractionType.Movement:
                if (meshRenderer && interactionConfig.highlightMaterial)
                    blinkCoroutine = StartCoroutine(BlinkMaterial());
                break;

            case ProcedureInteractionType.Outline:
                if (outlineComponent)
                    blinkCoroutine = StartCoroutine(BlinkOutline());
                break;
        }
    }

    protected virtual void StopInteractionFeedback()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (meshRenderer)
            meshRenderer.material = originalMaterial;

        if (outlineComponent)
            outlineComponent.enabled = false;
    }

    protected IEnumerator BlinkMaterial()
    {
        while (!isDone)
        {
            meshRenderer.material = interactionConfig.highlightMaterial;
            yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
            meshRenderer.material = originalMaterial;
            yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
        }
    }

    protected IEnumerator BlinkOutline()
    {
        while (!isDone)
        {
            outlineComponent.enabled = true;
            yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
            outlineComponent.enabled = false;
            yield return new WaitForSeconds(interactionConfig.highlightBlinkRate * 0.5f);
        }
    }

    protected virtual void CompleteInteraction()
    {
        StopInteractionFeedback();
        isDone = true;
        gameObject.SetActive(false);
    }
}
