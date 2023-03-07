using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject activeBuilding;
    public GameObject activeSoldier;
    [SerializeField] private RectTransform infoPanelPower;
    [SerializeField] private RectTransform infoPanelBarracks;
    [SerializeField] private RectTransform infoPanelSoldier;
    [SerializeField] private GameObject tileClones;
    [SerializeField] private GameObject soldier1;
    [SerializeField] private GameObject soldier2;
    [SerializeField] private GameObject soldier3;
    [SerializeField] private GameObject soldierClones;
    private GridController gridController;
    


    /*                             A�IKLAMA
     *   -Barracks game objesinin pivot noktas� sol alt k��esi oldu�u i�in
     *    SpawnPoint olarak objenin kendi pozisyonunu kullan�yoruz
     *   -activeBuilding objesi BuildingScript taraf�ndan atan�yor
    */

    void Start()
    {
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
    }
    //Yap�lar i�in Destroy butonu
    public void destroy()
    {
        if (activeBuilding != null)
        {
            for (int i = 0; i < activeBuilding.GetComponent<BuildingScript>().collidedTiles.Count; i++)
            {
                activeBuilding.GetComponent<BuildingScript>().collidedTiles[i].GetComponent<Tile>().buildableBool = true;
                activeBuilding.GetComponent<BuildingScript>().collidedTiles[i].GetComponent<Tile>().walkableBool = true;
                gridController.grid[activeBuilding.GetComponent<BuildingScript>().collidedTiles[i].GetComponent<Tile>().index[0, 0], activeBuilding.GetComponent<BuildingScript>().collidedTiles[i].GetComponent<Tile>().index[0, 1]].walkable = true;
            }
            Destroy(activeBuilding);
            infoPanelBarracks.localScale = new Vector3(0, 1, 1);
            infoPanelPower.localScale = new Vector3(0, 1, 1);
        }
    }
    //Askerler i�in Destroy butonu
    public void destroySoldier()
    {
        if (activeSoldier != null)
        {
            activeSoldier.GetComponent<SoldierScript>().currentTile.GetComponent<Tile>().buildableBool = true;
            Destroy(activeSoldier);
            infoPanelSoldier.localScale = new Vector3(0, 1, 1);
        }
    }
    public void generateSoldier1()
    {
        GameObject activeTile = tileClones.transform.GetChild(0).gameObject;
        if (activeBuilding != null)
        {
            for (int i = 0; i < tileClones.transform.childCount; i++)
            {
                if (tileClones.transform.GetChild(i).GetComponent<Tile>().buildableBool && Vector2.Distance(activeTile.transform.position, activeBuilding.transform.position) > Vector2.Distance(tileClones.transform.GetChild(i).transform.position, activeBuilding.transform.position))
                {
                    activeTile = tileClones.transform.GetChild(i).gameObject;
                }
            }
            if (activeTile != null)
            {
                var soldier1Clone = Instantiate(soldier1, activeTile.transform.position, Quaternion.identity);
                soldier1Clone.GetComponent<SoldierScript>().currentTile = activeTile;
                soldier1Clone.transform.SetParent(soldierClones.transform);
                activeTile.GetComponent<Tile>().buildableBool = false;
            }
        }
    }
    public void generateSoldier2()
    {
        GameObject activeTile = tileClones.transform.GetChild(0).gameObject;
        if (activeBuilding != null)
        {
            for (int i = 0; i < tileClones.transform.childCount; i++)
            {
                if (tileClones.transform.GetChild(i).GetComponent<Tile>().buildableBool && Vector2.Distance(activeTile.transform.position, activeBuilding.transform.position) > Vector2.Distance(tileClones.transform.GetChild(i).transform.position, activeBuilding.transform.position))
                {
                    activeTile = tileClones.transform.GetChild(i).gameObject;
                }
            }
            if (activeTile != null)
            {
                var soldier2Clone = Instantiate(soldier2, activeTile.transform.position, Quaternion.identity);
                soldier2Clone.GetComponent<SoldierScript>().currentTile = activeTile;
                soldier2Clone.transform.SetParent(soldierClones.transform);
                activeTile.GetComponent<Tile>().buildableBool = false;
            }
        }
    }
    public void generateSoldier3()
    {
        GameObject activeTile = tileClones.transform.GetChild(0).gameObject;
        if (activeBuilding != null)
        {
            for (int i = 0; i < tileClones.transform.childCount; i++)
            {
                if (tileClones.transform.GetChild(i).GetComponent<Tile>().buildableBool && Vector2.Distance(activeTile.transform.position, activeBuilding.transform.position) > Vector2.Distance(tileClones.transform.GetChild(i).transform.position, activeBuilding.transform.position))
                {
                    activeTile = tileClones.transform.GetChild(i).gameObject;
                }
            }
            if (activeTile != null)
            {
                var soldier3Clone = Instantiate(soldier3, activeTile.transform.position, Quaternion.identity);
                soldier3Clone.GetComponent<SoldierScript>().currentTile = activeTile;
                soldier3Clone.transform.SetParent(soldierClones.transform);
                activeTile.GetComponent<Tile>().buildableBool = false;
            }
        }
    }
}
