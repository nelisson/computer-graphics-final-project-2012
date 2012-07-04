/*
 * Created by SharpDevelop.
 * User: Tibi
 * Date: 3/14/2008
 * Time: 1:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace FinalProjectCG.MilkshapeModel
{
    using System;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class MilkshapeModel
    {
        //suport incaracare ms3d
        //toate declaratiile sunt private - numai dupa traducerea in directX vor fi disponibile end-user
        //pentru debug declaram totul aici, urmand ca apoi sa curatam clasa
        private MilkshapeHeader Header;
        public short numVertices;
        public MilkshapeVertex[] Vertices;
        public short numTriangles;
        public MilkshapeTriangle[] Triangles;
        public short numGroups;

        public MilkshapeGroup[] Groups;
        public short numMaterials;
        public MilkshapeMaterial[] Materials;
        public float animFPS;
        private float currentTime;
        private int totalFrames;
        private short numJoints;
        private MilkshapeJoint[] Joints;

        private void loadMS3DFromFile(string FileName, GraphicsDevice gd)
        {
            //deschidere fisier
            FileStream fs = File.Open(FileName, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);

            //citim headerul
            Header = new MilkshapeHeader();
            Header.id = br.ReadChars(10);
            Header.version = br.ReadInt32();

            //citim numarul de puncte
            numVertices = br.ReadInt16();
            //citim punctele
            Vertices = new MilkshapeVertex[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                Vertices[i] = new MilkshapeVertex();
                Vertices[i].flags = br.ReadByte();
                Vertices[i].vertex = new float[3];
                Vertices[i].vertex[0] = br.ReadSingle();
                Vertices[i].vertex[1] = br.ReadSingle();
                Vertices[i].vertex[2] = br.ReadSingle();
                Vertices[i].boneId = (char) br.ReadByte();
                Vertices[i].referenceCount = br.ReadByte();
            }
            //citim numarul de triunghiuri
            numTriangles = br.ReadInt16();
            Triangles = new MilkshapeTriangle[numTriangles];
            for (int i = 0; i < numTriangles; i++)
            {
                Triangles[i] = new MilkshapeTriangle();
                Triangles[i].flags = br.ReadInt16();
                Triangles[i].vertexIndices = new short[3];
                Triangles[i].vertexIndices[0] = br.ReadInt16();
                Triangles[i].vertexIndices[1] = br.ReadInt16();
                Triangles[i].vertexIndices[2] = br.ReadInt16();

                Triangles[i].vertexNormals = new float[3][];
                Triangles[i].vertexNormals[0] = new float[3];
                Triangles[i].vertexNormals[1] = new float[3];
                Triangles[i].vertexNormals[2] = new float[3];
                Triangles[i].vertexNormals[0][0] = br.ReadSingle();
                Triangles[i].vertexNormals[0][1] = br.ReadSingle();
                Triangles[i].vertexNormals[0][2] = br.ReadSingle();
                Triangles[i].vertexNormals[1][0] = br.ReadSingle();
                Triangles[i].vertexNormals[1][1] = br.ReadSingle();
                Triangles[i].vertexNormals[1][2] = br.ReadSingle();
                Triangles[i].vertexNormals[2][0] = br.ReadSingle();
                Triangles[i].vertexNormals[2][1] = br.ReadSingle();
                Triangles[i].vertexNormals[2][2] = br.ReadSingle();

                Triangles[i].s = new float[3];
                Triangles[i].t = new float[3];

                Triangles[i].s[0] = br.ReadSingle();
                Triangles[i].s[1] = br.ReadSingle();
                Triangles[i].s[2] = br.ReadSingle();
                Triangles[i].t[0] = br.ReadSingle();
                Triangles[i].t[1] = br.ReadSingle();
                Triangles[i].t[2] = br.ReadSingle();
                Triangles[i].smoothingGroup = br.ReadByte();
                Triangles[i].groupIndex = br.ReadByte();
            }
            //citim numarul de grupuri
            numGroups = br.ReadInt16();
            Groups = new MilkshapeGroup[numGroups];
            for (int i = 0; i < numGroups; i++)
            {
                Groups[i] = new MilkshapeGroup();
                Groups[i].Flags = br.ReadByte();
                Groups[i].Name = br.ReadChars(32);
                Groups[i].NumTriangles = br.ReadInt16();
                Groups[i].TriangleIndices = new short[Groups[i].NumTriangles];
                for (int j = 0; j < Groups[i].NumTriangles; j++)
                    Groups[i].TriangleIndices[j] = br.ReadInt16();
                Groups[i].MaterialIndex = br.ReadSByte();
            }
            numMaterials = br.ReadInt16();
            Materials = new MilkshapeMaterial[numMaterials];
            for (int i = 0; i < numMaterials; i++)
            {
                Materials[i] = new MilkshapeMaterial();
                Materials[i].name = br.ReadChars(32);
                Materials[i].ambient = new float[4];
                Materials[i].diffuse = new float[4];
                Materials[i].emissive = new float[4];
                Materials[i].specular = new float[4];

                Materials[i].ambient[0] = br.ReadSingle();
                Materials[i].ambient[1] = br.ReadSingle();
                Materials[i].ambient[2] = br.ReadSingle();
                Materials[i].ambient[3] = br.ReadSingle();

                Materials[i].diffuse[0] = br.ReadSingle();
                Materials[i].diffuse[1] = br.ReadSingle();
                Materials[i].diffuse[2] = br.ReadSingle();
                Materials[i].diffuse[3] = br.ReadSingle();

                Materials[i].specular[0] = br.ReadSingle();
                Materials[i].specular[1] = br.ReadSingle();
                Materials[i].specular[2] = br.ReadSingle();
                Materials[i].specular[3] = br.ReadSingle();

                Materials[i].emissive[0] = br.ReadSingle();
                Materials[i].emissive[1] = br.ReadSingle();
                Materials[i].emissive[2] = br.ReadSingle();
                Materials[i].emissive[3] = br.ReadSingle();

                Materials[i].shininess = br.ReadSingle();
                Materials[i].transparency = br.ReadSingle();
                Materials[i].mode = br.ReadChar();

                string s = "";
                Materials[i].textureFN = br.ReadBytes(128);
                Materials[i].spheremapFN = br.ReadBytes(128);
                for (int j = 0; j < 128; j++)
                {
                    if (Materials[i].textureFN[j] == 0)
                        break;
                    s = s + (char) Materials[i].textureFN[j];
                }
                Materials[i].textureS = s;
                if (s.Trim() != "")
                {
                    try
                    {
                        s = s.Substring(s.LastIndexOf("/") + 1);
                        Materials[i].texture = Texture2D.FromStream(gd, new FileStream(FileName.Split(new Char[] { '\\' })[0] + "\\" + s, FileMode.Open));
                            //TextureLoader.FromFile(dev, s);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
                s = "";
                for (int j = 0; j < 128; j++)
                {
                    if (Materials[i].spheremapFN[j] == 0)
                        break;
                    s = s + (char) Materials[i].spheremapFN[j];
                }
                Materials[i].sphereMapS = s;
                s = s.Substring(s.LastIndexOf("/") + 1);
                if (s.Trim() != "")
                {
                    try
                    {
                        Materials[i].spheremap = Texture2D.FromStream(gd, new FileStream(FileName.Split(new Char [] {'\\'})[0]+"\\" + s, FileMode.Open));
                            //TextureLoader.FromFile(dev, s);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
                }
            }

            //incarcare bone-uri si keyframe-uri
            animFPS = br.ReadSingle();
            currentTime = br.ReadSingle();
            totalFrames = br.ReadInt32();
            animBoundMax = totalFrames;
            numJoints = br.ReadInt16();
            Joints = new MilkshapeJoint[numJoints];
            for (int i = 0; i < numJoints; i++)
            {
                Joints[i] = new MilkshapeJoint();
                Joints[i].flags = br.ReadByte();
                Joints[i].name = br.ReadChars(32);
                Joints[i].parentName = br.ReadChars(32);
                Joints[i].nameS = "";
                for (int k = 0; k < 32; k++)
                {
                    if (Joints[i].name[k] == (char) 0)
                        break;
                    Joints[i].nameS += Joints[i].name[k];
                }
                Joints[i].parentNameS = "";
                for (int k = 0; k < 32; k++)
                {
                    if (Joints[i].parentName[k] == (char) 0)
                        break;
                    Joints[i].parentNameS += Joints[i].parentName[k];
                }
                Joints[i].rotation = new float[3];
                Joints[i].position = new float[3];
                Joints[i].rotation[0] = br.ReadSingle();
                Joints[i].rotation[1] = br.ReadSingle();
                Joints[i].rotation[2] = br.ReadSingle();
                Joints[i].position[0] = br.ReadSingle();
                Joints[i].position[1] = br.ReadSingle();
                Joints[i].position[2] = br.ReadSingle();
                Joints[i].numRotKeyFrames = br.ReadInt16();
                Joints[i].numPosKeyFrames = br.ReadInt16();
                Joints[i].rotKeyFrames = new MilkshapeRotationKey[Joints[i].numRotKeyFrames];
                Joints[i].posKeyFrames = new MilkshapePositionKey[Joints[i].numPosKeyFrames];
                for (int k = 0; k < Joints[i].numRotKeyFrames; k++)
                {
                    Joints[i].rotKeyFrames[k] = new MilkshapeRotationKey();
                    Joints[i].rotKeyFrames[k].time = br.ReadSingle()*animFPS;
                    Joints[i].rotKeyFrames[k].rotation = new float[3];
                    Joints[i].rotKeyFrames[k].rotation[0] = br.ReadSingle();
                    Joints[i].rotKeyFrames[k].rotation[1] = br.ReadSingle();
                    Joints[i].rotKeyFrames[k].rotation[2] = br.ReadSingle();
                }
                for (int k = 0; k < Joints[i].numPosKeyFrames; k++)
                {
                    Joints[i].posKeyFrames[k] = new MilkshapePositionKey();
                    Joints[i].posKeyFrames[k].time = br.ReadSingle()*animFPS;
                    Joints[i].posKeyFrames[k].position = new float[3];
                    Joints[i].posKeyFrames[k].position[0] = br.ReadSingle();
                    Joints[i].posKeyFrames[k].position[1] = br.ReadSingle();
                    Joints[i].posKeyFrames[k].position[2] = br.ReadSingle();
                }
            }

            //pentru animatii care sa tina cont de influenta mai multor oase asupra unui vertex
            //comentarii
            if (fs.Position < fs.Length)
            {
                int subversion = br.ReadInt32();
                if (subversion == 1)
                {
                    //comentarii pe grupuri
                    int numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        //nu salvam nimic pentru ca nu ne intereseaza
                        int index;
                        index = br.ReadInt32();
                        index = br.ReadInt32();
                        if (index > 0)
                            br.ReadChars(index);
                    }
                    //comentarii pe materiale
                    numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        //nu salvam nimic pentru ca nu ne intereseaza
                        int index;
                        index = br.ReadInt32();
                        index = br.ReadInt32();
                        if (index > 0)
                            br.ReadChars(index);
                    }
                    //comentarii pe jointuri
                    numComments = br.ReadInt32();
                    for (int i = 0; i < numComments; i++)
                    {
                        //nu salvam nimic pentru ca nu ne intereseaza
                        int index;
                        index = br.ReadInt32();
                        index = br.ReadInt32();
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
                for (int i = 0; i < numVertices; i++)
                {
                    Vertices[i].boneIDs = new int[3];
                    Vertices[i].boneWeights = new int[3];
                    if ((subversion == 1) || (subversion == 2))
                    {
                        Vertices[i].boneIDs[0] = br.ReadSByte();
                        Vertices[i].boneIDs[1] = br.ReadSByte();
                        Vertices[i].boneIDs[2] = br.ReadSByte();
                        Vertices[i].boneWeights[0] = br.ReadSByte();
                        Vertices[i].boneWeights[1] = br.ReadSByte();
                        Vertices[i].boneWeights[2] = br.ReadSByte();
                        //pentru compatibilitate
                        if (subversion == 2)
                            br.ReadInt32();
                    }
                }
            }
            //creare schelet si chestii pentru animatie
            FirstRun = true;
            //SetupJoints();
            //creare model
            rebuildVertices();
            SetupJoints();
            br.Close();
            fs.Close();
        }

        /// <summary>
        /// reconstruim fiecare group dupa citirea din fisier
        /// </summary>
        public void rebuildVertices()
        {
            int cnt = 0;
            for (int i = 0; i < numGroups; i++)
            {
                Groups[i].NumVertices = Groups[i].NumTriangles*3;
                Groups[i].Vertices = new CustomVertexStruct[Groups[i].NumVertices];
                Groups[i].OriginalVertices = new CustomVertexStruct[Groups[i].NumVertices];
                MilkshapeGroup group = Groups[i];
                for (int k = 0; k < Groups[i].NumTriangles; k++)
                {
                    cnt += 3;
                    Groups[i].Vertices[k*3 + 0].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].vertex[0];
                    Groups[i].Vertices[k*3 + 0].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].vertex[1];
                    Groups[i].Vertices[k*3 + 0].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].vertex[2];
                    Groups[i].Vertices[k*3 + 0].Normal.X = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[0][0];
                    Groups[i].Vertices[k*3 + 0].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[0][1];
                    Groups[i].Vertices[k*3 + 0].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[0][2];
                    Groups[i].Vertices[k*3 + 0].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].s[0];
                    Groups[i].Vertices[k*3 + 0].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].t[0];
                    Groups[i].Vertices[k*3 + 0].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneId;
                    //ms3d_group[i].vertices[k * 3 + 0].boneWeights.X = 1;
                    MilkshapeJointAndWeight tmp = new MilkshapeJointAndWeight();
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneIDs[0];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneIDs[1];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneIDs[2];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneWeights[0];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneWeights[1];
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X = tmp.jointIndices[0];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y = tmp.jointIndices[1];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z = tmp.jointIndices[2];
                        Groups[i].Vertices[k*3 + 0].BoneIDs.W = tmp.jointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 0].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneId;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Y = 0;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.Z = 0;
                        Groups[i].Vertices[k*3 + 0].BoneIDs.W = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.X = 1;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Y = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.Z = 0;
                        Groups[i].Vertices[k*3 + 0].BoneWeights.W = 0;
                    }


                    Groups[i].Vertices[k*3 + 1].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].vertex[0];
                    Groups[i].Vertices[k*3 + 1].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].vertex[1];
                    Groups[i].Vertices[k*3 + 1].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].vertex[2];
                    Groups[i].Vertices[k*3 + 1].Normal.X = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[1][0];
                    Groups[i].Vertices[k*3 + 1].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[1][1];
                    Groups[i].Vertices[k*3 + 1].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[1][2];
                    Groups[i].Vertices[k*3 + 1].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].s[1];
                    Groups[i].Vertices[k*3 + 1].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].t[1];
                    Groups[i].Vertices[k*3 + 1].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneId;
                    Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneId;
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneIDs[0];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneIDs[1];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneIDs[2];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneWeights[0];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneWeights[1];
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X = tmp.jointIndices[0];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y = tmp.jointIndices[1];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z = tmp.jointIndices[2];
                        Groups[i].Vertices[k*3 + 1].BoneIDs.W = tmp.jointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[1]].boneId;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Y = 0;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.Z = 0;
                        Groups[i].Vertices[k*3 + 1].BoneIDs.W = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.X = 1;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Y = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.Z = 0;
                        Groups[i].Vertices[k*3 + 1].BoneWeights.W = 0;
                    }

                    Groups[i].Vertices[k*3 + 2].Position.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].vertex[0];
                    Groups[i].Vertices[k*3 + 2].Position.Y =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].vertex[1];
                    Groups[i].Vertices[k*3 + 2].Position.Z =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].vertex[2];
                    Groups[i].Vertices[k*3 + 2].Normal.X = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[2][0];
                    Groups[i].Vertices[k*3 + 2].Normal.Y = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[2][1];
                    Groups[i].Vertices[k*3 + 2].Normal.Z = Triangles[Groups[i].TriangleIndices[k]].vertexNormals[2][2];
                    Groups[i].Vertices[k*3 + 2].TexCoord1.X = Triangles[Groups[i].TriangleIndices[k]].s[2];
                    Groups[i].Vertices[k*3 + 2].TexCoord1.Y = Triangles[Groups[i].TriangleIndices[k]].t[2];
                    Groups[i].Vertices[k*3 + 2].BoneId =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneId;
                    Groups[i].Vertices[k*3 + 2].BoneIDs.X =
                        Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[0]].boneId;
                    Groups[i].Vertices[k*3 + 2].BoneWeights.X = 1;
                    if (Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs != null)
                    {
                        Groups[i].Vertices[k*3 + 1].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs[0];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs[1];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneIDs[2];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneWeights[0];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Y =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneWeights[1];
                        Groups[i].Vertices[k*3 + 2].BoneWeights.Z =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneWeights[2];
                        FillJointIndicesAndWeights(Groups[i].Vertices[k*3 + 0], tmp);
                        Groups[i].Vertices[k*3 + 2].BoneIDs.X = tmp.jointIndices[0];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Y = tmp.jointIndices[1];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.Z = tmp.jointIndices[2];
                        Groups[i].Vertices[k*3 + 2].BoneIDs.W = tmp.jointIndices[3];
                    }
                    else
                    {
                        Groups[i].Vertices[k*3 + 2].BoneIDs.X =
                            Vertices[Triangles[Groups[i].TriangleIndices[k]].vertexIndices[2]].boneId;
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

        private float animBoundMin;
        private float animBoundMax;

        public float getAnimBoundMin()
        {
            return animBoundMin;
        }

        public float getAnimBoundMax()
        {
            return animBoundMax;
        }

        //
        //pentru animatii
        //
        private void R_ConcatTransforms(MilkshapeMatrix3x4 in1, MilkshapeMatrix3x4 in2, MilkshapeMatrix3x4 o)
        {
            o.v[0][0] = in1.v[0][0]*in2.v[0][0] + in1.v[0][1]*in2.v[1][0] +
                        in1.v[0][2]*in2.v[2][0];
            o.v[0][1] = in1.v[0][0]*in2.v[0][1] + in1.v[0][1]*in2.v[1][1] +
                        in1.v[0][2]*in2.v[2][1];
            o.v[0][2] = in1.v[0][0]*in2.v[0][2] + in1.v[0][1]*in2.v[1][2] +
                        in1.v[0][2]*in2.v[2][2];
            o.v[0][3] = in1.v[0][0]*in2.v[0][3] + in1.v[0][1]*in2.v[1][3] +
                        in1.v[0][2]*in2.v[2][3] + in1.v[0][3];
            o.v[1][0] = in1.v[1][0]*in2.v[0][0] + in1.v[1][1]*in2.v[1][0] +
                        in1.v[1][2]*in2.v[2][0];
            o.v[1][1] = in1.v[1][0]*in2.v[0][1] + in1.v[1][1]*in2.v[1][1] +
                        in1.v[1][2]*in2.v[2][1];
            o.v[1][2] = in1.v[1][0]*in2.v[0][2] + in1.v[1][1]*in2.v[1][2] +
                        in1.v[1][2]*in2.v[2][2];
            o.v[1][3] = in1.v[1][0]*in2.v[0][3] + in1.v[1][1]*in2.v[1][3] +
                        in1.v[1][2]*in2.v[2][3] + in1.v[1][3];
            o.v[2][0] = in1.v[2][0]*in2.v[0][0] + in1.v[2][1]*in2.v[1][0] +
                        in1.v[2][2]*in2.v[2][0];
            o.v[2][1] = in1.v[2][0]*in2.v[0][1] + in1.v[2][1]*in2.v[1][1] +
                        in1.v[2][2]*in2.v[2][1];
            o.v[2][2] = in1.v[2][0]*in2.v[0][2] + in1.v[2][1]*in2.v[1][2] +
                        in1.v[2][2]*in2.v[2][2];
            o.v[2][3] = in1.v[2][0]*in2.v[0][3] + in1.v[2][1]*in2.v[1][3] +
                        in1.v[2][2]*in2.v[2][3] + in1.v[2][3];
        }

        private Vector4 QuaternionSlerp(Vector4 p, Vector4 q, float t)
        {
            float omega, cosom, sinom, sclp, sclq;
            Vector4 qt;
            // decide if one of the quaternions is backwards
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

            cosom = p.X*q.X + p.Y*q.Y + p.Z*q.Z + p.W*q.W;

            if ((1.0 + cosom) > 0.00000001)
            {
                if ((1.0 - cosom) > 0.00000001)
                {
                    omega = (float) Math.Acos(cosom);
                    sinom = (float) Math.Sin(omega);
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
            float angle;
            float sr, sp, sy, cr, cp, cy;
            Vector4 quaternion;
            // FIXME: rescale the inputs to 1/2 angle
            angle = (float) angles.Z*0.5f;
            sy = (float) Math.Sin(angle);
            cy = (float) Math.Cos(angle);
            angle = (float) angles.Y*0.5f;
            sp = (float) Math.Sin(angle);
            cp = (float) Math.Cos(angle);
            angle = (float) angles.X*0.5f;
            sr = (float) Math.Sin(angle);
            cr = (float) Math.Cos(angle);

            quaternion.X = sr*cp*cy - cr*sp*sy; // X
            quaternion.Y = cr*sp*cy + sr*cp*sy; // Y
            quaternion.Z = cr*cp*sy - sr*sp*cy; // Z
            quaternion.W = cr*cp*cy + sr*sp*sy; // W
            return quaternion;
        }

        private void QuaternionMatrix(Vector4 quaternion, MilkshapeMatrix3x4 matrix /*float (*matrix)[4] */)
        {
            matrix.v[0][0] = (float) (1.0f - 2.0*quaternion.Y*quaternion.Y - 2.0*quaternion.Z*quaternion.Z);
            matrix.v[1][0] = (float) (2.0f*quaternion.X*quaternion.Y + 2.0*quaternion.W*quaternion.Z);
            matrix.v[2][0] = (float) (2.0f*quaternion.X*quaternion.Z - 2.0*quaternion.W*quaternion.Y);

            matrix.v[0][1] = (float) (2.0f*quaternion.X*quaternion.Y - 2.0*quaternion.W*quaternion.Z);
            matrix.v[1][1] = (float) (1.0f - 2.0*quaternion.X*quaternion.X - 2.0*quaternion.Z*quaternion.Z);
            matrix.v[2][1] = (float) (2.0f*quaternion.Y*quaternion.Z + 2.0*quaternion.W*quaternion.X);

            matrix.v[0][2] = (float) (2.0f*quaternion.X*quaternion.Z + 2.0*quaternion.W*quaternion.Y);
            matrix.v[1][2] = (float) (2.0f*quaternion.Y*quaternion.Z - 2.0*quaternion.W*quaternion.X);
            matrix.v[2][2] = (float) (1.0f - 2.0*quaternion.X*quaternion.X - 2.0*quaternion.Y*quaternion.Y);
        }

        private int FindJointByName(String name)
        {
            for (int i = 0; i < numJoints; i++)
                if (Joints[i].nameS == name)
                    return i;
            return -1;
        }

        private void AngleMatrix(Vector3 angles, MilkshapeMatrix3x4 matrix)
        {
            float angle;
            float sr, sp, sy, cr, cp, cy;

            angle = angles.Z;
            sy = (float) Math.Sin(angle);
            cy = (float) Math.Cos(angle);
            angle = angles.Y;
            sp = (float) Math.Sin(angle);
            cp = (float) Math.Cos(angle);
            angle = angles.X;
            sr = (float) Math.Sin(angle);
            cr = (float) Math.Cos(angle);

            // matrix = (Z * Y) * X
            matrix.v[0][0] = cp*cy;
            matrix.v[1][0] = cp*sy;
            matrix.v[2][0] = -sp;
            matrix.v[0][1] = sr*sp*cy + cr*-sy;
            matrix.v[1][1] = sr*sp*sy + cr*cy;
            matrix.v[2][1] = sr*cp;
            matrix.v[0][2] = (cr*sp*cy + -sr*-sy);
            matrix.v[1][2] = (cr*sp*sy + -sr*cy);
            matrix.v[2][2] = cr*cp;
            matrix.v[0][3] = 0.0f;
            matrix.v[1][3] = 0.0f;
            matrix.v[2][3] = 0.0f;
        }

        private void SetupJoints()
        {
            for (int i = 0; i < numJoints; i++)
            {
                Joints[i].parentIndex = FindJointByName(Joints[i].parentNameS);
            }
            for (int i = 0; i < numJoints; i++)
            {
                Vector3 tmp = new Vector3(Joints[i].rotation[0], Joints[i].rotation[1], Joints[i].rotation[2]);
                Joints[i].matLocalSkeleton = new MilkshapeMatrix3x4();
                AngleMatrix(tmp, Joints[i].matLocalSkeleton);
                Joints[i].matLocalSkeleton.v[0][3] = Joints[i].position[0];
                Joints[i].matLocalSkeleton.v[1][3] = Joints[i].position[1];
                Joints[i].matLocalSkeleton.v[2][3] = Joints[i].position[2];

                if (Joints[i].parentIndex == -1)
                {
                    Joints[i].matGlobalSkeleton = new MilkshapeMatrix3x4();
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                            Joints[i].matGlobalSkeleton.v[k][l] = Joints[i].matLocalSkeleton.v[k][l];
                }
                else
                {
                    Joints[i].matGlobalSkeleton = new MilkshapeMatrix3x4();
                    R_ConcatTransforms(Joints[Joints[i].parentIndex].matGlobalSkeleton, Joints[i].matLocalSkeleton,
                                       Joints[i].matGlobalSkeleton);
                }

                SetupTangents();
            }
        }

        private void SetupTangents()
        {
            for (int j = 0; j < numJoints; j++)
            {
                MilkshapeJoint joint = Joints[j];
                int numPositionKeys = joint.numPosKeyFrames;
                joint.tangents = new MilkshapeTangent[numPositionKeys];

                // clear all tangents (zero derivatives)
                for (int k = 0; k < numPositionKeys; k++)
                {
                    joint.tangents[k] = new MilkshapeTangent();
                    joint.tangents[k].tangentIn.X = 0.0f;
                    joint.tangents[k].tangentIn.Y = 0.0f;
                    joint.tangents[k].tangentIn.Z = 0.0f;
                    joint.tangents[k].tangentOut.X = 0.0f;
                    joint.tangents[k].tangentOut.Y = 0.0f;
                    joint.tangents[k].tangentOut.Z = 0.0f;
                }

                // if there are more than 2 keys, we can calculate tangents, otherwise we use zero derivatives
                if (numPositionKeys > 2)
                {
                    for (int k = 0; k < numPositionKeys; k++)
                    {
                        // make the curve tangents looped
                        int k0 = k - 1;
                        if (k0 < 0)
                            k0 = numPositionKeys - 1;
                        int k1 = k;
                        int k2 = k + 1;
                        if (k2 >= numPositionKeys)
                            k2 = 0;
                        // calculate the tangent, which is the vector from key[k - 1] to key[k + 1]
                        float[] tangent = new float[3];
                        tangent[0] = (joint.posKeyFrames[k2].position[0] - joint.posKeyFrames[k0].position[0]);
                        tangent[1] = (joint.posKeyFrames[k2].position[1] - joint.posKeyFrames[k0].position[1]);
                        tangent[2] = (joint.posKeyFrames[k2].position[2] - joint.posKeyFrames[k0].position[2]);

                        // weight the incoming and outgoing tangent by their time to avoid changes in speed, if the keys are not within the same interval
                        float dt1 = joint.posKeyFrames[k1].time - joint.posKeyFrames[k0].time;
                        float dt2 = joint.posKeyFrames[k2].time - joint.posKeyFrames[k1].time;
                        float dt = dt1 + dt2;
                        joint.tangents[k1].tangentIn.X = tangent[0]*dt1/dt;
                        joint.tangents[k1].tangentIn.Y = tangent[1]*dt1/dt;
                        joint.tangents[k1].tangentIn.Z = tangent[2]*dt1/dt;

                        joint.tangents[k1].tangentOut.X = tangent[0]*dt2/dt;
                        joint.tangents[k1].tangentOut.Y = tangent[1]*dt2/dt;
                        joint.tangents[k1].tangentOut.Z = tangent[2]*dt2/dt;
                    }
                }
            }
        }

        private void rebuildJoint(MilkshapeJoint joint)
        {
            //
            // calculate joint animation matrix, this matrix will animate matLocalSkeleton
            //
            float frame = currentTime;
            //float frame=15;
            Vector3 pos = new Vector3(0f, 0f, 0f);
            int numPositionKeys = joint.numPosKeyFrames;
            if (numPositionKeys > 0)
            {
                int i1 = -1;
                int i2 = -1;

                // find the two keys, where "frame" is in between for the position channel
                for (int i = 0; i < (numPositionKeys - 1); i++)
                {
                    if (frame >= joint.posKeyFrames[i].time && frame < joint.posKeyFrames[i + 1].time)
                    {
                        i1 = i;
                        i2 = i + 1;
                        break;
                    }
                }

                // if there are no such keys
                if (i1 == -1 || i2 == -1)
                {
                    // either take the first
                    if (frame < joint.posKeyFrames[0].time)
                    {
                        pos.X = joint.posKeyFrames[0].position[0];
                        pos.Y = joint.posKeyFrames[0].position[1];
                        pos.Z = joint.posKeyFrames[0].position[2];
                    }

                        // or the last key
                    else if (frame >= joint.posKeyFrames[numPositionKeys - 1].time)
                    {
                        pos.X = joint.posKeyFrames[numPositionKeys - 1].position[0];
                        pos.Y = joint.posKeyFrames[numPositionKeys - 1].position[1];
                        pos.Z = joint.posKeyFrames[numPositionKeys - 1].position[2];
                    }
                }

                    // there are such keys, so interpolate using hermite interpolation
                else
                {
                    MilkshapePositionKey p0 = joint.posKeyFrames[i1];
                    MilkshapePositionKey p1 = joint.posKeyFrames[i2];
                    MilkshapeTangent m0 = joint.tangents[i1];
                    MilkshapeTangent m1 = joint.tangents[i2];

                    // normalize the time between the keys into [0..1]
                    float t = (frame - joint.posKeyFrames[i1].time)/
                              (joint.posKeyFrames[i2].time - joint.posKeyFrames[i1].time);
                    float t2 = t*t;
                    float t3 = t2*t;

                    // calculate hermite basis
                    float h1 = 2.0f*t3 - 3.0f*t2 + 1.0f;
                    float h2 = -2.0f*t3 + 3.0f*t2;
                    float h3 = t3 - 2.0f*t2 + t;
                    float h4 = t3 - t2;

                    // do hermite interpolation
                    pos.X = h1*p0.position[0] + h3*m0.tangentOut.X + h2*p1.position[0] + h4*m1.tangentIn.X;
                    pos.Y = h1*p0.position[1] + h3*m0.tangentOut.Y + h2*p1.position[1] + h4*m1.tangentIn.Y;
                    pos.Z = h1*p0.position[2] + h3*m0.tangentOut.Z + h2*p1.position[2] + h4*m1.tangentIn.Z;
                }
            }

            Vector4 quat = new Vector4(0f, 0f, 0f, 1f);
            int numRotationKeys = (int) joint.numRotKeyFrames;
            if (numRotationKeys > 0)
            {
                int i1 = -1;
                int i2 = -1;

                // find the two keys, where "frame" is in between for the rotation channel
                for (int i = 0; i < (numRotationKeys - 1); i++)
                {
                    if (frame >= joint.rotKeyFrames[i].time && frame < joint.rotKeyFrames[i + 1].time)
                    {
                        i1 = i;
                        i2 = i + 1;
                        break;
                    }
                }

                // if there are no such keys
                if (i1 == -1 || i2 == -1)
                {
                    // either take the first key
                    if (frame < joint.rotKeyFrames[0].time)
                    {
                        Vector3 tmp = new Vector3(joint.rotKeyFrames[0].rotation[0], joint.rotKeyFrames[0].rotation[1],
                                                  joint.rotKeyFrames[0].rotation[2]);
                        quat = AngleQuaternion(tmp);
                    }

                        // or the last key
                    else if (frame >= joint.rotKeyFrames[numRotationKeys - 1].time)
                    {
                        Vector3 tmp = new Vector3(joint.rotKeyFrames[numRotationKeys - 1].rotation[0],
                                                  joint.rotKeyFrames[numRotationKeys - 1].rotation[1],
                                                  joint.rotKeyFrames[numRotationKeys - 1].rotation[2]);
                        quat = AngleQuaternion(tmp);
                    }
                }

                    // there are such keys, so do the quaternion slerp interpolation
                else
                {
                    float t = (frame - joint.rotKeyFrames[i1].time)/
                              (joint.rotKeyFrames[i2].time - joint.rotKeyFrames[i1].time);
                    Vector4 q1 = new Vector4();
                    Vector3 tmp = new Vector3(joint.rotKeyFrames[i1].rotation[0], joint.rotKeyFrames[i1].rotation[1],
                                              joint.rotKeyFrames[i1].rotation[2]);
                    q1 = AngleQuaternion(tmp);
                    Vector4 q2 = new Vector4();
                    tmp = new Vector3(joint.rotKeyFrames[i2].rotation[0], joint.rotKeyFrames[i2].rotation[1],
                                      joint.rotKeyFrames[i2].rotation[2]);
                    q2 = AngleQuaternion(tmp);
                    quat = QuaternionSlerp(q1, q2, t);
                }
            }

            // make a matrix from pos/quat
            MilkshapeMatrix3x4 matAnimate = new MilkshapeMatrix3x4();
            QuaternionMatrix(quat, matAnimate);
            matAnimate.v[0][3] = pos.X;
            matAnimate.v[1][3] = pos.Y;
            matAnimate.v[2][3] = pos.Z;

            // animate the local joint matrix using: matLocal = matLocalSkeleton * matAnimate
            if (joint.matLocal == null)
                joint.matLocal = new MilkshapeMatrix3x4();
            R_ConcatTransforms(joint.matLocalSkeleton, matAnimate, joint.matLocal);

            // build up the hierarchy if joints
            if (joint.parentIndex == -1)
            {
                if (joint.matGlobal == null)
                    joint.matGlobal = new MilkshapeMatrix3x4();
                for (int k = 0; k < 3; k++)
                    for (int l = 0; l < 4; l++)
                        joint.matGlobal.v[k][l] = joint.matLocal.v[k][l];
            }
            else
            {
                MilkshapeJoint parentJoint = Joints[joint.parentIndex];
                if (joint.matGlobal == null)
                    joint.matGlobal = new MilkshapeMatrix3x4();
                if (parentJoint.matGlobal == null)
                    parentJoint.matGlobal = new MilkshapeMatrix3x4();
                R_ConcatTransforms(parentJoint.matGlobal, joint.matLocal, joint.matGlobal);
            }
        }

        private void FillJointIndicesAndWeights(CustomVertexStruct vertex, MilkshapeJointAndWeight jw)
        {
            jw.jointIndices[0] = vertex.BoneId;
            jw.jointIndices[1] = (int) vertex.BoneIDs.X;
            jw.jointIndices[2] = (int) vertex.BoneIDs.Y;
            jw.jointIndices[3] = (int) vertex.BoneIDs.Z;
            jw.jointWeights[0] = 100;
            jw.jointWeights[1] = 0;
            jw.jointWeights[2] = 0;
            jw.jointWeights[3] = 0;
            if (vertex.BoneWeights.X != 0 || vertex.BoneWeights.Y != 0 || vertex.BoneWeights.Z != 0)
            {
                jw.jointWeights[0] = (int) vertex.BoneWeights.X;
                jw.jointWeights[1] = (int) vertex.BoneWeights.Y;
                jw.jointWeights[2] = (int) vertex.BoneWeights.Z;
                jw.jointWeights[3] = 100 - (int) (vertex.BoneWeights.X + vertex.BoneWeights.Y + vertex.BoneWeights.Z);
            }
        }

        private Vector3 VectorIRotate(Vector3 in1, MilkshapeMatrix3x4 in2)
        {
            Vector3 outVert;
            outVert.X = in1.X*in2.v[0][0] + in1.Y*in2.v[1][0] + in1.Z*in2.v[2][0];
            outVert.Y = in1.X*in2.v[0][1] + in1.Y*in2.v[1][1] + in1.Z*in2.v[2][1];
            outVert.Z = in1.X*in2.v[0][2] + in1.Y*in2.v[1][2] + in1.Z*in2.v[2][2];
            return outVert;
        }

        private Vector3 VectorTransform(Vector3 in1, MilkshapeMatrix3x4 in2)
        {
            Vector3 outVert;
            outVert.X = (in1.X*in2.v[0][0] + in1.Y*in2.v[0][1] + in1.Z*in2.v[0][2]) + in2.v[0][3];
            outVert.Y = (in1.X*in2.v[1][0] + in1.Y*in2.v[1][1] + in1.Z*in2.v[1][2]) + in2.v[1][3];
            outVert.Z = (in1.X*in2.v[2][0] + in1.Y*in2.v[2][1] + in1.Z*in2.v[2][2]) + in2.v[2][3];

            return outVert;
        }

        private Vector3 VectorITransform(Vector3 in1, MilkshapeMatrix3x4 in2)
        {
            Vector3 outVect;
            Vector3 tmp;
            tmp.X = in1.X - in2.v[0][3];
            tmp.Y = in1.Y - in2.v[1][3];
            tmp.Z = in1.Z - in2.v[2][3];
            outVect = VectorIRotate(tmp, in2);
            return outVect;
        }

        private CustomVertexStruct TransformVertex(CustomVertexStruct vertex)
        {
            //trebuie optimizata rau de tot
            MilkshapeJointAndWeight jw = new MilkshapeJointAndWeight();
            CustomVertexStruct outVertex = vertex;
            jw.jointIndices[0] = (int) outVertex.BoneIDs.X;
            jw.jointIndices[1] = (int) outVertex.BoneIDs.Y;
            jw.jointIndices[2] = (int) outVertex.BoneIDs.Z;
            jw.jointIndices[3] = (int) outVertex.BoneIDs.W;
            jw.jointWeights[0] = (int) (outVertex.BoneWeights.X*100);
            jw.jointWeights[1] = (int) (outVertex.BoneWeights.Y*100);
            jw.jointWeights[2] = (int) (outVertex.BoneWeights.Z*100);
            jw.jointWeights[3] = (int) (outVertex.BoneWeights.W*100);

            if (jw.jointIndices[0] < 0 || jw.jointIndices[0] >= numJoints || currentTime < 0.0f)
            {
            }
            else
            {
                // count valid weights
                int numWeights = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (jw.jointWeights[i] > 0 && jw.jointIndices[i] >= 0 && jw.jointIndices[i] < numJoints)
                        ++numWeights;
                    else
                        break;
                }

                // init
                outVertex.Position.X = 0.0f;
                outVertex.Position.Y = 0.0f;
                outVertex.Position.Z = 0.0f;
                outVertex.Normal.X = 0.0f;
                outVertex.Normal.Y = 0.0f;
                outVertex.Normal.Z = 0.0f;

                //float weights[4] = { (float) jointWeights[0] / 100.0f, (float) jointWeights[1] / 100.0f, (float) jointWeights[2] / 100.0f, (float) jointWeights[3] / 100.0f };
                float[] weights = new float[4];
                weights[0] = (float) jw.jointWeights[0]/100.0f;
                weights[1] = (float) jw.jointWeights[1]/100.0f;
                weights[2] = (float) jw.jointWeights[2]/100.0f;
                weights[3] = (float) jw.jointWeights[3]/100.0f;

                if (numWeights == 0)
                {
                    numWeights = 1;
                    weights[0] = 1.0f;
                }
                // add weighted vertices
                for (int i = 0; i < numWeights; i++)
                {
                    MilkshapeJoint joint = Joints[jw.jointIndices[i]];
                    Vector3 tmp, vert, norm;
                    tmp = VectorITransform(vertex.Position, joint.matGlobalSkeleton);
                    vert = VectorTransform(tmp, joint.matGlobal);

                    outVertex.Position.X += vert.X*weights[i];
                    outVertex.Position.Y += vert.Y*weights[i];
                    outVertex.Position.Z += vert.Z*weights[i];

                    tmp = new Vector3(0f, 0f, 0f);
                    tmp = VectorIRotate(vertex.Normal, joint.matGlobalSkeleton);
                    norm = VectorTransform(tmp, joint.matGlobal);
                    outVertex.Normal.X += norm.X;
                    outVertex.Normal.Y += norm.Y;
                    outVertex.Normal.Z += norm.Z;
                }
            }
            return outVertex;
        }

        public void rebuildJoints()
        {
            //luam fiecare joint in parte
            if (currentTime < 0.0f)
            {
                for (int i = 0; i < numJoints; i++)
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                        {
                            Joints[i].matGlobal.v[k][l] = Joints[i].matGlobalSkeleton.v[k][l];
                            Joints[i].matLocalSkeleton.v[k][l] = Joints[i].matLocal.v[k][l];
                        }
            }
            else
                for (int i = 0; i < numJoints; i++)
                {
                    rebuildJoint(Joints[i]);
                }
        }

        private bool FirstRun;

        public delegate void Stopped();
        public event Stopped StoppedAnimation;

        public void OnStoppedAnimation()
        {   
            var handler = StoppedAnimation;
            if (handler != null) 
                handler();
        }

        public bool Reverse { get; set; }

        public void advanceAnimation(float delta)
        {
            if (Reverse)
            {
                currentTime -= delta;

                if (currentTime < animBoundMin)
                {
                    OnStoppedAnimation();
                }
            }
            else
            {

                currentTime += delta;

                if (currentTime > animBoundMax)
                {
                    OnStoppedAnimation();
                }
            }

            //generam matricile
            if (FirstRun)
            {
                FirstRun = false;
                for (int i = 0; i < numJoints; i++)
                {
                    for (int k = 0; k < 3; k++)
                        for (int l = 0; l < 4; l++)
                        {
                            if (Joints[i].matLocalSkeleton == null)
                                Joints[i].matLocalSkeleton = new MilkshapeMatrix3x4();
                            if (Joints[i].matGlobalSkeleton == null)
                                Joints[i].matGlobalSkeleton = new MilkshapeMatrix3x4();
                            if (Joints[i].matLocal == null)
                                Joints[i].matLocal = new MilkshapeMatrix3x4();
                            if (Joints[i].matGlobal == null)
                                Joints[i].matGlobal = new MilkshapeMatrix3x4();
                            Joints[i].matLocal.v[k][l] = Joints[i].matLocalSkeleton.v[k][l];
                            Joints[i].matGlobal.v[k][l] = Joints[i].matGlobalSkeleton.v[k][l];
                        }
                }
            }
            else
                rebuildJoints();
                for (int i = 0; i < numGroups; i++)
                {
                    for (int j = 0; j < Groups[i].Vertices.Length; j++)
                    {
                        Groups[i].Vertices[j] = TransformVertex(Groups[i].OriginalVertices[j]);
                    }
                }
        }

        public void setAnimBounds(float minFrame, float maxFrame, bool reverse)
        {
            //de completat aici
            animBoundMin = minFrame;
            animBoundMax = maxFrame;

            Reverse = reverse;

            if (reverse)
            {
                if ((currentTime < animBoundMin) || (currentTime > animBoundMax))
                {
                    currentTime = animBoundMax;
                }
            }
            else
            {
                if ((currentTime < animBoundMin) || (currentTime > animBoundMax))
                {
                    currentTime = animBoundMin;
                }    
            }
            
        }

        public void setAnimBounds(float minFrame, float maxFrame)
        {
            //de completat aici
            animBoundMin = minFrame;
            animBoundMax = maxFrame;

            if ((currentTime < animBoundMin) || (currentTime > animBoundMax))
            {
                currentTime = animBoundMin;
            }

        }

        private GraphicsDevice gd;

        public MilkshapeModel(string FileName, GraphicsDevice gd)
        {
            //load ms3d file
            this.gd = gd;
            loadMS3DFromFile(FileName, gd);
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

        public static VertexDeclaration vd = new VertexDeclaration(CustomVertexStruct.VertexElements);
        public BasicEffect BasicEffect
        {
            get { return basicEffect ?? (basicEffect = new BasicEffect(gd)); }
            set { basicEffect = value; }
        }
        private BasicEffect basicEffect;
        
        public void Render()
        {
            gd.RasterizerState = RasterizerState.CullClockwise;
            
            BasicEffect.TextureEnabled = true;
            for (int i = 0; i < numGroups; i++)
            {
                gd.Textures[0] = Materials[Groups[i].MaterialIndex].texture;
                BasicEffect.Texture = (Texture2D) Materials[Groups[i].MaterialIndex].texture;

                foreach (EffectPass p in BasicEffect.CurrentTechnique.Passes)
                {
                    p.Apply();

                    gd.DrawUserPrimitives<CustomVertexStruct>(PrimitiveType.TriangleList, Groups[i].Vertices, 0,
                                                              Groups[i].NumTriangles, vd);
                }
            }
        }
    }
}
