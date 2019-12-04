using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{

    [SerializeField]
    private Material material;
    private TubeRenderer tubeRenderer;
    private float maxSectionSize = 2f;
    private int maxListSize = 50;
    public float spectrumOffset = 0f;

    public Vector3 currentPos;
    public Vector3 desiredPos;
    public Vector3 currentSpeed;

    public List<Vector3> positions;

    public Transform transformTop;
    public Transform desiredTransform;


    public float transitionSpeed = 20f;
    public GameObject leafPref;
    void Start()
    {
        //currentPos = transform.position;
        tubeRenderer = gameObject.AddComponent<TubeRenderer>();
        tubeRenderer.material = material;


    }

    public void SetPos(Vector3 position)
    {
        desiredPos = position;
        currentPos = position;
        positions = new List<Vector3> { position, position };
    }

    public void Grow(float yPos, bool forcedLerp = true, bool updateSpeed = true)
    {
        if (updateSpeed)
        {
            desiredPos.y = yPos;

            if (!forcedLerp)
            {
                currentSpeed.Normalize();
                currentSpeed += (desiredPos - currentPos) * 0.005f;

            }
            else
            {
                Vector3 deltaSpeed = (Vector3.Lerp(currentPos, desiredPos, Time.deltaTime * transitionSpeed) - currentPos) - currentSpeed;
                if (Vector3.Distance(deltaSpeed, currentSpeed) > 3f)
                {
                    //currentSpeed.Normalize();
                    currentSpeed += deltaSpeed * 0.2f;

                }
                else
                {
                    currentSpeed += deltaSpeed;
                }
            }
        }
        currentPos += currentSpeed;

        currentPos.y = yPos;
        UpdateMesh();
    }
    public void UpdateMesh()
    {
        positions[positions.Count - 1] = transformTop.position = currentPos;

        transformTop.localScale = new Vector3(1 + spectrumOffset, 1 + spectrumOffset, 1 + spectrumOffset);

        desiredTransform.position = currentPos + (desiredPos - currentPos) * 0.3f;
        desiredTransform.Translate((positions[positions.Count - 1] - positions[positions.Count - 2]).normalized * 5f);

        /*
        //transformTop.rotation = Quaternion.identity;
        // Determine which direction to rotate towards
        Vector3 targetDirection = positions[positions.Count - 1] - positions[positions.Count - 2];

        // The step size is equal to speed times frame time.
        float singleStep = 5f * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(transformTop.up, targetDirection, singleStep, 0.0f);

        // Draw a ray pointing at our target in
        //Debug.DrawRay(transform.position, newDirection, Color.red);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transformTop.rotation = Quaternion.LookRotation(newDirection);
        //transformTop.Rotate(new Vector3(90, 0, 0));
         */







        //transformTop.LookAt(desiredTransform, Vector3.left);
        //transformTop.Rotate(0, 0, Vector3.Angle(desiredTransform.position - transformTop.position, Vector3.up));

        if (Vector3.Distance(positions[positions.Count - 1], positions[positions.Count - 2]) > maxSectionSize)
        {
            InsertSection();
        }
        if (tubeRenderer != null)
        {
            tubeRenderer.SetPositions(positions.ToArray());
        }
    }

    public void SpawnLeaf(GameObject leaf)
    {
        GameObject newLeaf = PoolManager.instance.ReuseObject(leaf.gameObject, positions[positions.Count - 2], transform.rotation);
        newLeaf.GetComponentInChildren<Leaf>().RollOut();
        //GameObject newLeaf = Instantiate(leaf.gameObject);

        //newLeaf.transform.localRotation = (Quaternion.LookRotation(positions[positions.Count - 2] - positions[positions.Count - 1] + Vector3.right));
        newLeaf.transform.Rotate(new Vector3(0, Random.value * 360, 0));
        //newLeaf.transform.position = positions[positions.Count - 2];
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
    public void End(GameObject blossomObject)
    {
        Instantiate(blossomObject, transform).transform.position = transformTop.position;

        transformTop.gameObject.SetActive(false);
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(desiredPos, 1);
    }
}
