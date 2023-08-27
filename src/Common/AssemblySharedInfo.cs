using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyCompany("Automation Agent")]
[assembly: AssemblyProduct("Generative AI Library")]
[assembly: AssemblyCopyright("Copyright © Automation Agent 2023")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Make it easy to distinguish Debug and Release (i.e. Retail) builds;
// for example, through the file properties window.
#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
[assembly: AssemblyDescription("Flavor=Debug")] // a.k.a. "Comments"
#else
[assembly: AssemblyConfiguration("Release")]
[assembly: AssemblyDescription("Flavor=Release")] // a.k.a. "Comments"
#endif

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

//Make internal classes visible to Test dll for testing purpose
#if DEBUG
[assembly: InternalsVisibleTo("GenAIFramework.Test")]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.0.*")]
