using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollPanel : MonoBehaviour
{
    /*                                                     A�IKLAMA
     *       
     *       Uygulamaya GridLayoutGroup ve ContentSizeFitter componentleri ile default de�erleri belirleyerek ba�l�yoruz b�ylelikle 
     *       content elemanlar�m�z belirledi�imiz d�zende ba�l�yor. Ancak scroll ba�lad���nda bu componentleri pasif hale getiriyoruz
     *       ��nk� GridLayoutGroup alt objelerin rectTransformuna eri�ip d�zenlemeye engel oluyor ve boyutland�rmay� kendimiz yapt���m�z 
     *       i�in ContentSizeFitter'a ihtiyac�m�z kalm�yor.
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
        //Ekran geni�li�ine g�re ihtiyac�m�z olan kare buton boyutlar�n� belirliyoruz
        curButtonSize.x = (rectTransform.rect.width - gridLayoutPadding.left - gridLayoutPadding.right - scrollRect.content.GetComponent<GridLayoutGroup>().spacing.x) / 2;
        curButtonSize.y = curButtonSize.x;
    }
    void updateCellCount()
    {
        // GridLayoutGroup'ta belirledi�imiz padding ve y-space'leri y�ksekli�e b�l�p optimum sat�r say�s�n� buluyoruz
        rowCounter = Mathf.CeilToInt((height - (gridLayoutPadding.top + gridLayoutPadding.bottom)) / (curButtonSize.y + scrollRect.content.GetComponent<GridLayoutGroup>().spacing.y));

        // Sat�r say�s�n� bo�luk kalma ihtimaline kar�� 2 artt�r�yoruz
        rowCounter = rowCounter + 2;
    }
    void updateCells()
    {
        //Belirledi�imiz h�cre boyutlar�n� componente uyguluyoruz
        scrollRect.content.GetComponent<GridLayoutGroup>().cellSize = new Vector2(curButtonSize.x, curButtonSize.y);
    }
    void addButtons()
    {
        rectTransforms = new RectTransform[rowCounter, 2];

        //Buldu�umuz sat�r say�s� adedince 2 s�tun olmak �zere butonlar�m�z� ekliyoruz
        //Bunu yaparken de daha sonra de�i�tirmek �zere butonlar�n rectTransformlar�n� olu�turdu�umuz diziye aktar�yoruz
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
            
            //�ki sat�r aras� bo�lu�u hesapl�yoruz
            offsetY = rectTransforms[0, 0].position.y - rectTransforms[1, 0].position.y;
            
            //B�t�n sat�rlar�n Y d�zlemindeki orjin noktas�n� buluyoruz
            marginY = offsetY * rowCounter / 2;
            scrollRect.content.GetComponent<ContentSizeFitter>().enabled = false;
            scrollRect.content.GetComponent<GridLayoutGroup>().enabled = false;
        }
        for (int i = 0; i < rowCounter; i++)
        {
            /*  Sat�rdaki ilk objenin parent objesine g�reli uzakl���, buldu�umuz Y eksen orjininden b�y�kse
            sat�rdaki iki objeyi de en alta ta��yoruz */
            if (scrollRect.transform.InverseTransformPoint(rectTransforms[i, 0].gameObject.transform.position).y > marginY)
            {
                for (int j = 0; j < 2; j++)
                {
                    rectTransforms[i, j].position = new Vector2(rectTransforms[i, j].position.x, rectTransforms[i, j].position.y - (marginY * 2));
                    scrollRect.content.GetChild(rowCounter * 2 - 1).transform.SetAsFirstSibling();
                }
            }
            /*  Sat�rdaki ilk objenin parent objesine g�reli uzakl���, buldu�umuz Y eksen orjininden k���kse
            sat�rdaki iki objeyi de en yukar� ta��yoruz */

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
