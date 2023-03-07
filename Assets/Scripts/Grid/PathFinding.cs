using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{


    /*                                                         AÇIKLAMA
     *          Gridimiz node'lardan oluþuyor ve bu nodelar en kýsa yolu bulmak için temel 3 parametre barýndýrýyor:
     *          1-) gCost = Mevcut node'dan komþu node'a uzaklýðý
     *          2-) hCost = Komþu node'un hedef node'a olan uzaklýðý
     *          3-) fCost = gCost + hcost
     */

    GridController grid;
    public GameObject activeSoldier;

    private void Awake()
    {
        grid = GetComponent<GridController>();
    }
    public void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Aldýðýmýz Vektörler ile target ve start node'larý buluyoruz.

        Node startNode = grid.NodeFromPosition(startPos);
        Node targetNode = grid.NodeFromPosition(targetPos);

        //openSet kontrol edilecek komþu nodelar, closedSet kontrol edilmiþ komþu nodelar listesi olarak kullanýlacak

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //openSet listesine ilk node olarak balangýç node'unu ekliyoruz

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //targetNode'a ulaþtýðýnda yolu oluþturmak içim RetracePath fonksiyonunu çaðýrýyoruz

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //Komþu node'larý saptadýktan sonra aralarýnda en düþük gCost deðerine sahip olaný seçip Parent deðerine mevcut node'u atýyoruz

            foreach (Node neighboor in grid.GetNeighbors(currentNode))
            {
                if (!neighboor.walkable || closedSet.Contains(neighboor))
                {
                    continue;
                }

                int newMovementCostToNeighbour = GetDistance(currentNode, neighboor);

                if (newMovementCostToNeighbour < neighboor.gCost || !openSet.Contains(neighboor))
                {
                    neighboor.gCost = newMovementCostToNeighbour;
                    neighboor.hCost = GetDistance(neighboor, targetNode);
                    neighboor.parent = currentNode;
                    if (!openSet.Contains(neighboor))
                    {
                        openSet.Add(neighboor);
                    }
                }
            }
        }
    }
    void RetracePath(Node startNode, Node endNode)
    {
        //Yol belirlendikten sonra activeSoldier objesine yol bilgilerini gönderip harekete geçmesi için walk bool'u true yapýyoruz

        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        activeSoldier.GetComponent<SoldierScript>().path = path;
        activeSoldier.GetComponent<SoldierScript>().walk = true;
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + (10 * (dstX - dstY));
        }
        return 14 * dstX + (10 * (dstY - dstX));
    }
}
