using UnityEngine;
using System.Collections;
using System;


/*
Credits:

 */
public class MozaicController : MonoBehaviour {

	public bool saving = false;
	public Mozaic mozaic;

	public int widthSubdivisions = 6;
	public int heightSubdivisions = 6;

	//Options menu:
	public GameObject optionsMenu;

	// Use this for initialization
	void Start () {
		ScreenshotManager.OnImageSaved += ImageSaved;
		mozaic.Initialize( widthSubdivisions, heightSubdivisions );
	}

	public void Update ()
	{
		if ( !saving ) {
			mozaic.DoUpdate();
		}
	}

	//Take a picture:
	public void Take_Picture ()
	{
		Debug.Log("Click!");
        StartCoroutine(SavePicture());
	}
    public IEnumerator SavePicture()
    {
        string fileUID = "Mozaic" + DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day + "_"
            + DateTime.Now.Hour + "." + DateTime.Now.Minute + "." + DateTime.Now.Second + "." + DateTime.Now.Millisecond;
        saving = true;

        //Copy Texture:
        Texture2D mozaicTexture = new Texture2D(mozaic.screenTexture.width, mozaic.screenTexture.height);
        Graphics.CopyTexture(mozaic.screenTexture, mozaicTexture);
        
        //Save Texture:
        byte[] bytes = mozaicTexture.EncodeToPNG();
        string fileExt = ".png";
        string path = Application.persistentDataPath + "/" + fileUID + fileExt;
        yield return ScreenshotManager.Instance.Save(bytes, fileUID, path, ScreenshotManager.ImageType.IMAGE);
        
        //Add to the Gallery:
        
    }
	void ImageSaved(string path)
	{
		//		console.text += "\n" + texture.name + " finished saving to " + path;
		Debug.Log("Image finished saving!  Saved to: "+path);
		saving = false;
	}

	public void ClickOptions ()
	{
		optionsMenu.SetActive(!optionsMenu.activeSelf);
	}

}
