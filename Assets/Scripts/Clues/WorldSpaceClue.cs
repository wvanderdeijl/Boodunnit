using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldSpaceClue : MonoBehaviour
{
    public Clue ClueScriptableObject;
    public Popup Popup;
    public Popup ToBeContinuedPopup;

    private List<Clue> _listOfClues;

    private void Awake()
    {
        _listOfClues = Resources.LoadAll<Clue>("ScriptableObjects/Clues").ToList();
        
        if (SaveHandler.Instance.DoesPlayerHaveClue(ClueScriptableObject.Name))
        {
            gameObject.SetActive(false);
        }

        Outline outline = gameObject.AddComponent<Outline>();
        if (outline)
        {
            Color clueColor;
            ColorUtility.TryParseHtmlString("#fabd61", out clueColor);
            outline.OutlineColor = clueColor;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 5.0f;
            outline.enabled = false;
        }
    }

    public void AddToInventory()
    { 
        //Add this clue to the inventory of the player
        SaveHandler.Instance.SaveClue(ClueScriptableObject.Name);
        if(Popup)
        {
            Popup.OpenPopup();
        }

        gameObject.SetActive(false);
    }

    private bool DoIHaveAllClues()
    {
        return _listOfClues.All(clue => SaveHandler.Instance.DoesPlayerHaveClue(clue.name));
    }

    private void StartToBeContinuedPopup()
    {
        ToBeContinuedPopup.OpenPopup();
    }

    void OnMouseDown()
    {
        AddToInventory();

        if (DoIHaveAllClues())
        {
            StartToBeContinuedPopup();
        }
    }
}