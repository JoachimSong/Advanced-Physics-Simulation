using UnityEngine;
using System.Collections.Generic;
using System;

// List<T>
[Serializable]
public class SerializationList<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public SerializationList(List<T> target)
    {
        this.target = target;
    }
}

public class SerializeList
{
    public static string ListToJson<T>(List<T> l)
    {
        return JsonUtility.ToJson(new SerializationList<T>(l));
    }

    public static List<T> ListFromJson<T>(string str)
    {
        return JsonUtility.FromJson<SerializationList<T>>(str).ToList();
    }
}