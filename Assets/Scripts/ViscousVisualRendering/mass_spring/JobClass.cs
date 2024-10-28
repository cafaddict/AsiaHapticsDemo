using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
public class Job : ThreadedJob
{
    public Vector3[] InData;  // arbitary job data
    public Vector3[] OutData; // arbitary job data

    public int executioncount = 0;


    public List<MassClass> InMass = new List<MassClass>();
    public List<MassClass> OutMass;

    public List<SpringClass> InSpring = new List<SpringClass>();
    public List<SpringClass> OutSpring;


    public float stifness;
    public int num_node;
    public float damp;
    public float addingforce;

    public float deltaTimeSec;
    float addingforce_delta = 0;

    private void letsupdate()
    {
        if(addingforce_delta <= addingforce)
        {
            addingforce_delta += addingforce * deltaTimeSec;
        }
        else
        {
            addingforce_delta = addingforce;
        }
        InMass[0].massaddForce(new Vector3(0, 0, 0));
        InMass[0].is_fixed = true;

        InMass[1].massaddForce(new Vector3(0, 0, 0));
        InMass[1].is_fixed = true;

        InMass[2].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        InMass[3].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        InMass[4].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        InMass[5].massaddForce(new Vector3(0, -(addingforce_delta), 0));

        //InMass[0+6].massaddForce(new Vector3(0, 0, 0));
        //InMass[0+6].is_fixed = true;

        //InMass[1+6].massaddForce(new Vector3(0, 0, 0));
        //InMass[1+6].is_fixed = true;

        //InMass[2+6].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        //InMass[3+6].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        //InMass[4+6].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        //InMass[5+6].massaddForce(new Vector3(0, -(addingforce_delta), 0));
        for (int i = 0; i< InMass.Count; i++)
        {
            InMass[i].massupdate();
        }
        //for (int i = 0; i < InMass.Count; i++)
        //{
        //    if ((i % num_node) < (num_node / 2))
        //    {
        //        InMass[i].massaddForce(new Vector3(-(addingforce), 0, 0));
        //    }
        //    else
        //    {
        //        InMass[i].massaddForce(new Vector3(addingforce, 0, 0));
        //    }
        //    InMass[i].massupdate();
        //    //			Debug.Log ("MassForce = " + InMass [i].massforce);

        //}

        for (int i = 0; i < InSpring.Count; i++)
        {
            SpringClass spring = InSpring[i];

            if (spring.cutted == false)
            {
                MassClass m1 = spring.m1;
                MassClass m2 = spring.m2;
                spring.updateValues();

                Vector3 dir12 = spring.springdir;
                float o_length = spring.original_spring_length;
                float n_length = spring.new_spring_length;
                float cte = stifness * (n_length - o_length);

                Vector3 force1;
                force1.x = cte * dir12.x;
                force1.y = cte * dir12.y;
                force1.z = cte * dir12.z;

                Vector3 force2 = -force1;
                //Vector3 addingforce;
                //Vector3 addingforce2;

                if(m1.is_fixed)
                {
                    //addingforce.x = 0;
                    //addingforce.y = 0;
                    //addingforce.z = 0;
                    m1.massforce.x = 0;
                    m1.massforce.y = 0;
                    m1.massforce.z = 0;
                }
                else
                {

                    //addingforce.x = -force1.x - damp * m1.velocity.x;
                    //addingforce.y = -force1.y - damp * m1.velocity.y;
                    //addingforce.z = -force1.z - damp * m1.velocity.z;

                    m1.massforce.x = -force1.x - damp * m1.velocity.x;
                    m1.massforce.y = -force1.y - damp * m1.velocity.y;
                    m1.massforce.z = -force1.z - damp * m1.velocity.z;
                }

                if (m2.is_fixed)
                {
                    //addingforce2.x = 0;
                    //addingforce2.y = 0;
                    //addingforce2.z = 0;
                    m2.massforce.x = 0;
                    m2.massforce.y = 0;
                    m2.massforce.z = 0;
                }
                else
                {
                    m2.massforce.x = -force2.x - damp * m2.velocity.x;
                    m2.massforce.y = -force2.y - damp * m2.velocity.y;
                    m2.massforce.z = -force2.z - damp * m2.velocity.z;
                    //addingforce2.x = -force2.x - damp * m2.velocity.x;
                    //addingforce2.y = -force2.y - damp * m2.velocity.y;
                    //addingforce2.z = -force2.z - damp * m2.velocity.z;
                }


                if(!m1.is_fixed)
                {
                    //Debug.Log("here?\n");
                    //m1.massaddForce(addingforce);
                    m1.massupdate();
                }
                if(!m2.is_fixed)
                {
                    //m2.massaddForce(addingforce2);
                    m2.massupdate();
                }
                

                //if (Math.Abs(m1.massforce.y) > spring.threashold || Math.Abs(m2.massforce.y) > spring.threashold)
                //{
                //    spring.cut();
                //}
                if(n_length > spring.threashold)
                {
                    spring.cut();
                }



            }
        }
    }

    protected override void ThreadFunction()
    {


        while (true)
        {
            letsupdate();
            executioncount++;
            if(IsDone)
            {
                break;
            }
        }


        //

        // Do your threaded task. DON'T use the Unity API here
        //		while (true) {
        //			Debug.Log("I'm Here");
        //			for (int i = 0; i < 50; i++)
        //			{
        //				InData [0] = new Vector3 (i, 0, 0);
        //				Thread.Sleep (10);
        ////				OutData[i % OutData.Length] += InData[(i+1) % InData.Length];
        //			}
        //		}




    }
    protected override void OnFinished()
    {

        //		this.Start ();
        // This is executed by the Unity main thread when the job is finished
        //		foreach (MassClass mass in InMass) {
        //			mass.Mass.MovePosition (mass.new_position);
        //		}
        //		for (int i = 0; i < InData.Length; i++)
        //		{
        //			Debug.Log("Results(" + i + "): " + InData[i]);
        //		}
    }


}