using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SensorAPI.Controllers;
using System;

namespace SensorAPI {
    namespace Testing {
        public class TestBehavior : MonoBehaviour
        {
            HEGduino heg;
            public string portLocation = "/dev/tty.usbserial-01DFAAE3";
            public Text latest;
            public Text connected;
            public Text stable;
            public Text ambient;
            public Text light;
            StreamWriter primary_data_file;
            StreamWriter raw_data_file;
            string raw_filename;
            string primary_filename;
            DateTime last_written;
            bool started = false;

            public void StartHEG()
            {
                print("Start INIT");
                heg = new HEGduino(portLocation);
                heg["brain_bloodflow"].limit = 30;
                heg["brain_bloodflow"].deviationLimit = 1.5;
                heg.Start();
                print("Finish INIT");
                Directory.CreateDirectory("HEG_Data");
                raw_filename = "HEG_Data/RAWDATA_" + DateTime.Now.ToString("MM:dd:yy_(ddd)_HH.mm.ss") + ".csv";
                primary_filename = "HEG_Data/HEG_Data_" + DateTime.Now.ToString("MM:dd:yy_(ddd)_HH.mm.ss") + ".csv";
                primary_data_file = new StreamWriter(primary_filename);
                raw_data_file = new StreamWriter(raw_filename);
                primary_data_file.WriteLine("time,data,weighted_avg01,weighted_avg1,weighted_avg5,moving_avg10,avg");
                raw_data_file.WriteLine("time,red,ir,ratio,ambient,vel,accel");
                started = true;
            }

            public void StopHEG()
            {
                heg.Stop();
                File.Copy(raw_filename, "latest_raw.csv", true);
                File.Copy(primary_filename, "latest.csv", true);
                started = false;
            }

            public void Update()
            {
                //TODO: Change IsUpdating name
                if (started && heg.IsUpdating)
                {
                    if (heg.IsConnected) { connected.text = "Connected"; } else { connected.text = "Disconnected"; }
                    if (heg.IsStable) { stable.text = "Stable"; } else { stable.text = "Unstable"; }
                    if (heg.IsDark) { light.text = "Good ambient light"; } else { light.text = "Too much ambient light!"; }
                    if (heg["brain_bloodflow"].Count > 0 && heg["brain_bloodflow"].LastUpdate() > last_written)
                    {
                        primary_data_file.WriteLine(
                            heg["brain_bloodflow"].LastUpdate().ToString("m:s.ff") + "," +
                            (double)heg["brain_bloodflow"] + "," + heg["brain_bloodflow"].WeightedAverage(0.1) + "," +
                            heg["brain_bloodflow"].WeightedAverage(1) + "," + heg["brain_bloodflow"].WeightedAverage(5) + "," +
                            heg["brain_bloodflow"].MovingAverage(10) + "," + heg["brain_bloodflow"].Average()
                        );
                        raw_data_file.WriteLine(
                            heg["brain_bloodflow"].LastUpdate().ToString("m:s.ff") + "," +
                            heg["red_light"] + "," +
                            heg["ir_light"]+ "," +
                            heg["brain_bloodflow"] + "," +
                            heg["ambient_light"] + "," +
                            heg["velocity"] + "," +
                            heg["acceleration"]
                        );
                        last_written = heg["brain_bloodflow"].Latest().time;
                        latest.text = heg["brain_bloodflow"].WeightedAverage(1).ToString("0.###");
                        ambient.text = heg["ambient_light"];
                        
                    }
                }
                if (started && !heg.IsConnected)
                {
                    print("Disconnected.");
                }
            }

            public void OnDestroy()
            {
                heg.Stop();
                File.Copy(raw_filename, "latest_raw.csv", true);
                File.Copy(primary_filename, "latest.csv", true);
            }
        }
    }
}