using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class testScript : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage pic;
    byte[] bytes;
    void Start()
    {
       bytes = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAIAAAAB2CAIAAACPlwyYAAAgAElEQVR4Ac2d6ZNcx3Hg393XTM");
        Texture2D decodedTexture = new Texture2D(1920,1080); 


        decodedTexture.LoadImage(bytes); //decode back to a texture to show 
        decodedTexture.Apply();

        pic.texture = decodedTexture;
    }
}
