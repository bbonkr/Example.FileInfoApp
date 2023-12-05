using System.Text.Json.Serialization;

namespace Example.FileInfoApp;

public class FileInformationModel
{
    public string Name { get; set; } = string.Empty;

    [JsonIgnore]
    public long SizeValue { get; set; }

    public string Size
    {
        get
        {
            double sizeTemp = SizeValue;
            int index = 0;
            if (sizeTemp > 1024)
            {
                do
                {
                    sizeTemp = sizeTemp * 1.0 / 1024;
                    index += 1;
                }
                while (sizeTemp > 1024);

            }

            return $"{sizeTemp:f2} {Units[index]}";
        }
    }

    public string Length
    {
        get => $"{SizeValue:n0} {Units[0]}";
    }

    public string ContentType { get; set; } = string.Empty;

    private readonly string[] Units = new string[] {
        "bytes",
        "KB",
        "MB",
        "GB",
        "TB"
    };
}
