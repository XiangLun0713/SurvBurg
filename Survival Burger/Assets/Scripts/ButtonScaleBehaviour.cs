using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScaleBehaviour : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Vector3 _originalScale;
    private Button _button;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _originalScale = _rectTransform.localScale;
        _button = GetComponent<Button>();
    }

    public void ScaleUpButton()
    {
        if (_rectTransform != null && _button != null && _button.interactable) _rectTransform.localScale = _originalScale * 1.1f;
    }

    public void ScaleDownButton()
    {
        if (_rectTransform != null && _button != null && _button.interactable) _rectTransform.localScale = _originalScale;
    }
}