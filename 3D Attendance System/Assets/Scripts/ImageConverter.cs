using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text; 
using UnityEngine.UI; 

public class ImageConverter : MonoBehaviour
{
    public Texture2D myTexture;
    byte[] bytes;
    string encodedText; 
    public RawImage pic; 


    void Start()
    {
        bytes = myTexture.EncodeToPNG();    //encodes to a jpg byte array.
        //encodedText = Convert.ToBase64String(bytes);
        
        if(bytes == null)
        {
            Debug.Log("what"); 
        }
        else
        {
            Debug.Log("yep"); 
    
        }
        //Debug.Log(encodedText);

        // tex.LoadRawTextureData(myTexture.EncodeToJPG());
        // tex.Apply();
        pic.texture = myTexture; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
