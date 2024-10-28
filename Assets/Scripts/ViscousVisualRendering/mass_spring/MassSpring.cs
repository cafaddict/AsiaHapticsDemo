using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


public class MassSpring : MonoBehaviour
{

    //Let's use Thread!!!!
    Job calcJob;

    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    Vector3[] newNormals;

    float[] u;
    float[] v;

    MeshFilter mf;
    Mesh mesh;
    MeshRenderer mesh_renderer;


    public static List<MassClass> massList = new List<MassClass>();
    public static List<SpringClass> springList = new List<SpringClass>();

    public static List<Rigidbody> RmassList = new List<Rigidbody>();



    public static List<GameObject> mockspringList = new List<GameObject>();

    public GameObject mockspring;

    public Rigidbody Mass;


    //public Text framrate;
    //public Text brittleness;
    static int q;

    static int threadExecutionCount = 0;
    static float prevTimeStamp = 0.000f;
    static double frameCount = 0;
    static double nextUpdate = 0.0;
    static double fps = 0.0;
    static double updateRate = 4.0;  // 4 updates per sec.

    //	static int num_spring = 1;

    public int num_node;
    public int num_spring;
    public float stiffness;
    public float damp;
    public float threashold;
    public float deltaTimesec;
    public float addingforce;

    int N = 3;
    int M = 2;
    int K = 1;


