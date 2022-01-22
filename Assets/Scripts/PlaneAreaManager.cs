using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlaneAreaManager : MonoBehaviour
{
    ARPlaneManager planeManager;
    public double planeWidth { set; get; }
    public double planeHeight { set; get; }
    public bool enableMeasurement;

    void Start()
    {
        this.planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void LateUpdate()
    {
        planeWidth = 0;
        planeHeight = 0;

        if (enableMeasurement)
        {
            foreach (ARPlane plane in planeManager.trackables)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp && plane.subsumedBy != null)
                { 
                    planeWidth = plane.size.x;
                    planeHeight = plane.size.y;
                }
            }
        }
    }
}
