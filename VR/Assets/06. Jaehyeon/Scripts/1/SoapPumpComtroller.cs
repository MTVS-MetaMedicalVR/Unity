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
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            pumpAnimator.SetTrigger("Pump");
            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);
                foamParticle.Play();
            }
            isPumped = true;

            Debug.Log("�񴩸� �����߽��ϴ�.");
        }
    }
}
