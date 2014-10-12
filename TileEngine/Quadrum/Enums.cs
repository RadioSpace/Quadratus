using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quadrum
{
    /// <summary>
    /// identifies the type or types of shdaers to use
    /// </summary>
    [Flags] public enum ShaderTypes 
    {
        /// <summary>
        /// the Vertex Shader Stage
        /// </summary>
        Vertex = 1,
        /// <summary>
        /// the Hul Shader Stage
        /// </summary>
        Hull = 2, 
        /// <summary>
        /// the Domain Shader Stage
        /// </summary>
        Domain = 4, 
        /// <summary>
        /// the Pixel or Fragment Shader Satge
        /// </summary>
        Pixel = 8
    };
}
