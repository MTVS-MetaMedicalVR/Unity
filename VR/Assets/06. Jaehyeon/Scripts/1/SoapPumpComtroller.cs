using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // �� ���� �ִϸ�����
    public ParticleSystem foamParticle;  // ��ǰ ��ƼŬ
    private bool isPumped = false;  // �� ���� ���� Ȯ��

    private void Start()
    {
        // ��ƼŬ �ʱ� ��Ȱ��ȭ
        foamParticle.gameObject.SetActive(false);
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");  // ���� �ִϸ��̼� ����
            foamParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
            foamParticle.Play();  // ��ƼŬ ���
            isPumped = true;

            Debug.Log("�񴩸� �����߽��ϴ�.");
            FindObjectOfType<ProcedureM>().CompleteStep();  // ���� ������ �̵�
        }
    }
}
