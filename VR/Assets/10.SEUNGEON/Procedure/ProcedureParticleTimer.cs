// ProcedureParticleTimer.cs
using System.Collections;
using UnityEngine;
public class ProcedureParticleTimer : ProcedureObjectBase
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

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone || isParticleRunning) return;

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

    protected virtual void OnDisable()
    {
        if (interactionConfig.particleEffect != null)
        {
            interactionConfig.particleEffect.Stop();
            interactionConfig.particleEffect.gameObject.SetActive(false);
        }
        isParticleRunning = false;
    }
}