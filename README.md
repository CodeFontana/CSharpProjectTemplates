# Visual Studio Starter Project Templates
Spend less time on boilerplate and more time on your ideas.  These are some basic starter templates to get you off the ground faster and allow you to focus on building your ideas.

## .NET Generic Host
The gold standard for hosting your project with Configuration, Dependency Injection and Logging.  This is the foundation across all these templates, be it Console, Background Workers, Desktop or Web projects.

## Logging with FileLogger
Figuring out how to log messages is probably not the main focus of your new idea.  Thus, the Console, Desktop and Worker Service templates are already configured for FileLogger (https://github.com/CodeFontana/FileLogger), which integrates perfectly with the .NET Generic Host provider.  This is a simple, fast and efficient implementation on ILogger, outputting to both File and Console.

## Blazor with MudBlazor
MudBlazor is just awesome with a comprehensive library of components, features and utilities.  You'll spend a lot less time writing custom CSS, especially media queries for breakpoints.  Rest assured MudBlazor brings custom theme support and you always have the option of bringing your own CSS/styling to the table if need be.

## WPF, brought to you by MVVM
I'm a big fan of MVVM, just not with some random magical framework.  MVVM is too much of an art that needs to be appreiciated and understood, so this is one area I won't borrow someone else's technology, at least for now.  Basing this template on the lovely .NET Generic Host, there's enough demo code (Views and ViewModels) to demo how to wire all that up, including basic navigation.  Enjoy!

## Download Templates -- April 18, 2022
These are the latest exports, built from this repository.

Copy these ZIP files to:
```
C:\Users\<Username>\Documents\Visual Studio 2022\Templates\ProjectTemplates
```

[Blazor Server App - MudBlazor.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/8506516/Blazor.Server.App.-.MudBlazor.zip)  
[Blazor Web Assembly - MudBlazor.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/8506517/Blazor.Web.Assembly.-.MudBlazor.zip)  
[ConsoleApp.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/8506518/ConsoleApp.zip)  
[WorkerService.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/8506519/WorkerService.zip)  
[WpfApp.zip](https://github.com/CodeFontana/CSharpProjectTemplates/files/8506520/WpfApp.zip)  


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
	<PlatformTag>Windows</PlatformTag>
	<ProjectTypeTag>Web</ProjectTypeTag>
	<ProjectTypeTag>Brian</ProjectTypeTag>
...
```

Once imported, templates will be available on the Visual Studio _Getting Started_ page, based on tag filtering:

![image](https://user-images.githubusercontent.com/41308769/161820292-982baac6-7cf7-4e86-8c83-f10102bdb0ce.png)
