using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PullInteraction : XRBaseInteractable
{
    private Bow bow;
    public float PullAmount { get; private set; } = 0.0f;
    public Transform start, end;
    private XRBaseInteractor pullingInteractor = null;
    private float PullAmountTemp;

    [Header("Polish")]
    public LineRenderer stringLine;
    [ColorUsage(true,true)]
    public Color stringNormalCol, stringPulledCol;
    public ParticleSystem lineParticle;
    public bool showHandsOnPull = true;
    public GameObject rightHand, leftHand;

    protected override void Awake()
    {
        base.Awake();
        bow = GetComponentInParent<Bow>();
    }
    public void ForceInteract(XRBaseInteractor interactor)
    {
        OnSelectEnter(interactor);
    }
    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        base.OnSelectEnter(interactor);
        pullingInteractor = interactor;

        //haptic
        if(pullingInteractor.TryGetComponent(out XRController controller))
            HapticManager.Impulse(.5f, .05f, controller.inputDevice);

    }
    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        base.OnSelectExit(interactor);
        pullingInteractor = null;
        PullAmount = 0f;

        //polish
        stringLine.material.SetColor("_EmissionColor", stringNormalCol);
        lineParticle.Play();
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);
        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (isSelected)
            {
                Vector3 pullPosition = pullingInteractor.transform.position;
                PullAmount = CalculatePull(pullPosition);

                //haptic
                if (pullingInteractor.TryGetComponent(out XRController controller) && PullAmount > .3f)
                    HapticManager.Impulse(PullAmount/5f, .05f, controller.inputDevice);

                //polish
                stringLine.material.SetColor("_EmissionColor", 
                    Color.Lerp(stringNormalCol, stringPulledCol, PullAmount));
            }
        }
    }
    private float CalculatePull(Vector3 pullPosition)
    {
        Vector3 pullDirection = pullPosition - start.position;
        Vector3 targetDirection = end.position - start.position;
        float maxLength = targetDirection.magnitude;

        targetDirection.Normalize();
        float pullValue = Vector3.Dot(pullDirection, targetDirection) / maxLength;
        return Mathf.Clamp(pullValue, 0, 1);
    }

    public void ShowHand(bool show)
    {
        if (show)
        {
            if (showHandsOnPull && bow.selectingInteractor.TryGetComponent(out XRController controller))
            {
                //print(controller.inputDevice.role == UnityEngine.XR.InputDeviceRole.RightHanded);
                leftHand.SetActive(controller.inputDevice.role == UnityEngine.XR.InputDeviceRole.RightHanded);
                rightHand.SetActive(controller.inputDevice.role == UnityEngine.XR.InputDeviceRole.LeftHanded);
            }
        }
        else
        {
            if (showHandsOnPull)
            {
                leftHand.SetActive(false);
                rightHand.SetActive(false);
            }
        }
    }


}
