using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Windows.Forms.Integration;

namespace ALT.CVL.Interfaces
{
    public interface IConfigParam
    {
        string RecipePath { get; set; }
        string NGImgPath { get; set; }
        int CamCount { get; }
        ObservableCollection<MdCameraData> OCCameraData { get; set; }
        [JsonIgnore]
        ObservableCollection<WindowsFormsHost> OCWfhDisplay { get; }
    }
}
