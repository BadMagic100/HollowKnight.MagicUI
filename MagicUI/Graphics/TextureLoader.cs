using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MagicUI.Graphics
{
    /// <summary>
    /// Streamlines loading textures embedded in your assembly for usage with UI elements, such as buttons, borders, or images
    /// </summary>
    public class TextureLoader
    {
        private readonly string resourceNamespace;
        private readonly Assembly asm;
        private readonly Dictionary<string, Texture2D> textures = new();

        /// <summary>
        /// Creates a texture loader for a given namespace
        /// </summary>
        /// <param name="asm">The assembly to load from</param>
        /// <param name="resourceNamespace">The namespace to load the resource from</param>
        public TextureLoader(Assembly asm, string resourceNamespace)
        {
            this.asm = asm;
            this.resourceNamespace = resourceNamespace;
        }

        /// <summary>
        /// Internal helper to load a texture
        /// </summary>
        /// <param name="resourceName">The filename of the texture</param>
        /// <returns>The loaded texture</returns>
        private Texture2D LoadEmbeddedTexture(string resourceName)
        {
            using Stream imageStream = asm.GetManifestResourceStream(resourceName);
            byte[] buffer = new byte[imageStream.Length];
            imageStream.Read(buffer, 0, buffer.Length);

            Texture2D tex = new(1, 1);
            tex.LoadImage(buffer.ToArray());

            return tex;
        }

        /// <summary>
        /// Preloads all textures in this loader's namespace
        /// </summary>
        public void Preload()
        {
            if (textures.Count != 0)
            {
                throw new InvalidOperationException("You can only preload images once, and only before manually loading any textures.");
            }
            foreach (string longName in asm.GetManifestResourceNames())
            {
                if (!longName.StartsWith($"{resourceNamespace}.")) continue;
                IEnumerable<string> fragments = longName.Split('.');
                string shortName = string.Join(".", fragments.Skip(fragments.Count() - 2).ToArray());
                textures[shortName] = LoadEmbeddedTexture(longName);
            }
        }

        /// <summary>
        /// Gets the texture for the given file name, loading it first if needed
        /// </summary>
        /// <param name="name">The filename of the image to load</param>
        /// <returns>The loaded texture</returns>
        public Texture2D GetTexture(string name)
        {
            if (!textures.ContainsKey(name))
            {
                string longName = $"{resourceNamespace}.{name}";
                if (asm.GetManifestResourceNames().Contains(longName))
                {
                    textures[name] = LoadEmbeddedTexture(longName);
                }
                else
                {
                    throw new FileNotFoundException("No resource with the given name. Make sure the file exists and is compiled as an embedded resource.");
                }
            }
            return textures[name];
        }

        /// <summary>
        /// Cleans up the texture with the given file name if it is loaded
        /// </summary>
        /// <param name="name">The filename of the image to dispose</param>
        public void DisposeTexture(string name)
        {
            if (textures.ContainsKey(name))
            {
                UnityEngine.Object.Destroy(textures[name]);
                textures.Remove(name);
            }
        }
    }
}
