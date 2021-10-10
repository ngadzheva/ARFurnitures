using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PlaneAreaManager : MonoBehaviour
{
    ARPlaneManager planeManager;
    public double planeWidth { set; get; }
    public double planeHeight { set; get; }

    void Start()
    {
        this.planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void LateUpdate()
    {
        planeWidth = 0;
        planeHeight = 0;

        foreach (ARPlane plane in planeManager.trackables)
        {
            planeWidth += plane.size.x;
            planeHeight += plane.size.y;
        }
    }
}
