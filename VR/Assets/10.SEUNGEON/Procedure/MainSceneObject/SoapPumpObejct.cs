
// SoapPumpObject.cs
using Oculus.Interaction.Grab;
using Oculus.Interaction;
using UnityEngine;

// SoapPumpObject.cs
public class SoapPumpObject : ProcedureObjectBase
{
    [SerializeField] private ParticleSystem soapParticle;

    public override void Initialize()
    {
        base.Initialize();
        if (soapParticle != null)
        {
            soapParticle.gameObject.SetActive(false);
        }
    }

    protected override void OnHandGrabStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover && !isDone)
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
            soapParticle.gameObject.SetActive(true);
            soapParticle.Play();
        }
        CompleteInteraction();
    }
}