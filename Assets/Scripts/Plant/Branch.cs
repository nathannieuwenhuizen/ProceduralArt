using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{

    [SerializeField]
    private Material material;
    private TubeRenderer tubeRenderer;
    private float maxSectionSize = 5f;
    private int maxListSize = 20;
    public Vector3 currentPos;
    public Vector3 desiredPos;

    public List<Vector3> positions;

    public float transitionSpeed = .5f;
    public GameObject leafPref;
    void Start()
    {
        currentPos = transform.position;
        tubeRenderer = gameObject.AddComponent<TubeRenderer>();
        tubeRenderer.material = material;


    }

    public void SetPos(Vector3 position)
    {
        desiredPos = position;
        currentPos = position;
        positions = new List<Vector3> { position, position };
    }

    public void Grow(float yPos)
    {
        desiredPos.y = yPos;
        currentPos = Vector3.Lerp(currentPos, desiredPos, Time.deltaTime * transitionSpeed);
        //currentPos += (desiredPos - currentPos) *0.01f;
        //currentPos = desiredPos;
        currentPos.y = yPos;
        UpdateMesh();
    }
    public void UpdateMesh()
    {
        positions[positions.Count - 1] = currentPos;
        if (Vector3.Distance(positions[positions.Count - 1], positions[positions.Count - 2]) > maxSectionSize)
        {
            InsertSection();
        }
        tubeRenderer.SetPositions(positions.ToArray());
    }

    public void SpawnLeaf(GameObject leaf)
    {
        GameObject newLeaf = Instantiate(leaf.gameObject);

        //newLeaf.transform.localRotation = (Quaternion.LookRotation(positions[positions.Count - 2] - positions[positions.Count - 1] + Vector3.right));
        newLeaf.transform.Rotate(new Vector3(0, Random.value * 360, 0));
        newLeaf.transform.position = positions[positions.Count - 2];
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
            positions.Add(currentPos);
        }
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(desiredPos, 1);
    }
}
