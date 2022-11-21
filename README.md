# Visual Studio Starter Project Templates
Spend less time on boilerplate and more time on your ideas.  These are some basic starter templates to get you off the ground faster and allow you to focus on building your ideas.

## .NET Generic Host
The gold standard for hosting your project with Configuration, Dependency Injection and Logging.  This is the foundation across all these templates, be it Console, Background Workers, Desktop or Web projects.

## Logging with Serilog
Figuring out how to log messages is probably not the main focus of your new idea.  Thus, applicable templates are already configured for Serilog (https://github.com/serilog/serilog), which integrates perfectly with the .NET Generic Host provider.

## Blazor with MudBlazor
MudBlazor is just awesome with a comprehensive library of components, features and utilities.  You'll spend a lot less time writing custom CSS, especially media queries for breakpoints.  Rest assured MudBlazor brings custom theme support and you always have the option of bringing your own CSS/styling to the table if need be.

## Blazor with Bootstrap 5 + Blazorise
Blazorise is a component library supporting multiple CSS frameworks, including Bootstrap 5.  So for those looking for the familiarity of Bootstrap for their frontend, this starter template is the perfect combination of Blazor and Bootstrap to increase your development speed.

## Blazor with Bootstrap 5 + Radzen
Rapid Application Development (the Rad in RadZen) with this Blazor component library, also working in conjunction with Bootstrap 5!  So this component library brings the familiarity of working with Bootstrap and adds an army of components to your fingertips.

## WPF, brought to you by MVVM
I'm a big fan of MVVM, just not with some random magical framework.  MVVM is too much of an art that needs to be appreiciated and understood, so this is one area I won't borrow someone else's technology, at least for now.  Basing this template on the lovely .NET Generic Host, there's enough demo code (Views and ViewModels) to demo how to wire all that up, including basic navigation.  Enjoy!

## Web API + Data Library w/ EF Identity Database using JWT.
Save the best for last, API is probably the most important Web Project there is. Spanning two templats, WebAPI + DataLibrary, you'll start with an EF Identity Database for in-app authentication using JWT. Also included is API health checks and rate limiting packages. Save yourself an ENORMOUS amount of setup time, so you can dive right into the core of what your API needs to do.

## Download Templates -- *November 20, 2022*
These are the latest exports, built from this repository.

Copy these ZIP files to:
```
C:\Users\<Username>\Documents\Visual Studio 2022\Templates\ProjectTemplates
```
[Blazor Server App - Blazorise.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053194/Blazor.Server.App.-.Blazorise.zip)  
[Blazor Server App - MudBlazor.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053195/Blazor.Server.App.-.MudBlazor.zip)  
[Blazor Server App - RadzenBlazor.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053196/Blazor.Server.App.-.RadzenBlazor.zip)  
[Blazor Web Assembly - MudBlazor.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053197/Blazor.Web.Assembly.-.MudBlazor.zip)  
[Console App.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053198/Console.App.zip)  
[Data Library.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053199/Data.Library.zip)  
[Web API.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053200/Web.API.zip)  
[Worker Service.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053201/Worker.Service.zip)  
[WPF App.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10053202/WPF.App.zip)  

Each ZIP contains a _MyTemplate.vstemplate_ file, where you can adjust tagging, if necessary:
```
<VSTemplate Version="3.0.0" xmlns="http://schemas.microsoft.com/developer/vstemplate/2005" Type="Project">
  <TemplateData>
    <Name>Blazor Server App - MudBlazor</Name>
    <Description>A starter template based on MudBlazor.</Description>
    <ProjectType>CSharp</ProjectType>
    <ProjectSubType>
    </ProjectSubType>
	<LanguageTag>C#</LanguageTag>
	<PlatformTag>Linux</PlatformTag>
	<PlatformTag>macOS</PlatformTag>
	<PlatformTag>Windows</PlatformTag>
	<ProjectTypeTag>Web</ProjectTypeTag>
	<ProjectTypeTag>Brian</ProjectTypeTag>
	...
```

Once imported, templates will be available on the Visual Studio _Getting Started_ page, based on tag filtering:

![image](https://user-images.githubusercontent.com/41308769/179374861-9ba2dfe3-5d24-4f47-a8ca-3f1d1571ceb4.png)
  
  
# What will you build next?
