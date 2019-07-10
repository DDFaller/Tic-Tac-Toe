using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MachineLearning;
using Newtonsoft.Json;
using System.IO;

public class MergeScript : MonoBehaviour
{
    public string ori1;
    public string ori2;
    public string dest;

    public void Merge()
    {
        var dict1 = ReadData(ori1);
        var dict2 = ReadData(ori2);

        Dictionary<string, MarkData[]> mergedDict = new Dictionary<string, MarkData[]>();

        foreach (var item in dict1)
        {
            if (dict2.ContainsKey(item.Key)) mergedDict.Add(item.Key, SumMarkDatas(dict1[item.Key], dict2[item.Key]));

        }

        File.WriteAllText(dest, string.Empty);
        List<string> list = new List<string>();
        foreach (var key in mergedDict)
        {
            var json = JsonConvert.SerializeObject(key);
            list.Add(json);
        }
        File.AppendAllLines(dest, list);
    }

    public Dictionary<string, MarkData[]> ReadData(string path)
    {
        Dictionary<string, MarkData[]> temp = new Dictionary<string, MarkData[]>();
        string[] file = File.ReadAllLines(path);
        string line;

        int i = 0;
        while (i < file.Length)
        {
            line = file[i];
            var deserialized = JsonConvert.DeserializeObject<KeyValuePair<string, MarkData[]>>(line);
            temp.Add(deserialized.Key, deserialized.Value);
            i++;
        }
        return temp;
    }

    public MarkData[] SumMarkDatas(MarkData[] a, MarkData[] b)
    {
        MarkData[] temp = new MarkData[(a.Length >= b.Length ? a.Length : b.Length)];
        
        for (int i = 0; i < a.Length; i++)
        {
            temp[i] = a[i];
        }

        for (int i = 0; i < b.Length; i++)
        {
            for (int j = 0; j < temp.Length; j++)
            {
                if(b[i].pos == temp[j].pos)
                {
                    temp[j].val += b[i].val;
                }
            }
        }
        return temp;
    }

}
