using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class SnapSocketVisual : MonoBehaviour
{
    // SnapInteractable ������Ʈ ����, �� ������Ʈ�� ���� ��ȣ�ۿ��� �����
    [SerializeField] private SnapInteractable _snapInteractable;

    // ������ �ð��� ȿ���� ���� ��Ƽ����
    [SerializeField] private Material _material;

    // SnapInteractor ��ü�� ����, ���� SnapInteractable�� ����� ���ͷ��͸� �ǹ���
    private SnapInteractor _snapInteractor;

    // ������ ��Ÿ���� GameObject, ���� �ð�ȭ�� ���� ������
    private GameObject _socket;

    // Interactor View�� �߰��Ǿ��� �� �߻��ϴ� �̺�Ʈ�� �����ϴ� �Ӽ�
    public int OnSnapInteractorViewAdded { get; private set; }

    // ��ũ��Ʈ�� Ȱ��ȭ�� �� ȣ���
    private void OnEnable()
    {
        // SnapInteractable�� ���ͷ��Ͱ� �߰��Ǹ� �ش� �׼��� ȣ��
        _snapInteractable.WhenInteractorAdded.Action += WhenInteractorAdded_Action;

        // ���ͷ��� �䰡 �߰��ǰų� ���ŵǰų� ���õǾ��� �� �̺�Ʈ�� ����
        _snapInteractable.WhenSelectingInteractorViewAdded += SnapInteractable_WhenSelectingInteractorViewAdded;
        _snapInteractable.WhenInteractorViewRemoved += SnapInteractable_WhenInteractorViewRemoved;
        _snapInteractable.WhenInteractorViewAdded += SnapInteractable_WhenInteractorViewAdded;
    }

    // SnapInteractable�� ���ͷ��Ͱ� �߰��Ǿ��� �� ����Ǵ� �Լ�
    private void WhenInteractorAdded_Action(SnapInteractor interactor)
    {
        // ���ͷ��Ͱ� null�� ��� ���� ���ͷ��ͷ� ����
        if (interactor == null)
            _snapInteractor = interactor;
        // ���� ���ͷ��Ϳ� �ٸ� ���ο� ���ͷ��Ͱ� �߰��Ǹ� ���� ������ �����ϰ� ���� ����
        else if (_snapInteractor != interactor)
        {
            _snapInteractor = interactor;
            var temp = _socket;
            Destroy(temp);
            _socket = null;
        }
        else
            return;

        // ������ �����ϴ� �Լ� ȣ��
        SetSocket(interactor);
    }

    public List<SnapInteractor> interList;
    public List<Transform> posList;


    // SnapInteractor�� ����� ������ �����ϰ� �����ϴ� �Լ�
    private void SetSocket(SnapInteractor interactor)
    {
        // ���ͷ����� �θ� ������Ʈ �̸��� ����� ���ο� ���� GameObject ����
        _socket = new GameObject(interactor.transform.parent?.name);
        // ���� ������ ������ �θ� ���� ��ũ��Ʈ�� ����� ������Ʈ�� ����

        //���ͷ��� ��ȣ ã�� ( 3��)
        //�θ� posList(3��)�� ����

        _socket.transform.parent = transform;
        // ������ ũ�� �� ��ġ �ʱ�ȭ
        _socket.transform.localScale = Vector3.one;
        _socket.transform.localPosition = Vector3.zero;
        _socket.transform.localRotation = Quaternion.identity;

        // �θ� ������Ʈ���� MeshFilter ������Ʈ�� ������
        var parentMesh = interactor.transform.parent.GetComponent<MeshFilter>();
        if (parentMesh != null)
        {
            // MeshFilter�� MeshRenderer ������Ʈ�� ���Ͽ� �߰��Ͽ� ��Ƽ���� ����
            _socket.AddComponent<MeshFilter>().mesh = parentMesh.mesh;
            _socket.AddComponent<MeshRenderer>().material = _material;
        }

        // �ڽ� ������Ʈ���� MeshFilter ������Ʈ�� ������
        var childMesh = interactor.transform.parent.GetComponentsInChildren<MeshFilter>();
        if (childMesh != null)
        {
            // �ڽ� ������Ʈ���� �޽��� �����Ͽ� ���Ͽ� �߰�
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

    // Interactor View�� �߰��Ǿ��� �� ȣ���
    public void SnapInteractable_WhenInteractorViewAdded(IInteractorView view)
    {
        // ������ Ȱ��ȭ��
        _socket?.SetActive(true);
    }

    // Interactor View�� ���ŵǾ��� �� ȣ���
    public void SnapInteractable_WhenInteractorViewRemoved(IInteractorView view)
    {
        // ������ ��Ȱ��ȭ��
        _socket.SetActive(false);
    }

    // Interactor View�� ���õǾ��� �� ȣ���
    public void SnapInteractable_WhenSelectingInteractorViewAdded(IInteractorView view)
    {
        // ������ ��Ȱ��ȭ��
        _socket?.SetActive(false);
    }

    // Start() �Լ��� ���� �� ȣ��Ǹ� ����� ��� ����
    void Start()
    {
    }

    // �� �����Ӹ��� ȣ��Ǵ� Update �Լ��� ����� ��� ����
    void Update()
    {
    }
}
