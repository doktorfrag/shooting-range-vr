using System;
using System.IO;
using UnityEngine;
using UnityEngine.VR;
using System.Collections.Generic;

public class TrackHeadset : MonoBehaviour
{

    //serialized variables
    //select VR input
    [SerializeField]
    VRNode m_VRNode = VRNode.Head;

    //private variables
    private List<string> _headsetData = new List<string>();

    private void Update()
    {
        GetRotation();
    }

    private void GetRotation()
    {
        //get tracking data from headset, convert to Euler angles,
        //get UNIX timestamp, convert everything to a string, and add to list
        Quaternion quaternion = InputTracking.GetLocalRotation(m_VRNode);
        Vector3 euler = quaternion.eulerAngles;
        double dateReturn = Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        string timestamp = dateReturn.ToString();
        _headsetData.Add(string.Format("{0}, Euler {1}", timestamp, euler.ToString("F2")));
    }

    private void OnDestroy()
    {
        //must delete or move TXT file each time before game is restarted
        string path = "headsetData.txt";
        StreamWriter writer = new StreamWriter(path, true);
        if (_headsetData != null)
        {
            //write out each line of list to TXT file
            foreach (string listEntry in _headsetData)
            {
                writer.WriteLine(listEntry);
            }
        }
        writer.Close();
    }
}