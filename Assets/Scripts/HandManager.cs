using System.Collections;
using UnityEngine;
using System;
public class HandManager : MonoBehaviour {

    [Header("Select Model to Use")]
    public bool useFullHandGrip = true;         //InnerJntAngle, RotationalProjection 함수 적용 유/무, PendulumDynamic 내부 ColTipCnt=2
    public bool useObjDynamicModel = true;      //PendulumDynamic 함수 적용 유/무 (Gravity와 Friction은 둘 다 적용)
    public bool useForceWithGripShape = true;   //PendulumDynamic 함수 내부 Force Variation 적용 유무.
    public static bool isHapticAvail;

    [Space(5)]
    public EventManager_MultipointGrip eventManager;

    public enum gripMode
    {
        IDLE,
        SELECTION
    };

    [Space(5)]
    [Header("Multi-point Grip Info")]
    [HideInInspector]
    public bool isJointInfoSet = false;
    public gripMode state;
    public bool isGripped = false;

    public GameObject[] collisionPointsForDebugging;
    private GameObject[] colliderPoints;

    private Transform[] handTrans;
    private Vector3[] initHandAngle;
    private Matrix4x4 initRootAngleMatrix;
    private int handTransSize = 36;

    struct JointLength
    {
        public double sleeveToRoot;
        public double rootToThumbPalm;
        public double rootToFingerPalm;
        public double rootToMiddlePalm;
        public double rootToRingPalm;
        public double rootToPinkyPalm;
       
        public double thumbPalmToMCP;
        public double thumbMCPToPIP;
        public double thumbPIPToTIP;
        
        public double fingerPalmToMCP;
        public double fingerMCPToPIP;
        public double fingerPIPToDIP;
        public double fingerDIPToTIP;
        public double middlePalmToMCP;
        public double middleMCPToPIP;
        public double middlePIPToDIP;
        public double middleDIPToTIP;
        public double ringPalmToMCP;
        public double ringMCPToPIP;
        public double ringPIPToDIP;
        public double ringDIPToTIP;

        public double pinkyPalmToMCP;
        public double pinkyMCPToPIP;
        public double pinkyPIPToDIP;
        public double pinkyDIPToTIP;

        public double thumbPalmToPalm1_1;
        public double palm1_1ToPalm1;
        public double fingerPalmToPalm2_1;
        public double palm2_1ToPalm2;
        public double middlePalmToPalm3_1;
        public double palm3_1ToPalm3;
        public double ringPalmToPalm4_1;
        public double palm4_1ToPalm4;
        public double pinkyPalmToPalm5_1;
        public double palm5_1ToPalm5;

    };
    private JointLength jointLength;
    private int jointLengthNum = 35;
    private double[] jointLengthIdx;
    private Vector3[] jointDirIdx;

    private CheckGripState thumbGripState;
    private CheckGripState fingerGripState;
    private CheckGripState middleGripState;
    private CheckGripState ringGripState;
    private CheckGripState pinkyGripState;
    private bool[] isTipsCollide;

    private Transform colObjOriginalParent;
    private GameObject colObj;
    private Vector3 colObjToRootPos;
    private Vector3 colObjToRootRot;

    private double initGripExtent;
    
    void Awake () {

        isHapticAvail = HapticNativePlugin.prepareHaptics(0.3d);

        if (isHapticAvail)
            InitHandModel();
        else
            Debug.Log("<color=red>Haptic isn't available!!</color> ");
    }

    void Start()
    {
        if(isHapticAvail)
            HapticNativePlugin.startHaptics();
    }

