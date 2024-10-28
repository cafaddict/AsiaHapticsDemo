using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    public GameObject test;
    void Start () {

        Debug.Log(test.transform.GetComponent<MonotypeUnityTextPlugin.MP3DTextComponent>().name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
