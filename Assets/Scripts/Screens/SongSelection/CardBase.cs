using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBase
{
    protected GameObject cardObj;
    protected static int instanceNum = 0;
    public bool IsActive { get { return cardObj.activeInHierarchy; } }

    public CardBase(GameObject obj)
    {
        cardObj = obj;
        cardObj.SetActive(true);
    }

    public void Shift(int cardIndex, int currentIndex)
    {
        cardObj.GetComponent<RectTransform>().anchoredPosition = Globals.CardOrigin + (cardIndex - currentIndex) * Globals.CardOffset;
    }

    public void SetActive(bool active)
    {
        cardObj.SetActive(active);
    }
}
