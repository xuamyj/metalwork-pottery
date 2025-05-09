﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Object = UnityEngine.Object;

namespace Ruccho.Fang
{
    [CustomEditor(typeof(FangAutoTile)), CanEditMultipleObjects]
    public class FangAutoTileEditor : Editor
    {
        public static readonly string p_EnablePadding = "enablePadding";
        public static readonly string p_OneTilePerUnit = "oneTilePerUnit";
        public static readonly string p_PixelsPerUnit = "pixelsPerUnit";
        public static readonly string p_WrapMode = "wrapMode";
        public static readonly string p_FilterMode = "filterMode";
        public static readonly string p_MainChannel = "mainChannel";
        public static readonly string p_SubChannels = "subChannels";
        public static readonly string p_AnimationMinSpeed = "animationMinSpeed";
        public static readonly string p_AnimationMaxSpeed = "animationMaxSpeed";
        public static readonly string p_AnimationStartTime = "animationStartTime";
        public static readonly string p_ColliderType = "colliderType";
        public static readonly string p_ConnectableTiles = "connectableTiles";


        public static readonly string p_Packer = "packer";
        public static readonly string p_CompiledChannels = "compiledChannels";
        public static readonly string p_Combinations = "combinations";
        public static readonly string p_CombinationTable = "combinationTable";

        public static readonly string p_TC_CombinationId = "combinationId";
        public static readonly string p_TC_Frames = "frames";

        private static readonly int paddingSize = 2;

