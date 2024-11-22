using UnityEngine;

public class ActionController : MonoBehaviour
{
    public HeadFollow headFollow;
    public JawFollow jawFollow;

    void Start()
    {
        if (headFollow == null)
            headFollow = FindObjectOfType<HeadFollow>();

        if (jawFollow == null)
            jawFollow = FindObjectOfType<JawFollow>();
    }

    public void ExecuteAction(string action)
    {
        switch (action)
        {
            case "OpenMouth":
                if (jawFollow != null)
                {
                    jawFollow.isOpen = true;  // JawFollow에서 isOpen 플래그를 true로 설정하여 입을 벌리도록 함
                    Debug.Log("환자 모델이 입을 벌립니다.");
                }
                break;

            case "CloseMouth":
                if (jawFollow != null)
                {
                    jawFollow.isOpen = false;  // JawFollow에서 isOpen 플래그를 false로 설정하여 입을 닫도록 함
                    Debug.Log("환자 모델이 입을 닫습니다.");
                }
                break;

            case "TurnRightHead":
                if (headFollow != null)
                {
                    headFollow.isRight = true;  // HeadFollow에서 isRight 플래그를 true로 설정하여 고개를 오른쪽으로 돌리도록 함
                    headFollow.isLeft = false;  // 반대 방향 회전 방지
                    Debug.Log("환자 모델이 고개를 오른쪽으로 돌립니다.");
                }
                break;

            case "TurnLeftHead":
                if (headFollow != null)
                {
                    headFollow.isLeft = true;  // HeadFollow에서 isLeft 플래그를 true로 설정하여 고개를 왼쪽으로 돌리도록 함
                    headFollow.isRight = false;  // 반대 방향 회전 방지
                    Debug.Log("환자 모델이 고개를 왼쪽으로 돌립니다.");
                }
                break;

            case "ResetHead":
                if (headFollow != null)
                {
                    headFollow.isLeft = false;
                    headFollow.isRight = false;
                    Debug.Log("환자 모델의 고개를 정면으로 되돌립니다.");
                }
                break;

            default:
                Debug.Log("알 수 없는 동작: " + action);
                break;
        }
    }
}
