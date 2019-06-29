using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace NastyEngine
{
    public class Shader
    {
        private Effect shader;
        public Shader(string shaderName) { shader = ResourceManager.GetEffect(shaderName); }

        public void SetUniform<T>(string uniform,T value)
        {
            shader.Parameters[uniform].SetValue(true);
        }

        public Effect GetShader()
        {
            return shader;
        }

        public void ApplyShader()
        {
            //SetUniform("", 1);
            for (int i = 0; i < shader.CurrentTechnique.Passes.Count; i++)
                shader.CurrentTechnique.Passes[i].Apply();
        }

        public void ApplyPass(int p)
        {
            shader.CurrentTechnique.Passes[p].Apply();
        }
    }
}
