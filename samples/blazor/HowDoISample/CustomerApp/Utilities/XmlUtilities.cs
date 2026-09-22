using System.Xml.Serialization;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides lightweight XML serialization helpers for saving and loading map-related objects.
    /// </summary>
    /// <remarks>
    /// This utility wraps <see cref="XmlSerializer"/> to centralize file-based XML persistence behavior.
    /// </remarks>
    public static class XmlUtilities
    {
        /// <summary>
        /// Serializes an object instance to an XML file.
        /// </summary>
        /// <typeparam name="T">The object type to serialize.</typeparam>
        /// <param name="obj">The object instance to write to disk.</param>
        /// <param name="filePath">The destination XML file path.</param>
        /// <remarks>
        /// Creates the destination directory when needed before writing the file.
        /// </remarks>
        public static void SaveToXml<T>(T obj, string filePath)
        {
            try
            {
                // Ensure the directory exists before writing the file
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Create an XmlSerializer for the specified type
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                // Use a StreamWriter to write the serialized XML to the file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    // Serialize the object and write it to the file
                    serializer.Serialize(writer, obj);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Error saving to XML: {ex.Message}");
            }
        }

        /// <summary>
        /// Deserializes an object instance from an XML file.
        /// </summary>
        /// <typeparam name="T">The object type to deserialize.</typeparam>
        /// <param name="filePath">The source XML file path.</param>
        /// <returns>The deserialized object instance, or <see langword="default"/> when deserialization fails.</returns>
        public static T? LoadFromXml<T>(string filePath)
        {
            try
            {
                // Create an XmlSerializer for the specified type
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                // Use a StreamReader to read the XML from the file
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Deserialize the object from the XML file and return it
                    return (T?)serializer.Deserialize(reader);
                }
            }
            catch 
            {
                // Return default value of T when deserialization fails (e.g., file not found, invalid XML)
                return default;
            }
        }
    }
}
