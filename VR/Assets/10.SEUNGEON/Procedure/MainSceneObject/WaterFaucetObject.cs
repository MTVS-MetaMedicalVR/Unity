using Oculus.Interaction.Grab;
using Oculus.Interaction;
using UnityEngine;


// WaterFaucetObject.cs
public class WaterFaucetObject : ProcedureObjectBase
{
    [SerializeField] private ParticleSystem waterParticle;
    [SerializeField] private AudioSource waterAudio;
    private bool isWaterRunning = false;

    public override void Initialize()
    {
        base.Initialize();
        if (animator != null)
        {
            animator.SetBool("SinkON", false);
        }
        if (waterParticle != null)
        {
            waterParticle.Stop();
        }
        if (waterAudio != null)
        {
            waterAudio.Stop();
        }
        isWaterRunning = false;
    }

    protected override void OnHandGrabStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover && !isDone && !isWaterRunning && isInteractionEnabled)
        {
            TurnOnWater();
        }
    }

    private void TurnOnWater()
    {
        if (animator != null)
        {
            animator.SetBool("SinkON", true);
        }
        if (waterParticle != null)
        {
            waterParticle.Play();
        }
        if (waterAudio != null && !waterAudio.isPlaying)
        {
            waterAudio.Play();
        }
        isWaterRunning = true;
        CompleteInteraction();
    }

    public void TurnOffWater()
    {
        if (animator != null)
        {
            animator.SetBool("SinkON", false);
        }
        if (waterParticle != null)
        {
            waterParticle.Stop();
        }
        if (waterAudio != null)
        {
            waterAudio.Stop();
        }
        isWaterRunning = false;
    }
}