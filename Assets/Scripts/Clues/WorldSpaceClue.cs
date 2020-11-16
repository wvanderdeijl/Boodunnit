using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceClue : MonoBehaviour
{
    public Clue ClueScriptableObject;

    private void Awake()
    {
        if (SaveHandler.Instance.DoesPlayerHaveClue(ClueScriptableObject.Name))
        {
            Destroy(gameObject);
        }
    }

    public void AddToInventory()
    {
        //Add this clue to the inventory of the player
        SaveHandler.Instance.SaveClue(ClueScriptableObject.Name);
        Destroy(gameObject);
    }
}