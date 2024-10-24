using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class SnapSocketVisual : MonoBehaviour
{
    // SnapInteractable 컴포넌트 참조, 이 컴포넌트는 스냅 상호작용을 담당함
    [SerializeField] private SnapInteractable _snapInteractable;

    // 소켓의 시각적 효과를 위한 머티리얼
    [SerializeField] private Material _material;

    // SnapInteractor 객체를 저장, 현재 SnapInteractable과 연결된 인터랙터를 의미함
    private SnapInteractor _snapInteractor;

    // 소켓을 나타내는 GameObject, 스냅 시각화를 위해 생성됨
    private GameObject _socket;

    // Interactor View가 추가되었을 때 발생하는 이벤트를 추적하는 속성
    public int OnSnapInteractorViewAdded { get; private set; }

    // 스크립트가 활성화될 때 호출됨
    private void OnEnable()
    {
        // SnapInteractable에 인터랙터가 추가되면 해당 액션을 호출
        _snapInteractable.WhenInteractorAdded.Action += WhenInteractorAdded_Action;

        // 인터랙터 뷰가 추가되거나 제거되거나 선택되었을 때 이벤트에 구독
        _snapInteractable.WhenSelectingInteractorViewAdded += SnapInteractable_WhenSelectingInteractorViewAdded;
        _snapInteractable.WhenInteractorViewRemoved += SnapInteractable_WhenInteractorViewRemoved;
        _snapInteractable.WhenInteractorViewAdded += SnapInteractable_WhenInteractorViewAdded;
    }

    // SnapInteractable에 인터랙터가 추가되었을 때 실행되는 함수
    private void WhenInteractorAdded_Action(SnapInteractor interactor)
    {
        // 인터랙터가 null인 경우 현재 인터랙터로 설정
        if (interactor == null)
            _snapInteractor = interactor;
        // 현재 인터랙터와 다른 새로운 인터랙터가 추가되면 기존 소켓을 제거하고 새로 설정
        else if (_snapInteractor != interactor)
        {
            _snapInteractor = interactor;
            var temp = _socket;
            Destroy(temp);
            _socket = null;
        }
        else
            return;

        // 소켓을 설정하는 함수 호출
        SetSocket(interactor);
    }

    public List<SnapInteractor> interList;
    public List<Transform> posList;


    // SnapInteractor와 연결된 소켓을 생성하고 설정하는 함수
    private void SetSocket(SnapInteractor interactor)
    {
        // 인터랙터의 부모 오브젝트 이름을 사용해 새로운 소켓 GameObject 생성
        _socket = new GameObject(interactor.transform.parent?.name);
        // 새로 생성된 소켓의 부모를 현재 스크립트가 적용된 오브젝트로 설정

        //인터렉터 번호 찾고 ( 3번)
        //부모를 posList(3번)로 지정

        _socket.transform.parent = transform;
        // 소켓의 크기 및 위치 초기화
        _socket.transform.localScale = Vector3.one;
        _socket.transform.localPosition = Vector3.zero;
        _socket.transform.localRotation = Quaternion.identity;

        // 부모 오브젝트에서 MeshFilter 컴포넌트를 가져옴
        var parentMesh = interactor.transform.parent.GetComponent<MeshFilter>();
        if (parentMesh != null)
        {
            // MeshFilter와 MeshRenderer 컴포넌트를 소켓에 추가하여 머티리얼 적용
            _socket.AddComponent<MeshFilter>().mesh = parentMesh.mesh;
            _socket.AddComponent<MeshRenderer>().material = _material;
        }

        // 자식 오브젝트들의 MeshFilter 컴포넌트를 가져옴
        var childMesh = interactor.transform.parent.GetComponentsInChildren<MeshFilter>();
        if (childMesh != null)
        {
            // 자식 오브젝트들의 메쉬를 복사하여 소켓에 추가
            foreach (var item in childMesh)
            {
                var newGo = new GameObject(item.name);
                newGo.transform.parent = _socket.transform;
                newGo.transform.localPosition = item.transform.localPosition;
                newGo.transform.localRotation = item.transform.localRotation;
                newGo.transform.localScale = item.transform.localScale;
                newGo.AddComponent<MeshFilter>().mesh = item.mesh;
                newGo.AddComponent<MeshRenderer>().material = _material;
            }
        }
    }

    // Interactor View가 추가되었을 때 호출됨
    public void SnapInteractable_WhenInteractorViewAdded(IInteractorView view)
    {
        // 소켓을 활성화함
        _socket?.SetActive(true);
    }

    // Interactor View가 제거되었을 때 호출됨
    public void SnapInteractable_WhenInteractorViewRemoved(IInteractorView view)
    {
        // 소켓을 비활성화함
        _socket.SetActive(false);
    }

    // Interactor View가 선택되었을 때 호출됨
    public void SnapInteractable_WhenSelectingInteractorViewAdded(IInteractorView view)
    {
        // 소켓을 비활성화함
        _socket?.SetActive(false);
    }

    // Start() 함수는 시작 시 호출되며 현재는 비어 있음
    void Start()
    {
    }

    // 매 프레임마다 호출되는 Update 함수도 현재는 비어 있음
    void Update()
    {
    }
}
