using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UniRx;
using UniRx.Async;
using VRM;

public class VrmFileManager : MonoBehaviour
{
    private RuntimeAnimatorController loadedAnimatorController;

    private void Awake()
    {
        loadedAnimatorController = Resources.Load<RuntimeAnimatorController>("ファイルパス");
    }

    public async Task ImportVrmAsync(string fullPath, Transform parent)
    {
        var bytes = System.IO.File.ReadAllBytes(fullPath);
        var context = new VRMImporterContext();
        context.ParseGlb(bytes);
        await context.LoadAsyncTask();
        
        var root = context.Root;
        root.transform.SetParent(parent, false);
        root.transform.localPosition = new Vector3(0, 1f, 0);

        // 先にRigidbodyつける
        var rb = root.AddComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        // コライダつける
        var capsuleCollider = root.AddComponent<CapsuleCollider>();
        capsuleCollider.center = new Vector3(0, 0.7f, 0);
        capsuleCollider.radius = 0.2f;
        capsuleCollider.height = 1.5f;

        //  アニメーションできるように、Animatorつける
        var rootAnimator = root.GetComponent<Animator>();
        rootAnimator.runtimeAnimatorController = loadedAnimatorController;

        // Playerとして扱うならタグつける
        root.tag = TagName.Player;
        
        context.ShowMeshes();
    }
}
