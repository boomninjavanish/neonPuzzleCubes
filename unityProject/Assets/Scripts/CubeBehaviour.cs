using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class CubeBehaviour : MonoBehaviour
{    
    // the cube; all hail the cube
    public GameObject cube;

    // track rotation position
    private float cubeXAngle = 0.0f;
    private float cubeYAngle = 0.0f;

    // do not roatate again for X number of  seconds
    private float rotationTime = 5.0f;
    private float currentTime = 0.0f;
    private float finishRotatingTime = 0.0f;

    // vars for changing the rotation on the cube
    private GazeAware gazeAwareComponent;

    // track if the direction has focus
    private bool upHasFocus = false;
    private bool downHasFocus = false;
    private bool rightHasFocus = false;
    private bool leftHasFocus = false;

    private bool currentlyXRotating = false;
    private bool currentlyYRotating = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        gazeAwareComponent = GetComponent<GazeAware>();
        //cube.transform.Rotate(100.0f, 0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // get the current time
        currentTime = Time.time;
        
        
        // set rotation flag if something has focus
        if (gazeAwareComponent.HasGazeFocus)
        { 

            // rotate cube 90 degrees then wait before rotating again
            if (this.name.Contains("SphereUp") & currentlyYRotating == false)
            {
                upHasFocus = true;
            }

            if (this.name.Contains("SphereDown") & currentlyYRotating == false)
            {
                //downHasFocus = true;
                    
            }

            if (this.name.Contains("SphereLeft") & currentlyXRotating == false)
            {
                leftHasFocus = true;
            }

            if (this.name.Contains("SphereRight") & currentlyXRotating == false)
            {
                //rightHasFocus = true;   
            }
            
            
        }

        // get the angle and axis to see if we are currently rotating
        float angle = 0.0f;
        Vector3 axis = Vector3.zero;

        cube.transform.rotation.ToAngleAxis(out angle, out axis);

        // rotate cube 90 degrees based on focus flag and lock position for a moment
        if (upHasFocus == true)
        {
            Quaternion currentRotation = cube.transform.rotation;
            Quaternion toRotation = currentRotation * Quaternion.AngleAxis(90, Vector3.left);
            
            cube.transform.rotation = Quaternion.Slerp(currentRotation, toRotation, 2.0f * Time.deltaTime);

            cube.transform.rotation.ToAngleAxis(out cubeYAngle, out axis);

            if ((Mathf.Round(cubeYAngle) % 90) == 0)
            {
                upHasFocus = false;
                currentlyYRotating = false;
            } else
            {
                currentlyYRotating = true;
            }
            
        }

        if (downHasFocus == true)
        {

        }

        if (leftHasFocus == true)
        {
            Quaternion currentRotation = cube.transform.rotation;
            Quaternion toRotation = currentRotation * Quaternion.AngleAxis(90, Vector3.up);

            cube.transform.rotation = Quaternion.Slerp(currentRotation, toRotation, 2.0f * Time.deltaTime);

            cube.transform.rotation.ToAngleAxis(out cubeXAngle, out axis);

            if ((Mathf.Round(cubeXAngle) % 90) == 0)
            {
                leftHasFocus = false;
                currentlyXRotating = false;
            }
            else
            {
                currentlyXRotating = true;
            }
        }

        if (rightHasFocus == true)
        {

        }

        

    }
}
