using System.Collections;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �������� �ִϸ�����
    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� �� �� �������� ����

    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // ���������� �۵���ų �Ÿ�
    public float waterDuration = 30.0f;  // ���� ���� ���� ���� �ð� (��)

    // �� �ı� ��Ʈ�ѷ� ����
    public HandWashController handWashController;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // �ʱ� ��ƼŬ ��Ȱ��ȭ
        waterParticle.gameObject.SetActive(false);

        // �ʱ� �� �ı� �ִϸ��̼� ��Ȱ��ȭ
        if (handWashController != null)
        {
            handWashController.DisableAnimators();
        }
    }

    private void Update()
    {
        // ���� ���������� ��������� �� ���� Ƴ
        if (IsHandNearFaucet() && !isWaterTurnedOn)
        {
            TurnOnWater();
        }

        // ���� ���� ���¿��� ���� �� ��ƼŬ�� ��������� �� �� �ı� �ִϸ��̼� ����
        /*if (isWaterRunning && IsHandNearWater())
        {
            if (handWashController != null && !handWashController.IsWashing)
            {
                handWashController.StartHandWash();
            }
        }*/
    }

    private bool IsHandNearWater()
    {
        // �հ� �� ��ƼŬ ���� �Ÿ��� ���
        float distance = Vector3.Distance(handTransform.position, waterParticle.transform.position);
        return distance < activationDistance;
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

            // �� �ı� �ִϸ����� Ȱ��ȭ
            if (handWashController != null)
            {
                handWashController.EnableAnimators();
            }

            // 30�� �Ŀ� �ڵ����� ���� ��
            StartCoroutine(WaterDurationRoutine());
        }
    }

    private IEnumerator WaterDurationRoutine()
    {
        yield return new WaitForSeconds(waterDuration);  // 30�� ���
        RequestTurnOffWater();  // �� ���� ��û
    }

    public void RequestTurnOffWater()
    {
        if (isWaterRunning)
        {
            if (animator != null)
            {
                animator.SetBool("SinkON", false);  // �ִϸ��̼� ����
            }

            if (waterParticle != null)
            {
                waterParticle.Stop();  // ��ƼŬ ����
                waterParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
            }

            isWaterRunning = false;

            Debug.Log("���� �����ϴ�.");

            // �� �ı� �ִϸ����� ��Ȱ��ȭ
            if (handWashController != null && handWashController.IsWashing)
            {
                handWashController.StopHandWash();
            }
        }
    }
}
