using UnityEngine;

public class HandDryerObject : ProcedureParticleTimer
{
    [SerializeField] private ParticleSystem windParticle;
    [SerializeField] private AudioSource dryingAudio;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float activationDistance = 0.1f;
    [SerializeField] private float dryingTime = 5.0f;
    private bool isDrying = false;

    public override void Initialize()
    {
        base.Initialize();
        isDrying = false;
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);
        }
        if (dryingAudio != null)
        {
            dryingAudio.Stop();
        }

        // ParticleTimer 설정
        if (interactionConfig != null)
        {
            interactionConfig.particleEffect = windParticle;
            interactionConfig.timerDuration = dryingTime;
        }
    }

    private void Update()
    {
        if (!isDone && !isParticleRunning && IsHandNearDryer())
        {
            StartDrying();
        }
    }

    private bool IsHandNearDryer()
    {
        if (handTransform != null)
        {
            float distance = Vector3.Distance(handTransform.position, transform.position);
            return distance < activationDistance;
        }
        return false;
    }

    private void StartDrying()
    {
        if (!isDrying)
        {
            Debug.Log("손을 말리기 시작합니다.");
            isDrying = true;

            if (windParticle != null)
            {
                windParticle.gameObject.SetActive(true);
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
        Debug.Log("손 말리기 완료.");

        if (windParticle != null)
        {
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }

        if (dryingAudio != null && dryingAudio.isPlaying)
        {
            dryingAudio.Stop();
        }

        isDrying = false;
        base.OnParticleComplete();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (windParticle != null)
        {
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }

        if (dryingAudio != null && dryingAudio.isPlaying)
        {
            dryingAudio.Stop();
        }

        isDrying = false;
    }
}