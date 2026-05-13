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
    public string text;

    public string[] options;

    public FillInText textArea;
    public FillInOptions optionArea;
    public GameObject endGame;
    public TextMeshProUGUI scoreText;

    private List<Slot> slots;

    private Stopwatch stopwatch;


    void Start()
    {
        Setup();

        stopwatch = new();
        stopwatch.Start();
    }

    static T[] ShuffleArray<T>(T[] array)
    {
        System.Random random = new System.Random();
        return array.OrderBy(x => random.Next()).ToArray();
    }

    public void Setup()
    {
        //Create UI 
        slots = textArea.CreateText(text,textHeight,slotWidth,padding);
        optionArea.CreateOptions(ShuffleArray(options),4,textHeight,slotWidth);

        if (slots.Count > options.Length)
        {
            throw new IndexOutOfRangeException();
        }

        //Assign accepted values to slots
        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].acceptedOption = options[i];
        }
    }

    private float TimeFormula(float time)
    {
        return timeScaling / (time + timeScaling);
    }


    IEnumerator CheckScoreInternal()
    {
        int score = 0;
        foreach (Slot slot in slots)
        {
            if (slot.IsCorrect()) score += scoreForCorrect;
        }
        stopwatch.Stop();
        TimeSpan time = stopwatch.Elapsed;
        score += (int)(TimeFormula(time.Seconds) * scoreForTime);
        yield return new WaitForSeconds(3);
        scoreText.text = score.ToString();
        endGame.SetActive(true);
    }

    public void CheckScore()
    {
        StartCoroutine(CheckScoreInternal());
    }
}
