using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraScript : MonoBehaviour
{
    private bool click;
    private Vector3 dragOrigin;
    [SerializeField]private Camera cam;
    [SerializeField] private GridController gridController;
    
    void Update()
    {
        MoveBack();
        MoveCamera();
    }
    void MoveCamera()
    {   
        //Ýlk click pozisyonu ile mevctu pointer pozisyonunun farkýný kamera pozisyouna ekliyoruz.

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButtonDown(0))
        {
            click = true;
            dragOrigin =cam.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && click)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            transform.position += difference;
        }
        if (Input.GetMouseButtonUp(0))
        {
            click = false;
        }
    }
    void MoveBack()
    {
        //Kamera pozisyonu oyun alaný dýþýna çýkarsa tekrar oyun alanýna çekiyoruz.

        if (transform.position.x < 16)
        {
            transform.position = new Vector3(16, transform.position.y, transform.position.z);
        }
        if (transform.position.y < 10)
        {
            transform.position = new Vector3(transform.position.x, 10, transform.position.z);
        }
        if (transform.position.x > gridController._width - 16)
        {
            transform.position = new Vector3(gridController._width - 16, transform.position.y, transform.position.z);
        }
        if (transform.position.y > gridController._height - 10)
        {
            transform.position = new Vector3(transform.position.x, gridController._width - 10, transform.position.z);
        }
    }
}
