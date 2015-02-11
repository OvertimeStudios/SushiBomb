using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
[AddComponentMenu(D2D_Helper.ComponentMenuPrefix + "Destructible Sprite")]
public class D2D_DestructibleSprite : D2D_Destructible
{
	public static List<D2D_DestructibleSprite> DestructibleSprites = new List<D2D_DestructibleSprite>();
	
	public Material SourceMaterial;
	
	[D2D_RangeAttribute(1.0f, 100.0f)]
	public float Sharpness = 1.0f;
	
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	
	[SerializeField]
	private float expectedPixelsToUnits;
	
	[SerializeField]
	private Vector2 expectedPivot;
	
	[SerializeField]
	private Material clonedMaterial;
	
	public override Matrix4x4 WorldToPixelMatrix
	{
		get
		{
			if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
			
			if (AlphaTex != null)
			{
				var sprite = spriteRenderer.sprite;
				
				if (sprite != null)
				{
					var scale  = CalculateAlphaTexScale();
					var offset = CalculateAlphaTexOffset();
					var s      = D2D_Helper.ScalingMatrix(D2D_Helper.Reciprocal(scale));
					var t      = D2D_Helper.TranslationMatrix(-offset);
					
					return s * t * transform.worldToLocalMatrix;
				}
			}
			
			return transform.worldToLocalMatrix;
		}
	}
	
	[ContextMenu("Blur + Halve + Sharpen Alpha Tex")]
	public void CombinedHalveAlphaTex()
	{
		BlurAlphaTex();
		HalveAlphaTex();
		
		Sharpness *= 2;
	}
	
	protected override void ResetAlphaData()
	{
		base.ResetAlphaData();
		
		Sharpness = 1.0f;
	}
	
	protected override void OnEnable()
	{
		base.OnEnable();
		
		// Has this been cloned?
		if (clonedMaterial != null)
		{
			foreach (var destructibleSprite in DestructibleSprites)
			{
				if (destructibleSprite != null && destructibleSprite.clonedMaterial == clonedMaterial)
				{
					OnDuplicate();
				}
			}
		}
		
		DestructibleSprites.Add(this);
	}
	
	protected override void OnDisable()
	{
		base.OnDisable();
		
		DestructibleSprites.Remove(this);
	}
	
