using System.Collections;
using UnityEngine;
using Oculus.Interaction;

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // �ٶ� ��ƼŬ
    public OVRHand leftHand;  // �޼� OVRHand
    public OVRHand rightHand;  // ������ OVRHand

    private bool isDrying = false;  // �� ������ ���� Ȯ��
    private float dryingTime = 5.0f;  // ������ �ð� ����

    private void Start()
    {
        if (windParticle != null)
        {
            windParticle.gameObject.SetActive(false);  // ��ƼŬ �ʱ� ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        // �� ���� �𿴴��� Ȯ���ϰ� ������ ����
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();
        }
    }

    private bool AreHandsTogether()
    {
        if (leftHand != null && rightHand != null)
        {
            // �� ��ġ�� ����Ͽ� �Ÿ��� ���� �����̸� true ��ȯ
            Vector3 leftHandPosition = leftHand.transform.position;
            Vector3 rightHandPosition = rightHand.transform.position;
            float distance = Vector3.Distance(leftHandPosition, rightHandPosition);
            return distance < 0.1f;  // ���� ������ ���� ���
        }
        return false;
    }

    public void StartDrying()
    {
        if (windParticle != null)
        {
            isDrying = true;
            Debug.Log("���� ������ �����մϴ�.");
            windParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            windParticle.Play();  // ��ƼŬ ���

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
            windParticle.Stop();  // ��ƼŬ ����
            windParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
        }
        isDrying = false;  // ���� �ʱ�ȭ
    }
}
