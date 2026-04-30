using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement; 

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction touchPositionAction;
    private InputAction touchPressAction;

    public List<LetterTile> selectedTiles = new List<LetterTile>();
    public GridManager gridManager;
    public GameObject linePrefab;
    public GameObject winPanel;
    public TextMeshProUGUI timerText;

    private Vector2Int? lastGridPos = null;
    private Vector2Int? direction = null;
    private float elapsedTime = 0f;
    private bool isGameOver = false;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPositionAction = playerInput.actions["Touch"]; 
        touchPressAction = playerInput.actions["Press"];
    }

    void Update()
    {
        if (isGameOver) return;
        elapsedTime += Time.deltaTime;
        if (touchPressAction.IsPressed()) DetectTile();
        else if (touchPressAction.WasReleasedThisFrame())
        {
            CheckWord();
            ClearSelection();
        }
    }

    private void DetectTile()
{
    Vector2 pos = touchPositionAction.ReadValue<Vector2>();
    PointerEventData eventData = new PointerEventData(EventSystem.current) { position = pos };
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, results);

    foreach (var result in results)
    {
        LetterTile tile = result.gameObject.GetComponent<LetterTile>();
        if (tile != null && !selectedTiles.Contains(tile))
        {
            int index = tile.transform.GetSiblingIndex();
            
            int r = index / gridManager.columns;
            int c = index % gridManager.columns;
            Vector2Int currentGridPos = new Vector2Int(r, c);

            if (selectedTiles.Count == 0) 
            {
                AddTile(tile, currentGridPos);
            }
            else 
            {
                Vector2Int diff = currentGridPos - lastGridPos.Value;
                
                if (selectedTiles.Count == 1)
                {
                    if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1 && diff != Vector2Int.zero)
                    {
                        direction = diff;
                        AddTile(tile, currentGridPos);
                    }
                }
                else if (diff == direction.Value)
                {
                    AddTile(tile, currentGridPos);
                }
            }
        }
    }
}

    private void AddTile(LetterTile tile, Vector2Int gridPos)
    {
        tile.SetHighlight(true);
        selectedTiles.Add(tile);
        lastGridPos = gridPos;
    }

    private void CheckWord()
    {
        string word = "";
        foreach (var tile in selectedTiles) word += tile.GetLetter();

        if (gridManager.wordsToFind.Contains(word))
        {
            DrawStrikeLine();
            
            gridManager.wordsToFind.Remove(word);
            Debug.Log("Znaleziono: " + word + ". Pozostało: " + gridManager.wordsToFind.Count);

            if (gridManager.wordsToFind.Count == 0)
            {
                ShowWinScreen();
            }
        }
    }

    private void ShowWinScreen()
    {
        isGameOver = true;
        if (winPanel != null)
        {
            winPanel.SetActive(true);
            winPanel.transform.SetAsLastSibling(); 
            
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            timerText.text = string.Format("TWÓJ CZAS: {0:00}:{1:00}", minutes, seconds);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void DrawStrikeLine()
    {
        if (selectedTiles.Count < 2) return;

        GameObject line = Instantiate(linePrefab, gridManager.transform.parent);
        line.transform.SetAsLastSibling(); 

        RectTransform rt = line.GetComponent<RectTransform>();
        Vector3 startWorld = selectedTiles[0].GetComponent<RectTransform>().position;
        Vector3 endWorld = selectedTiles[selectedTiles.Count - 1].GetComponent<RectTransform>().position;

        rt.position = (startWorld + endWorld) / 2f;
        float dist = Vector3.Distance(startWorld, endWorld);
        float canvasScale = rt.lossyScale.x;
        rt.sizeDelta = new Vector2((dist / canvasScale) + 60f, 40f);

        Vector3 dir = endWorld - startWorld;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rt.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ClearSelection()
    {
        foreach (var tile in selectedTiles) tile.SetHighlight(false);
        selectedTiles.Clear();
        lastGridPos = null;
        direction = null;
    }
}