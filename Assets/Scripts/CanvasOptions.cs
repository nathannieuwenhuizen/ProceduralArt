using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOptions : MonoBehaviour
{
    [SerializeField]
    private Slider branchSlider;

    [SerializeField]
    private Plant plant;
    // Start is called before the first frame update
    void Start()
    {
        branchSlider.value = plant.AmountOfBranches;
        branchSlider.onValueChanged.AddListener(delegate {
            ValueChangeCheck();
        });
    }
    public void ValueChangeCheck()
    {
        plant.AmountOfBranches = (int)branchSlider.value;

        //Debug.Log(branchSlider.value);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
