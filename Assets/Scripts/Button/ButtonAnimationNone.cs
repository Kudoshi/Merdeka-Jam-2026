
using UnityEngine;

public class ButtonAnimationNone : ButtonAnimationBase
{
    public ButtonAnimationNone(MonoBehaviour mb, ButtonAnimationType animationType, CustomButton button) : base(mb, animationType, button)
    {
    }

    public override void OnButtonClicked()
    {
    }

    public override void OnButtonDeselect()
    {
    }

    public override void OnButtonHighlighted()
    {
    }
}