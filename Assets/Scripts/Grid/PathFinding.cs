using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{


    /*                                                         A�IKLAMA
     *          Gridimiz node'lardan olu�uyor ve bu nodelar en k�sa yolu bulmak i�in temel 3 parametre bar�nd�r�yor:
     *          1-) gCost = Mevcut node'dan kom�u node'a uzakl���
     *          2-) hCost = Kom�u node'un hedef node'a olan uzakl���
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
        //Ald���m�z Vekt�rler ile target ve start node'lar� buluyoruz.

        Node startNode = grid.NodeFromPosition(startPos);
        Node targetNode = grid.NodeFromPosition(targetPos);

        //openSet kontrol edilecek kom�u nodelar, closedSet kontrol edilmi� kom�u nodelar listesi olarak kullan�lacak

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        //openSet listesine ilk node olarak balang�� node'unu ekliyoruz

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

            //targetNode'a ula�t���nda yolu olu�turmak i�im RetracePath fonksiyonunu �a��r�yoruz

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //Kom�u node'lar� saptad�ktan sonra aralar�nda en d���k gCost de�erine sahip olan� se�ip Parent de�erine mevcut node'u at�yoruz

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
        //Yol belirlendikten sonra activeSoldier objesine yol bilgilerini g�nderip harekete ge�mesi i�in walk bool'u true yap�yoruz

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
