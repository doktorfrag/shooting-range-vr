using System;
using UnityEngine;
using UnityEngine.UI;

public class UnixTimeStamp : MonoBehaviour {

    //serialized variables
    [SerializeField] private Text _unixTimeStamp;

    // Get UNIX time stamp and write to canvas object
    void Update () {
        double dateReturn = Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        _unixTimeStamp.text = dateReturn.ToString();
    }
}
