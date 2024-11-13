using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // �ٶ� ��ƼŬ
    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // �հ� ������Ʈ ���� Ȱ��ȭ �Ÿ�
    public float dryingTime = 5.0f;  // ������ �ð� ����
    public AudioSource dryingAudio;

    private bool isDrying = false;  // �� ������ ���� Ȯ��
    private bool canStartDrying = true;  // ������ ���� ���� Ȯ��

    private void Start()
    {
        // �ʱ� ��ƼŬ ��Ȱ��ȭ
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);
        }

        if (dryingAudio != null)
        {
            dryingAudio.Stop();
        }
    }

    private void Update()
    {
        // ���� �� ������Ʈ�� ��������� �� ������ ����
        if (IsHandNearDryer() && !isDrying && canStartDrying)
        {
            StartDrying();
        }
    }

    private bool IsHandNearDryer()
    {
        // �հ� �� ������Ʈ ���� �Ÿ��� ���
        if (handTransform != null)
        {
            float distance = Vector3.Distance(handTransform.position, transform.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (!isDrying)
        {
            Debug.Log("���� ������ �����մϴ�.");
            isDrying = true;
            canStartDrying = false;  // ������ �߿� �ٽ� �������� �ʵ��� ����

            // ��ƼŬ Ȱ��ȭ �� ���
            if (windParticle != null)
            {
                windParticle.gameObject.SetActive(true);
                windParticle.Play();
            }

            // ����� ���
            if (dryingAudio != null && !dryingAudio.isPlaying)
            {
                dryingAudio.Play();
            }

            // ������ Ÿ�̸� ����
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // ������ �ð� ���� ���
        CompleteDrying();
        // ���� �ð� �Ŀ� �ٽ� �����⸦ ���
        yield return new WaitForSeconds(2.0f);  // ���÷� 2�� ��� �� �ٽ� ������ ����
        canStartDrying = true;
    }

    private void CompleteDrying()
    {
        Debug.Log("�� ������ �Ϸ�.");
        if (windParticle != null)
        {
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }

        // ����� ����
        if (dryingAudio != null && dryingAudio.isPlaying)
        {
            dryingAudio.Stop();
        }

        isDrying = false;  // ���� �ʱ�ȭ
    }
}
