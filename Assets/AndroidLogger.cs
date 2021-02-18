using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public static class AndroidLogger
{
    static int LoggerLineCounter = 0;
    static List<string> Logs = new List<string>();
    static int messageMaxSize = 15;

    public static void Log(string newLog, string color = "white")
    {
        string message = "";

        LoggerLineCounter++;

        if (LoggerLineCounter == messageMaxSize)
        {
            Logs.RemoveAt(0);
            LoggerLineCounter = messageMaxSize - 1;
        }

        Logs.Add($"<color=\"{color}\">" + newLog + "</color>");

        foreach (string log in Logs)
        {
            message += log + "\n";
        }
        GameObject.Find("ANDROIDLOGGER_TMP").GetComponent<TextMeshProUGUI>().SetText(message);
    }

    public static string GetPlayerLogColor(string playerName) => playerName == "Player1" ? "green" : "red";


    public static void Log_Result_Affect_Of_A_Priority_To_Win_Chance(string firstRollerAtStartOfGame, string firstAttacker, string winner)
    {
        string path = @"C:\Users\MrKom\Desktop\ORLOG_Logs\Affect_OF_A_Priority_To_Win_Chance.txt";
        // This text is added only once to the file.
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write("Pierwszy rollujący\tPierwszy atakujący\tZwyciezca\n");
            }
        }

        // This text is always added, making the file longer over time
        // if it is not deleted.
        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine($"{firstRollerAtStartOfGame}\t{firstAttacker}\t{winner}");
        }
    }
    public static void Log_Which_Player_Attack_First_and_how_many_rounds(
        string whoStartGameSession = "", 
        string whoRollinCurrentRund ="", 
        string numberOfRund = "",
        string whoAttackedFirstInThisRund ="", 
        string winner ="")
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Log("saving logs only for windows platform");
        }
        else
        {
            string path = @"C:\Users\MrKom\Desktop\ORLOG_Logs\Which_Player_Attack_First_and_how_many_rounds.txt";
            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write("Kto Zaczął grę\tkto pierwszy rollował w rundzie\tNumer rundy\tkto rolluje w tej rundzie\tkto atakuje w tej rundzie\t Zwyciezca\n");
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"{whoStartGameSession}\t{whoRollinCurrentRund}\t{numberOfRund}\t{whoAttackedFirstInThisRund}\t{winner}");
            }

        }
    }
    public static void DisplayFileContent(string fileName)
    {
        // Open the file to read from.
        using (StreamReader sr = File.OpenText($"C:\\Users\\MrKom\\Desktop\\ORLOG_Logs\\{fileName}.txt"))
        {
            string s = "";
            while ((s = sr.ReadLine()) != null)
            {
                Debug.Log(s);
            }
        }
    }
}