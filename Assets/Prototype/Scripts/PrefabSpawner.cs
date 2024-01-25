using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PrefabSpawner : XRSocketInteractor
{
    [SerializeField] GameObject prefab = default;
    private Vector3 attachOffset = Vector3.zero;
    public Arrow currentArrow;

    [Header("Sound")]
    public AudioClip grabClip;

    protected override void Awake()
    {
        base.Awake();
        CreateAndSelectPrefab();
        SetAttachOffset();
    }
    protected override void OnSelectExit(XRBaseInteractable interactable)
    {
        base.OnSelectExit(interactable);
        CreateAndSelectPrefab();
    }

    void CreateAndSelectPrefab()
    {
        Arrow interactable = CreatePrefab();
        SelectPrefab(interactable);

    }
    Arrow CreatePrefab()
    {
        currentArrow = Instantiate(prefab, transform.position - attachOffset, transform.rotation).GetComponent<Arrow>();
        return currentArrow;
    }
    void SelectPrefab(Arrow interactable)
    {
        OnSelectEnter(interactable);
        interactable.OnSelectEnter(this);
    }

    void SetAttachOffset()
    {
        if (selectTarget is XRGrabInteractable interactable)
        {
            attachOffset = interactable.attachTransform.localPosition;
        }
    }

    public void ForceDeinteract(XRBaseInteractable interactable)
    {
        OnSelectExit(interactable);
    }
}
