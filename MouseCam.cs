using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script makes it so when the player holds down the middle mouse button the camera moves with the mouse like a first person character.
//This script gets attached to the main camera that the player controls
public class MouseCam : MonoBehaviour {

    //The mouse position as a vector2
    Vector2 mousePos;

    //The smoothing to be applied to the movement
    Vector2 smoothingVector;

    //Mouse Sensitivity
    public float sensitivity = 5f;

    //The value to be applied to the smoothingVector
    public float smoothing = 2f;

    //Player GameObject (Duh :p)
    public GameObject Player;

    // Use this for initialization
    void Start () {

        //Retrieves the player object
        Player = GameObject.FindGameObjectWithTag("Player");
    }
	
	// Update is called once per frame
	void Update () {

        //Applies rotation when the player holds down the middle mouse button
        if (Input.GetMouseButton(2))
        {
            //The direction the mouse is moving
            var mouseDir = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            //Applies smoothing to the sensitivity and applies it to the movement
            mouseDir = Vector2.Scale(mouseDir, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

            //Defines the smoothing vector with the mouse movement
            smoothingVector.x = Mathf.Lerp(smoothingVector.x, mouseDir.x, 1f / smoothing);
            smoothingVector.y = Mathf.Lerp(smoothingVector.y, mouseDir.y, 1f / smoothing);

            //Adds the movement to the mouse position
            mousePos += smoothingVector;

            //Clamps the vertical movement to prevent odd angles
            mousePos.y = Mathf.Clamp(mousePos.y, -66, 90);

            //Rotates the camera based on the mouse position
            transform.localRotation = Quaternion.AngleAxis(-mousePos.y, Vector3.right);

            //Rotates the player to keep movement consistent
            Player.transform.localRotation = Quaternion.AngleAxis(mousePos.x, Player.transform.up);
        }
    }
}
