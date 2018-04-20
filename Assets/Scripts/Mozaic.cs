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

public class Mozaic : MonoBehaviour {
	
	//Textures:
	public WebCamTexture webcam;
	public Texture2D screenTexture;

	//Display:
	public PictureDisplay pictureDisplay;

	// --- Globals ---
	//Update:
	int widthSubdivisions;
	int heightSubdivisions;
	int tileWidth;
	int tileHeight;
	int quarterWidth;
	int quarterHeight;
	Color[] tileQuarter;
	//AssembleTile:
	Color[] tile;
	int forwardX = 0;
	int backwardX = 0;
	//TileTexture:
	Color[] tiledColors;



	// Use this for initialization
//	void Start () {
//		Initialize(6,6);
//	}
	public void Initialize(int newWidthSubdivisions, int newHeightSubdivisions)
	{
		//Webcam:
		var devices = WebCamTexture.devices;
		foreach (var device in devices)
		{
			Debug.Log("devicename:"+device.name);
		}
		#if UNITY_EDITOR
		webcam = new WebCamTexture(devices[1].name);
		#else
		webcam = new WebCamTexture(devices[0].name);
		#endif
		webcam.Play();
		Debug.Log("Webcam dimensions: "+webcam.width+","+webcam.height);


		// --- Globals ---
		//Update:
		widthSubdivisions = newWidthSubdivisions;
		heightSubdivisions = newHeightSubdivisions;
		quarterWidth = Screen.width / (2*widthSubdivisions);
		quarterHeight = Screen.height / (2*heightSubdivisions);
		tileWidth = quarterWidth * 2;
		tileHeight = quarterHeight * 2;
		//AssembleTile:
		tile = new Color[tileWidth*tileHeight];
		tiledColors = new Color[widthSubdivisions * tileWidth * heightSubdivisions * tileHeight];

		//Texture itself:
		screenTexture = new Texture2D( widthSubdivisions * tileWidth, heightSubdivisions * tileHeight);
		pictureDisplay.Render(screenTexture);
	}

	//public int framesToSkip = 10;
	//public int currentFrame = 0;
//	public void Update ()
//	{
//		DoUpdate();
//	}

	public void DoUpdate ()
	{
		tileQuarter = webcam.GetPixels(webcam.width/2,webcam.height/2, quarterWidth, quarterHeight);

		AssembleTile_BaseBottomLeft_AllFlip();
//		screenTexture.SetPixels(0,0,tileWidth, tileHeight, tile);

		TileTexture();
		screenTexture.SetPixels(tiledColors);
		screenTexture.Apply();
	}


	//Assemble Tile
	private void AssembleTile_BaseBottomLeft_AllFlip (  )
	{
		forwardX = 0;
		backwardX = 0;
		for ( int jj = 0; jj < tile.Length; jj++){
			tile[jj] = Color.blue;
		}
		for ( int ii = 0; ii < quarterHeight; ii++)
		{
			forwardX = 0;
			backwardX = quarterWidth - 1;
			//Bottom Left
			while (true)
			{
				tile[forwardX + (ii*tileWidth)] = tileQuarter[forwardX + ii*quarterWidth]; //BL
				tile[forwardX + ((quarterHeight-ii-1)*tileWidth) + (tileWidth * quarterHeight)] = tileQuarter[forwardX + ii*quarterWidth]; //TL
				tile[forwardX + (quarterWidth + ii * tileWidth)] = tileQuarter[backwardX + ii*quarterWidth]; //TR
				tile[forwardX + ((quarterHeight-ii-1)*tileWidth) + (tileWidth * quarterHeight + quarterWidth)] = tileQuarter[backwardX + ii*quarterWidth]; //BR
				forwardX++;
				backwardX--;
				if ( forwardX >= quarterWidth ) {
					break;
				}
			}
		}
	}

	//Tile Texture
	public void TileTexture ()
	{
		for ( int ii = 0; ii < widthSubdivisions; ii++ )
		{
			for ( int jj = 0; jj < heightSubdivisions; jj++ )
			{
				for ( int iiTile = 0; iiTile < tileWidth; iiTile++ )
				{
					for ( int jjTile = 0; jjTile < tileHeight; jjTile++ )
					{
						//         TileX index    Tile x      TileY index                        Tile y
						tiledColors[(ii * tileWidth) + iiTile  + (jj * tileWidth*tileHeight*widthSubdivisions) + jjTile*(tileWidth*widthSubdivisions)] = tile[iiTile+(jjTile*tileWidth)];
					}
				}
			}
		}
	}
}