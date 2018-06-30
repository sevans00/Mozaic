using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UndoButton : MonoBehaviour {

    public Image[] images;
    public Text[] texts;

    public void SetAlphas(float alpha)
    {
        foreach (var image in images)
        {
            var oldColor = image.color;
            var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            image.color = newColor;
        }
        foreach (var text in texts)
        {
            var oldColor = text.color;
            var newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
            text.color = newColor;
        }

    }

}
