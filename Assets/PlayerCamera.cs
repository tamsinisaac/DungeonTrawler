using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    //Variables
    public Transform player;
    public float mouseSensitiveity = 2f;
    float cameraVerticalRotation = 0f;

    bool lockedCursor = true;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
       
    }

    // Update is called once per frame
    void Update()
    {
        //collect mouse input

        float inputX = Input.GetAxis("Mouse X")*mouseSensitiveity;
        float inputY = Input.GetAxis("Mouse Y")*mouseSensitiveity;

        //rotate the camera around its local x axis

        cameraVerticalRotation -= inputY;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, -90f, 90f);
        transform.localEulerAngles = Vector3.right*cameraVerticalRotation;

        player.Rotate(Vector3.up * inputX);

    }
}
