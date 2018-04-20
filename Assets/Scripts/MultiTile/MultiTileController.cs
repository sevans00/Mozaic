using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


/*
Credits:
Me!  Shaun Evans
 */
public class MultiTileController : MonoBehaviour
{
    public bool saving = false;
    public bool initialized = false;

    public WebCamTexture webcam;
    public int webcamIndex = 0;
    public MozaicMultiTile mozaicMultiTile;

    public int widthSubdivisions = 6;
    public int heightSubdivisions = 6;

    public RawImage rawImage;

    //Options menu:
    public GameObject optionsMenu;
    //Gallery:
    public Gallery gallery;

    void Start()
    {
        StartCoroutine(WaitTillReady());
    }
    
    IEnumerator WaitTillReady()
    {
        Debug.LogWarning("Waiting...");
        yield return new WaitForSeconds(1f);
        while (WebCamTexture.devices.Length == 0)
        {
            Debug.LogWarning("Webcam null or not playing");
            yield return new WaitForSeconds(1f);
        }
        Initialize();
    }

    // Use this for initialization
    void Initialize()
    {
        ScreenshotManager.OnImageSaved += ImageSaved;

        //Webcam:
        var devices = WebCamTexture.devices;
        foreach (var device in devices)
        {
            Debug.Log("devicename:" + device.name);
            Debug.Log("front facing:" + device.isFrontFacing);
            var webcamtexture = new WebCamTexture(device.name);
            Debug.Log("Webcam " + webcamtexture.name + " dimensions: " + webcamtexture.width + "," + webcamtexture.height);
        }
#if UNITY_EDITOR
		webcam = new WebCamTexture(devices[webcamIndex].name);
#else
        webcam = new WebCamTexture(devices[webcamIndex].name);
#endif
        webcam.Play();
        
        mozaicMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);

        //TODO: Raw image?
        rawImage.texture = webcam;
        rawImage.canvasRenderer.SetAlpha(0.5f);

        gallery.OnGalleryClosed += OnGalleryClosed;
        initialized = true;
    }

    public void Update()
    {
        if (   mozaicMultiTile != null 
            && initialized 
            && mozaicMultiTile.initialized 
            && !saving)
        {
            mozaicMultiTile.DoUpdate();
        }
    }

    public void OnGalleryButtonClick()
    {
        gallery.OpenGallery();
        //Pause camera:
        webcam.Pause();
    }
    public void OnGalleryClosed()
    {
        Debug.Log("Gallery Closed!");
        //Start up camera again:
        webcam.Play();
    }

    public void Click_NextCamera()
    {
        if (WebCamTexture.devices.Length <= 1)
        {
            //return;
        }

        initialized = false;
        webcam.Stop();
        var devices = WebCamTexture.devices;
        webcamIndex++;
        if (webcamIndex >= WebCamTexture.devices.Length)
        {
            webcamIndex = 0;
        }
        webcam = new WebCamTexture(devices[webcamIndex].name);
        webcam.Play();

        mozaicMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);

        rawImage.texture = webcam;
        rawImage.canvasRenderer.SetAlpha(0.5f);
        initialized = true;
    }

    //Take a picture:
    public void Click_TakePicture()
    {
        Debug.Log("Click!");
        
        string fileUID = "Mozaic" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "_"
            + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
        saving = true;
        
        //Assemble full tile:
        var mozaicTexture = AssembleImage(mozaicMultiTile.tileTexture, widthSubdivisions, heightSubdivisions);
        
        //ScreenshotManager.SaveImage(mozaicTexture, fileUID, "png");
        StartCoroutine(SavePicture(mozaicTexture));
    }
    public IEnumerator SavePicture(Texture2D mozaicTexture)
    {
        string fileUID = "Mozaic" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "_"
            + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
        saving = true;
        
        //Save Texture:
        byte[] bytes = mozaicTexture.EncodeToPNG();
        string fileExt = ".png";
        string path = Application.persistentDataPath + "/" + fileUID + fileExt;
        yield return ScreenshotManager.Instance.Save(bytes, fileUID, path, ScreenshotManager.ImageType.IMAGE);

        Debug.Log("Picture Saved!  Now to add it to the gallery");
        //yield return new WaitForSeconds(1f);
        //Add to the Gallery:
        //gallery.imageLoader.AddImageToFront(mozaicTexture, fileUID);
        yield return gallery.imageLoader.AddImageToFront(path, fileUID);
        
        Debug.Log("Added it to the gallery");
        ImageSaved(path);
    }
    
    void ImageSaved(string path)
    {
        Debug.Log("Image finished saving!  Saved to: " + path);
        saving = false;
    }

    Texture2D AssembleImage(Texture2D tileTexture, int widthSubdivisions, int heightSubdivisions)
    {
        var image = new Texture2D(tileTexture.width * widthSubdivisions, tileTexture.height * heightSubdivisions);
        var tilePixels = tileTexture.GetPixels();
        for( var ii = 0; ii < widthSubdivisions; ii++ )
        {
            for ( var jj = 0; jj < heightSubdivisions; jj++ )
            {
                image.SetPixels(ii * tileTexture.width, jj * tileTexture.height, tileTexture.width, tileTexture.height, tilePixels);
            }
        }
        return image;
    }


    public void Click_Options()
    {
        Click_NextCamera();

        //optionsMenu.SetActive(!optionsMenu.activeSelf);
    }


}
