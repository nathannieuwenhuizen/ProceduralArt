using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{

    [SerializeField]
    private Material material;

    [SerializeField]
    private float subDivisions = 20;

    [SerializeField]
    private Vector2 planeSize;

    [SerializeField]
    private bool rollFromAbove = true;

    private Mesh mesh;

    private Vector3[] _vertices;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;

    // Start is called before the first frame update
    void Start()
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

        //drawEulerSpiral(2f, 20, 10);
        StartCoroutine(test());
    }
    IEnumerator test()
    {
        float i = 2f;
        while (i > 0.1f)
        {
            drawEulerSpiral(i, 20, 10f);
            i -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    List<Vector2> drawEulerSpiral(float T, float steps, float scale, float size = 5f)
    {
        List<Vector2> result = new List<Vector2>();
        float dx, dy, t = 0;
        Vector2 prev = new Vector2(0, 0);
        Vector2 current = new Vector2(0, 0);
        float dt = T / steps;
        while (steps > 0)
        {
            //Debug.Log(steps + " | " + current.x + " | " + current.y);
            dx = Mathf.Cos(t * t * Mathf.PI) * dt;
            dy = Mathf.Sin(t * t * Mathf.PI) * dt;
            t += dt;
            current.x += dx;
            current.y += dy;
            Debug.DrawLine(prev * scale, current * scale, Color.white, .1f);
            prev = current;
            steps--;
            result.Add(current);
        }
        return result;
    }

    public void RollOut()
    {

    }
    IEnumerator RollingOut()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
