using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataManager : MonoBehaviour
{
    string path = "Assets/PlayerVerticesData";
    public void SaveData(PersonVerticesData vData)
    {
        string jsonString = JsonUtility.ToJson(vData);
        
        System.IO.File.WriteAllText(path + "/" + vData.name + "VerticesData.json", jsonString);
    }

    public PersonVerticesData LoadData(string name)
    {
        if (!System.IO.File.Exists(path + "/" + name + "VerticesData.json"))
        {
            return null;
        }

        string jsonString = System.IO.File.ReadAllText(path + "/" + name + "VerticesData.json");
        PersonVerticesData myData = JsonUtility.FromJson<PersonVerticesData>(jsonString);
        return myData;
    }

}

[System.Serializable]
public class PersonVerticesData
{
    public List<int> selectedIndices;

    public string name;

    public PersonVerticesData(List<int> newList, string newName)
    {
        selectedIndices = newList;
        name = newName;
    }
}
