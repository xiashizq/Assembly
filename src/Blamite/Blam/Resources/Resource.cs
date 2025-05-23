﻿using System.Collections.Generic;

namespace Blamite.Blam.Resources
{
    public class ResourceFixup
    {
        public int Offset { get; set; }
        public uint Address { get; set; }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return (int)(Offset ^ Address);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            var other = obj as ResourceFixup;
            if (other == null)
                return false;

            return (Offset == other.Offset && Address == other.Address);
        }
    }

    public class ResourceDefinitionFixup
    {
        public int Offset { get; set; }
        public int Type { get; set; }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return Offset ^ Type;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;

            var other = obj as ResourceDefinitionFixup;
            if (other == null)
                return false;

            return (Offset == other.Offset && Type == other.Type);
        }
    }

    public class ResourcePredictionMolecule
    {
        public ResourcePredictionMolecule()
        {
            MoleculeAtoms = new List<ResourcePredictionMoleculeAtom>();
            Quantas = new List<ResourcePredictionQuanta>();
        }

        public ITag Tag { get; set; }

        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }

        public List<ResourcePredictionMoleculeAtom> MoleculeAtoms { get; private set; }
        public List<ResourcePredictionQuanta> Quantas { get; private set; }

        public int Index { get; set; }

        public long GetMoleculeAtomHash()
        {
            long result = 9103;
            for (int i = 0; i < MoleculeAtoms.Count; i++)
                for (int k = 0; k < MoleculeAtoms[i].Atom.Quantas.Count; k++)
                    result = result * 8171 + MoleculeAtoms[i].Atom.Quantas[k].Value.Value;

            return result;
        }

        public bool IsEmpty
        {
            get
            {
                return (MoleculeAtoms == null || MoleculeAtoms.Count == 0) &&
                    (Quantas == null || Quantas.Count == 0);
            }
        }
    }

    public class ResourcePredictionMoleculeAtom
    {
        public ushort Salt { get; set; }
        public ResourcePredictionAtom Atom { get; set; }

        public int Index { get; set; }

        public long GetAtomHash()
        {
            return (Atom.GetQuantaHash() << 1) * 5233;
        }

        public bool IsEmpty
        {
            get { return Atom == null || Atom.IsEmpty; }
        }
    }

    public class ResourcePredictionAtom
    {
        public ResourcePredictionAtom()
        {
            Quantas = new List<ResourcePredictionQuanta>();
        }

        public short OverallIndex { get; set; }
        public List<ResourcePredictionQuanta> Quantas { get; private set; }

        public int Index { get; set; }

        public long GetQuantaHash()
        {
            long result = 7057;
            for (int i = 0; i < Quantas.Count; i++)
                result = result * 8171 + (Quantas[i].Value.Value << i);

            return result;
        }

        public bool IsEmpty
        {
            get { return Quantas == null || Quantas.Count == 0; }
        }
    }

    public class ResourcePredictionQuanta
    {
        public DatumIndex Value { get; set; }

        public DatumIndex Resource { get; set; }
        public int SubResource { get; set; }

        public int Index { get; set; }
    }

    /// <summary>
    ///     A resource in a cache file.
    /// </summary>
    public class Resource
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Resource" /> class.
        /// </summary>
        public Resource()
        {
            ResourceFixups = new List<ResourceFixup>();
            DefinitionFixups = new List<ResourceDefinitionFixup>();
        }

        /// <summary>
        ///     Gets or sets the datum index of the resource.
        /// </summary>
        public DatumIndex Index { get; set; }

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
        ///     Gets or sets the tag associated with the resource.
        /// </summary>
        public ITag ParentTag { get; set; }

        /// <summary>
        ///     Gets or sets information about the resource's location.
        /// </summary>
        public ResourcePointer Location { get; set; }

        public List<ResourceFixup> ResourceFixups { get; private set; }
        public List<ResourceDefinitionFixup> DefinitionFixups { get; private set; }

        public int ResourceBits { get; set; }
        public int BaseDefinitionAddress { get; set; }
    }
}