﻿using System.Collections.Generic;
using Blamite.Blam;
using Blamite.Blam.Resources;
using System;

namespace Blamite.Injection
{
    /// <summary>
    ///     Contains information about an extracted resource's location.
    /// </summary>
    /// <seealso cref="ExtractedResourceInfo" />
    public class ExtractedResourcePointer
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractedResourcePointer" /> class.
        /// </summary>
        public ExtractedResourcePointer()
        {
            OriginalPrimaryPageIndex = -1;
            PrimaryOffset = -1;
            PrimarySize = null;
            OriginalSecondaryPageIndex = -1;
            SecondaryOffset = -1;
            SecondarySize = null;

            OriginalTertiaryPageIndex = -1;
            TertiaryOffset = -1;
            TertiarySize = null;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractedResourcePointer" /> class.
        /// </summary>
        /// <param name="basePointer">The original location information for the resource.</param>
        public ExtractedResourcePointer(ResourcePointer basePointer)
        {
            OriginalPrimaryPageIndex = (basePointer.PrimaryPage != null) ? basePointer.PrimaryPage.Index : -1;
            PrimaryOffset = basePointer.PrimaryOffset;
            PrimarySize = basePointer.PrimarySize;
            OriginalSecondaryPageIndex = (basePointer.SecondaryPage != null) ? basePointer.SecondaryPage.Index : -1;
            SecondaryOffset = basePointer.SecondaryOffset;
            SecondarySize = basePointer.SecondarySize;

            OriginalTertiaryPageIndex = (basePointer.TertiaryPage != null) ? basePointer.TertiaryPage.Index : -1;
            TertiaryOffset = basePointer.TertiaryOffset;
            TertiarySize = basePointer.TertiarySize;
        }

        /// <summary>
        ///     Gets or sets the original index of the resource's primary page.
        /// </summary>
        public int OriginalPrimaryPageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the offset of the resource in its primary page.
        /// </summary>
        public int PrimaryOffset { get; set; }

        /// <summary>
        ///     Gets or sets the original index of the resource's primary size.
        /// </summary>
        public ResourceSize PrimarySize { get; set; }

        /// <summary>
        ///     Gets or sets the original index of the resource's secondary page.
        /// </summary>
        public int OriginalSecondaryPageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the offset of the resource in its secondary page.
        /// </summary>
        public int SecondaryOffset { get; set; }

        /// <summary>
        ///     Gets or sets the original index of the resource's secondary size.
        /// </summary>
        public ResourceSize SecondarySize { get; set; }


        /// <summary>
        ///     Gets or sets the original index of the resource's secondary page.
        /// </summary>
        public int OriginalTertiaryPageIndex { get; set; }

        /// <summary>
        ///     Gets or sets the offset of the resource in its secondary page.
        /// </summary>
        public int TertiaryOffset { get; set; }

        /// <summary>
        ///     Gets or sets the original index of the resource's tertiary size.
        /// </summary>
        public ResourceSize TertiarySize { get; set; }
    }

