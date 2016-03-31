using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Construction;

namespace Clustering.SolutionModel.Integration
{
    // This wrapper is used in DevArch to support 3 different project API's.
    // Allthough we only need one here, keeping the wrapper increases DevArch compatability
    // -- and hiding the complexity of accessing projectItems doesnt hurt.
    public class ProjectWrapper
    {
        public readonly string Name;
        public readonly string Path;
        public Lazy<IEnumerable<string>> Items { get; }
        public readonly Guid? Id;
        public bool IsLoaded = true;
        public Guid? ParentGuid;
        //Constructor for MSBuild project
        public ProjectWrapper(ProjectInSolution project)
        {
            Name = project.ProjectName;
            Path = project.AbsolutePath;
            Items = new Lazy<IEnumerable<string>>
                (() => new Microsoft.Build.Evaluation.Project(project.AbsolutePath).Items.Select(x => x.EvaluatedInclude));
            Id = new Guid(project.ProjectGuid);
            if (project.ParentProjectGuid == null)
                ParentGuid = null;
            else
                ParentGuid = new Guid(project.ParentProjectGuid);
        }

    }
}