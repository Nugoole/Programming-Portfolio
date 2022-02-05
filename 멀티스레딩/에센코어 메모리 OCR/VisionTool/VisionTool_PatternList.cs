using ALT.Serialize;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    [Serializable]
    public class PatternTool : SerializableObjectBase
    {
        public CogPMAlignTool PMTool { get; set; }
        public bool Enable { get; set; }

        public PatternTool(CogPMAlignTool tool, bool enable)
        {
            PMTool = tool;
            Enable = enable;
        }

        private PatternTool(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }
    }

    public partial class VisionTool
    {
        private ObservableCollection<PatternTool> patternList;

        private void InitializePatternList()
        {
            patternList = new ObservableCollection<PatternTool>();
        }


        public void AddPattern()
        {
            var tool = new CogPMAlignTool
            {
                Name = "newPMTool" + patternList.Where(x => x.PMTool.Name.Contains("newPMTool")).Count()
            };

            tool.SearchRegion = CreateInteractiveRectangleAffine("PMTool SearchRegion");
            tool.Changed += VisionTool_Changed;
            tool.Pattern.TrainRegion = CreateInteractiveRectangleAffine("PMTool TrainRegion");

            patternList.Add(new PatternTool(tool, true));
        }

        private void VisionTool_Changed(object sender, CogChangedEventArgs e)
        {
            if((e.StateFlags & CogPMAlignTool.SfName) != 0)
            {
                var tool = sender as CogPMAlignTool;

                if (patternList.Select(x => x.PMTool.Name).Contains(tool.Name))
                    tool.Name = "already existing name";
            }
        }

        public void RemovePattern(CogPMAlignTool tool)
        {
            var value = patternList.Where(x => x.PMTool.Equals(tool)).FirstOrDefault();
            patternList.Remove(value);
        }

        public void Run_PatternList(ICogImage inputImage)
        {
            List<Task> taskList = new List<Task>();

            foreach (var toolPair in PatternList)
            {
                if (toolPair.PMTool.Pattern.Trained)
                {
                    toolPair.PMTool.InputImage = inputImage;

                    taskList.Add(new Task(() =>
                    {
                        if(toolPair.Enable)
                            toolPair.PMTool.Run();
                    }));
                }
            }

            Task[] tasks = taskList.ToArray();
            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);
        }
    }
}
