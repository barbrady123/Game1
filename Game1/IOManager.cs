using System.IO;
using Newtonsoft.Json;

namespace Game1
{
	public static class IOManager
	{
		public static T ObjectFromFile<T>(string filePath)
		{
			using (var reader = new StreamReader($"{filePath}.json"))
			{
				return (T)JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
			}
		}

		public static void ObjectToFile(string filePath, object obj)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			using (var writer = new StreamWriter($"{filePath}.json"))
			{
				writer.Write(JsonConvert.SerializeObject(obj));
			}
		}
	}
}