    void FixedUpdate()
    {
        if (isJointInfoSet && isHapticAvail){

            double[] gripExtent = HapticNativePlugin.GetChangedGripExtent();

            SetWholeHandTransform(gripExtent[0]);
            SetWholeHandJointInfo();
            HapticNativePlugin.SetHandModelJointInfo(jointLengthIdx, jointDirIdx);

            //1) If Grippable
            if (state == gripMode.IDLE && (thumbGripState.isCollide && fingerGripState.isCollide))
            {
                //Debug.Log("<color=red>Object is gripped</color>");
                state = gripMode.SELECTION;
                initGripExtent = gripExtent[2];
                isGripped = true;

                //Object를 Hand에 Snapping
                colObj = thumbGripState.colObject;
                colObjOriginalParent = colObj.transform.parent;
                colObj.transform.parent = transform;

                DataOutputManager.PrintInitGripInfo(colObj.name, initGripExtent);
                DataOutputManager.PrintCollisionPoints(colliderPoints, "before");

                //손가락 내부 관절의 물체 표면으로의 추가 회전
                if (useFullHandGrip)
                    CheckJntProjToSurf(thumbGripState.transform.position - fingerGripState.transform.position);

                DataOutputManager.PrintCollisionPoints(colliderPoints, "after");

                HapticNativePlugin.SetIsGrip(true);
                HapticNativePlugin.SetObjectHaptic(colObj.GetComponent<TouchableObject>().objectId, false);

                ///////////////////////////////
                DisplayCollisionPointsForDebugging();
                ///////////////////////////////

                SetTipPointsState();
                colObj.GetComponent<TouchableObject>().SetCollideTipsCnt(isTipsCollide);
                colObj.GetComponent<TouchableObject>().IncGrippedCnt();
            }
            //2) During Grip 
            else if (isGripped)
            {
                colObj.GetComponent<TouchableObject>().SetHandTipsTrans(thumbGripState.gameObject.transform, fingerGripState.gameObject.transform, middleGripState.gameObject.transform, ringGripState.gameObject.transform, pinkyGripState.gameObject.transform);

                //Set Gripper Force
                if (useObjDynamicModel)
                {
                    if (EventManager_MultipointGrip.fsm.State == EventManager_MultipointGrip.EventManagerStates.Task201 ||
                        EventManager_MultipointGrip.fsm.State == EventManager_MultipointGrip.EventManagerStates.Task202 ||
                        EventManager_MultipointGrip.fsm.State == EventManager_MultipointGrip.EventManagerStates.Task203)
                            SetDynamicGripperForce(colObj.GetComponent<TouchableObject>().gripperForceRange);
                }
                else
                {
                    if(EventManager_MultipointGrip.fsm.State == EventManager_MultipointGrip.EventManagerStates.Task101 || 
                       EventManager_MultipointGrip.fsm.State == EventManager_MultipointGrip.EventManagerStates.Task102)
                            HapticNativePlugin.SetForceModelInfo(colObj.GetComponent<TouchableObject>().constGripperForce);
                }

                if(IsGripReleased(gripExtent[2]))
                    isGripped = false;

                DataOutputManager.PrintGripExtent(gripExtent[2]);
            }
            //Finalize
            else
            {
                if (colObj != null)
                {
                    colObj.transform.parent = colObjOriginalParent;

                    HapticNativePlugin.UpdateObject(colObj.GetComponent<TouchableObject>().initHapticPos,
                                                    colObj.GetComponent<TouchableObject>().initHapticRot,
                                                    colObj.GetComponent<TouchableObject>().objectId);

                    HapticNativePlugin.SetIsGrip(false);
                    HapticNativePlugin.SetObjectHaptic(colObj.GetComponent<TouchableObject>().objectId, true);
                    setColObjToOriginTrans(colObj);

                    colObj = null;
                    state = gripMode.IDLE;

                    DataOutputManager.PrintGripFinishInfo();
                }
            }
        }
    }

    private void InitHandModel()
    {
        SetWholeHandInfo();
        SetWholeHandJointInfo();
        HapticNativePlugin.SetHandModelJointInfo(jointLengthIdx, jointDirIdx);

        isJointInfoSet = true;
        isTipsCollide = new bool[5];
        state = gripMode.IDLE;
    }

    private void SetTipPointsState()
    {
        isTipsCollide[0] = thumbGripState.isCollide;
        isTipsCollide[1] = fingerGripState.isCollide;
        isTipsCollide[2] = middleGripState.isCollide;
        isTipsCollide[3] = ringGripState.isCollide;
        isTipsCollide[4] = pinkyGripState.isCollide;
    }

