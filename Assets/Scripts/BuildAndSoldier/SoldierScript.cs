using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoldierScript : MonoBehaviour
{   
    public List<Node> path = new List<Node>();
    public bool walk;
    public bool attack;
    public Vector3 targetPosForAttack;
    public GameObject targetGameObject;
    public GameObject currentTile;
    public GameObject finalTile;

    [SerializeField] private int hp;
    [SerializeField] private int damage;
    [SerializeField] private UIController UIController;

    private ParticleSystem attackParticle;
    private ParticleSystem.EmissionModule attackParticleEmission;
    private ParticleSystem.ShapeModule attackParticleShapeModule;   
    private RectTransform informationPanelPowerPlant;
    private RectTransform informationPanelBarracks;
    private RectTransform informationPanelSoldier;
    private Text healthText;
    private Text damageText;
    private Text nameText;
    private Image Image;
    private Vector3 currentPos;
    private int currentNode;
    private PathFinding pathFinding;
    private GameObject tileClones;
    private Transform soldierSelectedPoint;

    void Start()
    {
        tileClones = GameObject.Find("TileClones");
        pathFinding = GameObject.Find("GridController").gameObject.GetComponent<PathFinding>();
        attackParticle = transform.Find("FireParticle").gameObject.GetComponent<ParticleSystem>();
        UIController = GameObject.Find("UIController").gameObject.GetComponent<UIController>();
        soldierSelectedPoint = GameObject.Find("SoldierSelectedPoint").gameObject.transform;
        informationPanelPowerPlant=GameObject.Find("InformationPanelPowerPlant").gameObject.GetComponent<RectTransform>();
        informationPanelBarracks=GameObject.Find("InformationPanelBarracks").gameObject.GetComponent<RectTransform>();
        informationPanelSoldier=GameObject.Find("InformationPanelSoldier").gameObject.GetComponent<RectTransform>();
        healthText=GameObject.Find("InformationPanelSoldier").gameObject.transform.Find("HealthText").GetComponent<Text>();
        damageText=GameObject.Find("InformationPanelSoldier").gameObject.transform.Find("DamageText").GetComponent<Text>();
        nameText=GameObject.Find("InformationPanelSoldier").gameObject.transform.Find("NameText").GetComponent<Text>();
        Image = GameObject.Find("InformationPanelSoldier").gameObject.transform.Find("Image").GetComponent<Image>();        
    }

    void Update()
    {
        // PathFinding/RetracePath fonksiyonunda walk true yap�ld���nda 

        if (walk)
        {
            currentPos = path[currentNode].worldPos;
            if (transform.position != currentPos)
            {
                transform.position = Vector3.Lerp(transform.position, currentPos, Time.deltaTime * 30f);
                soldierSelectedPoint.position = gameObject.transform.position;
            }
            else
            {
                if (currentNode < path.Count - 1)
                {
                    currentNode++;
                }
                else
                {
                    walk = false;
                    currentNode = 0;
                    if (attack)
                    {
                        //FireParticle'� ba�lat�yoruz
                        attack = false;
                        attackParticleShapeModule = attackParticle.shape;
                        attackParticleEmission = attackParticle.emission;

                        //Partical System Rate over time de�i�kenini damage ile orant�l� at�yoruz
                        attackParticleEmission.rateOverTime = damage / 2;
                        attackParticleEmission.enabled = true;

                        //Partical System y�n�n� hedefe �evirmek i�in GetDirectionForAttack fonksiyonunu �a��r�yoruz
                        attackParticleShapeModule.rotation = new Vector3(0, 0, GetDirectionForAttack());
                        attackParticle.Play();
                    }
                }
            }
        }
        if (targetGameObject == null)
        {
            //Hedef yok edildi�inde Partical System'i durduruyoruz
            attackParticleEmission = attackParticle.emission;
            attackParticleEmission.enabled = true;
            attackParticle.Stop();
        }
        if (hp <= 0)
        {
            DestroyGameObject();
        }
    }
    void DestroyGameObject()
    {
        //Objeyi yok etmeden �nce i�gal etti�i tile'� tekrar kullan�labilir yap�yoruz
        currentTile.GetComponent<Tile>().buildableBool = true;
        Destroy(gameObject);
    }
    float GetDirectionForAttack()
    {
        Vector3 dir = transform.InverseTransformPoint(targetPosForAttack);
        float a = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        return -a;
    }
    Vector3 GetTargetPosition()
    {
        //Hedefe minimum 1f yak�nl���ndaki nodelar aras�ndan en yak�n ve bo� olan� se�ip targetPos olarak d�nderiyoruz

        Vector3 targetPos = tileClones.transform.GetChild(0).transform.position;

        for (int i = 0; i < tileClones.transform.childCount; i++)
        {
            if (tileClones.transform.GetChild(i).GetComponent<Tile>().buildableBool && Vector2.Distance(transform.position, tileClones.transform.GetChild(i).transform.position) >= 1f && Vector2.Distance(transform.position, tileClones.transform.GetChild(i).transform.position) < Vector2.Distance(transform.position, targetPos))
            {
                targetPos = tileClones.transform.GetChild(i).gameObject.transform.position;
            }
        }

        return targetPos;
    }
    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(1))
        {
            //Obje �zerine sa� t�kland���nda activeSoldier objesi null de�ilse harekete ge�mesi i�in FindPath fonksiyonuna de�erleri g�nderiyoruz
            GameObject activeSoldier = GameObject.Find("UIController").gameObject.GetComponent<UIController>().activeSoldier;
            if (activeSoldier != null)
            {
                activeSoldier.GetComponent<SoldierScript>().targetPosForAttack = transform.position;
                activeSoldier.GetComponent<SoldierScript>().attack = true;
                activeSoldier.GetComponent<SoldierScript>().targetGameObject = gameObject;
                pathFinding.activeSoldier = activeSoldier;

                //activeSoldier objesinin hedefe atak yapaca�� optimum yeri saptamak i�in GetTargetPosition fonksiyonunu kullan�yoruz
                pathFinding.FindPath(activeSoldier.transform.position, GetTargetPosition());

                //Soldier objesi hedef haline geldi�inde BoxCollider2D componentindeki isTrigger �zelli�ini kapat�yoruz
                GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
    }   
    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        
        //T�kland���nda information paneli d�zenleyip UIController activeSoldier objesine atama yap�yoruz

        informationPanelPowerPlant.localScale = new Vector3(0, 1, 1);
        informationPanelBarracks.localScale = new Vector3(0, 1, 1);
        informationPanelSoldier.localScale = new Vector3(1, 1, 1);
        healthText.text = "Hp: " + hp;
        damageText.text = "Damage: " + damage;
        nameText.text = transform.name;
        Image.sprite = transform.Find("Image").GetComponent<SpriteRenderer>().sprite;
        soldierSelectedPoint.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        soldierSelectedPoint.position = gameObject.transform.position;
        UIController.activeSoldier = gameObject;
    }
    private void OnParticleCollision(GameObject other)
    {
        hp -= 2;
    }
}
