using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraFollow : MonoBehaviour
{

    [SerializeField]private GameObject target;
    [SerializeField]private float smoothSpeed = 4f;
    [SerializeField]private Vector3 offset;
    [SerializeField]private float smoothRotateSpeed = 4f;


    private void LateUpdate()
    {   //for camera positioning
        Transform targetTransform = target.transform;
        Vector3 desiredPosition = targetTransform.position+offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed*Time.deltaTime);
        //for camera rotation
        PlayerScript playerScript = target.GetComponent<PlayerScript>();
        Vector3 movementVelocity =  playerScript.GetMovementVelocity();
        Vector3 movementDirection = movementVelocity.normalized;
        if (movementVelocity!=Vector3.zero)
        {
            Vector3 direction = movementVelocity - transform.position;
            Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, smoothRotateSpeed * Time.deltaTime);
        }
     
    }
}
