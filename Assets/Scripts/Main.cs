using UnityEngine;
using System.Collections;
using System.IO;
using System;


/*
Credits:
<div>Icons made by <a href="http://www.flaticon.com/authors/anton-saputro" title="Anton Saputro">Anton Saputro</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>
<div>Icons made by <a href="http://www.flaticon.com/authors/designmodo" title="Designmodo">Designmodo</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>
http://game-icons.net/delapouite/gui/vertical-flip.html
<div>Icons made by <a href="http://www.freepik.com" title="Freepik">Freepik</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>
<div>Icons made by <a href="http://www.flaticon.com/authors/designerz-base" title="Designerz Base">Designerz Base</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>


<div>Icons made by <a href="http://www.flaticon.com/authors/google" title="Google">Google</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>
<div>Icons made by <a href="http://www.flaticon.com/authors/google" title="Google">Google</a> from <a href="http://www.flaticon.com" title="Flaticon">www.flaticon.com</a>             is licensed by <a href="http://creativecommons.org/licenses/by/3.0/" title="Creative Commons BY 3.0">CC BY 3.0</a></div>


 */
using UnityEngine.UI;


public class Main : MonoBehaviour {

	public PictureDisplay pictureDisplay;

	public int currentCameraIndex = 0;
	public WebCamTexture webcam;


	public Texture renderTexture = new Texture();
	public Texture2D screenTexture;
	public Texture2D pictureTexture;

	public bool rerenderTex2dByFrame = true;
	public bool saving = false;

	public GameObject optionsMenu;
	public Image optionsImage;
	private Color optionsImageBaseColor = new Color(19f/255f, 19f/255f, 19f/255f, 200f/255f);
	private Color optionsImageHighlitColor = new Color(100f/255f, 100f/255f, 100f/255f, 200f/255f);

	// Use this for initialization
	void Start () {
		pictureDisplay = GetComponent<PictureDisplay>();

		var devices = WebCamTexture.devices;
		foreach (var device in devices)
		{
			Debug.Log("devicename:"+device.name);
		}
#if UNITY_EDITOR
		webcam = new WebCamTexture(devices[0].name);
#else
		webcam = new WebCamTexture(devices[0].name);
#endif
		webcam.Play();

		//pictureDisplay.Render(webcam);
		screenTexture = new Texture2D( Screen.width, Screen.height);
		pictureDisplay.Render(screenTexture);
		ScreenshotManager.OnImageSaved += ImageSaved;
	}

	public int widthSubdivisions = 2;
	public int heightSubdivisions = 2;
	public FlipperUtility.FlipMode flipMode = FlipperUtility.FlipMode.Base_Bottom_Left_All_Flip;
	void Update () {
		if ( saving ) {
			return;
		}
		if ( Input.GetKey(KeyCode.Space) ) {
			return;
		}
		if ( Input.GetKeyDown(KeyCode.Alpha1) )
			flipMode = FlipperUtility.FlipMode.Base_Bottom_Left_All_Flip;
		if ( Input.GetKeyDown(KeyCode.Alpha2) )
			flipMode = FlipperUtility.FlipMode.Base_Top_Right_All_Flip;
		if ( Input.GetKeyDown(KeyCode.Alpha3) )
			flipMode = FlipperUtility.FlipMode.Base_Top_Right_Corner_Flip;
		if ( Input.GetKeyDown(KeyCode.Alpha4) )
			flipMode = FlipperUtility.FlipMode.No_Flip;

		if ( Input.GetKeyDown(KeyCode.Minus) && widthSubdivisions > 1 )
			widthSubdivisions--;
		if ( Input.GetKeyDown(KeyCode.Equals) )
			widthSubdivisions++;
		if ( Input.GetKeyDown(KeyCode.Alpha0))
			heightSubdivisions++;
		if ( Input.GetKeyDown(KeyCode.Alpha9) && heightSubdivisions > 1 )
			heightSubdivisions--;


		if ( rerenderTex2dByFrame ) {
//			FlipperUtility.ApplyFlipper(webcam, tex2d);
			//FlipperUtility.FullScreenFlips(webcam, screenTexture, widthSubdivisions, heightSubdivisions, flipMode, out pictureTexture, false);
			FlipperUtility.FullScreenFlips(webcam, screenTexture, widthSubdivisions, heightSubdivisions, flipMode, false);
			if ( Input.GetKeyDown(KeyCode.M) ) {
				//byte[] bytes = pictureTexture.EncodeToPNG();
				Debug.Log("Application Persistant DataPath: "+Application.persistentDataPath);
				string fileUID = "Mozaic"+DateTime.Now.Year+"."+DateTime.Now.Month+"."+DateTime.Now.Day+"_"
					+DateTime.Now.Hour+":"+DateTime.Now.Minute+":"+DateTime.Now.Second+":"+DateTime.Now.Millisecond;
				Debug.Log("FileUID:"+fileUID);
				//File.WriteAllBytes(Application.persistentDataPath+"/TileImage"+DateTime.Now.Year+".png", bytes);
			}
			pictureDisplay.Render(screenTexture);
		}

		if ( Input.GetKeyDown(KeyCode.A) ) 
		{
			Debug.Log("Set to webcam");
			pictureDisplay.Render(webcam);
		}
		if (Input.GetKeyDown(KeyCode.S) )
		{
			Camera_Toggle();
		}
		if ( Input.GetKeyDown(KeyCode.W) )
		{
			rerenderTex2dByFrame = !rerenderTex2dByFrame;
		}
		if ( Input.GetKeyDown(KeyCode.D) )
		{
			Debug.Log("Write to screen sized texture from webcam");
			screenTexture.SetPixels(webcam.GetPixels());
			screenTexture.Apply();
			Debug.Log("Pixels Length:"+webcam.GetPixels().Length );
			pictureDisplay.Render(screenTexture);
		}

	}




