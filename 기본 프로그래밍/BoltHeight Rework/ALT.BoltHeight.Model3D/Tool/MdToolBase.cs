using ALT.DSCamera.Interface;
using Cognex.VisionPro;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ALT.DSCamera.Tool
{
    public abstract class MdToolBase : I3DTool, INotifyPropertyChanged
    {
        #region PropertyChangedConfig
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public ICogImage InputImage { get; set; }
        public ICogRecords LastRunRecords { get; set; }

        public virtual CogGraphicCollection GetGraphicFromRecord(ICogRecord record)
        {
            var collection = new CogGraphicCollection();

            if (record.Content is ICogGraphic graphic)
            {
                collection.Add(graphic);
            }
            else if (record.Content is CogGraphicCollection graphics)
            {
                foreach (ICogGraphic item in graphics)
                {
                    collection.Add(item);
                }
            }

            if (record.SubRecords.Count > 0)
                foreach (ICogRecord subrecord in record.SubRecords)
                {
                    foreach (ICogGraphic innerGraphic in GetGraphicFromRecord(subrecord))
                    {
                        collection.Add(innerGraphic);
                    }
                }

            GC.Collect();

            return collection;
        }

        public virtual void Run()
        {
            (this as I3DTool).Run();
        }

    }
}
