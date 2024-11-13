using UnityEngine;

public class WaterFaucetObject : ProcedureObjectBase
{
	[SerializeField] private ParticleSystem waterParticle;
	[SerializeField] private AudioSource waterAudio;
	[SerializeField] private Transform handTransform;
	[SerializeField] private float activationDistance = 0.1f;

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
			waterParticle.gameObject.SetActive(false);
		}
		if (waterAudio != null)
		{
			waterAudio.Stop();
		}
		isWaterRunning = false;
	}

	private void Update()
	{
		if (!isDone && !isWaterRunning && Vector3.Distance(handTransform.position, transform.position) < activationDistance)
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
			waterParticle.gameObject.SetActive(true);
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
			waterParticle.gameObject.SetActive(false);
		}
		if (waterAudio != null)
		{
			waterAudio.Stop();
		}
		isWaterRunning = false;
	}

	private void OnDisable()
	{
		TurnOffWater();
	}
}