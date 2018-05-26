using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Gallery : MonoBehaviour
{
    public event Action OnGalleryClosed;

    public Canvas parentCanvas;
    public ImageLoader imageLoader;
    public ScrollSnapRect scrollSnapRect;
    public bool deleting = false;

    public int indexMarkedForDelete = -1;
    public FileInfo fileMarkedForDelete = null;
    public Texture2D textureMarkedForDelete = null;

    public IEnumerator Initialize(string imagePath)
    {
        yield return imageLoader.Initialize(imagePath);
        scrollSnapRect.Initialize();
    }

    public void OpenGallery()
    {
        gameObject.SetActive(true);
        //parentCanvas.sortingOrder = 1;
        scrollSnapRect.SetPagePositions();
        if ( imageLoader.files.Count > 0 )
            scrollSnapRect.SetPage(0);
        //Todo: load new images
    }

    public void OnBackButtonClick()
    {
        gameObject.SetActive(false);
        //parentCanvas.sortingOrder = -1;
        if ( OnGalleryClosed != null )
            OnGalleryClosed();
    }


    public void OnShareButtonClick()
    {
        var pageIndex = scrollSnapRect.GetNearestPage();
        var file = imageLoader.files[pageIndex];
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
        if (deleting == true)
        {
            Debug.Log("Still deleting...");
            return;
        }
        deleting = true;
        Debug.Log("Delete!");
        var pageIndex = scrollSnapRect.GetNearestPage();
        StartCoroutine(DoDeleteRoutine(pageIndex));
        //scrollSnapRect.SetPagePositionsAndLerp(pageIndex);
        //scrollSnapRect.SetPagePositions();

    }

    public IEnumerator DoDeleteRoutine(int pageIndex)
    {
        var startTime = Time.time;
        var animTime = 0.5f;
        while (Time.time - startTime < animTime)
        {
            var animTimePercentage = 1f - (Time.time - startTime) / animTime;
            var gameObject = imageLoader.GetImage(pageIndex);
            var image = gameObject.GetComponent<Image>();
            image.color = new Color(1, 1, 1, animTimePercentage);
            yield return new WaitForEndOfFrame(); //TODO: Animate picture deletion (fade + transform)
        }
        var imageToDelete = imageLoader.DeleteImage(pageIndex);
        var textureToDelete = imageLoader.DeleteTexture(pageIndex);
        MarkFileForDeletion(pageIndex, imageToDelete, textureToDelete);
        
        //Enable undo button
        //
        
        
        //scrollSnapRect.BackfillLerp(pageIndex);
        yield return scrollSnapRect.LerpToFillRemovedImage(pageIndex);

        deleting = false;
    }

    public void MarkFileForDeletion(int pageIndex, FileInfo file, Texture2D texture)
    {
        if ( fileMarkedForDelete != null )
        {
            fileMarkedForDelete.Delete();
        }
        indexMarkedForDelete = pageIndex;
        fileMarkedForDelete = file;
        textureMarkedForDelete = texture;
    }

    public void OnUndoButtonClick()
    {
        Debug.Log("Undo!");
        UndoDelete();
    }

    public void UndoDelete()
    {
        if ( fileMarkedForDelete == null )
            return;

        Debug.Log("Valid Undo!");

        //Time to re-add to imageLoader and ScrollSnapRect
        imageLoader.AddImageTo(indexMarkedForDelete, fileMarkedForDelete, textureMarkedForDelete);

        scrollSnapRect.Initialize();
        scrollSnapRect.SetPagePositions();
        scrollSnapRect.SetPage(indexMarkedForDelete);

        //Reset deletion:
        indexMarkedForDelete = -1;
        fileMarkedForDelete = null;
        textureMarkedForDelete = null;
    }

    public void OnOptionsButtonClick()
    {
        Debug.Log("Options!");

    }
}
