using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles camera/player movement. It gets attached to the Player object
public class Controller : MonoBehaviour {

    //How fast the camera moves
    float panSpeed = 7;

    //The space from the edge of the screen that the mouse will be detected
    float panDetect = 15;
	
	// Update is called once per frame
	void Update () {

        //Stores the player's inputs as floats for movement
        float moveX = Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime;
        float moveY = Input.GetAxisRaw("Mouse ScrollWheel");
        float moveZ = Input.GetAxis("Vertical") * panSpeed * Time.deltaTime;

        //Grabs the mouse position
        float xPos = Input.mousePosition.x;
        float yPos = Input.mousePosition.y;

        //Moves the player up and down
        if(transform.position.y >= -5 && transform.position.y <= 5)
        {
            moveY *= panSpeed;
            transform.Translate(new Vector3(0, moveY, 0));
        }

        //Handles left and right movement
        if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect)
        {
            moveX -= panSpeed;
        }
        else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect)
        {
            moveX += panSpeed;
        }

        //Handles forward and backwards movement
        if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect)
        {
            moveZ += panSpeed;
        }
        else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect)
        {
            moveZ -= panSpeed;
        }

        //Moves the player using the above values
        transform.Translate(new Vector3(moveX, Input.GetAxis("Mouse ScrollWheel") * panSpeed, moveZ));

        //Clamps the vertical movement
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 10, 100), transform.position.z);
    }
}
