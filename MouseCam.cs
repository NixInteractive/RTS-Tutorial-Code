using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script makes it so when the player holds down the middle mouse button the camera moves with the mouse like a first person character. This script gets attached to the main camera that the player controls
public class MouseCam : MonoBehaviour {

    Vector2 mousePos; //The mouse position as a vector2
    Vector2 smoothingVector; //The smoothing to be applied to the movement

    public float sensitivity = 5f; //Mouse Sensitivity
    public float smoothing = 2f; //The value to be applied to the smoothingVector

    private GameObject Player; //Player GameObject (Duh :p)

    // Use this for initialization
    void Start () {

        Player = GameObject.FindGameObjectWithTag("Player"); //Retrieves the player object
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetMouseButton(2)) //Is the player holding down the middle mouse button?
        {
            var mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")); //The direction the mouse is moving

            mouseDir = Vector2.Scale(mouseDir, new Vector2(sensitivity * smoothing, sensitivity * smoothing)); //Applies smoothing to the sensitivity and applies it to the movement

            smoothingVector.x = Mathf.Lerp(smoothingVector.x, mouseDir.x, 1f / smoothing); //Defines the x smoothing vector with the mouse movement
            smoothingVector.y = Mathf.Lerp(smoothingVector.y, mouseDir.y, 1f / smoothing); //Defines the y smoothing vector with the mouse movement

            mousePos += smoothingVector; //Adds the movement to the mouse position

            mousePos.y = Mathf.Clamp(mousePos.y, -66, 90); //Clamps the vertical movement to prevent odd angles

            transform.localRotation = Quaternion.AngleAxis(-mousePos.y, Vector3.right); //Rotates the camera based on the mouse position

            Player.transform.localRotation = Quaternion.AngleAxis(mousePos.x, Player.transform.up); //Rotates the player to keep movement consistent
        }
    }
}

