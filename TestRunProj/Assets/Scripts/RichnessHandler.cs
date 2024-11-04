using System.Collections;
using System.Collections.Generic;
using ButchersGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RichnessHandler : MonoBehaviour
{
    [SerializeField] private int currentMoney = 0;
    [SerializeField] private int allMoney = 0;

    // Money thresholds for activating each model
    [Header("Money Thresholds")]
    [SerializeField] private int[] moneyThresholds = { 100, 500, 1000, 2000, 5000 };
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Slider moneySlider;

    [Header("Player Models")]
    [SerializeField] private GameObject[] playerModels;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CanvasHandler _canvasHandler;

    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private Transform spawnPoint;

    private int activeModelIndex = -1;

    void Start()
    {
        ResetModel();
        InitializeSlider();
    }

    private void ResetModel()
    {
        currentMoney = 50;
        playerMovement.isStarted = false;


        UpdateActiveModel();
        UpdateMoneyText();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        Debug.Log("Money added. Current Money: " + currentMoney);
        UpdateActiveModel();
        UpdateMoneyText();
        UpdateSlider();
        soundManager.PlaySound3();
    }

    public void SubtractMoney(int amount)
    {
        currentMoney = Mathf.Max(0, currentMoney - amount);
        Debug.Log("Money subtracted. Current Money: " + currentMoney);
        UpdateActiveModel();
        UpdateMoneyText();
        UpdateSlider();
        soundManager.PlaySound2();
        
        if (currentMoney <= 0)
        {
            FailedGame();
            soundManager.PlaySound4();
        }
    }
    
    private void InitializeSlider()
    {
        if (moneySlider != null)
        {
            moneySlider.minValue = moneyThresholds[0];
            moneySlider.maxValue = moneyThresholds[moneyThresholds.Length - 1];
            moneySlider.value = currentMoney;
        }
    }

    private void UpdateActiveModel()
    {
        int newActiveModelIndex = -1;
        for (int i = 0; i < moneyThresholds.Length; i++)
        {
            if (currentMoney >= moneyThresholds[i])
            {
                newActiveModelIndex = i;
            }
        }
        
        if (newActiveModelIndex == -1)
        {
            newActiveModelIndex = 0;
        }


        if (newActiveModelIndex != activeModelIndex)
        {
            // Deactivate the currently active model
            if (activeModelIndex >= 0 && activeModelIndex < playerModels.Length)
            {
                playerModels[activeModelIndex].SetActive(false);
            }

            // Activate the new model
            if (newActiveModelIndex >= 0 && newActiveModelIndex < playerModels.Length)
            {
                playerModels[newActiveModelIndex].SetActive(true);
                SetAnimatorParameters(newActiveModelIndex);
                Debug.Log("Model activated: " + playerModels[newActiveModelIndex].name);
            }
            
            activeModelIndex = newActiveModelIndex;
        }
    }
    
    private void SetAnimatorParameters(int modelIndex)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Poor", false);
            playerAnimator.SetBool("Casual", false);
            playerAnimator.SetBool("Rich", false);

            if (playerMovement.isStarted)
            {
                // Set parameters based on the model index
                if (modelIndex == 0)
                {
                    playerAnimator.SetTrigger("Spin");
                    playerAnimator.SetBool("Poor", true);
                }
                else if (modelIndex == 1 || modelIndex == 2)
                {
                    playerAnimator.SetTrigger("Spin");
                    playerAnimator.SetBool("Casual", true);
                }
                else if (modelIndex == 3 || modelIndex == 4)
                {
                    playerAnimator.SetTrigger("Spin");
                    playerAnimator.SetBool("Rich", true);
                }
            }
        }
    }
    
    private void UpdateMoneyText()
    {
        if (moneyText != null)
        {
            moneyText.text = currentMoney.ToString();
        }
    }
    
    private void UpdateSlider()
    {
        if (moneySlider != null)
        {
            moneySlider.value = currentMoney; // Update the slider value to match the current money
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            AddMoney(other.GetComponent<MoneyHandler>().moneyAmount);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Bottle"))
        {
            SubtractMoney(other.GetComponent<MoneyHandler>().moneyAmount);
            Destroy(other.gameObject);
        }
        
        // Check for finish line triggers
        if (other.CompareTag("FinishLine"))
        {
            int activeModelIndex = ActiveModelIndex;
            
                int finishLineIndex = other.GetComponent<FinishHandler>().finishIndex;
                if (finishLineIndex <= activeModelIndex)
                {
                    // Activate the "Open" trigger in the finish line's animator
                    Animator finishLineAnimator = other.GetComponent<Animator>();
                    if (finishLineAnimator != null)
                    {
                        finishLineAnimator.SetTrigger("Open");
                        Debug.Log("Opened finish line: " + other.name);
                        other.enabled = false;
                    }
                    other.enabled = false;
                }
                else
                {
                    // Player cannot pass this finish line, end the game or show a message
                    Debug.Log("Player cannot pass this finish line: " + other.name);
                    other.GetComponent<SoundManager>().PlaySound2();
                    EndGame();
                    other.enabled = false;
                }
                
        }
    }
    
    public int ActiveModelIndex
    {
        get { return activeModelIndex; }
    }

    private void EndGame()
    {
        _canvasHandler.levelEndUI.SetActive(true);
        _canvasHandler.buttonMoneyText.text = currentMoney.ToString();
        playerMovement.isLevelEnded = true;
    }
    
    private void FailedGame()
    {
        _canvasHandler.levelFailedUI.SetActive(true);
        playerMovement.isLevelEnded = true;
    }

    public void GetMoney()
    {
        SetPlayerSpawnPosition();
        allMoney += currentMoney;
        _canvasHandler.GetMoneyButton(allMoney);
        StartCoroutine(StartNextLevel());
        _canvasHandler.tutorialUI.SetActive(true);
        _canvasHandler.levelEndUI.SetActive(false);
        playerMovement.isLevelEnded = false;
        playerMovement.isStarted = false;
        playerMovement.RotatePlayer(-90f);
        ResetModel();
    }
    
    public void RestartLevel()
    {
        SetPlayerSpawnPosition();
        StartCoroutine(RestartLevelCO());
        _canvasHandler.tutorialUI.SetActive(true);
        _canvasHandler.levelEndUI.SetActive(false);
        _canvasHandler.levelFailedUI.SetActive(false);
        playerMovement.isLevelEnded = false;
        playerMovement.isStarted = false;
        ResetModel();
    }

    private IEnumerator StartNextLevel()
    {
        yield return new WaitForSeconds(0.1f);
        _levelManager.NextLevel();
    }
    
    private IEnumerator RestartLevelCO()
    {
        yield return new WaitForSeconds(0.1f);
        _levelManager.RestartLevel();
    }
    
    private void SetPlayerSpawnPosition()
    {
        // Set the player's position to the spawn point
        transform.position = spawnPoint.position;
        
    }
}
