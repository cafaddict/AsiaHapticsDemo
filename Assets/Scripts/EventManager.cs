using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine; 

public class EventManager : MonoBehaviour {

    //Enter, Exit, FixedUpdate, Update, LateUpdate, Finally
    public enum EventManagerStates
    {
        Tutorial1, //Haptic Manipulation & Grip
        Tutorial2, //Grip & Pouring

        Task1, 
        Task2, 
        Task3 
    }

    public StateMachine<EventManagerStates> fsm;
    public GameObject mainCamera;
    public Transform cameraInitTrans;
    public HandManager handManager;
    
    [Space(5)]
    [Header("UI")]
    public UnityEngine.UI.Text modalTitle;
    public UnityEngine.UI.Text modalContent;
    public UnityEngine.UI.Button modalOKButton;
    public UnityEngine.UI.Button modalCloseButton;
    public GameObject modal;

    [Space(5)]
    public MonotypeUnityTextPlugin.MP3DTextComponent videoTitle;
    public UnityEngine.Video.VideoPlayer videoContent;
    public UnityEngine.Video.VideoClip tutorial2Clip;

    [Space(5)]
    [Header("Tutorial1")]
    public GameObject[] tutorial1Obj;
    public TouchableObject tutorial1_flask;

    [Space(5)]
    [Header("Tutorial2")]
    public GameObject[] tutorial2Obj;

    [Space(5)]
    [Header("Task1")]
    public GameObject[] task1Obj;

    [Space(5)]
    [Header("Task2")]
    public GameObject[] task2Obj;

    void Start () {

        //Unity + CHAI3D Disabled
        //Tutorial2
        for(int i=0; i<tutorial2Obj.Length; i++)
        {
            tutorial2Obj[i].SetActive(false);
            if (tutorial2Obj[i].GetComponent<TouchableObject>() != null)
                HapticNativePlugin.SetObjectHaptic(tutorial2Obj[i].GetComponent<TouchableObject>().objectId, false);
        }
        //Task1
        for (int i = 0; i < task1Obj.Length; i++)
        {
            task1Obj[i].SetActive(false);
            if (task1Obj[i].GetComponent<TouchableObject>() != null)
                HapticNativePlugin.SetObjectHaptic(task1Obj[i].GetComponent<TouchableObject>().objectId, false);
        }
        //Task2
        for (int i = 0; i < task2Obj.Length; i++)
        {
           task2Obj[i].SetActive(false);
        }

        fsm = StateMachine<EventManagerStates>.Initialize(this);
        fsm.ChangeState(EventManagerStates.Tutorial1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            fsm.ChangeState(EventManagerStates.Tutorial2);
        else if (Input.GetKeyDown(KeyCode.S))
            fsm.ChangeState(EventManagerStates.Task1);
        else if (Input.GetKeyDown(KeyCode.D))
            fsm.ChangeState(EventManagerStates.Task2);
    }

    void ClickedOKButton()
    {
        if (fsm.State == EventManagerStates.Tutorial1)
        {
            videoContent.Play();
        }
        else if (fsm.State == EventManagerStates.Tutorial2)
        {
            videoContent.Play();
        }

    }

    void Tutorial1_Enter()
    {
        mainCamera.transform.position = cameraInitTrans.position;
        mainCamera.transform.rotation = cameraInitTrans.rotation;

        modalTitle.text = "Tutorial 1";
        modalContent.text = "Grip object in the scene\n\n" + "Refer to movie";
        Animator animator = modal.GetComponent<Animator>();
        animator.Play("Fade-in");
    }

    void Tutorial1_Update()
    {
        if (IsArrivedToTarget() || Input.GetKeyDown(KeyCode.Keypad0))
        {
            fsm.ChangeState(EventManagerStates.Tutorial2);
        }
        
    }
   
    void Tutorial1_Finally()
    {
        Debug.Log("Finish tutorial1");
        videoContent.Stop();

        DestroyObjects(tutorial1Obj);

        handManager.FinalizeTask();
    }

    void Tutorial2_Enter()
    {
        Debug.Log("Tutoial2 Enter");
        //UI
        modalTitle.text = "Tutorial 2";
        modalContent.text = "Pour object in the scene\n" + "Second line";
        videoTitle.Text = "Tutorial 2 : Title";
        videoContent.clip = tutorial2Clip;
        Animator animator = modal.GetComponent<Animator>();
        animator.Play("Fade-in");

        //Objects Unity + CHAI3D Enabled
        for (int i = 0; i < tutorial2Obj.Length; i++)
        {
            tutorial2Obj[i].SetActive(true);
            if (tutorial2Obj[i].GetComponent<TouchableObject>())
                HapticNativePlugin.SetObjectHaptic(tutorial2Obj[i].GetComponent<TouchableObject>().objectId, true);
        }
    }

    void Tutorial2_Finally()
    {
        Debug.Log("Finish tutorial2");
        videoContent.Stop();

        DestroyObjects(tutorial2Obj);
        
        handManager.FinalizeTask();
    }

    void Task1_Enter()
    {
        Debug.Log("Enter Task1");
        //UI
        modalTitle.text = "Task1";
        modalContent.text = "Pour object in the scene\n" + "Second line";
        videoTitle.Text = "Task 1 : Title";
        videoContent.clip = tutorial2Clip;//
        Animator animator = modal.GetComponent<Animator>();
        animator.Play("Fade-in");

        //Objects Unity + CHAI3D Enabled
        for (int i = 0; i < task1Obj.Length; i++)
        {
            task1Obj[i].SetActive(true);
            if (task1Obj[i].GetComponent<TouchableObject>())
                HapticNativePlugin.SetObjectHaptic(task1Obj[i].GetComponent<TouchableObject>().objectId, true);
        }
    }

    void Task1_Finally()
    {
        Debug.Log("Finish Task1");
        videoContent.Stop();
        Destroy(task1Obj[0]); //Only Beaker
        handManager.FinalizeTask();
    }

    void Task2_Enter()
    {
        Debug.Log("Enter Task2");
        //UI
        modalTitle.text = "Task2";
        modalContent.text = "Pour object in the scene\n" + "Second line";
        videoTitle.Text = "Task 2 : Title";
        
        Animator animator = modal.GetComponent<Animator>();
        animator.Play("Fade-in");

        task2Obj[0].SetActive(true);
        
    }

    public void RelasedGrip(GameObject colObj)
    {
        colObj.transform.position = colObj.GetComponent<TouchableObject>().initPos;
        colObj.transform.rotation = colObj.GetComponent<TouchableObject>().initRot;
        colObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    private bool IsArrivedToTarget()
    {
        if (tutorial1_flask.colObj)
            if (tutorial1_flask.colObj.name.Equals("EndPoint"))
                return true;

        return false;
    }

    private void DestroyObjects(GameObject[] objects)
    {
        for (int i = 0; i < objects.Length; i++)
        {
            Destroy(objects[i]);

            if (objects[i].GetComponent<TouchableObject>() != null)
                HapticNativePlugin.SetObjectHaptic(objects[i].GetComponent<TouchableObject>().objectId, false);
        }
    }

}
