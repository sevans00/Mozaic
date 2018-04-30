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
        var page = scrollSnapRect.GetNearestPage();
        Debug.Log("Page:"+page);
    }

    public void OnEditButtonClick()
    {
        Debug.Log("Edit!");
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
