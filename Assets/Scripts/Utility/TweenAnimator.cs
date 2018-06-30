using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenAnimator : MonoBehaviour {

    public bool animatingForwards = false;
    public bool animatingBackwards = false;
    public bool animating { get { return animatingForwards || animatingBackwards; } }
    public float tweenTotalTime = 1.0f;
    public float tweenStartTime;

    public Vector3 startPosition = Vector3.zero;
    public Vector3 endPosition = Vector3.zero + Vector3.left * Screen.width / 2;


    public void Update()
    {
        if (animating)
        {
            var tweenPercentage = (Time.time - tweenStartTime) / tweenTotalTime;
            if (animatingForwards)
            {
                gameObject.transform.localPosition = Vector3.Lerp(endPosition, startPosition, tweenPercentage);
                if (tweenPercentage >= 1f)
                {
                    gameObject.transform.localPosition = startPosition;
                    animatingForwards = false;
                    animatingBackwards = false;
                }
            }
            else if (animatingBackwards)
            {
                gameObject.transform.localPosition = Vector3.Lerp(startPosition, endPosition, tweenPercentage);
                if (tweenPercentage >= 1f)
                {
                    gameObject.transform.localPosition = endPosition;
                    animatingForwards = false;
                    animatingBackwards = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void PlayForwards()
    {
        //Animate in
        tweenStartTime = Time.time;
        animatingBackwards = false;
        animatingForwards = true;
        gameObject.transform.localPosition = endPosition;
    }

    public void PlayBackwards()
    {
        //Animate out
        tweenStartTime = Time.time;
        animatingBackwards = true;
        animatingForwards = false;
        gameObject.transform.localPosition = startPosition;
    }

}
