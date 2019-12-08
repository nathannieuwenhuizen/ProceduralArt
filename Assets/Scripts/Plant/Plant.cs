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
    flocking,
    layeredCircle
}
public class Plant : MonoBehaviour
{
    private List<Branch> branches;

    private GameObject flowerTop;
    private GameObject flowerBLossom;

    [SerializeField]
    private GameObject branchObject;

    [SerializeField]
    public float spectrumAmplitude = 5f;

    [SerializeField]
    private GameObject leafObject;

    [SerializeField]
    public float verticalSpeed = 1f;

    [SerializeField]
    private Transform cameraPivot;

    [SerializeField]
    private GameObject flowerObject;

    [SerializeField]
    private GameObject finalFlowerObject;

    [SerializeField]
    private ParticleSystem groundBusrtParticle;

    public Vector3 topPosition;

    public PlantState state = PlantState.growing;
    public BranchFormation branchFormation = BranchFormation.circle;

    void Start()
    {
        topPosition = transform.position;
        branches = new List<Branch>();

        PoolManager.instance.CreatePool(leafObject, 2);
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
        groundBusrtParticle.Play();
        SpawnBranch();
    }

    private bool blossoming = false;
    public void End()
    {
        if (!blossoming)
        {
            blossoming = true;
            StartCoroutine(Ending());
        }
    }
    IEnumerator Ending()
    {
        while (AmountOfBranches > 0)
        {
            AmountOfBranches--;
            yield return new WaitForSeconds((60f / PlantManager.instance.bpm) * .5f);
        }
        state = PlantState.blossoming;
    }

