using UnityEngine;
using System.Collections;
using System.Linq;
using System;

public class FlipperUtility {


	public static void ApplyFlipper ( WebCamTexture from, Texture2D to )
	{
		//NoFlipper(from, to);
//		Oneflip(from, to);
//		Allflips(from, to);
//		FullScreenFlips(from, to);
	}

	public static void FullScreenFlips_Picture ( WebCamTexture from, Texture2D to, int widthSubdivisions, int heightSubdivisions, FlipMode flipMode, out Texture2D pictureTexture, bool outputPictureTexture )
	{
		if ( !from.isPlaying ) {
			Debug.LogWarning("Webcam not playing");
			pictureTexture = null;
			return;
		}
		if ( widthSubdivisions <= 0 ) {
			widthSubdivisions = 1; //Todo: this is invalid input
		}
		if ( heightSubdivisions <= 0 ) {
			heightSubdivisions = 1; //Todo: this is invalid input
		}

		//FillTexture ( to, Color.black ); //Commented out to try saving on expense
		int tileSectionWidth = to.width / widthSubdivisions / 2;
		int tileSectionHeight = to.height / heightSubdivisions / 2;
		//Picture rect:
		int x = (int)((from.width - tileSectionWidth) / 2);
		int y = (int)((from.height - tileSectionHeight) / 2);

		//Tile:
		var tile = AssembleTile ( from, x, y, tileSectionWidth, tileSectionHeight, flipMode );
		int tileWidth = tileSectionWidth * 2;
		int tileHeight = tileSectionHeight * 2;

		int xTiles = to.width / tileWidth;
		int yTiles = to.height / tileHeight;

		pictureTexture = new Texture2D( xTiles * tileWidth, yTiles * tileHeight );
		Color[] colors = TileTextureToFast( tile, xTiles, yTiles, tileWidth, tileHeight );
		pictureTexture.SetPixels(colors);
		to.Apply();
	}

	public static void FullScreenFlips ( WebCamTexture from, Texture2D to, int widthSubdivisions, int heightSubdivisions, FlipMode flipMode, bool outputPictureTexture )
	{
		if ( !from.isPlaying ) {
			Debug.LogWarning("Webcam not playing");
			return;
		}
		if ( widthSubdivisions <= 0 ) {
			widthSubdivisions = 1; //Todo: this is invalid input
		}
		if ( heightSubdivisions <= 0 ) {
			heightSubdivisions = 1; //Todo: this is invalid input
		}

		//FillTexture ( to, Color.black ); //Commented out to try saving on expense
		int tileSectionWidth = to.width / widthSubdivisions / 2;
		int tileSectionHeight = to.height / heightSubdivisions / 2;
		//Picture rect:
		int x = (int)((from.width - tileSectionWidth) / 2);
		int y = (int)((from.height - tileSectionHeight) / 2);

		//Tile:
		var tile = AssembleTile ( from, x, y, tileSectionWidth, tileSectionHeight, flipMode );
		int tileWidth = tileSectionWidth * 2;
		int tileHeight = tileSectionHeight * 2;

		int xTiles = to.width / tileWidth;
		int yTiles = to.height / tileHeight;
//		Debug.Log("xTiles = "+to.width+"/"+tileWidth+"="+(to.width/tileWidth)+" so "+xTiles+" remainder:"+to.width%tileWidth);
		int xOffset = (to.width%tileWidth) / 2;
		int yOffset = (to.height%tileHeight) / 2;

		/*
		//Possibly faster setpixels:
		Color[] colors = new Color[to.width * to.height];
		Color[] tileRow = new Color[tileWidth];
		for ( int tileY = 0; tileY < tileHeight; tileY++ )
		{
			//tile.CopyTo(tileRow, tileY);
			Array.Copy(tile, tileY, tileRow, 0, tileWidth);
			for ( int ii = 0; ii < xTiles; ii++ )
			{
				for ( int jj = 0; jj < yTiles; jj++ )
				{
					//to.SetPixels(xOffset + ii*tileWidth,yOffset + jj*tileHeight,tileWidth,tileHeight, tile);
					//tileRow.CopyTo(colors, (ii*tileWidth)+(jj*tileHeight));
					Array.Copy(tileRow, 0, colors, (ii*tileWidth)+(jj*tileHeight), tileWidth);
				}
			}
		}
		to.SetPixels(xOffset, yOffset, xTiles * tileWidth, yTiles * tileHeight, colors);
		pictureTexture = null;
		
		/*
		//Slower setpixels:
		for ( int ii = 0; ii < xTiles; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				to.SetPixels(xOffset + ii*tileWidth,yOffset + jj*tileHeight,tileWidth,tileHeight, tile);
			}
		}
		pictureTexture = null;
		/*/
		//Function-ifying setpixels:
		//pictureTexture = new Texture2D( xTiles * tileWidth, yTiles * tileHeight );
//		Color[] colors = TileTextureToFast( tile, xTiles, yTiles, tileWidth, tileHeight );
		Color[] colors = TileTextureToSlowToFast( tile, xTiles, yTiles, tileWidth, tileHeight );
		to.SetPixels( xOffset, yOffset, xTiles*tileWidth, yTiles*tileHeight, colors );
		
//		TileTextureTo( tile, to, xTiles, yTiles, tileWidth, tileHeight );
//		Color[] colors = to.GetPixels();
//		to.SetPixels( xOffset, yOffset, xTiles*tileWidth, yTiles*tileHeight, colors );
		
		//*/
		to.Apply();
	}

