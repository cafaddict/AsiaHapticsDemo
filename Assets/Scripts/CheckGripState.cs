using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckGripState : MonoBehaviour {

    public bool isCollide = false;
    public GameObject colObject;

    void OnTriggerEnter(Collider col)
    {
        if (col.name.Equals("FinishButton"))
            return;

        isCollide = true;
        colObject = col.gameObject;
    }

    void OnTriggerExit(Collider col)
    {
        isCollide = false;
        colObject = null;
    }
}
