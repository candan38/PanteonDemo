using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool activeTile;
    public bool buildableBool;
    public bool walkableBool;
    [SerializeField] private Color color1;
    [SerializeField] private Color color2;
    [SerializeField] private SpriteRenderer _sprt;
    [SerializeField] private PathFinding pathFinding;
    
    public int[,] index = new int[1, 2];

    public List<GameObject> neighborTiles = new List<GameObject>();
    public void init(bool _isOffset)
    {
        _sprt.color = _isOffset ? color1 : color2;
    }
   
    private void Start()
    {
        pathFinding = GameObject.Find("GridController").gameObject.GetComponent<PathFinding>();
        buildableBool = true;
        walkableBool = true;
    }
    private void OnMouseEnter()
    {
        activeTile = true;
    }

    private void OnMouseExit()
    {
        activeTile = false;
    }

    //      Tile üzerine sað týklandýðýnda activeSoldier null deðilse
    //  harekete geçmesi için pathFinding fonksiyonuna poziyon bilgilerini göderiyoruz
    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(1))
        {
            GameObject activeSoldier= GameObject.Find("UIController").gameObject.GetComponent<UIController>().activeSoldier;
            if (activeSoldier != null)
            {
                pathFinding.activeSoldier = activeSoldier;
                pathFinding.FindPath(activeSoldier.transform.position, transform.position);
            }
        }
    }
   


}
