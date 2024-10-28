using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishButtonManager : MonoBehaviour {

    [HideInInspector] public EventManager_MultipointGrip eventManager;

    private float beforeClickedTime;
    private float clickWaitingTime = 2;

    void Start()
    {
        eventManager = GameObject.Find("Event Module").GetComponent<EventManager_MultipointGrip>();
        if (eventManager == null)  
            Debug.Log("<color=red>Set EventManager of FinishButton</color>");

        beforeClickedTime = Time.time;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col)
        {
            //계속적인 충돌 방지를 위한 시간 간격 & 이미 띄워있는경우 방지
            if (Mathf.Abs(beforeClickedTime - Time.time) > clickWaitingTime && !eventManager.modal.activeSelf)
            {
                beforeClickedTime = Time.time;
                eventManager.PopUpFinishingUI();
            }
        }
    }

}
