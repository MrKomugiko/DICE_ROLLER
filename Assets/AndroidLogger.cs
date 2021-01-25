using System.Collections.Generic;
using UnityEngine;
using TMPro;

public static class AndroidLogger
{
    static int LoggerLineCounter = 0;
    static List<string> Logs = new List<string>();
    
    public static void Log(string newLog, string color = "white")
    {
        string message = "";
    
        LoggerLineCounter ++;
        
        if(LoggerLineCounter == 15)
        {
            Logs.RemoveAt(0);
            LoggerLineCounter = 14;
        }
         
        Logs.Add($"<color=\"{color}\">"+newLog+"</color>");

        foreach(string log in Logs)
        {
            message += log + "\n";
        }
        
        GameObject.Find("ANDROIDLOGGER_TMP").GetComponent<TextMeshProUGUI>().SetText(message);
    }
}