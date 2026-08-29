

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class CustomButton : Selectable, IPointerEnterHandler, IPointerClickHandler, ISelectHandler, ISubmitHandler
{
    private const string DEFAULT_SELECT_SOUND = "sfx_ui_hover";
    private const string DEFAULT_SUBMIT_SOUND = "sfx_ui_press";
    private const string DEFAULT_RIGHTCLICK_SOUND = "sfx_ui_rightclick";

    // Buttons

    [Serializable]
    /// <summary>
    /// Function definition for a button click event.
    /// </summary>
    public class ButtonClickedEvent : UnityEvent { }
    [Serializable]
    public class ButtonRightClickedEvent : UnityEvent { }

    // Event delegates triggered on click.
    [FormerlySerializedAs("onClick")]
    [SerializeField]
    private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();
    [SerializeField]
    private ButtonRightClickedEvent m_onRightClick = new ButtonRightClickedEvent();

    [Header("Additional Settings")]
    [Tooltip("Animation Type for the button")]
    [SerializeField] private ButtonAnimationType _buttonAnimationType;
    //[Tooltip("The object to be selected by event manager after pressing the button. Leave blank if don't need to select any buttons")]
    [SerializeField] private ButtonUIReference _uiObjectToSelectOnPress;
    [Tooltip("Leave blank for default sfx | Put '-' if to output no sound")]
    [SerializeField] private string _overrideDefaultSelectSound;
    [Tooltip("Leave blank for default sfx | Put '-' if to output no sound")]
    [SerializeField] private string _overrideDefaultSubmitSound;
    [Tooltip("Leave blank for default sfx | Put '-' if to output no sound")]
    [SerializeField] private string _overrideDefaultRightClickSound; 
    [SerializeField] private bool _enableRightClick = false;


    private ButtonAnimationBase _animation;

    

    public ButtonClickedEvent onClick
    {
        get { return m_OnClick; }
        set { m_OnClick = value; }
    }

    public ButtonRightClickedEvent onRightClickedEvent
    {
        get { return m_onRightClick; }
        set { m_onRightClick = value; }
    }

    #region Internal Event Handlers


    // ---- [ Button Clicked ] -----

    // PC Mouse Click
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ButtonPressed();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            ButtonRightClickPressed();
        }
    }

    /// <summary>
    /// Call all registered ISubmitHandler.
    /// </summary>
    /// Take a look at normal Unity Button source code for reference:
    /// </remarks>
    // Controller button click
    public virtual void OnSubmit(BaseEventData eventData)
    {
        ButtonPressed();

        // if we get set disabled during the press
        // don't run the coroutine.
        if (!IsActive() || !IsInteractable())
            return;

        DoStateTransition(SelectionState.Pressed, false);
        StartCoroutine(OnFinishSubmit());
    }

    // ---- [ Button Hover ] -----
    // PC Mouse Hover
    public override void OnPointerEnter(PointerEventData eventData)
     {
        base.OnPointerEnter(eventData);

        ButtonSelected();
    }

    // Controller button select
    public override void OnSelect(BaseEventData eventData)
    {
        base.OnSelect(eventData);

        ButtonSelected();
    }


    // ---- [ Button Deselect ] -----

    // PC mouse Exit
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        ButtonDeselect();
    }

    // Controller button deselect
    public override void OnDeselect(BaseEventData eventData)
    {
        base.OnDeselect(eventData);
        ButtonDeselect();
    }

    #endregion

    protected override void Start()
    {
        Util.WaitNextFrame(this, () =>
        {
            if (_animation == null) InitializeAnimation();
        });

    }
    private void ButtonPressed()
    {
        if (!IsActive() || !IsInteractable())
            return;

        PlayUISound(_overrideDefaultSubmitSound, DEFAULT_SUBMIT_SOUND);

        if (_animation == null) InitializeAnimation();

        _animation.OnButtonClicked();
        
        // Trigger UI Changes
        if (_uiObjectToSelectOnPress.Button != null && _uiObjectToSelectOnPress.UIRoot != null)
        {
            EventSystem.current.SetSelectedGameObject(_uiObjectToSelectOnPress.UIRoot);
            //InputFacade.SetAllUIFocus(_uiObjectToSelectOnPress.UIRoot, _uiObjectToSelectOnPress.Button);
        }

        m_OnClick?.Invoke();
    }

    private void ButtonRightClickPressed()
    {
        if (!IsActive() || !IsInteractable() || !_enableRightClick)
            return;

        PlayUISound(_overrideDefaultRightClickSound, DEFAULT_RIGHTCLICK_SOUND);

        if (_animation == null)
        _animation.OnButtonClicked();

        m_onRightClick?.Invoke();
    }

    private void ButtonSelected()
    {
        PlayUISound(_overrideDefaultSelectSound, DEFAULT_SELECT_SOUND);

        if (_animation == null) InitializeAnimation();
        _animation.OnButtonHighlighted();

    }

    private void ButtonDeselect()
    {
        if (_animation == null) InitializeAnimation();
        _animation.OnButtonDeselect();
    }

    private void PlayUISound(string overrideSound, string defaultSound)
    {
        if (overrideSound == "-")
            return;

        SoundManager.Instance.PlaySound(
            string.IsNullOrEmpty(overrideSound)
                ? defaultSound
                : overrideSound
        );
    }

    private IEnumerator OnFinishSubmit()
    {
        var fadeTime = colors.fadeDuration;
        var elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        DoStateTransition(currentSelectionState, false);
    }

    private void InitializeAnimation()
    {
        if (_buttonAnimationType == ButtonAnimationType.POP)
        {
            _animation = new ButtonAnimationPop(this, _buttonAnimationType, this);
        }
        else if (_buttonAnimationType == ButtonAnimationType.POP_MINOR)
        {
            _animation = new ButtonAnimationPopMinor(this, _buttonAnimationType, this);
        }
        else
        {
            _animation = new ButtonAnimationNone(this, _buttonAnimationType, this);
        }
    }
}

public enum ButtonAnimationType
{
    NONE, POP, POP_MINOR
}

[System.Serializable]
public class ButtonUIReference
{
    public GameObject UIRoot;
    public GameObject Button;
}