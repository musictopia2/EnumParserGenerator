namespace EnumParserGenerator;
internal class ParserClass(ClassDeclarationSyntax ourClass, Compilation compilation)
{
    public BasicList<ResultsModel> GetResults()
    {
        BasicList<ResultsModel> output = [];
        output = GetFluentResults(ourClass);
        return output;
    }
    private BasicList<ResultsModel> GetFluentResults(ClassDeclarationSyntax classDeclaration)
    {
        ParseContext context = new(compilation, classDeclaration);
        var members = classDeclaration.DescendantNodes().OfType<MethodDeclarationSyntax>();
        foreach (var m in members)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(m) as IMethodSymbol;
            if (symbol is not null && symbol.Name == "Configure") //has to be magic strings now.
            {
                BasicList<ResultsModel> output = [];
                ParseSettings(output, context, m);
                return output;
            }
        }
        return [];
    }
    private void ParseSettings(BasicList<ResultsModel> results, ParseContext context, MethodDeclarationSyntax syntax)
    {
        var makeCalls = ParseUtils.FindCallsOfMethodWithName(context, syntax, "Make");
        foreach (CallInfo make in makeCalls)
        {
            ResultsModel result = new();
            results.Add(result);
            INamedTypeSymbol possibleEnumSymbol = (INamedTypeSymbol)make.MethodSymbol.TypeArguments[0]!;
            EnumSimpleTypeCategory category = possibleEnumSymbol.GetVariableCategory();
            if (category != EnumSimpleTypeCategory.CustomEnum && category != EnumSimpleTypeCategory.StandardEnum)
            {
                continue; //only custom and standard enums are supported.   all else are ignored from this source generator
            }
            result.ClassName = possibleEnumSymbol.Name;
            result.Namespace = possibleEnumSymbol.ContainingNamespace.ToDisplayString();
            result.Category = category;
            if (category == EnumSimpleTypeCategory.StandardEnum)
            {
                //has to figure out how to get a list of names associated with this symbol
                result.Values = GetEnums(possibleEnumSymbol);
            }

            results.Add(result);
        }
    }
    private BasicList<string> GetEnums(ITypeSymbol symbol)
    {
        BasicList<string> output = [];
        //this is the work that is required.

        //var aa = symbol.DeclaringSyntaxReferences.Single().SyntaxTree;
        //var list = record.DescendantNodes().OfType<EnumDeclarationSyntax>();
        var firstEnums = symbol.DeclaringSyntaxReferences.Single().SyntaxTree.GetAllMembers().OfType<EnumDeclarationSyntax>().ToBasicList();

        foreach (var item in firstEnums)
        {
            string name = item.Identifier.ValueText;

            if (name == symbol.Name)
            {
                //this is what i need.
                var nexts = item.DescendantNodes().OfType<EnumMemberDeclarationSyntax>().ToBasicList();
                foreach (var next in nexts)
                {
                    output.Add(next.Identifier.ValueText);
                }
            }
        }
        return output;
    }
}