using System.Collections;
using UnityEngine;

public class SoapPumpController : MonoBehaviour
{
    public Animator pumpAnimator;  // �� ���� �ִϸ�����
    public ParticleSystem foamParticle;  // ��ǰ ��ƼŬ
    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // �� ���� Ȱ��ȭ �Ÿ�

    public HandWashController handWashController;  // �� �ı� ��Ʈ�ѷ�
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
        // ���� ���� ��ó�� ���� �� �� ���� ���� ����
        if (IsHandNearPump() && !isPumped)
        {
            PumpSoap();
        }
    }

    private bool IsHandNearPump()
    {
        // �հ� ���� ������Ʈ ������ �Ÿ� ���
        if (handTransform != null)
        {
            float distance = Vector3.Distance(handTransform.position, transform.position);
            return distance < activationDistance;  // Ư�� �Ÿ� �̳��� �� true ��ȯ
        }
        return false;
    }

    public void PumpSoap()
    {
        if (!isPumped)
        {
            isPumped = true;  // ���� �Ϸ� ���� ����

            // �ִϸ��̼� ����
            if (pumpAnimator != null)
            {
                pumpAnimator.SetTrigger("Pump");
            }

            // ��ƼŬ ���
            if (foamParticle != null)
            {
                foamParticle.gameObject.SetActive(true);
                foamParticle.Play();
                StartCoroutine(StopFoamParticleAfterDelay(3f));  // 3�� �Ŀ� ��ƼŬ ����
            }

            // �� �ı� �ִϸ��̼� ����
            if (handWashController != null && !handWashController.IsWashing)
            {
                handWashController.StartHandWash();
            }

            Debug.Log("�񴩸� �����߽��ϴ�.");
        }
    }
    private IEnumerator StopFoamParticleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (foamParticle != null)
        {
            foamParticle.Stop();
            foamParticle.gameObject.SetActive(false);
        }
    }
}
