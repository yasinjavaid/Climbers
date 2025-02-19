using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject player;        //Public variable to store a reference to the player game object


    private Vector3 offset;            //Private variable to store the offset distance between the player and camera

    private Vector3 tempPosition;
    // camera will follow this object
    public Transform Target;
    //camera transform
    public Transform camTransform;
    // offset between camera and target
    private Vector3 Offset;
    // change this value to get desired smoothness
    public float SmoothTime = 0.3f;

    // This value will change at the runtime depending on target movement. Initialize with zero vector.
    private Vector3 velocity = Vector3.zero;
    private void Start()
    {
        Offset = camTransform.position - Target.position;
    }
    private void LateUpdate()
    {
        // update position
        tempPosition = Target.position + Offset;
        tempPosition = new Vector3(transform.position.x, tempPosition.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, tempPosition, ref velocity, SmoothTime);

        // update rotation
       // transform.LookAt(Target);
    }
}