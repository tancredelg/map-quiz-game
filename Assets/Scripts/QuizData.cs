using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class QuizData
{
    public string Name;
    public int GuessesAllowed;
    public LocationData[] LocationsData;

    public QuizData()
    {
        Name = "Untitled Quiz";
        GuessesAllowed = 3;
        LocationsData = Array.Empty<LocationData>();
    }

    [Serializable]
    public class LocationData
    {
        public string Name;
        public Vector3 LocalPosition, LocalEulerAngles, LocalScale;

        public LocationData(LocationManager location)
        {
            Name = location.Name;
            LocalPosition = location.Parent.localPosition;
            LocalEulerAngles = location.Parent.localEulerAngles;
            LocalScale = location.Parent.localScale;
        }

        public override string ToString() =>
            $"{Name} - LocalPosition: {LocalPosition} - LocalEulerAngles: {LocalEulerAngles} - LocalScale: {LocalScale}";
    }
}