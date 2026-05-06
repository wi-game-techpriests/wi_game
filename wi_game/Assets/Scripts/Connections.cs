using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
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
}

public class Connections : MonoBehaviour
{
    public Image[] tileImages;
    public Sprite unselectedSprite;
    public Sprite selectedSprite;
    public TextMeshProUGUI[] words;

    private string url = "http://localhost:8080/test/connections";
    private int selectedCount = 0;

    void Start()
    {
        // StartCoroutine(LoadFromServer());
        // return;
        TextAsset jsonFile = Resources.Load<TextAsset>("connections_data");
        CategoryWrapper data = JsonUtility.FromJson<CategoryWrapper>(jsonFile.text);
        
        List<string> allWords = new List<string>();
        foreach (var category in data.ToList())
        {
            allWords.AddRange(category.categoryWords);
        }

        allWords = allWords.OrderBy(x => UnityEngine.Random.value).ToList();
        for (int i = 0; i < words.Length; i++)
        {
            words[i].text = allWords[i];
        }

        Debug.Log("Connections game started.");
    }

    IEnumerator<UnityWebRequestAsyncOperation> LoadFromServer()
    {
        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Błąd pobierania: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;

        // Parsowanie
        CategoryWrapper data = JsonUtility.FromJson<CategoryWrapper>(json);

        // Zbieranie słów
        List<string> allWords = new List<string>();

        foreach (var category in data.ToList())
        {
            allWords.AddRange(category.categoryWords);
        }

        // Mieszanie
        for (int i = 0; i < allWords.Count; i++)
        {
            int rnd = UnityEngine.Random.Range(i, allWords.Count);
            (allWords[i], allWords[rnd]) = (allWords[rnd], allWords[i]);
        }

        // Przypisanie do UI
        for (int i = 0; i < words.Length; i++)
        {
            words[i].text = allWords[i];
        }
    }

    public void Click(int id)
    {
        if (tileImages[id].sprite == unselectedSprite)
        {
            if (selectedCount >= 4)
            {
                Debug.Log("Cannot select more than 4 connections.");
                return;
            }
            selectedCount++;
            tileImages[id].sprite = selectedSprite;
        }
        else
        {
            selectedCount--;
            tileImages[id].sprite = unselectedSprite;
        }
    }
}
