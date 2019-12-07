using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [SerializeField]
    private int amountPerLayer = 5;

    [SerializeField]
    private int layerAmount = 2;

    [SerializeField]
    private GameObject flowerLeaf;

    [SerializeField]
    private bool idle = false;
    private void Start()
    {
        Blossom();
    }
    public void Blossom()
    {
        for (int i = 0; i < layerAmount; i++)
        {
            for (int j = 0; j < amountPerLayer; j++)
            {
                Leaf newLeaf = GameObject.Instantiate(flowerLeaf, transform).GetComponentInChildren<Leaf>();
                newLeaf.transform.parent.parent = transform;
                newLeaf.transform.Rotate(new Vector3(0, 360 / amountPerLayer * j + ((i % 2) * 360 / amountPerLayer), 0));
                newLeaf.transform.Translate(new Vector3(0, (float)i / 2f, 0));
                newLeaf.transform.parent.transform.localScale = new Vector3(1f / ((float)i + 1f), 1 / ((float)i + 1f), 1f / ((float)i + 1f));
                newLeaf.RollOut(idle);
            }
        }
    }

}
