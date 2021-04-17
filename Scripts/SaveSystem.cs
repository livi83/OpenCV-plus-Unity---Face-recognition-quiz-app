using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using OpenCvSharp.Demo;

public class SaveSystem 
{

    public static void SaveScore(QuizManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //vytvori path nezavisle od OS
        string path = Application.persistentDataPath +"/player.quiz";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.quiz";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("File not found in:" + path);
            return null;
        }

    }
}
