using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets attached to the Player object and provides the ability to pan the camera 
public class CameraPanning : MonoBehaviour {

    float panSpeed = 3f; //How fast the player moves
    float panDetect = 15f; //The space from the edge of the screen that the mouse will be detected
    float oldAltitude; //The stored altitude value for the player
    float altitude; //The current player altitude

    // Use this for initialization
    private void Start()
    {
        oldAltitude = Terrain.activeTerrain.SampleHeight(transform.position); //Sets the player's oldAltitude to be equal to altitude
    }

    // Update is called once per frame
    void Update()
    {

        float moveX = Input.GetAxis("Horizontal") * panSpeed * Time.deltaTime; //Left and right movement
        float moveY = Input.GetAxisRaw("Mouse ScrollWheel"); //Vertical movement
        float moveZ = Input.GetAxis("Vertical") * panSpeed * Time.deltaTime; //Forward and backward movement
        float xPos = Input.mousePosition.x; //The x position of the mouse
        float yPos = Input.mousePosition.y; //The y position of the mouse

        altitude = Terrain.activeTerrain.SampleHeight(transform.position); //Sets the player's altitude value

        if (oldAltitude != altitude) //Is the stored altitude value different from the current altitude value
        {
            AltAdjust(); //Calls the AltAdjust() method
            oldAltitude = altitude; //Sets the stored altitude value to be the current altitude value
        }

        if (transform.position.y >= -10 && transform.position.y <= 1000) //Is the y value of the player transform position between -10 and 1000
        {
            moveY *= 10; //Multiplies the moveY value by 10
            transform.Translate(new Vector3(0, moveY, 0)); //Moves the player up or down based on the moveY value
        }

        if (Input.GetKey(KeyCode.A) || xPos > 0 && xPos < panDetect) //Is the player holding down the A key or is the mouse within 15 pixels of the left edge of the screen?
        {
            moveX -= panSpeed; //Subtract the panSpeed from the moveX value
        }
        else if (Input.GetKey(KeyCode.D) || xPos < Screen.width && xPos > Screen.width - panDetect) //Is the player holding down the D key or is the mouse within 15 pixels of the right edge of the screen?
        {
            moveX += panSpeed; //Add the panSpeed to the moveX value
        }

        if (Input.GetKey(KeyCode.W) || yPos < Screen.height && yPos > Screen.height - panDetect) //Is the player holding down the W key or is the mouse within 15 pixels of the top edge of the screen?
        {
            moveZ += panSpeed; //Add the panSpeed to the moveZ value
        }
        else if (Input.GetKey(KeyCode.S) || yPos > 0 && yPos < panDetect) //Is the player holding down the S key or is the mouse within 15 pixels of the bottom edge of the screen?
        {
            moveZ -= panSpeed; //Subtract the panSpeed from the moveZ value
        }

        transform.Translate(new Vector3(moveX, Input.GetAxis("Mouse ScrollWheel") * panSpeed, moveZ)); //Moves the player using the above values

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, 10 + altitude, 100 + altitude), transform.position.z); //Clamps the vertical movement

    }

    //Adjusts the player height to follow the terrain. This needs to be smoothed out before it will be an effective method.
    public void AltAdjust()
    {
        float newHeight = transform.position.y; //stores the player transform's y position

        Vector3 pos = transform.position; //Stores the player transform position 

        pos.y = altitude; //Sets the y position value to equal the current altitude of the player

        if (altitude > oldAltitude)//Is the altitude greater than the oldAltitude?
        {
            pos.y += newHeight; //Adds the current y position value to the altitude
        }
        else
        {
            pos.y -= newHeight; //Subtracts the current y position value to the altitude
        }

        transform.position = pos; //Sets the player's transform position to the new height
    }
}
