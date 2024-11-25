using Oculus.Interaction;
using UnityEngine;

using Oculus.Interaction;
using UnityEngine;
using System.Collections;

public abstract class ProcedureParticleTimer : ProcedureObjectBase
{
    protected bool isParticleRunning = false;

    public override void Initialize()
    {
        base.Initialize();
        if (interactionConfig.particleEffect != null)
        {
            interactionConfig.particleEffect.gameObject.SetActive(false);
        }
        isParticleRunning = false;
    }

    protected override void OnHandGrabStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover && !isDone && !isParticleRunning)
        {
            StartParticleEffect();
        }
    }

    protected virtual void StartParticleEffect()
    {
        if (interactionConfig.particleEffect != null)
        {
            isParticleRunning = true;
            interactionConfig.particleEffect.gameObject.SetActive(true);
            interactionConfig.particleEffect.Play();
            StartCoroutine(ParticleTimerRoutine());
        }
    }

    protected IEnumerator ParticleTimerRoutine()
    {
        yield return new WaitForSeconds(interactionConfig.timerDuration);
        OnParticleComplete();
    }

    protected virtual void OnParticleComplete()
    {
        if (interactionConfig.particleEffect != null)
        {
            interactionConfig.particleEffect.Stop();
            interactionConfig.particleEffect.gameObject.SetActive(false);
        }
        isParticleRunning = false;
        CompleteInteraction();
    }

    protected void OnDisable()
    {
        if (interactionConfig.particleEffect != null)
        {
            interactionConfig.particleEffect.Stop();
            interactionConfig.particleEffect.gameObject.SetActive(false);
        }
        isParticleRunning = false;
    }
}