using System.IO;
using Assimp;

namespace GK.Utils
{
    public class ModelLoader
    {
        public static Model LoadFromFile(string filePath, PostProcessSteps ppSteps)
        {
            if (!File.Exists(filePath))
                return null;

            AssimpContext importer = new AssimpContext();

            Scene scene = importer.ImportFile(filePath, ppSteps);
            if (scene == null)
                return null;
            var model = new Model();

            foreach (var mesh in scene.Meshes)
            {
                var processed = Mesh.processMesh(mesh);
                model.meshes.Add(processed);
            }

            return model;
        }
    }
}