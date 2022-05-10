/*
 * Copyright (c) 2019 Razeware LLC
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * Notwithstanding the foregoing, you may not use, copy, modify, merge, publish, 
 * distribute, sublicense, create a derivative work, and/or sell copies of the 
 * Software in any work that is designed, intended, or marketed for pedagogical or 
 * instructional purposes related to programming, coding, application development, 
 * or information technology.  Permission for such use, copying, modification,
 * merger, publication, distribution, sublicensing, creation of derivative works, 
 * or sale is expressly withheld.
 *    
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DreamMesh : MonoBehaviour
{
    //DZ
    //to call and set box size
    public VFXUpdateSDF vfx;
    private AudioSource growthAudio;

    //DZ
    //To select person profile
    public string inputName;
    public DataManager dataManager;

    Mesh originalMesh;
    Mesh clonedMesh;
    MeshFilter meshFilter;

    [HideInInspector]
    public int targetIndex;

    [HideInInspector]
    public Vector3 targetVertex;

    [HideInInspector]
    public Vector3[] originalVertices;

    [HideInInspector]
    public Vector3[] modifiedVertices;

    [HideInInspector]
    public Vector3[] normals;

    [HideInInspector]
    public bool isMeshReady = false;
    public bool isEditMode = true;
    public bool showTransformHandle = true;
    public List<int> selectedIndices = new List<int>();
    public float pickSize = 0.01f;

    public float radiusOfEffect = 0.3f; //1 
    public float pullValue; //2
    public float duration; //3
    int currentIndex = 0; //4
    [HideInInspector]
    public bool isAnimate = false;
    float startTime = 0f;
    float runTime = 0f;

    //DZ
    //new list of radius effect values per vertex index
    [HideInInspector]
    public float[] radiusOfEffectList;

    void Start()
    {
        Init();

        //TEST
        //Debug.Log("NUMBERSSS");
        //Debug.Log(originalMesh.vertices.Length);
    }

    public void Init()
    {
        //DZ Check Data profile
        
        if (dataManager.LoadData(inputName) == null)
        {
            //if new person, create vertices profile
            //creaet list with 8 random numbers
            List<int> newList = createSelectedList();
            PersonVerticesData vData = new PersonVerticesData(newList, inputName);
            dataManager.SaveData(vData);
            selectedIndices = newList;
        } else
        {
            PersonVerticesData vData = dataManager.LoadData(inputName);
            selectedIndices = vData.selectedIndices;
        }

        //DZ Assign AudioSource for growth
        growthAudio = GetComponent<AudioSource>();


        meshFilter = GetComponent<MeshFilter>();
        isMeshReady = false;

        currentIndex = 0;

        //DZ
        //instantiate radiusOfEffect List
        radiusOfEffectList = new float[8];

        if (isEditMode)
        {
            originalMesh = meshFilter.sharedMesh;
            clonedMesh = new Mesh();
            clonedMesh.name = "clone";
            clonedMesh.vertices = originalMesh.vertices;
            clonedMesh.triangles = originalMesh.triangles;
            clonedMesh.normals = originalMesh.normals;
            meshFilter.mesh = clonedMesh;

            originalVertices = clonedMesh.vertices;
            normals = clonedMesh.normals;
            Debug.Log("Init & Cloned");
        }
        else
        {
            originalMesh = meshFilter.mesh;
            originalVertices = originalMesh.vertices;
            normals = originalMesh.normals;
            modifiedVertices = new Vector3[originalVertices.Length];
            for (int i = 0; i < originalVertices.Length; i++)
            {
                modifiedVertices[i] = originalVertices[i];
            }

            //called by pBB handler instead - only when data is being collected and parameters are ready
            //StartDisplacement();
        }

    }

    private List<int> createSelectedList()
    {
        List<int> newList = new List<int>();
        for (int i = 0; i < 8; i++)
        {
            newList.Add(UnityEngine.Random.Range(1, 515));
        }
        return newList;
        
    }

    public void StartDisplacement()
    {
        targetVertex = originalVertices[selectedIndices[currentIndex]]; //1
        startTime = Time.time; //2
        //isAnimate = true;

        //DZ play audio
        if (!growthAudio.isPlaying)
        {
            growthAudio.Play();
        }

    }

    protected void FixedUpdate() //1
    {
        if (!isAnimate) //2
        {
            return;
        }

        runTime = Time.time - startTime; //3

        if (runTime < duration)  //4
        {
            Vector3 targetVertexPos =
                meshFilter.transform.InverseTransformPoint(targetVertex);
            DisplaceVertices(targetVertexPos, pullValue, radiusOfEffectList[currentIndex]);
            
            
            
        }
        else //5
        {
            //DZ stop growth audio
            growthAudio.Stop();


            currentIndex++;
            if (currentIndex < selectedIndices.Count) //6
            {
                StartDisplacement();
            }
            else if (currentIndex == selectedIndices.Count)
            {
                currentIndex = 0;

                //update mesh vertex positions for next round of morphing
                originalVertices = meshFilter.mesh.vertices;
                StartDisplacement();
            }

            else //7
            {
                originalMesh = GetComponent<MeshFilter>().mesh;
                isAnimate = false;
                isMeshReady = true;
            }
        }
    }



    void DisplaceVertices(Vector3 targetVertexPos, float force, float radius)
    {
        Vector3 currentVertexPos = Vector3.zero;
        float sqrRadius = radius * radius; //1

        for (int i = 0; i < modifiedVertices.Length; i++) //2
        {
            currentVertexPos = modifiedVertices[i];
            float sqrMagnitude = (currentVertexPos - targetVertexPos).sqrMagnitude; //3
            if (sqrMagnitude > sqrRadius)
            {
                continue; //4
            }
            float distance = Mathf.Sqrt(sqrMagnitude); //5
            float falloff = GaussFalloff(distance, radius);
            Vector3 translate = (currentVertexPos * force) * falloff; //6
            translate.z = 0f;
            Quaternion rotation = Quaternion.Euler(translate);
            Matrix4x4 m = Matrix4x4.TRS(translate, rotation, Vector3.one);
            modifiedVertices[i] = m.MultiplyPoint3x4(currentVertexPos);

            //DEBUG
            Debug.Log("force: " + force + "\n");
            Debug.Log("radius: " + radius);
        }
        originalMesh.vertices = modifiedVertices; //7
        originalMesh.RecalculateNormals();

        //increase box size
        //vfx.IncreaseBoxSize();
    }

    public void ClearAllData()
    {
        selectedIndices = new List<int>();
        targetIndex = 0;
        targetVertex = Vector3.zero;
    }

    public Mesh SaveMesh()
    {
        Mesh nMesh = new Mesh();
        nMesh.name = "DreamMesh";
        nMesh.vertices = originalMesh.vertices;
        nMesh.triangles = originalMesh.triangles;
        nMesh.normals = originalMesh.normals;
        
        nMesh.RecalculateBounds();
        nMesh.RecalculateNormals();
        nMesh.RecalculateTangents();
        return nMesh;
    }

    //DZ
    // for testing pBB function call
    public void MorphMesh(decimal item)
    {
        Debug.Log(item);
    }


    #region HELPER FUNCTIONS

    static float LinearFalloff(float dist, float inRadius)
    {
        return Mathf.Clamp01(0.5f + (dist / inRadius) * 0.5f);
    }

    static float GaussFalloff(float dist, float inRadius)
    {
        return Mathf.Clamp01(Mathf.Pow(360, -Mathf.Pow(dist / inRadius, 2.5f) - 0.01f));
    }

    static float NeedleFalloff(float dist, float inRadius)
    {
        return -(dist * dist) / (inRadius * inRadius) + 1.0f;
    }

    #endregion
}
