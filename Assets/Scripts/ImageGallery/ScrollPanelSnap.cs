using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollPanelSnap : MonoBehaviour {

    public ScrollRect scrollRect;
    public GameObject contentPanel;
    public ImageLoader imageLoader;

    public List<Vector3> positions;
    
    public void Update()
    {
        //Debug.Log("Index: "+GetContentIndex());
        //Debug.Log("Content X: "+contentPanel.transform.position.x);

    }

    public void Initialize()
    {
        var childCount = contentPanel.transform.childCount;
        positions = new List<Vector3>();
        for (int ii = 0; ii < childCount; ii++)
        {
            var child = contentPanel.transform.GetChild(ii);
            var position = child.transform.localPosition;
            var image = child.GetComponent<Image>();
            position.y = 0;
            positions.Add(position);
        }
        //contentPanel.transform.localPosition = positions[0];
        //scrollRect.horizontalNormalizedPosition
    }

    private Vector3 FindClosestFrom(Vector3 start, System.Collections.Generic.List<Vector3> positions)
    {
        Vector3 closest = Vector3.zero;
        float distance = Mathf.Infinity;

        foreach (Vector3 position in positions)
        {
            if (Vector3.Distance(start, position) < distance)
            {
                distance = Vector3.Distance(start, position);
                closest = position;
            }
        }

        return closest;
    }

    private int GetContentIndex()
    {


        var xPos = contentPanel.transform.position.x;
        
        return 1;
    }

    public void OnBeginDrag()
    {
        Debug.Log("Start Drag!");
        
    }

    public void OnEndDrag()
    {
        //Initialize();
        Debug.Log("End Drag!");
        //scrollRect.StopMovement();
        //contentPanel.transform.localPosition = positions[0];
        Debug.Log("Position = "+positions[0]);

    }


    
}
