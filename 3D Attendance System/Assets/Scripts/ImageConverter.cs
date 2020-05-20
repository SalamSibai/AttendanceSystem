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

        Texture2D decodedTexture = new Texture2D(1920,1080); 


        decodedTexture.LoadImage(bytes); //decode back to a texture to show 
        decodedTexture.Apply();
        pic.texture = decodedTexture; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
