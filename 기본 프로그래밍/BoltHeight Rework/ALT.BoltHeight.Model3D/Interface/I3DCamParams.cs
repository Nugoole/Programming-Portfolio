namespace ALT.DSCamera.Interface
{
    public interface I3DCamParams : IAppliable<I3DCamParams>
    {

        double Exposure { get; set; }

        double Sensitivity { get; set; }

        string LaserMode { get; set; }

        int LaserPower { get; set; }

        int ScanLength { get; set; }

        double ZDetectionBase { get; set; }

        double ZDetectionHeight { get; set; }

        string AcquireDirection { get; set; }

        string EncoderDirection { get; set; }

        bool UseEncoder { get; set; }

        bool UseHardwareTrigger { get; set; }

        bool HighDynamicRange { get; set; }

        double XScale { get; set; }

        int EncoderRes { get; set; }

        double StepsPerLine { get; set; }

        double DistancePerCycle { get; set; }

        double FirstPixelLocation { get; set; }

        double MotionSpeed { get; set; }
    }
}
