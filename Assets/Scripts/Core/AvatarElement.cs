using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarElement : MonoBehaviour
{
    public PlayerType playerType;

    [SerializeField] private Image avatarImage;

    public void AvatarImageColor(Color32 color32)
    {
        avatarImage.color=color32;
    }
}
