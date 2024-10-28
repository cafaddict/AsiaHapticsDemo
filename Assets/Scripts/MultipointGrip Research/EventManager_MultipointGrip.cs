using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine; 

public class EventManager_MultipointGrip : MonoBehaviour {

    public enum EventManagerStates
    {
        Tutorial1,  
        Tutorial2,  
        Task101, //Task 1_1
        Task102, //Task 1_2
        Task201,
        Task202,
        Task203,
        Task3,      
        Task4,      
        Task5    
    }

    //Enter, Exit, FixedUpdate, Update, LateUpdate, Finally
    public static StateMachine<EventManagerStates> fsm;
    public GameObject mainCamera;
    public Transform cameraInitTrans;
    public HandManager handManager;

    [Space(5)]
    [Header("UI")]

    public UnityEngine.UI.Text modalTitle;
    public UnityEngine.UI.Text modalContent;
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
    [Header("Task1_1")]
    public GameObject[] task1_1Obj;

    [Space(5)]
    [Header("Task1_1")]
    public GameObject[] task1_2Obj;

    [Space(5)]
    [Header("Task2_1")]
    public GameObject[] task2_1Obj;

    [Space(5)]
    [Header("Task2_2")]
    public GameObject[] task2_2Obj;

    [Space(5)]
    [Header("Task2_3")]
    public GameObject[] task2_3Obj;

    private bool IsPopUpFinishingUI = false;            
    private string originModalContentText;



    void Start () {

        ////////////////////////////////////
        ////// Unity + CHAI3D Disabled /////
        ////////////////////////////////////
        DisableObjects(tutorial2Obj);
        DisableObjects(task1_1Obj);
        DisableObjects(task1_2Obj);
        DisableObjects(task2_1Obj);
        DisableObjects(task2_2Obj);
        DisableObjects(task2_3Obj);

        fsm = StateMachine<EventManagerStates>.Initialize(this);
        fsm.ChangeState(EventManagerStates.Tutorial1);
    }