	public static void TileTextureTo ( Color[] tile, Texture2D to, int xTiles, int yTiles, int tileWidth, int tileHeight )
	{
		for ( int ii = 0; ii < xTiles; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				to.SetPixels( ii*tileWidth, jj*tileHeight, tileWidth, tileHeight, tile );
			}
		}
	}

	//Slow but right TileTextureTo
	public static Color[] TileTextureToSlow ( Color[] tile, int xTiles, int yTiles, int tileWidth, int tileHeight )
	{
		Texture2D to = new Texture2D( xTiles*tileWidth, yTiles*tileHeight );
		for ( int ii = 0; ii < xTiles; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				to.SetPixels( ii*tileWidth, jj*tileHeight, tileWidth, tileHeight, tile );
			}
		}
		return to.GetPixels();
	}
	//Starting slow and speeding up while actually working (attempting to get to TileTextureToFast)
	public static Color[] TileTextureToSlowToFast ( Color[] tile, int xTiles, int yTiles, int tileWidth, int tileHeight )
	{
		Color[] colors = new Color[xTiles * tileWidth * yTiles * tileHeight];
		for ( int ii = 0; ii < xTiles; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				for ( int iiTile = 0; iiTile < tileWidth; iiTile++ )
				{
					for ( int jjTile = 0; jjTile < tileHeight; jjTile++ )
					{
						//         TileX index    Tile x      TileY index                        Tile y
						colors[(ii * tileWidth) + iiTile  + (jj * tileWidth*tileHeight*xTiles) + jjTile*(tileWidth*xTiles)] = tile[iiTile+(jjTile*tileWidth)];
					}
				}
			}
		}
		return colors;
	}
	//Fast version of TileTextureTo
	public static Color[] TileTextureToFast ( Color[] tile, int xTiles, int yTiles, int tileWidth, int tileHeight )
	{
		Color[] colors = new Color[xTiles * tileWidth * yTiles * tileHeight];
		//Color[] tileRow = new Color[tileWidth];

		int colorsIndex = 0;
		int tileRowIndex = 0;
		for ( int pixelY = 0; pixelY < yTiles * tileHeight; pixelY++ )
		{
			for ( int tileX = 0; tileX < xTiles; tileX++ )
			{
				Array.Copy(tile, tileRowIndex, colors, colorsIndex, tileWidth);
				colorsIndex += tileWidth;
			}
			tileRowIndex += tileWidth;
			if ( tileRowIndex >= tile.Length )
				tileRowIndex = 0;
		}
		return colors;
	}

	private static Color[] fill;
	public static void FillTexture ( Texture2D to, Color fillColor )
	{
		if ( fill.Length == 0 ) {
			fill = Enumerable.Repeat<Color>(fillColor, to.width*to.height).ToArray();
		}
		to.SetPixels(fill);
	}

	public Texture2D GenerateMosaic(WebCamTexture from, int width, int height, int widthSubdivisions, int heightSubdivisions)
	{
		Texture2D to = new Texture2D(width, height);

		int tileSectionWidth = to.width / widthSubdivisions / 2;
		int tileSectionHeight = to.height / heightSubdivisions / 2;
		//Picture rect:
		int x = (int)((from.width - tileSectionWidth) / 2);
		int y = (int)((from.height - tileSectionHeight) / 2);
		
		//Tile:
		var tile = AssembleTile ( from, x, y, tileSectionWidth, tileSectionHeight );
		int tileWidth = tileSectionWidth * 2;
		int tileHeight = tileSectionHeight * 2;

		int xTiles = to.width / tileWidth;
		int yTiles = to.height / tileHeight;
		for ( int ii = 0; ii < xTiles; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				to.SetPixels(ii*tileWidth,jj*tileHeight,tileWidth,tileHeight, tile);
			}
		}
		return to;
	}

	public static void Allflips ( WebCamTexture from, Texture2D to)
	{
		int tileSectionWidth = 100;
		int tileSectionHeight = 50;
		//Picture rect:
		int x = (int)((from.width - tileSectionWidth) / 2);
		int y = (int)((from.height - tileSectionHeight) / 2);
		
		var tile = AssembleTile ( from, x, y, tileSectionWidth, tileSectionHeight );
		int tileWidth = tileSectionWidth * 2;
		int tileHeight = tileSectionHeight * 2;
		
		int xTiles = to.width / tileWidth;
		int yTiles = to.height / tileHeight;
		Debug.Log("xTiles = "+to.width+"/"+tileWidth+"="+(to.width/tileWidth)+" so "+xTiles);
		for ( int ii = 0; ii < xTiles+1; ii++ )
		{
			for ( int jj = 0; jj < yTiles; jj++ )
			{
				to.SetPixels(ii*tileWidth,jj*tileHeight,tileWidth,tileHeight, tile);
			}
		}
		
		//		to.SetPixels(0,0,tileWidth*2,tileHeight*2, tile);
		//		to.SetPixels(tileWidth*2,0,tileWidth*2,tileHeight*2, tile);
		//		to.SetPixels(0,tileHeight*2,tileWidth*2,tileHeight*2, tile);
		//		to.SetPixels(tileWidth*2,tileHeight*2,tileWidth*2,tileHeight*2, tile);
		
		to.Apply();
	}

	public static void Oneflip ( WebCamTexture from, Texture2D to )
	{
		int tileWidth = 100;
		int tileHeight = 50;
		//Picture rect:
		int x = (int)((from.width - tileWidth) / 2);
		int y = (int)((from.height - tileHeight) / 2);

		var tile = AssembleTile ( from, x, y, tileWidth, tileHeight );
		//Tile will be 2x width and height

		to.SetPixels(x, y, tileWidth*2, tileHeight*2, tile);
		to.Apply();
	}

	public enum FlipMode
	{
		Base_Bottom_Left_All_Flip = 0,
		Base_Top_Right_Corner_Flip = 1,
		Base_Top_Right_All_Flip = 2,
		No_Flip = 4
	}

	//Tile will be 2x width and height
	public static Color[] AssembleTile ( WebCamTexture from, int x, int y, int tileWidth, int tileHeight, FlipMode flipMode = FlipMode.Base_Bottom_Left_All_Flip )
	{
		switch ( flipMode ) {
		case FlipMode.Base_Bottom_Left_All_Flip:
			return AssembleTile_BaseBottomLeft_AllFlip(from, x, y, tileWidth, tileHeight);
		case FlipMode.Base_Top_Right_All_Flip:
			return AssembleTile_BaseTopRight_AllFlip(from, x, y, tileWidth, tileHeight);
		case FlipMode.Base_Top_Right_Corner_Flip:
			return AssembleTile_BaseBottomLeft_CornerFlip(from, x, y, tileWidth, tileHeight);
		}
		return AssembleTile_NoFlip(from, x, y, tileWidth, tileHeight);
		//Pixels from rect:
		//var pixels = from.GetPixels( x, y, tileWidth, tileHeight);
		//TODO: Convert to a few matrix operations
		/*
		Color[] topLeft = pixels;
		Color[] topRight = new Color[pixels.Length];
		Color[] bottomLeft = new Color[pixels.Length];
		Color[] bottomRight = new Color[pixels.Length];
		for ( int ii = 0; ii < tileHeight; ii++)
		{
			var row = pixels.ToList().GetRange(ii*tileWidth, tileWidth);

			//Bottom left:
			row.CopyTo(bottomLeft, (tileHeight-ii-1)*tileWidth);

			//Top right:
			row.Reverse(); //Flip horizontally
			row.CopyTo(topRight, ii*tileWidth);

			//Bottom right:
			row.CopyTo(bottomRight, (tileHeight-ii-1)*tileWidth);
		}
		return topLeft;
		*/

	}


	private static Color[] AssembleTile_BaseBottomLeft_AllFlip_CORRECT ( Color[] pixels, int quarterWidth, int quarterHeight )
	{
		int tileWidth = quarterWidth * 2;
		int tileHeight = quarterHeight * 2;
		Color[] tile = new Color[tileWidth*tileHeight];

		int forwardX = 0;
		int backwardX = 0;
		for ( int ii = 0; ii < quarterHeight; ii++)
		{
			forwardX = 0;
			backwardX = quarterWidth - 1;
			//Bottom Left
			while (true)
			{
				tile[forwardX + (ii*tileWidth)] = pixels[forwardX + ii*quarterWidth]; //BL
				tile[forwardX + ((quarterHeight-ii-1)*tileWidth) + (tileWidth * quarterHeight)] = pixels[forwardX + ii*quarterWidth]; //TL
				tile[forwardX + (quarterWidth + ii * tileWidth)] = pixels[backwardX + ii*quarterWidth]; //TR
				//				tile[forwardX + tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2)] = pixels[backwardX + ii*quarterWidth];
				tile[forwardX + ((quarterHeight-ii-1)*tileWidth) + (tileWidth * quarterHeight + quarterWidth)] = pixels[backwardX + ii*quarterWidth]; //BR

				forwardX++;
				backwardX--;
				if ( forwardX >= quarterWidth ) {
					break;
				}
			}
		}
		return tile;
	}



	//Top Right is base image and everything flipped from there:
	private static Color[] AssembleTile_BaseTopRight_AllFlip ( WebCamTexture from, int x, int y, int tileWidth, int tileHeight )
	{
		//Pixels from rect:
		var pixels = from.GetPixels( x, y, tileWidth, tileHeight);
		Color[] tile = new Color[pixels.Length*4];
		for ( int ii = 0; ii < tileHeight; ii++)
		{
			var row = pixels.ToList().GetRange(ii*tileWidth, tileWidth).ToArray();
			//Bottom left://Now Top right:
			row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
			//Top left://Now Bottom right:
			row.CopyTo(tile, (tileWidth) + (ii*tileWidth*2));
			//Bottom right://Now Top Left:
			row.Reverse(); //Flip horizontally
			row.CopyTo(tile, (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2);
			//Top right: //Now bottom left:
			row.CopyTo(tile, (ii*tileWidth*2));//row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
		}
		return tile;
	}
	//Swapping Bottom Left with Top Right:
	private static Color[] AssembleTile_BaseBottomLeft_CornerFlip ( WebCamTexture from, int x, int y, int tileWidth, int tileHeight )
	{
		//Pixels from rect:
		var pixels = from.GetPixels( x, y, tileWidth, tileHeight);
		Color[] tile = new Color[pixels.Length*4];
		for ( int ii = 0; ii < tileHeight; ii++)
		{
			var row = pixels.ToList().GetRange(ii*tileWidth, tileWidth).ToArray();
			//Bottom left://Now Top right:
			row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
			//Top left:
			row.CopyTo(tile, (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2);
			//Bottom right:
			row.Reverse(); //Flip horizontally
			row.CopyTo(tile, (tileWidth) + (ii*tileWidth*2));
			//Top right: //Now bottom left:
			row.CopyTo(tile, (ii*tileWidth*2));//row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
		}
		return tile;
	}
	//Default - Bottom left is the base image, and all images are flipped from there
	private static Color[] AssembleTile_BaseBottomLeft_AllFlip ( WebCamTexture from, int x, int y, int tileWidth, int tileHeight )
	{
//		return AssembleTile_NoFlip(from, x, y, tileWidth, tileHeight);
		//*
		//Pixels from rect:
		var pixels = from.GetPixels( x, y, tileWidth*2, tileHeight*2);
		Color[] tile = new Color[pixels.Length * 4];
		//Faster version:
		Color[] row = new Color[tileWidth]; //
		//
		int forwardX = 0;
		int backwardX = 0;
		//
		for ( int jj = 0; jj < tile.Length; jj++ ) {
			tile[jj] = Color.blue;
		}

		for ( int ii = 0; ii < tileHeight; ii++)
		{
			Array.Copy(pixels, ii*tileWidth, row, 0, tileWidth);
			//Bottom left:
//			row.CopyTo(tile, (ii*tileWidth*2));
			//Top left:
			row.CopyTo(tile, (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2);

			forwardX = 0;
			backwardX = tileWidth - 1;
			//Bottom Left
			while (true)
			{

//				tile[forwardX + (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2] = Color.gray;
//				tile[backwardX + (tileWidth) + (ii*tileWidth*2)] = Color.gray;
//				tile[backwardX + tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2)] = Color.gray;


//				tile[forwardX + (ii*tileWidth)] = pixels[forwardX + ii*tileWidth];


//				tile[forwardX + (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2] = pixels[forwardX + ii*tileWidth];
//				tile[forwardX + (tileWidth) + (ii*tileWidth*2)] = pixels[backwardX + ii*tileWidth];
//				tile[forwardX + tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2)] = pixels[backwardX + ii*tileWidth];

//				tile[forwardX + (ii*tileWidth*2)] = Color.yellow;
//				tile[forwardX + (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2] = Color.red;
//				tile[backwardX + (tileWidth) + (ii*tileWidth*2)] = Color.magenta;
//				tile[backwardX + tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2)] = Color.green;

				forwardX++;
				backwardX--;
				if ( forwardX >= tileWidth ) {
					break;
				}
			}
			//OOOOOOOOOOOOOOOOOOOOOOPS - The XX in Tile's assignment need to be REVERSED!  So what we should do is combine all four into one loop
			//Bottom Right
//			for ( int xx = tileWidth - 1; xx >= 0; xx-- )
//			{
//				tile[xx + (tileWidth) + (ii*tileWidth*2)] = pixels[xx + ii*tileWidth];
//			}
//			//Top Right
//			for ( int xx = tileWidth - 1; xx >= 0; xx-- )
//			{
//				tile[xx + tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2)] = pixels[xx + ii*tileWidth];
//			}
//			//Top right:
//			row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
		}
		/*/
		//Slower version:
		var pixels = from.GetPixels( x, y, tileWidth, tileHeight);
		Color[] tile = new Color[pixels.Length*4];
		Color[] row = new Color[tileWidth];
		for ( int ii = 0; ii < tileHeight; ii++)
		{
			Array.Copy(pixels, ii*tileWidth, row, 0, tileWidth);
			//Bottom left:
			row.CopyTo(tile, (ii*tileWidth*2));
			//Top left:
			row.CopyTo(tile, (tileWidth * 2 * tileHeight ) + (tileHeight-ii-1)*tileWidth * 2);
			//Bottom right:
			System.Array.Reverse(row); //Flip horizontally
			row.CopyTo(tile, (tileWidth) + (ii*tileWidth*2));
			//Top right:
			row.CopyTo(tile, tileWidth + (tileWidth * 2 * tileHeight ) + ((tileHeight-ii-1) * tileWidth * 2));
		}
		//*/
		return tile;
	}
	private static Color[] AssembleTile_NoFlip ( WebCamTexture from, int x, int y, int tileWidth, int tileHeight )
	{
		var pixels = from.GetPixels( x, y, tileWidth * 2, tileHeight * 2);
		Color[] tile = new Color[pixels.Length * 4];
		pixels.CopyTo(tile, 0);
		return tile;
	}


	public static void NoFlipper (WebCamTexture from, Texture2D to)
	{
		to.SetPixels(from.GetPixels());
		to.Apply();
	}
}