    public void SpawnBranch(Branch OtherBranch = null)
    {

        Branch branch = Instantiate(branchObject).GetComponent<Branch>();
        branch.name = "branch #" + branches.Count;
        branch.transform.parent = transform;
        branches.Add(branch);

        //Debug.Log("Other: " + OtherBranch);
        if (OtherBranch != null)
        {
            branch.SetPos(OtherBranch.currentPos);
        } else
        {
            //Debug.Log("spawns at transform parent");
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
                branches[i].Grow(topPosition.y, branchFormation != BranchFormation.random, branchFormation != BranchFormation.flocking);
            }

            Vector3 cameraPos = cameraPivot.position;
            cameraPos.y = topPosition.y;
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, cameraPos, Time.deltaTime * 10f);
        }
    }

    public void UpdateBranchSpectrum(float[] spectrum)
    {
        if (spectrum.Length != 0 && branches.Count != 0)
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
                    SpawnBranch(branches[Random.Range(0, branches.Count - 1)]);
                }
            } else
            {
                for (int i = branches.Count; i > value; i--)
                {
                    int index = Random.Range(0, value - 1);
                    if (branches.Count == 1)
                    {
                        branches[index].End(finalFlowerObject);
                    }
                    else
                    {
                        branches[index].End(flowerObject);
                    }
                    branches.Remove(branches[index]);
                }
            }
        }
    }

    public void RandomPos()
    {
        for (int i = 0; i < branches.Count; i++)
        {
            float range = (float)branches.Count * .5f;
            branches[i].desiredPos = new Vector3(Random.Range(-range, range), topPosition.y, Random.Range(-range, range));
        }

    }
    public void UpdateFormation()
    {
        switch (branchFormation)
        {
            case BranchFormation.flocking:
                UpdateFlocking();
                break;
            case BranchFormation.wave:
                float waveSpeed = 3f;
                float waveAmplitude = 10f;
                float gapBetweenBranches = 3f;
                for (int i = 0; i < branches.Count; i++)
                {
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * waveSpeed + (i * .5f)) * waveAmplitude;
                    branches[i].desiredPos.z = (i - (float)branches.Count / 2f) * gapBetweenBranches;
                }
                break;
            case BranchFormation.circle:
                for (int i = 0; i < branches.Count; i++)
                {
                    float circleSpeed = 3f / (1 + (float)branches.Count / 2);
                    float circleSize = Mathf.Max(5, (float)branches.Count * 0.8f);
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * circleSpeed + (i * (Mathf.PI * 2f / (float)branches.Count))) * circleSize;
                    branches[i].desiredPos.z = Mathf.Sin(Time.time * circleSpeed + (i * (Mathf.PI * 2f / (float)branches.Count))) * circleSize;
                }
                break;

            case BranchFormation.layeredCircle:
                float layeredCircleSpeed = 3f;
                float layeredCircleSize = 0;
                float index = Mathf.PI * 2;
                float branchDistance = 2f;
                float left = -1;
                float radianDistance = branchDistance;
                for (int i = 0; i < branches.Count; i++)
                {
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * layeredCircleSpeed * left + index) * layeredCircleSize;
                    branches[i].desiredPos.z = Mathf.Sin(Time.time * layeredCircleSpeed * left + index) * layeredCircleSize;

                    //if (i == branches.Count - 1)
                    //{
                    //    Debug.Log("index is: " + index); 
                    //}

                    index += radianDistance;
                    if (index + (radianDistance / 2) >=  Mathf.PI * 2)
                    {
                        //Debug.Log("new layer!");
                        index = 0;
                        layeredCircleSize += 8f;
                        layeredCircleSpeed = 3f / (layeredCircleSize / 8f);
                        radianDistance = branchDistance / (layeredCircleSize / 8f);
                        if (((branches.Count - 1) - i) * radianDistance < Mathf.PI * 2)
                        {
                            radianDistance = (Mathf.PI * 2) / ((branches.Count - 1) - i);
                        }
                        left = left == 1 ? -1 : 1;
                    }
                }
                break;

            case BranchFormation.stripe:
                float stripeRotationSpeed = 1f;
                float branchPadding = 3f;

                for (int i = 0; i < branches.Count; i++)
                {
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * stripeRotationSpeed) * (i * branchPadding * 2 -  ((float)branches.Count - 1) * branchPadding);
                    branches[i].desiredPos.z = Mathf.Sin(Time.time * stripeRotationSpeed) * (i * branchPadding * 2 - ((float)branches.Count - 1) * branchPadding);
                } 

                break;
        }
    }
    public void UpdateFlocking()
    {
        for (int i = 0; i < branches.Count; i++)
        {
            Vector3 seperationDir, cohesionDir, allignDir;


            allignDir = new Vector3();
            List<Branch> allignNeighbours = Neighbours(branches[i], 6f);
            for (int j = 0; j < allignNeighbours.Count; j++)
            {
                allignDir += allignNeighbours[j].currentSpeed;
            }
            allignDir /= allignNeighbours.Count;
            allignDir.Normalize();


            cohesionDir = new Vector3();
            List<Branch> cohesionNeighbours = Neighbours(branches[i], 10f);
            for (int j = 0; j < cohesionNeighbours.Count; j++)
            {
                cohesionDir += cohesionNeighbours[j].currentPos;
            }
            cohesionDir /= cohesionNeighbours.Count;
            cohesionDir -= branches[i].currentPos;
            cohesionDir.Normalize();

            seperationDir = new Vector3();
            List<Branch> seperationNeighbours = Neighbours(branches[i], 3f);
            for (int j = 0; j < seperationNeighbours.Count; j++)
            {
                seperationDir -= (seperationNeighbours[j].currentPos - branches[i].currentPos) / Vector3.Distance(seperationNeighbours[j].currentPos, branches[i].currentPos);
            }
            //seperationDir *= -1f;
            seperationDir.Normalize();


            Vector3 newSpeed = (allignDir + cohesionDir + seperationDir * 3f).normalized;
            if (i == 0)
            {
                Debug.Log("speed: " + cohesionDir + " | " + seperationDir + " | " + allignDir );
            }
            //Debug.DrawLine(branches[i].currentPos, branches[i].currentPos + allignDir * 2f, Color.green);
            //Debug.DrawLine(branches[i].currentPos, branches[i].currentPos + cohesionDir * 2f, Color.red);
            //Debug.DrawLine(branches[i].currentPos, branches[i].currentPos + seperationDir * 2f, Color.blue);


            if (newSpeed.x == 0 && newSpeed.z == 0)
            {
                newSpeed = new Vector3(Random.value, Random.value, Random.value);
            }

            newSpeed = (newSpeed + (topPosition - branches[i].currentPos) * 0.5f).normalized;
            newSpeed *= 0.04f;

            branches[i].currentSpeed += newSpeed;
        }
    }

    public List<Branch> Neighbours(Branch branch, float maxDistance)
    {
        List<Branch> result = new List<Branch>();
        for (int i = 0; i < branches.Count; i++)
        {
            if (branch != branches[i])
            {
                if (Vector3.Distance(branch.currentPos, branches[i].currentPos) < maxDistance) {
                    result.Add(branches[i]);
                }

            }
        }
        return result;
    }

}
