using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    private bool isGameOn = false;

    [SerializeField] private GameObject blackCellVisual;
    [SerializeField] private GameObject whiteCellVisual;
    [SerializeField] private GameObject mine;
    [SerializeField] private GameObject blocker;
    [SerializeField] private TextMeshProUGUI restartText;
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private TextMeshProUGUI captureText;
    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private float cellSize = 1.5f;

    private const int gridSize = 10;
    private const int blueMineCount = 50;
    private const int peekTime = 3;

    private Mine[,] mines;
    private Blocker[,] blockers;
    private Grid<Vector2Int?> grid;
    private int correctCount;
    private bool isGuessingRed;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        grid = new Grid<Vector2Int?>(gridSize, gridSize, cellSize, Vector2.zero);

        mines = new Mine[gridSize, gridSize];
        blockers = new Blocker[gridSize, gridSize];
        for (int i = 0; i < gridSize; ++i)
            for (int j = 0; j < gridSize; ++j)
            {
                grid.SetValue(i, j, new Vector2Int(i, j));
                Vector2 position = grid.GetWorldPosition(i, j) + new Vector2(cellSize / 2, cellSize / 2);
                bool isBlack = (i + j) % 2 == 0;
                Instantiate(isBlack ? blackCellVisual : whiteCellVisual, new Vector3(position.x, position.y, 1), Quaternion.identity);
                mines[i, j] = Instantiate(mine, new Vector3(position.x, position.y, 0), Quaternion.identity).GetComponent<Mine>();
                blockers[i, j] = Instantiate(blocker, new Vector3(position.x, position.y, -1), Quaternion.identity).GetComponent<Blocker>();
            }
    }

    private void Update()
    {
        if (!isGameOn) return;
        ClickMine();
        CaptureBlocker();
    }

    private void ClickMine()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int? coords = grid.GetValue(worldPos);
            if (coords == null) return; //missed the grid

            blockers[coords.Value.x, coords.Value.y].Hide();
            ProcessMine(coords.Value.x, coords.Value.y);


        }
    }
    private const int maxCaptureCount = 3;
    private int captureCount = 3;
    private void CaptureBlocker()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int? coords = grid.GetValue(worldPos);
            if (coords == null) return; //missed the grid

            if (captureCount-- >= 1)
            {
                blockers[coords.Value.x, coords.Value.y].Hide();
                captureText.text = "ЗАХВАТ: " + captureCount.ToString();
            }

        }
    }
    public void RestartGame()
    {
        StartCoroutine(RestartCoroutine());
    }
    private IEnumerator RestartCoroutine()
    {
        isGameOn = false;

        gameOverText.gameObject.SetActive(false);
        correctCount = 0;

        ResetMines();

        HideBlockers();
        yield return new WaitForSeconds(peekTime);
        ShowBlockers();


        isGameOn = true;
    }

    private void ProcessMine(int x, int y)
    {
        Mine mine = mines[x, y];
        if (mine.isChecked) return;
        mine.isChecked = true;

        if (correctCount <= 0)
        {
            isGuessingRed = mine.isRed;
            correctCount = 1;
            UpdateCounterText();
        }
        else
        {
            if (isGuessingRed == mine.isRed)
            {
                correctCount++;
                UpdateCounterText();
            }
            else
            {
                isGameOn = false;
                DisplayGameOverText();
                restartText.text = "РЕСТАРТ";
            }
        }
    }


    private void ResetMines()
    {
        List<Vector2Int> coords = new List<Vector2Int>();
        for (int i = 0; i < gridSize; ++i)
            for (int j = 0; j < gridSize; ++j)
            {
                coords.Add(new Vector2Int(i, j));
                mines[i, j].isRed = true;
                mines[i, j].isChecked = false;
                mines[i, j].Redraw();
            }

        for (int i = 0; i < blueMineCount; ++i)
        {
            int index = UnityEngine.Random.Range(0, coords.Count);
            Vector2Int cell = coords[index];
            mines[cell.x, cell.y].isRed = false;

            mines[cell.x, cell.y].Redraw();
            coords.Remove(cell);
        }

    }
    private void HideBlockers()
    {
        for (int i = 0; i < gridSize; ++i)
            for (int j = 0; j < gridSize; ++j)
                blockers[i, j].Hide();

    }
    private void ShowBlockers()
    {
        for (int i = 0; i < gridSize; ++i)
            for (int j = 0; j < gridSize; ++j)
                blockers[i, j].Show();
    }


    private void UpdateCounterText()
    {
        if (counterText != null)
            counterText.text = "СЧЕТ : " + correctCount.ToString();
    }

    private void DisplayGameOverText()
    {
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "игра окончена,  ваш счет: " + correctCount.ToString();
        }
    }
}