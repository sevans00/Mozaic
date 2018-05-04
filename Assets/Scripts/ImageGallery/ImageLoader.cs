using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    [SerializeField]
    private string imageStoragePath;

    [SerializeField]
    [Tooltip("The panel where new images will be added as children")]
    private RectTransform content;

    public LinkedList<Texture2D> textures;
    public List<FileInfo> files;
    public List<FileInfo> loadedFiles;
    
    private const int BatchSize = 10;
    private int currentBatch = 0;

    public IEnumerator Initialize(string imagePath)
    {
        imageStoragePath = imagePath;
        
        Application.runInBackground = true;
        textures = new LinkedList<Texture2D>();

        DirectoryInfo di = new DirectoryInfo(imageStoragePath);
        var allFiles = di.GetFiles("*.png");
        files = allFiles.OrderByDescending(x => x.Name).ToList();

        yield return LoadImagesBatch(currentBatch);
    }


    public IEnumerator AddImageToFront(string path, string name)
    {
        //When loaded, add it to the front:
        yield return LoadTextureAsync(path, (Texture2D texture) => { textures.AddFirst(texture); });

        //TODO: Record filename to tell that we have created the images already

        var imageObject = CreateImage(textures.First(), name);
        imageObject.transform.SetAsFirstSibling();
    }

    public IEnumerator LoadImagesBatch(int batch)
    {
        foreach (var file in files)
        {
            Debug.Log("LoadImage: "+file.FullName);
            yield return LoadTextureAsync(file.FullName, (Texture2D texture) => { textures.AddLast(texture); });
            CreateImage(textures.Last(), file.Name);
        }
    }
    
    private GameObject CreateImage(Texture2D texture, string name)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(content);
        
        var image = imageObject.AddComponent<Image>();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        var rectTransform = image.rectTransform;
        
        if ( texture.width > texture.height )
        {
            var desiredWidth = Screen.width * 0.9f;
            float aspectRatio = (float)texture.height / (float)texture.width;
            image.rectTransform.sizeDelta = new Vector2(desiredWidth, desiredWidth * aspectRatio);
        }
        if ( texture.height >= texture.width )
        {
            var desiredHeight = Screen.height * 0.9f;
            float aspectRatio = (float)texture.width / (float)texture.height;
            image.rectTransform.sizeDelta = new Vector2(desiredHeight * aspectRatio, desiredHeight);
        }
        
        return imageObject;
    }
    
    public IEnumerator LoadTextureAsync(string originalFileName, Action<Texture2D> result)
    {
        string fileToLoad = GetCleanFileName(originalFileName);

        Debug.Log("Loading Image from path: " + fileToLoad);

        WWW www = new WWW(fileToLoad);
        yield return www;

        Texture2D loadedTexture = new Texture2D(1, 1);

        www.LoadImageIntoTexture(loadedTexture);

        if (result != null)
        {
            result(loadedTexture);
        }
    }

    private static string GetCleanFileName(string originalFileName)
    {
        string fileToLoad = originalFileName.Replace('\\', '/');

        if (fileToLoad.StartsWith("http") == false)
        {
            fileToLoad = string.Format("file://{0}", fileToLoad);
        }

        return fileToLoad;
    }

}