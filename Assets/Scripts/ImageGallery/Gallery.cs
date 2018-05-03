using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    public event Action OnGalleryClosed;
    
    public ImageLoader imageLoader;
    public ScrollSnapRect scrollSnapRect;

    public IEnumerator Initialize(string imagePath)
    {
        yield return imageLoader.Initialize(imagePath);
        scrollSnapRect.Initialize();
    }

    public void OpenGallery()
    {
        gameObject.SetActive(true);
        imageLoader.ShowImages();

        //Todo: load new images
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        if ( OnGalleryClosed != null )
            OnGalleryClosed();
    }


    public void OnShareButtonClick()
    {
        Debug.Log("Share!");
        var pageIndex = scrollSnapRect.GetNearestPage();
        Debug.Log("Page:"+pageIndex);

        var file = imageLoader.files[pageIndex];
        Debug.Log("File "+file);

        ShareUtility.ShareImage(file);
    }

    public void OnEditButtonClick()
    {
        Debug.Log("Edit!");

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        // Construct the intent.
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent");
        intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Here's the text I want to share.");
        intent.Call<AndroidJavaObject>("setType", "text/plain");

        // Display the chooser.
        AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Share");
        currentActivity.Call("startActivity", chooser);
    }

    public void OnDeleteButtonClick()
    {
        Debug.Log("Delete!");
    }


    public void OnOptionsButtonClick()
    {
        Debug.Log("Options!");
    }
}
