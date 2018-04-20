using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPanelSnap : MonoBehaviour {

    public ScrollRect scrollRect;
    

    public void OnBeginDrag()
    {
        Debug.Log("Start Drag!");
        
    }

    public void OnEndDrag()
    {
        Debug.Log("End Drag!");
        //scrollRect.StopMovement();
        
    }
    
}
