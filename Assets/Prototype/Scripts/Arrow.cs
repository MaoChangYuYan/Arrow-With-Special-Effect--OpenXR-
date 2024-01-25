using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class Arrow : XRGrabInteractable
{
    public float speed = 1000f;
    public Transform tip;
    bool inAir = false;
    Vector3 lastPosition = Vector3.zero;
    private Rigidbody rb;
    public Collider sphereCollider;

    [Header("Particles")]
    public ParticleSystem trailParticle;
    public ParticleSystem hitParticle;
    public TrailRenderer trailRenderer;

    [Header("Sound")]
    public AudioClip launchClip;
    public AudioClip hitClip;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();

    }
    private void FixedUpdate()
    {
        if (inAir)
        {
            CheckCollision();
            lastPosition = tip.position;
        }
    }

    private void CheckCollision()
    {
        if (Physics.Linecast(lastPosition, tip.position, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.TryGetComponent<Arrow>(out Arrow arrow))
                return;
            if (hitInfo.collider.name == "Archer_Bow")
                return;


            if (hitInfo.transform.TryGetComponent(out Rigidbody body))
            {
                if (body.TryGetComponent<Lantern>(out Lantern lantern))
                    lantern.TurnOn();

                if (body.TryGetComponent<Potion>(out Potion potion))
                {
                    potion.BreakPotion();
                    return;
                }
                rb.interpolation = RigidbodyInterpolation.None;
                transform.parent = hitInfo.transform;
                body.AddForce(rb.velocity, ForceMode.Impulse);
            }
            Stop();
        }
    }
    private void Stop()
    {
        inAir = false;
        SetPhysics(false);

        ArrowParticles(false);
        ArrowSounds(hitClip, 1.5f, 2, .8f, -2);
    }

    public void Release(float value)
    {
        inAir = true;
        SetPhysics(true);
        MaskAndFire(value);
        StartCoroutine(RotateWithVelocity());

        lastPosition = tip.position;

        ArrowParticles(true);
        ArrowSounds(launchClip, 4.2f + (.6f * value), 4.4f + (.6f * value), Mathf.Max(.7f, value), -1);


        //释放时实例化一个新箭
        if (MutilArrowManager._Instance.万箭齐发)
        {
            Release_Mirror(value, -10f);
            Release_Mirror(value, 10f);
            Release_Mirror(value, -20f);
            Release_Mirror(value, 20f);
        }
        
    }


    //释放1个镜像分身箭
    public void Release_Mirror(float value,float angle)
    {
        Transform arrowMirror = Instantiate(gameObject).transform;
        arrowMirror.position = transform.position;
        arrowMirror.rotation = transform.rotation;
        arrowMirror.eulerAngles = new Vector3( arrowMirror.eulerAngles.x, angle, arrowMirror.eulerAngles.z);

        Arrow arrow = arrowMirror.GetComponent<Arrow>();
        arrow.inAir = true;
        arrow.SetPhysics(true);
        arrow.MaskAndFire(value);
        arrow.StartCoroutine(RotateWithVelocity());

        arrow.lastPosition = tip.position;

        arrow.ArrowParticles(true);
        arrow.ArrowSounds(launchClip, 4.2f + (.6f * value), 4.4f + (.6f * value), Mathf.Max(.7f, value), -1);

    }

    private void SetPhysics(bool usePhysics)
    {
        rb.useGravity = usePhysics;
        rb.isKinematic = !usePhysics;
    }

    private void MaskAndFire(float power)
    {
        colliders[0].enabled = false;
        interactionLayerMask = 1 << LayerMask.NameToLayer("Ignore");
        Vector3 force = transform.forward * power * speed;
        rb.AddForce(force, ForceMode.Impulse);
    }
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (inAir)
        {
            Quaternion newRotation = Quaternion.LookRotation(rb.velocity, transform.up);
            transform.rotation = newRotation;
            yield return null;
        }
    }

    public void OnSelectEnter(XRBaseInteractor interactor)
    {
        base.OnSelectEnter(interactor);
    }

    public new void OnSelectExit(XRBaseInteractor interactor)
    {
        base.OnSelectExit(interactor);
    }

    public void ArrowHaptic(XRBaseInteractor interactor)
    {
        if (interactor is HandInteractor hand)
        {
            if (hand.TryGetComponent(out XRController controller))
                HapticManager.Impulse(.7f, .05f, controller.inputDevice);
        }
    }

    void ArrowParticles(bool release)
    {
        if (release)
        {
            trailParticle.Play();
            trailRenderer.emitting = true;
        }
        else
        {
            trailParticle.Stop();
            hitParticle.Play();
            trailRenderer.emitting = false;
        }
    }

    void ArrowSounds(AudioClip clip, float minPitch, float maxPitch, float volume, int id)
    {
        SFXPlayer.Instance.PlaySFX(clip, transform.position, new SFXPlayer.PlayParameters()
        {
            Pitch = Random.Range(minPitch, maxPitch),
            Volume = volume,
            SourceID = id
        });
    }

}
