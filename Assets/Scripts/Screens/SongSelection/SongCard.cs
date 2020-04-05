using StyleStar;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SongCard : CardBase
{
    private SongMetadata metadata;
    public string SongID { get { return metadata.SongID; } }

    public SongCard(GameObject obj, SongMetadata meta) : base(obj)
    {
        metadata = meta;

        // Set colors
        cardObj.SetColor(meta.ColorBack.IfNull(ThemeColors.GetColor(instanceNum)));
        cardObj.transform.Find("StarAccent").gameObject.SetColor(meta.ColorFore.IfNull(ThemeColors.GetColor(instanceNum).LerpBlackAlpha(0.3f, 0.1f)));
        cardObj.transform.Find("AlbumAccent").gameObject.SetColor(meta.ColorFore.IfNull(ThemeColors.GetColor(instanceNum).LerpBlackAlpha(0.3f, 0.1f)));

        // Set album image
        if (metadata.AlbumImage != null)
            cardObj.transform.Find("AlbumImage").gameObject.GetComponent<Image>().sprite = metadata.AlbumImage;

        // Set title/artist cards or text depending on what's available
        if(metadata.TitleImage != null)
        {
            cardObj.transform.Find("TitleImage").gameObject.GetComponent<Image>().sprite = metadata.TitleImage;
            cardObj.transform.Find("TitleImage").gameObject.SetActive(true);
            cardObj.transform.Find("TitleText").gameObject.SetActive(false);
        }
        else
            cardObj.transform.Find("TitleText").gameObject.SetText(meta.Title);

        if(metadata.ArtistImage != null)
        {
            cardObj.transform.Find("ArtistImage").gameObject.GetComponent<Image>().sprite = metadata.ArtistImage;
            cardObj.transform.Find("ArtistImage").gameObject.SetActive(true);
            cardObj.transform.Find("ArtistText").gameObject.SetActive(false);
        }
        else
            cardObj.transform.Find("ArtistText").gameObject.SetText(meta.Artist);

        // Set song difficulty numbers
        string[] names = new string[] { "Difficulty", "ActiveDifficulty0", "ActiveDifficulty1", "ActiveDifficulty2" };

        foreach (var name in names)
        {
            for (int i = 0; i < 3; i++)
            {
                var difficultyText = cardObj.transform.Find(name).gameObject.transform.Find("Difficulty" + i).gameObject;

                if (meta.ChildMetadata.Count == 0)
                {
                    if ((int)meta.Difficulty == i)
                    {
                        difficultyText.SetActive(true);
                        difficultyText.SetText(meta.Level.ToString("D2"));
                    }
                    else
                        difficultyText.SetActive(false);
                }
                else
                {
                    var child = meta.ChildMetadata.FirstOrDefault(x => (int)x.Difficulty == i);
                    if (child != null)
                    {
                        difficultyText.SetActive(true);
                        difficultyText.SetText(child.Level.ToString("D2"));
                    }
                    else
                        difficultyText.SetActive(false);
                }
                if (name.Contains("Active"))
                    cardObj.transform.Find(name).gameObject.SetActive(false);
            }
        }
        

        instanceNum++;
    }

    public void SetDifficulty(int difficulty)
    {
        for (int i = 0; i < 3; i++)
            cardObj.transform.Find("ActiveDifficulty" + i).gameObject.SetActive(i == difficulty);
    }
}
