using System;
using System.IO;
using System.Linq;
using UnityEngine;

public static class SerializationManager
{
    private static readonly string SavePath = Application.persistentDataPath + "/saves/";
    
    private static string _quizLoadPath = "";

    public static void SetQuizLoadPath(string path) => _quizLoadPath = path;
    
    public static bool SaveExists(string saveName) => File.Exists(SavePath + saveName + ".json");

    public static void SaveQuiz(QuizData saveData)
    {
        if (!Directory.Exists(SavePath))
            Directory.CreateDirectory(SavePath);

        File.WriteAllText(SavePath + saveData.Name + ".json", JsonUtility.ToJson(saveData));
    }
    
    public static QuizData LoadQuizData() => LoadQuizData(_quizLoadPath);

    private static QuizData LoadQuizData(string path)
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