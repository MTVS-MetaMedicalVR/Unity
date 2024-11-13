
// HandWashObject.cs
using UnityEngine;

public class HandWashObject : ProcedureParticleTimer
{
    [SerializeField] private Animator[] handAnimators;
    [SerializeField] private GameObject[] handObjects;

    public override void Initialize()
    {
        base.Initialize();
        foreach (var animator in handAnimators)
        {
            animator.enabled = false;
            animator.ResetTrigger("Wash");
        }
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerHand") || isDone || isParticleRunning) return;

        foreach (var handObject in handObjects)
        {
            handObject.SetActive(true);
        }
        foreach (var animator in handAnimators)
        {
            animator.enabled = true;
            animator.SetTrigger("Wash");
        }

        base.OnTriggerEnter(other);
    }

    protected override void OnParticleComplete()
    {
        foreach (var animator in handAnimators)
        {
            animator.enabled = false;
            animator.ResetTrigger("Wash");
        }
        foreach (var handObject in handObjects)
        {
            handObject.SetActive(false);
        }
        base.OnParticleComplete();
    }
}