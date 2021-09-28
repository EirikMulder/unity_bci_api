using UnityEngine;
using SensorAPI;
using System.IO;

public class TestBehavior : MonoBehaviour
{
    HEGduino heg;
    public string portLocation = "/dev/tty.usbserial-01DFAAE3";
    StreamWriter file;

    public void Start()
    {
        heg = new HEGduino(portLocation);
        heg.Data["brain_bloodflow"].limit = 30;
        heg.Data["brain_bloodflow"].deviationLimit = 1.5;
        heg.Start();
        print("INIT");
        file = new StreamWriter("test_data.csv");
        file.WriteLine("data,avg1,avg0.5");
    }

    public void Update()
    {
        //TODO: Change IsUpdating name
        if (heg.IsUpdating)
        {
            //print("Data: " + (double)heg.Data["brain_bloodflow"]);
            print("Amount: " + heg.Data["brain_bloodflow"].data.Count);
            print("AVG Data: " + heg.Data["brain_bloodflow"].WeightedAverage(0.5));
            file.WriteLine(heg.Data["brain_bloodflow"] + "," + heg.Data["brain_bloodflow"].WeightedAverage(1) + "," + heg.Data["brain_bloodflow"].WeightedAverage(0.5));
        }
        if (!heg.IsConnected)
        {
            print("Disconnected.");
        }
    }
}
