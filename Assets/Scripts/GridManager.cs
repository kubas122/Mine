using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private int gridSize = 5;
    [SerializeField] private int numberOfMines = 6;

    private bool[,] mineGrid;
    private GameObject[,] tileGrid;

    private void Start()
    {
        GenerateGrid();
        PlaceMines();
    }

    private void GenerateGrid()
    {
        mineGrid = new bool[gridSize, gridSize];
        tileGrid = new GameObject[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, gridParent);
                tileGrid[x, y] = newTile;

                int tempX = x;
                int tempY = y;
                newTile.GetComponent<Button>().onClick.AddListener(() => OnTileClicked(tempX, tempY));
            }
        }
    }

    private void PlaceMines()
    {
        int placedMines = 0;

        while (placedMines < numberOfMines)
        {
            int x = Random.Range(0, gridSize);
            int y = Random.Range(0, gridSize);

            if (!mineGrid[x, y])
            {
                mineGrid[x, y] = true;
                placedMines++;
            }
        }
    }

    private void OnTileClicked(int x, int y)
    {
        if (mineGrid[x, y])
        {
            Debug.Log("Trafiono na mine!");
            RevealBoard();
        }
        else
        {
            Debug.Log("Bezpieczne pole!");
            tileGrid[x, y].GetComponent<Image>().color = Color.green;
        }
    }

    private void RevealBoard()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (mineGrid[x, y])
                {
                    tileGrid[x, y].GetComponent<Image>().color = Color.red;
                }
                else
                {
                    tileGrid[x, y].GetComponent<Image>().color = Color.green;
                }
                tileGrid[x, y].GetComponent<Button>().interactable = false;
            }
        }
    }
}
