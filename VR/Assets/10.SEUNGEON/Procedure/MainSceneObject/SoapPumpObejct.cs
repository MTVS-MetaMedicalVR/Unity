
// SoapPumpObject.cs
using UnityEngine;

public class SoapPumpObject : ProcedureObjectBase
{
    [SerializeField] private ParticleSystem soapParticle;
    [SerializeField] private Transform handTransform;
    [SerializeField] private float activationDistance = 0.1f;

    public override void Initialize()
    {
        base.Initialize();
        if (soapParticle != null)
        {
            soapParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isDone && Vector3.Distance(handTransform.position, transform.position) < activationDistance)
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
