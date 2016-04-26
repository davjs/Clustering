using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.MojoFM
{
    public class Vertex
    {
        public bool Matched { get; set; }
        public bool IsLeft { get; set; }
        public int Outdeg { get; set; }
        public int Indeg { get; set; }
    }

    public class BipartiteGraph
    {
        public List<List<int>> AdjList { get; }
        public Vertex[] Vertices { get; set; }
        private List<int> augmentPath;
        private int left;

        public BipartiteGraph(int total, int left)
        {
            this.left = left;

            AdjList = new List<List<int>>();
            Vertices = new Vertex[total];
            augmentPath = new List<int>();

            for (int i = 0; i < total; i++)
            {
                Vertices[i] = new Vertex();
                if (i < left) Vertices[i].IsLeft = true;
                AdjList.Insert(i, new List<int>());
            }
        }

        public void AddEdge(int start, int end)
        {
            AdjList[start].Add(end);
            Vertices[start].Outdeg++;
            Vertices[end].Indeg++;

            if (IsRight(start) && IsLeft(end))
            {
                Vertices[start].Matched = true;
                Vertices[end].Matched = true;
            }
        }

        public void RemoveEdge(int start, int end)
        {
            int index = AdjList[start].IndexOf(end);
            if (index > -1) AdjList[start].RemoveAt(index);
            Vertices[start].Outdeg--;
            Vertices[end].Indeg--;

            if (IsRight(start) && Vertices[start].Outdeg == 0) Vertices[start].Matched = false;

            if (IsLeft(end) && Vertices[end].Indeg == 0) Vertices[end].Matched = false;
        }

        private void ReverseEdge(int start, int end)
        {
            RemoveEdge(start, end);
            AddEdge(end, start);
        }

        private void XOR()
        {
            int start = augmentPath[0];
            for (int i = 1; i < augmentPath.Count; i++)
            {
                int end = augmentPath[i];
                ReverseEdge(start, end);
                start = end;
            }
        }

        public void Matching()
        {
            while (FindAugmentPath())
            {
                XOR();
            }
        }

        private bool FindAugmentPath()
        {
            augmentPath.Clear();
            for (int i = 0; i < left; i++)
            {
                if (!Vertices[i].Matched)
                {
                    if (FindPath(i)) return true;
                    augmentPath.Clear();
                }
            }
            return false;
        }

        private bool FindPath(int start)
        {
            if (Vertices[start].Outdeg == 0) return false;
            augmentPath.Add(start);

            for (int i = 0; i < AdjList[start].Count; i++)
            {
                int next = AdjList[start][i];
                if(augmentPath.IndexOf(next) > -1) continue;
                if (!Vertices[next].Matched)
                {
                    augmentPath.Add(next);
                    return true;
                }
                if (FindPath(next)) return true;
            }
            augmentPath.RemoveAt(augmentPath.IndexOf(start));
            return false;
        }

        private bool IsLeft(int point)
        {
            if (point < left) return true;
            return false;
        }

        private bool IsRight(int point)
        {
            if (point > left - 1) return true;
            return false;
        }
    }
}
