using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class HeadPoseTracking : MonoBehaviour
{
    HeadPose headPose;

    //public Transform Head;

    public float Responsiveness = 10f;

    private string xPosition, yPosition, zPosition, rotation;
    private float xRotation, yRotation = 0.0f;
    private Vector3 cameraRotation;
    public Vector2 LeftEyePosition { get; private set; }
    public Vector2 RightEyePosition { get; private set; }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        headPose = TobiiAPI.GetHeadPose();
        /*
        if (headPose.IsRecent())
        {

            //transform.localRotation = Quaternion.Lerp(transform.localRotation, headPose.Rotation, Time.unscaledDeltaTime * Responsiveness);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, headPose.Rotation, Time.unscaledDeltaTime * Responsiveness);
        }
        */
        var gazePoint = TobiiAPI.GetGazePoint();
        if (gazePoint.IsRecent() && Camera.main != null)
        {
            var eyeRotation = Quaternion.Euler((gazePoint.Viewport.y - 0.5f) * Camera.main.fieldOfView, (gazePoint.Viewport.x - 0.5f) * Camera.main.fieldOfView * Camera.main.aspect, 0);
            

            var eyeLocalRotation = Quaternion.Inverse(transform.localRotation) * eyeRotation;

            var pitch = eyeLocalRotation.eulerAngles.x;
            

            if (pitch > 180) pitch -= 360;
            var yaw = eyeLocalRotation.eulerAngles.y;
            if (yaw > 180) yaw -= 360;

            LeftEyePosition = new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad));
            RightEyePosition = new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad));
        }
        
    }
}
