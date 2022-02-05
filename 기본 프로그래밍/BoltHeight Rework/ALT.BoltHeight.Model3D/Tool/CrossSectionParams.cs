using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro3D;

using System;
using System.Linq;

namespace ALT.DSCamera.Tool
{
    internal class CrossSectionParams : I3DCrossSectionParams
    {
        private string selectedOperatorName;

        internal Cog3DRangeImageCrossSectionOperatorsParams Operators = new Cog3DRangeImageCrossSectionOperatorsParams();
        internal event Action<Cog3DRangeImageCrossSectionProfile, Cog3DRangeImageCrossSectionOperatorsParams> OnOperatorRan;


        internal string SelectedOperatorName
        {
            get => selectedOperatorName; set
            {
                if (string.IsNullOrEmpty(value))
                    return;

                selectedOperatorName = value;
                var list = Operators.Where(x => x.Name.Equals(value));
                Cog3DRangeImageCrossSectionOperatorBase op = null;

                if (list.Count() > 0)
                {
                    op = list.First();

                    if (op.Regions?.Count > 0)
                        (op.Regions[0] as ICogGraphicInteractive).Selected = true;
                }

            }
        }

        public Cog3DRangeImageCrossSectionProfile Profile { get; set; }

        internal CrossSectionParams()
        {
            Operators.InsertingItem += Operators_InsertingItem;
            Operators.RemovingItem += Operators_RemovingItem;
            Operators.Clearing += Operators_Clearing;
        }

        private void Operators_Clearing(object sender, EventArgs e)
        {
            foreach (Cog3DRangeImageCrossSectionOperatorBase item in Operators)
            {
                if (item is Cog3DRangeImageCrossSectionDistancePointLine)
                {
                    item.Changed -= DistancePointLineChanged;
                    continue;
                }

                (item.Regions[0] as CogRectangleAffine).DraggingStopped -= CrossSectionParams_DraggingStopped;
            }
        }

        private void Operators_RemovingItem(object sender, CogCollectionRemoveEventArgs e)
        {
            if (e.Value is Cog3DRangeImageCrossSectionDistancePointLine op)
            {
                op.Changed -= DistancePointLineChanged;
                return;
            }

            ((e.Value as Cog3DRangeImageCrossSectionOperatorBase).Regions[0] as CogRectangleAffine).DraggingStopped -= CrossSectionParams_DraggingStopped;
        }

        private void Operators_InsertingItem(object sender, CogCollectionInsertEventArgs e)
        {
            var item = e.Value as Cog3DRangeImageCrossSectionOperatorBase;

            if (e.Value is Cog3DRangeImageCrossSectionDistancePointLine op)
            {
                op.Changed += DistancePointLineChanged;
                selectedOperatorName = op.Name;
                return;
            }

            if (e.Value is Cog3DRangeImageCrossSectionExtractPoint extractPoint)
            {
                extractPoint.Name += Operators.Where(x => x.Name.Contains("Point")).Count().ToString();
            }
            else if (e.Value is Cog3DRangeImageCrossSectionExtractLineSegment extractLine)
            {
                extractLine.Name += Operators.Where(x => x.Name.Contains("Line")).Count().ToString();
            }

            (item.Regions[0] as CogRectangleAffine).TipText = item.Name;
            (item.Regions[0] as CogRectangleAffine).Interactive = true;
            (item.Regions[0] as CogRectangleAffine).GraphicDOFEnable = CogRectangleAffineDOFConstants.All;

            selectedOperatorName = item.Name;
            (item.Regions[0] as CogRectangleAffine).DraggingStopped += CrossSectionParams_DraggingStopped;
        }

        //private void Operators_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
        //    {
        //        foreach (Cog3DRangeImageCrossSectionOperatorBase item in e.NewItems)
        //        {
        //            if (item is Cog3DRangeImageCrossSectionDistancePointLine)
        //            {
        //                item.Changed += DistancePointLineChanged;
        //                selectedOperatorName = item.Name;
        //                continue;
        //            }

        //            if (item is Cog3DRangeImageCrossSectionExtractPoint extractPoint)
        //            {
        //                extractPoint.Name += Operators.Where(x => x.Name.Contains("Point")).Count().ToString();
        //            }
        //            else if (item is Cog3DRangeImageCrossSectionExtractLineSegment extractLine)
        //            {
        //                extractLine.Name += Operators.Where(x => x.Name.Contains("Line")).Count().ToString();
        //            }

        //            (item.Regions[0] as CogRectangleAffine).TipText = item.Name;
        //            selectedOperatorName = item.Name;
        //            (item.Regions[0] as CogRectangleAffine).DraggingStopped += CrossSectionParams_DraggingStopped;
        //        }
        //    }
        //    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
        //    {
        //        foreach (Cog3DRangeImageCrossSectionOperatorBase item in e.NewItems)
        //        {
        //            if (item is Cog3DRangeImageCrossSectionDistancePointLine)
        //            {
        //                item.Changed -= DistancePointLineChanged;
        //                continue;
        //            }

        //            (item.Regions[0] as CogRectangleAffine).DraggingStopped -= CrossSectionParams_DraggingStopped;
        //        }
        //    }
        //    else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
        //    {
        //        foreach (Cog3DRangeImageCrossSectionOperatorBase item in e.NewItems)
        //        {
        //            if (item is Cog3DRangeImageCrossSectionDistancePointLine)
        //            {
        //                item.Changed -= DistancePointLineChanged;
        //                continue;
        //            }

        //            (item.Regions[0] as CogRectangleAffine).DraggingStopped -= CrossSectionParams_DraggingStopped;
        //        }
        //    }
        //}

        private void DistancePointLineChanged(object sender, CogChangedEventArgs e)
        {
            if (((e.StateFlags & Cog3DRangeImageCrossSectionDistancePointLine.SfLineSegment) | (e.StateFlags & Cog3DRangeImageCrossSectionDistancePointLine.SfPoint)) > 0)
                if (Profile != null)
                    OnOperatorRan?.Invoke(Profile, Operators);
        }

        private void CrossSectionParams_DraggingStopped(object sender, CogDraggingEventArgs e)
        {

            var currentOperator = Operators.Where(x => x.Name.Equals(e.DragGraphic.TipText)).First();

            if (currentOperator != null)
            {
                //currentOperator.Execute(Profile);

                e.DragGraphic.DraggingStopped += CrossSectionParams_DraggingStopped;

                currentOperator.Regions.Insert(0, e.DragGraphic as ICogRegion);
                currentOperator.Regions.RemoveAt(1);
                OnOperatorRan?.Invoke(Profile, Operators);
            }
        }
    }
}
