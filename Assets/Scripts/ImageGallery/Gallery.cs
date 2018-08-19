using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

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

    public UndoButton undoButton;
    public Coroutine fadeoutCoroutine = null;

    public IEnumerator Initialize(string imagePath)
    {
        yield return imageLoader.Initialize(imagePath);
        scrollSnapRect.Initialize();

    }

    public void OpenGallery()
    {
        gameObject.SetActive(true);
        scrollSnapRect.SetPagePositions();
        if ( imageLoader.files.Count > 0 )
            scrollSnapRect.SetPage(0);
        //Todo: load new images

        SetAlphas(1f);
        undoButton.gameObject.SetActive(false);
    }

    public void Close()
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
        pageIndex = Mathf.Clamp(pageIndex, 0, scrollSnapRect.pageCount - 1);
        StartCoroutine(DoDeleteRoutine(pageIndex));
        //scrollSnapRect.SetPagePositionsAndLerp(pageIndex);
        //scrollSnapRect.SetPagePositions();

        SetAlphas(1f);
        undoButton.gameObject.SetActive(true);
        
        if ( fadeoutCoroutine != null )
            StopCoroutine(fadeoutCoroutine);
        fadeoutCoroutine = StartCoroutine(DelayFadeout());
    }

    public IEnumerator DoDeleteRoutine(int pageIndex)
    {
        var startTime = Time.time;
        var animTime = 0.5f;
        var gameObject = imageLoader.GetImage(pageIndex);
        var image = gameObject.GetComponent<Image>();
        while (Time.time - startTime < animTime)
        {
            var animTimePercentage = 1f - (Time.time - startTime) / animTime;
            image.color = new Color(1, 1, 1, animTimePercentage);
            yield return new WaitForEndOfFrame(); //TODO: Animate picture deletion (fade + transform)
        }
        var imageToDelete = imageLoader.DeleteImage(pageIndex);
        var textureToDelete = imageLoader.DeleteTexture(pageIndex);
        MarkFileForDeletion(pageIndex, imageToDelete, textureToDelete);

        //Enable undo button
        SetAlphas(1f);
        undoButton.gameObject.SetActive(true);
        
        yield return scrollSnapRect.LerpToFillRemovedImage(pageIndex);

        deleting = false;
    }

    private void MarkFileForDeletion(int pageIndex, FileInfo file, Texture2D texture)
    {
        DeleteMarkedFile();
        indexMarkedForDelete = pageIndex;
        fileMarkedForDelete = file;
        textureMarkedForDelete = texture;
    }

    public void DeleteMarkedFile()
    {
        if (fileMarkedForDelete != null)
        {
            fileMarkedForDelete.Delete();
            fileMarkedForDelete = null;
        }
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

        SetAlphas(1f);

        //Time to re-add to imageLoader and ScrollSnapRect
        imageLoader.AddImageTo(indexMarkedForDelete, fileMarkedForDelete, textureMarkedForDelete);

        scrollSnapRect.Initialize();
        scrollSnapRect.SetPagePositions();
        if (indexMarkedForDelete < scrollSnapRect.pageCount - 1)
            scrollSnapRect.SetPage(indexMarkedForDelete + 1);
        else
            scrollSnapRect.SetPage(indexMarkedForDelete - 1); //Edge-case of pageCount = 0 is covered by SetPage
        scrollSnapRect.LerpToPage(indexMarkedForDelete);

        //Reset deletion:
        indexMarkedForDelete = -1;
        fileMarkedForDelete = null;
        textureMarkedForDelete = null;
        fadeoutCoroutine = StartCoroutine(FadeoutUndoButton());
    }

    private IEnumerator DelayFadeout(float delay = 2f)
    {
        var startTime = Time.time;

        SetAlphas(1f);
        undoButton.gameObject.SetActive(true);

        while ( Time.time < startTime + delay )
        {
            yield return new WaitForEndOfFrame();
        }
        fadeoutCoroutine = StartCoroutine(FadeoutUndoButton());
    }

    private IEnumerator FadeoutUndoButton(float duration = 1f)
    {
        var startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            var timePercentage = 1 - (startTime + duration - Time.time) / duration;
            var alpha = Mathf.Lerp(1, 0, timePercentage);
            SetAlphas(alpha);
            yield return new WaitForEndOfFrame();
        }
        undoButton.gameObject.SetActive(false);
        SetAlphas(1f);
        DeleteMarkedFile();
        if (fadeoutCoroutine != null)
            StopCoroutine(fadeoutCoroutine);
        fadeoutCoroutine = null;
    }

    private void SetAlphas(float alpha)
    {
        undoButton.SetAlphas(alpha);
    }

    public void OnOptionsButtonClick()
    {
        Debug.Log("Options!");

    }
}
