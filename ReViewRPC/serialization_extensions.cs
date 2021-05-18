using Lemniscate;
using ReView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReViewRPC
{
	public static class Matrix_Serialization_Extensions
	{
		public static void Serialize(this Linear_Serializer serializer, Matrix4x4 value)
		{
			double[] values = new double[16];
			for (int i = 0; i < 16; i++)
				values[i] = value[i];
			serializer.Serialize(values);
		}

		public static void Deserialize(this Linear_Serializer serializer, out Matrix4x4 value)
		{
			double[] values;
			serializer.Deserialize(out values);
			value = new Matrix4x4();
			for (int i = 0; i < 16; i++)
				value[i] = values[i];
		}

		public static void Serialize(this Linear_Serializer serializer, Vector3 value)
		{
			double[] values = { value.x, value.y, value.z };
			serializer.Serialize(values);
		}

		public static void Deserialize(this Linear_Serializer serializer, out Vector3 value)
		{
			double[] values;
			serializer.Deserialize(out values);
			value = new Vector3(values[0], values[1], values[2]);
		}

		public static void Serialize(this Linear_Serializer serializer, Color32 value)
		{
			byte[] values = { value.R, value.G, value.B, value.A };
			serializer.Serialize(value.components);
		}

		public static void Deserialize(this Linear_Serializer serializer, out Color32 value)
		{
			byte[] values;
			serializer.Deserialize(out values);
			value = new Color32(values[0], values[1], values[2], values[3]);
		}
	}
}
