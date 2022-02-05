#include "pch.h"

#include "SICK_Calib_CLR.h"



namespace SICKCalibCLR
{
	void SICK_Calibrator::ApplyCalibrationToImage(GenIStream::IFrame^ frame)
	{
		void* inputCoRectified[2] = { (void *)(frame->GetReflectance()->GetData()), nullptr };
		

		if (scatterEnabled)
		{
			GenIStream::IComponent^ scatter = frame->GetRegion(GenIStream::RegionId::REGION_1)->GetComponent(GenIStream::ComponentId::SCATTER);

			inputCoRectified[1] = (void *)scatter->GetData();
		}
			
		applyCalibration(*filterHandle, (uint8_t *)frame->GetRange()->GetData().ToPointer(), X, Z);
		rectify(*rectificationFilterHandle, X, Z, inputCoRectified, rangeOut, rectifiedImages2D);

		float sum = 0;
		int count = 0;
		float missingDataValue = 0.0;

		for (int pixelIndex = 0; pixelIndex < dataSize; pixelIndex++)
		{
			if (rangeOut[pixelIndex] != missingDataValue)
			{
				sum += rangeOut[pixelIndex];
				count += 1;
			}
		}
	}

	SICK_Calibrator::SICK_Calibrator() {};

	void SICK_Calibrator::LoadCalibrationFile(String ^fileName, GenIStream::ICamera^ camera)
	{
		destroyAllRectificationFilters();
		destroyAllRectificationFilters();
		destroyAllModels();



		int length = fileName->Length;
		char* buffer = new char[length];
		for (int i = 0; i < fileName->Length; i++)
		{
			buffer[i] = fileName->default[i];
		}


		ModelHandle model;
		createModelFromFilePath(buffer, &model);
		
		genistream::sheetoflight::ImageSize imageSize = ImageSizeFromCamera(camera);
		SensorTraits sensorTraits = SensorTraitsFromCamera(camera);
		
		


		WorldRangeTraits worldRangeTraits;
		SplatSettings splatSettings;
		dataSize = imageSize.width * imageSize.lineCount;

		GenIStream::RegionParameters^ extractionRegion = camera->GetCameraParameters()->Region(GenIStream::RegionId::SCAN_3D_EXTRACTION_1);
		int scatterdEnabled = extractionRegion->Component(GenIStream::ComponentId::SCATTER)->ComponentEnable->Get();

		PixelFormat format2D[2];
		format2D[0] = MONO_8;
		PixelFormat scatteredFormat;

		if (scatterdEnabled)
		{
			scatteredFormat = extractionRegion->Component(GenIStream::ComponentId::SCATTER)->PixelFormat->Get().ToString()->Equals("MONO_16") ?
				PixelFormat::MONO_16 : PixelFormat::MONO_8;

			format2D[1] = scatteredFormat;
		}


		unsigned int nCoRectified = scatterdEnabled ? 2 : 1;
		CoRectifiedDescription coRectifiedDescription = CoRectifiedDescription{ nCoRectified,format2D };

		calculateCalibrationBounds(model, sensorTraits, imageSize, &worldRangeTraits, &splatSettings, 0);

		
		createCalibrationFilter(model, sensorTraits, imageSize, PFNC_COORD_3D_C16, filterHandle);
		

		float missingDataValue = 0.0;

		

		createRectificationFilter(splatSettings, MEAN, worldRangeTraits, imageSize, coRectifiedDescription, missingDataValue, rectificationFilterHandle);

		rangeOut = new float[dataSize];
		vector<unsigned char> reflectanceOut(dataSize);
		vector<unsigned short> scatterOutUint16;
		vector<unsigned char> scatterOutUint8;
		rectifiedImages2D = new void* [2];
		rectifiedImages2D[0] = reflectanceOut.data();
		
		if (scatterdEnabled && scatteredFormat == PixelFormat::MONO_16)
		{
			scatterOutUint16 = vector<unsigned short>(dataSize);
			rectifiedImages2D[0] = scatterOutUint16.data();
		}
		else if (scatterdEnabled && scatteredFormat == PixelFormat::MONO_8)
		{
			scatterOutUint8 = std::vector<unsigned char>(dataSize);
			rectifiedImages2D[1] = scatterOutUint8.data();
		}


		
	}

	ImageSize SICK_Calibrator::ImageSizeFromCamera(Sick::GenIStream::ICamera^ camera)
	{
		GenIStream::CameraParameters^ camParams = camera->GetCameraParameters();

		uint16_t aoiWidth = static_cast<uint16_t>(camParams->Region(GenIStream::RegionId::REGION_1)->Width->Get());
		uint16_t numOfLines = static_cast<uint16_t>(camParams->Region(GenIStream::RegionId::SCAN_3D_EXTRACTION_1)->Width->Get());

		return genistream::sheetoflight::ImageSize{ aoiWidth, numOfLines };
	}

	SensorTraits SICK_Calibrator::SensorTraitsFromCamera(GenIStream::ICamera^ camera)
	{
		SensorTraits sensorTraits;
		GenIStream::CameraParameters^ camParams = camera->GetCameraParameters();
		sensorTraits.aoiHeight = static_cast<uint16_t>(camParams->Region(GenIStream::RegionId::REGION_1)->Width->Get());
		sensorTraits.aoiHeight = static_cast<uint16_t>(camParams->Region(GenIStream::RegionId::REGION_1)->Height->Get());
		sensorTraits.xT.origin = static_cast<double>(camParams->Region(GenIStream::RegionId::REGION_1)->OffsetX->Get());
		sensorTraits.xT.scale = 1.0;

		double offsetZ = static_cast<double>(camParams->Region(GenIStream::RegionId::REGION_1)->OffsetY->Get());
		const double ranger3SubPixeling = 1.0 / 16.0;
		bool rangeAxisReversed = camParams->Scan3dExtraction(GenIStream::Scan3dExtractionId::SCAN_3D_EXTRACTION_1)->RangeAxis->Get().Equals(GenIStream::RangeAxis::REVERSE);
		sensorTraits.zT.scale = rangeAxisReversed ? -ranger3SubPixeling : ranger3SubPixeling;
		sensorTraits.zT.origin = rangeAxisReversed ? offsetZ + sensorTraits.aoiHeight : offsetZ;

		return sensorTraits;
	}
}