	//Take a picture:
	public void Take_Picture ()
	{
		Debug.Log("Click!");
		FlipperUtility.FullScreenFlips_Picture(webcam, screenTexture, widthSubdivisions, heightSubdivisions, flipMode, out pictureTexture, true);
		//FlipperUtility.FullScreenFlips(webcam, screenTexture, widthSubdivisions, heightSubdivisions, flipMode, out pictureTexture, true);
		string fileUID = "Mozaic"+DateTime.Now.Year+"."+DateTime.Now.Month+"."+DateTime.Now.Day+"_"
			+DateTime.Now.Hour+"."+DateTime.Now.Minute+"."+DateTime.Now.Second+"."+DateTime.Now.Millisecond;
		//TODO: Write to file:
		saving = true;
		ScreenshotManager.SaveImage(screenTexture, fileUID, "png");
	}
	void ImageSaved(string path)
	{
//		console.text += "\n" + texture.name + " finished saving to " + path;
		Debug.Log("Image finished saving!  Saved to: "+path);
		saving = false;
	}




	public void Camera_Toggle ()
	{
		currentCameraIndex++;
		if ( currentCameraIndex >= WebCamTexture.devices.Length )
		{
			currentCameraIndex = 0;
		}

		Debug.Log("Now using camera "+currentCameraIndex+", "+WebCamTexture.devices[currentCameraIndex].name);
		webcam = new WebCamTexture(WebCamTexture.devices[currentCameraIndex].name);
		webcam.Play();

		/*
		screenTexture = new Texture2D( Screen.width, Screen.height);
		*/
	}

	//Show/hide button for options:
	public void Options_Toggle ()
	{
		Debug.Log("Options!");
		optionsMenu.SetActive( !optionsMenu.activeSelf );
		if ( optionsMenu.activeSelf ) {
			optionsImage.color = optionsImageHighlitColor;
		} else {
			optionsImage.color = optionsImageBaseColor;
		}
	}


	//Flip mode:
	public void Options_Toggle_FlipMode ()
	{
		switch ( flipMode )
		{
		case FlipperUtility.FlipMode.Base_Bottom_Left_All_Flip:
			flipMode = FlipperUtility.FlipMode.Base_Top_Right_All_Flip;
			break;
		case FlipperUtility.FlipMode.Base_Top_Right_All_Flip:
			flipMode = FlipperUtility.FlipMode.Base_Top_Right_Corner_Flip;
			break;
		case FlipperUtility.FlipMode.Base_Top_Right_Corner_Flip:
			flipMode = FlipperUtility.FlipMode.No_Flip;
			break;
		case FlipperUtility.FlipMode.No_Flip:
			flipMode = FlipperUtility.FlipMode.Base_Bottom_Left_All_Flip;
			break;
		}
	}

	//Subdivisions:
	public void Options_Increment_WidthSubdivisions ()
	{
		if ( widthSubdivisions < screenTexture.width )
			widthSubdivisions++;
	}
	public void Options_Decrement_WidthSubdivisions ()
	{
		if ( widthSubdivisions > 1 )
			widthSubdivisions--;
	}
	public void Options_Increment_HeightSubdivisions ()
	{
		if ( heightSubdivisions < screenTexture.height )
			heightSubdivisions++;
	}
	public void Options_Decrement_HeightSubdivisions ()
	{
		if ( heightSubdivisions > 1 )
			heightSubdivisions--;
	}

	public void Options_Reset ()
	{
		heightSubdivisions = 6;
		widthSubdivisions = 6;
		flipMode = FlipperUtility.FlipMode.Base_Bottom_Left_All_Flip;
	}

}