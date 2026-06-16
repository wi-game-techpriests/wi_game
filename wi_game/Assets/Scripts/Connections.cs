using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Category
{
    public string categoryName;
    public string[] categoryWords;
}

[Serializable]
public class CategoryWrapper
{
    public Category categoryA;
    public Category categoryB;
    public Category categoryC;
    public Category categoryD;

    public List<Category> ToList()
    {
        return new List<Category> { categoryA, categoryB, categoryC, categoryD };
    }

    public string CheckCategory(List<string> selectedWords)
    {
        var categories = ToList();
        foreach (var category in categories)
        {
            if (selectedWords.All(word => category.categoryWords.Contains(word)))
            {
                return category.categoryName;
            }
        }
        return null;
    }
}

public class Connections : MonoBehaviour
{
    public Image[] tileImages;
    public Sprite unselectedSprite;
    public Sprite selectedSprite;
    public Sprite doneSprite;
    public TextMeshProUGUI[] words;
    public MainGameController mainGameController;
    public Image[] lifeImages;
    public Sprite liveSprite;
    public Sprite deadSprite;

    private CategoryWrapper categoryData;
    private int currentTry = 3;
    private List<string> selectedWords = new List<string>(4);
    private int score = 0;
    private float gameStartTime;
    private int categoriesSolved = 0;

    public void StartGame()
    {
        ClearGame();
        FetchData();
    }

    private void ClearGame()
    {
        selectedWords.Clear();
        for (int i = 0; i < tileImages.Length; i++)
        {
            tileImages[i].sprite = unselectedSprite;
        }
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = liveSprite;
        }
        currentTry = 3;
        score = 0;
        gameStartTime = Time.time;
        categoriesSolved = 0;
    }

    private void FetchData()
    {
        GameManager.Instance.GetGameData("connections", 
            (jsonData) => {
                try
                {
                    Debug.Log("Fetched data: " + jsonData);
                    categoryData = JsonUtility.FromJson<CategoryWrapper>(jsonData);
                    // TextAsset jsonFile = Resources.Load<TextAsset>("connections_test");
                    // categoryData = JsonUtility.FromJson<CategoryWrapper>(jsonFile.text);
                    List<string> allWords = new List<string>();
                    foreach (var category in categoryData.ToList())
                    {
                        allWords.AddRange(category.categoryWords);
                    }
                    allWords = allWords.OrderBy(x => UnityEngine.Random.value).ToList();
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i].text = allWords[i].Replace("\\n", "\n"); // for text wrapping
                    }
                    mainGameController.StartGame();
                }
                catch (Exception ex)
                {
                    Debug.LogError("Błąd parsowania JSON: " + ex.Message);
                }
            },
            (error) => {
                Debug.LogError("Błąd: " + error);
            }
        );
    }

    public void Click(int id)
    {
        if (tileImages[id].sprite == doneSprite)
        {
            return; // Ignore clicks on already completed tiles
        }
        if (tileImages[id].sprite == unselectedSprite)
        {
            if (selectedWords.Count >= 4)
            {
                Debug.Log("Cannot select more than 4 connections.");
                return;
            }
            tileImages[id].sprite = selectedSprite;
            selectedWords.Add(words[id].text);
        }
        else
        {
            tileImages[id].sprite = unselectedSprite;
            selectedWords.Remove(words[id].text);
        }
    }

    public void Check()
    {
        if (selectedWords.Count != 4)
        {
            Debug.Log("Please select exactly 4 connections.");
            return;
        }

        if (currentTry > 0)
        {
            string correctCategory = categoryData.CheckCategory(selectedWords);
            if (correctCategory != null)
            {
                int elapsedTime = (int)(Time.time - gameStartTime);
                int pointsEarned = Mathf.Max(10, 260 - 5 * elapsedTime);
                score += pointsEarned;
                categoriesSolved++;
                Debug.Log($"Kategoria {correctCategory} znaleziona! Czas: {elapsedTime:F1}s, Punkty: {pointsEarned}, Razem: {score}");
                
                if (categoriesSolved > 3)
                {
                    score = Mathf.Min(score, 1000);
                    GameManager.Instance.SetCurrentScore(score);
                    GameManager.Instance.SetSceneResult(score);
                    mainGameController.EndGame();
                }
                else
                {
                    for (int i = 0; i < tileImages.Length; i++)
                    {
                        if (tileImages[i].sprite == selectedSprite)
                        {
                            tileImages[i].sprite = doneSprite;
                        }
                    }
                    selectedWords.Clear();
                }
            } else {
                StartCoroutine(FlashWrongAnswerAndLoseLife());
            }
        }
        else
        {
            GameManager.Instance.SetCurrentScore(score);
            GameManager.Instance.SetSceneResult(score);
            mainGameController.EndGame();
        }
    }

    private IEnumerator FlashWrongAnswerAndLoseLife()
    {
        // Collect selected tiles
        List<int> selectedIndices = new List<int>();
        for (int i = 0; i < tileImages.Length; i++)
        {
            if (tileImages[i].sprite == selectedSprite)
                selectedIndices.Add(i);
        }

        // Store originals
        var origTileColors = new List<Color>();
        var origTileScales = new List<Vector3>();
        foreach (var idx in selectedIndices)
        {
            origTileColors.Add(tileImages[idx].color);
            origTileScales.Add(tileImages[idx].rectTransform.localScale);
        }
        Color origLifeColor = lifeImages[currentTry].color;

        float duration = 0.6f;
        float elapsed = 0f;

        // Animate pulse + red flash
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = Mathf.Sin(t * Mathf.PI); // ease in/out

            // Tiles: color and scale
            for (int i = 0; i < selectedIndices.Count; i++)
            {
                int idx = selectedIndices[i];
                tileImages[idx].color = Color.Lerp(origTileColors[i], Color.red, pulse);
                tileImages[idx].rectTransform.localScale = Vector3.Lerp(origTileScales[i], origTileScales[i] * 1.15f, pulse);
                tileImages[idx].rectTransform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, -8f, pulse));
            }

            // Life image: flash red
            lifeImages[currentTry].color = Color.Lerp(origLifeColor, Color.red, pulse);

            yield return null;
        }

        // Small hold on red
        yield return new WaitForSeconds(0.12f);

        // Revert tiles and mark as unselected
        for (int i = 0; i < selectedIndices.Count; i++)
        {
            int idx = selectedIndices[i];
            tileImages[idx].color = origTileColors[i];
            tileImages[idx].rectTransform.localScale = origTileScales[i];
            tileImages[idx].rectTransform.localRotation = Quaternion.identity;
        }

        // Mark life as dead and reset color
        lifeImages[currentTry].sprite = deadSprite;
        lifeImages[currentTry].color = Color.white;

        // decrement tries
        currentTry--;
    }
}
