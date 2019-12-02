using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantState
{
    inGround,
    growing,
    blossoming
}
public enum BranchFormation
{
    random,
    circle,
    stripe,
    wave,
    flocking
}
public class Plant : MonoBehaviour
{
    private List<Branch> branches;

    private GameObject flowerTop;
    private GameObject flowerBLossom;

    [SerializeField]
    private GameObject branchObject;

    [SerializeField]
    private float spectrumAmplitude = 2f;

    [SerializeField]
    private GameObject leafObject;

    [SerializeField]
    private float verticalSpeed = 1f;

    [SerializeField]
    private Transform cameraPivot;

    public Vector3 topPosition;

    private PlantState state = PlantState.growing;
    [SerializeField]
    private BranchFormation branchFormation = BranchFormation.circle;
    void Start()
    {
        topPosition = transform.position;
        branches = new List<Branch>();

        StartGrowing();

        PoolManager.instance.CreatePool(leafObject, 2);
        StartCoroutine(spawningTest());

    }
    IEnumerator spawningTest()
    {
        yield return new WaitForSeconds(0.5f);
        //SpawnLeafs();
        StartCoroutine(spawningTest());
    }

    public void SpawnLeafs()
    {
        for (int i = 0; i < branches.Count; i++)
        {
            branches[i].SpawnLeaf(leafObject);
        }
    }
    public void StartGrowing()
    {
        state = PlantState.growing;
        SpawnBranch();
    }
    public void End()
    {

    }
    public void SpawnBranch(Branch OtherBranch = null)
    {

        Branch branch = Instantiate(branchObject).GetComponent<Branch>();
        branch.name = "branch #" + branches.Count;
        branch.transform.parent = transform;
        branches.Add(branch);

        Debug.Log("Other: " + OtherBranch);
        if (OtherBranch != null)
        {
            branch.SetPos(OtherBranch.currentPos);
        } else
        {
            Debug.Log("spawns at transform parent");
            branch.SetPos(transform.position);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBranch(branches[branches.Count - 1]);
        }

        if (state == PlantState.growing)
        {
            UpdateFormation();

            topPosition.y += verticalSpeed;
            for (int i = 0; i < branches.Count; i++)
            {
                branches[i].Grow(topPosition.y);
            }
            Vector3 cameraPos = cameraPivot.position;
            cameraPos.y = topPosition.y;
            cameraPivot.position = cameraPos;
        }
    }

    public void UpdateBranchSpectrum(float[] spectrum)
    {
        if (branches.Count < spectrum.Length)
        {
            float aspect =  spectrum.Length / branches.Count;
            for (int i = 0; i < branches.Count; i++)
            {
                branches[i].spectrumOffset = spectrum[Mathf.FloorToInt(i * aspect)] * spectrumAmplitude;
            }
        }
    }

    public int AmountOfBranches
    {
        get { return branches.Count; }
        set
        {
            if (value > branches.Count)
            {
                for (int i = branches.Count; i < value; i++)
                {
                    SpawnBranch(branches[branches.Count - 1]);
                }
            } else
            {
                for (int i = branches.Count; i > value; i--)
                {
                    branches[branches.Count - 1].SpawnLeaf(leafObject);
                    branches.Remove(branches[branches.Count - 1]);
                }
            }
        }
    }
    public void UpdateFormation()
    {
        switch (branchFormation)
        {
            case BranchFormation.wave:
                float waveSpeed = 3f;
                float waveAmplitude = 10f;
                float gapBetweenBranches = 3f;
                for (int i = 0; i < branches.Count; i++)
                {
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * waveSpeed + (i * .5f)) * waveAmplitude;
                    branches[i].desiredPos.z = (i - (float)branches.Count / 2f) * gapBetweenBranches;
                    //Debug.Log(branches[i].desiredPos);
                }
                break;
            case BranchFormation.circle:
                for (int i = 0; i < branches.Count; i++)
                {
                    float circleSpeed = 3f / ( 1 + (float)branches.Count / 2);
                    float circleSize = Mathf.Max(10, branches.Count * 1);
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * circleSpeed + (i * (Mathf.PI * 2f / (float)branches.Count))) * circleSize;
                    branches[i].desiredPos.z = Mathf.Sin(Time.time * circleSpeed + (i * (Mathf.PI * 2f / (float)branches.Count))) * circleSize;
                    //Debug.Log(branches[i].desiredPos);
                }
                break;

        }
    }

}
