using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    public static ComboManager Instance;

   [SerializeField] private float _comboTimer = 3f;
    private float _comboTimerConstant;

   [ReadOnly] public int comboStage;

    private bool _canUpdate;

    [SerializeField] private List<ComboElement> _comboElementList;

    private void Awake()
    {
        Instance = this;
        _comboTimerConstant = _comboTimer;
    }

    private void Update()
    {
        if (!_canUpdate) return;
        _comboTimer -= Time.deltaTime;
        if (_comboTimer <= 0)
        {
            comboStage = 0;
            _canUpdate = false;
        }
    }

    public void IncreaseComboStage(Vector3 hexagonHolderPos)
    {
        comboStage++;
        _comboTimer = _comboTimerConstant;
        _canUpdate = true;
        if (comboStage > 1)
        {
            ComboState(hexagonHolderPos);
        }
    }

    public void ComboState(Vector3 hexagonHolderPos)
    {
        var comboElement = ReturnComboElement();
        int comboStageCount = comboStage - 2;
        comboElement.ComboAnim(comboStageCount, hexagonHolderPos);
    }

    private ComboElement ReturnComboElement()
    {
        var comboElement = _comboElementList.FirstOrDefault(g => !g.gameObject.activeInHierarchy);
        if (comboElement)
        {
            return comboElement;
        }
        else
        {
            var tempComboElement = Instantiate(_comboElementList[0], transform);
            tempComboElement.gameObject.SetActive(false);
            _comboElementList.Add(tempComboElement);
            return tempComboElement;
        }
    }

    public void ResetComboStage()
    {
        _comboTimer = 0;
        comboStage = 0;
        _canUpdate = false;
    }
}