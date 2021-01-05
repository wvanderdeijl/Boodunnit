using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCanvas : MonoBehaviour
{
    [HideInInspector]
    public GameObject GameObject;
    public bool IconCanvasDisabled;
    public List<Image> _enabledIconImages;
    public List<Image> _enabledAlwaysActiveIconImages;
    public Image[] IconImages;
    public Sprite[] sprites;
    public GameObject[] AlwaysEnabledIconLocations;
    public Image ImagePrefab;


    // Update is called once per frame
    void Update()
    {
        UpdateImagePosition();
        UpdateAlwaysActiveImagePosition();
    }

    public void DisableIcons()
    {
        foreach (Image iconImage in _enabledIconImages)
        {
            iconImage.gameObject.SetActive(false);
        }
        _enabledIconImages.Clear();
    }

    public void DisableAlwaysActiveIcons()
    {
        foreach (Image iconImage in _enabledAlwaysActiveIconImages)
        {
            iconImage.gameObject.SetActive(false);
        }
        foreach (Image iconImage in _enabledAlwaysActiveIconImages)
        {
            Destroy(iconImage.gameObject);
        }
        _enabledAlwaysActiveIconImages.Clear();
    }

    public void EnableIcons()
    {
        if (GameObject.GetComponent<IPossessable>() != null)
        {
            if (PossessionBehaviour.PossessionTarget)
            {
                EnableIcon(WorldIconType.TalkTo);
            }
            else
            {
                EnableIcon(WorldIconType.Possess);
            }
        }
        else if (GameObject.GetComponent<ILevitateable>() != null)
        {
            EnableIcon(WorldIconType.Levitate);
        }
        else if (GameObject.GetComponent<WorldSpaceClue>() != null)
        {
            EnableIcon(WorldIconType.PickupClue);
        }
    }

    public void EnableAlwaysActiveIcons()
    {
        if (PossessionBehaviour.PossessionTarget)
        {
            if (PossessionBehaviour.PossessionTarget.GetComponent<BirdBehaviour>() != null)
            {
                EnableAlwaysEnabledIcon(WorldIconType.BirdGlide);
            }
            else if (PossessionBehaviour.PossessionTarget.GetComponent<RatBehaviour>() != null)
            {
                EnableAlwaysEnabledIcon(WorldIconType.RatClimb);
            }
        }
    }

    private void EnableIcon(WorldIconType iconType)
    {
        foreach (Image image in IconImages)
        {
            if (image.name.Contains(iconType.ToString()))
            {
                _enabledIconImages.Add(image);
                UpdateImagePosition();
                image.gameObject.SetActive(true);
            }
        }
    }

    private void EnableAlwaysEnabledIcon(WorldIconType iconType)
    {
        foreach (GameObject location in AlwaysEnabledIconLocations)
        {
            if (location.name.Contains(iconType.ToString()))
            {
                Image iconImage = Instantiate(ImagePrefab, Vector3.zero, Quaternion.identity);
                iconImage.name = "Image" + location.name;
                foreach (Sprite sprite in sprites)
                {
                    if (sprite.name.Contains(iconType.ToString()))
                    {
                        iconImage.sprite = sprite;
                        iconImage.transform.SetParent(transform, false);
                        break;
                    }
                }
                _enabledAlwaysActiveIconImages.Add(iconImage);
                UpdateImagePosition();
                iconImage.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateImagePosition()
    {
        if (_enabledIconImages != null)
        {
            foreach (Image image in _enabledIconImages)
            {
                image.rectTransform.position = Camera.main.WorldToScreenPoint(GameObject.transform.position);
            }
        }
    }

    private void UpdateAlwaysActiveImagePosition()
    {
        foreach (Image image in _enabledAlwaysActiveIconImages)
        {
            foreach (GameObject location in AlwaysEnabledIconLocations)
            {
                if (image.name.Contains(location.name))
                {
                    image.rectTransform.position = Camera.main.WorldToScreenPoint(location.transform.position);
                    break;
                }
            }
        }
    }
}
