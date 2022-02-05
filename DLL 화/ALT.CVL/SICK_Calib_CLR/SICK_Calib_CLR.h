




using namespace std;
using namespace System;
using namespace Sick;
using namespace genistream::sheetoflight;

namespace SICKCalibCLR
{
	/*public struct Transform
	{
		Transform(double scale, double origin)
		{
			this->scale = scale;
			this->origin = origin;
		}

	public:
		double scale;
		double origin;

		explicit operator genistream::sheetoflight::Transform() {
			genistream::sheetoflight::Transform buffer = genistream::sheetoflight::Transform();
			buffer.scale = this->scale;
			buffer.origin = this->origin;
			return buffer;
		}
	};
	public struct SensorTraits
	{
	public:
		Transform xT;
		Transform zT;
		unsigned short aoiWidth;
		unsigned short aoiHeight;

		explicit operator genistream::sheetoflight::SensorTraits()
		{
			genistream::sheetoflight::SensorTraits buffer = genistream::sheetoflight::SensorTraits();
			buffer.xT = (genistream::sheetoflight::Transform)this->xT;
			buffer.zT = (genistream::sheetoflight::Transform)this->zT;
			buffer.aoiWidth = this->aoiWidth;
			buffer.aoiHeight = this->aoiHeight;

			return buffer;
		}
	};

	public struct ImageSize
	{
	public:
		unsigned short width;
		unsigned short lineCount;

		explicit operator genistream::sheetoflight::ImageSize()
		{
			genistream::sheetoflight::ImageSize buffer = genistream::sheetoflight::ImageSize();
			buffer.width = this->width;
			buffer.lineCount = this->lineCount;

			return buffer;
		}
	};*/

	public ref class SICK_Calibrator
	{
	
	private:
		static CalibrationFilterHandle* filterHandle;
		static RectificationFilterHandle* rectificationFilterHandle;
		
		static bool scatterEnabled;
		static float* rangeOut;
		static float* X;
		static float* Z;
		static void** rectifiedImages2D;
		static int dataSize;
		static SensorTraits SensorTraitsFromCamera(GenIStream::ICamera^ camera);
		static ImageSize ImageSizeFromCamera(Sick::GenIStream::ICamera^ camera);
		SICK_Calibrator();

	public:
		static void LoadCalibrationFile(System::String^ fileName, GenIStream::ICamera^ camera);
		static void ApplyCalibrationToImage(GenIStream::IFrame^ frame);
		


	};
}
