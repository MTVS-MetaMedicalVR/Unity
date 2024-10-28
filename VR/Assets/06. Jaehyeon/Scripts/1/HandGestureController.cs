using System.Collections;
using UnityEngine;
using Oculus.Interaction;  // Oculus Hand Gesture ���

public class HandGestureController : MonoBehaviour
{
    public ParticleSystem windParticle;  // �ٶ� ��ƼŬ
    public OVRHand leftHand;  // �޼� OVRHand
    public OVRHand rightHand;  // ������ OVRHand

    private bool isDrying = false;  // �� ������ ���� Ȯ��
    private float dryingTime = 5.0f;  // �� ������ �ð� (��)

    private void Start()
    {
        // �ʱ⿡�� ��ƼŬ ��Ȱ��ȭ
        windParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // �� ���� �𿴴��� Ȯ�� (�� ����ó: �չٴ� �� �Ÿ� ����)
        if (AreHandsTogether() && !isDrying)
        {
            StartDrying();  // �� ������ ����
        }
    }

    private bool AreHandsTogether()
    {
        // �޼հ� �������� �չٴ� ��ġ�� ������ �Ÿ� ���
        Vector3 leftHandPosition = leftHand.transform.position;
        Vector3 rightHandPosition = rightHand.transform.position;

        float distance = Vector3.Distance(leftHandPosition, rightHandPosition);

        // �� ���� �Ÿ��� ���� �����̸� ���� ���� ������ ����
        return distance < 0.1f;
    }

    private void StartDrying()
    {
        isDrying = true;
        Debug.Log("���� ������ �����մϴ�.");

        // �ٶ� ��ƼŬ Ȱ��ȭ �� ���
        windParticle.gameObject.SetActive(true);
        windParticle.Play();

        // �� ������ Ÿ�̸� ����
        StartCoroutine(DryingRoutine());
    }

    private IEnumerator DryingRoutine()
    {
        yield return new WaitForSeconds(dryingTime);  // ������ �ð� ���� ���
        CompleteDrying();
    }

    private void CompleteDrying()
    {
        Debug.Log("�� ������ ������ �Ϸ�Ǿ����ϴ�.");

        // �ٶ� ��ƼŬ ���� �� ��Ȱ��ȭ
        windParticle.Stop();
        windParticle.gameObject.SetActive(false);

        // ���� ������ �̵�
        FindObjectOfType<ProcedureM>().CompleteStep();
        isDrying = false;
    }
}
