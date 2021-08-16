using UnityEngine;

public static class UtilityHelper
{

    public static void RotateTowardCamera(GameObject augmentation)
    {
        if (Vuforia.VuforiaManager.Instance.ARCameraTransform != null)
        {
            var lookAtPosition = Vuforia.VuforiaManager.Instance.ARCameraTransform.position - augmentation.transform.position;
            lookAtPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookAtPosition);
            augmentation.transform.rotation = rotation;
        }
    }

    public static void EnableRendererColliderCanvas(GameObject gameObject, bool enable)
    {
        var rendererComponents = gameObject.GetComponentsInChildren<Renderer>(true);
        var colliderComponents = gameObject.GetComponentsInChildren<Collider>(true);
        var canvasComponents = gameObject.GetComponentsInChildren<Canvas>(true);

        foreach (var component in rendererComponents)
            component.enabled = enable;

        foreach (var component in colliderComponents)
            component.enabled = enable;

        foreach (var component in canvasComponents)
            component.enabled = enable;
    }
}
