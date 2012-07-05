namespace FinalProjectCG.MilkshapeModel
{
    using System;
    using System.IO;
    using System.Security.AccessControl;
    using System.Windows.Forms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class MilkshapeModel
    {
        private MilkshapeHeader _header;
        public short NumVertices;
        public MilkshapeVertex[] Vertices;
        public short NumTriangles;
        public MilkshapeTriangle[] Triangles;
        public short NumGroups;

        public MilkshapeGroup[] Groups;
        public short NumMaterials;
        public MilkshapeMaterial[] Materials;
        public float AnimFPS;
        private float _currentTime;
        private int _totalFrames;
        private short _numJoints;
        private MilkshapeJoint[] _joints;

        private void LoadMS3DFromFile(string fileName, GraphicsDevice gd)
        {
            var fs = File.Open(fileName, FileMode.Open);
            var br = new BinaryReader(fs);

            _header = new MilkshapeHeader {Id = br.ReadChars(10), Version = br.ReadInt32()};

            NumVertices = br.ReadInt16();
            Vertices = new MilkshapeVertex[NumVertices];
            for (int i = 0; i < NumVertices; i++)
            {
                Vertices[i] = new MilkshapeVertex {Flags = br.ReadByte(), Vertex = new float[3]};
                Vertices[i].Vertex[0] = br.ReadSingle();
                Vertices[i].Vertex[1] = br.ReadSingle();
                Vertices[i].Vertex[2] = br.ReadSingle();
                Vertices[i].BoneId = (char) br.ReadByte();
                Vertices[i].ReferenceCount = br.ReadByte();
            }
            NumTriangles = br.ReadInt16();
            Triangles = new MilkshapeTriangle[NumTriangles];
            for (int i = 0; i < NumTriangles; i++)
            {
                Triangles[i] = new MilkshapeTriangle {Flags = br.ReadInt16(), VertexIndices = new short[3]};
                Triangles[i].VertexIndices[0] = br.ReadInt16();
                Triangles[i].VertexIndices[1] = br.ReadInt16();
                Triangles[i].VertexIndices[2] = br.ReadInt16();

                Triangles[i].VertexNormals = new float[3][];
                Triangles[i].VertexNormals[0] = new float[3];
                Triangles[i].VertexNormals[1] = new float[3];
                Triangles[i].VertexNormals[2] = new float[3];
                Triangles[i].VertexNormals[0][0] = br.ReadSingle();
                Triangles[i].VertexNormals[0][1] = br.ReadSingle();
                Triangles[i].VertexNormals[0][2] = br.ReadSingle();
                Triangles[i].VertexNormals[1][0] = br.ReadSingle();
                Triangles[i].VertexNormals[1][1] = br.ReadSingle();
                Triangles[i].VertexNormals[1][2] = br.ReadSingle();
                Triangles[i].VertexNormals[2][0] = br.ReadSingle();
                Triangles[i].VertexNormals[2][1] = br.ReadSingle();
                Triangles[i].VertexNormals[2][2] = br.ReadSingle();

                Triangles[i].S = new float[3];
                Triangles[i].T = new float[3];

                Triangles[i].S[0] = br.ReadSingle();
                Triangles[i].S[1] = br.ReadSingle();
                Triangles[i].S[2] = br.ReadSingle();
                Triangles[i].T[0] = br.ReadSingle();
                Triangles[i].T[1] = br.ReadSingle();
                Triangles[i].T[2] = br.ReadSingle();
                Triangles[i].SmoothingGroup = br.ReadByte();
                Triangles[i].GroupIndex = br.ReadByte();
            }
            NumGroups = br.ReadInt16();
            Groups = new MilkshapeGroup[NumGroups];
            for (int i = 0; i < NumGroups; i++)
            {
                Groups[i] = new MilkshapeGroup
                                {Flags = br.ReadByte(), Name = br.ReadChars(32), NumTriangles = br.ReadInt16()};
                Groups[i].TriangleIndices = new short[Groups[i].NumTriangles];
                for (int j = 0; j < Groups[i].NumTriangles; j++)
                    Groups[i].TriangleIndices[j] = br.ReadInt16();
                Groups[i].MaterialIndex = br.ReadSByte();
            }
            NumMaterials = br.ReadInt16();
            Materials = new MilkshapeMaterial[NumMaterials];
            for (int i = 0; i < NumMaterials; i++)
            {
                Materials[i] = new MilkshapeMaterial
                                   {
                                       Name = br.ReadChars(32),
                                       Ambient = new float[4],
                                       Diffuse = new float[4],
                                       Emissive = new float[4],
                                       Specular = new float[4]
                                   };

                Materials[i].Ambient[0] = br.ReadSingle();
                Materials[i].Ambient[1] = br.ReadSingle();
                Materials[i].Ambient[2] = br.ReadSingle();
                Materials[i].Ambient[3] = br.ReadSingle();

                Materials[i].Diffuse[0] = br.ReadSingle();
                Materials[i].Diffuse[1] = br.ReadSingle();
                Materials[i].Diffuse[2] = br.ReadSingle();
                Materials[i].Diffuse[3] = br.ReadSingle();

                Materials[i].Specular[0] = br.ReadSingle();
                Materials[i].Specular[1] = br.ReadSingle();
                Materials[i].Specular[2] = br.ReadSingle();
                Materials[i].Specular[3] = br.ReadSingle();

                Materials[i].Emissive[0] = br.ReadSingle();
                Materials[i].Emissive[1] = br.ReadSingle();
                Materials[i].Emissive[2] = br.ReadSingle();
                Materials[i].Emissive[3] = br.ReadSingle();

                Materials[i].Shininess = br.ReadSingle();
                Materials[i].Transparency = br.ReadSingle();
                Materials[i].Mode = br.ReadChar();

                string s = "";
                Materials[i].TextureFN = br.ReadBytes(128);
                Materials[i].SpheremapFN = br.ReadBytes(128);
                for (int j = 0; j < 128; j++)
                {
                    if (Materials[i].TextureFN[j] == 0)
                        break;
                    s = s + (char) Materials[i].TextureFN[j];
                }
                Materials[i].TextureS = s;
                if (s.Trim() != "")
                {
                    try
                    {
                        s = s.Substring(s.LastIndexOf("/") + 1);
                        Materials[i].Texture = Texture2D.FromStream(gd, new FileStream(fileName.Split(new[] { '\\' })[0] + "\\" + s, FileMode.Open,FileAccess.Read,FileShare.Read));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                s = "";
                for (int j = 0; j < 128; j++)
                {
                    if (Materials[i].SpheremapFN[j] == 0)
                        break;
                    s = s + (char) Materials[i].SpheremapFN[j];
                }
                Materials[i].SphereMapS = s;
                s = s.Substring(s.LastIndexOf("/") + 1);
                if (s.Trim() != "")
                {
                    try
                    {
                        Materials[i].Spheremap = Texture2D.FromStream(gd, new FileStream(fileName.Split(new[] {'\\'})[0]+"\\" + s, FileMode.Open));
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            AnimFPS = br.ReadSingle();
            _currentTime = br.ReadSingle();
            _totalFrames = br.ReadInt32();
            _animBoundMax = _totalFrames;
            _numJoints = br.ReadInt16();
            _joints = new MilkshapeJoint[_numJoints];
            for (int i = 0; i < _numJoints; i++)
            {
                _joints[i] = new MilkshapeJoint
                                 {
                                     Flags = br.ReadByte(),
                                     Name = br.ReadChars(32),
                                     ParentName = br.ReadChars(32),
                                     NameS = ""
                                 };
                for (int k = 0; k < 32; k++)
                {
                    if (_joints[i].Name[k] == (char) 0)
                        break;
                    _joints[i].NameS += _joints[i].Name[k];
                }
                _joints[i].ParentNameS = "";
                for (int k = 0; k < 32; k++)
                {
                    if (_joints[i].ParentName[k] == (char) 0)
                        break;
                    _joints[i].ParentNameS += _joints[i].ParentName[k];
                }
                _joints[i].Rotation = new float[3];
                _joints[i].Position = new float[3];
                _joints[i].Rotation[0] = br.ReadSingle();
                _joints[i].Rotation[1] = br.ReadSingle();
                _joints[i].Rotation[2] = br.ReadSingle();
                _joints[i].Position[0] = br.ReadSingle();
                _joints[i].Position[1] = br.ReadSingle();
                _joints[i].Position[2] = br.ReadSingle();
                _joints[i].NumRotKeyFrames = br.ReadInt16();
                _joints[i].NumPosKeyFrames = br.ReadInt16();
                _joints[i].RotKeyFrames = new MilkshapeRotationKey[_joints[i].NumRotKeyFrames];
                _joints[i].PosKeyFrames = new MilkshapePositionKey[_joints[i].NumPosKeyFrames];
                for (int k = 0; k < _joints[i].NumRotKeyFrames; k++)
                {
                    _joints[i].RotKeyFrames[k] = new MilkshapeRotationKey
                                                     {Time = br.ReadSingle()*AnimFPS, Rotation = new float[3]};
                    _joints[i].RotKeyFrames[k].Rotation[0] = br.ReadSingle();
                    _joints[i].RotKeyFrames[k].Rotation[1] = br.ReadSingle();
                    _joints[i].RotKeyFrames[k].Rotation[2] = br.ReadSingle();
                }
                for (int k = 0; k < _joints[i].NumPosKeyFrames; k++)
                {
                    _joints[i].PosKeyFrames[k] = new MilkshapePositionKey
                                                     {Time = br.ReadSingle()*AnimFPS, Position = new float[3]};
                    _joints[i].PosKeyFrames[k].Position[0] = br.ReadSingle();
                    _joints[i].PosKeyFrames[k].Position[1] = br.ReadSingle();
                    _joints[i].PosKeyFrames[k].Position[2] = br.ReadSingle();
                }
            }

            if (fs.Position < fs.Length)
            {
                int subversion = br.ReadInt32();
                if (subversion == 1)
                {
                    int numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        br.ReadInt32();
                        int index = br.ReadInt32();
                        if (index > 0)
                            br.ReadChars(index);
                    }
                    numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        br.ReadInt32();
                        int index = br.ReadInt32();
                        if (index > 0)
                            br.ReadChars(index);
                    }
                    numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        br.ReadInt32();
                        int index = br.ReadInt32();
                        if (index > 0)
                            br.ReadChars(index);
                    }
                    numComments = br.ReadInt32();
                    if (numComments == 1)
                    {
                        int index = br.ReadInt32();
                        br.ReadChars(index);
                    }
                }
            }
            if (fs.Position < fs.Length)
            {
                int subversion = br.ReadInt32();
                for (int i = 0; i < NumVertices; i++)
                {
                    Vertices[i].BoneIDs = new int[3];
                    Vertices[i].BoneWeights = new int[3];
                    if ((subversion == 1) || (subversion == 2))
                    {
                        Vertices[i].BoneIDs[0] = br.ReadSByte();
                        Vertices[i].BoneIDs[1] = br.ReadSByte();
                        Vertices[i].BoneIDs[2] = br.ReadSByte();
                        Vertices[i].BoneWeights[0] = br.ReadSByte();
                        Vertices[i].BoneWeights[1] = br.ReadSByte();
                        Vertices[i].BoneWeights[2] = br.ReadSByte();
                        if (subversion == 2)
                            br.ReadInt32();
                    }
                }
            }
            _firstRun = true;
            RebuildVertices();
            SetupJoints();
            br.Close();
            fs.Close();
        }

        public void RebuildVertices()
        {
            for (int i = 0; i < NumGroups; i++)
            {
                Groups[i].NumVertices = Groups[i].NumTriangles*3;
                Groups[i].Vertices = new CustomVertexStruct[Groups[i].NumVertices];
                Groups[i].OriginalVertices = new CustomVertexStruct[Groups[i].NumVertices];

                for (int k = 0; k < Groups[i].NumTriangles; k++)
                {
                    Groups[i].Vertices[k*3 + 0].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].Vertex[0];
                    Groups[i].Vertices[k*3 + 0].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].Vertex[1];
                    Groups[i].Vertices[k*3 + 0].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].Vertex[2];
                    Groups[i].Vertices[k*3 + 0].Normal.X = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[0][0];
                    Groups[i].Vertices[k*3 + 0].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[0][1];
                    Groups[i].Vertices[k*3 + 0].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[0][2];
                    Groups[i].Vertices[k*3 + 0].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].S[0];
                    Groups[i].Vertices[k*3 + 0].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].T[0];
                    Groups[i].Vertices[k*3 + 0].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneId;
                    var tmp = new MilkshapeJointAndWeight();
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneIDs[0];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneIDs[1];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneIDs[2];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneWeights[0];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneWeights[1];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X = tmp.JointIndices[0];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y = tmp.JointIndices[1];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z = tmp.JointIndices[2];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.W = tmp.JointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneId;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y = 0;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z = 0;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.W = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.X = 1;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Y = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Z = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.W = 0;
                    }


                    Groups[i].Vertices[k*3 + 1].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].Vertex[0];
                    Groups[i].Vertices[k*3 + 1].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].Vertex[1];
                    Groups[i].Vertices[k*3 + 1].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].Vertex[2];
                    Groups[i].Vertices[k*3 + 1].Normal.X = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[1][0];
                    Groups[i].Vertices[k*3 + 1].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[1][1];
                    Groups[i].Vertices[k*3 + 1].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[1][2];
                    Groups[i].Vertices[k*3 + 1].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].S[1];
                    Groups[i].Vertices[k*3 + 1].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].T[1];
                    Groups[i].Vertices[k*3 + 1].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneId;
                    Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneId;
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneIDs[0];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneIDs[1];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneIDs[2];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneWeights[0];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneWeights[1];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X = tmp.JointIndices[0];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y = tmp.JointIndices[1];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z = tmp.JointIndices[2];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.W = tmp.JointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[1]].BoneId;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y = 0;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z = 0;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.W = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.X = 1;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Y = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Z = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.W = 0;
                    }

                    Groups[i].Vertices[k*3 + 2].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].Vertex[0];
                    Groups[i].Vertices[k*3 + 2].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].Vertex[1];
                    Groups[i].Vertices[k*3 + 2].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].Vertex[2];
                    Groups[i].Vertices[k*3 + 2].Normal.X = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[2][0];
                    Groups[i].Vertices[k*3 + 2].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[2][1];
                    Groups[i].Vertices[k*3 + 2].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].VertexNormals[2][2];
                    Groups[i].Vertices[k*3 + 2].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].S[2];
                    Groups[i].Vertices[k*3 + 2].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].T[2];
                    Groups[i].Vertices[k*3 + 2].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneId;
                    Groups[i].Vertices[k*3 + 2].BoneIDs.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[0]].BoneId;
                    Groups[i].Vertices[k*3 + 2].BoneWeights.X = 1;
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs[0];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs[1];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneIDs[2];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneWeights[0];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneWeights[1];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 2].BoneIDs.X = tmp.JointIndices[0];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Y = tmp.JointIndices[1];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Z = tmp.JointIndices[2];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.W = tmp.JointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 2].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].VertexIndices[2]].BoneId;
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Y = 0;
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Z = 0;
                        Groups[i].Vertices[k*3 + 2].BoneIDs.W = 0;
                        Groups[i].Vertices[k*3 + 2].BoneWeights.X = 1;
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Y = 0;
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Z = 0;
                        Groups[i].Vertices[k*3 + 2].BoneWeights.W = 0;
                    }
                    Groups[i].OriginalVertices[k*3 + 0] = Groups[i].Vertices[k*3 + 0];
                    Groups[i].OriginalVertices[k*3 + 1] = Groups[i].Vertices[k*3 + 1];
                    Groups[i].OriginalVertices[k*3 + 2] = Groups[i].Vertices[k*3 + 2];
                }
            }
        }

        private float _animBoundMin;
        private float _animBoundMax;

        public float GetAnimBoundMin()
        {
            return _animBoundMin;
        }

        public float GetAnimBoundMax()
        {
            return _animBoundMax;
        }

        private void R_ConcatTransforms(MilkshapeMatrix3X4 in1, MilkshapeMatrix3X4 in2, MilkshapeMatrix3X4 o)
        {
            o.V[0][0] = in1.V[0][0]*in2.V[0][0] + in1.V[0][1]*in2.V[1][0] +
                        in1.V[0][2]*in2.V[2][0];
            o.V[0][1] = in1.V[0][0]*in2.V[0][1] + in1.V[0][1]*in2.V[1][1] +
                        in1.V[0][2]*in2.V[2][1];
            o.V[0][2] = in1.V[0][0]*in2.V[0][2] + in1.V[0][1]*in2.V[1][2] +
                        in1.V[0][2]*in2.V[2][2];
            o.V[0][3] = in1.V[0][0]*in2.V[0][3] + in1.V[0][1]*in2.V[1][3] +
                        in1.V[0][2]*in2.V[2][3] + in1.V[0][3];
            o.V[1][0] = in1.V[1][0]*in2.V[0][0] + in1.V[1][1]*in2.V[1][0] +
                        in1.V[1][2]*in2.V[2][0];
            o.V[1][1] = in1.V[1][0]*in2.V[0][1] + in1.V[1][1]*in2.V[1][1] +
                        in1.V[1][2]*in2.V[2][1];
            o.V[1][2] = in1.V[1][0]*in2.V[0][2] + in1.V[1][1]*in2.V[1][2] +
                        in1.V[1][2]*in2.V[2][2];
            o.V[1][3] = in1.V[1][0]*in2.V[0][3] + in1.V[1][1]*in2.V[1][3] +
                        in1.V[1][2]*in2.V[2][3] + in1.V[1][3];
            o.V[2][0] = in1.V[2][0]*in2.V[0][0] + in1.V[2][1]*in2.V[1][0] +
                        in1.V[2][2]*in2.V[2][0];
            o.V[2][1] = in1.V[2][0]*in2.V[0][1] + in1.V[2][1]*in2.V[1][1] +
                        in1.V[2][2]*in2.V[2][1];
            o.V[2][2] = in1.V[2][0]*in2.V[0][2] + in1.V[2][1]*in2.V[1][2] +
                        in1.V[2][2]*in2.V[2][2];
            o.V[2][3] = in1.V[2][0]*in2.V[0][3] + in1.V[2][1]*in2.V[1][3] +
                        in1.V[2][2]*in2.V[2][3] + in1.V[2][3];
        }

        private Vector4 QuaternionSlerp(Vector4 p, Vector4 q, float t)
        {
            float sclp, sclq;
            Vector4 qt;
            float a = 0;
            float b = 0;
            a += (float) Math.Pow(p.X - q.X, 2);
            a += (float) Math.Pow(p.Y - q.Y, 2);
            a += (float) Math.Pow(p.Z - q.Z, 2);
            a += (float) Math.Pow(p.W - q.W, 2);
            b += (float) Math.Pow(p.X + q.X, 2);
            b += (float) Math.Pow(p.Y + q.Y, 2);
            b += (float) Math.Pow(p.Z + q.Z, 2);
            b += (float) Math.Pow(p.W + q.W, 2);
            if (a > b)
            {
                q.X = -q.X;
                q.Y = -q.Y;
                q.Z = -q.Z;
                q.W = -q.W;
            }

            float cosom = p.X*q.X + p.Y*q.Y + p.Z*q.Z + p.W*q.W;

            if ((1.0 + cosom) > 0.00000001)
            {
                if ((1.0 - cosom) > 0.00000001)
                {
                    var omega = (float) Math.Acos(cosom);
                    var sinom = (float) Math.Sin(omega);
                    sclp = (float) Math.Sin((1.0 - t)*omega)/sinom;
                    sclq = (float) Math.Sin(t*omega)/sinom;
                }
                else
                {
                    sclp = 1.0f - t;
                    sclq = t;
                }
                qt.X = sclp*p.X + sclq*q.X;
                qt.Y = sclp*p.Y + sclq*q.Y;
                qt.Z = sclp*p.Z + sclq*q.Z;
                qt.W = sclp*p.W + sclq*q.W;
            }
            else
            {
                qt.X = -p.Y;
                qt.Y = p.X;
                qt.Z = -p.W;
                qt.W = p.Z;
                sclp = (float) Math.Sin((1.0 - t)*0.5*Math.PI);
                sclq = (float) Math.Sin(t*0.5*Math.PI);
                qt.X = sclp*p.X + sclq*qt.X;
                qt.Y = sclp*p.Y + sclq*qt.Y;
                qt.Z = sclp*p.Z + sclq*qt.Z;
            }
            return qt;
        }

        private Vector4 AngleQuaternion(Vector3 angles)
        {
            Vector4 quaternion;
            var angle = angles.Z*0.5f;
            var sy = (float) Math.Sin(angle);
            var cy = (float) Math.Cos(angle);
            angle = angles.Y*0.5f;
            var sp = (float) Math.Sin(angle);
            var cp = (float) Math.Cos(angle);
            angle = angles.X*0.5f;
            var sr = (float) Math.Sin(angle);
            var cr = (float) Math.Cos(angle);

            quaternion.X = sr*cp*cy - cr*sp*sy; // X
            quaternion.Y = cr*sp*cy + sr*cp*sy; // Y
            quaternion.Z = cr*cp*sy - sr*sp*cy; // Z
            quaternion.W = cr*cp*cy + sr*sp*sy; // W
            return quaternion;
        }

        private void QuaternionMatrix(Vector4 quaternion, MilkshapeMatrix3X4 matrix)
        {
            matrix.V[0][0] = (float) (1.0f - 2.0*quaternion.Y*quaternion.Y - 2.0*quaternion.Z*quaternion.Z);
            matrix.V[1][0] = (float) (2.0f*quaternion.X*quaternion.Y + 2.0*quaternion.W*quaternion.Z);
            matrix.V[2][0] = (float) (2.0f*quaternion.X*quaternion.Z - 2.0*quaternion.W*quaternion.Y);

            matrix.V[0][1] = (float) (2.0f*quaternion.X*quaternion.Y - 2.0*quaternion.W*quaternion.Z);
            matrix.V[1][1] = (float) (1.0f - 2.0*quaternion.X*quaternion.X - 2.0*quaternion.Z*quaternion.Z);
            matrix.V[2][1] = (float) (2.0f*quaternion.Y*quaternion.Z + 2.0*quaternion.W*quaternion.X);

            matrix.V[0][2] = (float) (2.0f*quaternion.X*quaternion.Z + 2.0*quaternion.W*quaternion.Y);
            matrix.V[1][2] = (float) (2.0f*quaternion.Y*quaternion.Z - 2.0*quaternion.W*quaternion.X);
            matrix.V[2][2] = (float) (1.0f - 2.0*quaternion.X*quaternion.X - 2.0*quaternion.Y*quaternion.Y);
        }

        private int FindJointByName(String name)
        {
            for (int i = 0; i < _numJoints; i++)
                if (_joints[i].NameS == name)
                    return i;
            return -1;
        }

        private void AngleMatrix(Vector3 angles, MilkshapeMatrix3X4 matrix)
        {
            var angle = angles.Z;
            var sy = (float) Math.Sin(angle);
            var cy = (float) Math.Cos(angle);
            angle = angles.Y;
            var sp = (float) Math.Sin(angle);
            var cp = (float) Math.Cos(angle);
            angle = angles.X;
            var sr = (float) Math.Sin(angle);
            var cr = (float) Math.Cos(angle);

            matrix.V[0][0] = cp*cy;
            matrix.V[1][0] = cp*sy;
            matrix.V[2][0] = -sp;
            matrix.V[0][1] = sr*sp*cy + cr*-sy;
            matrix.V[1][1] = sr*sp*sy + cr*cy;
            matrix.V[2][1] = sr*cp;
            matrix.V[0][2] = (cr*sp*cy + -sr*-sy);
            matrix.V[1][2] = (cr*sp*sy + -sr*cy);
            matrix.V[2][2] = cr*cp;
            matrix.V[0][3] = 0.0f;
            matrix.V[1][3] = 0.0f;
            matrix.V[2][3] = 0.0f;
        }

        private void SetupJoints()
        {
            for (int i = 0; i < _numJoints; i++)
            {
                _joints[i].ParentIndex = FindJointByName(_joints[i].ParentNameS);
            }
            for (int i = 0; i < _numJoints; i++)
            {
                var tmp = new Vector3(_joints[i].Rotation[0], _joints[i].Rotation[1], _joints[i].Rotation[2]);
                _joints[i].MatLocalSkeleton = new MilkshapeMatrix3X4();
                AngleMatrix(tmp, _joints[i].MatLocalSkeleton);
                _joints[i].MatLocalSkeleton.V[0][3] = _joints[i].Position[0];
                _joints[i].MatLocalSkeleton.V[1][3] = _joints[i].Position[1];
                _joints[i].MatLocalSkeleton.V[2][3] = _joints[i].Position[2];

                if (_joints[i].ParentIndex == -1)
                {
                    _joints[i].MatGlobalSkeleton = new MilkshapeMatrix3X4();
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                            _joints[i].MatGlobalSkeleton.V[k][l] = _joints[i].MatLocalSkeleton.V[k][l];
                }
                else
                {
                    _joints[i].MatGlobalSkeleton = new MilkshapeMatrix3X4();
                    R_ConcatTransforms(_joints[_joints[i].ParentIndex].MatGlobalSkeleton, _joints[i].MatLocalSkeleton,
                                       _joints[i].MatGlobalSkeleton);
                }

                SetupTangents();
            }
        }

        private void SetupTangents()
        {
            for (int j = 0; j < _numJoints; j++)
            {
                var joint = _joints[j];
                int numPositionKeys = joint.NumPosKeyFrames;
                joint.Tangents = new MilkshapeTangent[numPositionKeys];

                for (int k = 0; k < numPositionKeys; k++)
                {
                    joint.Tangents[k] = new MilkshapeTangent
                                            {
                                                TangentIn = {X = 0.0f, Y = 0.0f, Z = 0.0f},
                                                TangentOut = {X = 0.0f, Y = 0.0f, Z = 0.0f}
                                            };
                }

                if (numPositionKeys > 2)
                {
                    for (int k = 0; k < numPositionKeys; k++)
                    {
                        int k0 = k - 1;
                        if (k0 < 0)
                            k0 = numPositionKeys - 1;
                        int k1 = k;
                        int k2 = k + 1;
                        if (k2 >= numPositionKeys)
                            k2 = 0;
                        var tangent = new float[3];
                        tangent[0] = (joint.PosKeyFrames[k2].Position[0] - joint.PosKeyFrames[k0].Position[0]);
                        tangent[1] = (joint.PosKeyFrames[k2].Position[1] - joint.PosKeyFrames[k0].Position[1]);
                        tangent[2] = (joint.PosKeyFrames[k2].Position[2] - joint.PosKeyFrames[k0].Position[2]);

                        float dt1 = joint.PosKeyFrames[k1].Time - joint.PosKeyFrames[k0].Time;
                        float dt2 = joint.PosKeyFrames[k2].Time - joint.PosKeyFrames[k1].Time;
                        float dt = dt1 + dt2;
                        joint.Tangents[k1].TangentIn.X = tangent[0]*dt1/dt;
                        joint.Tangents[k1].TangentIn.Y = tangent[1]*dt1/dt;
                        joint.Tangents[k1].TangentIn.Z = tangent[2]*dt1/dt;

                        joint.Tangents[k1].TangentOut.X = tangent[0]*dt2/dt;
                        joint.Tangents[k1].TangentOut.Y = tangent[1]*dt2/dt;
                        joint.Tangents[k1].TangentOut.Z = tangent[2]*dt2/dt;
                    }
                }
            }
        }

        private void RebuildJoint(MilkshapeJoint joint)
        {
            float frame = _currentTime;
            var pos = new Vector3(0f, 0f, 0f);
            int numPositionKeys = joint.NumPosKeyFrames;
            if (numPositionKeys > 0)
            {
                int i1 = -1;
                int i2 = -1;

                for (int i = 0; i < (numPositionKeys - 1); i++)
                {
                    if (frame >= joint.PosKeyFrames[i].Time && frame < joint.PosKeyFrames[i + 1].Time)
                    {
                        i1 = i;
                        i2 = i + 1;
                        break;
                    }
                }

                if (i1 == -1 || i2 == -1)
                {
                    if (frame < joint.PosKeyFrames[0].Time)
                    {
                        pos.X = joint.PosKeyFrames[0].Position[0];
                        pos.Y = joint.PosKeyFrames[0].Position[1];
                        pos.Z = joint.PosKeyFrames[0].Position[2];
                    }

                    else if (frame >= joint.PosKeyFrames[numPositionKeys - 1].Time)
                    {
                        pos.X = joint.PosKeyFrames[numPositionKeys - 1].Position[0];
                        pos.Y = joint.PosKeyFrames[numPositionKeys - 1].Position[1];
                        pos.Z = joint.PosKeyFrames[numPositionKeys - 1].Position[2];
                    }
                }

                else
                {
                    MilkshapePositionKey p0 = joint.PosKeyFrames[i1];
                    MilkshapePositionKey p1 = joint.PosKeyFrames[i2];
                    MilkshapeTangent m0 = joint.Tangents[i1];
                    MilkshapeTangent m1 = joint.Tangents[i2];

                    float t = (frame - joint.PosKeyFrames[i1].Time)/
                              (joint.PosKeyFrames[i2].Time - joint.PosKeyFrames[i1].Time);
                    float t2 = t*t;
                    float t3 = t2*t;

                    float h1 = 2.0f*t3 - 3.0f*t2 + 1.0f;
                    float h2 = -2.0f*t3 + 3.0f*t2;
                    float h3 = t3 - 2.0f*t2 + t;
                    float h4 = t3 - t2;

                    pos.X = h1*p0.Position[0] + h3*m0.TangentOut.X + h2*p1.Position[0] + h4*m1.TangentIn.X;
                    pos.Y = h1*p0.Position[1] + h3*m0.TangentOut.Y + h2*p1.Position[1] + h4*m1.TangentIn.Y;
                    pos.Z = h1*p0.Position[2] + h3*m0.TangentOut.Z + h2*p1.Position[2] + h4*m1.TangentIn.Z;
                }
            }

            var quat = new Vector4(0f, 0f, 0f, 1f);
            var numRotationKeys = (int) joint.NumRotKeyFrames;
            if (numRotationKeys > 0)
            {
                int i1 = -1;
                int i2 = -1;

                for (int i = 0; i < (numRotationKeys - 1); i++)
                {
                    if (frame >= joint.RotKeyFrames[i].Time && frame < joint.RotKeyFrames[i + 1].Time)
                    {
                        i1 = i;
                        i2 = i + 1;
                        break;
                    }
                }

                if (i1 == -1 || i2 == -1)
                {
                    if (frame < joint.RotKeyFrames[0].Time)
                    {
                        var tmp = new Vector3(joint.RotKeyFrames[0].Rotation[0], joint.RotKeyFrames[0].Rotation[1],
                                                  joint.RotKeyFrames[0].Rotation[2]);
                        quat = AngleQuaternion(tmp);
                    }

                    else if (frame >= joint.RotKeyFrames[numRotationKeys - 1].Time)
                    {
                        var tmp = new Vector3(joint.RotKeyFrames[numRotationKeys - 1].Rotation[0],
                                                  joint.RotKeyFrames[numRotationKeys - 1].Rotation[1],
                                                  joint.RotKeyFrames[numRotationKeys - 1].Rotation[2]);
                        quat = AngleQuaternion(tmp);
                    }
                }

                else
                {
                    float t = (frame - joint.RotKeyFrames[i1].Time)/
                              (joint.RotKeyFrames[i2].Time - joint.RotKeyFrames[i1].Time);
                    var tmp = new Vector3(joint.RotKeyFrames[i1].Rotation[0], joint.RotKeyFrames[i1].Rotation[1],
                                              joint.RotKeyFrames[i1].Rotation[2]);
                    var q1 = AngleQuaternion(tmp);
                    tmp = new Vector3(joint.RotKeyFrames[i2].Rotation[0], joint.RotKeyFrames[i2].Rotation[1],
                                      joint.RotKeyFrames[i2].Rotation[2]);
                    var q2 = AngleQuaternion(tmp);
                    quat = QuaternionSlerp(q1, q2, t);
                }
            }

            var matAnimate = new MilkshapeMatrix3X4();
            QuaternionMatrix(quat, matAnimate);
            matAnimate.V[0][3] = pos.X;
            matAnimate.V[1][3] = pos.Y;
            matAnimate.V[2][3] = pos.Z;

            if (joint.MatLocal == null)
                joint.MatLocal = new MilkshapeMatrix3X4();
            R_ConcatTransforms(joint.MatLocalSkeleton, matAnimate, joint.MatLocal);

            if (joint.ParentIndex == -1)
            {
                if (joint.MatGlobal == null)
                    joint.MatGlobal = new MilkshapeMatrix3X4();
                for (int k = 0; k < 3; k++)
                    for (int l = 0; l < 4; l++)
                        joint.MatGlobal.V[k][l] = joint.MatLocal.V[k][l];
            }
            else
            {
                MilkshapeJoint parentJoint = _joints[joint.ParentIndex];
                if (joint.MatGlobal == null)
                    joint.MatGlobal = new MilkshapeMatrix3X4();
                if (parentJoint.MatGlobal == null)
                    parentJoint.MatGlobal = new MilkshapeMatrix3X4();
                R_ConcatTransforms(parentJoint.MatGlobal, joint.MatLocal, joint.MatGlobal);
            }
        }

        private void FillJointIndicesAndWeights(CustomVertexStruct vertex, MilkshapeJointAndWeight jw)
        {
            jw.JointIndices[0] = vertex.BoneId;
            jw.JointIndices[1] = (int) vertex.BoneIDs.X;
            jw.JointIndices[2] = (int) vertex.BoneIDs.Y;
            jw.JointIndices[3] = (int) vertex.BoneIDs.Z;
            jw.JointWeights[0] = 100;
            jw.JointWeights[1] = 0;
            jw.JointWeights[2] = 0;
            jw.JointWeights[3] = 0;
            if (vertex.BoneWeights.X != 0 || vertex.BoneWeights.Y != 0 || vertex.BoneWeights.Z != 0)
            {
                jw.JointWeights[0] = (int) vertex.BoneWeights.X;
                jw.JointWeights[1] = (int) vertex.BoneWeights.Y;
                jw.JointWeights[2] = (int) vertex.BoneWeights.Z;
                jw.JointWeights[3] = 100 - (int) (vertex.BoneWeights.X + vertex.BoneWeights.Y + vertex.BoneWeights.Z);
            }
        }

        private Vector3 VectorIRotate(Vector3 in1, MilkshapeMatrix3X4 in2)
        {
            Vector3 outVert;
            outVert.X = in1.X*in2.V[0][0] + in1.Y*in2.V[1][0] + in1.Z*in2.V[2][0];
            outVert.Y = in1.X*in2.V[0][1] + in1.Y*in2.V[1][1] + in1.Z*in2.V[2][1];
            outVert.Z = in1.X*in2.V[0][2] + in1.Y*in2.V[1][2] + in1.Z*in2.V[2][2];
            return outVert;
        }

        private Vector3 VectorTransform(Vector3 in1, MilkshapeMatrix3X4 in2)
        {
            Vector3 outVert;
            outVert.X = (in1.X*in2.V[0][0] + in1.Y*in2.V[0][1] + in1.Z*in2.V[0][2]) + in2.V[0][3];
            outVert.Y = (in1.X*in2.V[1][0] + in1.Y*in2.V[1][1] + in1.Z*in2.V[1][2]) + in2.V[1][3];
            outVert.Z = (in1.X*in2.V[2][0] + in1.Y*in2.V[2][1] + in1.Z*in2.V[2][2]) + in2.V[2][3];

            return outVert;
        }

        private Vector3 VectorITransform(Vector3 in1, MilkshapeMatrix3X4 in2)
        {
            Vector3 tmp;
            tmp.X = in1.X - in2.V[0][3];
            tmp.Y = in1.Y - in2.V[1][3];
            tmp.Z = in1.Z - in2.V[2][3];
            Vector3 outVect = VectorIRotate(tmp, in2);
            return outVect;
        }

        private CustomVertexStruct TransformVertex(CustomVertexStruct vertex)
        {
            var jw = new MilkshapeJointAndWeight();
            CustomVertexStruct outVertex = vertex;
            jw.JointIndices[0] = (int) outVertex.BoneIDs.X;
            jw.JointIndices[1] = (int) outVertex.BoneIDs.Y;
            jw.JointIndices[2] = (int) outVertex.BoneIDs.Z;
            jw.JointIndices[3] = (int) outVertex.BoneIDs.W;
            jw.JointWeights[0] = (int) (outVertex.BoneWeights.X*100);
            jw.JointWeights[1] = (int) (outVertex.BoneWeights.Y*100);
            jw.JointWeights[2] = (int) (outVertex.BoneWeights.Z*100);
            jw.JointWeights[3] = (int) (outVertex.BoneWeights.W*100);

            if (jw.JointIndices[0] < 0 || jw.JointIndices[0] >= _numJoints || _currentTime < 0.0f)
            {
            }
            else
            {
                int numWeights = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (jw.JointWeights[i] > 0 && jw.JointIndices[i] >= 0 && jw.JointIndices[i] < _numJoints)
                        ++numWeights;
                    else
                        break;
                }

                outVertex.Position.X = 0.0f;
                outVertex.Position.Y = 0.0f;
                outVertex.Position.Z = 0.0f;
                outVertex.Normal.X = 0.0f;
                outVertex.Normal.Y = 0.0f;
                outVertex.Normal.Z = 0.0f;

                var weights = new float[4];
                weights[0] = jw.JointWeights[0]/100.0f;
                weights[1] = jw.JointWeights[1]/100.0f;
                weights[2] = jw.JointWeights[2]/100.0f;
                weights[3] = jw.JointWeights[3]/100.0f;

                if (numWeights == 0)
                {
                    numWeights = 1;
                    weights[0] = 1.0f;
                }
                for (int i = 0; i < numWeights; i++)
                {
                    MilkshapeJoint joint = _joints[jw.JointIndices[i]];
                    Vector3 tmp = VectorITransform(vertex.Position, joint.MatGlobalSkeleton);
                    Vector3 vert = VectorTransform(tmp, joint.MatGlobal);

                    outVertex.Position.X += vert.X*weights[i];
                    outVertex.Position.Y += vert.Y*weights[i];
                    outVertex.Position.Z += vert.Z*weights[i];

                    tmp = VectorIRotate(vertex.Normal, joint.MatGlobalSkeleton);
                    Vector3 norm = VectorTransform(tmp, joint.MatGlobal);
                    outVertex.Normal.X += norm.X;
                    outVertex.Normal.Y += norm.Y;
                    outVertex.Normal.Z += norm.Z;
                }
            }
            return outVertex;
        }

        public void RebuildJoints()
        {
            if (_currentTime < 0.0f)
            {
                for (int i = 0; i < _numJoints; i++)
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                        {
                            _joints[i].MatGlobal.V[k][l] = _joints[i].MatGlobalSkeleton.V[k][l];
                            _joints[i].MatLocalSkeleton.V[k][l] = _joints[i].MatLocal.V[k][l];
                        }
            }
            else
                for (int i = 0; i < _numJoints; i++)
                {
                    RebuildJoint(_joints[i]);
                }
        }

        private bool _firstRun;

        public delegate void Stopped();
        public event Stopped StoppedAnimation;

        public void OnStoppedAnimation()
        {   
            var handler = StoppedAnimation;
            if (handler != null) 
                handler();
        }

        public bool Reverse { get; set; }

        public void AdvanceAnimation(float delta)
        {
            if (Reverse)
            {
                _currentTime -= delta;

                if (_currentTime < _animBoundMin)
                {
                    OnStoppedAnimation();
                }
            }
            else
            {

                _currentTime += delta;

                if (_currentTime > _animBoundMax)
                {
                    OnStoppedAnimation();
                }
            }

            if (_firstRun)
            {
                _firstRun = false;
                for (int i = 0; i < _numJoints; i++)
                {
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                        {
                            if (_joints[i].MatLocalSkeleton == null)
                                _joints[i].MatLocalSkeleton = new MilkshapeMatrix3X4();
                            if (_joints[i].MatGlobalSkeleton == null)
                                _joints[i].MatGlobalSkeleton = new MilkshapeMatrix3X4();
                            if (_joints[i].MatLocal == null)
                                _joints[i].MatLocal = new MilkshapeMatrix3X4();
                            if (_joints[i].MatGlobal == null)
                                _joints[i].MatGlobal = new MilkshapeMatrix3X4();
                            _joints[i].MatLocal.V[k][l] = _joints[i].MatLocalSkeleton.V[k][l];
                            _joints[i].MatGlobal.V[k][l] = _joints[i].MatGlobalSkeleton.V[k][l];
                        }
                }
            }
            else
                RebuildJoints();
                for (int i = 0; i < NumGroups; i++)
                {
                    for (int j = 0; j < Groups[i].Vertices.Length; j++)
                    {
                        Groups[i].Vertices[j] = TransformVertex(Groups[i].OriginalVertices[j]);
                    }
                }
        }

        public void SetAnimBounds(float minFrame, float maxFrame, bool reverse = false)
        {
            _animBoundMin = minFrame;
            _animBoundMax = maxFrame;

            Reverse = reverse;

            if (reverse)
            {
                if ((_currentTime < _animBoundMin) || (_currentTime > _animBoundMax))
                {
                    _currentTime = _animBoundMax;
                }
            }
            else
            {
                if ((_currentTime < _animBoundMin) || (_currentTime > _animBoundMax))
                {
                    _currentTime = _animBoundMin;
                }    
            }
            
        }

        private readonly GraphicsDevice _gd;

        public MilkshapeModel(string fileName, GraphicsDevice gd)
        {
            _gd = gd;
            LoadMS3DFromFile(fileName, gd);
        }

        public short MaterialIndex
        {
            set
            {
                foreach (var milkshapeGroup in Groups)
                {
                    milkshapeGroup.MaterialIndex = value;
                }
            }
        }

        public static VertexDeclaration Vd = new VertexDeclaration(CustomVertexStruct.VertexElements);
        public BasicEffect BasicEffect
        {
            get { return _basicEffect ?? (_basicEffect = new BasicEffect(_gd)); }
            set { _basicEffect = value; }
        }
        private BasicEffect _basicEffect;
        
        public void Render()
        {
            _gd.RasterizerState = RasterizerState.CullClockwise;
            
            BasicEffect.TextureEnabled = true;
            for (int i = 0; i < NumGroups; i++)
            {
                _gd.Textures[0] = Materials[Groups[i].MaterialIndex].Texture;
                BasicEffect.Texture = (Texture2D) Materials[Groups[i].MaterialIndex].Texture;

                foreach (EffectPass p in BasicEffect.CurrentTechnique.Passes)
                {
                    p.Apply();

                    _gd.DrawUserPrimitives(PrimitiveType.TriangleList, Groups[i].Vertices, 0,
                                                              Groups[i].NumTriangles, Vd);
                }
            }
        }
    }
}
