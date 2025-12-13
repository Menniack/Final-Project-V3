using UnityEngine;
using System.Collections;

public class ArrowImpactHandler : MonoBehaviour
{
    [Header("Impact Settings")]
    [SerializeField] private bool _explodeOnImpact = false;
    [SerializeField] private float _stickDuration = 3f;
    [SerializeField] private float _minEmbedDepth = 0.05f;
    [SerializeField] private float _maxEmpedDepth = 0.15f;
    [SerializeField] private LayerMask _ignorelayers;
    [SerializeField] private Transform _tip;


    [Header("Visual and Audio Effects")]
    [SerializeField] private GameObject _impactGameObject;
    [SerializeField] private MeshRenderer _arrowMeshRenderer;

    [SerializeField] private AudioClip _impactSound;


    private AudioSource _audioSource;

    private ArrowLauncher _arrowLauncher;
    private Rigidbody _rigidBody;
    private bool _hasHit = false;

    private void Awake()
    {
        // Get required components
        _arrowLauncher = GetComponent<ArrowLauncher>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource.GetComponent<AudioSource>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        // if arrow has hit OR the collision layer and ignorelayers are not equal to 0...
        if (_hasHit || ((1 << collision.gameObject.layer) & _ignorelayers) != 0)
        {
            return;
        }
        
        _hasHit = true;
        _arrowLauncher.StopFlight();
        
        // If the arrow wants to explode on impact...
        if (_explodeOnImpact)
        {
            HandleExplosion();
        }
        else
        {
            HandleStick(collision);
        }
    }

    // Helper function that will help handle the explosion, if needed
    private void HandleExplosion()
    {
        // If the arrow is visually there, remove the mesh to make it invisible
        if(_arrowMeshRenderer != null)
        {
            _arrowMeshRenderer.enabled = false;
        }
        // If the game object is null, then make it not null
        if (_impactGameObject != null)
        {
            Instantiate(_impactGameObject, transform.position, Quaternion.identity);
        }
        // Be sure to destroy as not to eat up performance
        Destroy(gameObject);
    }


    // Helper function that will help with the arrow "sticking" into an object
    private void HandleStick(Collision collision)
    {
        // Get the correct arrow direction and rotation, to correctly simulate the stick
        Vector3 arrowDirection = transform.forward;
        Vector3 arrowUp = transform.up;
        ContactPoint contact = collision.GetContact(0);

        float randomDepth = Random.Range(_minEmbedDepth, _maxEmpedDepth);
        Quaternion finalRotation = Quaternion.LookRotation(arrowDirection, arrowUp);
        Vector3 centerOffset = _tip.localPosition;
        // This is a LOTTA mumbo jumbo, but it's all just math, to ensure the final position is within the object,
        // not sticking outside the objects collision box
        Vector3 finalPosition = contact.point - (finalRotation * centerOffset) + contact.normal * -randomDepth;

        transform.SetPositionAndRotation(finalPosition, finalRotation);

        // Create a joint that allows the arrow to do this
        CreateStabJoint(collision, randomDepth);

        transform.SetParent(collision.transform, true);
        _audioSource.clip = _impactSound;
        _audioSource.Play();
        StartCoroutine(DespawnAfterDelay());
    }

    public ConfigurableJoint CreateStabJoint(Collision collision, float randomDepth)
    {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = collision.rigidbody;
        // Ensure the arrow moves with the object, with the same orientation as the object, but can still move
        // with the object
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;

        var limit = joint.linearLimit;
        limit.limit = randomDepth;
        joint.linearLimit = limit;
        return joint;
    }

    // function to destroy the arrow after a few seconds
    private IEnumerator DespawnAfterDelay()
    {
        yield return new WaitForSeconds(_stickDuration);
        Destroy(gameObject);
    }
}