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
    [Tooltip("The folder where images will be loaded from")]
    private string imagePath;

    [SerializeField]
    [Tooltip("The panel where new images will be added as children")]
    private RectTransform content;

    private LinkedList<Texture2D> textures;
    private List<FileInfo> files;
    private List<FileInfo> loadedFiles;
    private const int BatchSize = 10;
    private int currentBatch = 0;

    public void Initialize()
    {
        //Application.runInBackground = true;
        textures = new LinkedList<Texture2D>();

        DirectoryInfo di = new DirectoryInfo(imagePath);
        var allFiles = di.GetFiles("*.png");
        files = allFiles.OrderByDescending(x => x.Name).ToList();

        StartCoroutine(LoadImagesBatch(currentBatch));
    }


    public IEnumerator AddImageToFront(string path, string name)
    {
        Action<Texture2D> onTextureLoaded = (Texture2D texture) => { textures.AddFirst(texture); };

        yield return LoadTextureAsync(path, onTextureLoaded);
        var imageObject = CreateImage(textures.First(), name);
        imageObject.transform.SetAsFirstSibling();
    }
    public void AddImageToFront(Texture2D texture, string name)
    {
        textures.AddFirst(texture);
        var imageObject = CreateImage(textures.First(), name);
        imageObject.transform.SetAsFirstSibling();
    }

    public IEnumerator LoadImagesBatch(int batch)
    {
        foreach (var file in files)
        {
            Debug.Log(file.FullName);
            yield return LoadTextureAsync(file.FullName, AddLoadedTextureToCollection);
            CreateImage(textures.Last(), file.Name);
        }
    }
    
    private void AddLoadedTextureToCollection(Texture2D texture)
    {
        textures.AddLast(texture);
    }
    private GameObject CreateImage(Texture2D texture, string name)
    {
        GameObject imageObject = new GameObject(name);
        imageObject.transform.SetParent(content);
        imageObject.AddComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
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

        result(loadedTexture);
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