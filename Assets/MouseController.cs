using System;
using UnityEngine;
using System.Collections;
using System.Xml.Linq;

public class MouseController : MonoBehaviour {

    private Vector3 start;
    private Vector3 end;
    
    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButtonUp(0)) {
            end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            double angle = Math.Atan2(end.y - start.y, end.x - start.x) * 180 / Mathf.PI;
            
            Debug.Log(angle);
        }
    }
}