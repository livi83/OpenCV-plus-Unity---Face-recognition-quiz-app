
using UnityEngine;
using UnityEngine.UI;
public class History : MonoBehaviour
{
    public int score = 0;
    Text txt;
    public void Start()
    {
        LoadPlayer();
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        score = data.score;
        txt = gameObject.GetComponent<Text>();
        txt.text = "HIGHEST SCORE: "+score.ToString();
        Debug.Log(score);
    }
    public int GetSavedScore()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        score = data.score;
        return score;
    }
}