    void Update()
    {
        /////////////////////////
        //////For Debugging//////
        /////////////////////////
        if (Input.GetKeyDown(KeyCode.A))
            fsm.ChangeState(EventManagerStates.Tutorial2);
        else if (Input.GetKeyDown(KeyCode.S))
            fsm.ChangeState(EventManagerStates.Task101);
        else if (Input.GetKeyDown(KeyCode.D))
            fsm.ChangeState(EventManagerStates.Task102);
        else if (Input.GetKeyDown(KeyCode.F))
            fsm.ChangeState(EventManagerStates.Task201);
        else if (Input.GetKeyDown(KeyCode.G))
            fsm.ChangeState(EventManagerStates.Task202);
        else if (Input.GetKeyDown(KeyCode.H))
            fsm.ChangeState(EventManagerStates.Task203);
        else if (Input.GetKeyDown(KeyCode.J))
            fsm.ChangeState(EventManagerStates.Task3);

        //Enable/Disable Notifiaction UI
        if(Input.GetKeyDown(KeyCode.Space) && !IsPopUpFinishingUI)
            flipNotificationUIState();

        //Wheter move on to next task
        if (IsPopUpFinishingUI)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                IsPopUpFinishingUI = false;
                ChangeToNextTask();
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                IsPopUpFinishingUI = false;
                modalContent.text = originModalContentText;
                flipNotificationUIState();
            }
        }
    }

    void Tutorial1_Enter()
    {
        mainCamera.transform.position = cameraInitTrans.position;
        mainCamera.transform.rotation = cameraInitTrans.rotation;

        InitializeTask();
        handManager.SetModelToUse(true, false, false);
    }

    void Tutorial1_Update()
    {
        if (IsArrivedToTarget())
            ChangeToNextTask();
    }

    void Tutorial1_Finally()
    {
        handManager.FinalizeTask();
        FinalizeTask(tutorial1Obj);
    }
    
    void Tutorial2_Enter()
    {
        InitializeTask();
        EnableObjects(tutorial2Obj);

        handManager.SetModelToUse(true, false, false);
    }

    void Tutorial2_Finally()
    {
        handManager.FinalizeTask();
        FinalizeTask(tutorial2Obj);
    }

    void Task101_Enter()
    {
        InitializeTask();
        EnableObjects(task1_1Obj);
        handManager.SetModelToUse(true, false, false);

        SetConstGripperForceToObj(DataGenerationManager.GetConstGripperForces("1_1")); 
    }

    void Task101_Finally()
    {
        handManager.FinalizeTask();

        DataOutputManager.PrintObjGrippedCnt(GetObjGrippedCnt());
        DataOutputManager.PrintMixtureData(GetMixtureData());
        DataOutputManager.PrintGripFinishInfo();

        FinalizeTask(task1_1Obj);
    }

    void Task102_Enter()
    {
        InitializeTask();
        EnableObjects(task1_2Obj);
        handManager.SetModelToUse(true, false, false);

        SetConstGripperForceToObj(DataGenerationManager.GetConstGripperForces("1_2"));
    }

    void Task102_Finally()
    {
        handManager.FinalizeTask();

        DataOutputManager.PrintObjGrippedCnt(GetObjGrippedCnt());
        DataOutputManager.PrintMixtureData(GetMixtureData());
        DataOutputManager.PrintGripFinishInfo();

        FinalizeTask(task1_2Obj);
    }

    void Task201_Enter()
    {
        InitializeTask();
        EnableObjects(task2_1Obj);
        handManager.SetModelToUse(true, true, false);

        SetConstGripperForceToObj(DataGenerationManager.GetConstGripperForces("2_1"));
        SetGripperForceRangeToObj(DataGenerationManager.gripperForceRange_task2_1);
    }

    void Task201_Finally()
    {
        handManager.FinalizeTask();
        FinalizeTask(task2_1Obj);

        DataOutputManager.PrintObjGrippedCnt(GetObjGrippedCnt());
        DataOutputManager.PrintMixtureData(GetMixtureData());
        DataOutputManager.PrintGripFinishInfo();
    }

    void Task202_Enter()
    {
        InitializeTask();
        EnableObjects(task2_2Obj);
        handManager.SetModelToUse(true, true, false);

        SetConstGripperForceToObj(DataGenerationManager.GetConstGripperForces("2_2"));
        SetGripperForceRangeToObj(DataGenerationManager.gripperForceRange_task2_2);
    }

    void Task202_Finally()
    {
        handManager.FinalizeTask();
        FinalizeTask(task2_2Obj);
        DataOutputManager.PrintObjGrippedCnt(GetObjGrippedCnt());
        DataOutputManager.PrintMixtureData(GetMixtureData());
        DataOutputManager.PrintGripFinishInfo();
    }

    void Task203_Enter()
    {
        InitializeTask();
        EnableObjects(task2_3Obj);
        handManager.SetModelToUse(true, true, false);

        SetConstGripperForceToObj(DataGenerationManager.GetConstGripperForces("2_3"));
        SetGripperForceRangeToObj(DataGenerationManager.gripperForceRange_task2_3);
    }

    void Task203_Finally()
    {
        handManager.FinalizeTask();
        FinalizeTask(task2_3Obj);
        DataOutputManager.PrintObjGrippedCnt(GetObjGrippedCnt());
        DataOutputManager.PrintMixtureData(GetMixtureData());
        DataOutputManager.PrintGripFinishInfo();
    }

    void Task3_Enter()
    {
        InitializeTask();
    }

    private void DisableObjects(GameObject[] _gameObjects)
    {
        for (int i = 0; i < _gameObjects.Length; i++)
        {
            _gameObjects[i].SetActive(false);
            if (_gameObjects[i].GetComponent<TouchableObject>() != null)
                HapticNativePlugin.SetObjectHaptic(_gameObjects[i].GetComponent<TouchableObject>().objectId, false);
        }
    }
    private void EnableObjects(GameObject[] _gameObjects)
    {
        for (int i = 0; i < _gameObjects.Length; i++)
        {
            _gameObjects[i].SetActive(true);
            if (_gameObjects[i].GetComponent<TouchableObject>())
                HapticNativePlugin.SetObjectHaptic(_gameObjects[i].GetComponent<TouchableObject>().objectId, true);
        }
    }

    private void SetUIContents(EventManagerStates taskNum)
    {
        Animator animator;
        if (!modal.activeSelf)
            modal.SetActive(true);

        switch (taskNum)
        {
            case EventManagerStates.Tutorial1:
                modalTitle.text = "Tutorial 1";
                modalContent.text = "Grip object in the scene\n\nRefer to movie \n\nBy pressing 'spaceBar' you can show on/off this guidlines";
                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Tutorial2:
                modalTitle.text = "Tutorial 2";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Tutorial 2 : Title";
                videoContent.clip = tutorial2Clip;
                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task101:
                modalTitle.text = "Task1_1";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 1_1 : Title";
                videoContent.clip = tutorial2Clip;
                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task102:
                modalTitle.text = "Task1_2";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 1_2 : Title";
                videoContent.clip = tutorial2Clip;
                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task201:
                modalTitle.text = "Task2_1";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 2_1 : Title";

                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task202:
                modalTitle.text = "Task2_2";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 2_2 : Title";

                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task203:
                modalTitle.text = "Task2_3";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 2_3 : Title";

                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

            case EventManagerStates.Task3:
                modalTitle.text = "Task3";
                modalContent.text = "Pour object in the scene\n" + "Second line";
                videoTitle.Text = "Task 3 : Title";

                animator = modal.GetComponent<Animator>();
                animator.Play("Fade-in");
                break;

        }
        IsPopUpFinishingUI = false;
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

    private bool IsArrivedToTarget()
    {
        if (tutorial1_flask.colObj)
            if (tutorial1_flask.colObj.name.Equals("EndPoint"))
                return true;

        return false;
    }

    private int[] GetObjGrippedCnt()
    {
        int[] objGrippedCnt = null;
        int j;

        switch (fsm.State)
        {
            case EventManagerStates.Task101:
                objGrippedCnt = new int[task1_1Obj.Length - 1];
                j = 0;

                for (int i = 0; i < task1_1Obj.Length; i++)
                    if (task1_1Obj[i].GetComponent<TouchableObject>())
                        objGrippedCnt[j++] = task1_1Obj[i].GetComponent<TouchableObject>().grippedCnt;

                break;

            case EventManagerStates.Task102:
                objGrippedCnt = new int[task1_2Obj.Length - 1];
                j = 0;

                for (int i = 0; i < task1_2Obj.Length; i++)
                    if (task1_2Obj[i].GetComponent<TouchableObject>())
                        objGrippedCnt[j++] = task1_2Obj[i].GetComponent<TouchableObject>().grippedCnt;

                break;

            case EventManagerStates.Task201:
                objGrippedCnt = new int[task2_1Obj.Length - 1];
                j = 0;

                for (int i = 0; i < task2_1Obj.Length; i++)
                    if (task2_1Obj[i].GetComponent<TouchableObject>())
                        objGrippedCnt[j++] = task2_1Obj[i].GetComponent<TouchableObject>().grippedCnt;

                break;

            case EventManagerStates.Task202:
                objGrippedCnt = new int[task2_2Obj.Length - 1];
                j = 0;

                for (int i = 0; i < task2_2Obj.Length; i++)
                    if (task2_2Obj[i].GetComponent<TouchableObject>())
                        objGrippedCnt[j++] = task2_2Obj[i].GetComponent<TouchableObject>().grippedCnt;

                break;

            case EventManagerStates.Task203:
                objGrippedCnt = new int[task2_3Obj.Length - 1];
                j = 0;

                for (int i = 0; i < task2_3Obj.Length; i++)
                    if (task2_3Obj[i].GetComponent<TouchableObject>())
                        objGrippedCnt[j++] = task2_3Obj[i].GetComponent<TouchableObject>().grippedCnt;

                break;

        }
        
        return objGrippedCnt;
    }

    private void SetConstGripperForceToObj(double[] constGripperForces)
    {
        int j;
        switch (fsm.State)
        {
            case EventManagerStates.Task101:

                j = 0;
                for(int i=0; i<task1_1Obj.Length; i++)
                    if (task1_1Obj[i].GetComponent<TouchableObject>() != null)
                        task1_1Obj[i].GetComponent<TouchableObject>().constGripperForce = constGripperForces[j++];

                break;

            case EventManagerStates.Task102:
                j = 0;
                for (int i = 0; i < task1_1Obj.Length; i++)
                    if (task1_2Obj[i].GetComponent<TouchableObject>() != null)
                        task1_2Obj[i].GetComponent<TouchableObject>().constGripperForce = constGripperForces[j++];

                break;

            case EventManagerStates.Task201:
                j = 0;
                for (int i = 0; i < task2_1Obj.Length; i++)
                    if (task2_1Obj[i].GetComponent<TouchableObject>() != null)
                        task2_1Obj[i].GetComponent<TouchableObject>().constGripperForce = constGripperForces[j++];

                break;

            case EventManagerStates.Task202:
                j = 0;
                for (int i = 0; i < task2_2Obj.Length; i++)
                    if (task2_2Obj[i].GetComponent<TouchableObject>() != null)
                        task2_2Obj[i].GetComponent<TouchableObject>().constGripperForce = constGripperForces[j++];

                break;

            case EventManagerStates.Task203:
                j = 0;
                for (int i = 0; i < task2_3Obj.Length; i++)
                    if (task2_3Obj[i].GetComponent<TouchableObject>() != null)
                        task2_3Obj[i].GetComponent<TouchableObject>().constGripperForce = constGripperForces[j++];

                break;

        }
       
    }

    private void SetGripperForceRangeToObj(double[] gripperForceRange)
    {
        int j;
        switch (fsm.State)
        {
            case EventManagerStates.Task201:
                j = 0;
                for (int i = 0; i < task2_1Obj.Length; i++)
                    if (task2_1Obj[i].GetComponent<TouchableObject>() != null)
                        task2_1Obj[i].GetComponent<TouchableObject>().gripperForceRange = gripperForceRange[j++];

                break;

            case EventManagerStates.Task202:
                j = 0;
                for (int i = 0; i < task2_2Obj.Length; i++)
                    if (task2_2Obj[i].GetComponent<TouchableObject>() != null)
                        task2_2Obj[i].GetComponent<TouchableObject>().gripperForceRange = gripperForceRange[j++];

                break;

            case EventManagerStates.Task203:
                j = 0;
                for (int i = 0; i < task2_3Obj.Length; i++)
                    if (task2_3Obj[i].GetComponent<TouchableObject>() != null)
                        task2_3Obj[i].GetComponent<TouchableObject>().gripperForceRange = gripperForceRange[j++];

                break;
        }
    }

    private void InitializeTask()
    {
        SetUIContents(fsm.State);
        DataOutputManager.InitToSavingData(fsm.State.ToString());
    }

    private void FinalizeTask(GameObject[] _gameObjects)
    {
        Debug.Log("Finsih : " + fsm.State.ToString());
        videoContent.Stop();
        DataOutputManager.PrintElapsedTime();

        DestroyObjects(_gameObjects);
    }

    private float[] GetMixtureData()
    {
        MixtureObject mixtureObject = null;
        switch (fsm.State)
        {
            case EventManagerStates.Tutorial2:
                mixtureObject = tutorial2Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;

            case EventManagerStates.Task101:
                mixtureObject = task1_1Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;

            case EventManagerStates.Task102:
                mixtureObject = task1_2Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;

            case EventManagerStates.Task201:
                mixtureObject = task2_1Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;

            case EventManagerStates.Task202:
                mixtureObject = task2_2Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;

            case EventManagerStates.Task203:
                mixtureObject = task2_3Obj[0].transform.Find("flask").Find("liquid").GetComponent<MixtureObject>();
                break;
        }

        mixtureObject.SetMixtureData();

        return mixtureObject.mixtureData;
    }

    private void flipNotificationUIState()
    {
        bool IsActive = modal.activeSelf;
        modal.SetActive(!IsActive);
    }

    public void PopUpFinishingUI()
    {
        if (!IsPopUpFinishingUI)
        {
            Debug.Log("POP UP Finishing UI");
            IsPopUpFinishingUI = true;

            originModalContentText = modalContent.text;
            modalContent.text = "Will you move on to next task?\n\nPress 'Y' for Yes \n\nPress 'N' for No";
            modal.SetActive(true);
        }
    }

    public static void ChangeToNextTask()
    {
        if (fsm.State == EventManagerStates.Tutorial1)
            fsm.ChangeState(EventManagerStates.Tutorial2);

        else if (fsm.State == EventManagerStates.Tutorial2)
            fsm.ChangeState(EventManagerStates.Task101);

        else if (fsm.State == EventManagerStates.Task101)
            fsm.ChangeState(EventManagerStates.Task102);

        else if (fsm.State == EventManagerStates.Task102)
            fsm.ChangeState(EventManagerStates.Task201);

        else if (fsm.State == EventManagerStates.Task201)
            fsm.ChangeState(EventManagerStates.Task202);

        else if (fsm.State == EventManagerStates.Task202)
            fsm.ChangeState(EventManagerStates.Task203);

        else if (fsm.State == EventManagerStates.Task203)
            fsm.ChangeState(EventManagerStates.Task3);
    }
}
