using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    public AudioClip clip;
    [SerializeField]
    private AudioSource audioS;
    [SerializeField]
    private Plant plant;

    [SerializeField]
    private CameraMovement camMovement;

    [SerializeField]
    private Material branchMaterial;
    private ColorChangeMaterial colorMaterialChanger;

    public int bpm;
    private bool bpmStarted = false;

    [SerializeField]
    public bool autoChange = true;

    private bool plantIncreasing = false;

    private float[] smoothSpectrum;

    public static PlantManager instance;
    private AudioProcessor processor;

    private float audioSeedValue = 0f;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        smoothSpectrum = new float[12];

        processor = FindObjectOfType<AudioProcessor>();
        processor.onBeat.AddListener(onOnbeatDetected);
        processor.onSpectrum.AddListener(onSpectrum);

        colorMaterialChanger = gameObject.AddComponent<ColorChangeMaterial>();
        colorMaterialChanger.Setup(branchMaterial);
    }

    public void PlayMusic()
    {
        bpm = UniBpmAnalyzer.AnalyzeBpm(clip);
        audioS.clip = clip;
        audioS.Play();
    }
    private void Update()
    {
        if (!audioS.isPlaying)
        {
            if (plant.state == PlantState.growing)
            {
                plant.End();
                camMovement.angleSpeed.y = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            camMovement.angleSpeed.y = 0;
            plant.End();
            audioS.Stop();
        }
    }
    private void UpdateAudioSeedValue()
    {
        //sort of random but gives same result on the same music
        if (smoothSpectrum.Length != 0)
        {
            audioSeedValue = (audioSeedValue + (smoothSpectrum[0] * 100f)) % 1f;
        }
        Debug.Log("seed value " + audioSeedValue + "   |  spectrum " + smoothSpectrum[0]);
    }

    void onOnbeatDetected()
    {
        //plant.SpawnLeafs();
        if (!bpmStarted)
        {
            bpmStarted = true;
            plant.StartGrowing();
            StartCoroutine(WaitBPM());
        }
    }

    //This event will be called every frame while music is playing
    void onSpectrum(float[] spectrum)
    {
        //The spectrum is logarithmically averaged
        //to 12 bands
        UpdateSpectrum(spectrum);
        plant.UpdateBranchSpectrum(smoothSpectrum);

        for (int i = 0; i < smoothSpectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i, 0, 0);
            Vector3 end = new Vector3(i, smoothSpectrum[i] * 10, 0);
            //Debug.DrawLine(start, end, Color.red);
        }
    }
    public void UpdateSpectrum(float[] input)
    {
        //Debug.Log(smoothSpectrum + " | " + input);
        if (smoothSpectrum == null)
        {
            smoothSpectrum = new float[input.Length];
            for (int i = 0; i < input.Length; ++i)
            {
                smoothSpectrum[i] = input[i];
            }
        }
        else
        {
            //Debug.Log("sizes: " + smoothSpectrum.Length + " | " + input.Length);
            if (smoothSpectrum.Length != input.Length)
            {
                smoothSpectrum = new float[input.Length];
                for (int i = 0; i < input.Length; ++i)
                {
                    smoothSpectrum[i] = input[i];
                }
            }
            else
            {

                for (int i = 0; i < input.Length; ++i)
                {
                    if (smoothSpectrum[i] >= input[i])
                    {
                        //Debug.Log("lowering...");
                        smoothSpectrum[i] -= Mathf.Min(0.01f, (smoothSpectrum[i] - input[i]) / 2f);
                    }
                    else
                    {
                        //Debug.Log("highering...");
                        smoothSpectrum[i] += Mathf.Max(0.01f, (smoothSpectrum[i] - input[i]) / 2f);
                    }
                }
            }
        }

    }

    IEnumerator WaitBPM()
    {
        while (audioS.isPlaying)
        {
            OnBPM();
            yield return new WaitForSeconds(60f / (float)bpm);
        }
    }
    int beats = 0;
    public void OnBPM()
    {
        beats++;
        UpdateAudioSeedValue();

        if (beats % 2 == 0)
        {
            OnTwoBPM();
        }
        if (autoChange)
        {
            if (beats % 4 == 0)
            {
                OnFourBPM();
            }
            if (beats % 8 == 0)
            {
                OnEightBPM();
            }
            if (beats % 16 == 0)
            {
                OnSixTeenBPM();
            }
            if (beats % 32 == 0)
            {
                OnThirtyTwoBPM();
            }
        }
    }
    public void OnTwoBPM()
    {
        plant.SpawnLeafs();

        if (plant.branchFormation == BranchFormation.random)
        {
            plant.RandomPos();
        }

    }
    public void OnFourBPM()
    {
    }
    public void OnEightBPM()
    {
        colorMaterialChanger.Pulse();

        //plant.verticalSpeed = 0.5f + audioSeedValue;

        //if (!plantIncreasing)
        //{
        //    if (audioSeedValue < 0.5f && plant.AmountOfBranches > 2)
        //    {
        //        plant.AmountOfBranches--;
        //    }
        //    else
        //    {
        //        plant.AmountOfBranches++;
        //    }
        //}
    }
    public void OnSixTeenBPM()
    {
        if (camMovement.angleSpeed.y == 0)
        {
            camMovement.angleSpeed.y = (audioSeedValue * 2f - 1f) * .3f;
        } else
        {
            camMovement.angleSpeed.y = 0;
        }

        StartCoroutine(camMovement.ChangingXAngle( 80f * audioSeedValue - 40f));

        if (!plantIncreasing)
        {
            StartCoroutine(
                ChangingBranchAmounts(1 + (int)(Mathf.Floor(audioSeedValue * 20f)))
                );
        }
    }

    public void OnThirtyTwoBPM()
    {
        Array values = Enum.GetValues(typeof(BranchFormation));
        plant.branchFormation = (BranchFormation)values.GetValue((int)(Mathf.Floor(audioSeedValue * values.Length)));
    }


    IEnumerator ChangingBranchAmounts(int value)
    {
        plantIncreasing = true;
        bool increasing = plant.AmountOfBranches < value;
        while (plant.AmountOfBranches != value)
        {
            if (Mathf.Abs(plant.AmountOfBranches - value) < 2)
            {
                plant.AmountOfBranches += increasing ? 1 : -1;
            } else
            {
                plant.AmountOfBranches += increasing ? 1 : -1;
            }
            yield return new WaitForSeconds(30f / (float)bpm);
        }
        plantIncreasing = false;
    }
}