        public override void OnInspectorGUI()
        {
            serializedObject.UpdateIfRequiredOrScript();

            EditorGUI.BeginChangeCheck();

            if (Foldout("tile", "Tile Settings"))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_AnimationMinSpeed));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_AnimationMaxSpeed));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_AnimationStartTime));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_ColliderType));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_ConnectableTiles));
                }

                GUILayout.Space(20f);
            }

            if (Foldout("generation", "Tile Generation Settings"))
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_EnablePadding));

                    var oneTilePerUnitProp = serializedObject.FindProperty(p_OneTilePerUnit);
                    EditorGUILayout.PropertyField(oneTilePerUnitProp);

                    if (!oneTilePerUnitProp.boolValue)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(p_PixelsPerUnit));
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_WrapMode));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_FilterMode));


                    var mainChannelProp = serializedObject.FindProperty(p_MainChannel);

                    if (!serializedObject.isEditingMultipleObjects)
                    {
                        GUILayout.Space(EditorGUIUtility.singleLineHeight * 0.5f);

                        EditorGUILayout.LabelField("Sources", EditorStyles.boldLabel);

                        mainChannelProp.objectReferenceValue = EditorGUILayout.ObjectField("Main Channel",
                            mainChannelProp.objectReferenceValue, typeof(Texture2D), false);
                    }
                    else
                    {
                        EditorGUILayout.ObjectField(mainChannelProp, typeof(Texture2D));
                    }

                    EditorGUILayout.PropertyField(serializedObject.FindProperty(p_SubChannels));
                }

                GUILayout.Space(20f);
            }

            bool generate = false;

            if (!serializedObject.isEditingMultipleObjects)
            {
                bool isValid = CheckValidity(out string validityMessage);


                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty(p_Packer));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox(validityMessage, isValid ? MessageType.Info : MessageType.Error);

                EditorGUI.BeginDisabledGroup(!isValid);
                generate = GUILayout.Button("Generate!", GUILayout.Height(50f));
                EditorGUI.EndDisabledGroup();

                Color c = GUI.color;
                GUI.color = Color.red;
                if (GUILayout.Button("Clear all generated contents")) Clear();
                GUI.color = c;
                EditorGUILayout.HelpBox(GetInfo(), MessageType.Info);
            }


            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (generate) Generate();
        }

        private static bool Foldout(string id, string title)
        {
            bool display = EditorPrefs.GetBool($"{typeof(FangAutoTileEditor).FullName}/foldout/{id}", false);

            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.fontSize = 12;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if (e.type == EventType.Repaint)
            {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
            {
                display = !display;
                EditorPrefs.SetBool($"{typeof(FangAutoTileEditor).FullName}/foldout/{id}", display);
                e.Use();
            }

            return display;
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var combinationsProp = serializedObject.FindProperty(p_Combinations);
            if (combinationsProp.arraySize > 0)
            {
                var framesProp = combinationsProp.GetArrayElementAtIndex(0).FindPropertyRelative(p_TC_Frames);
                if (framesProp.arraySize > 0)
                {
                    var sprite = framesProp.GetArrayElementAtIndex(0).objectReferenceValue as Sprite;
                    if (sprite)
                    {
                        Texture2D p = AssetPreview.GetAssetPreview(sprite);
                        if (p)
                        {
                            Texture2D f = new Texture2D(width, height);
                            EditorUtility.CopySerialized(p, f);
                            return f;
                        }
                    }
                }
            }

            return base.RenderStaticPreview(assetPath, subAssets, width, height);
        }


        public bool CheckValidity(out string message)
        {
            var mainChannelProp = serializedObject.FindProperty(p_MainChannel);

            var mainChannel = mainChannelProp.objectReferenceValue as Texture2D;

            if (!mainChannel)
            {
                message = "Set main channel texture.";
                return false;
            }

            int width = mainChannel.width;
            int height = mainChannel.height;

            int tileSize = height / 5;

            if (height % 5 != 0 || width % tileSize != 0)
            {
                message = "Size of the texture has to be specific format:\n" +
                          " width:  (Tile size) * (Number of frame)\n" +
                          " height: (Tile size) * 5\n";
                return false;
            }

            /*
            if (!mainChannel.isReadable)
            {
                message = "Turn on \"Read / Write Enabled\" option in the Texture Import Settings.";
                return false;
            }
            */

            var subChannelsProp = serializedObject.FindProperty(p_SubChannels);

            int numSubChannels = subChannelsProp.arraySize;

            for (int i = 0; i < numSubChannels; i++)
            {
                var e = subChannelsProp.GetArrayElementAtIndex(i);
                var subChannel = e.objectReferenceValue as Texture2D;

                if (subChannel == null)
                {
                    message = "Sub Channels contain empty element.";
                    return false;
                }

                if (subChannel.width != width || subChannel.height != height)
                {
                    message = "Size of sub channel texture doesn't match one of the main channel.";
                    return false;
                }

                /*
                if (!subChannel.isReadable)
                {
                    message =
                        "Turn on \"Read / Write Enabled\" option of the sub channel texture in the Texture Import Settings.";
                    return false;
                }
                */
            }

            message = "Ready to generate a tile!";
            return true;
        }

        private string GetInfo()
        {
            var combinationsProp = serializedObject.FindProperty(p_Combinations);

            if (combinationsProp.arraySize == 0)
            {
                return "No tiles generated";
            }

            var framesProp = combinationsProp.GetArrayElementAtIndex(0).FindPropertyRelative(p_TC_Frames);

            int frames = framesProp.arraySize;

            if (frames == 0 || !framesProp.GetArrayElementAtIndex(0).objectReferenceValue)
            {
                return "Combinations enumerated but no sprites generated";
            }

            var sprite = framesProp.GetArrayElementAtIndex(0).objectReferenceValue as Sprite;
            var texture = sprite.texture;

            return $"Generated:" +
                   $" Combinations: {combinationsProp.arraySize}\n" +
                   $" Number of frames: {frames}\n" +
                   $" Tile size: {sprite.rect.width} x {sprite.rect.height}\n" +
                   $" Texture size: {texture.width} x {texture.height}";
        }

        private void Generate()
        {
            if (!CheckValidity(out var mes))
            {
                EditorUtility.DisplayDialog("Fang Auto Tile", mes, "OK");
                return;
            }

            var packerProp = serializedObject.FindProperty(p_Packer);
            bool isPacked = packerProp.objectReferenceValue;

            if (isPacked && !EditorUtility.DisplayDialog("Fang Auto Tile",
                    "This tile is set to be used with texture packer. Are you sure?", "OK", "Cancel"))
            {
                return;
            }

            try
            {
                GenerateCombination();
                GenerateTexture();
            }
            finally
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));

                serializedObject.ApplyModifiedProperties();
            }
        }

        private void Clear()
        {
            if (!EditorUtility.DisplayDialog("Confirm",
                    "Clearing all generated contents may cause missing references of textures. Are you sure?", "Yes",
                    "No"))
            {
                return;
            }

            ClearCombinations();
            ClearTextures();

            /*
            foreach(var o in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(target)))
            {
                if (AssetDatabase.IsSubAsset(o))
                {
                    DestroyImmediate(o, true);
                }
            }
            */

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
            serializedObject.ApplyModifiedProperties();
        }

        public void GenerateCombination()
        {
            var combinationTableProp = serializedObject.FindProperty(p_CombinationTable);
            var combinationsProp = serializedObject.FindProperty(p_Combinations);

            //いちど生成すれば内容は固定
            if (combinationsProp.arraySize > 0) return;

            ClearCombinations();

            //List<uint> combinationIds = new List<uint>();
            int[] combinationIds = new int[2341];

            int numCombinations = 0;
            for (int neighborCombination = 0; neighborCombination < 256; neighborCombination++)
            {
                uint combinationId = GetCombinationId((byte)neighborCombination);

                if (combinationIds[combinationId] == 0)
                {
                    combinationIds[combinationId] = numCombinations + 1;
                    numCombinations++;
                }

                var tableElement = combinationTableProp.GetArrayElementAtIndex(neighborCombination);
                tableElement.intValue = combinationIds[combinationId] - 1;
            }

            combinationsProp.arraySize = numCombinations;

            for (int i = 0; i < combinationIds.Length; i++)
            {
                if (combinationIds[i] != 0)
                {
                    combinationsProp
                        .GetArrayElementAtIndex(combinationIds[i] - 1).FindPropertyRelative(p_TC_CombinationId)
                        .longValue = i;
                }
            }

            uint GetCombinationId(byte neighborCombination)
            {
                bool bl = (neighborCombination & (1 << 0)) != 0;
                bool bc = (neighborCombination & (1 << 1)) != 0;
                bool br = (neighborCombination & (1 << 2)) != 0;
                bool mr = (neighborCombination & (1 << 3)) != 0;
                bool tr = (neighborCombination & (1 << 4)) != 0;
                bool tc = (neighborCombination & (1 << 5)) != 0;
                bool tl = (neighborCombination & (1 << 6)) != 0;
                bool ml = (neighborCombination & (1 << 7)) != 0;

                byte c_bl = DetermineKind(bc, ml, bl);
                byte c_br = DetermineKind(bc, mr, br);
                byte c_tl = DetermineKind(tc, ml, tl);
                byte c_tr = DetermineKind(tc, mr, tr);

                // UInt32 combinationId: ....................[.bl][.br][.tl][.tr]
                uint combinationId = 0;

                combinationId += (uint)(c_bl << 9);
                combinationId += (uint)(c_br << 6);
                combinationId += (uint)(c_tl << 3);
                combinationId += (uint)(c_tr << 0);

                return combinationId;
            }

            byte DetermineKind(bool vertical, bool horizontal, bool corner)
            {
                if (!vertical && !horizontal) return 0;
                if (vertical && !horizontal) return 1;
                if (!vertical && horizontal) return 2;
                if (!corner) return 3;
                return 4;
            }
        }

        private void GenerateTexture()
        {
            var mainChannelProp = serializedObject.FindProperty(p_MainChannel);
            var subChannelsProp = serializedObject.FindProperty(p_SubChannels);
            var compiledChannelsProp = serializedObject.FindProperty(p_CompiledChannels);
            var packerProp = serializedObject.FindProperty(p_Packer);

            packerProp.objectReferenceValue = null;

            var enablePadding = serializedObject.FindProperty(p_EnablePadding).boolValue;
            var wrapMode = (TextureWrapMode)serializedObject.FindProperty(p_WrapMode).enumValueIndex;
            var filterMode = (FilterMode)serializedObject.FindProperty(p_FilterMode).enumValueIndex;

            int wholeChannels = subChannelsProp.arraySize + 1;


            IReadOnlyList<TileCombinationSegment> segments = GetSegments().ToList();

            var segmentsOrdered = segments.OrderByDescending((seg) => seg.Width * seg.Height);

            int texSize = GetSuitableTextureSize(segmentsOrdered, enablePadding);

            //Validate textures
            for (int i = 0; i < Mathf.Max(wholeChannels, compiledChannelsProp.arraySize); i++)
            {
                if (wholeChannels <= i)
                {
                    //Need to be deleted
                    var tex = compiledChannelsProp.GetArrayElementAtIndex(i).objectReferenceValue as Texture2D;
                    if (!tex) continue;

                    DestroyImmediate(tex, true);
                }
                else
                {
                    if (compiledChannelsProp.arraySize <= i)
                    {
                        compiledChannelsProp.InsertArrayElementAtIndex(i);
                        compiledChannelsProp.GetArrayElementAtIndex(i).objectReferenceValue = null;
                    }

                    var element = compiledChannelsProp.GetArrayElementAtIndex(i);
                    var tex = element.objectReferenceValue as Texture2D;
                    var format = GraphicsFormat.R8G8B8A8_SRGB;
                    
                    if (tex)
                    {
                        if (tex.width != texSize || tex.height != texSize || tex.graphicsFormat != format)
                        {
                            tex.Resize(texSize, texSize, format, false);
                        }
                    }
                    else
                    {
                        tex = new Texture2D(texSize, texSize, DefaultFormat.LDR, TextureCreationFlags.None);
                        tex.Resize(texSize, texSize, format, false);
                        tex.name = "Texture";
                        AssetDatabase.AddObjectToAsset(tex, target);
                        element.objectReferenceValue = tex;
                    }

                    tex.wrapMode = wrapMode;
                    tex.filterMode = filterMode;
                }
            }

            {
                var src = mainChannelProp.objectReferenceValue as Texture2D;
                var srcBuffer = new TemporaryTexture2DBuffer(src);

                var dest = compiledChannelsProp.GetArrayElementAtIndex(0).objectReferenceValue as Texture2D;
                GenerateTilesForTexture(dest, segmentsOrdered.Select(s => new TileDrawingItem(s, srcBuffer)),
                    enablePadding, true);
            }

            for (int i = 1; i < wholeChannels; i++)
            {
                var src = subChannelsProp.GetArrayElementAtIndex(i - 1).objectReferenceValue as Texture2D;
                var srcBuffer = new TemporaryTexture2DBuffer(src);

                var dest = compiledChannelsProp.GetArrayElementAtIndex(i).objectReferenceValue as Texture2D;
                GenerateTilesForTexture(dest, segmentsOrdered.Select(s => new TileDrawingItem(s, srcBuffer)),
                    enablePadding, false);
            }
        }

        public static int GetSuitableTextureSize(IEnumerable<TileCombinationSegment> segmentsOrdered,
            bool enablePadding)
        {
            ulong square = 0;
            foreach (var segment in segmentsOrdered)
            {
                square += (ulong)(segment.Width * segment.Height);
                if (enablePadding)
                    square += (ulong)(segment.Width * paddingSize + segment.Height * paddingSize +
                                      paddingSize * paddingSize);
            }

            int minTexSizeExp = Mathf.CeilToInt(Mathf.Log(2f, Mathf.Sqrt(square)));
            while (true)
            {
                int currentTexSize = (int)Mathf.Pow(2, minTexSizeExp);

                using (var segmentOrdered = segmentsOrdered.GetEnumerator())
                {
                    int x = 0;
                    int y = 0;
                    int hMax = 0;
                    bool isFailed = false;
                    while (segmentOrdered.MoveNext())
                    {
                        int w = segmentOrdered.Current.Width;
                        if (enablePadding) w += paddingSize;

                        int h = segmentOrdered.Current.Width;
                        if (enablePadding) h += paddingSize;

                        hMax = Mathf.Max(h, hMax);

                        if (currentTexSize <= y + h)
                        {
                            isFailed = true;
                            break;
                        }

                        x += w;

                        if (currentTexSize <= x)
                        {
                            x = w;

                            y += hMax;
                            hMax = 0;

                            if (currentTexSize <= y)
                            {
                                isFailed = true;
                                break;
                            }
                        }
                    }

                    if (!isFailed) break;

                    minTexSizeExp++;
                }
            }

            return (int)Mathf.Pow(2, minTexSizeExp);
        }

        public static void GenerateTilesForTexture(Texture2D dstChannel,
            IEnumerable<TileDrawingItem> orderedSegments, bool enablePadding, bool createSprite)
        {
            int texSize = dstChannel.width;

            var dstBuffer = new TemporaryTexture2DBuffer(dstChannel);
            dstBuffer.ClearPixels();

            using (var segment = orderedSegments.GetEnumerator())
            {
                int x = 0;
                int y = 0;
                int hMax = 0;
                while (segment.MoveNext())
                {
                    var srcChannel = segment.Current.SourceBuffer;
                    var s = segment.Current.Segment;

                    int w = s.Width;
                    if (enablePadding) w += paddingSize;

                    int h = s.Width;
                    if (enablePadding) h += paddingSize;

                    hMax = Mathf.Max(h, hMax);


                    if (texSize <= x + w)
                    {
                        x = 0;

                        y += hMax;
                        hMax = 0;

                        if (texSize <= y)
                        {
                            throw new InvalidOperationException();
                        }
                    }

                    //Debug.Log($"x: {x}, y : {y}");
                    var spriteRect = new Rect(x, y, s.Width, s.Height);
                    if (enablePadding)
                    {
                        s.Copy(srcChannel, dstBuffer, x + 1, y + 1, true);
                        spriteRect.x += 1;
                        spriteRect.y += 1;
                    }
                    else
                    {
                        s.Copy(srcChannel, dstBuffer, x, y, false);
                    }

                    if (createSprite)
                    {
                        var existSprite = s.SpriteProperty.objectReferenceValue as Sprite;
                        if (existSprite && existSprite.texture == dstChannel && existSprite.rect.Equals(spriteRect) &&
                            existSprite.pixelsPerUnit == s.PixelsPerUnit)
                        {
                            //Use existing
                            existSprite.hideFlags |= HideFlags.HideInHierarchy;
                        }
                        else
                        {
                            if (existSprite) DestroyImmediate(existSprite, true);

                            Sprite sprite = Sprite.Create(dstChannel, spriteRect, new Vector2(0.5f, 0.5f),
                                s.PixelsPerUnit);
                            sprite.hideFlags |= HideFlags.HideInHierarchy;

                            AssetDatabase.AddObjectToAsset(sprite, s.SpriteProperty.serializedObject.targetObject);

                            s.SpriteProperty.objectReferenceValue = sprite;
                        }
                    }

                    x += w;
                }
            }

            dstBuffer.Apply();
        }

        public IEnumerable<TileCombinationSegment> GetSegments()
        {
            var mainChannelProp = serializedObject.FindProperty(p_MainChannel);

            var explicitPixelsPerUnit = serializedObject.FindProperty(p_PixelsPerUnit).intValue;
            var oneTilePerUnit = serializedObject.FindProperty(p_OneTilePerUnit).boolValue;

            var mainChannel = mainChannelProp.objectReferenceValue as Texture2D;

            int width = mainChannel.width;
            int height = mainChannel.height;

            int tileSize = height / 5;
            int numFrames = width / tileSize;

            int pixelsPerUnit = oneTilePerUnit ? tileSize : explicitPixelsPerUnit;

            var combinationsProp = serializedObject.FindProperty(p_Combinations);
            for (int combination = 0; combination < combinationsProp.arraySize; combination++)
            {
                var element = combinationsProp.GetArrayElementAtIndex(combination);
                var combinationIdProp = element.FindPropertyRelative(p_TC_CombinationId);
                var framesProp = element.FindPropertyRelative(p_TC_Frames);

                // Delete sprites that are exactly unused
                for (int f = numFrames; f < framesProp.arraySize; f++)
                {
                    var frameProp = framesProp.GetArrayElementAtIndex(f);
                    var frame = frameProp.objectReferenceValue;
                    if (frame)
                    {
                        DestroyImmediate(frame, true);
                    }
                }

                framesProp.arraySize = numFrames;

                uint combinationId = (uint)combinationIdProp.longValue;

                byte tr = (byte)((combinationId >> 0) & 0b111);
                byte tl = (byte)((combinationId >> 3) & 0b111);
                byte br = (byte)((combinationId >> 6) & 0b111);
                byte bl = (byte)((combinationId >> 9) & 0b111);

                //Debug.Log($"BL: {bl}, BR: {br}, TL: {tl}, TR: {tr}");

                for (int frame = 0; frame < numFrames; frame++)
                {
                    var frameProp = framesProp.GetArrayElementAtIndex(frame);
                    yield return new TileCombinationSegment(
                        GetSegment(tileSize, 0, frame, bl),
                        GetSegment(tileSize, 1, frame, br),
                        GetSegment(tileSize, 2, frame, tl),
                        GetSegment(tileSize, 3, frame, tr),
                        frameProp,
                        pixelsPerUnit
                    );
                }
            }
        }

        private void ClearCombinations()
        {
            var combinationTableProp = serializedObject.FindProperty(p_CombinationTable);
            var combinationsProp = serializedObject.FindProperty(p_Combinations);

            ClearSprites();

            combinationTableProp.ClearArray();
            combinationTableProp.arraySize = 1 << 8;

            combinationsProp.ClearArray();
            combinationsProp.arraySize = 0;
        }

        private void ClearSprites()
        {
            var combinationsProp = serializedObject.FindProperty(p_Combinations);
            for (int c = 0; c < combinationsProp.arraySize; c++)
            {
                var framesProp = combinationsProp.GetArrayElementAtIndex(c).FindPropertyRelative(p_TC_Frames);

                int numFrames = framesProp.arraySize;
                for (int i = 0; i < numFrames; i++)
                {
                    var frameProp = framesProp.GetArrayElementAtIndex(i);
                    var frame = frameProp.objectReferenceValue as Sprite;
                    if (!frame) continue;

                    frameProp.objectReferenceValue = null;
                    DestroyImmediate(frame, true);
                }
            }
        }

        public void ClearTextures()
        {
            var compiledChannelsProp = serializedObject.FindProperty(p_CompiledChannels);
            for (int c = 0; c < compiledChannelsProp.arraySize; c++)
            {
                var channelProp = compiledChannelsProp.GetArrayElementAtIndex(c);

                var channel = channelProp.objectReferenceValue as Texture2D;
                if (!channel) continue;

                DestroyImmediate(channel, true);

                channelProp.objectReferenceValue = null;
            }
        }


        private static TileSegment GetSegment(int tileSize, int quarter, int frame, int kind)
        {
            int baseX = frame * tileSize;
            int baseY = (4 - kind) * tileSize;
            int tileX = (quarter % 2 == 0) ? 0 : tileSize / 2;
            int tileY = (quarter / 2 == 0) ? 0 : tileSize / 2;
            int width = (quarter % 2 == 0) ? tileSize / 2 : tileSize - tileSize / 2;
            int height = (quarter / 2 == 0) ? tileSize / 2 : tileSize - tileSize / 2;

            return new TileSegment(baseX + tileX, baseY + tileY, width, height);
        }
    }


    public class TileDrawingItem
    {
        public TileCombinationSegment Segment { get; }
        public TemporaryTexture2DBuffer SourceBuffer { get; }

        public TileDrawingItem(TileCombinationSegment segment, TemporaryTexture2DBuffer sourceBuffer)
        {
            Segment = segment;
            SourceBuffer = sourceBuffer;
        }
    }

    public class TileCombinationSegment
    {
        public TileSegment Bl { get; }
        public TileSegment Br { get; }
        public TileSegment Tl { get; }
        public TileSegment Tr { get; }

        public SerializedProperty SpriteProperty { get; }

        public int PixelsPerUnit { get; }

        public int Width => Bl.W + Br.W;

        public int Height => Bl.H + Br.H;

        public TileCombinationSegment(TileSegment bl, TileSegment br, TileSegment tl, TileSegment tr,
            SerializedProperty spriteProperty, int pixelsPerUnit)
        {
            Bl = bl;
            Br = br;
            Tl = tl;
            Tr = tr;
            SpriteProperty = spriteProperty;
            PixelsPerUnit = pixelsPerUnit;
        }

        public void Copy(TemporaryTexture2DBuffer src, TemporaryTexture2DBuffer dst, int dstX, int dstY,
            bool expand)
        {
            Bl.Copy(src, dst, dstX, dstY);
            Br.Copy(src, dst, dstX + Bl.W, dstY);
            Tl.Copy(src, dst, dstX, dstY + Bl.H);
            Tr.Copy(src, dst, dstX + Bl.W, dstY + Bl.H);

            if (expand)
            {
                for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Color c = dst.Pixels[dst.Width * (dstY + y) + dstX + x];
                    if (y == 0) dst.Pixels[dst.Width * (dstY + y - 1) + dstX + x] = c;
                    if (y == Height - 1) dst.Pixels[dst.Width * (dstY + y + 1) + dstX + x] = c;
                    if (x == 0) dst.Pixels[dst.Width * (dstY + y) + dstX + x - 1] = c;
                    if (x == Width - 1) dst.Pixels[dst.Width * (dstY + y) + dstX + x + 1] = c;
                }

                dst.Pixels[dst.Width * (dstY - 1) + dstX - 1] = dst.Pixels[dst.Width * (dstY) + dstX];
                dst.Pixels[dst.Width * (dstY - 1) + dstX + Width] =
                    dst.Pixels[dst.Width * (dstY) + dstX + Width - 1];
                dst.Pixels[dst.Width * (dstY + Height) + dstX - 1] =
                    dst.Pixels[dst.Width * (dstY + Height - 1) + dstX];
                dst.Pixels[dst.Width * (dstY + Height) + dstX + Width] =
                    dst.Pixels[dst.Width * (dstY + Height - 1) + dstX + Width - 1];
            }
        }
    }

    [CustomPreview(typeof(FangAutoTile))]
    public class FangAutoTilePreview : ObjectPreview
    {
        private GUIContent previewTitle = new GUIContent("Generated Tiles");

        public override bool HasPreviewGUI()
        {
            var tile = target as FangAutoTile;
            if (!tile) return false;
            if (!tile.GetAllSprites().Any()) return false;
            return true;
        }

        public override GUIContent GetPreviewTitle()
        {
            return previewTitle;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            float padding = 5f;

            Rect previewRect = r;
            previewRect.width -= 40f;
            previewRect.height -= 40f;
            previewRect.x += 20f;
            previewRect.y += 20f;

            var tile = target as FangAutoTile;
            if (!tile) return;
            var sprites = tile.GetAllSprites();
            int maxCount = 120;
            int spritesCount = sprites.Count();

            int count = Mathf.Min(maxCount, spritesCount);
            int numX = Mathf.CeilToInt(Mathf.Sqrt(previewRect.width / previewRect.height * count));

            float gridSize = previewRect.width / numX;

            int i = 0;
            foreach (var s in sprites)
            {
                if (count <= i) break;

                int x = i % numX;
                int y = i / numX;

                var previewTexture = AssetPreview.GetAssetPreview(s);
                if (previewTexture)
                {
                    Rect texureRect = previewRect;
                    texureRect.width = gridSize - padding * 2f;
                    texureRect.height = gridSize - padding * 2f;
                    texureRect.x += x * gridSize + padding;
                    texureRect.y += y * gridSize + padding;

                    EditorGUI.DrawTextureTransparent(texureRect, previewTexture);
                }

                i++;
            }

            float labelHeight =
                new GUIStyle("PreOverlayLabel").CalcHeight(
                    new GUIContent($"Previewing {count} of {spritesCount} Objects"),
                    r.width);
            EditorGUI.DropShadowLabel(new Rect(r.x, r.yMax - labelHeight - 5f, r.width, labelHeight),
                $"Previewing {count} of {spritesCount} Objects");
        }
    }

    public class TileSegment
    {
        public int X { get; }
        public int Y { get; }
        public int W { get; }
        public int H { get; }

        public TileSegment(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public void Copy(TemporaryTexture2DBuffer src, TemporaryTexture2DBuffer dst, int dstX, int dstY)
        {
            //Debug.Log($"Copy segment src: ({X}, {Y}), dst: ({dstX}, {dstY}), size: ({W}, {H})");
            src.CopyTo(dst, X, Y, dstX, dstY, W, H);
        }
    }

    public class TemporaryTexture2DBuffer
    {
        public Texture2D Texture { get; }
        public Color[] Pixels { get; private set; }

        public int Width => Texture.width;
        public int Height => Texture.height;

        public TemporaryTexture2DBuffer(Texture2D texture)
        {
            Texture = texture;

            if (!texture.isReadable)
            {
                var path = AssetDatabase.GetAssetPath(texture);
                if (!string.IsNullOrEmpty(path))
                {
                    var importer = AssetImporter.GetAtPath(path);
                    if (importer is TextureImporter textureImporter)
                    {
                        textureImporter.isReadable = true;
                        textureImporter.SaveAndReimport();
                        Debug.LogWarning(
                            $"[FangAutoTile] \"Read/Write Enabled\" setting of \"{texture.name}\" has been turned on automatically.");
                    }
                }
            }

            Pixels = Texture.GetPixels();
        }

        public void ClearPixels()
        {
            Pixels = new Color[Width * Height];
        }

        public void Apply()
        {
            Texture.SetPixels(Pixels);
            Texture.Apply();
        }

        public void CopyTo(TemporaryTexture2DBuffer dst, int srcX, int srcY, int dstX, int dstY, int w, int h)
        {
            if (Width < srcX + w || Height < srcY + h) throw new InvalidOperationException();
            if (dst.Width < dstX + w || dst.Height < dstY + h) throw new InvalidOperationException();

            for (int y = 0; y < h; y++)
            {
                Array.Copy(Pixels, (srcY + y) * Width + srcX, dst.Pixels, (dstY + y) * dst.Width + dstX, w);
            }
        }
    }
}