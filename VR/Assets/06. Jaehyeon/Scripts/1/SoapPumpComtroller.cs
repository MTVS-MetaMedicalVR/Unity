using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // �� ���� �ִϸ�����
    public ParticleSystem foamParticle;  // ��ǰ ��ƼŬ
    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // �� ���� Ȱ��ȭ �Ÿ�

    private bool isPumped = false;  // �� ���� ���� Ȯ��

    private void Start()
    {
        // ��ƼŬ �ʱ� ��Ȱ��ȭ
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        // ���� ���� ��ó�� ���� �� ���� ���� ����
        if (IsHandNearPump() && !isPumped)
        {
            PumpSoap();
        }
    }

    private bool IsHandNearPump()
    {
        // �հ� �� ���� ���� �Ÿ��� ���
        float distance = Vector3.Distance(handTransform.position, transform.position);
        return distance < activationDistance;
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");  // �ִϸ��̼� ����

            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);  // ��ƼŬ Ȱ��ȭ
                foamParticle.Play();  // ��ƼŬ ���
            }

            isPumped = true;  // ���� �Ϸ� ���� ����
            Debug.Log("�񴩸� �����߽��ϴ�.");
        }
    }
}
