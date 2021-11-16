// ===============================================================================================
//	The MIT License (MIT) for UnityFBXExporter
//
//  UnityFBXExporter was created for Building Crafter (http://u3d.as/ovC) a tool to rapidly 
//	create high quality buildings right in Unity with no need to use 3D modeling programs.
//
//  Copyright (c) 2016 | 8Bit Goose Games, Inc.
//		
//	Permission is hereby granted, free of charge, to any person obtaining a copy 
//	of this software and associated documentation files (the "Software"), to deal 
//	in the Software without restriction, including without limitation the rights 
//	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
//	of the Software, and to permit persons to whom the Software is furnished to do so, 
//	subject to the following conditions:
//		
//	The above copyright notice and this permission notice shall be included in all 
//	copies or substantial portions of the Software.
//		
//	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//	INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
//	PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
//	HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
//	OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
//	OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
// ===============================================================================================


using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinarySerializer;
using UnityEngine;

namespace Ray1Map.Jade {
	public class FBXExporter {
		public static string VersionInformation => "Ray1Map FBX Export";

		public static long GetRandomFBXId() => System.BitConverter.ToInt64(System.Guid.NewGuid().ToByteArray(), 0);

		public static async UniTask ExportFBXAsync(OBJ_GameObject gao, string outputPath) {
			System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

			string outputFile = Path.Combine(outputPath, $"{gao.Key.Key:X8}_{gao.Export_FileBasename}.fbx");

			StringBuilder sb = new StringBuilder();
			WriteHeader(sb);
			WriteGameObject(sb, gao, outputPath);

			string buildMesh = sb.ToString();

			if (System.IO.File.Exists(outputFile))
				System.IO.File.Delete(outputFile);

			System.IO.File.WriteAllText(outputFile, buildMesh);
		}