    void Start()
    {



        q = num_node;

        nextUpdate = Time.time;

        calcJob = new Job();
        calcJob.num_node = num_node;
        calcJob.stifness = stiffness / num_spring;
        calcJob.damp = damp;
        calcJob.addingforce = addingforce;
        calcJob.deltaTimeSec = deltaTimesec;
        //for (double z = 0; z < q; z++)
        //{
        //    for (double y = 0; y < q; y++)
        //    {
        //        for (double x = 0; x < q; x++)
        //        {

        //            Vector3 startPosition = new Vector3((float)x, (float)(y + 0.5), (float)z);
        //            Rigidbody rMass = Instantiate(Mass, startPosition, Quaternion.identity);
        //            RmassList.Add(rMass);
        //            calcJob.InMass.Add(new MassClass((float)0.5, startPosition, deltaTimesec));

        //        }


        //    }
        //}


        //Vector3 globalpos = transform.position;
        Vector3 globalpos = new Vector3(0.0f, 2.0f, 0.0f);
        Debug.Log(transform.position);
        //Debug.Log(transform.localPosition);

        //Vector3 FixedPosition1 = new Vector3(-0.3f, 1.8f, 9.5f);
        //Vector3 FixedPosition1 = new Vector3(-0.1f, 0.0f, 0.0f);
        Vector3 FixedPosition1 = new Vector3(globalpos.x-0.03f, globalpos.y, globalpos.z);

        //FixedPosition1 = transform.TransformPoint(FixedPosition1);
        Debug.Log(FixedPosition1);
        Rigidbody rMass = Instantiate(Mass, FixedPosition1, Quaternion.identity);
        rMass.transform.parent = gameObject.transform;
        RmassList.Add(rMass);
        calcJob.InMass.Add(new MassClass((float)0.5, FixedPosition1, deltaTimesec));

        Vector3 FixedPosition2 = new Vector3(globalpos.x + 0.03f, globalpos.y, globalpos.z);
        //FixedPosition2 = transform.TransformPoint(FixedPosition2);
        Debug.Log(FixedPosition2);
        Rigidbody rMass2 = Instantiate(Mass, FixedPosition2, Quaternion.identity);
        rMass2.transform.parent = gameObject.transform;
        RmassList.Add(rMass2);
        calcJob.InMass.Add(new MassClass((float)0.5, FixedPosition2, deltaTimesec));

        Vector3 dropMassPos = new Vector3(globalpos.x - 0.03f, globalpos.y - 0.01f, globalpos.z);
        Debug.Log(dropMassPos);
        //dropMassPos = transform.TransformPoint(dropMassPos);
        Rigidbody rMass3 = Instantiate(Mass, dropMassPos, Quaternion.identity);
        rMass3.transform.parent = gameObject.transform;
        RmassList.Add(rMass3);
        calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos, deltaTimesec));

        Vector3 dropMassPos2 = new Vector3(globalpos.x + 0.03f, globalpos.y-0.01f, globalpos.z);
        Debug.Log(dropMassPos2);
        //dropMassPos2 = transform.TransformPoint(dropMassPos2);
        Rigidbody rMass4 = Instantiate(Mass, dropMassPos2, Quaternion.identity);
        rMass4.transform.parent = gameObject.transform;
        RmassList.Add(rMass4);
        calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos2, deltaTimesec));

        Vector3 dropMassPos3 = new Vector3(globalpos.x - 0.03f, globalpos.y - 0.02f, globalpos.z);
        //dropMassPos3 = transform.TransformPoint(dropMassPos3);
        Rigidbody rMass5 = Instantiate(Mass, dropMassPos, Quaternion.identity);
        rMass5.transform.parent = gameObject.transform;
        RmassList.Add(rMass5);
        calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos3, deltaTimesec));

        Vector3 dropMassPos4 = new Vector3(globalpos.x + 0.03f, globalpos.y - 0.02f, globalpos.z);
        //dropMassPos4 = transform.TransformPoint(dropMassPos4);
        Rigidbody rMass6 = Instantiate(Mass, dropMassPos2, Quaternion.identity);
        rMass6.transform.parent = gameObject.transform;
        RmassList.Add(rMass6);
        calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos4, deltaTimesec));

        //Vector3 FixedPosition1_2 = new Vector3(-0.3f, 0.0f, 9.55f);
        ////Rigidbody rMass = Instantiate(Mass, FixedPosition1, Quaternion.identity);
        ////RmassList.Add(rMass);
        //calcJob.InMass.Add(new MassClass((float)0.5, FixedPosition1_2, deltaTimesec));

        //Vector3 FixedPosition2_2 = new Vector3(0.3f, 0.0f, 9.55f);
        ////Rigidbody rMass2 = Instantiate(Mass, FixedPosition2, Quaternion.identity);
        ////RmassList.Add(rMass2);
        //calcJob.InMass.Add(new MassClass((float)0.5, FixedPosition2_2, deltaTimesec));

        //Vector3 dropMassPos_2 = new Vector3(-0.2f, 0.0f, 9.55f);
        ////Rigidbody rMass3 = Instantiate(Mass, dropMassPos, Quaternion.identity);
        ////RmassList.Add(rMass3);
        //calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos_2, deltaTimesec));

        //Vector3 dropMassPos2_2 = new Vector3(0.2f, 1.7f, 9.55f);
        ////Rigidbody rMass4 = Instantiate(Mass, dropMassPos2, Quaternion.identity);
        ////RmassList.Add(rMass4);
        //calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos2_2, deltaTimesec));

        //Vector3 dropMassPos3_2 = new Vector3(-0.1f, 0.0f, 9.55f);
        ////Rigidbody rMass3 = Instantiate(Mass, dropMassPos, Quaternion.identity);
        ////RmassList.Add(rMass3);
        //calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos3_2, deltaTimesec));

        //Vector3 dropMassPos4_2 = new Vector3(0.1f, 0.0f, 9.55f);
        ////Rigidbody rMass4 = Instantiate(Mass, dropMassPos2, Quaternion.identity);
        ////RmassList.Add(rMass4);
        //calcJob.InMass.Add(new MassClass((float)0.5, dropMassPos4_2, deltaTimesec));

        calcJob.InMass[0].is_fixed = true;
        calcJob.InMass[1].is_fixed = true;

        //calcJob.InMass[0 + 6].is_fixed = true;
        //calcJob.InMass[1 + 6].is_fixed = true;

        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[0], calcJob.InMass[2], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[1], calcJob.InMass[3], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[0], calcJob.InMass[3], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[1], calcJob.InMass[2], threashold));

        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2], calcJob.InMass[3], threashold));


        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2], calcJob.InMass[4], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[3], calcJob.InMass[5], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2], calcJob.InMass[5], threashold));
        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[3], calcJob.InMass[4], threashold));

        calcJob.InSpring.Add(new SpringClass(calcJob.InMass[4], calcJob.InMass[5], threashold));

        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[0 + 6], calcJob.InMass[2 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[1 + 6], calcJob.InMass[3 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[0 + 6], calcJob.InMass[3 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[1 + 6], calcJob.InMass[2 + 6], threashold));


        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2 + 6], calcJob.InMass[3 + 6], threashold));

        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2 + 6], calcJob.InMass[4 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[3 + 6], calcJob.InMass[5 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[2 + 6], calcJob.InMass[5 + 6], threashold));
        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[3 + 6], calcJob.InMass[4 + 6], threashold));


        //calcJob.InSpring.Add(new SpringClass(calcJob.InMass[4 + 6], calcJob.InMass[5 + 6], threashold));



        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;
        mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.material.color = new Color(123.0f / 255.0f, 63.0f / 255.0f, 0.0f, 0.0f);

        newVertices = new Vector3[K * N * M];
        newUV = new Vector2[K * N * M];
        newNormals = new Vector3[K * N * M];


        //for (int i = N-1; i > -1; i--)
        //{
        //    for (int j = 0; j < M; j++)
        //    {
        //        int dx = i * (N-1) + j;
        //        float M_1 = (float)M - 1.0f;
        //        float N_1 = (float)N - 1.0f;
        //        //newVertices[dx] = new Vector3((10.0f / M_1) * (float)j, (10.0f / N_1) * (float)i, 0.0f);
        //        Debug.Log(dx);
        //        newVertices[dx] = calcJob.InMass[dx].position;
        //        //Debug.Log(newVertices[dx]);
        //        newUV[dx] = new Vector2((1.0f / M_1) * (float)j, (1.0f / N_1) * (float)i);
        //        newNormals[dx] = Vector3.forward;

        //    }
        //}
        for (int k = 0; k < K; k++)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < M; j++)
                {
                    int dx = k * (N * M) + i * M + j;
                    float M_1 = (float)M - 1.0f;
                    float N_1 = (float)N - 1.0f;
                    //newVertices[dx] = new Vector3((10.0f / M_1) * (float)j, (10.0f / N_1) * (float)i, 0.0f);
                    //Debug.Log(dx);
                    newVertices[dx] = calcJob.InMass[dx].position;
                    //Debug.Log(newVertices[dx]);
                    newUV[dx] = new Vector2((1.0f / M_1) * (float)j, (1.0f / N_1) * (float)i);
                    newNormals[dx] = Vector3.forward;

                }
            }

        }

        newTriangles = new int[K * (N - 1) * (M - 1) * 6];
        for (int k = 0; k < K; k++)
        {
            for (int i = 0; i < N - 1; i++)
            {
                for (int j = 0; j < M - 1; j++)
                {
                    //Debug.Log("j: " + j);
                    int dx = k * (M * N) + i * M + j;
                    //Debug.Log("dx: " + dx);
                    int tx;

                    if (dx != 0)
                    {
                        tx = (dx - i - 4 * k) * 6;
                    }
                    else
                    {
                        tx = dx * 6;
                    }
                    //Debug.Log("tx: " + tx);
                    //newTriangles[tx] = dx;
                    //newTriangles[tx + 1] = dx + N;
                    //newTriangles[tx + 2] = dx + 1;
                    //newTriangles[tx + 3] = dx + N;
                    //newTriangles[tx + 4] = dx + N + 1;
                    //newTriangles[tx + 5] = dx + 1;
                    newTriangles[tx] = dx;
                    newTriangles[tx + 1] = dx + 1;
                    newTriangles[tx + 2] = dx + M;
                    newTriangles[tx + 3] = dx + 1;
                    newTriangles[tx + 4] = dx + 1 + M;
                    newTriangles[tx + 5] = dx + M;
                }
            }
        }


        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUV;
        calcJob.Start();




    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();

        }
        frameCount++;
        float currentTimeStamp = (float)Time.time;
        //if (currentTimeStamp - prevTimeStamp > 1)
        //{
        //    //			print ("threadExecution count = " + calcJob.executioncount);
        //    //framrate.text = "Frame Rate = " + calcJob.executioncount;
        //    //brittleness.text = "Brittleness = " + (1 / (threashold * stiffness));
        //    prevTimeStamp = currentTimeStamp;

        //    calcJob.executioncount = 0;
        //}

        //		if (Time.time > nextUpdate)
        //		{
        //			nextUpdate += 1.0/updateRate;
        //			fps = frameCount * updateRate;
        //			frameCount = 0;
        //
        //		}



        for (int i = 0; i < calcJob.InMass.Count; i++)
        {
            RmassList[i].MovePosition(calcJob.InMass[i].new_position);
        }

        Vector3[] vertices = mesh.vertices;
        for (int k = 0; k < K; k++)
        {
            for (int i = N - 1; i > -1; i--)
            {
                for (int j = 0; j < M; j++)
                {
                    int dx = k * (M * N) + i * M + j;
                    float M_1 = (float)M - 1.0f;
                    float N_1 = (float)N - 1.0f;
                    //newVertices[dx] = new Vector3((10.0f / M_1) * (float)j, (10.0f / N_1) * (float)i, 0.0f);
                    Vector3 new_ver = calcJob.InMass[dx].new_position;
                    if (new_ver.y < 0.2)
                    {
                        //calcJob.InMass[0].is_fixed = false;
                        //calcJob.InMass[1].is_fixed = false;
                        //calcJob.InMass[0 + 6].is_fixed = false;
                        //calcJob.InMass[1 + 6].is_fixed = false;
                        vertices[dx] = new Vector3(new_ver.x, 0.2f, new_ver.z);

                        //Debug.Log((float)(currentTimeStamp - prevTimeStamp));
                        //OnApplicationQuit();
                    }
                    else
                    {
                        vertices[dx] = new_ver;
                    }

                    //Debug.Log(newVertices[dx]);
                    //newUV[dx] = new Vector2((1.0f / M_1) * (float)j, (1.0f / N_1) * (float)i);
                    //newNormals[dx] = Vector3.forward;

                }
            }
        }


        mesh.vertices = vertices;
        mesh.RecalculateBounds();


        if (calcJob != null)
        {
            if (calcJob.Update())
            {
                calcJob = null;
            }
        }
        //		springList = calcJob.InSpring;



    }

    void OnApplicationQuit()
    {
        Debug.Log("finished");
        calcJob.IsDone = true;

    }
    //	private void UpdateCylinderPosition(GameObject cylinder, Vector3 beginPoint, Vector3 endPoint)
    //	{
    //		Vector3 offset = endPoint - beginPoint;
    //		Vector3 position = beginPoint + (offset / 2.0f);
    //
    //		cylinder.transform.position = position;
    //		cylinder.transform.LookAt(beginPoint);
    //		Vector3 localScale = cylinder.transform.localScale;
    ////		localScale.z = (endPoint - beginPoint).magnitude;
    //		localScale.z = 0.1f;
    //		localScale.x = 0.1f;
    //		cylinder.transform.localScale = localScale;
    //	}


    //	void OnDrawGizmos() {
    //		Gizmos.color = Color.yellow;
    //
    //		for (int i = 0; i < calcJob.InSpring.Count; i++) {
    //			Gizmos.DrawLine (calcJob.InSpring[i].m1.position,calcJob.InSpring[i].m2.position);
    //		}
    ////		Gizmos.DrawSphere(transform.position, 1);
    //	}

    //	public static void restart() {
    //		string[] endings = new string[]{
    //			"exe", "x86", "x86_64", "app"
    //		};
    //		string executablePath = Application.dataPath + "/..";
    //		foreach (string file in System.IO.Directory.GetFiles(executablePath)) {
    //			foreach (string ending in endings) {
    //				if (file.ToLower ().EndsWith ("." + ending)) {
    //					System.Diagnostics.Process.Start (executablePath + file);
    //					Application.Quit ();
    //					return;
    //				}
    //			}
    //
    //		}
    //	}

}
