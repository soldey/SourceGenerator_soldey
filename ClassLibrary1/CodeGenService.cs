using CodeGenService.entity;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeGenService
{
    [Generator]
    public class CodeGenService : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            string filepath = @"E:\work\ИТМО\техи\codegen_ya_ebal\ConsoleApp1\ClassLibrary1\cfg\swagger.json";
            SwaggerEntity swaggerEntity = JsonSerializer.Deserialize<SwaggerEntity>(File.ReadAllText(filepath));

            var namespaceBlock = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("CodeGenClient")).NormalizeWhitespace();
            foreach (var definition in swaggerEntity.Definitions)
            {
                var classBlock = SyntaxFactory.ClassDeclaration(definition.Key).AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
                foreach (var property in definition.Value.Properties)
                {
                    if (property.Value.Type is null)
                        continue;
                    var propertyBlock = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(property.Value.FormatType()), property.Key)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddAccessorListAccessors(SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));
                    classBlock = classBlock.AddMembers(propertyBlock);
                }
                context.AddSource($"{definition.Key}.g.cs", namespaceBlock.AddMembers(classBlock).NormalizeWhitespace().ToFullString());
            }
            namespaceBlock = namespaceBlock.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Net.Http")),
                SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading.Tasks")));
            var classServiceBlock = SyntaxFactory.ClassDeclaration("CodeGenClientService").AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));
            foreach (var path in swaggerEntity.Paths)
            {
                foreach (var methodE in path.Value)
                {

                    var parameterSyntaxList = new List<ParameterSyntax>();
                    if (methodE.Value.Parameters != null)
                    {
                        foreach (var parameter in methodE.Value.Parameters)
                        {
                            parameterSyntaxList.Add(
                                SyntaxFactory.Parameter(SyntaxFactory.Identifier(parameter.Name)).WithType(SyntaxFactory.ParseTypeName(parameter.GetType()))
                            );
                        }
                    }

                    classServiceBlock = classServiceBlock.AddMembers(SyntaxFactory.MethodDeclaration(
                            SyntaxFactory.ParseTypeName(
                                "Task<" + methodE.Value.Responses["200"].Schema.GetType() + ">"),
                            FormatMethod(methodE))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                        .AddParameterListParameters(parameterSyntaxList.ToArray())
                        .WithBody(SyntaxFactory.Block(
                            SyntaxFactory.ReturnStatement()
                                .WithExpression(SyntaxFactory.ParseExpression($"await new HttpClient(){FormatMethodCall(path.Key, methodE)}")))
                        ));
                }
            }
            Console.WriteLine(namespaceBlock.AddMembers(classServiceBlock).NormalizeWhitespace().ToFullString());
            context.AddSource($"CodeGenClientService.g.cs", namespaceBlock.AddMembers(classServiceBlock).NormalizeWhitespace().ToFullString());
        }

        private string FormatMethod(KeyValuePair<string, entity.Path> method)
        {
            string res = "";
            if (method.Key != null) res += method.Key[0].ToString().ToUpper() + method.Key.Substring(1);
            if (method.Value.Summary != null) res += method.Value.Summary[0].ToString().ToUpper() + method.Value.Summary.Substring(1);
            return res;
        }

        private string FormatMethodCall(string uri, KeyValuePair<string, entity.Path> method)
        {
            string call = "";
            string body = null;
            if (method.Key == "get")
                call = "GetAsync(\"";
            else if (method.Key == "put")
                call = "PutAsync(\"";
            else if (method.Key == "delete")
                call = "DeleteAsync(\"";
            else
                call = "PostAsync(\"";
            if (method.Value.Parameters != null)
            {
                foreach (var parameter in method.Value.Parameters)
                {
                    if (parameter.In == "query")
                    {
                        uri += String.Format("{0}{1}=\" + {2}", uri.Contains("?") ? "\"&" : "?", parameter.Name, parameter.Name);
                    }
                    else if (parameter.In == "body")
                    {
                        body = parameter.Name;
                    }
                }
            }
            call += String.Format("{0}{1})",
                uri.Contains("?") ? uri : uri + "\"",
                (method.Key != "get" && method.Key != "delete") ? String.Format(", {0}", body == null ? "null" : $"new StringContent({body}.ToString())") : "");
            return $".{call}.Result.Content.ReadAsAsync<{method.Value.Responses["200"].Schema.GetType()}>()";
        }
    }
}
