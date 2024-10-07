<div align="Center">
    <img
        src="https://github.com/FatRainbowPony/Shell-previewer/blob/main/img/ShellPreviewer.svg"
        alt="ShellPreviewer" 
        width="250" 
        height="200">
</div>

# Shell Previewer
[![Nuget](https://img.shields.io/nuget/v/ShellPreviewer)](https://www.nuget.org/packages/ShellPreviewer)
[![Downloads](https://img.shields.io/nuget/dt/ShellPreviewer)](https://www.nuget.org/packages/ShellPreviewer)

This control allows you to host [Preview Handler](https://learn.microsoft.com/en-us/windows/win32/shell/preview-handlers) in the WPF application

## Features
- Displaying the preview as handler, thumbnail or icon
- Automatic resizing of preview handler when the parent container is resized
- Data binding to `Source` and `Status` properties of control

## Properties
- `Source` dependency property contains the path as `string` to the current file for which the preview is handling. Use this property to get or set the path to file for preview
- `Status` readonly dependency property contains current status as `StatusValue` for preview. Use this one to get status for preview in form:
  - `StatusValue.None`: Default value of preview status
  - `StatusValue.Handler`: Preview as handler
  - `StatusValue.Thumbnail`: Preview as thumbnail if handler hasn't been success loaded
  - `StatusValue.Icon`: Preview as icon if handler and thumbnail hasn't been success loaded
  - `StatusValue.Error`: Preview as error message if none of the cases above hasn't been success loaded

## Usage
Install the NuGet package and insert a reference to the `ShellPreviewer.Controls.Wpf` namespace. Then add the `ShellPreviewer` control
```xml
<Window 
    x:Class="ShellPreviewerDemo.MainWindow" 
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation" 
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml" 
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008" 
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006" 
    xmlns:shell="clr-namespace:ShellPreviewer.Controls.Wpf;assembly=ShellPreviewer" 
    mc:Ignorable="d" 
    Title="MainWindow" 
    Height="400" 
    Width="700">
  <Grid>
    <shell:ShellPreviewer 
        x:Name="ShellPreviewer"
        Source="{Binding Path, Mode=TwoWay}"/>
  </Grid>
</Window>
```

You can see [Shell previewer demo](https://github.com/FatRainbowPony/Shell-previewer/tree/main/src/Shell%20previewer%20demo) for an example project