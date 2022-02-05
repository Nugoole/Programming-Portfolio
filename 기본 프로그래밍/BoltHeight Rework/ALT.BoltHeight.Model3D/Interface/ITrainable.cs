using Cognex.VisionPro;

namespace ALT.DSCamera.Interface
{
    interface ITrainable
    {
        bool Trained { get; set; }

        ICogImage TrainImage { get; set; }
        ICogRegion TrainRegion { get; set; }

        void Train();
        void UnTrain();
    }
}
