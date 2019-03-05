using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Lite between two points
[System.Serializable]
public struct Edge
{
    public Vector3 begin, end;

    public Edge(Vector3 first, Vector3 second)
    {
        // Make a better comparison
        if(first.x < second.x || first.x == second.x && (first.y < second.y || (first.y == second.y && (first.z <= second.z))) )
        {
            this.begin = first;
            this.end = second;
        }
        // Begin is to the right of End
        else
        {
            this.begin = second;
            this.end = first;
        }
    }

    public bool Equal(Edge other)
    {
        return true;
        return begin == other.begin && end == other.end;
    }
}

public class ShowEdges : MonoBehaviour
{
    public Mesh mesh;
    public List<Edge> edges;

    void Start()
    {
        GetEdges();
    }

    Vector3[] GetVertexPositions()
    {
        List<Vector3> verticies = new List<Vector3>();

        mesh.GetVertices(verticies);

        return verticies.ToArray();
    }

    List<Edge> GetEdges()
    {
        int[] triangles = mesh.GetTriangles(0);
        List<Vector3> verticies = new List<Vector3>();
        mesh.GetVertices(verticies);

        List<Edge> edges = new List<Edge>();

        // Loop through the triangles
        for(int i = 0; i < triangles.Length; i += 3)
        {
            int firstVert = triangles[i];
            int secondVert = triangles[i + 1];
            int thirdVert = triangles[i + 2];

            edges.Add(new Edge(verticies[firstVert], verticies[secondVert]));
            edges.Add(new Edge(verticies[secondVert], verticies[thirdVert]));
            edges.Add(new Edge(verticies[thirdVert], verticies[firstVert]));
        }

        for (int i = edges.Count - 1; i > 0; i--)
        {
            // Check if the same edge exists
            for (int j = edges.Count - 1; j > 0; j--)
            {
                if(j == i)
                    continue;

                if(edges[j].Equal(edges[i]))
                {
                    if(i > j)
                    {
                        edges.RemoveAt(i);
                        edges.RemoveAt(j);
                    }
                    else
                    {
                        edges.RemoveAt(j);
                        edges.RemoveAt(i);
                    }

                    i -= 1;
                    break;
                }
            }
        }

        this.edges = edges;

        return edges;
    }

    void OnDrawGizmos()
    {
        if(mesh)
        {
            List<Edge> edges = GetEdges();
            for (int i = 0; i < edges.Count; i++)
            {
                Gizmos.DrawLine(edges[i].begin, edges[i].end);
            }
        }
    }
}
