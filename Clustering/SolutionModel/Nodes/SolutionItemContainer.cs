using System.Collections.Generic;

namespace Clustering.SolutionModel.Nodes
{
    //Abstract class inhereted by both SolutionFolder and Solution, they can both contain solution items
    public abstract class SolutionItemContainer : Node
    {
        //Gives a flat list of all projects found in the solution
        public IEnumerable<ProjectNode> Projects()
        {
            foreach (var node in Children)
            {
                if (node is ProjectNode)
                {
                    yield return node as ProjectNode;
                }
                else if (node is SolutionFolderNode)
                {
                    foreach (var projNode in (node as SolutionFolderNode).Projects())
                    {
                        yield return projNode;
                    }
                }
            }
        }

        public SolutionItemContainer(string name, IEnumerable<Node> children, Node parent) : base(name, children, parent)
        {
        }
    }

    //Abstract class inhereted by both Namespace and Projec, they can both contain solution items
    /*public abstract class ClassContainer : Node
    {
        //Gives a flat list of all projects found in the solution
        public IEnumerable<ClassNode> Classes()
        {
            foreach (var node in Children)
            {
                if (node is ClassNode)
                {
                    yield return node as ClassNode;
                }
                else if (node is ClassContainer)
                {
                    foreach (var classNode in (node as ClassContainer).Classes())
                    {
                        yield return classNode;
                    }
                }
            }
        }

        public ClassContainer(string name, IEnumerable<Node> children, Node parent) : base(name, children, parent)
        {
        }
    }*/
}