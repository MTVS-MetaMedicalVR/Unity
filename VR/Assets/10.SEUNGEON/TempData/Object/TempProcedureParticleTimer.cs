// TempProcedureParticleTimer.cs - ��ƼŬ Ÿ�̸� ���ν���
using System.Collections;
using UnityEngine;

public class TempProcedureParticleTimer : TempProcedureObjectBase
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone) return;

        if (interactionConfig.particleEffect)
        {
            interactionConfig.particleEffect.Play();
            StartCoroutine(WaitForTimer());
        }
    }

    private IEnumerator WaitForTimer()
    {
        yield return new WaitForSeconds(interactionConfig.timerDuration);

        if (interactionConfig.particleEffect)
            interactionConfig.particleEffect.Stop();

        CompleteInteraction();
    }
}