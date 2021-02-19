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

    public static string path = "";
     public static int latestLogNumber = 0;
    public static void ApplicationStartedCheckLogFilesAndChangeNumber()
    {
        Debug.Log("ELO MORDY");
            if (Application.platform == RuntimePlatform.Android)
            {
                path = Application.persistentDataPath+$"\\{latestLogNumber.ToString("000")}_Game_Combat_Logs.txt";
            }
            else
            {
                path = $"C:\\Users\\MrKom\\Desktop\\ORLOG_Logs\\{latestLogNumber.ToString("000")}_Which_Player_Attack_First_and_how_many_rounds.txt";
                Debug.Log("szukanacja to: "+path);
            }

            while (true)
            {   
                Debug.Log("szukaj lognumber : "+latestLogNumber);
                if (File.Exists(path))
                {
                    Debug.Log("juz istnieje"+path);
                    latestLogNumber++;
                    path = path.Replace((latestLogNumber-1).ToString("000"),latestLogNumber.ToString("000"));
                }

                if(!File.Exists(path))
                {
                    Debug.Log("nowa ścieżka = "+path);
                    using (StreamWriter sw = File.CreateText(path))
                    {
                        sw.Write("Kto Zaczął grę\tkto pierwszy rollował w rundzie\tNumer rundy\tkto atakuje w tej rundzie\t Zwyciezca\t P1_HP\t P1_Gold\t P2_HP\t P2_Gold\n");
                    }
                break;
                }
            }
    }
    public static void Log_Which_Player_Attack_First_and_how_many_rounds(
        string whoStartGameSession = "", 
        string whoRollinCurrentRund ="", 
        string numberOfRund = "",
        string whoAttackedFirstInThisRund ="", 
        string winner ="",

        string P1_HP = "",
        string P1_Gold = "",
        string P2_HP = "",
        string P2_Gold = ""
        )
        {        
            // This text is added only once to the file.

            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.Write("Kto Zaczął grę\tkto pierwszy rollował w rundzie\tNumer rundy\tkto atakuje w tej rundzie\t Zwyciezca\t P1_HP\t P1_Gold\t P2_HP\t P2_Gold\n");
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine($"{whoStartGameSession}\t{whoRollinCurrentRund}\t{numberOfRund}\t{whoAttackedFirstInThisRund}\t{winner}\t{P1_HP}\t{P1_Gold}\t{P2_HP}\t{P2_Gold}");
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