using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI WhoText;
    public TextMeshProUGUI[] buttonList;
    private string playerSide;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    private int moveCount;
    public GameObject RestartButton;

     void Awake()
    {
        RestartButton.SetActive(false);
        gameOverPanel.SetActive(false);    
        playerSide = "X";
        WhoText.text = "Player " + playerSide + " ist am Zug!";
        SetGameControllerReferenceOnButtons();
        moveCount = 0;
    }
    void SetGameControllerReferenceOnButtons()
    {
        for(int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameControllerReference(this);
        }
    }
     public string GetPlayerSide()
    {
        return playerSide;
    }
    public void EndTurn()
    {
        moveCount++;
        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide)
        {
            GameOver(playerSide);
        }
        if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        } 
        if(buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver(playerSide);
        }
        if(moveCount >= 9)
        {
            GameOver("unentschieden");
        }   
        ChangeSides();
    }
    void GameOver(string winningPlayer)
    {    
        SetBoardInteractable(false);
        if(winningPlayer == "unentschieden")
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = "Es ist ein unentschieden!!";
        }
        else
        {
            gameOverPanel.SetActive(true);
            gameOverText.text = playerSide + " hat gewonnen!!";
        }
        RestartButton.SetActive(true);
    }
    void ChangeSides()
    {
        playerSide = (playerSide == "X") ? "O" : "X";
        WhoText.text = "Player " + playerSide + " ist am Zug!";
    }
    void SetGameOverText(string value)
    {    
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }
    public void RestartGame()
    {
        RestartButton.SetActive(false);
        playerSide = "X";
        moveCount = 0;
        gameOverPanel.SetActive(false);
        SetBoardInteractable(true);
        for (int i = 0; i < buttonList.Length; i++) 
        { 
             buttonList[i].GetComponentInParent<Button>().interactable = true; 
             buttonList[i].text = ""; 
        }
    }
    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++) 
        { 
             buttonList[i].GetComponentInParent<Button>().interactable = toggle; 
        }
    }
}