    private void SetWholeHandInfo()
    {
        //Set Hand Trasnforms 
        handTrans = new Transform[handTransSize];
        handTrans[0] = this.transform;
        handTrans[1] = GameObject.Find("ThumbCollider").transform;
        handTrans[2] = GameObject.Find("FingerCollider").transform;
        handTrans[3] = GameObject.Find("MiddleCollider").transform;
        handTrans[4] = GameObject.Find("RingCollider").transform;
        handTrans[5] = GameObject.Find("PinkyCollider").transform;

        handTrans[6] = GameObject.Find("ThumbPIP_proxy").transform;
        handTrans[7] = GameObject.Find("ThumbMCP_proxy").transform;
        handTrans[8] = GameObject.Find("FingerDIP_proxy").transform;
        handTrans[9] = GameObject.Find("FingerPIP_proxy").transform;
        handTrans[10] = GameObject.Find("FingerMCP_proxy").transform;
        handTrans[11] = GameObject.Find("MiddleDIP_proxy").transform;
        handTrans[12] = GameObject.Find("MiddlePIP_proxy").transform;
        handTrans[13] = GameObject.Find("MiddleMCP_proxy").transform;
        handTrans[14] = GameObject.Find("RingDIP_proxy").transform;
        handTrans[15] = GameObject.Find("RingPIP_proxy").transform;
        handTrans[16] = GameObject.Find("RingMCP_proxy").transform;
        handTrans[17] = GameObject.Find("PinkyDIP_proxy").transform;
        handTrans[18] = GameObject.Find("PinkyPIP_proxy").transform;
        handTrans[19] = GameObject.Find("PinkyMCP_proxy").transform;

        handTrans[20] = GameObject.Find("PalmPoint1_proxy").transform;
        handTrans[21] = GameObject.Find("PalmPoint1-1_proxy").transform;
        handTrans[22] = GameObject.Find("ThumbPalmPoint_proxy").transform;
        handTrans[23] = GameObject.Find("PalmPoint2_proxy").transform;
        handTrans[24] = GameObject.Find("PalmPoint2-1_proxy").transform;
        handTrans[25] = GameObject.Find("FingerPalmPoint_proxy").transform;
        handTrans[26] = GameObject.Find("PalmPoint3_proxy").transform;
        handTrans[27] = GameObject.Find("PalmPoint3-1_proxy").transform;
        handTrans[28] = GameObject.Find("MiddlePalmPoint_proxy").transform;
        handTrans[29] = GameObject.Find("PalmPoint4_proxy").transform;
        handTrans[30] = GameObject.Find("PalmPoint4-1_proxy").transform;
        handTrans[31] = GameObject.Find("RingPalmPoint_proxy").transform;
        handTrans[32] = GameObject.Find("PalmPoint5_proxy").transform;
        handTrans[33] = GameObject.Find("PalmPoint5-1_proxy").transform;
        handTrans[34] = GameObject.Find("PinkyPalmPoint_proxy").transform;
        handTrans[35] = GameObject.Find("sleeve").transform;

        //Set init Hand angles
        initHandAngle = new Vector3[handTransSize];
        for (int i = 0; i < initHandAngle.Length; i++)
            initHandAngle[i] = handTrans[i].localEulerAngles;

        //Set each colliderPoints
        int colliderNum = 24;
        colliderPoints = new GameObject[colliderNum];

        colliderPoints[0] = GameObject.Find("ThumbCollider");
        colliderPoints[1] = GameObject.Find("ThumbPIPCollider");
        colliderPoints[2] = GameObject.Find("ThumbMCPCollider");
        colliderPoints[3] = GameObject.Find("ThumbPalmPointCollider");

        colliderPoints[4] = GameObject.Find("FingerCollider");
        colliderPoints[5] = GameObject.Find("FingerDIPCollider");
        colliderPoints[6] = GameObject.Find("FingerPIPCollider");
        colliderPoints[7] = GameObject.Find("FingerMCPCollider");
        colliderPoints[8] = GameObject.Find("FingerPalmPointCollider");

        colliderPoints[9] = GameObject.Find("MiddleCollider");
        colliderPoints[10] = GameObject.Find("MiddleDIPCollider");
        colliderPoints[11] = GameObject.Find("MiddlePIPCollider");
        colliderPoints[12] = GameObject.Find("MiddleMCPCollider");
        colliderPoints[13] = GameObject.Find("MiddlePalmPointCollider");

        colliderPoints[14] = GameObject.Find("RingCollider");
        colliderPoints[15] = GameObject.Find("RingDIPCollider");
        colliderPoints[16] = GameObject.Find("RingPIPCollider");
        colliderPoints[17] = GameObject.Find("RingMCPCollider");
        colliderPoints[18] = GameObject.Find("RingPalmPointCollider");

        colliderPoints[19] = GameObject.Find("PinkyCollider");
        colliderPoints[20] = GameObject.Find("PinkyDIPCollider");
        colliderPoints[21] = GameObject.Find("PinkyPIPCollider");
        colliderPoints[22] = GameObject.Find("PinkyMCPCollider");
        colliderPoints[23] = GameObject.Find("PinkyPalmPointCollider");

        thumbGripState = colliderPoints[0].GetComponent<CheckGripState>();
        fingerGripState = colliderPoints[4].GetComponent<CheckGripState>();
        middleGripState = colliderPoints[9].GetComponent<CheckGripState>();
        ringGripState = colliderPoints[14].GetComponent<CheckGripState>();
        pinkyGripState = colliderPoints[19].GetComponent<CheckGripState>();

        initRootAngleMatrix = Matrix4x4.Rotate(handTrans[0].transform.rotation);
    }
    
