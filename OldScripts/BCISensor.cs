using UnityEngine;

// Abstract BCISensor implements the base class which all sensors inherit from.
abstract public class BCISensor : MonoBehaviour
{
    // Status
    [Header("Status")]
    public bool connected;

    // Methods
    public abstract void Destroy();
}
