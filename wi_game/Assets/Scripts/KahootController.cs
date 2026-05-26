using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;



public class KahootController : MonoBehaviour
{
    
    public TMP_Text questionNumberText; 
    public TMP_Text questionText;
    public Button[] answerButtons;
    public MainGameController mainGameController;

    private int currentQuestionIndex = 0;
    private QuestionList questionList;

    private int score = 0;
    private float questionStartTime;


    public void A()
    {
        Question currentQuestion = questionList.questions[currentQuestionIndex];
        int responseTime = 10 - (int)(Time.time - questionStartTime);
        //responseTimes.Add(responseTime);
        Debug.Log("Czas odpowiedzi: " + responseTime.ToString() + "s");
        
        if (currentQuestion.correctAnswer == "A")
        {
            score += (int)(Mathf.Max(responseTime, 0) / 10.0f * 250);
        }
        currentQuestionIndex++;
        NextQuestion();
    }

    public void B()
    {
        Question currentQuestion = questionList.questions[currentQuestionIndex];
        int responseTime = 10 - (int)(Time.time - questionStartTime);
        //responseTimes.Add(responseTime);
        Debug.Log("Czas odpowiedzi: " + responseTime.ToString() + "s");
        
        if (currentQuestion.correctAnswer == "B")
        {
            score += (int)(Mathf.Max(responseTime, 0) / 10.0f * 250);
        }
        currentQuestionIndex++;
        NextQuestion();
    }

    public void C()
    {
        Question currentQuestion = questionList.questions[currentQuestionIndex];
        int responseTime = 10 - (int)(Time.time - questionStartTime);
        //responseTimes.Add(responseTime);
        Debug.Log("Czas odpowiedzi: " + responseTime.ToString() + "s");
        
        if (currentQuestion.correctAnswer == "C")
        {
            score += (int)(Mathf.Max(responseTime, 0) / 10.0f * 250);
        }
        currentQuestionIndex++;
        NextQuestion();

    }

    public void D()
    {
        Question currentQuestion = questionList.questions[currentQuestionIndex];
        int responseTime = 10 - (int)(Time.time - questionStartTime);
        //responseTimes.Add(responseTime);
        Debug.Log("Czas odpowiedzi: " + responseTime.ToString() + "s");
        
        if (currentQuestion.correctAnswer == "D")
        {
            score += (int)(Mathf.Max(responseTime, 0) / 10.0f * 250);
        }
        currentQuestionIndex++;
        NextQuestion();
    }

    public void NextQuestion()
    {
        if (currentQuestionIndex < questionList.questions.Length)
        {
            questionStartTime = Time.time;
            Question currentQuestion = questionList.questions[currentQuestionIndex];
            Debug.Log(currentQuestion.correctAnswer);
            questionNumberText.text = "Pytanie " + (currentQuestionIndex + 1) + "/" + questionList.questions.Length;
            questionText.text = currentQuestion.question;

            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (i < currentQuestion.answers.Length)
                {
                    answerButtons[i].GetComponentInChildren<TMP_Text>().text = currentQuestion.answers[i].answer;
                }
            }
        }
        else
        {
            GameManager.Instance.SetCurrentScore(score);
            GameManager.Instance.SetSceneResult(score);
            mainGameController.EndGame();
        }
    }

    public void StartGame()
    {
        FetchData();
        score = 0;
        currentQuestionIndex = 0;
    }

    public void FetchData()
    {
        GameManager.Instance.GetGameData("kahoot", 
            (jsonData) => {
                try
                {
                    string wrappedJson = "{\"questions\":" + jsonData + "}";
                    questionList = JsonUtility.FromJson<QuestionList>(wrappedJson);
                    Debug.Log("Pytania załadowane: " + questionList.questions.Length);
                    questionStartTime = Time.time;
                    mainGameController.StartGame();
                    NextQuestion();
                    
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Błąd parsowania JSON: " + ex.Message);
                }
            },
            (error) => {
                Debug.LogError("Błąd: " + error);
            }
        );
    }

}
[System.Serializable]
public class Answer
{
    public string key;
    public string answer;
}

[System.Serializable]
public class Question
{
    public string question;
    public string correctAnswer;
    public Answer[] answers;
}

[System.Serializable]
public class QuestionList
{
    public Question[] questions;
}