		#region Header
		private static void WriteHeader(StringBuilder sb) {
			// ========= Create header ========
			// Intro
			sb.AppendLine("; FBX 7.3.0 project file");
			sb.AppendLine("; Copyright (C) 1997-2010 Autodesk Inc. and/or its licensors.");
			sb.AppendLine("; All rights reserved.");
			sb.AppendLine("; ----------------------------------------------------");
			sb.AppendLine();

			// The header
			sb.AppendLine("FBXHeaderExtension:  {");
			sb.AppendLine("\tFBXHeaderVersion: 1003");
			sb.AppendLine("\tFBXVersion: 7300");

			// Creationg Date Stamp
			System.DateTime currentDate = System.DateTime.Now;
			sb.AppendLine("\tCreationTimeStamp:  {");
			sb.AppendLine("\t\tVersion: 1000");
			sb.AppendLine("\t\tYear: " + currentDate.Year);
			sb.AppendLine("\t\tMonth: " + currentDate.Month);
			sb.AppendLine("\t\tDay: " + currentDate.Day);
			sb.AppendLine("\t\tHour: " + currentDate.Hour);
			sb.AppendLine("\t\tMinute: " + currentDate.Minute);
			sb.AppendLine("\t\tSecond: " + currentDate.Second);
			sb.AppendLine("\t\tMillisecond: " + currentDate.Millisecond);
			sb.AppendLine("\t}");

			// Info on the Creator
			sb.AppendLine("\tCreator: \"" + VersionInformation + "\"");
			sb.AppendLine("\tSceneInfo: \"SceneInfo::GlobalInfo\", \"UserData\" {");
			sb.AppendLine("\t\tType: \"UserData\"");
			sb.AppendLine("\t\tVersion: 100");
			sb.AppendLine("\t\tMetaData:  {");
			sb.AppendLine("\t\t\tVersion: 100");
			sb.AppendLine("\t\t\tTitle: \"\"");
			sb.AppendLine("\t\t\tSubject: \"\"");
			sb.AppendLine("\t\t\tAuthor: \"\"");
			sb.AppendLine("\t\t\tKeywords: \"\"");
			sb.AppendLine("\t\t\tRevision: \"\"");
			sb.AppendLine("\t\t\tComment: \"\"");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t\tProperties70:  {");

			// Information on how this item was originally generated
			/*string documentInfoPaths = Application.dataPath + newPath + ".fbx";
			sb.AppendLine("\t\t\tP: \"DocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");
			sb.AppendLine("\t\t\tP: \"SrcDocumentUrl\", \"KString\", \"Url\", \"\", \"" + documentInfoPaths + "\"");*/
			sb.AppendLine("\t\t\tP: \"Original\", \"Compound\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"Original|FileName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved\", \"Compound\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVendor\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationName\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|ApplicationVersion\", \"KString\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t\tP: \"LastSaved|DateTime_GMT\", \"DateTime\", \"\", \"\", \"\"");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// The Global information
			sb.AppendLine("GlobalSettings:  {");
			sb.AppendLine("\tVersion: 1000");
			sb.AppendLine("\tProperties70:  {");

			// Change this for each axis system
			sb.AppendLine("\t\tP: \"UpAxis\", \"int\", \"Integer\", \"\",2");
			sb.AppendLine("\t\tP: \"UpAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"FrontAxis\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"FrontAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"CoordAxis\", \"int\", \"Integer\", \"\",0");
			sb.AppendLine("\t\tP: \"CoordAxisSign\", \"int\", \"Integer\", \"\",1");
			sb.AppendLine("\t\tP: \"OriginalUpAxis\", \"int\", \"Integer\", \"\",-1");
			sb.AppendLine("\t\tP: \"OriginalUpAxisSign\", \"int\", \"Integer\", \"\",1");

			sb.AppendLine("\t\tP: \"UnitScaleFactor\", \"double\", \"Number\", \"\",100"); // NOTE: This sets the resize scale upon import
			sb.AppendLine("\t\tP: \"OriginalUnitScaleFactor\", \"double\", \"Number\", \"\",100");
			sb.AppendLine("\t\tP: \"AmbientColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\tP: \"DefaultCamera\", \"KString\", \"\", \"\", \"Producer Perspective\"");
			sb.AppendLine("\t\tP: \"TimeMode\", \"enum\", \"\", \"\",11");
			sb.AppendLine("\t\tP: \"TimeSpanStart\", \"KTime\", \"Time\", \"\",0");
			sb.AppendLine("\t\tP: \"TimeSpanStop\", \"KTime\", \"Time\", \"\",479181389250");
			sb.AppendLine("\t\tP: \"CustomFrameRate\", \"double\", \"Number\", \"\",-1");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// The Object definations
			sb.AppendLine("; Object definitions");
			sb.AppendLine(";------------------------------------------------------------------");
			sb.AppendLine("");
			sb.AppendLine("Definitions:  {");
			sb.AppendLine("\tVersion: 100");
			sb.AppendLine("\tCount: 4");

			sb.AppendLine("\tObjectType: \"GlobalSettings\" {");
			sb.AppendLine("\t\tCount: 1");
			sb.AppendLine("\t}");


			sb.AppendLine("\tObjectType: \"Model\" {");
			sb.AppendLine("\t\tCount: 1"); // TODO figure out if this count matters
			sb.AppendLine("\t\tPropertyTemplate: \"FbxNode\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"QuaternionInterpolate\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingOffset\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"TranslationMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationSpaceForLimitOnly\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationStiffnessZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"AxisLen\", \"double\", \"Number\", \"\",10");
			sb.AppendLine("\t\t\t\tP: \"PreRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"PostRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"RotationMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingActive\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMinZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxX\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxY\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"ScalingMaxZ\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"GeometricTranslation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"GeometricRotation\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"GeometricScaling\", \"Vector3D\", \"Vector\", \"\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampRangeZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampRangeZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MinDampStrengthZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"MaxDampStrengthZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleX\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleY\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PreferedAngleZ\", \"double\", \"Number\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"LookAtProperty\", \"object\", \"\", \"\"");
			sb.AppendLine("\t\t\t\tP: \"UpVectorProperty\", \"object\", \"\", \"\"");
			sb.AppendLine("\t\t\t\tP: \"Show\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"NegativePercentShapeSupport\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",-1");
			sb.AppendLine("\t\t\t\tP: \"Freeze\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"LODBox\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"Visibility\", \"Visibility\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"Visibility Inheritance\", \"Visibility Inheritance\", \"\", \"\",1");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// The geometry
			sb.AppendLine("\tObjectType: \"Geometry\" {");
			sb.AppendLine("\t\tCount: 1"); // TODO - this must be set by the number of items being placed.
			sb.AppendLine("\t\tPropertyTemplate: \"FbxMesh\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"Color\", \"ColorRGB\", \"Color\", \"\",0.8,0.8,0.8");
			sb.AppendLine("\t\t\t\tP: \"BBoxMin\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"BBoxMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Primary Visibility\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Casts Shadows\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Receive Shadows\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// The materials
			sb.AppendLine("\tObjectType: \"Material\" {");
			sb.AppendLine("\t\tCount: 1");
			sb.AppendLine("\t\tPropertyTemplate: \"FbxSurfacePhong\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"ShadingModel\", \"KString\", \"\", \"\", \"Phong\"");
			sb.AppendLine("\t\t\t\tP: \"MultiLayer\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"EmissiveColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"AmbientColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
			sb.AppendLine("\t\t\t\tP: \"AmbientFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"DiffuseColor\", \"Color\", \"\", \"A\",0.8,0.8,0.8");
			sb.AppendLine("\t\t\t\tP: \"DiffuseFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"Bump\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"NormalMap\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"BumpFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TransparencyFactor\", \"Number\", \"\", \"A\",0");
			sb.AppendLine("\t\t\t\tP: \"DisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"DisplacementFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"VectorDisplacementColor\", \"ColorRGB\", \"Color\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"VectorDisplacementFactor\", \"double\", \"Number\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"SpecularColor\", \"Color\", \"\", \"A\",0.2,0.2,0.2");
			sb.AppendLine("\t\t\t\tP: \"SpecularFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"ShininessExponent\", \"Number\", \"\", \"A\",20");
			sb.AppendLine("\t\t\t\tP: \"ReflectionColor\", \"Color\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"ReflectionFactor\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			// Explanation of how textures work
			sb.AppendLine("\tObjectType: \"Texture\" {");
			sb.AppendLine("\t\tCount: 2"); // TODO - figure out if this texture number is important
			sb.AppendLine("\t\tPropertyTemplate: \"FbxFileTexture\" {");
			sb.AppendLine("\t\t\tProperties70:  {");
			sb.AppendLine("\t\t\t\tP: \"TextureTypeUse\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"Texture alpha\", \"Number\", \"\", \"A\",1");
			sb.AppendLine("\t\t\t\tP: \"CurrentMappingType\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"WrapModeU\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"WrapModeV\", \"enum\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"UVSwap\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"PremultiplyAlpha\", \"bool\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"Translation\", \"Vector\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Rotation\", \"Vector\", \"\", \"A\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"Scaling\", \"Vector\", \"\", \"A\",1,1,1");
			sb.AppendLine("\t\t\t\tP: \"TextureRotationPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"TextureScalingPivot\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			sb.AppendLine("\t\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",1");
			sb.AppendLine("\t\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"default\"");
			sb.AppendLine("\t\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t\tP: \"UseMipMap\", \"bool\", \"\", \"\",0");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");

			sb.AppendLine("}");
			sb.AppendLine("");
		}
		#endregion


		private static void WriteGameObject(StringBuilder sb, OBJ_GameObject gameObj, string outputPath) {
			StringBuilder objectProps = new StringBuilder();
			objectProps.AppendLine("; Object properties");
			objectProps.AppendLine(";------------------------------------------------------------------");
			objectProps.AppendLine("");
			objectProps.AppendLine("Objects:  {");

			StringBuilder objectConnections = new StringBuilder();
			objectConnections.AppendLine("; Object connections");
			objectConnections.AppendLine(";------------------------------------------------------------------");
			objectConnections.AppendLine("");
			objectConnections.AppendLine("Connections:  {");
			objectConnections.AppendLine("\t");

			// Dictionary to collect all materials used by this FBX
			var materials = new Dictionary<uint, GEO_Object>();

			// First finds all unique materials and compiles them (and writes to the object connections) for funzies
			string materialsObjectSerialized = "";
			string materialConnectionsSerialized = "";

			// Run recursive FBX Mesh grab over the entire gameobject
			WriteGeometryForGameObject(gameObj, ref objectProps, ref objectConnections, materials, setToZeroPosition: true);

			AllMaterialsToString(materials, outputPath, out materialsObjectSerialized, out materialConnectionsSerialized);


			// write the materials to the objectProps here. Should not do it in the above as it recursive.

			objectProps.Append(materialsObjectSerialized);
			objectConnections.Append(materialConnectionsSerialized);

			// Close up both builders;
			objectProps.AppendLine("}");
			objectConnections.AppendLine("}");

			sb.Append(objectProps.ToString());
			sb.Append(objectConnections.ToString());
		}

		#region Geometry
		/// <summary>
		/// Gets all the meshes and outputs to a string (even grabbing the child of each gameObject)
		/// </summary>
		/// <returns>The mesh to string.</returns>
		/// <param name="gao">GameObject Parent.</param>
		/// <param name="materials">Every Material in the parent that can be accessed.</param>
		/// <param name="objects">The StringBuidler to create objects for the FBX file.</param>
		/// <param name="connections">The StringBuidler to create connections for the FBX file.</param>
		/// <param name="parentObject">Parent object, if left null this is the top parent.</param>
		/// <param name="parentModelId">Parent model id, 0 if top parent.</param>
		public static long WriteGeometryForGameObject(OBJ_GameObject gao,
										   ref StringBuilder objects,
										   ref StringBuilder connections,
										   Dictionary<uint, GEO_Object> materials,
										   OBJ_GameObject parentObject = null,
										   long parentModelId = 0,
										   bool setToZeroPosition = false) {
			bool appendComma = false;
			StringBuilder tempObjectSb = new StringBuilder();
			StringBuilder tempConnectionsSb = new StringBuilder();

			long geometryId = FBXExporter.GetRandomFBXId();
			long modelId = gao.Key.Key;

			GRO_GraphicRenderObject gro = gao?.Base?.Visual?.GeometricObject?.Value?.RenderObject?.Value;
			GEO_GeometricObject geo = gro != null ? gro as GEO_GeometricObject : null;

			string meshName = gao.Export_FileBasename;

			// A NULL parent means that the gameObject is at the top
			string isMesh = "Null";

			if (geo != null) {
				isMesh = "Mesh";
			}

			if (parentModelId == 0)
				tempConnectionsSb.AppendLine("\t;Model::" + meshName + ", Model::RootNode");
			else
				tempConnectionsSb.AppendLine("\t;Model::" + meshName + ", Model::USING PARENT");
			tempConnectionsSb.AppendLine("\tC: \"OO\"," + modelId + "," + parentModelId);
			tempConnectionsSb.AppendLine();
			tempObjectSb.AppendLine("\tModel: " + modelId + ", \"Model::" + meshName + "\", \"" + isMesh + "\" {");
			tempObjectSb.AppendLine("\t\tVersion: 232");
			tempObjectSb.AppendLine("\t\tProperties70:  {");
			tempObjectSb.AppendLine("\t\t\tP: \"RotationOrder\", \"enum\", \"\", \"\",4");
			tempObjectSb.AppendLine("\t\t\tP: \"RotationActive\", \"bool\", \"\", \"\",1");
			tempObjectSb.AppendLine("\t\t\tP: \"InheritType\", \"enum\", \"\", \"\",1");
			tempObjectSb.AppendLine("\t\t\tP: \"ScalingMax\", \"Vector3D\", \"Vector\", \"\",0,0,0");
			tempObjectSb.AppendLine("\t\t\tP: \"DefaultAttributeIndex\", \"int\", \"Integer\", \"\",0");

			// ===== Local Transform =========
			Jade_Matrix mtx = gao.Matrix;

			var pos = setToZeroPosition ? UnityEngine.Vector3.zero : mtx.GetPosition(convertAxes: false);
			var rot = setToZeroPosition ? UnityEngine.Vector3.zero : mtx.GetRotation(convertAxes: false).eulerAngles;
			var scl = setToZeroPosition ? UnityEngine.Vector3.one : mtx.GetScale(convertAxes: false);

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Translation\", \"Lcl Translation\", \"\", \"A+\",{0},{1},{2}", pos.x, pos.y, pos.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Rotation\", \"Lcl Rotation\", \"\", \"A+\",{0},{1},{2}", rot.x, rot.y, rot.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendFormat("\t\t\tP: \"Lcl Scaling\", \"Lcl Scaling\", \"\", \"A\",{0},{1},{2}", scl.x, scl.y, scl.z);
			tempObjectSb.AppendLine();

			tempObjectSb.AppendLine("\t\t\tP: \"currentUVSet\", \"KString\", \"\", \"U\", \"map1\"");
			tempObjectSb.AppendLine("\t\t}");
			tempObjectSb.AppendLine("\t\tShading: T");
			tempObjectSb.AppendLine("\t\tCulling: \"CullingOff\"");
			tempObjectSb.AppendLine("\t}");

			// Adds in geometry if it exists, if it it does not exist, this is a empty gameObject file and skips over this
			if (geo != null) {

				// =================================
				//         General Geometry Info
				// =================================
				// Generate the geometry information for the mesh created

				tempObjectSb.AppendLine($"\tGeometry: {geometryId}, \"Geometry::\", \"Mesh\" {{");

				// ===== WRITE THE VERTICES =====
				var vertices = geo.Vertices;
				int vertCount = vertices.Length * 3; // <= because the list of points is just a list of comma seperated values, we need to multiply by three

				tempObjectSb.AppendLine("\t\tVertices: *" + vertCount + " {");
				tempObjectSb.Append("\t\t\ta: ");
				for (int i = 0; i < vertices.Length; i++) {
					if (i > 0) tempObjectSb.Append(",");

					tempObjectSb.AppendFormat("{0},{1},{2}", vertices[i].X, vertices[i].Y, vertices[i].Z);
				}

				tempObjectSb.AppendLine();
				tempObjectSb.AppendLine("\t\t} ");

				// ======= WRITE THE TRIANGLES ========
				int triangleCount = geo.Elements.Sum(e => e.Triangles.Length) * 3;
				tempObjectSb.AppendLine("\t\tPolygonVertexIndex: *" + triangleCount + " {");

				// Write triangle indices
				tempObjectSb.Append("\t\t\ta: ");
				appendComma = false;
				foreach (var e in geo.Elements) {
					foreach (var t in e.Triangles) {
						if (appendComma) tempObjectSb.Append(",");
						appendComma = true;

						tempObjectSb.AppendFormat("{0},{1},{2}",
												  t.Vertex0,
												  t.Vertex1,
												  -t.Vertex2 - 1); // <= Tells the poly is ended

					}
				}

				tempObjectSb.AppendLine();

				tempObjectSb.AppendLine("\t\t} ");
				tempObjectSb.AppendLine("\t\tGeometryVersion: 124");

				bool hasNormals = geo.Normals != null;
				if (hasNormals) {
					// ===== WRITE THE NORMALS ==========
					var normals = geo.Normals;
					tempObjectSb.AppendLine("\t\tLayerElementNormal: 0 {");
					tempObjectSb.AppendLine("\t\t\tVersion: 101");
					tempObjectSb.AppendLine("\t\t\tName: \"\"");
					tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByVertex\"");
					tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"Direct\"");

					tempObjectSb.AppendLine("\t\t\tNormals: *" + (normals.Length * 3) + " {");
					tempObjectSb.Append("\t\t\t\ta: ");

					for (int i = 0; i < normals.Length; i++) {
						if (i > 0) tempObjectSb.Append(",");

						tempObjectSb.AppendFormat("{0},{1},{2},", normals[i].X, normals[i].Y, normals[i].Z);
					}

					tempObjectSb.AppendLine();
					tempObjectSb.AppendLine("\t\t\t}");
					tempObjectSb.AppendLine("\t\t}");
				}

				// ===== WRITE THE COLORS =====
				bool hasColors = geo.Colors != null && geo.Colors.Length == geo.Vertices.Length;

				if (hasColors) {
					var colors = geo.Colors;

					Dictionary<UnityEngine.Color, int> colorTable = new Dictionary<UnityEngine.Color, int>(); // reducing amount of data by only keeping unique colors.
					int idx = 0;
					int[] colorIndices = new int[colors.Length];

					// build index table of all the different colors present in the mesh            
					for (int i = 0; i < colors.Length; i++) {
						var c = colors[i].GetColor();
						if (!colorTable.ContainsKey(c)) {
							colorTable[c] = idx;
							idx++;
						}
						colorIndices[i] = colorTable[c];
					}

					tempObjectSb.AppendLine("\t\tLayerElementColor: 0 {");
					tempObjectSb.AppendLine("\t\t\tVersion: 101");
					tempObjectSb.AppendLine("\t\t\tName: \"Col\"");
					tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByVertex\"");
					tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
					tempObjectSb.AppendLine("\t\t\tColors: *" + colorTable.Count * 4 + " {");
					tempObjectSb.Append("\t\t\t\ta: ");

					bool first = true;
					foreach (KeyValuePair<UnityEngine.Color, int> color in colorTable) {
						if (!first) tempObjectSb.Append(",");

						tempObjectSb.AppendFormat("{0},{1},{2},{3}", color.Key.r, color.Key.g, color.Key.b, 1f);
						first = false;
					}
					tempObjectSb.AppendLine();

					tempObjectSb.AppendLine("\t\t\t\t}");

					// Color index
					tempObjectSb.AppendLine("\t\t\tColorIndex: *" + colors.Length + " {");
					tempObjectSb.Append("\t\t\t\ta: ");

					for (int i = 0; i < colors.Length; i++) {
						if (i > 0) tempObjectSb.Append(",");

						tempObjectSb.AppendFormat("{0}", colorIndices[i]);
					}

					tempObjectSb.AppendLine();

					tempObjectSb.AppendLine("\t\t\t}");
					tempObjectSb.AppendLine("\t\t}");
				}



				// ================ UV CREATION =========================

				// -- UV 1 Creation
				int uvLength = geo.UVs.Length;
				var uvs = geo.UVs;

				tempObjectSb.AppendLine("\t\tLayerElementUV: 0 {"); // the Zero here is for the first UV map
				tempObjectSb.AppendLine("\t\t\tVersion: 101");
				tempObjectSb.AppendLine("\t\t\tName: \"map1\"");
				tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygonVertex\"");
				tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");
				tempObjectSb.AppendLine("\t\t\tUV: *" + uvLength * 2 + " {");
				tempObjectSb.Append("\t\t\t\ta: ");

				for (int i = 0; i < uvLength; i++) {
					if (i > 0) tempObjectSb.Append(",");

					tempObjectSb.AppendFormat("{0},{1}", uvs[i].U, uvs[i].V);
				}
				tempObjectSb.AppendLine();

				tempObjectSb.AppendLine("\t\t\t\t}");

				// UV tile index coords
				tempObjectSb.AppendLine("\t\t\tUVIndex: *" + triangleCount + " {");
				tempObjectSb.Append("\t\t\t\ta: ");

				appendComma = false;
				foreach (var e in geo.Elements) {
					foreach (var t in e.Triangles) {
						if (appendComma) tempObjectSb.Append(",");
						appendComma = true;

						tempObjectSb.AppendFormat("{0},{1},{2}", t.UV0, t.UV1, t.UV2);

					}
				}

				tempObjectSb.AppendLine();

				tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t}");

				// ============ MATERIALS =============

				tempObjectSb.AppendLine("\t\tLayerElementMaterial: 0 {");
				tempObjectSb.AppendLine("\t\t\tVersion: 101");
				tempObjectSb.AppendLine("\t\t\tName: \"\"");
				tempObjectSb.AppendLine("\t\t\tMappingInformationType: \"ByPolygon\"");
				tempObjectSb.AppendLine("\t\t\tReferenceInformationType: \"IndexToDirect\"");

				int totalFaceCount = 0;

				// So by polygon means that we need 1/3rd of how many indicies we wrote.
				int numberOfSubmeshes = (int)geo.ElementsCount;

				StringBuilder submeshesSb = new StringBuilder();

				for (int i = 0; i < geo.Elements.Length; i++) {
					var e = geo.Elements[i];
					foreach (var t in e.Triangles) {
						submeshesSb.AppendFormat("{0},", e.MaterialID);
						totalFaceCount += 1;
					}
				}

				tempObjectSb.AppendLine("\t\t\tMaterials: *" + totalFaceCount + " {");
				tempObjectSb.Append("\t\t\t\ta: ");
				tempObjectSb.AppendLine(submeshesSb.ToString());
				tempObjectSb.AppendLine("\t\t\t} ");
				tempObjectSb.AppendLine("\t\t}");

				// ============= INFORMS WHAT TYPE OF LATER ELEMENTS ARE IN THIS GEOMETRY =================
				tempObjectSb.AppendLine("\t\tLayer: 0 {");
				tempObjectSb.AppendLine("\t\t\tVersion: 100");
				if (hasNormals) {
					tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
					tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementNormal\"");
					tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
					tempObjectSb.AppendLine("\t\t\t}");
				}
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementMaterial\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementTexture\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				if (hasColors) {
					tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
					tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementColor\"");
					tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
					tempObjectSb.AppendLine("\t\t\t}");
				}
				tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
				tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 0");
				tempObjectSb.AppendLine("\t\t\t}");
				// TODO: Here we would add UV layer 1 for ambient occlusion UV file
				//			tempObjectSb.AppendLine("\t\t\tLayerElement:  {");
				//			tempObjectSb.AppendLine("\t\t\t\tType: \"LayerElementUV\"");
				//			tempObjectSb.AppendLine("\t\t\t\tTypedIndex: 1");
				//			tempObjectSb.AppendLine("\t\t\t}");
				tempObjectSb.AppendLine("\t\t}");
				tempObjectSb.AppendLine("\t}");

				// Add the connection for the model to the geometry so it is attached the right mesh
				tempConnectionsSb.AppendLine("\t;Geometry::, Model::" + meshName);
				tempConnectionsSb.AppendLine("\tC: \"OO\"," + geometryId + "," + modelId);
				tempConnectionsSb.AppendLine();

				// Add the connection of all the materials in order of submesh
				var gro_material = gao.Base?.Visual?.Material?.Value?.RenderObject;
				if (gro_material != null) {
					switch (gro_material.Type) {
						case GRO_Type.MAT_MTT:
						case GRO_Type.MAT_SIN:
							// Write only this key
							{
								uint materialID = gro_material.Object.Key.Key;
								string materialName = $"{materialID:X8}";
								tempConnectionsSb.AppendLine($"\t;Material::{materialName}, Model::{meshName}");
								tempConnectionsSb.AppendLine($"\tC: \"OO\",{materialID},{modelId}");
								tempConnectionsSb.AppendLine();
								materials[materialID] = gro_material.Object;
							}
							break;
						case GRO_Type.MAT_MSM:
							var msm = (MAT_MSM_MultiSingleMaterial)gro_material.Value;
							foreach (var matRef in msm.Materials) {
								uint materialID = matRef.Key.Key;
								string materialName = $"{materialID:X8}";
								tempConnectionsSb.AppendLine($"\t;Material::{materialName}, Model::{meshName}");
								tempConnectionsSb.AppendLine($"\tC: \"OO\",{materialID},{modelId}");
								tempConnectionsSb.AppendLine();
								materials[materialID] = matRef.Value;
							}
							// Write all keys
							break;
					}
				} else {
					//UnityEngine.Debug.LogError("ERROR: the game object " + meshName + " has no material");
				}
			}

