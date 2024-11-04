using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasHandler : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI buttonMoneyText;

    public GameObject tutorialUI;
    public GameObject levelEndUI;
    public GameObject levelFailedUI;

    public void GetMoneyButton(int currentMoney)
    {
        moneyText.text = currentMoney.ToString();
    }
}
