using Entities.Humans;
using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCanvas : MonoBehaviour
{
    [HideInInspector]
    public GameObject IconTarget;

    public GridLayoutGroup GridLayoutGroup;
    public RectTransform GridLayoutTransform;

    public bool IconCanvasDisabled;
    public Image[] IconImages;

    private List<Image> _enabledIconImages = new List<Image>();

    // Update is called once per frame
    void Update()
    {
        UpdateImagePosition();
    }

    public void DisableIcons()
    {
        foreach (Image iconImage in _enabledIconImages)
        {
            iconImage.gameObject.SetActive(false);
            iconImage.transform.SetParent(transform, false);
        }
        _enabledIconImages.Clear();
    }

    public void EnableIcons()
    {
        DisableIcons();
        IPossessable possessable = IconTarget.GetComponent<IPossessable>();
        if (possessable != null)
        {
            if (possessable.getEmotionalState() == EmotionalState.Fainted)
            {
                EnableIcon(WorldIconType.Ragdoll);
            }
            else if (PossessionBehaviour.PossessionTarget)
            {
                EnableIcon(WorldIconType.TalkTo);
                EnableIconEmotionalStates(possessable);
            }
            else if (!PossessionBehaviour.PossessionTarget)
            {
                EnableIcon(WorldIconType.Possess);
                EnableIconEmotionalStates(possessable);
                if (IconTarget.gameObject.GetComponent<EmmieBehaviour>() != null)
                {
                    EnableIcon(WorldIconType.TalkTo);
                }
            }
        }
        else if (IconTarget.GetComponent<ILevitateable>() != null)
        {
            EnableIcon(WorldIconType.Levitate);
        }
        else if (IconTarget.GetComponent<WorldSpaceClue>() != null)
        {
            EnableIcon(WorldIconType.PickupClue);
        }
        else if (IconTarget.layer == 10)
        {
            EnableIcon(WorldIconType.Dash);
        }
        else if (PossessionBehaviour.PossessionTarget)
        {
            if (IconTarget.GetComponent<AirVent>() != null && PossessionBehaviour.PossessionTarget.GetComponent<BirdBehaviour>() != null)
            {
                EnableIcon(WorldIconType.BirdGlide);
            }
            else if (IconTarget.layer == 12 && PossessionBehaviour.PossessionTarget.GetComponent<RatBehaviour>() != null)
            {
                EnableIcon(WorldIconType.RatClimb);
            }
        }
    }

    private void EnableIconEmotionalStates(IPossessable possessable)
    {
        if (possessable.getEmotionalState() == EmotionalState.Calm)
        {
            EnableIcon(WorldIconType.Normal);
        }
        else if (possessable.getEmotionalState() == EmotionalState.Scared)
        {
            EnableIcon(WorldIconType.Scared);
        }
        else if (possessable.getEmotionalState() == EmotionalState.Terrified)
        {
            EnableIcon(WorldIconType.Terrified);
        }
    }

    private void EnableIcon(WorldIconType iconType)
    {
        foreach (Image iconImage in IconImages)
        {
            if (iconImage.name.Contains(iconType.ToString()))
            {
                _enabledIconImages.Add(iconImage);
                iconImage.transform.SetParent(GridLayoutGroup.transform, false);
                UpdateImagePosition();
                iconImage.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateImagePosition()
    {
        if (_enabledIconImages != null && IconTarget != null)
        {
            GridLayoutTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, CalculateGridWidth());

            Vector3 pos = IconTarget.transform.position;

            if (IconTarget.GetComponent<AirVent>() == null && IconTarget.GetComponent<ClimableBehaviour>() == null)
            {
                pos.y += IconTarget.GetComponent<Collider>().bounds.max.y;
            }

            GridLayoutTransform.position = Camera.main.WorldToScreenPoint(pos);
        }
    }

    private int CalculateGridWidth()
    {
        int listSize = _enabledIconImages.Count;
        if (listSize > 0)
        {
            return (listSize * 40) + ((listSize - 1) * 5);
        }
        return 0;
    }
}
