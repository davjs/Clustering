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

        // IF EMPTY: http://stackoverflow.com/questions/25070323/roslyn-workspace-opensolutionasync-projects-always-empty
        private static Solution GetRoslynSolutionFromPath(string path)
        {
            var openSolutionAsync = MSBuildWorkspace.Create().OpenSolutionAsync(path);
            openSolutionAsync.Wait();
            var roslynSolutionFromPath = openSolutionAsync.Result;
            return roslynSolutionFromPath;
        }

        public static SolutionNode FromPath(string path) => 
            new SolutionModelBuilder(path).BuildSolutionModel();

        private SolutionNode BuildSolutionModel() =>
            new SolutionNode(_name)
            .WithChildren(GetProjectTreeFromPath(_path))
            as SolutionNode;
        
        private IEnumerable<Node> GetProjectTreeFromPath(string path)
        {
            path = Path.GetFullPath(path);
            if(!_solution.Projects.Any())
                throw new Exception("Unable to access projects");

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
            var proj = _solution.Projects.FirstOrDefault(x => x.FilePath == projectNode.ProjectProperties.Path);
            if(proj == null)
                return new List<Node>();
            var semanticModels = proj.Documents.Select(d => d.GetSemanticModelAsync().Result).ToList();
            return SemanticModelWalker.GetSymbolTreeFromModels(semanticModels).ToList();
        }
    }
    
    public static class ProjectExtensions
    {
        public static bool IsParentOf(this ProjectInSolution parent, ProjectInSolution child) =>
            child.ParentProjectGuid == parent.ProjectGuid;
    }
}
