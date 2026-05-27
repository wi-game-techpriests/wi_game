using System.Linq;
using UnityEngine;
using System.Diagnostics;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class FillInPuzzle : MonoBehaviour
{
    public int textHeight;
    public int slotWidth;
    public int padding;
    public int scoreForCorrect;
    public int scoreForTime;
    public float timeScaling;

    [TextArea(15,20)]
    public string backupText;

    public string backupTitle;
    public string[] backupCorrect;

    public List<string> backupIncorrect;

    public FillInText textArea;
    public FillInOptions optionArea;
    public MainGameController mainGameController;
    public TextMeshProUGUI titleTexObject;

    private List<Slot> slots;

    private Stopwatch stopwatch;

    [System.Serializable]
    public class FillInAnswer
    {
        public int answerNumber;
        public string answer;
        public List<string> otherChoices;
    }

    [System.Serializable]
    public class FillInData
    {
        public List<string> fragments;
        public List<FillInAnswer> entries;
    }

    public void StartGame()
    {
        Clear();

        titleTexObject.text = "Ładowanie...";

        GameManager.Instance.GetGameData(
            "fill_in",
            (jsonData) =>
            {
                UnityEngine.Debug.Log(jsonData);

                FillInData data = JsonUtility.FromJson<FillInData>(jsonData);

                List<string> incorrectOptions = new();
                string[] correctOptions = new string[data.entries.Count];

                int i = 0; //
                foreach (var answer in data.entries)
                {
                    //correctOptions[answer.answerNumber] = answer.answer;
                    correctOptions[i] = answer.answer; //
                    i++; //

                    incorrectOptions.AddRange(answer.otherChoices);
                }

                Setup(data.fragments[0],data.fragments[1],correctOptions,incorrectOptions);
            },
            (error) =>
            {
                UnityEngine.Debug.LogError(error);
                Setup(backupTitle,backupText,backupCorrect,backupIncorrect);
            }
        );
    }



    static T[] ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        return array.OrderBy(x => random.Next()).ToArray();
    }

    public void Clear()
    {
        textArea.ClearText();
        optionArea.ClearOptions();
    }

    public void Setup(string titleText, string text, string[] correctOptions, List<string> incorrectOptions)
    {
        //Create UI
        titleTexObject.text = titleText;

        incorrectOptions.AddRange(correctOptions);
        string[] options = incorrectOptions.ToArray();

        slots = textArea.CreateText(text,textHeight,slotWidth,padding);
        optionArea.CreateOptions(ShuffleArray(options),4,textHeight,slotWidth);

        if (slots.Count > options.Length)
        {
            throw new IndexOutOfRangeException();
        }

        //Assign accepted values to slots
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].acceptedOption = correctOptions[i];
        }

        stopwatch = new();
        stopwatch.Start();
    }

    private float TimeFormula(float time)
    {
        return timeScaling / (Math.Max(time-30,0) + timeScaling);
    }


    IEnumerator CheckScoreInternal()
    {
        int correct = 0;
        foreach (Slot slot in slots)
        {
            if (slot.IsCorrect()) correct += 1;
        }
        stopwatch.Stop();
        TimeSpan time = stopwatch.Elapsed;
        int score = (int)(correct/(float)slots.Count * scoreForCorrect);
        score += (int)(TimeFormula(Math.Max(0,time.Seconds)) * scoreForTime);
        yield return new WaitForSeconds(1);
        GameManager.Instance.SetCurrentScore(score);
        mainGameController.EndGame();
    }

    public void CheckScore()
    {
        StartCoroutine(CheckScoreInternal());
    }

}
