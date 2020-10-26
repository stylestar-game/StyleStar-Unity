using UnityEngine;
public class CaretManager
{
    private int upCaretsDegreeRotateTop = 0;
    private int upCaretsDegreeRotateMid = 0;
    private int upCaretsDegreeRotateBot = 0;
    private int upCaretsFrame = 0;
    private int downCaretsDegreeRotateTop = 0;
    private int downCaretsDegreeRotateMid = 0;
    private int downCaretsDegreeRotateBot = 0;
    private int downCaretsFrame = 0;
    private const int ROTATE_DEG_Y = 6;
    private const int ROTATE_DEF_HALF_ROT = 180;

    public CaretManager()
    {
    }

    public void Enable(GameObject CanvasObj)
    {
        CanvasObj.transform.Find("UpCarets").gameObject.SetActive(true);
        CanvasObj.transform.Find("DownCarets").gameObject.SetActive(true);
    }

    public void Disable(GameObject CanvasObj)
    {
        CanvasObj.transform.Find("UpCarets").gameObject.SetActive(false);
        CanvasObj.transform.Find("DownCarets").gameObject.SetActive(false);
    }

    public void TriggerCarets(StyleStar.Motion currMotion)
    {
        if (currMotion == StyleStar.Motion.Down)
        {
            downCaretsDegreeRotateTop += ROTATE_DEF_HALF_ROT;
            downCaretsFrame += 60;
        }
        else if (currMotion == StyleStar.Motion.Up)
        {
            upCaretsDegreeRotateTop += ROTATE_DEF_HALF_ROT;
            upCaretsFrame += 60;
        }
    }

    public void AnimateCarets(GameObject CanvasObj)
    {
        var downCarets = CanvasObj.transform.Find("DownCarets");
        if (downCaretsDegreeRotateTop > 0)
        {
            downCarets.Find("DownCaret0").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            if (downCaretsFrame % 60 == 45) // 0.25 seconds
                downCaretsDegreeRotateMid += ROTATE_DEF_HALF_ROT;
            downCaretsDegreeRotateTop -= ROTATE_DEG_Y;

        }
        if (downCaretsDegreeRotateMid > 0)
        {
            downCarets.Find("DownCaret1").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            if (downCaretsFrame % 60 == 30) // 0.5 seconds
                downCaretsDegreeRotateBot += ROTATE_DEF_HALF_ROT;
            downCaretsDegreeRotateMid -= ROTATE_DEG_Y;
        }
        if (downCaretsDegreeRotateBot > 0)
        {
            downCarets.Find("DownCaret2").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            downCaretsDegreeRotateBot -= ROTATE_DEG_Y;
        }
        if (downCaretsFrame >= 1)
            downCaretsFrame--;

        var upCarets = CanvasObj.transform.Find("UpCarets");
        if (upCaretsDegreeRotateTop > 0)
        {
            upCarets.Find("UpCaret0").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            if (upCaretsFrame % 60 == 45) // 0.25 seconds
                upCaretsDegreeRotateMid += ROTATE_DEF_HALF_ROT;
            upCaretsDegreeRotateTop -= ROTATE_DEG_Y;

        }
        if (upCaretsDegreeRotateMid > 0)
        {
            upCarets.Find("UpCaret1").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            if (upCaretsFrame % 60 == 30) // 0.5 seconds
                upCaretsDegreeRotateBot += ROTATE_DEF_HALF_ROT;
            upCaretsDegreeRotateMid -= ROTATE_DEG_Y;
        }
        if (upCaretsDegreeRotateBot > 0)
        {
            upCarets.Find("UpCaret2").transform.Rotate(new Vector3(0, 1, 0), ROTATE_DEG_Y);
            upCaretsDegreeRotateBot -= ROTATE_DEG_Y;
        }
        if (upCaretsFrame >= 1)
            upCaretsFrame--;
    }
}
