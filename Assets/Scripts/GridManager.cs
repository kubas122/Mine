using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Slider minesSlider;
    [SerializeField] private Button betButton;
    [SerializeField] private TextMeshProUGUI minesCountText;
    [SerializeField] private int gridSize = 5;

    private int numberOfMines;
    private bool[,] mineGrid;
    private GameObject[,] tileGrid;

    private void Start()
    {
        numberOfMines = Mathf.RoundToInt(minesSlider.value);
        minesSlider.onValueChanged.AddListener(UpdateMineCount);
        UpdateMineCount(minesSlider.value);
        betButton.interactable = true;
    }

    private void UpdateMineCount(float value)
    {
        numberOfMines = Mathf.RoundToInt(value);
        minesCountText.text = numberOfMines.ToString();
    }

    public void OnBetClicked()
    {
        ClearGrid();
        GenerateGrid();
        PlaceMines();
    }

    private void ClearGrid()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
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
            RevealBoard();
        }
        else
        {
            DiscoverTile(x, y);
        }
    }

    private void DiscoverTile(int x, int y)
    {
        tileGrid[x, y].GetComponent<Image>().color = Color.green;
        tileGrid[x, y].GetComponent<Button>().interactable = false;
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