    private void SetWholeHandJointInfo()
    {
        jointLengthIdx = new double[jointLengthNum];
        jointDirIdx = new Vector3[jointLengthNum];
        int idx = 0, dirIdx = 0;

        //Set jointLength & jointDirIdx
        jointDirIdx[dirIdx] = (handTrans[0].position - handTrans[35].position).normalized; dirIdx++;
        jointLength.sleeveToRoot = CalcDoubleLength(handTrans[0].position, handTrans[35].position);

        jointDirIdx[dirIdx] = (handTrans[22].position - handTrans[0].position).normalized; dirIdx++;
        jointLength.rootToThumbPalm = CalcDoubleLength(handTrans[0].position, handTrans[22].position);

        jointDirIdx[dirIdx] = (handTrans[25].position - handTrans[0].position).normalized; dirIdx++;
        jointLength.rootToFingerPalm = CalcDoubleLength(handTrans[0].position, handTrans[25].position);

        jointDirIdx[dirIdx] = (handTrans[28].position - handTrans[0].position).normalized; dirIdx++;
        jointLength.rootToMiddlePalm = CalcDoubleLength(handTrans[0].position, handTrans[28].position);

        jointDirIdx[dirIdx] = (handTrans[31].position - handTrans[0].position).normalized; dirIdx++;
        jointLength.rootToRingPalm = CalcDoubleLength(handTrans[0].position, handTrans[31].position);

        jointDirIdx[dirIdx] = (handTrans[34].position - handTrans[0].position).normalized; dirIdx++;
        jointLength.rootToPinkyPalm = CalcDoubleLength(handTrans[0].position, handTrans[34].position);

        jointDirIdx[dirIdx] = (handTrans[7].position - handTrans[22].position).normalized; dirIdx++;
        jointLength.thumbPalmToMCP = CalcDoubleLength(handTrans[22].position, handTrans[7].position);

        jointDirIdx[dirIdx] = (handTrans[6].position - handTrans[7].position).normalized; dirIdx++;
        jointLength.thumbMCPToPIP = CalcDoubleLength(handTrans[7].position, handTrans[6].position);

        jointDirIdx[dirIdx] = (handTrans[1].position - handTrans[6].position).normalized; dirIdx++;
        jointLength.thumbPIPToTIP = CalcDoubleLength(handTrans[6].position, handTrans[1].position);

        jointDirIdx[dirIdx] = (handTrans[10].position - handTrans[25].position).normalized; dirIdx++;
        jointLength.fingerPalmToMCP = CalcDoubleLength(handTrans[25].position, handTrans[10].position);

        jointDirIdx[dirIdx] = (handTrans[9].position - handTrans[10].position).normalized; dirIdx++;
        jointLength.fingerMCPToPIP = CalcDoubleLength(handTrans[10].position, handTrans[9].position);

        jointDirIdx[dirIdx] = (handTrans[8].position - handTrans[9].position).normalized; dirIdx++;
        jointLength.fingerPIPToDIP = CalcDoubleLength(handTrans[9].position, handTrans[8].position);

        jointDirIdx[dirIdx] = (handTrans[2].position - handTrans[8].position).normalized; dirIdx++;
        jointLength.fingerDIPToTIP = CalcDoubleLength(handTrans[8].position, handTrans[2].position);

        jointDirIdx[dirIdx] = (handTrans[13].position - handTrans[28].position).normalized; dirIdx++;
        jointLength.middlePalmToMCP = CalcDoubleLength(handTrans[28].position, handTrans[13].position);

        jointDirIdx[dirIdx] = (handTrans[12].position - handTrans[13].position).normalized; dirIdx++;
        jointLength.middleMCPToPIP = CalcDoubleLength(handTrans[13].position, handTrans[12].position);

        jointDirIdx[dirIdx] = (handTrans[11].position - handTrans[12].position).normalized; dirIdx++;
        jointLength.middlePIPToDIP = CalcDoubleLength(handTrans[12].position, handTrans[11].position);

        jointDirIdx[dirIdx] = (handTrans[3].position - handTrans[11].position).normalized; dirIdx++;
        jointLength.middleDIPToTIP = CalcDoubleLength(handTrans[11].position, handTrans[3].position);

        jointDirIdx[dirIdx] = (handTrans[16].position - handTrans[31].position).normalized; dirIdx++;
        jointLength.ringPalmToMCP = CalcDoubleLength(handTrans[31].position, handTrans[16].position);

        jointDirIdx[dirIdx] = (handTrans[15].position - handTrans[16].position).normalized; dirIdx++;
        jointLength.ringMCPToPIP = CalcDoubleLength(handTrans[16].position, handTrans[15].position);

        jointDirIdx[dirIdx] = (handTrans[14].position - handTrans[15].position).normalized; dirIdx++;
        jointLength.ringPIPToDIP = CalcDoubleLength(handTrans[15].position, handTrans[14].position);

        jointDirIdx[dirIdx] = (handTrans[4].position - handTrans[14].position).normalized; dirIdx++;
        jointLength.ringDIPToTIP = CalcDoubleLength(handTrans[14].position, handTrans[4].position);

        jointDirIdx[dirIdx] = (handTrans[19].position - handTrans[34].position).normalized; dirIdx++;
        jointLength.pinkyPalmToMCP = CalcDoubleLength(handTrans[34].position, handTrans[19].position);

        jointDirIdx[dirIdx] = (handTrans[18].position - handTrans[19].position).normalized; dirIdx++;
        jointLength.pinkyMCPToPIP = CalcDoubleLength(handTrans[19].position, handTrans[18].position);

        jointDirIdx[dirIdx] = (handTrans[17].position - handTrans[18].position).normalized; dirIdx++;
        jointLength.pinkyPIPToDIP = CalcDoubleLength(handTrans[18].position, handTrans[17].position);

        jointDirIdx[dirIdx] = (handTrans[5].position - handTrans[17].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[17].position, handTrans[5].position);

        jointDirIdx[dirIdx] = (handTrans[21].position - handTrans[22].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[21].position, handTrans[22].position);

        jointDirIdx[dirIdx] = (handTrans[20].position - handTrans[21].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[20].position, handTrans[21].position);

        jointDirIdx[dirIdx] = (handTrans[24].position - handTrans[25].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[24].position, handTrans[25].position);

        jointDirIdx[dirIdx] = (handTrans[23].position - handTrans[24].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[23].position, handTrans[24].position);

        jointDirIdx[dirIdx] = (handTrans[27].position - handTrans[28].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[27].position, handTrans[28].position);

        jointDirIdx[dirIdx] = (handTrans[26].position - handTrans[27].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[26].position, handTrans[27].position);

        jointDirIdx[dirIdx] = (handTrans[30].position - handTrans[31].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[30].position, handTrans[31].position);

        jointDirIdx[dirIdx] = (handTrans[29].position - handTrans[30].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[29].position, handTrans[30].position);

        jointDirIdx[dirIdx] = (handTrans[33].position - handTrans[34].position).normalized; dirIdx++;
        jointLength.pinkyDIPToTIP = CalcDoubleLength(handTrans[33].position, handTrans[34].position);

        //Set jointLengthIdx
        jointLengthIdx[idx] = jointLength.sleeveToRoot; idx++;

        jointLengthIdx[idx] = jointLength.rootToThumbPalm; idx++;
        jointLengthIdx[idx] = jointLength.rootToFingerPalm; idx++;
        jointLengthIdx[idx] = jointLength.rootToMiddlePalm; idx++;
        jointLengthIdx[idx] = jointLength.rootToRingPalm; idx++;
        jointLengthIdx[idx] = jointLength.rootToPinkyPalm; idx++;

        jointLengthIdx[idx] = jointLength.thumbPalmToMCP; idx++;
        jointLengthIdx[idx] = jointLength.thumbMCPToPIP; idx++;
        jointLengthIdx[idx] = jointLength.thumbPIPToTIP; idx++;

        jointLengthIdx[idx] = jointLength.fingerPalmToMCP; idx++;
        jointLengthIdx[idx] = jointLength.fingerMCPToPIP; idx++;
        jointLengthIdx[idx] = jointLength.fingerPIPToDIP; idx++;
        jointLengthIdx[idx] = jointLength.fingerDIPToTIP; idx++;

        jointLengthIdx[idx] = jointLength.middlePalmToMCP; idx++;
        jointLengthIdx[idx] = jointLength.middleMCPToPIP; idx++;
        jointLengthIdx[idx] = jointLength.middlePIPToDIP; idx++;
        jointLengthIdx[idx] = jointLength.middleDIPToTIP; idx++;

        jointLengthIdx[idx] = jointLength.ringPalmToMCP; idx++;
        jointLengthIdx[idx] = jointLength.ringMCPToPIP; idx++;
        jointLengthIdx[idx] = jointLength.ringPIPToDIP; idx++;
        jointLengthIdx[idx] = jointLength.ringDIPToTIP; idx++;

        jointLengthIdx[idx] = jointLength.pinkyPalmToMCP; idx++;
        jointLengthIdx[idx] = jointLength.pinkyMCPToPIP; idx++;
        jointLengthIdx[idx] = jointLength.pinkyPIPToDIP; idx++;
        jointLengthIdx[idx] = jointLength.pinkyDIPToTIP; idx++;

        jointLengthIdx[idx] = jointLength.thumbPalmToPalm1_1; idx++;
        jointLengthIdx[idx] = jointLength.palm1_1ToPalm1; idx++;
        jointLengthIdx[idx] = jointLength.fingerPalmToPalm2_1; idx++;
        jointLengthIdx[idx] = jointLength.palm2_1ToPalm2; idx++;
        jointLengthIdx[idx] = jointLength.middlePalmToPalm3_1; idx++;
        jointLengthIdx[idx] = jointLength.palm3_1ToPalm3; idx++;
        jointLengthIdx[idx] = jointLength.ringPalmToPalm4_1; idx++;
        jointLengthIdx[idx] = jointLength.palm4_1ToPalm4; idx++;
        jointLengthIdx[idx] = jointLength.pinkyPalmToPalm5_1; idx++;
        jointLengthIdx[idx] = jointLength.palm5_1ToPalm5; idx++;
    }

