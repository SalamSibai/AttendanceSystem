using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text; 
using UnityEngine.UI; 

public class ImageConverter : MonoBehaviour
{
    public List<Texture2D> myTextures = new List<Texture2D>(); 
    byte[] bytes;
    string encodedText; 
    //public RawImage pic; 


    void Start()
    {

        for(int i= 0; i< 3; i++)
        {
            bytes = myTextures[i].EncodeToPNG();    //encodes to a jpg byte array.
            encodedText = Convert.ToBase64String(bytes); 
            Debug.Log("encoded text " + i + ": " + encodedText);
        }

        bytes = Convert.FromBase64String(encodedText);

        Texture2D decodedTexture = new Texture2D(1920,1080); 


        decodedTexture.LoadImage(bytes); //decode back to a texture to show 
        decodedTexture.Apply();
        //pic.texture = decodedTexture; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
