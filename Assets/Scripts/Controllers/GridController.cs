using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    /*                                       AÇIKLAMA
     *     Verilen grid boyutlarýna göre _tilePrefab objemizi klonlayarak gridimizi oluþturuyoruz.   
     */

    public Node[,] grid;
    public int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;
    [SerializeField] private GameObject tileClones;
    
    void Start()
    {
        grid = new Node[_height, _width];
        GenerateGrid();
    }
    void GenerateGrid()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                var tileClone = Instantiate(_tilePrefab, new Vector3(i, j, 0), Quaternion.identity);
                tileClone.name = $"tileClone{i}{j}";

                //Klonlarý tileClone ismindeki game objesinin içerisine atýyoruz

                tileClone.transform.SetParent(tileClones.transform);

                //Klonlarýn index bilgilerini içlerine atýyoruz

                tileClone.index[0, 0] = i;
                tileClone.index[0, 1] = j;
                grid[i, j] = new Node(true, tileClone.transform.position, i, j);
                var isOffset = (i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0);
                tileClone.init(isOffset);

            }
        }
        _cam.transform.position = new Vector3((float)_width / 2 - 0.5f, (float)_height / 2 - 0.5f, -10f);
    }
    public Node NodeFromPosition(Vector3 position)
    {
        //Pozisyon bilgisini node indexi olarak dönderiyoruz

        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);

        return grid[x, y];
    }
    public List<Node> GetNeighbors(Node node)
    {
        //Node bilgisini komþe node'lar olarak dönderiyoruz;

        List<Node> neighbors = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0) { continue; }

                int checkX = node.gridX + i;
                int checkY = node.gridY + j;

                if (checkX >= 0 && checkX < _width && checkY >= 0 && checkY < _height)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

}
