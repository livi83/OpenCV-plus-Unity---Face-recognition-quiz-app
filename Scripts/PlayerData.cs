using OpenCvSharp.Demo;

[System.Serializable]
public class PlayerData
{

    public int score;

    public PlayerData(QuizManager QuizManager)
    {
        score = QuizManager.score;
    }
}
