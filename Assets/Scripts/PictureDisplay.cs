using UnityEngine;
using System.Collections;

public class PictureDisplay : MonoBehaviour {

	public Renderer rend;
	public MeshFilter meshFilter;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		meshFilter = GetComponent<MeshFilter>();

		SizeToCamera();
	}

	public void SizeToCamera (Camera camera = null)
	{
		camera = camera ?? Camera.main;
		//Get the world-bounds of the camera:

		//Set the bounds of the quad:
		var camHeight = camera.orthographicSize*2f;
		var camWidth = camHeight * Screen.width / Screen.height;
		transform.localScale = new Vector3( camWidth, camHeight);
	}

	public void Render ( Texture texture )
	{
		rend.material.mainTexture = texture;
	}
}
