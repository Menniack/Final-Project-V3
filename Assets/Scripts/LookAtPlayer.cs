using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    [SerializeField] GameObject _object;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 
        transform.rotation = Quaternion.LookRotation(transform.position - _object.transform.position);
    }
}
