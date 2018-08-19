using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleToCanvas : MonoBehaviour {

    public CanvasScaler canvasScaler;

    public void Initialize()
    {
        var referenceX = canvasScaler.referenceResolution.x;
        var referenceY = canvasScaler.referenceResolution.y;
        var ratio = referenceX / referenceY;
        Debug.LogWarning("Canvas Ratio: "+ratio);
        var screenRatio = (float)Screen.width / (float)(Screen.height);
        Debug.LogWarning("Screen Ratio: " + screenRatio);
        Debug.LogWarning("Widths: "+Screen.width+", "+referenceX);
        Debug.LogWarning("HEights: " + Screen.height + ", " + referenceY);
        var rawImage = GetComponent<RawImage>();
        var heightRatio = Screen.height / referenceY;
        //rawImage.rectTransform.localScale = new Vector3(1f, heightRatio, 1f);

    }
}
