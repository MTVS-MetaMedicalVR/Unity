using System.Collections;
using UnityEngine;

public class FaucetController : MonoBehaviour
{
<<<<<<< HEAD
    public ParticleSystem waterParticle;  // �� ��ƼŬ
    private Animator animator;  // �������� �ִϸ�����
    public AudioSource waterAudio; //�� �Ҹ� ����� �ҽ�
=======
    public List<ParticleSystem> waterParticles = new List<ParticleSystem>();  // �� ��ƼŬ ����Ʈ
    private Animator animator;  // �������� �ִϸ�����
    public AudioSource waterAudio; // �� �Ҹ� ����� �ҽ�
>>>>>>> origin/JaeVR(Beta+Procedure)

    private bool isWaterRunning = false;  // �� ���� Ȯ��
    private bool isWaterTurnedOn = false;  // ���� �� �� �������� ����

    public Transform handTransform;  // ���� Transform (�� ��ġ ����)
    public float activationDistance = 0.1f;  // ���������� �۵���ų �Ÿ�
    public float waterDuration = 30.0f;  // ���� ���� ���� ���� �ð� (��)
<<<<<<< HEAD

    // �� �ı� ��Ʈ�ѷ� ���� (�� �κ��� ���� ����)
    // public HandWashController handWashController;
=======
>>>>>>> origin/JaeVR(Beta+Procedure)

    private void Start()
    {
        animator = GetComponent<Animator>();

<<<<<<< HEAD
        if(waterParticle != null)
        {
            // �ʱ� ��ƼŬ ��Ȱ��ȭ
            waterParticle.gameObject.SetActive(false);
            //��ƼŬ �ʱ�ȭ 
            waterParticle.Stop(); 
        }
        

        // �ʱ� �� �ı� �ִϸ��̼� ��Ȱ��ȭ (�ʿ�� ����)
        // if (handWashController != null)
        // {
        //     handWashController.DisableAnimators();
        // }

        if(waterAudio != null)
        {
            waterAudio.Stop();
        }

=======
        // ��� ��ƼŬ �ʱ�ȭ
        foreach (var particle in waterParticles)
        {
            if (particle != null)
            {
                particle.gameObject.SetActive(false); // �ʱ� ��ƼŬ ��Ȱ��ȭ
                particle.Stop(); // ��ƼŬ ����
            }
        }

        if (waterAudio != null)
        {
            waterAudio.Stop();
        }
>>>>>>> origin/JaeVR(Beta+Procedure)
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
            isWaterRunning = true;
            isWaterTurnedOn = true;  // ���� ���� ���� ����

            Debug.Log("���� Ʋ�����ϴ�.");

<<<<<<< HEAD
            // �� ��ƼŬ ���
            if (waterParticle != null)
            {
                waterParticle.gameObject.SetActive(true);
                waterParticle.Play();

                // �� ��ƼŬ�� ����� �� �Ҹ� ����
                if (waterAudio != null && !waterAudio.isPlaying)
                {
                    waterAudio.Play();
                }
            }

            // 30�� �Ŀ� �ڵ����� ���� ��

=======
            // ��� ��ƼŬ Ȱ��ȭ �� ���
            foreach (var particle in waterParticles)
            {
                if (particle != null)
                {
                    particle.gameObject.SetActive(true);
                    particle.Play();
                }
            }

            // �� ��ƼŬ�� ����� �� �Ҹ� ����
            if (waterAudio != null && !waterAudio.isPlaying)
            {
                waterAudio.Play();
            }

            // ������ �ð� �� ���� ��
>>>>>>> origin/JaeVR(Beta+Procedure)
            StartCoroutine(WaterDurationRoutine());
        }
    }

    private IEnumerator WaterDurationRoutine()
    {
<<<<<<< HEAD
        yield return new WaitForSeconds(waterDuration);  // 30�� ���
=======
        yield return new WaitForSeconds(waterDuration);  // ������ �ð� ���
>>>>>>> origin/JaeVR(Beta+Procedure)
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

<<<<<<< HEAD
            if (waterParticle != null)
            {
                waterParticle.Stop();  // ��ƼŬ ����
                waterParticle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
            }


=======
            // ��� ��ƼŬ ���� �� ��Ȱ��ȭ
            foreach (var particle in waterParticles)
            {
                if (particle != null)
                {
                    particle.Stop();  // ��ƼŬ ����
                    particle.gameObject.SetActive(false);  // ��ƼŬ ��Ȱ��ȭ
                }
            }

>>>>>>> origin/JaeVR(Beta+Procedure)
            // �� �Ҹ� ����
            if (waterAudio != null && waterAudio.isPlaying)
            {
                waterAudio.Stop();
            }

<<<<<<< HEAD

=======
>>>>>>> origin/JaeVR(Beta+Procedure)
            isWaterRunning = false;

            Debug.Log("���� �����ϴ�.");

            // �� �ı� �ִϸ����� ��Ȱ��ȭ (���� �Ǵ� �ʿ信 ���� ����)
            // if (handWashController != null && handWashController.IsWashing)
            // {
            //     handWashController.StopHandWash();
            // }
        }
    }
}
