using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.SolutionModel;
using Clustering.SolutionModel.Nodes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Tests.Building.TestExtensions
{
    public static class RoslynFaker
    {
        public class DisposableResult<TRes> : IDisposable 
        {
            public IDisposable Disposable;
            public TRes result;
            public void Dispose() {Disposable.Dispose();}
        }

        public static DisposableResult<IEnumerable<SymbolNode>> GetSymbolTreeFromDocuments(params string[] documentContents)
        {
            var fakeWorkspace = new AdhocWorkspace();
            var project = fakeWorkspace.AddProject("ProjectA", LanguageNames.CSharp);
            var i = 0;

            foreach (var content in documentContents)
            {
                fakeWorkspace.AddDocument(project.Id, "doc" + i, SourceText.From(content));
                i++;
            }

            var semanticModels = fakeWorkspace.CurrentSolution.GetProject(project.Id).Documents
                .Select(d => d.GetSemanticModelAsync().Result).ToList();

            return
                new DisposableResult<IEnumerable<SymbolNode>>
                {
                    Disposable = fakeWorkspace,
                    result = SemanticModelWalker.GetSymbolTreeFromModels(semanticModels).ToList()
                };
        }
    }
}
