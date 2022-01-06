#if False
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class OBBEditorManager : EditorWindow
{
    static void Init()
    {
        GetWindow<OBBEditorManager>();
    }

    void OnEnable()
    {
        AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
    }

    void OnDisable()
    {
        AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
        AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
    }

    public void OnBeforeAssemblyReload()
    {
        Debug.Log("Before Assembly Reload");
    }

    public void OnAfterAssemblyReload()
    {
        Debug.Log("After Assembly Reload");
    }
}
#endif