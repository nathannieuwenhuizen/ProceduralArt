using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : PoolObject
{

    [SerializeField]
    private int subDivisions = 20;

    [SerializeField]
    private Vector2 planeSize;

    [SerializeField]
    private bool rollFromAbove = true;

    [SerializeField]
    private float rollOutSpeed = 10f;

    [SerializeField]
    private float beginstand = 2f;

    [SerializeField]
    private float endStand = 0.5f;

    private Mesh mesh;

    private Vector3[] _vertices;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    public bool disappear = true;

    // Start is called before the first frame update
    void Start()
    {
        //UpdateMesh(CalculateEulerSpiral(2f, subDivisions, 10));
        //RollOut();
    }

    private void Setup()
    {
        _meshFilter = GetComponent<MeshFilter>();
        if (_meshFilter == null)
        {
            _meshFilter = gameObject.AddComponent<MeshFilter>();
        }

        _meshRenderer = GetComponent<MeshRenderer>();
        if (_meshRenderer == null)
        {
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }
        mesh = new Mesh();
        _meshFilter.mesh = mesh;

    }

    public override void Destroy()
    {
        transform.parent.gameObject.SetActive(false);
        //base.Destroy();
    }

    public void RollOut()
    {
        StopAllCoroutines();
        if (mesh == null)
        {
            Setup();
        }
        StartCoroutine(RollingOut());
    }

    IEnumerator RollingOut()
    {
        float i = beginstand;
        while (i > endStand + 0.1f)
        {
            UpdateMesh(CalculateEulerSpiral( i, subDivisions, planeSize.y));
            i = Mathf.Lerp(i, endStand, 1f/60f * rollOutSpeed);
            yield return new WaitForSeconds(1f/60f);
        }

        if (disappear)
        {
            yield return new WaitForSeconds(2f);
            Destroy();
        }
    }

    List<Vector2> CalculateEulerSpiral(float T, float steps, float size = 10f)
    {
        List<Vector2> result = new List<Vector2>();
        float dx, dy, t = 0;
        Vector2 prev = new Vector2(0, 0);
        Vector2 current = new Vector2(0, 0);

        float dt = T / steps;
        float scale = size / T;
        float tempVal;
        while (steps > 0)
        {
            tempVal = t * t * Mathf.PI;
            dx = Mathf.Cos(tempVal) * dt;
            dy = Mathf.Sin(tempVal) * dt;
            if (!rollFromAbove)
            {
                dy *= -1;
            }
            t += dt;
            current.x += dx;
            current.y += dy;
            //Debug.DrawLine(prev * scale, current * scale, Color.white, 1000.1f);
            prev = current;
            steps--;
            result.Add(current * scale);
        }
        return result;
    }
    public void UpdateMesh(List<Vector2> coords)
    {
        
        Vector3[] vertices = new Vector3[(subDivisions + 1) * 2];
        vertices[0] = new Vector3(planeSize.x / 2f, 0, 0);
        vertices[1] = new Vector3(-planeSize.x / 2f, 0, 0);

        for (int i = 1; i < subDivisions + 1; i ++)
        {
            vertices[i * 2 + 1] = new Vector3(-planeSize.x / 2f, coords[i - 1].y, -coords[i - 1].x);
            vertices[i * 2] = new Vector3(planeSize.x / 2f, coords[i - 1].y, -coords[i - 1].x);
            //Debug.Log(vertices[i * 2]);
            //Debug.Log(vertices[i * 2 + 1]);
            //Debug.DrawLine(vertices[i * 2], vertices[i * 2 + 1], Color.red, 1000.1f);

        }
        mesh.vertices = vertices;
        if (mesh.triangles.Length != subDivisions * 6)
        {
            if (Globals.leafMesh == null)
            {
                int[] triangles = new int[subDivisions * 6];
                for (int i = 0; i < subDivisions; i++)
                {
                    triangles[i * 6] = i * 2;
                    triangles[i * 6 + 1] = i * 2 + 2;
                    triangles[i * 6 + 2] = i * 2 + 1;

                    triangles[i * 6 + 3] = i * 2 + 2;
                    triangles[i * 6 + 4] = i * 2 + 3;
                    triangles[i * 6 + 5] = i * 2 + 1;
                }
                mesh.triangles = triangles;

                mesh.RecalculateBounds();

                Vector2[] uv = new Vector2[(subDivisions + 1) * 2];
                for (int i = 0; i < uv.Length - 1; i += 2)
                {
                    uv[i] = new Vector2(1f, i / (float)(uv.Length - 2) * 1f);
                    uv[i + 1] = new Vector2(0f, i / (float)(uv.Length - 2) * 1f);
                }

                mesh.uv = uv;

                Globals.leafMesh = mesh;

            } else
            {
                mesh.triangles = Globals.leafMesh.triangles;
                mesh.RecalculateBounds();
                mesh.uv = Globals.leafMesh.uv;
            }
        }


    }

    //purely for testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //RollOut();
        }
    }
}
