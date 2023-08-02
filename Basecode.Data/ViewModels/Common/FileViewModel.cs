namespace Basecode.Data.ViewModels.Common;

public class FileViewModel
{
    private string _fileName;
    public string FileAbsolutePath { get; private set; }
    public string FilePath { get; set; }
    public string NonRelativePath { get; set; }
    public string LocalFileName { get; set; }

    public string FileName
    {
        get => _fileName;

        set
        {
            _fileName = value;

            FileAbsolutePath = string.Concat(NonRelativePath, _fileName);
        }
    }
}