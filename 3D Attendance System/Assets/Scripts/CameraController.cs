using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CameraController : MonoBehaviour
{
    public GameObject controller;
    public GameObject sceneCamera; 
    public GameObject closeUpCamera; 
    public Canvas realLifeCanvas; 
    public Canvas signUpCanvas; 
    public Text pictureAlreadySaved;
    public Text noPictureFound; 


    public void DeviceCamOn()
    {
        if(!controller.GetComponent<DBController>().pictureTaken && !controller.GetComponent<DBController>().done)
        {
            noPictureFound.GetComponent<Text>().enabled = false;
            signUpCanvas.GetComponent<Canvas>().enabled = false;
            StartCoroutine(camSwitch());
        }
        else if(controller.GetComponent<DBController>().done)
        {
            pictureAlreadySaved.GetComponent<Text>().enabled = true;
        }
    }

    IEnumerator camSwitch()
    {
        sceneCamera.SetActive(false); 
        yield return new WaitForSeconds(2f); 
        realLifeCanvas.GetComponent<Canvas>().enabled = true; 
        controller.GetComponent<DBController>().pictureTaken = true;
    }

    void Update()
    {
        if(controller.GetComponent<DBController>().done)
        {
            StartCoroutine(switchBack());
        }
    }

    IEnumerator switchBack()
    {
        realLifeCanvas.GetComponent<Canvas>().enabled = false;
        yield return new WaitForSeconds(0.2f);
        sceneCamera.SetActive(true);
        yield return new WaitForSeconds(1f);
        if(controller.GetComponent<DBController>().pictureTaken)
        {
            signUpCanvas.GetComponent<Canvas>().enabled = true;
        }
        controller.GetComponent<DBController>().pictureTaken = false;
    }
}
