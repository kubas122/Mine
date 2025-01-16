using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private Slider minesSlider;
    [SerializeField] private Button betButton;
    [SerializeField] private TMP_InputField betInputField;
    [SerializeField] private TextMeshProUGUI minesCountText;
    [SerializeField] private TextMeshProUGUI inGameMinesText;
    [SerializeField] private TextMeshProUGUI gemsCountText;
    [SerializeField] private TextMeshProUGUI totalProfitText;
    [SerializeField] private TextMeshProUGUI betAmountText;
    [SerializeField] private Button cashoutButton;
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject inGamePanel;
    [SerializeField] private int gridSize = 5;

    private int numberOfMines;
    private int gemsCollected;
    private float totalProfit;
    private float betAmount;
    private bool gameActive;

    private bool[,] mineGrid;
    private GameObject[,] tileGrid;

    private void Start()
    {
        numberOfMines = Mathf.RoundToInt(minesSlider.value);
        minesSlider.onValueChanged.AddListener(UpdateMineCount);
        UpdateMineCount(minesSlider.value);
        betButton.onClick.AddListener(StartGame);
        cashoutButton.onClick.AddListener(Cashout);
        betButton.interactable = true;
        inGamePanel.SetActive(false);
    }

    private void UpdateMineCount(float value)
    {
        numberOfMines = Mathf.RoundToInt(value);
        int gems = gridSize * gridSize - numberOfMines;
        minesCountText.text = "Mines: " + numberOfMines + " Gems: " + gems;
    }

    public void StartGame()
    {
        if (!float.TryParse(betInputField.text, out betAmount) || betAmount <= 0)
        {
            Debug.LogError("Enter a positive number.");
            return;
        }

        leftPanel.SetActive(false);
        inGamePanel.SetActive(true);
        ClearGrid();
        GenerateGrid();
        PlaceMines();
        gemsCollected = 0;
        totalProfit = 0f;
        UpdateInGameUI();
        gameActive = true;
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
        if (!gameActive) return;

        if (mineGrid[x, y])
        {
            gameActive = false;
            RevealBoard();
            Invoke(nameof(ResetAfterLoss), 3f);
        }
        else
        {
            gemsCollected++;
            totalProfit += CalculateProfit();
            DiscoverTile(x, y);
            UpdateInGameUI();
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

    private void UpdateInGameUI()
    {
        int remainingGems = gridSize * gridSize - numberOfMines - gemsCollected;
        inGameMinesText.text = "Mines: " + numberOfMines;
        gemsCountText.text = "Gems: " + remainingGems;
        totalProfitText.text = "Total Profit: $" + totalProfit.ToString("F2");
        betAmountText.text = "Bet: $" + betAmount.ToString("F2");
    }

    private float CalculateProfit()
    {
        return betAmount * gemsCollected * 0.1f * numberOfMines;
    }

    private void Cashout()
    {
        gameActive = false;
        RevealBoard();
        Invoke(nameof(ClearAfterCashout), 3f);
    }

    private void ClearAfterCashout()
    {
        ClearGrid();
        gemsCollected = 0;
        totalProfit = 0f;
        betAmount = 0f;
        inGameMinesText.text = "";
        gemsCountText.text = "";
        totalProfitText.text = "";
        betAmountText.text = "";
        inGamePanel.SetActive(false);
        leftPanel.SetActive(true);
        betInputField.text = "";
    }

    private void ResetAfterLoss()
    {
        ClearGrid();
        gemsCollected = 0;
        totalProfit = 0f;
        betAmount = 0f;
        inGameMinesText.text = "";
        gemsCountText.text = "";
        totalProfitText.text = "";
        betAmountText.text = "";
        inGamePanel.SetActive(false);
        leftPanel.SetActive(true);
        betInputField.text = "";
    }
}
