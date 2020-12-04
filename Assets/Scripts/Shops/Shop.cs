using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//The type name MUST be the same as the folder name in Resources
public enum ShopType
{
    HatShop,
    Bakery
}

public class Shop : MonoBehaviour
{
    public ShopType ShopType;
    public float ShopRestockCooldown;

    private List<GameObject> _itemPool;
    private List<Transform> _itemLocations;

    private Dictionary<Transform, GameObject> _dicCurrentItemsForSale = new Dictionary<Transform, GameObject>();

    private void Awake()
    {
        _itemPool = Resources.LoadAll<GameObject>($"ShopItemPrefabs/{ShopType}").ToList();
        _itemLocations = transform.Find("Locations (Empty GO)").GetComponentsInChildren<Transform>().Where(t => t.name != "Locations (Empty GO)").ToList();

        foreach (Transform itemLocation in _itemLocations)
        {
            StockItemLocation(itemLocation);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            BuyRandomItem();
        }
    }

    public GameObject BuyRandomItem()
    {
        if (_dicCurrentItemsForSale == null || _dicCurrentItemsForSale.Count == 0)
        {
            return null;
        }

        List<GameObject> listItemsToSell = Enumerable.ToList(_dicCurrentItemsForSale.Values);

        GameObject itemToBuy = listItemsToSell[Random.Range(0, listItemsToSell.Count)];
        Transform itemSlot = _dicCurrentItemsForSale.First(i => i.Value == itemToBuy).Key;

        _dicCurrentItemsForSale.Remove(itemSlot);

        //Destroy(itemToBuy);

        StartCoroutine(RestockEmptyShopSlot(itemSlot));
        return itemToBuy;
    }

    private void StockItemLocation(Transform itemLocation)
    {
        if (_dicCurrentItemsForSale.ContainsKey(itemLocation) && _dicCurrentItemsForSale[itemLocation] != null)
        {
            return;
        }

        bool newItemIsFound = false;
        while (!newItemIsFound)
        {
            GameObject itemToAdd = _itemPool[Random.Range(0, _itemPool.Count)];
            if (!_dicCurrentItemsForSale.Any(i => i.Value.name == itemToAdd.name + "(Clone)"))
            {
                GameObject spawnedItem = Instantiate(itemToAdd, itemLocation.position, Quaternion.identity, itemLocation);
                _dicCurrentItemsForSale.Add(itemLocation, spawnedItem);

                newItemIsFound = true;
            }
        }
    }

    //Restocks the current items for sale if there are empty slots available
    IEnumerator RestockEmptyShopSlot(Transform itemLocationToRestock)
    {
        yield return new WaitForSeconds(ShopRestockCooldown);

        StockItemLocation(itemLocationToRestock);
    }
}

