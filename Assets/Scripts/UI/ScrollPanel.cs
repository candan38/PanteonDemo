using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPanel : MonoBehaviour
{
    /*                                                     AÇIKLAMA
     *       
     *       Uygulamaya GridLayoutGroup ve ContentSizeFitter componentleri ile default deðerleri belirleyerek baþlýyoruz böylelikle 
     *       content elemanlarýmýz belirlediðimiz düzende baþlýyor. Ancak scroll baþladýðýnda bu componentleri pasif hale getiriyoruz
     *       çünkü GridLayoutGroup alt objelerin rectTransformuna eriþip düzenlemeye engel oluyor ve boyutlandýrmayý kendimiz yaptýðýmýz 
     *       için ContentSizeFitter'a ihtiyacýmýz kalmýyor.
     *    
     */

    private bool componentBool;

    [SerializeField] private GameObject button1;
    [SerializeField] private GameObject button2;

    private ScrollRect scrollRect;
    private RectTransform rectTransform;
    private RectTransform[,] rectTransforms;
    private RectOffset gridLayoutPadding;

    private Vector2 curButtonSize;

    private float height;
    private float offsetY;
    private float marginY;
    private int rowCounter;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        scrollRect = GetComponent<ScrollRect>();
        height = rectTransform.rect.height;
        scrollRect.onValueChanged.AddListener(OnScroll);
        gridLayoutPadding = scrollRect.content.GetComponent<GridLayoutGroup>().padding;

        updateCellSize();
        updateCellCount();
        updateCells();
        addButtons();
    }

    void updateCellSize()
    {
        //Ekran geniþliðine göre ihtiyacýmýz olan kare buton boyutlarýný belirliyoruz
        curButtonSize.x = (rectTransform.rect.width - gridLayoutPadding.left - gridLayoutPadding.right - scrollRect.content.GetComponent<GridLayoutGroup>().spacing.x) / 2;
        curButtonSize.y = curButtonSize.x;
    }
    void updateCellCount()
    {
        // GridLayoutGroup'ta belirlediðimiz padding ve y-space'leri yüksekliðe bölüp optimum satýr sayýsýný buluyoruz
        rowCounter = Mathf.CeilToInt((height - (gridLayoutPadding.top + gridLayoutPadding.bottom)) / (curButtonSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y));

        // Satýr sayýsýný boþluk kalma ihtimaline karþý 2 arttýrýyoruz
        rowCounter = rowCounter + 2;
    }
    void updateCells()
    {
        //Belirlediðimiz hücre boyutlarýný componente uyguluyoruz
        scrollRect.content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(curButtonSize.x, curButtonSize.y);
    }
    void addButtons()
    {
        rectTransforms = new RectTransform[rowCounter, 2];

        //Bulduðumuz satýr sayýsý adedince 2 sütun olmak üzere butonlarýmýzý ekliyoruz
        //Bunu yaparken de daha sonra deðiþtirmek üzere butonlarýn rectTransformlarýný oluþturduðumuz diziye aktarýyoruz
        for (int i = 0; i < rowCounter; i++)
        {
            var button1Clone = Instantiate(button1, scrollRect.content);
            rectTransforms[i, 0] = button1Clone.GetComponent<RectTransform>();
            var button2Clone = Instantiate(button2, scrollRect.content);
            rectTransforms[i, 1] = button2Clone.GetComponent<RectTransform>();
        }
    }
    public void OnScroll(Vector2 pos)
    {
        if (!componentBool)
        {
            componentBool = true;
            
            //Ýki satýr arasý boþluðu hesaplýyoruz
            offsetY = rectTransforms[0, 0].position.y - rectTransforms[1, 0].position.y;
            
            //Bütün satýrlarýn Y düzlemindeki orjin noktasýný buluyoruz
            marginY = offsetY * rowCounter / 2;
            scrollRect.content.GetComponent<ContentSizeFitter>().enabled = false;
            scrollRect.content.GetComponent<GridLayoutGroup>().enabled = false;
        }
        for (int i = 0; i < rowCounter; i++)
        {
            /*  Satýrdaki ilk objenin parent objesine göreli uzaklýðý, bulduðumuz Y eksen orjininden büyükse
            satýrdaki iki objeyi de en alta taþýyoruz */
            if (scrollRect.transform.InverseTransformPoint(rectTransforms[i, 0].gameObject.transform.position).y > marginY)
            {
                for (int j = 0; j < 2; j++)
                {
                    rectTransforms[i, j].position = new Vector2(rectTransforms[i, j].position.x, rectTransforms[i, j].position.y - (marginY * 2));
                    scrollRect.content.GetChild(rowCounter * 2 - 1).transform.SetAsFirstSibling();
                }
            }
            /*  Satýrdaki ilk objenin parent objesine göreli uzaklýðý, bulduðumuz Y eksen orjininden küçükse
            satýrdaki iki objeyi de en yukarý taþýyoruz */

            else if (scrollRect.transform.InverseTransformPoint(rectTransforms[i, 0].gameObject.transform.position).y < -marginY)
            {
                for (int j = 0; j < 2; j++)
                {
                    rectTransforms[i, j].position = new Vector2(rectTransforms[i, j].position.x, rectTransforms[i, j].position.y + (marginY * 2));
                    scrollRect.content.GetChild(0).transform.SetAsLastSibling();
                }
            }

        }
    }
}
