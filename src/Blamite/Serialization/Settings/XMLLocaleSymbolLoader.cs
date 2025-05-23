﻿using System;
using System.Text;
using System.Xml.Linq;
using Blamite.Util;

namespace Blamite.Serialization.Settings
{
    /// <summary>
    ///     Loads locale symbol data from XML files.
    /// </summary>
    public class XMLLocaleSymbolLoader : IComplexSettingLoader
    {
        /// <summary>
        ///     Loads setting data from a path.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <returns>
        ///     The loaded setting data.
        /// </returns>
        public object LoadSetting(string path)
        {
            return LoadLocaleSymbols(path);
        }

        /// <summary>
        ///     Loads setting data from a path, with a secondary path.
        /// </summary>
        /// <param name="path">The path to load from.</param>
        /// <param name="altPath">The alternate path to load from.</param>
        /// <returns>The loaded setting data.</returns>
        public object LoadSetting(string path, string altPath)
        {
            //alt not used
            return LoadSetting(path);
        }

        /// <summary>
        ///     Loads all of the locale symbols defined in an XML document.
        /// </summary>
        /// <param name="document">The XML document to load locale symbols from.</param>
        /// <param name="path">The path to the original XML. For debugging purposes.</param>
        /// <returns>The symbols that were loaded.</returns>
        public static LocaleSymbolCollection LoadLocaleSymbols(XDocument document, string path)
        {
            // Make sure there is a root <symbols> tag
            XContainer container = document.Element("symbols");
            if (container == null)
                throw new ArgumentException($"Invalid symbols document.\r\n{path}");

            // Symbol tags have the format:
            // <symbol unicode="0x(hex code for character)" display="(engine name)" />
            var result = new LocaleSymbolCollection();
            foreach (XElement symbol in container.Elements("symbol"))
            {
                string code = XMLUtil.GetStringAttribute(symbol, "unicode").Replace("0x", "");
                string display = XMLUtil.GetStringAttribute(symbol, "display");

                if (!short.TryParse(code, System.Globalization.NumberStyles.HexNumber, null, out short codeVal))
                    throw new ArgumentException($"Invalid unicode value \"{code}\" for display value \"{display}\".\r\n{path}");

                result.AddSymbol((char)codeVal, display);
            }
            return result;
        }

        /// <summary>
        ///     Loads all of the locale symbols defined in an XML document.
        /// </summary>
        /// <param name="documentPath">The path to the XML document to load.</param>
        /// <returns>The symbols that were loaded.</returns>
        public static LocaleSymbolCollection LoadLocaleSymbols(string documentPath)
        {
            return LoadLocaleSymbols(XDocument.Load(documentPath), documentPath);
        }
    }
}