using UnityEngine;
using System.Collections.Generic;
using System.IO.Ports;

public class BCIController : MonoBehaviour
{
    // Variables
    List<BCISensor> sensors = new List<BCISensor>();
    HEGduino heg;
    [Header("Data")]
    public double score;
    public double weightedScore;
    public double slope;
    public double weightedSlope;
    long time;
    public long elapsedTime;

    public void Start()
    {
        print("Starting");
        heg = GetComponent<HEGduino>();
        // TODO: Let user set up device however desired.
        heg.Connect("/dev/tty.usbserial-01DFAAD5");
        sensors.Add(heg);
        print("Starting Finished");
    }

    public void Update()
    {
        //HEGduino
        if (heg.time != time)
        {
            slope = (heg.score - score)*10/((heg.time - time)/61000);
            weightedSlope = (heg.weightedScore - weightedScore) * 10 / ((heg.time - time) / 61000);
            score = heg.score;
            weightedScore = heg.weightedScore;
            // TODO: Debug Variable
            elapsedTime = heg.time - time;
            time = heg.time;
        }
    }

    public void OnDestroy()
    {
        foreach (BCISensor sensor in sensors)
        {
            sensor.Destroy();
        }
    }
}