    private double CalcDoubleLength(Vector3 v1, Vector3 v2)
    {
        //Vector3 is float...
        double doubleL = Math.Sqrt(((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z)));
        return doubleL;
    }
    
    private void SetWholeHandTransform(double gripperDiff)
    {
        // Root의 Transform 갱신
        handTrans[0].localPosition = new Vector3(HapticNativePlugin.GetProxyPosition(0).x, HapticNativePlugin.GetProxyPosition(0).y, HapticNativePlugin.GetProxyPosition(0).z);
        handTrans[0].transform.rotation = GetRootRotation();

        // Gripper의 일정 이상 변화 시 다른 Joint들도 갱신 
        if (!isGripped)
            //if (gripperDiff > 0.01)
                SetInnerJntAngle(gripperDiff);
    }

    private Quaternion GetRootRotation()
    {
        Matrix4x4 deviceGlobalRot = HapticNativePlugin.GetRootRotation();

        Vector4 v1 = new Vector4(1, 0, 0, 0);
        Vector4 v2 = new Vector4(0, 0, 1, 0);
        Vector4 v3 = new Vector4(0, 1, 0, 0);
        Vector4 v4 = new Vector4(0, 0, 0, 1);
        Matrix4x4 axisConversion = new Matrix4x4(v1, v2, v3, v4);

        Matrix4x4 convertXYZFromCHAI3D = axisConversion * deviceGlobalRot * axisConversion;

        Matrix4x4 rootRotAngle = convertXYZFromCHAI3D * initRootAngleMatrix;

        Quaternion q = new Quaternion();

        float trace = rootRotAngle[0, 0] + rootRotAngle[1, 1] + rootRotAngle[2, 2];
        if (trace > 0)
        {
            float s = 0.5f / Mathf.Sqrt(trace + 1.0f);
            q.w = 0.25f / s;
            q.x = (rootRotAngle[2, 1] - rootRotAngle[1, 2]) * s;
            q.y = (rootRotAngle[0, 2] - rootRotAngle[2, 0]) * s;
            q.z = (rootRotAngle[1, 0] - rootRotAngle[0, 1]) * s;
        }
        else
        {
            if (rootRotAngle[0, 0] > rootRotAngle[1, 1] && rootRotAngle[0, 0] > rootRotAngle[2, 2])
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + rootRotAngle[0, 0] - rootRotAngle[1, 1] - rootRotAngle[2, 2]);
                q.w = (rootRotAngle[2, 1] - rootRotAngle[1, 2]) / s;
                q.x = 0.25f * s;
                q.y = (rootRotAngle[0, 1] + rootRotAngle[1, 0]) / s;
                q.z = (rootRotAngle[0, 2] + rootRotAngle[2, 0]) / s;
            }
            else if (rootRotAngle[1, 1] > rootRotAngle[2, 2])
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + rootRotAngle[1, 1] - rootRotAngle[0, 0] - rootRotAngle[2, 2]);
                q.w = (rootRotAngle[0, 2] - rootRotAngle[2, 0]) / s;
                q.x = (rootRotAngle[0, 1] + rootRotAngle[1, 0]) / s;
                q.y = 0.25f * s;
                q.z = (rootRotAngle[1, 2] + rootRotAngle[2, 1]) / s;
            }
            else
            {
                float s = 2.0f * Mathf.Sqrt(1.0f + rootRotAngle[2, 2] - rootRotAngle[0, 0] - rootRotAngle[1, 1]);
                q.w = (rootRotAngle[1, 0] - rootRotAngle[0, 1]) / s;
                q.x = (rootRotAngle[0, 2] + rootRotAngle[2, 0]) / s;
                q.y = (rootRotAngle[1, 2] + rootRotAngle[2, 1]) / s;
                q.z = 0.25f * s;
            }
        }
        return q;
    }

    private void SetInnerJntAngle(double gripExtent)
    {
        /*
         * 1) handTrans index
         * 
         * Palm   : 22,25,28,31,34(Thumb to Pinky)
         * Thumb  : 7,6
         * Finger : 10,9,8(MCP, PIP, DIP)
         * Middle : 13,12,11
         * Ring   : 16,15,14
         * Pinky  : 19,18,17
         * 
         * 2) Rotation axis & angle
         * 
         * Palm   : 'Global Z'  10 -> 0 
         * MCP    : 'Global X'  0 -> -45  
         * PIP    : 'Global X'  0 -> -40  
         * DIP    : 'Global X'  0 -> -55  
         * 
         * 3) Grip Extent
         * 0.0108 ~ 0.499 (Default ~ 디바이스 최대 그립 시)
         */

        //각 Jnt의 초기 Local angle에 기반하여 회전.(Finger to Pinky)
        float maxGripExtent = 0.489f;
        float maxPalmAngle = 10, maxMCPAngle = 45, maxPIPAngle = 40, maxDIPAngle = 55;

        float palmRotAngle = (maxPalmAngle / maxGripExtent) * (float)(gripExtent - 0.01f);
        float mcpRotAngle =  (maxMCPAngle  / maxGripExtent) * (float)(gripExtent - 0.01f);
        float pipRotAngle =  (maxPIPAngle  / maxGripExtent) * (float)(gripExtent - 0.01f);
        float dipRotAngle =  (maxDIPAngle  / maxGripExtent) * (float)(gripExtent - 0.01f);

        if (useFullHandGrip)
        {
            //Middle
            handTrans[28].localEulerAngles = initHandAngle[28] - new Vector3(0, 0, palmRotAngle);
            handTrans[13].localEulerAngles = initHandAngle[13] - new Vector3(mcpRotAngle, 0, 0);
            handTrans[12].localEulerAngles = initHandAngle[12] - new Vector3(pipRotAngle, 0, 0);
            handTrans[11].localEulerAngles = initHandAngle[11] - new Vector3(dipRotAngle, 0, 0);

            //Ring
            handTrans[31].localEulerAngles = initHandAngle[31] - new Vector3(0, 0, palmRotAngle);
            handTrans[16].localEulerAngles = initHandAngle[16] - new Vector3(mcpRotAngle, 0, 0);
            handTrans[15].localEulerAngles = initHandAngle[15] - new Vector3(pipRotAngle, 0, 0);
            handTrans[14].localEulerAngles = initHandAngle[14] - new Vector3(dipRotAngle, 0, 0);

            //Pinky
            handTrans[34].localEulerAngles = initHandAngle[34] - new Vector3(0, 0, palmRotAngle);
            handTrans[19].localEulerAngles = initHandAngle[19] - new Vector3(mcpRotAngle, 0, 0);
            handTrans[18].localEulerAngles = initHandAngle[18] - new Vector3(pipRotAngle, 0, 0);
            handTrans[17].localEulerAngles = initHandAngle[17] - new Vector3(dipRotAngle, 0, 0);
        }

        //Finger
        handTrans[25].localEulerAngles = initHandAngle[25] - new Vector3(0, 0, palmRotAngle);
        handTrans[10].localEulerAngles = initHandAngle[10] - new Vector3(mcpRotAngle, 0, 0);
        handTrans[9].localEulerAngles = initHandAngle[9] - new Vector3(pipRotAngle, 0, 0);
        handTrans[8].localEulerAngles = initHandAngle[8] - new Vector3(dipRotAngle, 0, 0);

        //Thumb
        mcpRotAngle = (float)((20 / 0.489) * (gripExtent - 0.01));
        pipRotAngle = (float)((8 / 0.489) * (gripExtent - 0.01));
        handTrans[7].localEulerAngles = initHandAngle[7] - new Vector3(mcpRotAngle, 0, 0);
        handTrans[6].localEulerAngles = initHandAngle[6] - new Vector3(pipRotAngle, 0, 0);
    }

    private void CheckJntProjToSurf(Vector3 projDir)
    {
        float maxMCPRotAngle = 30;
        float maxPIPRotAngle = 20;
        float maxDIPRotAngle = 10;

        //MiddleFinger 
        IsProjJntCollide(handTrans[11].localEulerAngles, maxMCPRotAngle, 11, 3, projDir);
        IsProjJntCollide(handTrans[12].localEulerAngles, maxPIPRotAngle, 12, 3, projDir);
        IsProjJntCollide(handTrans[13].localEulerAngles, maxDIPRotAngle, 13, 3, projDir);

        //RingFinger 
        IsProjJntCollide(handTrans[14].localEulerAngles, maxMCPRotAngle, 14, 4, projDir);
        IsProjJntCollide(handTrans[15].localEulerAngles, maxPIPRotAngle, 15, 4, projDir);
        IsProjJntCollide(handTrans[16].localEulerAngles, maxDIPRotAngle, 16, 4, projDir);

        //Pinky 
        IsProjJntCollide(handTrans[17].localEulerAngles, maxMCPRotAngle, 17, 5, projDir);
        IsProjJntCollide(handTrans[18].localEulerAngles, maxPIPRotAngle, 18, 5, projDir);
        IsProjJntCollide(handTrans[19].localEulerAngles, maxDIPRotAngle, 19, 5, projDir);

    }

    private bool IsProjJntCollide(Vector3 initAngle, float maxAngle, int transIdx, int tipIdx, Vector3 projDir)
    {
        bool isCollide = false;
        float iterNum = 30;

        Rigidbody tipRigid = handTrans[tipIdx].GetComponent<Rigidbody>();
        RaycastHit hit;
        float collisionCheckDistance = 0.001f;

        for (float i = 0; i < iterNum; i++)
        {
            handTrans[transIdx].localEulerAngles = handTrans[transIdx].localEulerAngles - new Vector3(maxAngle / iterNum, 0, 0);

            if (tipRigid.SweepTest(projDir, out hit, collisionCheckDistance))
            {
                SetColliderPoint(transIdx);
                isCollide = true;
                break;
            }
        }

        if (!isCollide)
            handTrans[transIdx].localEulerAngles = initAngle;

        return isCollide;
    }

    private void SetColliderPoint(int transIdx)
    {
        switch (transIdx)
        {
            case 11:
                colliderPoints[9].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 12:
                colliderPoints[10].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 13:
                colliderPoints[11].GetComponent<CheckGripState>().isCollide = true;
                break;

            case 14:
                colliderPoints[14].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 15:
                colliderPoints[15].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 16:
                colliderPoints[16].GetComponent<CheckGripState>().isCollide = true;
                break;

            case 17:
                colliderPoints[19].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 18:
                colliderPoints[20].GetComponent<CheckGripState>().isCollide = true;
                break;
            case 19:
                colliderPoints[21].GetComponent<CheckGripState>().isCollide = true;
                break;
        }
    }

    private void SetDynamicGripperForce(double gripperForceRange)
    {
        double gripperForce = colObj.GetComponent<TouchableObject>().CalcPendulumDynamic(useFullHandGrip, useForceWithGripShape, gripperForceRange);

        HapticNativePlugin.SetForceModelInfo(gripperForce);

    }

    private bool IsGripReleased(double curGripExtent)
    {
        double gripChangeBound = 0.15;
        double minGripperAngle = 0.02;

        if ((initGripExtent - curGripExtent) > gripChangeBound)
            return true;
        if (curGripExtent < minGripperAngle)
            return true;

        return false;
    }

    private void setColObjToOriginTrans(GameObject colObj)
    {
        colObj.transform.position = colObj.GetComponent<TouchableObject>().initPos;
        colObj.transform.rotation = colObj.GetComponent<TouchableObject>().initRot;
        colObj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }

    public void SetModelToUse(bool _useFullHandGrip, bool _useObjDynamicModel, bool _useForceWithGripShape)
    {
        useFullHandGrip = _useFullHandGrip;
        useObjDynamicModel = _useObjDynamicModel;
        useForceWithGripShape = _useForceWithGripShape;
    }

    public void FinalizeTask()
    {
        if (isHapticAvail)
        {
            isGripped = false;

            HapticNativePlugin.SetIsGrip(false);
            thumbGripState.isCollide = false;
            fingerGripState.isCollide = false;
            state = gripMode.IDLE;
        }
    }

    void OnDestroy()
    {
        if (isHapticAvail)
            HapticNativePlugin.stopHaptics();
    }

    void DisplayCollisionPointsForDebugging()
    {
        collisionPointsForDebugging = new GameObject[colliderPoints.Length];

        for (int i = 0; i < colliderPoints.Length; i++)
            if (colliderPoints[i].GetComponent<CheckGripState>().isCollide)
                collisionPointsForDebugging[i] = colliderPoints[i];
    }
    //private IEnumerator PrintCurJointLength()
    //{
    //    yield return new WaitForSeconds(2);

    //    //Update 이후 joint Length 출력(palm내부 포인트들 업데이트 전, 즉 25개만 출력)
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[35].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[22].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[25].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[28].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[31].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[0].position, handTrans[34].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[22].position, handTrans[7].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[7].position, handTrans[6].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[6].position, handTrans[1].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[25].position, handTrans[10].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[10].position, handTrans[9].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[9].position, handTrans[8].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[8].position, handTrans[2].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[28].position, handTrans[13].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[13].position, handTrans[12].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[12].position, handTrans[11].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[11].position, handTrans[3].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[31].position, handTrans[16].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[16].position, handTrans[15].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[15].position, handTrans[14].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[14].position, handTrans[4].position));

    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[34].position, handTrans[19].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[19].position, handTrans[18].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[18].position, handTrans[17].position));
    //    streamWriter.WriteLine(CalcDoubleLength(handTrans[17].position, handTrans[5].position));

    //    double[] getHandLength = HapticNativePlugin.SendBackDebugSignal("1");

    //    streamWriter.WriteLine("------------------------");
    //    for (int i = 0; i < getHandLength.Length; i++)
    //        streamWriter.WriteLine(getHandLength[i]);
    //}
}
