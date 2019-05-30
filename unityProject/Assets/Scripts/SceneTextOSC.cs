using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.Gaming;

public class SceneTextOSC : MonoBehaviour
{
    public OSC osc;
    public TextMesh sceneText;
    private GazeAware gazeAwareComponent;

    // grab some outside elements to make the appear and disappear
    public UnityEngine.Video.VideoPlayer video;
    public MeshRenderer cube1;
    public MeshRenderer cube2;
    public MeshRenderer cube3;

    // video is ready to appear flag
    private bool playVideo = false;

    // use timer to track how long we are looking in seconds
    private float gazeTimeElasped = 0.0f;
    private float gazeTimeStarted = 0.0f;
    private const float GAZE_TIME = 3.0f;
    private bool gazeStarted = false;
    private bool messageSent = false;

    // the colors, man, the colors
    private MeshRenderer meshRenderer;
    public Color selectionColor;
    private Color lerpColor;
    private Color deselectedColor;
    private const float FADE_SPEED = 0.01f;

    // track the scene which we are in
    private int sceneNumber;

    private void Start()
    {
        // setup the text to be gaze aware
        gazeAwareComponent = GetComponent<GazeAware>();

        // setup the color handeling
        meshRenderer = GetComponent<MeshRenderer>();

        
        lerpColor = meshRenderer.material.color;
        deselectedColor = Color.yellow;

        // receive the OSC messages
        osc.SetAddressHandler("/scene", OnReceive);

        

    }

    void Update()
    {
        // change text color to selection color
        if (meshRenderer.material.color != lerpColor)
        {
            meshRenderer.material.color = Color.Lerp(meshRenderer.material.color, lerpColor, FADE_SPEED);
        }

        // upon gaze aware start to change disk color
        if (gazeAwareComponent.HasGazeFocus)
        {
            // lerp the color
            SetLerpColor(selectionColor);

            if (gazeStarted == false)
            {
                // set the timer to the current time
                gazeTimeStarted = Time.time;
                gazeStarted = true;
            }

            // send request for new scene number 
            if (gazeTimeElasped < GAZE_TIME)
            {
                // set the elapsed time
                gazeTimeElasped = Time.time - gazeTimeStarted;

                // send the scene request
                if (gazeTimeElasped > 2.0f)
                {
                    if (messageSent == false)
                    {
                        // send scene request to Max
                        // Max will send back a scene number to confirm the change has taken place
                        OscMessage sceneRequest = new OscMessage();
                        sceneRequest.address = "/sceneRequest";
                        sceneRequest.values.Add("bang");
                        osc.Send(sceneRequest);
                        messageSent = true;
                    }
                    

                }
            }
           
        } else
        {
            // change back to white
            SetLerpColor(deselectedColor);

            // set gaze timer to zero
            gazeTimeElasped = 0.0f;
            gazeStarted = false;
            messageSent = false;

        }

        // change the video alpha based on the state of the playback flag
        if (playVideo == true & video.targetCameraAlpha < 1)
        {
            video.targetCameraAlpha += 0.04f * Time.deltaTime;
        }

        if (playVideo == false & video.targetCameraAlpha > 0)
        {
            video.targetCameraAlpha -= 0.04f * Time.deltaTime;
        }

        Debug.Log(video.targetCameraAlpha);
    }

    void OnReceive(OscMessage message)
    {
        // get the scene number
        sceneNumber = message.GetInt(0);

        // update the scene number on the text
        sceneText.text = sceneNumber.ToString();

        // disappear and reappear elements on the screen based on the scene number
        switch (sceneNumber)
        {
            
            case 1:
                // play video
                //video.Play();
                playVideo = true;

                // make middle cube appear (1)
                cube1.enabled = true;

                break;

            case 2:
                // make right cube appear (3)
                cube3.enabled = true;

                break;

            case 3:
                // make left cube appear (2)
                cube2.enabled = true;

                break;

            case 4:
                // kill video
                //video.Stop();
                playVideo = false;
                

                // disappear all the cubes
                cube1.enabled = false;
                cube2.enabled = false;
                cube3.enabled = false;

                break;
        }

    }

    // update the color used for lerping
    public void SetLerpColor(Color lerpColor)
    {
        this.lerpColor = lerpColor;
    }
}