	protected virtual void Update()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		// Copy alpha from main tex
		if (AlphaTex == null && spriteRenderer.sprite != null)
		{
			ReplaceAlphaWith(spriteRenderer.sprite);
			
			RecalculateOriginalSolidPixelCount();
		}
		
#if UNITY_EDITOR
		D2D_Helper.MakeTextureReadable(DensityTex);
#endif
	}
	
	protected override void OnDestroy()
	{
		base.OnDestroy();
		
		DestroyMaterial();
	}
	
	protected virtual void OnWillRenderObject()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		UpdateSourceMaterial();
		
		DestroyMaterialIfSettingsDiffer();
		
		var sprite = spriteRenderer.sprite;
		
		if (SourceMaterial != null && sprite != null)
		{
			// Clone new material?
			if (clonedMaterial == null)
			{
				clonedMaterial = D2D_Helper.Clone(SourceMaterial, false);
			}
			else
			{
				clonedMaterial.CopyPropertiesFromMaterial(SourceMaterial);
			}
			
			clonedMaterial.hideFlags = HideFlags.HideInInspector;
			
			D2D_Helper.BeginStealthSet(clonedMaterial);
			{
				clonedMaterial.SetTexture("_MainTex", sprite.texture);
				clonedMaterial.SetTexture("_AlphaTex", AlphaTex);
				clonedMaterial.SetVector("_AlphaScale", CalculateAlphaScale(sprite));
				clonedMaterial.SetVector("_AlphaOffset", CalculateAlphaOffset(sprite));
				clonedMaterial.SetFloat("_Sharpness", Sharpness);
			}
			D2D_Helper.EndStealthSet();
		}
		
		if (spriteRenderer.sharedMaterial != clonedMaterial)
		{
			spriteRenderer.sharedMaterial = clonedMaterial;
		}
	}
	
	protected virtual void OnDuplicate()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		if (clonedMaterial == spriteRenderer.sharedMaterial)
		{
			clonedMaterial = D2D_Helper.Clone(clonedMaterial);
			
			spriteRenderer.sharedMaterial = clonedMaterial;
		}
	}
	
	private Vector2 CalculateAlphaScale(Sprite sprite)
	{
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var scaleX      = D2D_Helper.Divide(texture.width , Mathf.Floor(textureRect.width ) + AlphaShiftX) * D2D_Helper.Divide(OriginalWidth , AlphaWidth );
		var scaleY      = D2D_Helper.Divide(texture.height, Mathf.Floor(textureRect.height) + AlphaShiftY) * D2D_Helper.Divide(OriginalHeight, AlphaHeight);
		
		return new Vector2(scaleX, scaleY);
	}
	
	private Vector2 CalculateAlphaOffset(Sprite sprite)
	{
		var scalingX = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ), OriginalWidth );
		var scalingY = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height), OriginalHeight);
		
		var texture     = sprite.texture;
		var textureRect = sprite.textureRect;
		var offsetX     = D2D_Helper.Divide(Mathf.Ceil(textureRect.x + AlphaX * scalingX) - AlphaShiftX / 2, texture.width );
		var offsetY     = D2D_Helper.Divide(Mathf.Ceil(textureRect.y + AlphaY * scalingY) - AlphaShiftY / 2, texture.height);
		
		return new Vector2(offsetX, offsetY);
	}
	
	private void UpdateSourceMaterial()
	{
		// Do we need to set a source material?
		if (SourceMaterial == null)
		{
			if (spriteRenderer.sharedMaterial != null)
			{
				SourceMaterial = spriteRenderer.sharedMaterial;
			}
			else
			{
				SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			}
		}
		
		// Replace Sprites-Default with Sprites-Default (Destructible 2D)?
		if (SourceMaterial != null && SourceMaterial.HasProperty("_AlphaTex") == false)
		{
			if (SourceMaterial.name == "Sprites-Default")
			{
				SourceMaterial = Resources.Load<Material>("Sprites-Default (Destructible 2D)");
			}
		}
	}
	
	public Vector3 CalculateAlphaTexScale()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var scale  = Vector3.one;
		var sprite = spriteRenderer.sprite;
		
		if (AlphaTex != null && sprite != null)
		{
			scale.x = D2D_Helper.Divide(sprite.bounds.size.x, sprite.rect.width ) * D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ) + AlphaShiftX, AlphaWidth) * D2D_Helper.Divide(AlphaWidth, OriginalWidth );
			scale.y = D2D_Helper.Divide(sprite.bounds.size.y, sprite.rect.height) * D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height) + AlphaShiftY, AlphaHeight) * D2D_Helper.Divide(AlphaHeight, OriginalHeight);
		}
		
		return scale;
	}
	
	public Vector3 CalculateAlphaTexOffset()
	{
		if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
		
		var offset = Vector3.one;
		var sprite = spriteRenderer.sprite;
		
		if (AlphaTex != null && sprite != null)
		{
			var scalingX = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.width ), OriginalWidth );
			var scalingY = D2D_Helper.Divide(Mathf.Floor(sprite.textureRect.height), OriginalHeight);
			
			offset.x = sprite.bounds.min.x + sprite.bounds.size.x * (D2D_Helper.Divide(Mathf.Ceil(sprite.textureRectOffset.x) + AlphaX * scalingX - AlphaShiftX / 2, sprite.rect.width ));
			offset.y = sprite.bounds.min.y + sprite.bounds.size.y * (D2D_Helper.Divide(Mathf.Ceil(sprite.textureRectOffset.y) + AlphaY * scalingY - AlphaShiftY / 2, sprite.rect.height));
		}
		
		return offset;
	}
	
	private void DestroyMaterialIfSettingsDiffer()
	{
		if (clonedMaterial != null)
		{
			if (SourceMaterial == null)
			{
				DestroyMaterial(); return;
			}
			
			if (clonedMaterial.shader != SourceMaterial.shader)
			{
				DestroyMaterial(); return;
			}
		}
	}
	
	private void DestroyMaterial()
	{
		D2D_Helper.Destroy(clonedMaterial);
		
		clonedMaterial = null;
	}
}