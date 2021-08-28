using System.Collections.Generic;
using System;

/* IController interface defines how a BCI device driver should
 * behave.
 *
 * The constructor should automatically set up the device connection,
 * and may optionally take parameters to specify the correct device.
 *
 * the data property is a Dictionary with string keys (lowercase), and DataList objects.
 *
 * The DataList object (defined in Data.cs) is a list of 'TimedData' objects, 
 * and can take averages, weighted averages, and preform other operations on the data.
 *
 */
public interface IController
{
    Dictionary<string, DataList> data { get; } // The latest data from the device.
    bool isConnected { get; } // Is the device connected to the PC?
    bool isUpdating { get; } // Is the data property being updated by the device?
    void Start(); // Start data collection, if connected.
    void Stop(); // Stop data collection.
}
