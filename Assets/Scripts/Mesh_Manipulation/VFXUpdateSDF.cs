using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.SDF;

public class VFXUpdateSDF : MonoBehaviour
{
    MeshToSDFBaker m_Baker;
    MeshRenderer m_MeshRenderer;
    Mesh m_Mesh;
    VisualEffect m_VFX;
    public int maxResolution = 64;
    public Vector3 center;
    public Vector3 sizeBox;
    public int signPassCount = 1;
    public float threshold = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        m_Mesh = GetComponent<MeshFilter>().mesh;
        m_VFX = GetComponent<VisualEffect>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_Baker = new MeshToSDFBaker(sizeBox, center, maxResolution, m_Mesh, signPassCount, threshold);
        m_Baker.BakeSDF();
        m_VFX.SetTexture("meshSDF", m_Baker.SdfTexture);
        m_VFX.SetVector3("BoxSize", m_Baker.GetActualBoxSize());

    }

    // Update is called once per frame
    void Update()
    {
        m_Baker.BakeSDF();
        m_VFX.SetTexture("meshSDF", m_Baker.SdfTexture);
    }

    public void AccelForce(Vector3 accelVector)
    {
        
        m_VFX.SetVector3("acceleration", accelVector);
    }

    public void IncreaseBoxSize()
    {
        Vector3 currSize = sizeBox;
        Vector3 newSize = new Vector3(currSize.x+1f, currSize.y+1f, currSize.z+1f);
        sizeBox = newSize;
        m_VFX.SetVector3("BoxSize", newSize);
    }

    void OnDestroy()
    {
        if (m_Baker != null)
        {
            m_Baker.Dispose();
        }
    }
}
