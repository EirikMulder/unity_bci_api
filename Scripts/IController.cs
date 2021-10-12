using System.Collections.Generic;
using SensorAPI.Data;

namespace SensorAPI
{
namespace Interfaces
{
/** @brief IController interface defines how a BCI device driver should
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
    /** The latest data from the device. */
    Dictionary<string, DataList> Data { get; }
    /** @brief Is the device connected to the PC? */
    bool IsConnected { get; }
    /** @brief Is the data property being updated by the device? */
    bool IsUpdating { get; }
    /** @brief Start data collection, if connected. */
    void Start();
    /** @brief Stop data collection. */
    void Stop();
}
} // namespace Interfaces
} // namespace SensorAPI
