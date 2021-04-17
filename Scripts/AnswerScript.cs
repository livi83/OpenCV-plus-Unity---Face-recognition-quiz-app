using OpenCvSharp.Demo;
using UnityEngine;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;

    public QuizManager QuizManager;
    
    /*
     * SK: Metóda volá Correct() alebo Wrong()
     metódu v závislosti na bool isCorrect
     * 
     ENG: Method which is calling Correct() or Wrong() 
     method from QuizManager depending on bool value of isCorrect.
     */
    public void Answer()
    {
        
        if (isCorrect)
        {
            QuizManager.Correct();
        }
        else
        {
            QuizManager.Wrong();
        }
    }
}
