using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

public class NonogramManager : MonoBehaviour
{
    public GameObject animation;
    public SkeletonGraphic skeletonGraphic;
    public Image winImage;
    
    public int gridRaw = 5;
    public int gridColumn = 5;
    public GameObject cellPrefab;
    public Transform gridParent;
    private GridLayoutGroup gridLayoutGroup;

    public GameObject clueRawPrefab;
    public Transform clueRowParent;
    private GridLayoutGroup clueRawLayoutGroup;

    public GameObject clueColumnPrefab;
    public Transform clueColumnParent;
    private GridLayoutGroup clueColimnLayoutGroup;

    private int[][] rowClue;
    private int[][] columnClue;

    private int[,] solutionGrid =
    {
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {1, 1, 1, 1, 1},
        {0, 1, 1, 1, 0},
        {0, 0, 1, 0, 0}
    };

    private int[,] playerGrid;


    public void PlayAnimation(string animationName)
    {
        // Проигрывать указанную анимацию
        skeletonGraphic.AnimationState.SetAnimation(0, animationName, false);
    }
    void Start()
    {
        skeletonGraphic.AnimationState.SetAnimation(0, "Level Completed", false); // Укажите имя анимации
        animation.SetActive(false);
        playerGrid = new int[gridRaw, gridColumn];

        GenerateGrid();
        FillClueMatrix(solutionGrid, out rowClue, out columnClue);

        GenerateClueRaw();
        GenerateClueColumn();
    }

    private void GenerateGrid()
    {
        for (int row = 0; row < gridRaw; row++)
        {
            for (int col = 0; col < gridColumn; col++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                Cell cellScript = cell.GetComponent<Cell>();
                cellScript.Initialize(row, col, this);

                cell.GetComponent<Button>().onClick.AddListener(cellScript.ToggleState);
            }
        }
    }

    private void GenerateClueRaw()
    {
        for (int i = 0; i < rowClue.Length; i++)
        {
            GameObject clueCell = Instantiate(clueRawPrefab, clueColumnParent);
            CellClue clue = clueCell.GetComponent<CellClue>();

            clue.Initialization(rowClue[i], i, true);
        }
    }

    private void GenerateClueColumn()
    {
        for (int i = 0; i < columnClue.Length; i++)
        {
            GameObject clueCell = Instantiate(clueColumnPrefab, clueRowParent);
            CellClue clue = clueCell.GetComponent<CellClue>();

            clue.Initialization(columnClue[i], i, false);
        }
    }

    public static void FillClueMatrix(int[,] matrix, out int[][] rowClue, out int[][] columnClue)
    {
        int rowCount = matrix.GetLength(0);
        int colCount = matrix.GetLength(1);

        rowClue = new int[rowCount][];
        columnClue = new int[colCount][];

        for (int i = 0; i < rowCount; i++)
        {
            rowClue[i] = GetClueForLine(matrix, i);
        }
        for (int j = 0; j < colCount; j++)
        {
            columnClue[j] = GetClueForLine(matrix, j, true);
        }
    }

    private static int[] GetClueForLine(int[,] matrix, int index, bool column = false)
    {
        int count = 0;
        int maxClueCount = 0;

        int length = column ? matrix.GetLength(0) : matrix.GetLength(1);
        for (int i = 0; i < length; i++)
        {
            int current = column ? matrix[i, index] : matrix[index, i];

            if (current == 1)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    maxClueCount++;
                    count = 0;
                }
            }
        }

        if (count > 0)
        {
            maxClueCount++;
        }

        int[] clue = new int[maxClueCount];
        int currentClueIndex = 0;
        count = 0;

        for (int i = 0; i < length; i++)
        {
            int current = column ? matrix[i, index] : matrix[index, i];

            if (current == 1)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    clue[currentClueIndex++] = count;
                    count = 0;
                }
            }
        }

        if (count > 0)
        {
            clue[currentClueIndex] = count;
        }

        return clue;
    }

    public void OnCellStateChanged(int row, int column, bool isFilled)
    {
        playerGrid[row, column] = isFilled ? 1 : 0;
        CheckClues();
    }

    private void CheckClues()
    {
        bool isNonogramSolved = true;

        for (int i = 0; i < gridRaw; i++)
        {
            bool rowSolved = IsClueSolved(playerGrid, rowClue[i], i, true);
            bool colSolved = IsClueSolved(playerGrid, columnClue[i], i, false);
            
            CellClue rowClueCell = clueColumnParent.GetChild(i).GetComponent<CellClue>();
            CellClue colClueCell = clueRowParent.GetChild(i).GetComponent<CellClue>();

            rowClueCell.UpdateClueColor(rowSolved);
            colClueCell.UpdateClueColor(colSolved);

            if (!rowSolved || !colSolved)
            {
                isNonogramSolved = false;
            }
        }

        if (isNonogramSolved)
        {
            animation.SetActive(true);
            PlayAnimation("Level Completed");
            ActivateObject();
            
            Debug.Log("Congratulations! The Nonogram is solved.");
        }
    }
    
    public void ActivateObject()
    {
        Invoke("Activate", 1f);
    }

    private void Activate()
    {
        winImage.gameObject.SetActive(true);
    }

    private bool IsClueSolved(int[,] grid, int[] clue, int index, bool isRow)
    {
        int[] line = isRow ? GetLine(grid, index, true) : GetLine(grid, index, false);
        int[] lineClue = GetCluesFromLine(line);

        return clue.Length == lineClue.Length && clue.SequenceEqual(lineClue);
    }

    private int[] GetLine(int[,] grid, int index, bool isRow)
    {
        int[] line = new int[gridRaw];
        for (int i = 0; i < gridRaw; i++)
        {
            line[i] = isRow ? grid[index, i] : grid[i, index];
        }
        return line;
    }

    private int[] GetCluesFromLine(int[] line)
    {
        List<int> clues = new List<int>();
        int count = 0;

        foreach (int cell in line)
        {
            if (cell == 1)
            {
                count++;
            }
            else
            {
                if (count > 0)
                {
                    clues.Add(count);
                    count = 0;
                }
            }
        }
        if (count > 0) clues.Add(count);
        return clues.ToArray();
    }

    public void Win()
    {
        
    }
}
