using UnityEngine;
using System.Collections;

public class TouchableObject : MonoBehaviour {
    
    public int objectId;
    public GameObject colObj;

    [HideInInspector] public Vector3 initPos;
    [HideInInspector] public Quaternion initRot;
    [HideInInspector] public Vector3 initHapticPos, initHapticRot;
    [HideInInspector] public Vector3 pivotPos;
    [HideInInspector] public int grippedCnt = 0;

    public static double maxGripperForce = 2.8;
    public double constGripperForce;
    public double gripperForceRange;

    private Vector3 CoM;
    private Rigidbody rigidBody;
    private Transform thumbTrans, fingerTrans, middleTrans, ringTrans, pinkyTrans;
    private GameObject hapticOrigin;
    private int tipColCnt = 0;

    void Start () {

        if (HandManager.isHapticAvail)
        {
            SetObjectInfo();

            initPos = transform.position;
            initRot = transform.rotation;
            initHapticPos = transform.localPosition - hapticOrigin.transform.localPosition;
            initHapticRot = transform.localRotation.eulerAngles;
        }
        rigidBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        CoM = transform.TransformPoint(rigidBody.centerOfMass);
    }

    private void SetObjectInfo()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[,] triangles = new int[(mesh.triangles.Length / 3), 3];

        for (int i = 0; i < mesh.triangles.Length / 3; i++)
        {
            triangles[i, 0] = mesh.triangles[3 * i];
            triangles[i, 1] = mesh.triangles[3 * i + 1];
            triangles[i, 2] = mesh.triangles[3 * i + 2];
        }

        hapticOrigin = GameObject.Find("Haptic Origin");
        objectId = HapticNativePlugin.AddObject(transform.localPosition - hapticOrigin.transform.localPosition, transform.localScale, transform.localRotation.eulerAngles, vertices, normals, mesh.vertices.Length, triangles, mesh.triangles.Length / 3);
        
    }

    public void SetCollideTipsCnt(bool[] isTipsCollide)
    {
        tipColCnt = 0;
        for (int i = 0; i < isTipsCollide.Length; i++)
            if (isTipsCollide[i])
                tipColCnt += 1;
    }

    public void SetHandTipsTrans(Transform _thumbTrans, Transform _fingerTrans, Transform _middleTrans, Transform _ringTrans, Transform _pinkyTrans)
    {
        thumbTrans  = _thumbTrans;
        fingerTrans = _fingerTrans;
        middleTrans = _middleTrans;
        ringTrans   = _ringTrans;
        pinkyTrans  = _pinkyTrans;

        pivotPos = (thumbTrans.position + fingerTrans.position) / 2;
    }

    public double CalcPendulumDynamic(bool useFullHandGrip, bool useForceWithGripShape, double gripperForceRange)
    {
        float length = (CoM - pivotPos).magnitude;
        float mass = 2f;//in case of beaker

        Vector3 v1 = transform.up;
        Vector3 v2 = Vector3.up; //global Axis
        float angle = Mathf.Acos(Vector3.Dot(v1, v2) / (v1.magnitude * v2.magnitude)); //radian

        double pendulumTorque = (mass * 9.8) * length * Mathf.Sin(angle);

        ////////////////////////////
        //Force Variation with Grip Shape
        double finalTorque = 0;

        if (useForceWithGripShape)
        {
            //if (!useFullHandGrip)
            //    tipColCnt = 2;

            //float frictionCoeff = 0.5f;
            //float massFingerTip = 0.04f;
            //float friction = frictionCoeff * (9.8f * massFingerTip) * tipColCnt * 0.3f;

            ////Shear Force
            //finalTorque = pendulumTorque - friction;

            ////Normal Force
            //finalTorque = finalTorque - (0.1f * finalTorque) * tipColCnt;
        }
        else
            finalTorque = pendulumTorque;
        ////////////////////////////
        double maxGripperForce = constGripperForce + gripperForceRange;

        double maxTorque = (mass * 9.8) * length * Mathf.Sin(Mathf.Deg2Rad * 90);
        double gripperForce = ((maxGripperForce - constGripperForce) / maxTorque) * finalTorque + constGripperForce;

        Debug.Log(" constGripperForce : " + constGripperForce + " gripperForce : " + gripperForce + " angle : " + angle * Mathf.Rad2Deg);
        
        return gripperForce;
    }

    public void IncGrippedCnt()
    {
        grippedCnt++;
    }

    void OnTriggerEnter(Collider col)
    {
        colObj = col.gameObject;
    }
    void OnTriggerExit(Collider col)
    {
        colObj = null;
    }
}
