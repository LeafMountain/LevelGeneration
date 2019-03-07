using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lite between two points
[System.Serializable]
public struct Edge
{
    public Vector3 begin, end;
    public int indexFirst, indexSecond;

    public Edge(int indexFirst, Vector3 first, int indexSecond, Vector3 second)
    {
        // Make a better comparison
        if(first.x < second.x || first.x == second.x && (first.y < second.y || (first.y == second.y && (first.z <= second.z))) )
        {
            this.begin = first;
            this.end = second;
            this.indexFirst = indexFirst;
            this.indexSecond = indexSecond;
        }
        // Begin is to the right of End
        else
        {
            this.begin = second;
            this.end = first;
            this.indexFirst = indexSecond;
            this.indexSecond = indexFirst;
        }
    }

    public bool Equal(Edge other)
    {
        return begin == other.begin && end == other.end;
    }
}

public class ShowEdges : MonoBehaviour
{
    public Mesh mesh;
    public List<Edge> edges;
    public List<int> edgeVerticies;

    void Start()
    {
        GetEdges();
    }

    int[] GetEdges()
    {
        int[] triangles = mesh.GetTriangles(0);
        List<Vector3> verticies = new List<Vector3>();
        mesh.GetVertices(verticies);

        List<Edge> edges = new List<Edge>();

        // Loop through the triangles and add each edge
        for(int i = 0; i < triangles.Length; i += 3)
        {
            int firstVert = triangles[i];
            int secondVert = triangles[i + 1];
            int thirdVert = triangles[i + 2];

            edges.Add(new Edge(firstVert, verticies[firstVert], secondVert, verticies[secondVert]));
            edges.Add(new Edge(secondVert, verticies[secondVert], thirdVert, verticies[thirdVert]));
            edges.Add(new Edge(thirdVert, verticies[thirdVert], firstVert, verticies[firstVert]));
        }

        // Remove all the duplicates
        for (int i = edges.Count - 1; i >= 0; i--)
        {
            for (int j = edges.Count - 1; j >= 0; j--)
            {
                if(j == i)
                    continue;

                if(edges[j].Equal(edges[i]))
                {
                    edges.RemoveAt(i);
                    edges.RemoveAt(j);
                    i -= 1;
                    break;
                }
            }
        }

        // Sort
        for (int i = 0; i < edges.Count; i++)
        {
            // Find the edge with the same begin as this end
            for (int j = 0; j < edges.Count; j++)
            {
                if(edges[j].begin == edges[i].end)
                {
                    Edge temp = edges[j];
                    edges.RemoveAt(j);
                    edges.Insert(i + 1, temp);
                }
            }
        }

        // Make it more like the triangle list (vertex index)
        List<int> edgeVerticies = new List<int>();
        for (int i = 0; i < edges.Count; i++)
        {
            edgeVerticies.Add(edges[i].indexFirst);
            edgeVerticies.Add(edges[i].indexSecond);
        }

        // Remove duplicates
        // for (int i = edgeVerticies.Count - 1; i >= 1; i -= 2)
        // {
        //     Debug.Log("Index: " + i + " Count: " + edgeVerticies.Count);
        //     edgeVerticies.RemoveAt(i - 1);
        //     Debug.Log("aa Index: " + i + " Count: " + edgeVerticies.Count);
        // }

        this.edges = edges;
        this.edgeVerticies = edgeVerticies;

        return edgeVerticies.ToArray();
    }

    void OnDrawGizmos()
    {
        if(mesh)
        {
            List<Vector3> verticies = new List<Vector3>();
            mesh.GetVertices(verticies);
            int[] edges = GetEdges();

            for (int i = 0; i < edges.Length; i += 2)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawLine(verticies[edges[i]], verticies[edges[i + 1]]);
                Gizmos.color = Color.red;
                
                // For debug purposes
                float radius = Random.Range(.1f, .3f);
                Gizmos.DrawSphere(verticies[edges[i]], radius);
                Gizmos.DrawSphere(verticies[edges[i + 1]], radius);
            }
        }
    }
}
