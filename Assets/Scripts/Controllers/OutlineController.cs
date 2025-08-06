using UnityEngine;

public class OutlineController : MonoBehaviour {
    [Header("Outline Settings")]
    [SerializeField] private Material outlineMaterial;
    [SerializeField] private bool autoFindRenderers = true;
    [SerializeField] private bool replaceBaseMaterial = false;
    [SerializeField] private int baseMaterialIndex = 0;

    private Renderer[] renderers;
    private Material[][] originalMaterials;
    private bool isOutlined = false;

    private void Awake() {
        if (autoFindRenderers) {
            FindRenderers();
        }
    }

    private void FindRenderers() {
        // Buscar todos los renderers en el GameObject y sus hijos
        renderers = GetComponentsInChildren<Renderer>();
        StoreOriginalMaterials();
    }

    public void SetRenderers(Renderer[] customRenderers) {
        renderers = customRenderers;
        StoreOriginalMaterials();
    }

    private void StoreOriginalMaterials() {
        if (renderers == null || renderers.Length == 0) return;

        originalMaterials = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] != null) {
                originalMaterials[i] = renderers[i].materials;
            }
        }
    }

    public void ShowOutline() {
        if (isOutlined || renderers == null || outlineMaterial == null) return;

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] != null && originalMaterials[i] != null) {
                if (replaceBaseMaterial) {
                    // Reemplazar el material base con el outline
                    Material[] materialsWithOutline = new Material[originalMaterials[i].Length];
                    System.Array.Copy(originalMaterials[i], materialsWithOutline, originalMaterials[i].Length);

                    // Reemplazar el material en el índice especificado
                    if (baseMaterialIndex < materialsWithOutline.Length) {
                        materialsWithOutline[baseMaterialIndex] = outlineMaterial;
                    }

                    renderers[i].materials = materialsWithOutline;
                }
                else {
                    // Agregar outline como material adicional (comportamiento original)
                    Material[] materialsWithOutline = new Material[originalMaterials[i].Length + 1];
                    System.Array.Copy(originalMaterials[i], materialsWithOutline, originalMaterials[i].Length);
                    materialsWithOutline[materialsWithOutline.Length - 1] = outlineMaterial;
                    renderers[i].materials = materialsWithOutline;
                }
            }
        }

        isOutlined = true;
    }

    public void HideOutline() {
        if (!isOutlined || renderers == null) return;

        for (int i = 0; i < renderers.Length; i++) {
            if (renderers[i] != null && originalMaterials[i] != null) {
                renderers[i].materials = originalMaterials[i];
            }
        }

        isOutlined = false;
    }

    public void ToggleOutline() {
        if (isOutlined) {
            HideOutline();
        }
        else {
            ShowOutline();
        }
    }

    public bool IsOutlined => isOutlined;

    private void OnDestroy() {
        HideOutline();
    }

    // Método para refrescar los renderers si la jerarquía cambia
    public void RefreshRenderers() {
        if (autoFindRenderers) {
            FindRenderers();
        }
    }
}