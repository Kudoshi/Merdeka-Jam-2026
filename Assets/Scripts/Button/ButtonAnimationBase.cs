
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonAnimationBase
{
    protected MonoBehaviour _mb;
    protected ButtonAnimationType _animationType;
    protected Image _target;
    protected CustomButton _button;

    protected ButtonAnimationBase(MonoBehaviour mb, ButtonAnimationType animationType, CustomButton button)
    {
        _mb = mb;
        _animationType = animationType;
        _button = button;
        _target = _button.image;
    }


    public abstract void OnButtonClicked();

    public abstract void OnButtonHighlighted();

    public abstract void OnButtonDeselect();

}