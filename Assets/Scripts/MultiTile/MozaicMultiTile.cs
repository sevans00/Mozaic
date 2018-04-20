

/*
 */
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public class MozaicMultiTile : MonoBehaviour {

	//Textures:
	public WebCamTexture webcam;
	public Texture2D tileTexture;
	//Tile Material:
	public Material screenMaterial;

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

	public GameObject singleTilePrefab;

    public bool initialized = false;

    
    // Use this for initialization
    //	void Start () {
    //		Initialize(6,6);
    //	}
    public void ClearMultiTile()
    {
        this.webcam = null;
        while (gameObject.transform.childCount > 0)
        {
            Destroy(gameObject.transform.GetChild(0));
        }
    }
    
	public void Initialize(WebCamTexture webcam, int newWidthSubdivisions, int newHeightSubdivisions, bool useWebcamDimensions = false)
	{
        this.initialized = false;
        this.webcam = webcam;
        DestroyTiles();
        // --- Globals ---
        //Update:
        widthSubdivisions = newWidthSubdivisions;
		heightSubdivisions = newHeightSubdivisions;
        //*
        quarterWidth = Screen.width / (2*widthSubdivisions);
		quarterHeight = Screen.height / (2*heightSubdivisions);
        /*/
        quarterWidth = webcam.width / (2 * widthSubdivisions);
        quarterHeight = webcam.height / (2 * heightSubdivisions);
        //*/
        tileWidth = quarterWidth * 2;
		tileHeight = quarterHeight * 2;
		//AssembleTile:
		tile = new Color[tileWidth*tileHeight];
		tiledColors = new Color[widthSubdivisions * tileWidth * heightSubdivisions * tileHeight];

		//Texture itself:
		tileTexture = new Texture2D( tileWidth, tileHeight);


        //Scale everything:
        var camera = Camera.main;
        var camHeight = camera.orthographicSize * 2f;
        var camWidth = camHeight * Screen.width / Screen.height;
        var childScale = new Vector3(camWidth / widthSubdivisions, camHeight / heightSubdivisions);
        //Sub-tiles:
        bool evenWidth = (widthSubdivisions%2 == 0);
		bool evenHeight = (heightSubdivisions%2 == 0);
		GameObject singleTile;
		float offsetX;
		float offsetY;
		for ( int ii = 0; ii < widthSubdivisions; ii++ ) {
			for ( int jj = 0; jj < heightSubdivisions; jj++ ) {
				singleTile = Instantiate(singleTilePrefab);
				singleTile.transform.parent = gameObject.transform;
				offsetX = 0 - Mathf.Floor(widthSubdivisions/2) + ii + (evenWidth ? 0.5f : 0);
				offsetY = 0 - Mathf.Floor(heightSubdivisions/2) + jj + (evenHeight ? 0.5f : 0);
                //singleTile.transform.localPosition = new Vector3(offsetX, offsetY, 0);
                singleTile.transform.localPosition = new Vector3(offsetX * childScale.x, offsetY * childScale.y, 0);
				singleTile.GetComponent<MeshRenderer>().material.mainTexture = tileTexture;

                singleTile.transform.localScale = childScale;
            }
        }

        //transform.localScale = childScale;
        this.initialized = true;
	}

    public void DestroyTiles()
    {
        for ( int ii = 0; ii < gameObject.transform.childCount; ii++ )
        {
            Destroy(gameObject.transform.GetChild(ii).gameObject);
        }
       
    }

    //public int framesToSkip = 10;
    //public int currentFrame = 0;
    //	public void Update ()
    //	{
    //		DoUpdate();
    //	}

    public void DoUpdate ()
	{
        if ( webcam == null || !webcam.isPlaying )
        {
            //Debug.LogWarning("Webcam null or not playing");
            return;
        }
        
		tileQuarter = webcam.GetPixels(webcam.width/2,webcam.height/2, quarterWidth, quarterHeight);

		AssembleTile_BaseBottomLeft_AllFlip();
		//		screenTexture.SetPixels(0,0,tileWidth, tileHeight, tile);
		tileTexture.SetPixels(tile);
		tileTexture.Apply();

		//TileTexture();
		//screenTexture.SetPixels(tile);
		//screenTexture.Apply()
	}

	//Assemble Tile
	private void AssembleTile_BaseBottomLeft_AllFlip (  )
	{
		forwardX = 0;
		backwardX = 0;
		//for ( int jj = 0; jj < tile.Length; jj++){
		//	tile[jj] = Color.blue;
		//}
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