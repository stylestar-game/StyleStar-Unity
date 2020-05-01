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

    public void Shift(int cardIndex, int currentIndex, bool isAnimating = false, float movementX = 0.0f, float movementY = 0.0f)
    {
        if (!isAnimating)
            cardObj.GetComponent<RectTransform>().anchoredPosition = Globals.CardOrigin + ((cardIndex - currentIndex) * Globals.CardOffset);
        else
            cardObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(movementX, movementY);
    }

    public void SetActive(bool active)
    {
        cardObj.SetActive(active);
    }
}
