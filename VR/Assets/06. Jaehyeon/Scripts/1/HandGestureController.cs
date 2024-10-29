using System.Collections;
using UnityEngine;
using Oculus.Interaction;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // �ٶ� ��ƼŬ
    public Transform leftHandTransform;  // �޼� Transform
    public Transform rightHandTransform;  // ������ Transform
    public float activationDistance = 0.05f;  // �� ���� ������ ���̴� �Ÿ�

    private bool isDrying = false;  // �� ������ ���� Ȯ��
    private float dryingTime = 5.0f;  // �� ������ �ð�

    private void Start()
    {
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);  // ��ƼŬ �ʱ� ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        // ���� ������ ���� �� �� ������ ����
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool AreHandsTogether()
    {
        if (leftHandTransform != null && rightHandTransform != null)
        {
            // �� ���� �Ÿ��� ����Ͽ� ���� �Ÿ� ���ϸ� true ��ȯ
            float distance = Vector3.Distance(leftHandTransform.position, rightHandTransform.position);
            return distance < activationDistance;
        }
        return false;
    }

    public void StartDrying()
    {
        if (windParticle != null && !isDrying)
        {
            isDrying = true;
            Debug.Log("���� ������ �����մϴ�.");

            // �ٶ� ��ƼŬ Ȱ��ȭ �� ���
            windParticle.gameObject.SetActive(true);
            windParticle.Play();

            // ���� �ð� �� ��ƼŬ ����
            StartCoroutine(DryingRoutine());
        }
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // �� ������ �ð� ���
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        if (windParticle != null)
        {
            Debug.Log("�� ������ �Ϸ�.");

            // �ٶ� ��ƼŬ ���� �� ��Ȱ��ȭ
            windParticle.Stop();
            windParticle.gameObject.SetActive(false);
        }
        isDrying = false;  // ���� �ʱ�ȭ
    }
}