			// TODO: Export skeleton group (& group?)
			/*
			// Recursively add all the other objects to the string that has been built.
			for (int i = 0; i < gao.transform.childCount; i++) {
				GameObject childObject = gao.transform.GetChild(i).gameObject;

				FBXUnityMeshGetter.GetMeshToString(childObject, materials, ref tempObjectSb, ref tempConnectionsSb, gao, modelId);
			}*/

			objects.Append(tempObjectSb.ToString());
			connections.Append(tempConnectionsSb.ToString());


			return modelId;
		}
		#endregion

		
		public static void AllMaterialsToString(Dictionary<uint, GEO_Object> materials, string outputPath, out string matObjects, out string connections) {
			StringBuilder tempObjectSb = new StringBuilder();
			StringBuilder tempConnectionsSb = new StringBuilder();



			foreach(var matKV in materials) {
				var key = matKV.Key;
				var mat = matKV.Value;

				// We rename the material if it is being copied
				string materialName = $"{key:X8}";

				uint referenceId = key;

				// Header
				tempObjectSb.AppendLine();
				tempObjectSb.AppendLine("\tMaterial: " + referenceId + ", \"Material::" + materialName + "\", \"\" {");
				tempObjectSb.AppendLine("\t\tVersion: 102");
				tempObjectSb.AppendLine("\t\tShadingModel: \"phong\"");
				tempObjectSb.AppendLine("\t\tMultiLayer: 0");
				tempObjectSb.AppendLine("\t\tProperties70:  {");


				// Colors
				Jade_Color diffuse, ambient, specular;
				Jade_TextureReference texture = null;
				switch (mat.RenderObject.Value) {
					case MAT_SIN_SingleMaterial sin:
						ambient = sin.Ambiant;
						diffuse = sin.Diffuse;
						specular = sin.Specular;
						texture = sin.Texture;
						break;
					case MAT_MTT_MultiTextureMaterial mtt:
						ambient = mtt.Ambiant;
						diffuse = mtt.Diffuse;
						specular = mtt.Specular;
						if ((mtt.Levels?.Length ?? 0) > 0) {
							for (int i = 0; i < mtt.Levels.Length; i++) {
								var texRef = mtt.Levels[i].Texture;
								if (!texRef.IsNull) {
									texture = texRef;
									break;
								}
							}
						}
						break;
					default:
						throw new Exception($"Unexpected RenderObject of type {mat.RenderObject.Value.GetType()} being parsed as material!");
				}

				void writeColor(string name, Jade_Color color) {

					tempObjectSb.AppendLine($"\t\t\tP: \"{name}\", \"Vector3D\", \"Vector\", \"\",{color.Red},{color.Green},{color.Blue}");
					tempObjectSb.AppendLine($"\t\t\tP: \"{name}Color\", \"Color\", \"\", \"A\",{color.Red},{color.Green},{color.Blue}");
					// Todo: check this factor
					tempObjectSb.AppendLine($"\t\t\tP: \"{name}Factor\", \"Number\", \"\", \"A\",{color.Alpha}");
				}

				writeColor("Ambient", ambient);
				writeColor("Diffuse", diffuse);
				writeColor("Specular", specular);

				/*if (mat.HasProperty("_Mode")) {
					Color color = Color.white;

					switch ((int)mat.GetFloat("_Mode")) {
						case 0: // Map is opaque

							break;

						case 1: // Map is a cutout
								//  TODO: Add option if it is a cutout
							break;

						case 2: // Map is a fade
							color = mat.GetColor("_Color");

							tempObjectSb.AppendFormat("\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",{0},{1},{2}", FE.FBXFormat(color.r), FE.FBXFormat(color.g), FE.FBXFormat(color.b));
							tempObjectSb.AppendLine();
							tempObjectSb.AppendFormat("\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",{0}", FE.FBXFormat(color.a));
							tempObjectSb.AppendLine();
							break;

						case 3: // Map is transparent
							color = mat.GetColor("_Color");

							tempObjectSb.AppendFormat("\t\t\tP: \"TransparentColor\", \"Color\", \"\", \"A\",{0},{1},{2}", FE.FBXFormat(color.r), FE.FBXFormat(color.g), FE.FBXFormat(color.b));
							tempObjectSb.AppendLine();
							tempObjectSb.AppendFormat("\t\t\tP: \"Opacity\", \"double\", \"Number\", \"\",{0}", FE.FBXFormat(color.a));
							tempObjectSb.AppendLine();
							break;
					}
				}

				// NOTE: Unity doesn't currently import this information (I think) from an FBX file.
				if (mat.HasProperty("_EmissionColor")) {
					Color color = mat.GetColor("_EmissionColor");

					tempObjectSb.AppendFormat("\t\t\tP: \"Emissive\", \"Vector3D\", \"Vector\", \"\",{0},{1},{2}", FE.FBXFormat(color.r), FE.FBXFormat(color.g), FE.FBXFormat(color.b));
					tempObjectSb.AppendLine();

					float averageColor = (color.r + color.g + color.b) / 3f;

					tempObjectSb.AppendFormat("\t\t\tP: \"EmissiveFactor\", \"Number\", \"\", \"A\",{0}", FE.FBXFormat(averageColor));
					tempObjectSb.AppendLine();
				}*/

				// TODO: Add these to the file based on their relation to the PBR files
				//				tempObjectSb.AppendLine("\t\t\tP: \"ShininessExponent\", \"Number\", \"\", \"A\",6.31179285049438");
				//				tempObjectSb.AppendLine("\t\t\tP: \"Shininess\", \"double\", \"Number\", \"\",6.31179285049438");
				//				tempObjectSb.AppendLine("\t\t\tP: \"Reflectivity\", \"double\", \"Number\", \"\",0");

				tempObjectSb.AppendLine("\t\t}");
				tempObjectSb.AppendLine("\t}");

				// Textures

				// TODO: FBX import currently only supports Diffuse Color and Normal Map
				// Because it is undocumented, there is no way to easily find out what other textures
				// can be attached to an FBX file so it is imported into the PBR shaders at the same time.
				// Also NOTE, Unity 5.1.2 will import FBX files with legacy shaders. This is fix done
				// in at least 5.3.4.

				// Serialize first texture only
				if (texture != null) {
					SerializeOneTexture(texture, "DiffuseColor", tempObjectSb, tempConnectionsSb, true, outputPath, mat.Key.Key, materialName);
				}
			}

			matObjects = tempObjectSb.ToString();
			connections = tempConnectionsSb.ToString();
		}


