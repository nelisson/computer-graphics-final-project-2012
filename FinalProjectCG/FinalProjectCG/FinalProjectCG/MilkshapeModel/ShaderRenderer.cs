namespace MilkshapeViewer.MilkshapeModel
{
    using System;
    using System.Collections.Generic;
    using FinalProjectCG.MilkshapeModel;

    public class ShaderRenderer
    {
        public List<ShaderBatch> Batches = new List<ShaderBatch>();

        public void BuildBatches(MilkshapeModel model)
        {
            //primul triunghi creaza un batch
            ShaderBatch s = new ShaderBatch();
            MilkshapeTriangle t = model.Triangles[0];
            Batches.Add(s);
            AddToBatch(model, s, t);
            //triangle vertices must be rendered togheter
            for (int i = 0; i < model.numTriangles; i++)
            {
                //cautam un batch care sa contina toate bone-urile
                ShaderBatch sb = FindBatch(model.Triangles[i]);
                if (sb != null)
                {
                    AddToBatch(model, sb, model.Triangles[i]);
                }
                else
                {
                    sb = FindBestMatch(model.Triangles[i]);
                    if (sb != null)
                    {
                        AddToBatch(model, sb, model.Triangles[i]);
                    }
                    else
                    {
                        ShaderBatch sbnew = new ShaderBatch();
                        AddToBatch(model, sb, model.Triangles[i]);
                        Batches.Add(sbnew);
                    }
                }
            }
        }

        private ShaderBatch FindBestMatch(MilkshapeTriangle milkshapeTriangle)
        {
            throw new NotImplementedException();
        }

        private static void AddToBatch(MilkshapeModel model, ShaderBatch s, MilkshapeTriangle t)
        {
            if (model.Vertices[t.vertexIndices[0]].boneIDs[0] != -1)
            {
                s.BoneIds.Add(model.Vertices[model.Triangles[0].vertexIndices[0]].boneIDs[0]);
            }
            if (model.Vertices[t.vertexIndices[0]].boneIDs[1] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[0]].boneIDs[1]);
            }
            if (model.Vertices[t.vertexIndices[0]].boneIDs[2] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[0]].boneIDs[2]);
            }
            if (model.Vertices[t.vertexIndices[0]].boneIDs[3] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[0]].boneIDs[3]);
            }
            if (model.Vertices[t.vertexIndices[1]].boneIDs[0] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[1]].boneIDs[0]);
            }
            if (model.Vertices[t.vertexIndices[1]].boneIDs[1] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[1]].boneIDs[1]);
            }
            if (model.Vertices[t.vertexIndices[1]].boneIDs[2] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[1]].boneIDs[2]);
            }
            if (model.Vertices[t.vertexIndices[1]].boneIDs[3] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[1]].boneIDs[3]);
            }
            if (model.Vertices[t.vertexIndices[2]].boneIDs[0] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[2]].boneIDs[0]);
            }
            if (model.Vertices[t.vertexIndices[2]].boneIDs[1] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[2]].boneIDs[1]);
            }
            if (model.Vertices[t.vertexIndices[2]].boneIDs[2] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[2]].boneIDs[2]);
            }
            if (model.Vertices[t.vertexIndices[2]].boneIDs[3] != -1)
            {
                s.BoneIds.Add(model.Vertices[t.vertexIndices[2]].boneIDs[3]);
            }
        }

        private ShaderBatch FindBatch(MilkshapeTriangle milkshapeTriangle)
        {
            for (int i = 0; i < Batches.Count; i++)
            {
                //if (
            }
            return null;
        }
    }
}