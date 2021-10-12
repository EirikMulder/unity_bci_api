using System;
using System.Collections.Generic;
using SensorAPI.Data;
using SensorAPI.Interfaces;

namespace SensorAPI
{
namespace Testing
{
/** @brief A test controller emulating a fake device.
 *  Can be used to test applications which rely on a device,
 *  as well as testing out the Data library on fake data sets.
 */
public class Tester : IController
{
    /** @brief The data */
    public Dictionary<string, DataList> Data { get; private set; }
    /** @brief Is the fake device connected. Typically true. */
    public bool IsConnected { get; set; }
    /** @brief Is the Data attribute being refreshed and is the device on (if it can be turned off). */
    public bool IsUpdating { get; set; }
    /** @brief Starts the data stream */
    public void Start()
    {
    }

    /** @brief Stops the data stream */
    public void Stop()
    {
    }

    /** @brief Initializes the tester (currently just prints INIT) */
    public Tester()
    {
        Console.WriteLine("INIT");
    }

    ~Tester()
    {
    }
}
}
} // SensorAPI
