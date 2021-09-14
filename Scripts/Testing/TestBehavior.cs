using UnityEngine;
using SensorAPI;

public class TestBehavior : MonoBehavior
{
    HEGduino heg;
    StreamWriter file = new StreamWriter("test_data.csv");

    public void Start()
    {
        heg = new HEGduino("COM4");
        heg.Data["brain_bloodflow"].limit = 30;
        heg.Data["brain_bloodflow"].deviationLimit
        Print("INIT");
        file.WriteLine("data,avg1,avg0.5");
    }

    public void Update()
    {
        if (IsUpdating)
        {
            Print("Data: " + heg.Data["brain_bloodflow"]);
            Print("AVG Data: " + heg.Data["brain_bloodflow"].WeightedAverage(0.5));
            file.WriteLine(heg.Data["brain_bloodflow"] + "," + heg.Data["brain_bloodflow"].WeightedAverage(1) + "," + heg.Data["brain_bloodflow"].WeightedAverage(0.5))
        }
        if (!IsConnected)
        {
            Print("Disconnected.")
        }
    }
}
