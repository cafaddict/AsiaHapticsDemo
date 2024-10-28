using System.Collections;
using UnityEngine;

public class DataGenerationManager : MonoBehaviour {

    public static double[] gripperForceRange_task2_1;
    public static double[] gripperForceRange_task2_2;
    public static double[] gripperForceRange_task2_3;

    public static double[] GetConstGripperForces(string taskNum)
    {
        
        double[] constGripperForces = null;
        int beakerNum;

        switch (taskNum)
        {
            case "1_1":
                beakerNum = 3;
                constGripperForces = new double[beakerNum];
                constGripperForces[0] = 0.4;
                constGripperForces[1] = 0.8;
                constGripperForces[2] = 1.6;
                Shuffle(constGripperForces);
                DataOutputManager.PrintConstGripperForces(constGripperForces);
                break;

            case "1_2":
                beakerNum = 3;
                constGripperForces = new double[beakerNum];
                constGripperForces[0] = 0;
                constGripperForces[1] = 1.2;
                constGripperForces[2] = 2.2;
                Shuffle(constGripperForces);
                DataOutputManager.PrintConstGripperForces(constGripperForces);
                break;

            case "2_1":
                beakerNum = 3;
                constGripperForces = new double[beakerNum];
                constGripperForces[0] = 0.4;
                constGripperForces[1] = 1.6;
                constGripperForces[2] = 1.6;

                gripperForceRange_task2_1 = new double[beakerNum];
                gripperForceRange_task2_1[0] = 1.2;
                gripperForceRange_task2_1[1] = 0.4;
                gripperForceRange_task2_1[2] = 1.2;
                Shuffle2(constGripperForces, gripperForceRange_task2_1);
                DataOutputManager.PrintConstGripperForces(constGripperForces);
                DataOutputManager.PrintGripperForceRanges(gripperForceRange_task2_1);
                break;

            case "2_2":
                beakerNum = 3;
                constGripperForces = new double[beakerNum];
                constGripperForces[0] = 1.2;
                constGripperForces[1] = 1.2;
                constGripperForces[2] = 2.2;

                gripperForceRange_task2_2 = new double[beakerNum];
                gripperForceRange_task2_2[0] = 0.8;
                gripperForceRange_task2_2[1] = 1;
                gripperForceRange_task2_2[2] = 0.6;
                Shuffle2(constGripperForces, gripperForceRange_task2_2);
                DataOutputManager.PrintConstGripperForces(constGripperForces);
                DataOutputManager.PrintGripperForceRanges(gripperForceRange_task2_2);
                break;

            case "2_3":
                beakerNum = 3;
                constGripperForces = new double[beakerNum];
                constGripperForces[0] = 0;
                constGripperForces[1] = 0.8;
                constGripperForces[2] = 0.8;

                gripperForceRange_task2_3 = new double[beakerNum];
                gripperForceRange_task2_3[0] = 1.4;
                gripperForceRange_task2_3[1] = 1;
                gripperForceRange_task2_3[2] = 2;
                Shuffle2(constGripperForces, gripperForceRange_task2_3);
                DataOutputManager.PrintConstGripperForces(constGripperForces);
                DataOutputManager.PrintGripperForceRanges(gripperForceRange_task2_3);
                break;

        }
        return constGripperForces;
    }
    
    private static void Shuffle(double[] array)
    {
        // Knuth shuffle algorithm
        for (int t = 0; t < array.Length; t++)
        {
            double tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;
        }
    }

    //Shuffle이후에도 constGripperForce와 gripperForceRange의 매칭을 유지하기 위한 함수.
    private static void Shuffle2(double[] array, double[] array2)
    {
        for (int t = 0; t < array.Length; t++)
        {
            double tmp = array[t];
            int r = Random.Range(t, array.Length);
            array[t] = array[r];
            array[r] = tmp;

            double tmp2 = array2[t];
            array2[t] = array2[r];
            array2[r] = tmp2;
        }
    }
}
