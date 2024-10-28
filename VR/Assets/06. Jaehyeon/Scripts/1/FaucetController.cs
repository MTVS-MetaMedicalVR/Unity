using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �ִϸ�����
    private bool isWaterRunning = false;  // �� ���� Ȯ��

    public Transform handTransform;  // ������� �� Transform
    public Collider faucetHandleCollider;  // ���������� Collider

    private void Start()
    {
        animator = GetComponent<Animator>();

        // �ʱ� ��ƼŬ ��Ȱ��ȭ
        waterParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ���� ���������� �浹���� �� ���� Ʈ�� ���� ����
        if (IsHandGrabbingHandle() && !isWaterRunning)
        {
            TurnOnWater();
        }
        else if (!IsHandGrabbingHandle() && isWaterRunning)
        {
            TurnOffWater();
        }
    }

    private bool IsHandGrabbingHandle()
    {
        // �հ� �������� �ڵ��� �浹�� ����
        return faucetHandleCollider.bounds.Contains(handTransform.position);
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning)
        {
            animator.SetBool("SinkON",true);  // �ִϸ��̼� ����
            waterParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            waterParticle.Play();  // ��ƼŬ ���
            isWaterRunning = true;
            Debug.Log("���� Ʈ����.");
        }
    }

    public void TurnOffWater()
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

