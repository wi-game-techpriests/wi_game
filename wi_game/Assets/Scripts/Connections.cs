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
    [Header("Tiles")]
    public Image[] tileImages;
    public Sprite unselectedSprite;
    public Sprite selectedSprite;
    public Sprite doneSprite;
    public TextMeshProUGUI[] words;

    [Header("Game")]
    public MainGameController mainGameController;

    [Header("Lives")]
    public Image[] lifeImages;
    public Sprite liveSprite;
    public Sprite deadSprite;

    [Header("Dynamic board layout")]
    [SerializeField] private GridLayoutGroup activeTilesGrid;
    [SerializeField] private LayoutElement activeTilesGridLayout;
    [SerializeField] private GameObject[] solvedCategoryColumns;
    [SerializeField] private TextMeshProUGUI[] solvedCategoryTexts;
    [SerializeField] private CanvasGroup[] solvedCategoryCanvasGroups;

    private CategoryWrapper categoryData;

    private int currentTry = 3;
    private int score = 0;
    private float gameStartTime;
    private int categoriesSolved = 0;

    private string[] tileWordValues;

    private readonly List<int> selectedIndices = new List<int>(4);
    private readonly HashSet<int> solvedIndices = new HashSet<int>();

    private bool isAnimatingWrongAnswer = false;

    public void StartGame()
    {
        ClearGame();
        FetchData();
    }

    private void ClearGame()
    {
        selectedIndices.Clear();
        solvedIndices.Clear();

        for (int i = 0; i < tileImages.Length; i++)
        {
            tileImages[i].gameObject.SetActive(true);
            tileImages[i].sprite = unselectedSprite;
            tileImages[i].color = Color.white;
            tileImages[i].rectTransform.localScale = Vector3.one;
            tileImages[i].rectTransform.localRotation = Quaternion.identity;

            if (i < words.Length && words[i] != null)
            {
                words[i].gameObject.SetActive(true);
                words[i].text = "";
            }
        }

        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].sprite = liveSprite;
            lifeImages[i].color = Color.white;
        }

        if (solvedCategoryColumns != null)
        {
            for (int i = 0; i < solvedCategoryColumns.Length; i++)
            {
                if (solvedCategoryColumns[i] != null)
                {
                    RectTransform rectTransform = solvedCategoryColumns[i].GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.localScale = Vector3.one;
                    }

                    solvedCategoryColumns[i].SetActive(false);
                }
            }
        }

        if (solvedCategoryCanvasGroups != null)
        {
            for (int i = 0; i < solvedCategoryCanvasGroups.Length; i++)
            {
                if (solvedCategoryCanvasGroups[i] != null)
                {
                    solvedCategoryCanvasGroups[i].alpha = 1f;
                }
            }
        }

        if (solvedCategoryTexts != null)
        {
            for (int i = 0; i < solvedCategoryTexts.Length; i++)
            {
                if (solvedCategoryTexts[i] != null)
                {
                    solvedCategoryTexts[i].text = "";
                }
            }
        }

        currentTry = 3;
        score = 0;
        gameStartTime = Time.time;
        categoriesSolved = 0;
        isAnimatingWrongAnswer = false;

        RefreshBoardLayout();
    }

    private void FetchData()
    {
        GameManager.Instance.GetGameData("connections",
            (jsonData) =>
            {
                try
                {
                    Debug.Log("Fetched data: " + jsonData);

                    categoryData = JsonUtility.FromJson<CategoryWrapper>(jsonData);

                    List<string> allWords = new List<string>();

                    foreach (var category in categoryData.ToList())
                    {
                        allWords.AddRange(category.categoryWords);
                    }

                    allWords = allWords.OrderBy(x => UnityEngine.Random.value).ToList();

                    tileWordValues = new string[words.Length];

                    for (int i = 0; i < words.Length; i++)
                    {
                        tileWordValues[i] = allWords[i];

                        // Tylko tekst wyświetlany dostaje zamianę \n.
                        // Do logiki gry używamy tileWordValues[i], bez tej zamiany.
                        words[i].text = allWords[i].Replace("\\n", "\n");
                    }

                    RefreshBoardLayout();
                    mainGameController.StartGame();
                }
                catch (Exception ex)
                {
                    Debug.LogError("Błąd parsowania JSON: " + ex.Message);
                }
            },
            (error) =>
            {
                Debug.LogError("Błąd: " + error);
            }
        );
    }

    public void Click(int id)
    {
        if (isAnimatingWrongAnswer)
        {
            return;
        }

        if (id < 0 || id >= tileImages.Length)
        {
            Debug.LogWarning("Invalid tile id: " + id);
            return;
        }

        if (tileWordValues == null || id >= tileWordValues.Length)
        {
            Debug.LogWarning("Tile word values are not initialized yet.");
            return;
        }

        if (solvedIndices.Contains(id))
        {
            return;
        }

        if (selectedIndices.Contains(id))
        {
            selectedIndices.Remove(id);
            tileImages[id].sprite = unselectedSprite;
            return;
        }

        if (selectedIndices.Count >= 4)
        {
            Debug.Log("Cannot select more than 4 connections.");
            return;
        }

        selectedIndices.Add(id);
        tileImages[id].sprite = selectedSprite;
    }

    public void Check()
    {
        if (isAnimatingWrongAnswer)
        {
            return;
        }

        if (selectedIndices.Count != 4)
        {
            Debug.Log("Please select exactly 4 connections.");
            return;
        }

        if (currentTry > 0)
        {
            List<string> selectedWords = selectedIndices
                .Select(index => tileWordValues[index])
                .ToList();

            string correctCategory = categoryData.CheckCategory(selectedWords);

            if (correctCategory != null)
            {
                int elapsedTime = (int)(Time.time - gameStartTime);
                int pointsEarned = Mathf.Max(10, 260 - 5 * elapsedTime);

                score += pointsEarned;
                categoriesSolved++;

                Debug.Log($"Kategoria {correctCategory} znaleziona! Czas: {elapsedTime:F1}s, Punkty: {pointsEarned}, Razem: {score}");

                SolveSelectedCategory(correctCategory);

                if (categoriesSolved > 3)
                {
                    score = Mathf.Min(score, 1000);
                    GameManager.Instance.SetCurrentScore(score);
                    GameManager.Instance.SetSceneResult(score);
                    mainGameController.EndGame();
                }
            }
            else
            {
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

    private void SolveSelectedCategory(string categoryName)
    {
        int solvedSlot = categoriesSolved - 1;

        ShowSolvedCategory(solvedSlot, categoryName);

        foreach (int idx in selectedIndices)
        {
            solvedIndices.Add(idx);

            tileImages[idx].sprite = unselectedSprite;
            tileImages[idx].color = Color.white;
            tileImages[idx].rectTransform.localScale = Vector3.one;
            tileImages[idx].rectTransform.localRotation = Quaternion.identity;

            tileImages[idx].gameObject.SetActive(false);

            if (idx < words.Length && words[idx] != null)
            {
                words[idx].gameObject.SetActive(false);
            }
        }

        selectedIndices.Clear();

        RefreshBoardLayout();
    }

    private void ShowSolvedCategory(int solvedSlot, string categoryName)
    {
        if (
            solvedCategoryColumns == null ||
            solvedSlot < 0 ||
            solvedSlot >= solvedCategoryColumns.Length ||
            solvedCategoryColumns[solvedSlot] == null
        )
        {
            return;
        }

        if (
            solvedCategoryTexts != null &&
            solvedSlot < solvedCategoryTexts.Length &&
            solvedCategoryTexts[solvedSlot] != null
        )
        {
            solvedCategoryTexts[solvedSlot].text = categoryName;
        }

        GameObject column = solvedCategoryColumns[solvedSlot];

        column.SetActive(true);

        CanvasGroup canvasGroup = null;

        if (
            solvedCategoryCanvasGroups != null &&
            solvedSlot < solvedCategoryCanvasGroups.Length
        )
        {
            canvasGroup = solvedCategoryCanvasGroups[solvedSlot];
        }

        StartCoroutine(AnimateSolvedCategoryAppear(column, canvasGroup));
    }

    private IEnumerator AnimateSolvedCategoryAppear(GameObject column, CanvasGroup canvasGroup)
    {
        RectTransform rectTransform = column.GetComponent<RectTransform>();

        Vector3 targetScale = rectTransform != null
            ? rectTransform.localScale
            : column.transform.localScale;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        if (rectTransform != null)
        {
            rectTransform.localScale = targetScale * 0.85f;
        }
        else
        {
            column.transform.localScale = targetScale * 0.85f;
        }

        float duration = 0.35f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float ease = 1f - Mathf.Pow(1f - t, 3f);

            float bounce = Mathf.Sin(t * Mathf.PI) * 0.08f;
            float scaleMultiplier = Mathf.Lerp(0.85f, 1f, ease) + bounce;

            if (canvasGroup != null)
            {
                canvasGroup.alpha = ease;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = targetScale * scaleMultiplier;
            }
            else
            {
                column.transform.localScale = targetScale * scaleMultiplier;
            }

            yield return null;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
        }

        if (rectTransform != null)
        {
            rectTransform.localScale = targetScale;
        }
        else
        {
            column.transform.localScale = targetScale;
        }
    }

    private void RefreshBoardLayout()
    {
        if (activeTilesGrid == null)
        {
            return;
        }

        int remainingRows = Mathf.Max(1, 4 - categoriesSolved);

        activeTilesGrid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        activeTilesGrid.constraintCount = remainingRows;

        if (activeTilesGridLayout != null)
        {
            Vector2 cellSize = activeTilesGrid.cellSize;
            Vector2 spacing = activeTilesGrid.spacing;

            float height =
                remainingRows * cellSize.y +
                Mathf.Max(0, remainingRows - 1) * spacing.y;

            activeTilesGridLayout.minHeight = height;
            activeTilesGridLayout.preferredHeight = height;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            activeTilesGrid.GetComponent<RectTransform>()
        );
    }

    private IEnumerator FlashWrongAnswerAndLoseLife()
    {
        isAnimatingWrongAnswer = true;

        List<int> flashingIndices = new List<int>(selectedIndices);

        var origTileColors = new List<Color>();
        var origTileScales = new List<Vector3>();
        var origTileRotations = new List<Quaternion>();

        foreach (var idx in flashingIndices)
        {
            origTileColors.Add(tileImages[idx].color);
            origTileScales.Add(tileImages[idx].rectTransform.localScale);
            origTileRotations.Add(tileImages[idx].rectTransform.localRotation);
        }

        Color origLifeColor = lifeImages[currentTry].color;

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float pulse = Mathf.Sin(t * Mathf.PI);

            for (int i = 0; i < flashingIndices.Count; i++)
            {
                int idx = flashingIndices[i];

                tileImages[idx].color = Color.Lerp(origTileColors[i], Color.red, pulse);

                tileImages[idx].rectTransform.localScale = Vector3.Lerp(
                    origTileScales[i],
                    origTileScales[i] * 1.15f,
                    pulse
                );

                tileImages[idx].rectTransform.localRotation = Quaternion.Euler(
                    0f,
                    0f,
                    Mathf.Lerp(0f, -8f, pulse)
                );
            }

            lifeImages[currentTry].color = Color.Lerp(origLifeColor, Color.red, pulse);

            yield return null;
        }

        yield return new WaitForSeconds(0.12f);

        for (int i = 0; i < flashingIndices.Count; i++)
        {
            int idx = flashingIndices[i];

            tileImages[idx].color = origTileColors[i];
            tileImages[idx].rectTransform.localScale = origTileScales[i];
            tileImages[idx].rectTransform.localRotation = origTileRotations[i];
        }

        lifeImages[currentTry].sprite = deadSprite;
        lifeImages[currentTry].color = Color.white;

        currentTry--;

        isAnimatingWrongAnswer = false;
    }
}