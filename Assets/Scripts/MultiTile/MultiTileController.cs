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
    //public MozaicMultiTile mozaicMultiTile;
    public UIMultiTile uiMultiTile;

    public int widthSubdivisions = 6;
    public int heightSubdivisions = 6;

    public RawImage rawImage;

    //Options menu:
    public OptionsMenu optionsMenu;
    //Overlay
    public GameObject cameraOverlay;
    public Slider slider;
    //Gallery:
    public Gallery gallery;

    public class PlayerPrefsKeys
    {
        public static string OVERLAY_SLIDER = "OVERLAY_SLIDER";
        public static string START_CAMERA_INDEX = "START_CAMERA_INDEX";
    }

    void Start()
    {
        Screen.fullScreen = false;
        StartCoroutine(WaitTillReady());
    }
    
    IEnumerator WaitTillReady()
    {
        Debug.LogWarning("Waiting...");
        //yield return new WaitForSeconds(1f);
        while (WebCamTexture.devices.Length == 0)
        {
            Debug.LogWarning("Webcam null or not playing");
            yield return new WaitForSeconds(1f);
        }
        Debug.LogWarning("Initializing");
        yield return Initialize();
    }

    // Use this for initialization
    public IEnumerator Initialize()
    {
        ScreenshotManager.OnImageSaved += ImageSaved;

        InitializeSettings();

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
        Debug.LogWarning("Webcam rotation: "+webcam.videoRotationAngle);
        //webcam.videoRotationAngle = 0f;
        while (webcam.isPlaying == false)
            yield return new WaitForEndOfFrame();

        //TODO: Turn off loading screen
        
        //mozaicMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);
        uiMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);
        
        rawImage.texture = webcam;
        //rawImage.transform.rotation = Quaternion.AngleAxis(webcam.videoRotationAngle, Vector3.back);

        yield return gallery.Initialize(Application.persistentDataPath + "/");
        yield return new WaitForEndOfFrame();

        gallery.OnGalleryClosed += OnGalleryClosed;
        Debug.Log("Initialized!");
        initialized = true;
    }

    private void InitializeSettings()
    {
        webcamIndex = PlayerPrefs.GetInt(PlayerPrefsKeys.START_CAMERA_INDEX, 0);
        slider.value = PlayerPrefs.GetFloat(PlayerPrefsKeys.OVERLAY_SLIDER, 0);
        cameraOverlay.GetComponent<RawImage>().CrossFadeAlpha(PlayerPrefs.GetFloat(PlayerPrefsKeys.OVERLAY_SLIDER, 0), 0, true);
    }

    public void Update()
    {
        if ( uiMultiTile != null 
            && initialized 
            && uiMultiTile.initialized 
            && !saving)
        {
            //mozaicMultiTile.DoUpdate();
            uiMultiTile.DoUpdate();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gallery.enabled)
            {
                gallery.Close();
            }
            if (optionsMenu.enabled)
            {
                optionsMenu.Close();
            }
        }
    }

    public void OnGalleryButtonClick()
    {
        //Pause camera:
        webcam.Pause();
        gallery.OpenGallery();
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
        PlayerPrefs.SetInt(PlayerPrefsKeys.START_CAMERA_INDEX, webcamIndex);
        webcam = new WebCamTexture(devices[webcamIndex].name);
        webcam.Play();

        //mozaicMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);
        uiMultiTile.Initialize(webcam, widthSubdivisions, heightSubdivisions);

        rawImage.texture = webcam;
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
        var mozaicTexture = AssembleImage(uiMultiTile.tileTexture, widthSubdivisions, heightSubdivisions);
        
        //ScreenshotManager.SaveImage(mozaicTexture, fileUID, "png");
        StartCoroutine(SavePicture(mozaicTexture));
    }
    public IEnumerator SavePicture(Texture2D mozaicTexture)
    {
        string fileUID = "Mozaic" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "_"
            + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
        saving = true;

        var startSaveTime = Time.time;
        var saveDelay = 1.5f;

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
        gallery.scrollSnapRect.SetPagePositions();

        /*
        while (Time.time - startSaveTime < saveDelay)
        {
            yield return new WaitForEndOfFrame();
        }
        */

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
        //Click_NextCamera();
        Debug.LogWarning("Options click");
        optionsMenu.Open();

        //optionsMenu.SetActive(!optionsMenu.activeSelf);

    }

    public void Click_EnableOverlay()
    {
        cameraOverlay.SetActive(cameraOverlay.activeSelf);
    }

    public void OnOverlaySliderChanged()
    {
        Debug.LogWarning("Slider Changed");

        var value = slider.value;

        PlayerPrefs.SetFloat(PlayerPrefsKeys.OVERLAY_SLIDER, value);
        cameraOverlay.GetComponent<RawImage>().CrossFadeAlpha(value, 0, true);
    }
    
    public void Click_SwitchResolution()
    {
        throw new NotImplementedException();
    }

    public void Click_About()
    {
        //Display About

    }

    public void Click_Exit()
    {
        gallery.DeleteMarkedFile();
        Application.Quit();
    }

    public void OnApplicationQuit()
    {
        gallery.DeleteMarkedFile();
    }


}
