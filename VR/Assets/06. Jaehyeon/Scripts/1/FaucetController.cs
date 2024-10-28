using Oculus.Interaction;
using System.Collections;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �ִϸ�����
    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� ���� ���� ����

    public OVRGrabbable faucetHandle;  // OVRGrabbable �ڵ� ����

    private void Start()
    {
        animator = GetComponent<Animator>();

        // ��ƼŬ �ʱ� ��Ȱ��ȭ
        if (waterParticle != null)
        {
            waterParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // �ڵ��� ������ ���� ������ �ʾҴٸ� ���� ƴ
        if (faucetHandle.isGrabbed && !isWaterTurnedOn)
        {
            TurnOnWater();
        }

        // �ڵ鿡�� ���� ���� ���� ��
        if (!faucetHandle.isGrabbed && isWaterTurnedOn)
        {
            TurnOffWater();
        }
    }

    public void TurnOnWater()
    {
        if (!isWaterRunning && animator != null && waterParticle != null)
        {
            animator.SetBool("SinkON", true);  // �ִϸ��̼� ����
            waterParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            waterParticle.Play();  // ��ƼŬ ���
            isWaterRunning = true;
            isWaterTurnedOn = true;  // ���� ���� ���� ����

            Debug.Log("���� Ʋ�����ϴ�.");
        }
    }

    public void TurnOffWater()
    {
        if (isWaterRunning && animator != null && waterParticle != null)
        {
            animator.SetBool("SinkON", false);  // �ִϸ��̼� ����
            waterParticle.Stop();  // ��ƼŬ ����
            waterParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
            isWaterRunning = false;
            isWaterTurnedOn = false;  // ���� �ʱ�ȭ

            Debug.Log("���� �����ϴ�.");
        }
    }

    public void RequestTurnOffWater()
    {
        TurnOffWater();
        Debug.Log("�ܺο��� �� ���� ��û�� ó���߽��ϴ�.");
    }

}
