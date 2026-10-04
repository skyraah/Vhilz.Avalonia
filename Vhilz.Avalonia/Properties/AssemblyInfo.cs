using Avalonia.Metadata;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Vhilz.Avalonia.Tests")]
[assembly: InternalsVisibleTo("Vhilz.Avalonia.Demo")]

// 同一个 XAML 前缀同时公开主题入口和控件；URI 仅作标识，不需要联网。
[assembly: XmlnsDefinition("https://github.com/skyraah/Vhilz.Avalonia", "Vhilz.Avalonia.Theme")]
[assembly: XmlnsDefinition("https://github.com/skyraah/Vhilz.Avalonia", "Vhilz.Avalonia.Theme.Controls")]
[assembly: XmlnsPrefix("https://github.com/skyraah/Vhilz.Avalonia", "vhz")]
