using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{

    [SerializeField] GameObject vrCam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (vrCam != null)
        {
            // get camera's current y rotation, x and z position
            float cameraYRot = vrCam.transform.rotation.y;
            float cameraXPos = vrCam.transform.position.x;
            float cameraZPos = vrCam.transform.position.z;

            // apply the y rotation to the UI canvas, and also move the 
            transform.rotation = Quaternion.LookRotation(transform.position - vrCam.transform.position);
            transform.position = new Vector3(cameraXPos, 2.4f, cameraZPos + 5f);
        }
        
    }
}
