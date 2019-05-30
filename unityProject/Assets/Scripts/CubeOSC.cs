using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOSC : MonoBehaviour
{
    // setup open sound control 
    public OSC osc;

   

    // Update is called once per frame
    void Update()
    {
        // send the OSC messages
        SendOSC();
        
    }

    private void SendOSC()
    {
        // start OSC message objects
        OscMessage messageX = new OscMessage();
        OscMessage messageY = new OscMessage();
        OscMessage messageZ = new OscMessage();
        OscMessage messageW = new OscMessage();

        // set the osc addresses
        messageX.address = "/" + this.name + "X";
        messageY.address = "/" + this.name + "Y";
        messageZ.address = "/" + this.name + "Z";
        messageW.address = "/" + this.name + "W";

        // send the rotation values via OSC
        messageX.values.Add(this.transform.rotation.x);
        messageY.values.Add(this.transform.rotation.y);
        messageZ.values.Add(this.transform.rotation.z);
        messageW.values.Add(this.transform.rotation.w);

        osc.Send(messageX);
        osc.Send(messageY);
        osc.Send(messageZ);
        osc.Send(messageW);
    }

   
}
