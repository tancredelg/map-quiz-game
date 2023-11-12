using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SerializationManager
{
    private static readonly string SavePath = Application.persistentDataPath + "/saves/";
    
    private static string _quizToLoadPath = "";

    public static void SetQuizToLoadPath(string path) => _quizToLoadPath = path;
    
    public static bool SaveExists(string saveName) => File.Exists(SavePath + saveName + ".json");

    public static void SaveQuiz(QuizData saveData)
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);

        File.WriteAllText(SavePath + saveData.Name + ".json", JsonUtility.ToJson(saveData));
    }
    
    public static QuizData LoadQuiz() => LoadQuiz(_quizToLoadPath);

    private static QuizData LoadQuiz(string path)
    {
        try
        {
            string jsonData = File.ReadAllText(path);
            return JsonUtility.FromJson<QuizData>(jsonData);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static string[] GetAllQuizPaths()
    {
        if (!Directory.Exists(SavePath))
            return Array.Empty<string>();
        
        return Directory.GetFiles(SavePath);
    }

    public static bool GetQuizDetails(string quizPath, out string name, out string[] locations)
    {
        name = "";
        locations = Array.Empty<string>();
        
        var quizData = LoadQuiz(quizPath);
        if (quizData == null)
            return false;
        
        name = quizData.Name;
        locations = quizData.LocationsData.Select(l => l.Name).ToArray();
        return true;
    }
    
    private static string GetLastQuizPath()
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);

        if (Directory.GetFiles(SavePath).Length == 0)
        {
            Debug.LogError("No save files found.");
            return null;
        }

        return Directory.GetFiles(SavePath).OrderByDescending(File.GetLastAccessTime).First();
    }
}