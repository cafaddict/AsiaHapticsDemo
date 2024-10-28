using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightField_CPU : MonoBehaviour {
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    Vector3[] newNormals;
    public int N = 9;//row
    public int M = 9;//column
    float width = 10;
    float height = 10;

    
    float[] u;
    float[] v;
    int framecounter;
    float prevtime;
    MeshFilter mf;
    Mesh mesh;
    MeshRenderer mesh_renderer;
    // Use this for initialization
    void Start () {

        framecounter = 0;
        prevtime = Time.time;

        u = new float[N * M];
        v = new float[N * M];
        mf = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;
        mesh_renderer = GetComponent<MeshRenderer>();
        mesh_renderer.material.color = new Color(123.0f / 255.0f, 63.0f / 255.0f, 0.0f,0.0f);
        //mesh_renderer.material.color = new Color(1.0f, 1.0f, 1.0f);
        //mesh_renderer.sharedMaterial = new Material(Shader.Find("Diffuse"));

        int num_vert = 0;
        Debug.Log(N);
        for(int i = 0; i < N; i++)
        {
            int k;
            if(i < N/2)
            {
                k = i * 2 + 1;
            }
            else
            {
                k = (N - 1 - i) * 2 + 1;
            }
            for(int j =0; j<k;j++)
            {
                num_vert += 1;
            }

        }
        Debug.Log(num_vert);
        newVertices = new Vector3[num_vert];
        newUV = new Vector2[num_vert];
        newNormals = new Vector3[num_vert];

        int dx_p = 0;
        float angle_between = Mathf.PI / ((float)(N - 1));
        for (int i = 0; i < N; i++)
        {
            float vert_angle = i * angle_between;
            int k;
            if (i < N / 2)
            {
                k = i * 2 + 1;
            }
            else
            {
                k = (N - 1 - i) * 2 + 1;
            }
            for (int j = 0; j < k; j++)
            {
                float x_pos;
                if(i < N/2)
                {
                    x_pos = Mathf.Cos(-Mathf.PI / 2.0f - vert_angle + j * angle_between);
                }
                else
                {
                    x_pos = Mathf.Cos(-Mathf.PI / 2.0f - vert_angle - j * angle_between);
                }
                
                float y_pos = Mathf.Sin(-Mathf.PI / 2.0f - vert_angle);
               
                newVertices[dx_p] = new Vector3(x_pos, y_pos, 0.0f);
                //Debug.Log(newVertices[dx_p]);
                newUV[dx_p] = new Vector2(x_pos / 2.0f + 0.5f, y_pos / 2.0f + 0.5f);
                newNormals[dx_p] = Vector3.forward;
                u[dx_p] = 0.0f;
                dx_p = dx_p + 1;

            }

        }



        //newVertices = new Vector3[N * M];
        //newUV = new Vector2[N * M];
        //newNormals = new Vector3[N * M];
        //for (int i = 0; i < N; i++)
        //{
        //    for (int j = 0; j < M; j++)
        //    {
        //        int dx = i * M + j;
        //        float M_1 = (float)M - 1.0f;
        //        float N_1 = (float)N - 1.0f;
        //        newVertices[dx] = new Vector3((10.0f / M_1) * (float)j, (10.0f / N_1) * (float)i, 0.0f);
        //        //Debug.Log(newVertices[dx]);
        //        newUV[dx] = new Vector2((1.0f / M_1) * (float)j, (1.0f / N_1) * (float)i);
        //        newNormals[dx] = Vector3.forward;
        //        if(i > N * 0.8f)
        //        {
        //            u[dx] = 1.0f;
        //        }
        //        else
        //        {
        //            u[dx] = 0.0f;
        //        }
        //    }
        //}

        //newTriangles = new int[(N - 1) * (M - 1) * 6];
        //Debug.Log(newTriangles.Length);
        //Debug.Log(6 * ((M - 2) * N + (M - 2)));


        int dx_t = 0;
        int tx;
        newTriangles = new int[(dx_p-N)*2*3];
        //Debug.Log((dx_p - N) * 2 * 3);
        for(int i = 0; i < N/2; i++)
        {
            int k = i*2 + 1;

            for (int j = 0; j < k; j++)
            {
                tx = 6 * dx_t;
                Debug.Log(dx_p - 1 - dx_t);
                Debug.Log(dx_p - 1 - dx_t-k-1);
                Debug.Log(dx_p - 1 - dx_t + 1);
                //Debug.Log(dx_t);
                if (i%2 == 0)
                {
                    if(j%2 == 0)
                    {
                        
                        newTriangles[tx] = dx_t;
                        newTriangles[tx + 1] = dx_t + k + 1;
                        newTriangles[tx + 2] = dx_t + k;
                        newTriangles[tx + 3] = dx_t;
                        newTriangles[tx + 4] = dx_t + k + 2;
                        newTriangles[tx + 5] = dx_t + k + 1;
                        


                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 4] = dx_p - 1 - dx_t -k - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 3] = dx_p - 1 - dx_t - k;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 2] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 1] = dx_p - 1 - dx_t -k - 2;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t -k - 1;
                    }
                    else
                    {
                        tx = 6 * dx_t;
                        newTriangles[tx] = dx_t;
                        newTriangles[tx + 1] = dx_t + k + 1;
                        newTriangles[tx + 2] = dx_t - 1;
                        newTriangles[tx + 3] = dx_t;
                        newTriangles[tx + 4] = dx_t + 1;
                        newTriangles[tx + 5] = dx_t + k + 1;
                        

                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 4] = dx_p - 1 - dx_t - k - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 3] = dx_p - 1 - dx_t + 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 2] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 1] = dx_p - 1 - dx_t - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t - k - 1;
                    }
                }
                else
                {
                    if (j % 2 == 0)
                    {
                        
                        newTriangles[tx] = dx_t;
                        newTriangles[tx + 1] = dx_t + k + 1;
                        newTriangles[tx + 2] = dx_t + k;
                        newTriangles[tx + 3] = dx_t;
                        newTriangles[tx + 4] = dx_t + k + 2;
                        newTriangles[tx + 5] = dx_t + k + 1;

                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 4] = dx_p - 1 - dx_t - k - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 3] = dx_p - 1 - dx_t - k;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 2] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 1] = dx_p - 1 - dx_t - k - 2;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t - k - 1;
                    }
                    else
                    {
                        tx = 6 * dx_t;
                        newTriangles[tx] = dx_t;
                        newTriangles[tx + 1] = dx_t + k + 1;
                        newTriangles[tx + 2] = dx_t - 1;
                        newTriangles[tx + 3] = dx_t;
                        newTriangles[tx + 4] = dx_t + 1;
                        newTriangles[tx + 5] = dx_t + k + 1;

                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 4] = dx_p - 1 - dx_t - k - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 3] = dx_p - 1 - dx_t + 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 2] = dx_p - 1 - dx_t;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 1] = dx_p - 1 - dx_t - 1;
                        newTriangles[(dx_p - N) * 2 * 3 - 1 - tx - 5] = dx_p - 1 - dx_t - k - 1;
                    }
                }

                dx_t++;
                
            }
            

        }


        //for (int i = 0; i < N - 1; i++)
        //{
        //    for (int j = 0; j < M - 1; j++)
        //    {
        //        int dx = i * N + j;
        //        int tx;
        //        if (dx != 0)
        //        {
        //            tx = (dx - i) * 6;
        //        }
        //        else
        //        {
        //            tx = dx * 6;
        //        }

        //        newTriangles[tx] = dx;
        //        newTriangles[tx + 1] = dx + N;
        //        newTriangles[tx + 2] = dx + 1;
        //        newTriangles[tx + 3] = dx + N;
        //        newTriangles[tx + 4] = dx + N + 1;
        //        newTriangles[tx + 5] = dx + 1;
        //    }
        //}

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        mesh.uv = newUV;

    }

    // Update is called once per frame
    void Update()
    {
        framecounter += 1;
        float currentTimeStamp = Time.time;
        if (currentTimeStamp - prevtime >= 1)
        {
            //print((float) framecounter / (currentTimeStamp - prevtime));
            prevtime = currentTimeStamp;
            framecounter = 0;
        }
        updateHeightField(0.05f);
        Vector3[] vertices = mesh.vertices;
        int i = 0;
        while (i < vertices.Length)
        {
            vertices[i] = new Vector3(vertices[i].x, vertices[i].y, u[i]);
            //print(u[i]);
            i++;
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();



        //Vector3[] vertices = mesh.vertices;
        //Debug.Log(vertices.Length);
        //int i = 0;
        //while (i < vertices.Length)
        //{
        //    vertices[i] += Vector3.up * Time.deltaTime;
        //    i++;
        //}
        //mesh.vertices = vertices;
        //mesh.RecalculateBounds();
    }

    void updateHeightField(float dt)
    {
        float M_1 = (float)M - 1.0f;
        float N_1 = (float)N - 1.0f;
        float hm = (10.0f / M_1);
        float hn = (10.0f / N_1);

        float[] u1 = new float[N * M];
        float[] v1 = new float[N * M];
        float c = 1.0f;
        float max_c = (1.0f / dt);
        if(c > max_c)
        {
            //print("Warning: invalid C value\n");
            //return;
            c = max_c;
        }

        dt = dt * 4.9f;
        for(int i = 0; i <N;i++)
        {
            for(int j = 0; j < M;j++)
            {
                int current = i * N + j;
                int bottom = (i == 0) ? i * N + j : (i - 1) * N + j;
                int top = (i == N - 1) ? i * N + j : (i + 1) * N + j;
                int left = (j == 0) ? i * N + j : i * N + (j - 1);
                int right = (j == M - 1) ? i * N + j : i * N + (j + 1);
                float f = ((c * c) * ((u[right] + u[left] + u[bottom] + u[top])) - (4.0f * u[current])) / (2.0f*2.0f);
                if (f > 0.1)
                {
                    
                    f = 0.1f;
                    
                }
                else if (f < -0.1)
                {
                    
                    f = -0.1f;
                    
                }
                v1[current] = v[current] + f * dt;
                u1[current] = u[current] + v1[current] * dt;
                v1[current] *= 0.995f;


            }
        }
        u = u1;
        v = v1;
    }
}
