using UnityEngine;
using UnityEngine.UI;

public class UIMultiTile : MonoBehaviour
{
    //Textures:
    public WebCamTexture webcam;
    public Texture2D tileTexture;
    public Sprite sprite;
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
    
    public GameObject singleTilePrefab;

    public GridLayoutGroup gridLayoutGroup;
    public CanvasScaler canvasScaler;

    public bool initialized = false;
    
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
        var referenceResolution = canvasScaler.referenceResolution;
        var referenceWidth = referenceResolution.x / widthSubdivisions;
        var referenceHeight = referenceResolution.y / heightSubdivisions;
        /*/
        var referenceResolution = Screen.currentResolution;
        var referenceWidth = referenceResolution.width / widthSubdivisions;
        var referenceHeight = referenceResolution.height / heightSubdivisions;
        //*/
        gridLayoutGroup.cellSize = new Vector2(referenceWidth, referenceHeight);
        
        //Now, figure out where the center bottom left quarter of the camera is
        //plus the proportional size of the camera relative to the canvas
        //Assume that the camera and canvas size match, so we don't need to worry about canvas ratios
        var widthRatio = webcam.width / referenceWidth;
        var heightRatio = webcam.height / referenceHeight;

        Debug.Log("Ratios: "+widthRatio+" "+heightRatio);

        var camTileWidth = Mathf.FloorToInt(webcam.width / widthSubdivisions);
        var camTileHeight = Mathf.FloorToInt(webcam.height / heightSubdivisions);
        quarterWidth = camTileWidth / 2;
        quarterHeight = camTileHeight / 2;

        tileWidth = camTileWidth;
        tileHeight = camTileHeight;

        //AssembleTile:
        tile = new Color[tileWidth * tileHeight];
        tileTexture = new Texture2D(tileWidth, tileHeight);
        sprite = Sprite.Create(tileTexture, new Rect(Vector2.zero, new Vector2(tileWidth, tileHeight)), new Vector2(0.5f, 0.5f));

        GameObject singleTile;
        for (int ii = 0; ii < widthSubdivisions; ii++)
        {
            for (int jj = 0; jj < heightSubdivisions; jj++)
            {
                singleTile = Instantiate(singleTilePrefab);
                singleTile.transform.SetParent(gameObject.transform);
                singleTile.transform.localScale = Vector3.one;
                //singleTile.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);
                singleTile.GetComponent<Image>().sprite = sprite;
            }
        }
        
        this.initialized = true;
    }

    public void DestroyTiles()
    {
        for (int ii = 0; ii < gameObject.transform.childCount; ii++)
        {
            Destroy(gameObject.transform.GetChild(ii).gameObject);
        }

    }
    
    public void DoUpdate()
    {
        if (webcam == null || !webcam.isPlaying)
        {
            //Debug.LogWarning("Webcam null or not playing");
            return;
        }

        tileQuarter = webcam.GetPixels(webcam.width / 2, webcam.height / 2, quarterWidth, quarterHeight);

        AssembleTile_BaseBottomLeft_AllFlip();
        tileTexture.SetPixels(tile);
        tileTexture.Apply();

        //TileTexture();
        //screenTexture.SetPixels(tile);
        //screenTexture.Apply()
    }

    //Assemble Tile
    private void AssembleTile_BaseBottomLeft_AllFlip()
    {
        forwardX = 0;
        backwardX = 0;
        /*
        for ( int jj = 0; jj < tile.Length; jj++) {
            tile[jj] = new Color(0, 0, 1, 1); //Color.blue;
        }
        /*/
        for (int ii = 0; ii < quarterHeight; ii++)
        {
            forwardX = 0;
            backwardX = quarterWidth - 1;
            //Bottom Left
            while (true)
            {
                tile[forwardX + (ii * tileWidth)] = tileQuarter[forwardX + ii * quarterWidth]; //BL
                tile[forwardX + ((quarterHeight - ii - 1) * tileWidth) + (tileWidth * quarterHeight)] = tileQuarter[forwardX + ii * quarterWidth]; //TL
                tile[forwardX + (quarterWidth + ii * tileWidth)] = tileQuarter[backwardX + ii * quarterWidth]; //TR
                tile[forwardX + ((quarterHeight - ii - 1) * tileWidth) + (tileWidth * quarterHeight + quarterWidth)] = tileQuarter[backwardX + ii * quarterWidth]; //BR
                forwardX++;
                backwardX--;
                if (forwardX >= quarterWidth)
                {
                    break;
                }
            }
        }
        //*/
    }
    
}