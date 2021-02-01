using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class AndroidLogger
{
    static int LoggerLineCounter = 0;
    static List<string> Logs = new List<string>();
    static int messageMaxSize = 15;
    
    public static void Log(string newLog, string color = "white")
    {
        string message = "";
    
        LoggerLineCounter ++;
        
        if(LoggerLineCounter == messageMaxSize)
        {
            Logs.RemoveAt(0);
            LoggerLineCounter = messageMaxSize-1;
        }
         
        Logs.Add($"<color=\"{color}\">"+newLog+"</color>");

        foreach(string log in Logs)
        {
            message += log + "\n";
        }
        
        GameObject.Find("ANDROIDLOGGER_TMP").GetComponent<TextMeshProUGUI>().SetText(message);
    }

    public static string GetPlayerLogColor(string playerName) => playerName == "Player1" ? "green" : "red";
}