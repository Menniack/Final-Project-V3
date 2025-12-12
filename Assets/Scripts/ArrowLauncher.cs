using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowLauncher : MonoBehaviour
{

    [SerializeField] private float _speed = 10f;

    [SerializeField] private GameObject _trailSystem;

    private Rigidbody _rigidBody;
    private bool _inAir = false;
    private XRPullInteractable _pullInteractable;

    private void Awake()
    {
        InitializeComponents();
        SetPhysics(false);
    }

    private void InitializeComponents()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if (_rigidBody == null)
        {
            Debug.LogError($"No Rigidbody component on Arrow {gameObject.name}");
        }
    }


    // this is to help simulate the way an arrow behaves in the air
    private IEnumerator RotateWithVelocity()
    {
        yield return new WaitForFixedUpdate();
        while (_inAir)
        {
            if (_rigidBody != null && _rigidBody.linearVelocity.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(_rigidBody.linearVelocity, transform.up);

            }
            yield return null;
        }
    }
    public void Initialize(XRPullInteractable pullInteractable)
    {
        _pullInteractable = pullInteractable;
        _pullInteractable.PullActionReleased += Release;
    }
    private void OnDestroy()
    {
        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= Release;
        }
    }

    private void Release(float value)
    {
        if (_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= Release;
        }

        gameObject.transform.parent = null;
        _inAir = true;
        SetPhysics(true);

        Vector3 force = transform.forward * value * _speed;
        _rigidBody.AddForce(force, ForceMode.Impulse);

        StartCoroutine(RotateWithVelocity());

        //_trailSystem.SetActive(true);
    }

    public void StopFlight()
    {
        _inAir = false;
        SetPhysics(false);
        _trailSystem.SetActive(false);
    }
    // this is to make sure the arrow doesn't fall when ON the bow
    private void SetPhysics(bool usePhysics)
    {
        if (_rigidBody != null)
        {
            _rigidBody.useGravity = usePhysics;
            _rigidBody.isKinematic = !usePhysics;
        }
    }
}
