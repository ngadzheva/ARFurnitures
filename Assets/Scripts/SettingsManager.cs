using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Toggle scaleToggle;

    void Start()
    {
        scaleToggle.isOn = false;
    }

    public void ToggleOcclusion(bool toggle)
    {
        AROcclusionManager occlusionManager = Camera.main.GetComponent<AROcclusionManager>();
        occlusionManager.enabled = toggle;
    }

    public void TogglePinchScaling(bool toggle)
    {
        TouchHandler touchHandler = FindObjectOfType<FurniturePlacement>().GetComponent<TouchHandler>();
        touchHandler.enablePinchScaling = toggle;
    }

    public void ToggleMeasurement(bool toggle)
    {
        PlaneAreaManager planeAreaManager = FindObjectOfType<PlaneManager>().GetComponent<PlaneAreaManager>();
        planeAreaManager.enableMeasurement = toggle;
    }
}
