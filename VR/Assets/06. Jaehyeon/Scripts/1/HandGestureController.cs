using System.Collections;
using UnityEngine;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // �ٶ� ��ƼŬ
    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public Transform targetObject;  // ���� ������ ������ ��� ������Ʈ (��: �ڵ� ����̾�)
    public float activationDistance = 0.1f;  // �հ� ������Ʈ ���� Ȱ��ȭ �Ÿ�

    private bool isDrying = false;  // �� ������ ���� Ȯ��
    private float dryingTime = 5.0f;  // ������ �ð� ����

    private void Start()
    {
        // �ٶ� ��ƼŬ �ʱ� ��Ȱ��ȭ
        if (windParticle != null)
        {
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // ���� ��� ������Ʈ�� ������ ���� ������ ����
        if (IsHandNearTarget() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool IsHandNearTarget()
    {
        // �հ� ��� ������Ʈ ������ �Ÿ��� ����Ͽ� Ȱ��ȭ ���� ���� ���� ��ȯ
        if (handTransform != null && targetObject != null)
        {
            float distance = Vector3.Distance(handTransform.position, targetObject.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (!isDrying && windParticle != null)
        {
            isDrying = true;
            Debug.Log("���� ������ �����մϴ�.");

            // ��ƼŬ Ȱ��ȭ �� ���
            windParticle.gameObject.SetActive(true);
            windParticle.Play();

            // ������ Ÿ�̸� ����
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // ������ �ð� ���� ���
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        if (windParticle != null)
        {
            Debug.Log("�� ������ �Ϸ�.");

            // ��ƼŬ ���� �� ��Ȱ��ȭ
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }
        isDrying = false;  // ���� �ʱ�ȭ
    }
}
