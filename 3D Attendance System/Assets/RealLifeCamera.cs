using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class RealLifeCamera : MonoBehaviour
{
    private bool camAvailable; 
    private WebCamTexture deviceCam;
    Texture defaultBackground; 
    public RawImage background; 

    public AspectRatioFitter fit; 
    float scaleY; 

    void Start()
    {
        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices; 

        if(devices.Length == 0 )
        {
            //no cam detected. no need for image. 
            camAvailable = false;
            return; 
        }
        
        for(int i=0; i < devices.Length ; i++)
        {
            if(devices[i].isFrontFacing)
            {
                deviceCam = new WebCamTexture(devices[i].name, Screen.width, Screen.height); 
            }
        }

        deviceCam.Play();
        background.texture = deviceCam; 

        camAvailable = true;

    }

    void Update()
    {
        if(!camAvailable)
        {
            return; 
        }
        
        float ratio = (float)deviceCam.width / (float)deviceCam.height;
        fit.aspectRatio = ratio; 

        if(deviceCam.videoVerticallyMirrored)
        {
            scaleY = -1; 
        }
        else
        {
            scaleY = 1; 
        }

        background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

        int orient = -deviceCam.videoRotationAngle; 
        background.rectTransform.localEulerAngles = new Vector3(0,0, orient);

    }
}
