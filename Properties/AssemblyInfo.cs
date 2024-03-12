using MelonLoader;
using System.Reflection;
using static FadeIn.Properties.MelonBuildInfo;
using Main = FadeIn.Main;

[assembly: MelonInfo(typeof(Main), ModName, ModVersion, Author)]
[assembly: MelonGame("PeroPeroGames", "MuseDash")]
[assembly: MelonColor(255, 241, 196, 15)]

[assembly: AssemblyTitle(ModName)]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Muse Dash Modding Community")]
[assembly: AssemblyProduct(ModName)]
[assembly: AssemblyCopyright("Copyright © 2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion(ModVersion)]
[assembly: AssemblyFileVersion(ModVersion)]