		private static bool SerializeOneTexture(Jade_TextureReference texture,
												string textureType,
												StringBuilder objectsSb,
												StringBuilder connectionsSb,
												bool extractTextures,
												string outputPath,
												uint materialId,
												string materialName) {
			if (texture == null) return false;

			string textureFilePathFullName = $"Textures/{texture.Key:X8}.png";
			var fullPath = Path.Combine(outputPath, textureFilePathFullName);

			if (extractTextures && !File.Exists(fullPath)) {
				var tex = (texture.Content ?? texture.Info).ToTexture2D();
				if (tex == null) tex = GeometryHelpers.CreateDummyTexture();
				Util.ByteArrayToFile(fullPath, tex.EncodeToPNG());
			}

			long textureReference = texture.Key.Key;
			var textureName = $"{texture.Key:X8}";

			objectsSb.AppendLine($"\tTexture: {textureReference}, \"Texture::{materialName}\", \"\" {{");
			objectsSb.AppendLine("\t\tType: \"TextureVideoClip\"");
			objectsSb.AppendLine("\t\tVersion: 202");
			objectsSb.AppendLine($"\t\tTextureName: \"Texture::{materialName}\"");
			objectsSb.AppendLine("\t\tProperties70:  {");
			objectsSb.AppendLine("\t\t\tP: \"CurrentTextureBlendMode\", \"enum\", \"\", \"\",0");
			objectsSb.AppendLine("\t\t\tP: \"UVSet\", \"KString\", \"\", \"\", \"map1\"");
			objectsSb.AppendLine("\t\t\tP: \"UseMaterial\", \"bool\", \"\", \"\",1");
			objectsSb.AppendLine("\t\t}");
			objectsSb.AppendLine($"\t\tMedia: \"Video::{materialName}\"");

			// Sets the absolute path for the copied texture
			objectsSb.AppendLine($"\t\tFileName: \"/{textureFilePathFullName}\"");

			// Sets the relative path for the copied texture
			objectsSb.AppendLine($"\t\tRelativeFilename: \"/{textureFilePathFullName}\"");

			objectsSb.AppendLine("\t\tModelUVTranslation: 0,0"); // TODO: Figure out how to get the UV translation into here
			objectsSb.AppendLine("\t\tModelUVScaling: 1,1"); // TODO: Figure out how to get the UV scaling into here
			objectsSb.AppendLine("\t\tTexture_Alpha_Source: \"None\""); // TODO: Add alpha source here if the file is a cutout.
			objectsSb.AppendLine("\t\tCropping: 0,0,0,0");
			objectsSb.AppendLine("\t}");

			connectionsSb.AppendLine($"\t;Texture::{textureName}, Material::{materialName}");
			connectionsSb.AppendLine($"\tC: \"OP\",{textureReference},{materialId}, \"{textureType}\"");

			connectionsSb.AppendLine();

			return true;
		}
	}
}
