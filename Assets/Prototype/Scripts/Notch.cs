using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Notch : XRSocketInteractor
{
    private PullInteraction pullInteraction;
    private Arrow currentArrow;

    [Header("Sound")]
    public AudioClip attatchClip;
    protected override void Awake()
    {
        base.Awake();
        pullInteraction = GetComponent<PullInteraction>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        pullInteraction.onSelectExit.AddListener(TryToReleaseArrow);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        pullInteraction.onSelectExit.AddListener(TryToReleaseArrow);
    }

    protected override void OnSelectEnter(XRBaseInteractable interactable)
    {
        base.OnSelectEnter(interactable);
        StoreArrow(interactable);
    }

    protected override void OnHoverEnter(XRBaseInteractable interactable)
    {
        if(interactable is Arrow arrow && arrow.selectingInteractor is HandInteractor hand)
        {
            arrow.OnSelectExit(hand);
            hand.ForceDeinteract(arrow);
            pullInteraction.ForceInteract(hand);
            hand.ForceInteract(pullInteraction);

            NotchSounds(attatchClip, 3, 3.3f, 5);
        }
    }

    private void StoreArrow(XRBaseInteractable interactable)
    {
        if (interactable is Arrow arrow)
            currentArrow = arrow;
    }

    private void TryToReleaseArrow(XRBaseInteractor interactor)
    {
        if (currentArrow)
        {
            ForceDeselect();
            ReleaseArrow();
        }
    }

    private void ForceDeselect()
    {
        base.OnSelectExit(currentArrow);
        currentArrow.OnSelectExit(this);
    }

    private void ReleaseArrow()
    {
        currentArrow.Release(pullInteraction.PullAmount);
        currentArrow = null;
    }

    public override XRBaseInteractable.MovementType? selectedInteractableMovementTypeOverride
    {
        get { return XRBaseInteractable.MovementType.Instantaneous; }
    }

    void NotchSounds(AudioClip clip, float minPitch, float maxPitch, int id)
    {
        SFXPlayer.Instance.PlaySFX(clip, transform.position, new SFXPlayer.PlayParameters()
        {
            Pitch = Random.Range(minPitch, maxPitch),
            Volume = 1.0f,
            SourceID = id
        });
    }

}