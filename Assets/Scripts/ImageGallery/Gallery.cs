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

        var pageIndex = scrollSnapRect.GetNearestPage();
        var file = imageLoader.files[pageIndex];
        
        ShareUtility.EditImage(file);
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
