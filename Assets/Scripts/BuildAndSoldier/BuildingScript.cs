using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingScript : MonoBehaviour
{
    /*                                AÇIKLAMA
     *      -collidedSoldiers ve collidedBuildings listeleri boþ ise yapý inþaa edilebilir
     *      -Yapý inþaa edilirse dropBool true olur
     */

    [SerializeField] private string buildingType;
    [SerializeField] private int hp;
    [SerializeField] private GameObject canvas;
    [SerializeField] private Image healthBar;
    public List<Tile> collidedTiles = new List<Tile>();
    private int startHp;
    private bool dropBool;
    private bool clickableBool;
    private GameObject tileClones;
    private List<GameObject> collidedSoldiers = new List<GameObject>();
    private List<GameObject> collidedBuildings = new List<GameObject>();
    private GridController gridController;
    private PathFinding pathFinding;
    private GameObject informationPanelBarracks;
    private GameObject informationPanelPower;
    private GameObject buildingClones;
    void Start()
    {
        startHp = hp;
        clickableBool = true;
        pathFinding = GameObject.Find("GridController").gameObject.GetComponent<PathFinding>();
        tileClones = GameObject.Find("TileClones");
        informationPanelPower = GameObject.Find("InformationPanelPowerPlant").gameObject;
        informationPanelBarracks = GameObject.Find("InformationPanelBarracks").gameObject;
        gridController = GameObject.Find("GridController").GetComponent<GridController>();
        buildingClones = GameObject.Find("BuildingClones").gameObject;
    }
    void Update()
    {
        healthBar.fillAmount = (float)hp / startHp;
        if (!dropBool)
        {
            for (int i = 0; i < tileClones.transform.childCount; i++)
            {
                if (tileClones.transform.GetChild(i).GetComponent<Tile>().activeTile)
                {
                    transform.position = tileClones.transform.GetChild(i).transform.position;
                }
            }
        }

        //Yapý inþaa edilmemiþse direkt olarak yok ediyoruz
        if (Input.GetMouseButtonDown(1) && !dropBool)
        {
            Destroy(gameObject);
        }
        //Yapý inþaa edildiyse DestroyGameObject fonksiyonu ile önce iþgal ettiði nodelarý tekrar aktif hale getiriyoruz
        if (hp <= 0)
        {
            DestroyGameObject();
        }
    }
    void DestroyGameObject()
    {
        for (int i = 0; i < collidedTiles.Count; i++)
        {
            collidedTiles[i].buildableBool = true;
            collidedTiles[i].walkableBool = true;
            gridController.grid[collidedTiles[i].index[0, 0], collidedTiles[i].index[0, 1]].walkable = true;
        }
        informationPanelBarracks.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
        informationPanelPower.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
        Destroy(gameObject);
    }
    private void OnMouseOver()
    {
        //Yapý üzerine sað sýklandýðýnda activeSoldier null deðilse atak yapmasý için FindPath fonksiyonuna gerekli bilgileri gönderiyoruz
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(1))
        {
            GameObject activeSoldier = GameObject.Find("UIController").gameObject.GetComponent<UIController>().activeSoldier;
            if (activeSoldier != null)
            {
                activeSoldier.GetComponent<SoldierScript>().targetPosForAttack = transform.position;
                activeSoldier.GetComponent<SoldierScript>().attack = true;
                activeSoldier.GetComponent<SoldierScript>().targetGameObject = gameObject;
                pathFinding.activeSoldier = activeSoldier;
                pathFinding.FindPath(activeSoldier.transform.position, GetTargetPosition());
            }
        }
    }
    Vector3 GetTargetPosition()
    {
        //Hedefe minimum 2f uzaklýkta olan nodelar içerisinden en yakýn ve boþ olan node'u buluyoruz
        Vector3 targetPos = tileClones.transform.GetChild(0).transform.position;

        for (int i = 0; i < tileClones.transform.childCount; i++)
        {
            if (tileClones.transform.GetChild(i).GetComponent<Tile>().buildableBool && Vector2.Distance(transform.position, tileClones.transform.GetChild(i).transform.position) >= 2f && Vector2.Distance(transform.position, tileClones.transform.GetChild(i).transform.position) < Vector2.Distance(transform.position, targetPos))
            {
                targetPos = tileClones.transform.GetChild(i).gameObject.transform.position;
            }
        }
        return targetPos;
    }
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (clickableBool)
        {
            dropBool = true;
            canvas.SetActive(true);
            transform.SetParent(buildingClones.transform);
        }
        if (buildingType == "power")
        {

            for (int i = 0; i < collidedTiles.Count; i++)
            {
                collidedTiles[i].buildableBool = false;
                collidedTiles[i].walkableBool = false;
                gridController.grid[collidedTiles[i].index[0, 0], collidedTiles[i].index[0, 1]].walkable = false;
            }

            informationPanelBarracks.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            informationPanelPower.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("InformationPanelSoldier").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            informationPanelPower.transform.Find("HealthText").GetComponent<Text>().text = "Hp: %" + (float)hp / startHp * 100f;
            GameObject.Find("UIController").GetComponent<UIController>().activeBuilding = gameObject;
            GameObject.Find("SelectedPoint").gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("SelectedPoint").gameObject.transform.position = gameObject.transform.position;
        }
        if (buildingType == "barracks")
        {
            for (int i = 0; i < collidedTiles.Count; i++)
            {
                collidedTiles[i].buildableBool = false;
                collidedTiles[i].walkableBool = false;
                gridController.grid[collidedTiles[i].index[0, 0], collidedTiles[i].index[0, 1]].walkable = false;
            }
            informationPanelPower.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            informationPanelBarracks.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("InformationPanelSoldier").gameObject.GetComponent<RectTransform>().localScale = new Vector3(0, 1, 1);
            informationPanelBarracks.transform.Find("HealthText").GetComponent<Text>().text = "Hp: %" + (float)hp / startHp * 100f;
            GameObject.Find("UIController").GetComponent<UIController>().activeBuilding = gameObject;
            GameObject.Find("SelectedPoint").gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
            GameObject.Find("SelectedPoint").gameObject.transform.position = gameObject.transform.position;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.collider.tag == "Barracks" || collision.collider.tag == "PowerPlant") && !dropBool)
        {
            collidedBuildings.Add(collision.gameObject);
            clickableBool = false;
            transform.Find("Square").gameObject.SetActive(true);
        }
        if (collision.collider.tag == "Tile")
        {
            collidedTiles.Add(collision.gameObject.GetComponent<Tile>());
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.collider.tag == "Barracks" || collision.collider.tag == "PowerPlant") && !dropBool)
        {
            collidedBuildings.Remove(collision.gameObject);
        }
        if (collidedSoldiers.Count == 0 && collidedBuildings.Count == 0)
        {
            clickableBool = true;
            transform.Find("Square").gameObject.SetActive(false);
        }
        if (collision.collider.tag == "Tile")
        {
            collidedTiles.Remove(collision.gameObject.GetComponent<Tile>());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Soldier" && !dropBool)
        {
            collidedSoldiers.Add(collision.gameObject);
            clickableBool = false;
            transform.Find("Square").gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Soldier" && !dropBool)
        {
            collidedSoldiers.Remove(collision.gameObject);
        }
        if (collidedSoldiers.Count == 0 && collidedBuildings.Count == 0)
        {
            clickableBool = true;
            transform.Find("Square").gameObject.SetActive(false);
        }
    }
    private void OnParticleCollision(GameObject other)
    {
        hp -= 2;
    }
}
