﻿using System.Collections;
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
    private GameObject leafObject;

    [SerializeField]
    private float verticalSpeed = 1f;

    [SerializeField]
    private Transform cameraPivot;

    public Vector3 topPosition;

    private PlantState state = PlantState.growing;
    private BranchFormation branchFormation = BranchFormation.wave;
    void Start()
    {
        topPosition = transform.position;
        branches = new List<Branch>();

        StartGrowing();

        StartCoroutine(spawningTest());

    }
    IEnumerator spawningTest()
    {
        yield return new WaitForSeconds(0.5f);
        SpawnLeafs();
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
            SpawnBranch(branches[0]);
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
    public void UpdateFormation()
    {
        switch (branchFormation)
        {
            case BranchFormation.wave:
                float waveSpeed = 3f;
                float waveAmplitude = 20f;
                float gapBetweenBranches = 3f;
                for (int i = 0; i < branches.Count; i++)
                {
                    branches[i].desiredPos.x = Mathf.Cos(Time.time * waveSpeed + (i * .5f)) * waveAmplitude;
                    branches[i].desiredPos.z = (i - (float)branches.Count / 2f) * gapBetweenBranches;
                    //Debug.Log(branches[i].desiredPos);
                }
                break;
        }
    }

}
