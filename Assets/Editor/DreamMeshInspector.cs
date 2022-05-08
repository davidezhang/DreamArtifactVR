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

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(DreamMesh))]
public class DreamMeshInspector : Editor
{
    private DreamMesh mesh;
    private Transform handleTransform;
    private Quaternion handleRotation;

    //DZ
    private ObjExporter objExporter;

    void OnSceneGUI()
    {
        mesh = target as DreamMesh;
        handleTransform = mesh.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ? handleTransform.rotation : Quaternion.identity;

        // ShowHandles on Mesh
        if (mesh.isEditMode)
        {
            if (mesh.originalVertices == null || mesh.normals.Length == 0)
            {
                mesh.Init();
            }
            for (int i = 0; i < mesh.originalVertices.Length; i++)
            {
                ShowHandle(i);
            }
        }

        // Show/ Hide Transform Tool
        if (mesh.showTransformHandle)
        {
            Tools.current = Tool.Move;
        }
        else
        {
            Tools.current = Tool.None;
        }
    }

    void ShowHandle(int index)
    {
        Vector3 point = handleTransform.TransformPoint(mesh.originalVertices[index]);

        // unselected vertex
        if (!mesh.selectedIndices.Contains(index))
        {
            Handles.color = Color.blue;
            if (Handles.Button(point, handleRotation, mesh.pickSize, mesh.pickSize,
                Handles.DotHandleCap)) //1
            {
                mesh.selectedIndices.Add(index); //2
            }
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        mesh = target as DreamMesh;

        if (mesh.isEditMode || mesh.isMeshReady)
        {
            if (GUILayout.Button("Show Normals"))
            {
                Vector3[] verts = mesh.modifiedVertices.Length == 0 ? mesh.originalVertices : mesh.modifiedVertices;
                Vector3[] normals = mesh.normals; Debug.Log(normals.Length);
                for (int i = 0; i < verts.Length; i++)
                {
                    Debug.DrawLine(handleTransform.TransformPoint(verts[i]), handleTransform.TransformPoint(normals[i]), Color.green, 4.0f, true);
                }
            }
        }

        if (GUILayout.Button("Clear Selected Vertices"))
        {
            mesh.ClearAllData();
        }

        //if (!mesh.isEditMode && mesh.isMeshReady)
        if (!mesh.isEditMode)
        {
            

            if (GUILayout.Button("Save Mesh"))
            {

                //string path = "Assets/Prefabs/CustomHeart.prefab"; //1
                //DZ: save and name each mesh artifact
                string ObjPath = "Assets/SavedMeshes/" + mesh.inputName + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff" + ".obj");
                string path = "Assets/Prefabs/" + mesh.inputName + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".prefab";


                /*
                mesh.isMeshReady = false;
                Object prefabToInstantiate =
                    AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)); //2
                Object referencePrefab =
                    AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                GameObject gameObj =
                    (GameObject)PrefabUtility.InstantiatePrefab(prefabToInstantiate);
                Mesh prefabMesh = (Mesh)AssetDatabase.LoadAssetAtPath(path,
                    typeof(Mesh)); //3
                if (!prefabMesh)
                {
                    prefabMesh = new Mesh();
                    AssetDatabase.AddObjectToAsset(prefabMesh, path);
                }
                else
                {
                    prefabMesh.Clear();
                }
                prefabMesh = mesh.SaveMesh(); //4
                AssetDatabase.AddObjectToAsset(prefabMesh, path);
                gameObj.GetComponentInChildren<MeshFilter>().mesh = prefabMesh; //5
                PrefabUtility.SaveAsPrefabAsset(gameObj, path); //6
                Object.DestroyImmediate(gameObj); //7
                */

                //DZ Export as OBJ
                ObjExporter.MeshToFile(mesh.GetComponent<MeshFilter>(), ObjPath);
            }
        }

    }


}
