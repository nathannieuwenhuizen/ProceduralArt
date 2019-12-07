using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasOptions : MonoBehaviour
{

    [SerializeField]
    private Plant plant;

    [SerializeField]
    private CameraMovement camMovement;

    //start screen
    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private Dropdown clipsDropDown;

    //editor
    [SerializeField]
    private Slider branchSlider;
    [SerializeField]
    private Text branchText;

    [SerializeField]
    private Toggle autoToggle;

    [SerializeField]
    private Slider cameraRotSlider;

    [SerializeField]
    private Slider verticalSlider;

    [SerializeField]
    private Slider spectrumSlider;

    [SerializeField]
    private Dropdown formationDorpDown;

    // Start is called before the first frame update
    void Start()
    {

        List<Dropdown.OptionData> clipsOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < clips.Length; i++)
        {
            clipsOptions.Add(new Dropdown.OptionData(clips[i].name));
        }
        clipsDropDown.AddOptions(clipsOptions);

        List<Dropdown.OptionData> formationOptions = new List<Dropdown.OptionData>();
        for (int i = 0; i < Enum.GetNames(typeof(BranchFormation)).Length; i++)
        {
            string name = ((BranchFormation)i).ToString();
            formationOptions.Add(new Dropdown.OptionData(name));
        }
        formationDorpDown.AddOptions(formationOptions);

        UpdateOptions();

        SelectClip();
    }

    public void ValueChangeCheck()
    { 
        plant.AmountOfBranches = (int)branchSlider.value;
        branchText.text = plant.AmountOfBranches + " branches";

        camMovement.angleSpeed.y = cameraRotSlider.value;
        plant.verticalSpeed = verticalSlider.value;
        plant.spectrumAmplitude = spectrumSlider.value;

        PlantManager.instance.autoChange = autoToggle.isOn;
        plant.branchFormation = (BranchFormation)formationDorpDown.value;
    }
    public void UpdateOptions()
    {
        branchSlider.value = plant.AmountOfBranches;
        branchText.text = plant.AmountOfBranches + " branches";

        cameraRotSlider.value = camMovement.angleSpeed.y;
        verticalSlider.value = plant.verticalSpeed;
        spectrumSlider.value = plant.spectrumAmplitude;

        formationDorpDown.value = (int)plant.branchFormation;

        autoToggle.isOn = PlantManager.instance.autoChange;

    }
    public void SelectClip()
    {
        PlantManager.instance.clip = clips[clipsDropDown.value];
    }

    // Update is called once per frame
    void Update()
    {
        UpdateOptions();
    }
}
