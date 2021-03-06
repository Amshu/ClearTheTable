﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class CardDeck : MonoBehaviour
{
    [SerializeField]
    private GameObject _cardPrefab;

    public readonly List<Card> CardList = new List<Card>();

    public void InstanatiateDeck(string cardBundlePath)
    {
        AssetBundle cardBundle = BundleSingleton.Instance.LoadBundle(DirectoryUtility.ExternalAssets() + cardBundlePath);
        string[] nameArray = cardBundle.GetAllAssetNames();

        for (int i = 0; i < nameArray.Length; ++i)
        {
            GameObject cardInstance = (GameObject)Instantiate(_cardPrefab);
            Card card = cardInstance.GetComponent<Card>();
            card.gameObject.name = Path.GetFileNameWithoutExtension(nameArray[i]);
            card.TexturePath = nameArray[i];
            card.SourceAssetBundlePath = cardBundlePath;
            card.transform.position = new Vector3(0, 10, 0);
            //----------------------Amshu--------------------------
            card.cardValue = StringToValue(card.gameObject.name);
            //----------------------Amshu--------------------------
            //card.FaceValue = StringToFaceValue(card.gameObject.name);
            CardList.Add(card);
        }
    }

    private int StringToFaceValue(string input)
    {
        for (int i = 2; i < 11; ++i)
        {
            if (input.Contains(i.ToString()))
            {
                return i;
            }
        }
        if (input.Contains("K") ||
            input.Contains("C") ||
            input.Contains("J"))
        {
            return 10;
        }
        if (input.Contains("T"))
        {
            return 11;
        }
        return 0;
    }

    //---------------------Amshu-----------------------//
    private Vector2 StringToValue(string input)
    {
        int no = -1;

        if (input.StartsWith("t"))
        {
            //Debug.Log("Throne Card");
            no = 1;
        }
        // If the card is K, C or J
        else if (input.StartsWith("c"))
        {
            //Debug.Log("Face Card C");
            no = 11;
        }
        else if (input.StartsWith("j"))
        {
            //Debug.Log("Face Card J");
            no = 12;
        }
        else if (input.StartsWith("k"))
        {
            //Debug.Log("Face Card K");
            no = 13;
        }
        else
        {
            for (int i = 2; i < 11; i++)
            {
                if (input.StartsWith(i.ToString()))
                {
                    no = i;
                    break;
                }
            }
        }
        
        if (input.Contains("arrows"))
        {
            return new Vector2(no, 1);
        }
        else if (input.Contains("bones"))
        {
            return new Vector2(no, 2);
        }
        else if (input.Contains("hammers"))
        {
            return new Vector2(no, 3);
        }
        else if (input.Contains("swords"))
        {
            return new Vector2(no, 4);
        }
        return new Vector2(0, 0);
    }
    //------------------------------------------------//
}
