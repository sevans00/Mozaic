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

    private List<Texture2D> textures;
    private List<FileInfo> files;
    private List<FileInfo> loadedFiles;
    private const int BatchSize = 10;
    private int currentBatch = 0;

    private void Start()
    {
        //Application.runInBackground = true;
        textures = new List<Texture2D>();

        DirectoryInfo di = new DirectoryInfo(imagePath);
        var allFiles = di.GetFiles("*.png");
        files = allFiles.OrderByDescending(x => x.Name).ToList();

        StartCoroutine(LoadImagesBatch(currentBatch));
    }

    public IEnumerator LoadImageIntoFront(string path)
    {
        yield return LoadTextureAsync(path, AddLoadedTextureToFrontOfCollection);
        CreateImage(textures.First());
    }

    public IEnumerator LoadImagesBatch(int batch)
    {

        foreach (var file in files)
        {
            Debug.Log(file.FullName);
            yield return LoadTextureAsync(file.FullName, AddLoadedTextureToCollection);
            CreateImage(textures.Last());
        }
    }
    
    public IEnumerator LoadImages()
    {
        textures = new List<Texture2D>();

        DirectoryInfo di = new DirectoryInfo(imagePath);
        var allFiles = di.GetFiles("*.png");
        var files = allFiles.OrderByDescending(x => x.Name);

        foreach (var file in files)
        {
            Debug.Log(file.FullName);
            yield return LoadTextureAsync(file.FullName, AddLoadedTextureToCollection);
        }

        CreateImages();
    }

    public IEnumerator LoadImagesSequential()
    {
        textures = new List<Texture2D>();

        DirectoryInfo di = new DirectoryInfo(imagePath);
        var allFiles = di.GetFiles("*.png");
        var files = allFiles.OrderByDescending(x => x.Name);

        var startTime = DateTime.Now;
        foreach (var file in files)
        {
            Debug.Log(file.FullName);
            yield return LoadTextureAsync(file.FullName, AddLoadedTextureToCollection);
            CreateImage(textures.Last());
        }
        var endTime = DateTime.Now;
        Debug.Log("Load texture async time: "+(startTime - endTime));
    }
    
    private void AddLoadedTextureToCollection(Texture2D texture)
    {
        textures.Add(texture);
    }

    private void CreateImage(Texture2D texture)
    {
        GameObject imageObject = new GameObject("Image");
        imageObject.transform.SetParent(content);
        imageObject.AddComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        Debug.Log("Sprite: " + texture.width + ", " + texture.height);
        //imageObject.GetComponent<Image>().minHeight = texture.height;
    }

    private void CreateImages()
    {
        foreach (var texture in textures)
        {
            CreateImage(texture);
        }
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