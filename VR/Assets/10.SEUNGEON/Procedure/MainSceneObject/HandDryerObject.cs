using Oculus.Interaction.Grab;
using Oculus.Interaction;
using UnityEngine;

// HandDryerObject.cs
using UnityEngine;

public class HandDryerObject : ProcedureParticleTimer
{
    [SerializeField] private ParticleSystem windParticle;
    [SerializeField] private AudioSource dryingAudio;
    [SerializeField] private float dryingTime = 5.0f;
    private bool isDrying = false;

    public override void Initialize()
    {
        base.Initialize();
        isDrying = false;
        if (windParticle != null)
        {
            windParticle.Stop();
        }
        if (dryingAudio != null)
        {
            dryingAudio.Stop();
        }
        if (interactionConfig != null)
        {
            interactionConfig.particleEffect = windParticle;
            interactionConfig.timerDuration = dryingTime;
        }
    }

    protected override void HandleInteraction()
    {
        if (!isDone && !isParticleRunning)
        {
            StartDrying();
        }
    }

    private void StartDrying()
    {
        if (!isDrying)
        {
            Debug.Log("���� ������ �����մϴ�.");
            isDrying = true;
            if (windParticle != null)
            {
                windParticle.Play();
            }
            if (dryingAudio != null && !dryingAudio.isPlaying)
            {
                dryingAudio.Play();
            }
            StartCoroutine(ParticleTimerRoutine());
        }
    }

    protected override void OnParticleComplete()
    {
        Debug.Log("�� ������ �Ϸ�.");
        if (windParticle != null)
        {
            windParticle.Stop();
        }
        if (dryingAudio != null && dryingAudio.isPlaying)
        {
            dryingAudio.Stop();
        }
        isDrying = false;
        base.OnParticleComplete();
    }
}