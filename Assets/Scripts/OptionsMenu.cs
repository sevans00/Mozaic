using UnityEngine;
using System.Collections;

public class OptionsMenu : MonoBehaviour {

    public bool opening = false;
    public bool closing = false;
    public bool animating { get { return opening || closing; } }
    public float tweenTotalTime = 1.0f;
    public float tweenStartTime;

    public Vector3 openPosition = Vector3.zero;
    public Vector3 closedPosition = Vector3.zero + Vector3.left * Screen.width / 2;


    public void Update()
    {
        if (animating)
        {
            var tweenPercentage = (Time.time - tweenStartTime) / tweenTotalTime;
            if (opening)
            {
                gameObject.transform.localPosition = Vector3.Lerp(closedPosition, openPosition, tweenPercentage);
                if (tweenPercentage >= 1f)
                {
                    gameObject.transform.localPosition = openPosition;
                    opening = false;
                    closing = false;
                }
            }
            else if (closing)
            {
                gameObject.transform.localPosition = Vector3.Lerp(openPosition, closedPosition, tweenPercentage);
                if (tweenPercentage >= 1f)
                {
                    gameObject.transform.localPosition = closedPosition;
                    opening = false;
                    closing = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void Open()
    {
        //Animate in
        tweenStartTime = Time.time;
        closing = false;
        opening = true;
        gameObject.transform.localPosition = closedPosition;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        //Animate out
        tweenStartTime = Time.time;
        closing = true;
        opening = false;
        gameObject.transform.localPosition = openPosition;
    }



}
