using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{

    [SerializeField]
    private Material material;
    private TubeRenderer tubeRenderer;
    private float maxSectionSize = .1f;
    private int maxListSize = 50;
    private Vector3 topPos;
    private List<Vector3> positions;

    void Start()
    {
        topPos = transform.position;
        tubeRenderer = gameObject.AddComponent<TubeRenderer>();
        tubeRenderer.material = material;

        positions = new List<Vector3> { topPos, topPos };

        tubeRenderer.SetPositions(positions.ToArray());

    }

    void Update()
    {
        topPos.x += Input.GetAxis("Horizontal") * 0.5f;
        topPos.z += Input.GetAxis("Vertical") * 0.5f;
        topPos.y += 1f;
        positions[positions.Count - 1] = topPos;
        if (Vector3.Distance(positions[positions.Count - 1], positions[positions.Count - 2]) > maxSectionSize)
        {
            InsertSection();
        }
        tubeRenderer.SetPositions(positions.ToArray());

        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.y = topPos.y;
        //Camera.main.transform.position = cameraPos;
    }
    void InsertSection()
    {

        if (positions.Count >= maxListSize)
        {
            for (int i = 0; i < positions.Count - 1; i++)
            {
                positions[i] = positions[i + 1];
            }
        }
        else
        {
            positions.Add(topPos);
        }
    }
}
