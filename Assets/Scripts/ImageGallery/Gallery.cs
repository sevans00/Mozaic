using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour {

    public event Action OnGalleryClosed;

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        OnGalleryClosed();
    }

    public void OpenGallery()
    {
        gameObject.SetActive(true);
        ScrollRect rect;
        
        //Todo: load
    }

}
