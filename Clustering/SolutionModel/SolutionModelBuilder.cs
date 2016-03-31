using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel.Nodes;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace Clustering.SolutionModel
{
    public class SolutionModelBuilder
    {
        private SolutionModelBuilder(string path)
        {
            _path = path;
            _solution = GetRoslynSolutionFromPath(path);
            _name = Path.GetFileName(path);
        }

        private readonly Solution _solution;
        private readonly string _name;
        private readonly string _path;

        private static Solution GetRoslynSolutionFromPath(string path) =>
            MSBuildWorkspace.Create().OpenSolutionAsync(path).Result;

        public static SolutionNode FromPath(string path) => 
            new SolutionModelBuilder(path).BuildSolutionModel();

        private SolutionNode BuildSolutionModel() =>
            new SolutionNode(_name)
            .WithChildren(GetProjectTreeFromPath(_path))
            as SolutionNode;
        
        private IEnumerable<Node> GetProjectTreeFromPath(string path)
        {
            path = Path.GetFullPath(path);
            var solFile = SolutionFile.Parse(path);

            var projects = solFile.ProjectsInOrder.ToList();

            var topProjects = projects.Where(x => x.ParentProjectGuid == null).ToList();

            // Retrieve top level projects / folders
            return topProjects.Select(x => GetProjectNode(x, projects));
        }

        private Node GetProjectNode(ProjectInSolution project, IReadOnlyCollection<ProjectInSolution> possibleChildren)
        {
            var children = possibleChildren.Where(project.IsParentOf);
            var childProjects = children.Select(x => GetProjectNode(x, possibleChildren)).ToList();
            if (childProjects.Any())
                return new SolutionFolderNode(project.ProjectName).WithChildren(childProjects);
            //(childProjects.Empty()) 
                var projectNode = ProjectNode.FromMsBuildProject(project);
                return projectNode.WithChildren(GetSymbolTreeForProject(projectNode));
        }

        private IEnumerable<Node> GetSymbolTreeForProject(ProjectNode projectNode)
        {
            if (projectNode.ProjectProperties.Id == null)
                return new List<Node>();
            //var id = ProjectId.CreateFromSerialized(projectNode.ProjectProperties.Id.Value);
            var proj = _solution.Projects.First(x => x.FilePath == projectNode.ProjectProperties.Path);
            var semanticModels = proj.Documents.Select(d => d.GetSemanticModelAsync().Result).ToList();
            return SemanticModelWalker.GetClassTree(semanticModels).ToList();
        }
    }
    
    public static class ProjectExtensions
    {
        public static bool IsParentOf(this ProjectInSolution parent, ProjectInSolution child) =>
            child.ParentProjectGuid == parent.ProjectGuid;
    }
}
