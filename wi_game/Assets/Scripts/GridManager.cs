using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject letterTilePrefab;
    public int rows = 8;
    public int columns = 8;
    public List<string> wordsToFind;
    private string polishChars = "AĄBCĆDEĘFGHIJKLŁMNŃOQÓPRSŚTUVWYXZŹŻ";
    [SerializeField] private TextMeshProUGUI Remaining; // Referencja do UI
    private int cnt;


    [System.Serializable]
    public class WordData
    {
        public List<string> words;
    }
    void Awake() 
    {
        // wordsToFind = new List<string> { "JAVA", "HASKELL", "ERLANG", "GEOMY" };
        ActualizeCnt();
    }


    void Start() 
    { 
        FetchWordSearchData();
    }
    public void StartAgain()
    {
        FetchWordSearchData();
    }

    private void FetchWordSearchData()
    {
        GameManager.Instance.GetGameData("wordsearch", 
            (jsonData) => {
                Debug.Log("Dane wordsearch: " + jsonData);
                
                WordData data = JsonUtility.FromJson<WordData>(jsonData);
                wordsToFind = data.words.Select(w => w.ToUpper()).ToList();
                cnt = wordsToFind.Count;
                ActualizeCnt();
                GenerateBoard();
            },
            (error) => {
                Debug.LogError("Błąd: " + error);
                
                List<string> backupWords = new List<string> { "HASKELL", "RUBY", "ICON", "FORTRAN", "JAVA", "SCALA" };
                wordsToFind = backupWords.Select(w => w.ToUpper()).ToList();
                
                cnt = wordsToFind.Count;
                ActualizeCnt();
                GenerateBoard();
            }
        );
    }
    public void GenerateBoard()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        char[,] grid = new char[rows, columns];

        foreach (string word in wordsToFind)
        {
            PlaceWord(word.ToUpper(), grid);
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                char finalChar = grid[r, c];
                if (finalChar == '\0') 
                    finalChar = polishChars[Random.Range(0, polishChars.Length)];

                GameObject tile = Instantiate(letterTilePrefab, transform);
                tile.name = $"R{r}_C{c}";
                tile.GetComponent<LetterTile>().SetLetter(finalChar.ToString());
            }
        }
    }

    private void PlaceWord(string word, char[,] grid)
    {
        Vector2Int[] dirs = { 
            new Vector2Int(0, 1),  
            new Vector2Int(1, 0),  
            new Vector2Int(-1, 0), 
            new Vector2Int(1, 1),  
            new Vector2Int(-1, 1)
        };

        bool placed = false;
        int safety = 0;

        while (!placed && safety < 200)
        {
            safety++;
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];
            int startRow = Random.Range(0, rows);
            int startCol = Random.Range(0, columns);

            if (CheckFit(word, startRow, startCol, dir, grid))
            {
                for (int i = 0; i < word.Length; i++)
                {
                    grid[startRow + i * dir.x, startCol + i * dir.y] = word[i];
                }
                placed = true;
            }
        }
    }

    private bool CheckFit(string word, int r, int c, Vector2Int dir, char[,] grid)
    {
        for (int i = 0; i < word.Length; i++)
        {
            int nr = r + i * dir.x;
            int nc = c + i * dir.y;

            if (nr < 0 || nr >= rows || nc < 0 || nc >= columns) return false;

            if (grid[nr, nc] != '\0' && grid[nr, nc] != word[i]) return false;
        }
        return true;
    }

    public void WordFound()
    {
        cnt--;
        ActualizeCnt();
    }

    private void ActualizeCnt()
{
    if (Remaining != null)
    {
        Remaining.text = "Pozostało: " + cnt;
    }
}
}