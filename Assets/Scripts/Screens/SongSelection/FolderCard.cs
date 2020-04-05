using StyleStar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FolderCard : CardBase
{
    public FolderCard(GameObject obj, string text) : base(obj)
    {
        // Set colors
        cardObj.SetColor(ThemeColors.GetColor(instanceNum));
        cardObj.transform.Find("StarAccent").gameObject.SetColor(ThemeColors.GetColor(instanceNum).LerpBlackAlpha(0.3f, 0.1f));

        // Set text
        cardObj.transform.Find("Text").gameObject.SetText(text);

        instanceNum++;
    }
}
