using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneAreaManager : MonoBehaviour
{
    void Update()
    {
        if (Input.touchCount > 0) 
        { 
            var touch = Input.GetTouch(0); 

            if (touch.phase == TouchPhase.Ended) 
            { 
                if (Input.touchCount == 1) 
                {
                    Ray raycast = Camera.main.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(raycast, out RaycastHit raycastHit)) 
                    {
                        var planeAreaBehaviour = raycastHit.collider.gameObject.GetComponent<PlaneAreaBehaviour>(); 
                        
                        if (planeAreaBehaviour != null) 
                        { 
                            // planeAreaBehaviour.ToggleAreaView(); 
                        }
                    }
                } 
            } 
        }
    }
}
