using UnityEngine;

public class TestImportImage : MonoBehaviour
{
    private static readonly int MainTex = Shader.PropertyToID("_MainTex");

    public void ApplyTexture(Texture2D tex)
    {
        this.gameObject.GetComponent<Renderer>().sharedMaterial.SetTexture(MainTex, tex);
    }
}
