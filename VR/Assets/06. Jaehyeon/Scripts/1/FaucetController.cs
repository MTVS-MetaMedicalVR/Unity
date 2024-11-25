using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public List<ParticleSystem> waterParticles = new List<ParticleSystem>();  // �� ��ƼŬ ����Ʈ
    private Animator animator;  // �������� �ִϸ�����
    public AudioSource waterAudio; // �� �Ҹ� ����� �ҽ�

    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� �� �� �������� ����

    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // ���������� �۵���ų �Ÿ�
    public float waterDuration = 30.0f;  // ���� ���� ���� ���� �ð� (��)

    private void Start()
    {
        animator = GetComponent<Animator>();

        // ��� ��ƼŬ �ʱ�ȭ
        foreach (var particle in waterParticles)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(false); // �ʱ� ��ƼŬ ��Ȱ��ȭ
                particle.Stop(); // ��ƼŬ ����
            }
        }

        if (waterAudio != null)
        {
            waterAudio.Stop();
        }
    }

    private void Update()
    {
        // ���� ���������� ��������� �� ���� Ƴ
        if (IsHandNearFaucet() && !isWaterTurnedOn)
        {
            TurnOnWater();
        }
    }

    private bool IsHandNearFaucet()
    {
        // �հ� �������� ���� �Ÿ��� ���
        float distance = Vector3.Distance(handTransform.position, transform.position);
        return distance < activationDistance;
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning)
        {
            animator.SetBool("SinkON", true);  // �ִϸ��̼� ����
            isWaterRunning = true;
            isWaterTurnedOn = true;  // ���� ���� ���� ����

            Debug.Log("���� Ʋ�����ϴ�.");

            // ��� ��ƼŬ Ȱ��ȭ �� ���
            foreach (var particle in waterParticles)
            {
                if (particle != null)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }
            }

            // �� ��ƼŬ�� ����� �� �Ҹ� ����
            if (waterAudio != null && !waterAudio.isPlaying)
            {
                waterAudio.Play();
            }

            // ������ �ð� �� ���� ��
            StartCoroutine(WaterDurationRoutine());
        }
    }

    private IEnumerator WaterDurationRoutine()
    {
        yield return new WaitForSeconds(waterDuration);  // ������ �ð� ���
        RequestTurnOffWater();  // �� ���� ��û
    }

    public void RequestTurnOffWater()
    {
        if (isWaterRunning)
        {
            if (animator != null)
            {
                animator.SetBool("SinkON", false);  // �ִϸ��̼� ����
            }

            // ��� ��ƼŬ ���� �� ��Ȱ��ȭ
            foreach (var particle in waterParticles)
            {
                if (particle != null)
                {
                    particle.Stop();  // ��ƼŬ ����
                    particle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
                }
            }

            // �� �Ҹ� ����
            if (waterAudio != null && waterAudio.isPlaying)
            {
                waterAudio.Stop();
            }

            isWaterRunning = false;

            Debug.Log("���� �����ϴ�.");
        }
    }
}
