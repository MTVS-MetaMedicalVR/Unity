using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

public class SnapSocketVisual : MonoBehaviour
{
    [SerializeField] private SnapInteractable _snapInteractable;
    [SerializeField] private Material _material;

    private SnapInteractor _snapInteractor;
    private GameObject _socket;

    public int OnSnapInteractorViewAdded { get; private set; }

    private void OnEnable()
    {
        _snapInteractable.WhenInteractorAdded.Action += WhenInteractorAdded_Action;
        _snapInteractable.WhenSelectingInteractorViewAdded += SnapInteractable_WhenSelectingInteractorViewAdded;
        _snapInteractable.WhenInteractorViewRemoved += SnapInteractable_WhenInteractorViewRemoved;
        _snapInteractable.WhenInteractorViewAdded += SnapInteractable_WhenInteractorViewAdded;
    }

    private void WhenInteractorAdded_Action(SnapInteractor interactor)
    {
        if (interactor == null)
            _snapInteractor = interactor;
        else if (_snapInteractor != interactor)
        {
            _snapInteractor = interactor;
            var temp = _socket;
            Destroy(temp);
            _socket = null;
        }
        else
            return;

        SetSocket(interactor);
    }

    private void SetSocket(SnapInteractor interactor)
    {
        _socket = new GameObject(interactor.transform.parent?.name);
        _socket.transform.parent = transform;
        _socket.transform.localScale = Vector3.one;
        _socket.transform.position = interactor.transform.position;
        _socket.transform.localRotation = Quaternion.identity;

        var parentMesh = interactor.transform.parent.GetComponent<MeshFilter>();
        if (parentMesh != null)
        {
            _socket.AddComponent<MeshFilter>().mesh = parentMesh.mesh;
            _socket.AddComponent<MeshRenderer>().material = _material;
        }

        var childMesh = interactor.transform.parent.GetComponentsInChildren<MeshFilter>();
        if (childMesh != null)
        {
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

    public void SnapInteractable_WhenInteractorViewAdded(IInteractorView view)
    {
        _socket?.SetActive(true);
    }

    public void SnapInteractable_WhenInteractorViewRemoved(IInteractorView view)
    {
        _socket.SetActive(false);
    }

    public void SnapInteractable_WhenSelectingInteractorViewAdded(IInteractorView view)
    {
        _socket?.SetActive(false);
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}