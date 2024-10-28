using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class HapticNativePlugin
{
    [DllImport("UnityChai3dBridge")]
    public static extern bool prepareHaptics(double hapticScale);

    [DllImport("UnityChai3dBridge")]
    public static extern void startHaptics();

    [DllImport("UnityChai3dBridge")]
    public static extern void stopHaptics();

    [DllImport("UnityChai3dBridge")]
    protected static extern void getProxyPosition(double[] array, int handIdx, double[] temp);

    public static Vector4 GetProxyPosition(int handIdx)
    {
        double[] temp = new double[1];

        double[] arrayToUse = new double[3];
        getProxyPosition(arrayToUse, handIdx, temp);
        return new Vector4((float)arrayToUse[0], (float)arrayToUse[1], (float)arrayToUse[2], (float)temp[0]);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void getDevicePosition(double[] array, int handIdx);
    public static Vector3 GetDevicePosition(int handIdx)
    {
        double[] arrayToUse = new double[3];
        getDevicePosition(arrayToUse, handIdx);
        return new Vector3((float)arrayToUse[0], (float)arrayToUse[1], (float)arrayToUse[2]);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void addObject(double[] objectPos, double[] objectScale, double[] objectRotation, double[,] vertPos, double[,] normals, int vertNum, int[,] tris, int triNum, int objectCount);
    private static int objectCount = 0;
    public static int AddObject(Vector3 position, Vector3 scale, Vector3 rotation, Vector3[] vertPos, Vector3[] normals, int vertNum, int[,] tris, int triNum)
    {
        double[] objectPosition = new double[3];
        objectPosition[0] = (double)position.x;
        objectPosition[1] = (double)position.y;
        objectPosition[2] = (double)position.z;

        double[] objectScale = new double[3];
        objectScale[0] = (double)scale.x;
        objectScale[1] = (double)scale.y;
        objectScale[2] = (double)scale.z;

        double[] objectRotation = new double[3];
        objectRotation[0] = (double)rotation.x;
        objectRotation[1] = (double)rotation.y;
        objectRotation[2] = (double)rotation.z;

        double[,] objectVertPos = new double[vertNum, 3];
        for (int i = 0; i < vertNum; i++)
        {
            objectVertPos[i, 0] = (double)vertPos[i].x;
            objectVertPos[i, 1] = (double)vertPos[i].y;
            objectVertPos[i, 2] = (double)vertPos[i].z;
        }

        double[,] objectNormals = new double[vertNum, 3];
        for (int i = 0; i < vertNum; i++)
        {
            objectNormals[i, 0] = (double)normals[i].x;
            objectNormals[i, 1] = (double)normals[i].y;
            objectNormals[i, 2] = (double)normals[i].z;
        }
        objectCount++;

        addObject(objectPosition, objectScale, objectRotation, objectVertPos, objectNormals, vertNum, tris, triNum, objectCount);

        return (objectCount);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void updateObject(double[] objectPos, double[] objectRotation, int objectCount);
    public static void UpdateObject(Vector3 position, Vector3 rotation, int objectCount)
    {
        double[] objectPosition = new double[3];
        objectPosition[0] = (double)position.x;
        objectPosition[1] = (double)position.y;
        objectPosition[2] = (double)position.z;

        double[] objectRotation = new double[3];
        objectRotation[0] = (double)rotation.x;
        objectRotation[1] = (double)rotation.y;
        objectRotation[2] = (double)rotation.z;

        updateObject(objectPosition, objectRotation, objectCount);

    }


    [DllImport("UnityChai3dBridge")]
    protected static extern void setObjectHaptic(int objectCount, bool isHaptic);
    public static void SetObjectHaptic(int objectCount, bool isHaptic)
    {
        setObjectHaptic(objectCount, isHaptic);
    }


    [DllImport("UnityChai3dBridge")]
    protected static extern void getRootRotation(double[] array);
    public static Matrix4x4 GetRootRotation()
    {
        double[] arrayToUse = new double[9];
        getRootRotation(arrayToUse);

        Vector4 v0 = new Vector4((float)arrayToUse[0], (float)arrayToUse[1], (float)arrayToUse[2], 0);
        Vector4 v1 = new Vector4((float)arrayToUse[3], (float)arrayToUse[4], (float)arrayToUse[5], 0);
        Vector4 v2 = new Vector4((float)arrayToUse[6], (float)arrayToUse[7], (float)arrayToUse[8], 0);
        Vector4 v3 = new Vector4(0, 0, 0, 1);

        Matrix4x4 m = new Matrix4x4(v0, v1, v2, v3);
        
        return m;
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void setHandModelJointInfo(double[] jointLength, int lengthNum, double[,] jointDir, int dirNum);
    public static void SetHandModelJointInfo(double[] jointLength, Vector3[] jointDir)
    {
        int jointDirNum = jointDir.Length;

        double[,] dirInfo = new double[jointDirNum, 3];
        for (int i = 0; i < jointDirNum; i++)
        {
            dirInfo[i, 0] = jointDir[i].x;
            dirInfo[i, 1] = jointDir[i].y;
            dirInfo[i, 2] = jointDir[i].z;
        }

        setHandModelJointInfo(jointLength, jointLength.Length, dirInfo, jointDirNum);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void sendDebugSignal(int later);
    public static void SendDebugSignal(string str)
    {
        int tmp = 0;
        sendDebugSignal(tmp);
        Debug.Log("SEND : " + str);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void sendBackDebugSignal(double[] result);
    public static double[] SendBackDebugSignal(string str)
    {
        double[] arrayToUse = new double[10];
        sendBackDebugSignal(arrayToUse);

        ////
        Vector3 v1 = new Vector3((float)arrayToUse[4], (float)arrayToUse[5], (float)arrayToUse[6]);
        Vector3 v2 = new Vector3((float)arrayToUse[7], (float)arrayToUse[8], (float)arrayToUse[9]);

        double doubleL = Math.Sqrt( (arrayToUse[4] - arrayToUse[7]) * (arrayToUse[4] - arrayToUse[7]) + 
                                    (arrayToUse[5] - arrayToUse[8]) * (arrayToUse[5] - arrayToUse[8]) + 
                                    (arrayToUse[6] - arrayToUse[9]) * (arrayToUse[6] - arrayToUse[9]));


        Debug.Log("Double length : " + doubleL);


        return arrayToUse;
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void getChangedGripExtent(double[] changedExtent);
    public static double[] GetChangedGripExtent()
    {
        /*
         * changedExtent[0] = abs(initRevertedGripperAngle - tool->revertedGripperAngle);
		 * changedExtent[1] = initRevertedGripperAngle;
		 * changedExtent[2] = tool->revertedGripperAngle;
         */

        double[] changedExtent = new double[3];
        for (int i = 0; i < changedExtent.Length; i++)
            changedExtent[i] = 0;

        getChangedGripExtent(changedExtent);
        return changedExtent;
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void setIsGrip(bool isGrip);
    public static void SetIsGrip(bool isGrip)
    {
        setIsGrip(isGrip);
    }

    [DllImport("UnityChai3dBridge")]
    protected static extern void setForceModelInfo(double gripperForce);
    public static void SetForceModelInfo(double gripperForce)
    {
        setForceModelInfo(gripperForce);
    }

}