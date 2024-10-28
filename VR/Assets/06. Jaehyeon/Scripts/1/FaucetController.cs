using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �ִϸ�����
    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� �� �� �������� ����

    public Transform handTransform;  // ������� �� Transform
    public Collider faucetHandleCollider;  // ���������� Collider
    public ProcedureM procedureM;  // ProcedureManager ����


    private void Start()
    {
        animator = GetComponent<Animator>();

        // �ʱ� ��ƼŬ ��Ȱ��ȭ
        waterParticle.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ���� �ڵ��� ��� �ְ� ���� ���� Ʈ�� ���� �ʴٸ� ���� ƴ
        if (IsHandGrabbingHandle() && !isWaterTurnedOn)
        {
            TurnOnWater();
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
            animator.SetBool("SinkON", true);  // �ִϸ��̼� ����
            waterParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            waterParticle.Play();  // ��ƼŬ ���
            isWaterRunning = true;
            isWaterTurnedOn = true;  // ���� �� �� ���� ���·� ����
            Debug.Log("���� Ʈ����.");

            procedureM.CompleteStep();
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

    // �ܺο��� �� ���� ��û�� ���� �� �ֵ��� �޼��� �߰�
    public void RequestTurnOffWater()
    {
        if (isWaterTurnedOn)
        {
            TurnOffWater();
            isWaterTurnedOn = false;  // �� ���� �ʱ�ȭ
        }
    }
}
