using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class PlaneAreaBehaviour : MonoBehaviour
{
    public ARPlane arPlane;

    public float planeWidth { get; private set; }

    public float planeHeight { get; private set; }

    void Start() 
    {
        arPlane.boundaryChanged += ArPlaneBoundaryChanged;
    }

    void OnDestroy() 
    {
        arPlane.boundaryChanged -= ArPlaneBoundaryChanged;
    }

    private void ArPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs obj) 
    { 
        // areaText.text = CalculatePlaneArea(arPlane).ToString(); 
        this.planeWidth = arPlane.size.x;
        this.planeHeight = arPlane.size.y;
    } 
    
    private float CalculatePlaneArea(ARPlane plane) 
    { 
        return plane.size.x * plane.size.y; 
    }
}
