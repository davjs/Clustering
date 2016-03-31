using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Integration;
using Microsoft.Build.Construction;

namespace Clustering.SolutionModel.Nodes
{
    public class ProjectNode : Node
    {
        public readonly ProjectWrapper ProjectProperties;

        public ProjectNode(ProjectWrapper wrapper,IEnumerable<Node> children = null, Node parent = null) : base(wrapper.Name,children,parent)
        {
            ProjectProperties = wrapper;
        }
        
        public static ProjectNode FromMsBuildProject(ProjectInSolution proj) 
            => new ProjectNode(new ProjectWrapper(proj));

        public override Node WithChildren(IEnumerable<Node> children)
            => new ProjectNode(ProjectProperties, children);
        

        public override Node WithParent(Node parent) 
            => new ProjectNode(ProjectProperties, Children, parent);

        public IEnumerable<ClassNode> Classes()
            => Children.Cast<NameSpaceNode>().SelectMany(x => x.Classes());
    }
}
