﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject controller;
    public GameObject sceneCamera; 
    public GameObject closeUpCamera; 
    public GameObject realLifeCamera; 
    public Canvas realLifeCanvas; 
    public Canvas signUpCanvas; 

    public void DeviceCamOn()
    {
        signUpCanvas.GetComponent<Canvas>().enabled = false;
        StartCoroutine(camSwitch());
    }

    IEnumerator camSwitch()
    {
        sceneCamera.SetActive(false); 
        yield return new WaitForSeconds(2f); 
        realLifeCamera.SetActive(true); 
        realLifeCanvas.GetComponent<Canvas>().enabled = true; 
    }

    void Update()
    {
        if(controller.GetComponent<DBController>().pictureTaken)
        {
            StartCoroutine(switchBack());
        }
    }

    IEnumerator switchBack()
    {
        realLifeCanvas.GetComponent<Canvas>().enabled = false;
        realLifeCamera.SetActive(false); 
        yield return new WaitForSeconds(2f);
        sceneCamera.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        signUpCanvas.GetComponent<Canvas>().enabled = true;
        controller.GetComponent<DBController>().pictureTaken = false;

    }
}
