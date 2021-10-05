using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerManager : MonoBehaviour
{
    public List<Transform> objList = new List<Transform>();
    public LineRenderer lineObj;
    
    public void SetInit(Vector3 pos)
    {
        objList[0].transform.position = pos;
        lineObj.SetPosition(0, pos);
    }

    public void SetObj(Vector3 pos)
    {
        objList[1].transform.position = pos;
        lineObj.SetPosition(1, pos);
    }

    void Update()
    {
        Vector3 tVec = objList[1].transform.position - objList[0].transform.position;
        float tDis = tVec.magnitude;
    }
}
