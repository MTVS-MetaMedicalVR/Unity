
// SoapPumpObject.cs
using Oculus.Interaction.Grab;
using Oculus.Interaction;
using UnityEngine;
public class SoapPumpObject : ProcedureObjectBase
{
    [SerializeField] private ParticleSystem soapParticle;

    public override void Initialize()
    {
        base.Initialize();
        if (soapParticle != null)
        {
            soapParticle.Stop();
        }
    }

    protected override void OnHandGrabStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover && !isDone && isInteractionEnabled)
        {
            PumpSoap();
        }
    }

    private void PumpSoap()
    {
        if (animator != null)
        {
            animator.SetTrigger("Pump");
        }
        if (soapParticle != null)
        {
            soapParticle.Play();
        }
        CompleteInteraction();
    }
}
