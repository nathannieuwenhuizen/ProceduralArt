using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioSource audioS;
    [SerializeField]
    private Plant plant;

    public int bpm;
    private bool bpmStarted = false;

    private float[] smoothSpectrum;

    public static PlantManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioS.clip = clip;
        audioS.Play();

        smoothSpectrum = new float[12];
        bpm = UniBpmAnalyzer.AnalyzeBpm(clip);

        AudioProcessor processor = FindObjectOfType<AudioProcessor>();
        processor.onBeat.AddListener(onOnbeatDetected);
        processor.onSpectrum.AddListener(onSpectrum);
    }
    private void Update()
    {
        if (!audioS.isPlaying)
        {
            if (plant.state != PlantState.blossoming)
            {
                plant.End();
            }
        }
    }

    void onOnbeatDetected()
    {
        Debug.Log("Beat!!!");
        //plant.SpawnLeafs();
        if (!bpmStarted)
        {
            bpmStarted = true;
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
        //Debug.Log("average..." + smoothSpectrum[0] + " | " + input[0]);

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
        if (beats % 2 == 0)
        {
            plant.SpawnLeafs();
            if (plant.branchFormation == BranchFormation.random)
            {
                plant.RandomPos();
            }
        }
        //Debug.Log("bam! " + beats);
    }
}
