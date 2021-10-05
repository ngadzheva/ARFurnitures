using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class DistanceManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public Vector2 centerVec;

    public Transform pivotCamera;
    public Transform pivot;
    public Transform rulerPool;
    public GameObject rulerObject;
    private RulerManager ruler;
    private List<RulerManager> rulerObjList = new List<RulerManager>();
    private bool rulerEnabled;
    private Vector3 rulerPosSave;

    void Start()
    {
        centerVec = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
    }

    void Update()
    {
        hits.Clear();

        if(raycastManager.Raycast(centerVec, hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;
            rulerEnabled = true;
            rulerPosSave = hitPose.position;
            pivot.rotation = Quaternion.Lerp(pivot.rotation, hitPose.rotation, 0.2f);

            if (ruler != null)
            {
                ruler.SetObj(hitPose.position);
            }
        }
        else
        {
            rulerEnabled = false;
            Quaternion tRot = Quaternion.Euler(90f, 0, 0);
            pivot.rotation = Quaternion.Lerp(pivot.rotation, tRot, 0.5f);
        }
    }

    public void MakeRulerObj()
    {
        if (rulerEnabled)
        {
            if (ruler == null)
            {
                Debug.Log(rulerObject);

                GameObject tObj = Instantiate(rulerObject) as GameObject;
                tObj.transform.SetParent(rulerPool);
                tObj.transform.position = Vector3.zero;
                tObj.transform.localScale = new Vector3(1, 1, 1);

                RulerManager tRuler = tObj.GetComponent<RulerManager>();
                tRuler.SetInit(rulerPosSave);
                rulerObjList.Add(tRuler);
                ruler = tRuler;
            }
            else
            {
                ruler = null;
            }
        }
    }
}
