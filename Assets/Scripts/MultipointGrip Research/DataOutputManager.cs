using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataOutputManager : MonoBehaviour {

    private FileStream file;
    private FileStream file2;
    private static StreamWriter streamWriter;
    private static StreamWriter streamWriter2;
    private static float taskInitTime;

    void Start () {
        file = new FileStream("D:\\multiPointGripOutput.txt", FileMode.Create, FileAccess.ReadWrite);
        streamWriter = new StreamWriter(file);

        file2 = new FileStream("D:\\multiPointGripExtentOutput.txt", FileMode.Create, FileAccess.ReadWrite);
        streamWriter2 = new StreamWriter(file2);
    }

    public static void InitToSavingData(string taskNum)
    {
        streamWriter.WriteLine("\n" + taskNum);
        streamWriter.WriteLine("------------------------");

        taskInitTime = Time.time;       
    }

    public static void PrintElapsedTime()
    {
        streamWriter.WriteLine("");
        streamWriter.Write("Elapsed Time : ");
        streamWriter.WriteLine(Time.time - taskInitTime + "s");
        
        streamWriter.WriteLine("------------------------");
    }

    public static void PrintMixtureData(float[] mixtureData)
    {
        streamWriter.WriteLine("");
        streamWriter.Write("Filled Amount : ");
        for (int i=0; i<mixtureData.Length; i++)
        {
            if (i == mixtureData.Length / 2)
            {
                streamWriter.WriteLine("");
                streamWriter.Write("Goal Amount : ");
            }

            streamWriter.Write(mixtureData[i] + " ");
            
        }
        streamWriter.WriteLine("");
    }

    public static void PrintObjGrippedCnt(int[] grippedCnt)
    {
        streamWriter.WriteLine("");
        streamWriter.WriteLine("Gripped Count of each Object");

        for (int i = 0; i < grippedCnt.Length; i++)
        {
            streamWriter.Write("Object" + (i + 1) + " : " + grippedCnt[i]);
            streamWriter.WriteLine();
        }
    }

    public static void PrintConstGripperForces(double[] constGripperForces)
    {
        /////////////////////////////////////////////
        //// 랜덤으로 섞인 Const Gripper Force들은
        //// EventModule의 Task Obj Element 순서대로 할당된다.
        //// EventManager의 SetConstGripperForceToObj 참조

        streamWriter.WriteLine("Const Gripper Forces");

        for (int i=0; i<constGripperForces.Length; i++)
        {
            streamWriter.Write("Object"+(i+1) + " : " + constGripperForces[i]);
            streamWriter.WriteLine("");
        }
    }

    public static void PrintGripperForceRanges(double[] gripperForceRanges)
    {
        /////////////////////////////////////////////
        streamWriter.WriteLine("Gripper Force Ranges");

        for (int i = 0; i < gripperForceRanges.Length; i++)
        {
            streamWriter.Write("Object" + (i + 1) + " : " + gripperForceRanges[i]);
            streamWriter.WriteLine("");
        }
    }

    public static void PrintInitGripInfo(string objName, double gripExtent)
    {
        streamWriter2.Write(EventManager_MultipointGrip.fsm.State + " ");
        streamWriter2.WriteLine("Gripped Object : " + objName);
        streamWriter2.WriteLine("Init Grip Extent : " + gripExtent);     
    }

    public static void PrintCollisionPoints(GameObject[] colliderPoints, string projToSurf)
    {
        streamWriter2.Write("Gripped Points " + projToSurf + ": ");

        string[] seperator = new string[] { "Collider" };

        for (int i = 0; i < colliderPoints.Length; i++)
            if (colliderPoints[i].GetComponent<CheckGripState>().isCollide)
            {
                string name = colliderPoints[i].name.Split(seperator, System.StringSplitOptions.None)[0];
                streamWriter2.Write(name + " ");
            }
        streamWriter2.WriteLine("");
    }

    public static void PrintGripExtent(double gripExtent)
    {
        streamWriter2.WriteLine(gripExtent + " ");
    }

    public static void PrintGripFinishInfo()
    {
        streamWriter2.WriteLine("---------------------------------");
    }

    void OnDestroy()
    {
        streamWriter.Close();
        streamWriter2.Close();
        file.Close();
        file2.Close();
    }
}
