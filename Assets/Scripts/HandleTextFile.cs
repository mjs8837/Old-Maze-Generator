using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class HandleTextFile : MonoBehaviour
{
    static string fileName = "Test";

    [MenuItem("Tools/Write file")]
    public static void WriteString(Vector3 point)
    {
        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        StreamWriter writer = new StreamWriter(path + "/ProjectData/Test.txt", true);

        //StreamWriter writer = new StreamWriter(path + "/ProjectData/" + fileName + ".txt", true);

        writer.WriteLine("(" + (decimal)point.x + "," + (decimal)point.z + ")");
        writer.Close();
    }
}
