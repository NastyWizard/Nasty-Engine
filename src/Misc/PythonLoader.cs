using System;
#if IRON_PYTHON
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
#endif
public class PythonLoader
{

#if IRON_PYTHON
    private static ScriptEngine engine;
    private static ScriptScope scope;

    public static ObjectOperations ops { get { return engine.Operations; } }
#endif

    public static void Initialize()
    {
#if IRON_PYTHON
        // IRONPYTHON
        engine = Python.CreateEngine();
        scope = engine.CreateScope();
        
        var paths = engine.GetSearchPaths();
        paths.Add(@"C:\Program Files\IronPython 2.7\Lib");
        engine.SetSearchPaths(paths);
        //
#else
        throw new System.Exception("ERROR: IronPython is not linked.");
#endif
    }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  

    public static void ExecuteFile(string file, params Tuple<string, object>[] variables)
    {
#if IRON_PYTHON
        foreach(var v in variables)
            scope.SetVariable(v.Item1, v.Item2);

        engine.ExecuteFile(file, scope);
#else
        throw new System.Exception("ERROR: IronPython is not linked.");
#endif
    }

    public static dynamic GetVariable(string variable)
    {
#if IRON_PYTHON
        return scope.GetVariable(variable);
#else
        throw new System.Exception("ERROR: IronPython is not linked.");
#endif
    }

    public static dynamic Cast(dynamic obj, Type castTo) { return Convert.ChangeType(obj, castTo); }

}