    /// <summary>
    ///     Contains information about a resource link extracted from a cache file.
    /// </summary>
    public class ExtractedResourceInfo
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractedResourceInfo" /> class.
        /// </summary>
        /// <param name="originalIndex">The original datum index of the resource.</param>
        public ExtractedResourceInfo(DatumIndex originalIndex)
        {
            OriginalIndex = originalIndex;
            ResourceFixups = new List<ResourceFixup>();
            DefinitionFixups = new List<ResourceDefinitionFixup>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractedResourceInfo" /> class.
        /// </summary>
        /// <param name="originalIndex">The original datum index of the resource.</param>
        /// <param name="baseResource">The <see cref="Resource" /> to initialize the instance with.</param>
        public ExtractedResourceInfo(DatumIndex originalIndex, Resource baseResource)
        {
            if (originalIndex.Index != baseResource.Index.Index)
                throw new InvalidOperationException("originalIndex.Index != baseResource.Index.Index");

            OriginalIndex = originalIndex;
            Flags = baseResource.Flags;
            Type = baseResource.Type;
            Info = baseResource.Info;
            OriginalParentTagIndex = (baseResource.ParentTag != null) ? baseResource.ParentTag.Index : DatumIndex.Null;
            Location = (baseResource.Location != null) ? new ExtractedResourcePointer(baseResource.Location) : null;
            ResourceFixups = new List<ResourceFixup>(baseResource.ResourceFixups);
            DefinitionFixups = new List<ResourceDefinitionFixup>(baseResource.DefinitionFixups);
            ResourceBits = baseResource.ResourceBits;
            BaseDefinitionAddress = baseResource.BaseDefinitionAddress;
        }

        /// <summary>
        ///     Gets the original datum index used to reference the resource.
        /// </summary>
        public DatumIndex OriginalIndex { get; private set; }

        /// <summary>
        ///     Gets or sets flags for the resource.
        /// </summary>
        public uint Flags { get; set; }

        /// <summary>
        ///     Gets or sets the name of the resource's type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Gets or sets the info buffer for the resource.
        /// </summary>
        public byte[] Info { get; set; }

        /// <summary>
        ///     Gets or sets the original datum index of the tag associated with the resource.
        /// </summary>
        public DatumIndex OriginalParentTagIndex { get; set; }

        /// <summary>
        ///     Gets or sets information about the resource's original location.
        /// </summary>
        public ExtractedResourcePointer Location { get; set; }

        public List<ResourceFixup> ResourceFixups { get; private set; }
        public List<ResourceDefinitionFixup> DefinitionFixups { get; private set; }

        public int ResourceBits { get; set; }
        public int BaseDefinitionAddress { get; set; }
    }

    public class ExtractedResourcePredictionMolecule
    {
        public ExtractedResourcePredictionMolecule()
        {
            MoleculeAtoms = new List<ExtractedResourcePredictionMoleculeAtom>();
            Quantas = new List<ExtractedResourcePredictionQuanta>();
        }
        public ExtractedResourcePredictionMolecule(ResourcePredictionMolecule pred)
        {
            MoleculeAtoms = new List<ExtractedResourcePredictionMoleculeAtom>();
            Quantas = new List<ExtractedResourcePredictionQuanta>();

            OriginalIndex = pred.Index;
            OriginalTagIndex = pred.Tag.Index;
            Unknown1 = pred.Unknown1;
            Unknown2 = pred.Unknown2;
        }

        public DatumIndex OriginalTagIndex { get; set; }

        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }

        public List<ExtractedResourcePredictionMoleculeAtom> MoleculeAtoms { get; private set; }
        public List<ExtractedResourcePredictionQuanta> Quantas { get; private set; }

        public int OriginalIndex { get; set; }
    }

    public class ExtractedResourcePredictionMoleculeAtom
    {
        public ExtractedResourcePredictionMoleculeAtom() { }
        public ExtractedResourcePredictionMoleculeAtom(ResourcePredictionMoleculeAtom pred)
        {
            OriginalIndex = pred.Index;
            Salt = pred.Salt;
            BEntry = new ExtractedResourcePredictionAtom(pred.Atom);
        }

        public int OriginalIndex { get; set; }
        public ushort Salt { get; set; }
        public ExtractedResourcePredictionAtom BEntry { get; set; }
    }

    public class ExtractedResourcePredictionAtom
    {
        public ExtractedResourcePredictionAtom()
        {
            Quantas = new List<ExtractedResourcePredictionQuanta>();
        }
        public ExtractedResourcePredictionAtom(ResourcePredictionAtom pred)
        {
            Quantas = new List<ExtractedResourcePredictionQuanta>();

            OriginalIndex = pred.Index;
            OverallIndex = pred.OverallIndex;
        }

        public int OriginalIndex { get; set; }
        public short OverallIndex { get; set; }
        public List<ExtractedResourcePredictionQuanta> Quantas { get; private set; }
    }

    public class ExtractedResourcePredictionQuanta
    {
        public ExtractedResourcePredictionQuanta() { }
        public ExtractedResourcePredictionQuanta(ResourcePredictionQuanta pred)
        {
            OriginalIndex = pred.Index;
            OriginalResourceIndex = pred.Resource;
            OriginalResourceSubIndex = pred.SubResource;
        }

        public int OriginalIndex { get; set; }

        public DatumIndex OriginalResourceIndex { get; set; }

        public string OriginalResourceName { get; set; }
        public int OriginalResourceGroup { get; set; }

        public int OriginalResourceSubIndex { get; set; }
    }
}