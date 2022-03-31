using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor;

public class HandleTextFile : MonoBehaviour
{
    // Check how many times the file name has been written
    static int nameCount = 0;
    // check how many times the hour portion of the current time has been parsed from a string to an int. 
    static int parseCount = 0;

    // Set the current time in the format of "HH : mm"
    static string time = DateTime.UtcNow.ToLocalTime().ToString("HH:mm");
    //Set the current date in the format of "MM-dd"
    static string date = DateTime.Now.ToString("MM-dd");
    // Set the substring of the current time that represents the hour of day in the format "HH."This is used when parsing down below to check if it is AM or PM 
    static string hours = time.Substring(0, 2);
    // Set to the substring of the current time that represents the minutes of the day in the format of "mm"
    static string minutes = time.Substring(3, 2);
    // This method opens up a new file with in the format "Pre/PostMaze_date_hours-minutesAM/PM_name.txt” in the folder called ProjectData on the Desktop. 

    [MenuItem("Tools/Write file")]
    public static void WriteString(Vector3 point, string name)
    {
        StreamWriter writer;
        int startHour;

        if (parseCount <= 0)
        {
            int.TryParse(hours, out startHour);
            if (startHour > 12)
            {
                hours = (startHour - 12).ToString();
            }
            parseCount++;
        }
        else
        {
            string tempHour = time.Substring(0, 2);
            int tempHourNum;
            int.TryParse(tempHour, out tempHourNum);

            if (tempHourNum >= 12)
            {
                startHour = 100;
            }
            else
            {
                startHour = 1;
            }
        }

        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;

        string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        if (sceneName == "PreMazeTest")
        {
            if (startHour >= 12)
            {
                writer = new StreamWriter(path + "/ProjectData/PreMaze" + "_" + date + "_" + hours + "-" + minutes + "PM" + "_" + name + ".txt", true);

                if (nameCount <= 0 && nameCount < 2)
                {
                    writer.WriteLine("Pre-Maze Data - " + date + " - " + hours + ":" + minutes + "PM " + "- " + name + "\n");
                    nameCount++;
                }
            }
            else
            {
                writer = new StreamWriter(path + "/ProjectData/PreMaze" + "_" + date + "_" + hours + "-" + minutes + "AM" + "_" + name + ".txt", true);

                if (nameCount <= 0 && nameCount < 2)
                {
                    writer.WriteLine("Pre-Maze Data - " + date + " - " + hours + ":" + minutes + "AM " + "- " + name + "\n");
                    nameCount++;
                }
            }
        }
        else if (sceneName == "PostMazeTest")
        {
            Debug.Log(nameCount);
            if (startHour >= 12)
            {
                writer = new StreamWriter(path + "/ProjectData/PostMaze" + "_" + date + "_" + hours + "-" + minutes + "PM" + "_" + name + ".txt", true);

                if (nameCount == 1)
                {
                    writer.WriteLine("Post-Maze Data - " + date + " - " + hours + ":" + minutes + "PM " + "- " + name + "\n");
                    nameCount++;
                }
            }
            else
            {
                writer = new StreamWriter(path + "/ProjectData/PostMaze" + "_" + date + "_" + hours + "-" + minutes + "AM" + "_" + name + ".txt", true);

                if (nameCount == 1)
                {
                    writer.WriteLine("Post-Maze Data - " + date + " - " + hours + ":" + minutes + "AM " + "- " + name + "\n");
                    nameCount++;
                }
            }
        }
        else
        {
            writer = new StreamWriter(path + "/ProjectData/Test.txt", true);
        }

        writer.WriteLine("(" + (decimal)point.x + "," + (decimal)point.z + ")");
        writer.Close();
    }
}
