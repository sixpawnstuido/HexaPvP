using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EmojiBase : MonoBehaviour
{
    private List<Animator> _emojis;

    private void Awake()
    {
        _emojis = GetComponentsInChildren<Animator>(true).ToList();
    }
    private void OnEnable()
    {
        CloseEmojis();
        _emojis[_emojis.Count-1].gameObject.SetActive(true);
    }
    private void CloseEmojis()
    {
        foreach (var item in _emojis)
        {
            item.gameObject.SetActive(false);
        }
    }
}
