using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicWebcam : MonoBehaviour {

    public WebCamTexture webcam;
    public RawImage screenTexture;
    public Texture2D screenTexture2;

    // Use this for initialization
    void Start () {

        var devices = WebCamTexture.devices;
        webcam = new WebCamTexture(devices[0].name);

        Debug.Log("Webcam '" + webcam.name + "' dimensions: " + webcam.width + "," + webcam.height + ", isplaying=" + webcam.isPlaying);

        webcam.Play();

        screenTexture.texture = webcam;

        screenTexture2 = new Texture2D(webcam.width, webcam.height);
        GetComponent<MeshRenderer>().material.mainTexture = screenTexture2;
    }

    // Update is called once per frame
    void Update () {
        if (!webcam.isPlaying)
        {
            return;
        }

        //Debug.Log("Basic: "+WebCamTexture.devices.Length);
        foreach (var device in WebCamTexture.devices)
        {
            var webcam = new WebCamTexture(device.name);
            //Debug.Log("Webcam '" + webcam.name + "' dimensions: " + webcam.width + "," + webcam.height + ", isplaying=" + webcam.isPlaying);
        }
        //Debug.Log("Webcam '" + webcam.name + "' dimensions: " + webcam.width + "," + webcam.height + ", isplaying=" + webcam.isPlaying);
        

        var image = webcam.GetPixels();
        screenTexture2.SetPixels(image);
        screenTexture2.Apply();
    }
}
