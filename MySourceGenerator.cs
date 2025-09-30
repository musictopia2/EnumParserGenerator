namespace EnumParserGenerator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ClassDeclarationSyntax> declares1 = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        var declares2 = context.CompilationProvider.Combine(declares1.Collect());

        // Step 3: Parse and flatten results
        var declares3 = declares2.Select((tuple, _) =>
        {
            var (compilation, customList) = tuple;
            var results = new List<ResultsModel>();

            foreach (var item in customList)
            {
                results.AddRange(GetResults(item, compilation));
            }

            return new CompleteModel(compilation.AssemblyName ?? "UnknownAssembly", [.. results]);
        });
        context.RegisterSourceOutput(declares3, Execute);
    }
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        bool rets = syntax is ClassDeclarationSyntax;
        return rets;
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode(); //can use the sematic model at this stage
        if (ourClass == null)
        {
            return null;
        }
        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol symbol)
        {
            return null;
        }

        // Check for fluent style inheritance
        if (symbol.InheritsFrom("BaseTypeParsingSettingsContext") && symbol.Name != "BaseTypeParsingSettingsContext")
        {
            return ourClass;
        }
        return null;
    }
    private static ImmutableHashSet<ResultsModel> GetResults(
        ClassDeclarationSyntax ourClass,
        Compilation compilation
        )
    {
        ParserClass parses = new(ourClass, compilation);
        BasicList<ResultsModel> output = parses.GetResults();
        return [.. output];
    }
    private void Execute(SourceProductionContext context, CompleteModel complete)
    {
        EmitClass emit = new(complete, context);
        emit.Emit(); //start out with console.  later do reals once ready.
    }
}