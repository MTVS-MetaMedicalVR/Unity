using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �ִϸ�����
    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� �� �� �������� ����

    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // ���������� �۵���ų �Ÿ�

    private void Start()
    {
        animator = GetComponent<Animator>();

        // �ʱ� ��ƼŬ ��Ȱ��ȭ
        waterParticle.gameObject.SetActive(false);
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
            waterParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            waterParticle.Play();  // ��ƼŬ ���
            isWaterRunning = true;
            isWaterTurnedOn = true;  // ���� ���� ���� ����

            Debug.Log("���� Ʋ�����ϴ�.");
        }
    }

    public void RequestTurnOffWater()
    {
        if (isWaterRunning)
        {
            animator.SetBool("SinkON", false);  // �ִϸ��̼� ����
            waterParticle.Stop();  // ��ƼŬ ����
            waterParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
            isWaterRunning = false;

            Debug.Log("���� �����ϴ�.");
        }
    }
}
