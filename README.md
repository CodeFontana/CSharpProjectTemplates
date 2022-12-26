# Visual Studio Starter Project Templates
Spend less time on boilerplate and more time on your ideas.  These are some basic starter templates to get you off the ground faster and allow you to focus on building your ideas.

## .NET Generic Host
The gold standard for hosting your project with Configuration, Dependency Injection and Logging.  This is the foundation across all these templates, be it Console, Background Workers, Desktop or Web projects.

## Logging with Serilog
Figuring out how to log messages is probably not the main focus of your new idea.  Thus, applicable templates are already configured for Serilog (https://github.com/serilog/serilog), which integrates perfectly with the .NET Generic Host provider.

## Blazor with MudBlazor
MudBlazor is just awesome with a comprehensive library of components, features and utilities.  You'll spend a lot less time writing custom CSS, especially media queries for breakpoints.  Rest assured MudBlazor brings custom theme support and you always have the option of bringing your own CSS/styling to the table if need be.

## WPF, brought to you by MVVM
I'm a big fan of MVVM, just not with some random magical framework.  MVVM is too much of an art that needs to be appreiciated and understood, so this is one area I won't borrow someone else's technology, at least for now.  Basing this template on the lovely .NET Generic Host, there's enough demo code (Views and ViewModels) to demo how to wire all that up, including basic navigation.  Enjoy!

## Web API + Data Library w/ EF Identity Database using JWT.
Save the best for last, API is probably the most important Web Project there is. Spanning two templats, WebAPI + DataLibrary, you'll start with an EF Identity Database for in-app authentication using JWT. Also included is API health checks and rate limiting packages. Save yourself an ENORMOUS amount of setup time, so you can dive right into the core of what your API needs to do.

## Download Templates -- *December 25, 2022*
These are the latest exports, built from this repository.

Copy these ZIP files to:
```
C:\Users\<Username>\Documents\Visual Studio 2022\Templates\ProjectTemplates
```
[ConsoleApp.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301336/ConsoleApp.zip)  
[DataLibrary.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301337/DataLibrary.zip)  
[MudBlazorServer.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301338/MudBlazorServer.zip)  
[MudBlazorWasm.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301339/MudBlazorWasm.zip)  
[WebApi.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301340/WebApi.zip)  
[WorkerService.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301341/WorkerService.zip)  
[WpfApp.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/10301342/WpfApp.zip)  

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
