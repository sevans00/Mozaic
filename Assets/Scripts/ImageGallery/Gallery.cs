using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour {

    public event Action OnGalleryClosed;

    public ScrollRect scrollRect;
    public GameObject contentPanel;
    public ImageLoader imageLoader;

    public void Start()
    {
        imageLoader.Initialize();
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        if ( OnGalleryClosed != null )
            OnGalleryClosed();
    }

    public void OpenGallery()
    {
        gameObject.SetActive(true);
        
        
        //Todo: load
    }

}
