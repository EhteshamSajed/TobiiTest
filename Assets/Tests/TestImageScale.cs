using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestImageScale : MonoBehaviour
{
    public float standardDeviation = 0.184f;
    public float mean = 2.74f;
    public Image refBgImage, refFgImage;
    public Image liveFbImage;
    float minPupilSize = 2.20f;
    float maxPupilSize = 3.20f;
    public Text FpsText;
    long ticksUpdate;
    System.Random random = new System.Random();
    // Start is called before the first frame update
    void Start()
    {
        refBgImage.transform.localScale = Vector2.one * (mean + standardDeviation) / mean;
        refFgImage.transform.localScale = Vector2.one * (mean - standardDeviation) / mean;
    }

    // Update is called once per frame
    void Update()
    {
        float pupilSize = (float)(random.NextDouble() * (maxPupilSize - minPupilSize) + minPupilSize);
        liveFbImage.transform.localScale = Vector2.one * (pupilSize) / mean;
        Debug.Log ("Update elapsed ticks: " + TimeSpan.FromTicks(System.DateTime.Now.Ticks - ticksUpdate).TotalSeconds);
        ticksUpdate = System.DateTime.Now.Ticks;
        FpsText.text = "" + 1.0f / Time.smoothDeltaTime;
    }
}
