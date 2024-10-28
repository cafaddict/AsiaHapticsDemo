using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Threading;

public class HapticManager : MonoBehaviour
{
    [SerializeField] private int handIdx = -1;

    private void Awake()
    {
        //SetHandIdx();
    }

    private void Update()
    {
    }

    private void SetHandIdx()
    {
        string[] fingerName = this.name.Split('_');

        if (this.name.Equals("root"))
            handIdx = 0;
        else if (fingerName[0].Equals("Thumb"))
            handIdx = 1;
        else if (fingerName[0].Equals("Finger"))
            handIdx = 2;
        else if (fingerName[0].Equals("Middle"))
            handIdx = 3;
        else if (fingerName[0].Equals("Ring"))
            handIdx = 4;
        else if (fingerName[0].Equals("Pinky"))
            handIdx = 5;

        else if (fingerName[0].Equals("ThumbPIP"))
            handIdx = 6;
        else if (fingerName[0].Equals("ThumbMCP"))
            handIdx = 7;

        else if (fingerName[0].Equals("FingerDIP"))
            handIdx = 8;
        else if (fingerName[0].Equals("FingerPIP"))
            handIdx = 9;
        else if (fingerName[0].Equals("FingerMCP"))
            handIdx = 10;

        else if (fingerName[0].Equals("MiddleDIP"))
            handIdx = 11;
        else if (fingerName[0].Equals("MiddlePIP"))
            handIdx = 12;
        else if (fingerName[0].Equals("MiddleMCP"))
            handIdx = 13;

        else if (fingerName[0].Equals("RingDIP"))
            handIdx = 14;
        else if (fingerName[0].Equals("RingPIP"))
            handIdx = 15;
        else if (fingerName[0].Equals("RingMCP"))
            handIdx = 16;

        else if (fingerName[0].Equals("PinkyDIP"))
            handIdx = 17;
        else if (fingerName[0].Equals("PinkyPIP"))
            handIdx = 18;
        else if (fingerName[0].Equals("PinkyMCP"))
            handIdx = 19;

        else if (fingerName[0].Equals("PalmPoint1"))
            handIdx = 20;
        else if (fingerName[0].Equals("PalmPoint1-1"))
            handIdx = 21;
        else if (fingerName[0].Equals("ThumbPalmPoint"))
            handIdx = 22;

        else if (fingerName[0].Equals("PalmPoint2"))
            handIdx = 23;
        else if (fingerName[0].Equals("PalmPoint2-1"))
            handIdx = 24;
        else if (fingerName[0].Equals("FingerPalmPoint"))
            handIdx = 25;

        else if (fingerName[0].Equals("PalmPoint3"))
            handIdx = 26;
        else if (fingerName[0].Equals("PalmPoint3-1"))
            handIdx = 27;
        else if (fingerName[0].Equals("MiddlePalmPoint"))
            handIdx = 28;

        else if (fingerName[0].Equals("PalmPoint4"))
            handIdx = 29;
        else if (fingerName[0].Equals("PalmPoint4-1"))
            handIdx = 30;
        else if (fingerName[0].Equals("RingPalmPoint"))
            handIdx = 31;

        else if (fingerName[0].Equals("PalmPoint5"))
            handIdx = 32;
        else if (fingerName[0].Equals("PalmPoint5-1"))
            handIdx = 33;
        else if (fingerName[0].Equals("PinkyPalmPoint"))
            handIdx = 34;
        else if (this.name.Equals("sleeve"))
            handIdx = 35;
        else
            handIdx = 100;
    }
}