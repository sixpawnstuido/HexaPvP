using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AvatarElement : MonoBehaviour
{
    public PlayerType playerType;

    [SerializeField] private Image avatarImage;

    [SerializeField] private TextMeshProUGUI collectedHexagonAmountText;
    [SerializeField] private TextMeshProUGUI targetAmountText;

    public void AvatarImageColor(Color32 color32)
    {
        avatarImage.color=color32;
    }


    public void SetCollectedHexagonText()
    {
      //  collectedHexagonAmountText.SetText();
    }

    public void SetTargetAmountText(int target)
    {
        targetAmountText.SetText($"{target}");
    }
}
